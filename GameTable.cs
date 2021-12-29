using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Platformer
{
    public enum Direction
    {
        None = -1,
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
    public class Sector
    {
        private GameTable Owner;
        private Point TblPosition;
        public Dictionary<Direction, Sector> NeighborSector => new Dictionary<Direction, Sector>
        {
            { Direction.Up, TblPosition.X > 0 ? Owner[TblPosition + new Size(-1, 0)] : null },
            { Direction.Right, TblPosition.Y < Owner.TblSize.Height - 1 ? Owner[TblPosition + new Size(0, 1)] : null },
            { Direction.Down, TblPosition.X < Owner.TblSize.Width - 1 ? Owner[TblPosition + new Size(1, 0)] : null },
            { Direction.Left, TblPosition.Y > 0 ? Owner[TblPosition + new Size(0, -1)] : null }
        };
        protected bool RightWall;
        protected bool DownWall;
        protected Direction VoidPassage;
        public Dictionary<Direction, bool> CanGoWithoutDoors => new Dictionary<Direction, bool>
        {
            { Direction.Up, !NeighborSector[Direction.Up]?.DownWall ?? VoidPassage == Direction.Up },
            { Direction.Right, !RightWall },
            { Direction.Down, !DownWall },
            { Direction.Left, !NeighborSector[Direction.Left]?.RightWall ?? VoidPassage == Direction.Left }
        };

        protected Dictionary<Direction, (Door door, bool blockState)> Doors;
        public void AddDoor(Door door)
        {
            if (!Doors.ContainsKey(door.Wall))
            {
                var succes = NeighborSector[door.OpenDirection]?.AddDoorInfo(door);
                if (succes == false)
                    return;
                Doors.Add(door.Wall, (door, !door.OpenState));
            }
        }
        private bool AddDoorInfo(Door door)
        {
            if (!Doors.ContainsKey(door.Wall))
            {
                Doors.Add(door.Wall, (door, door.OpenState));
                return true;
            }
            else
                return false;
        }
        public void DoorActivate(Direction doorWall, bool animated)
        {
            if (Doors.ContainsKey(doorWall))
                Doors[doorWall].door.Activate(animated);
        }
        public (Door door, bool blockState)[] GetDoors => Doors.Select(D => D.Value).ToArray();
        private bool DoorState(Direction direction)
            => !Doors.ContainsKey(direction) || Doors[direction].door.OpenState == Doors[direction].blockState;
        public Dictionary<Direction, bool> CanGo => new Dictionary<Direction, bool>
        {
            { Direction.Up, CanGoWithoutDoors[Direction.Up] && DoorState(Direction.Up) && (NeighborSector[Direction.Up]?.DoorState(Direction.Down) ?? true) },
            { Direction.Right, CanGoWithoutDoors[Direction.Right] && DoorState(Direction.Right) && (NeighborSector[Direction.Right]?.DoorState(Direction.Left) ?? true) },
            { Direction.Down, CanGoWithoutDoors[Direction.Down] && DoorState(Direction.Down) && (NeighborSector[Direction.Down]?.DoorState(Direction.Up) ?? true) },
            { Direction.Left, CanGoWithoutDoors[Direction.Left] && DoorState(Direction.Left) && (NeighborSector[Direction.Left]?.DoorState(Direction.Right) ?? true) }
        };
        public Dictionary<Direction, RectangleF> WallBounds => new Dictionary<Direction, RectangleF>
        {
            { Direction.Up, CanGo[Direction.Up] ? RectangleF.Empty : new RectangleF(ImgLocation.X, ImgLocation.Y, Scale, Scale / 10) },
            { Direction.Right, CanGo[Direction.Right] ? RectangleF.Empty : new RectangleF(ImgLocation.X + Scale * 9 / 10, ImgLocation.Y, Scale / 10, Scale) },
            { Direction.Down, CanGo[Direction.Down] ? RectangleF.Empty : new RectangleF(ImgLocation.X, ImgLocation.Y + Scale * 9 / 10, Scale, Scale / 10) },
            { Direction.Left, CanGo[Direction.Left] ? RectangleF.Empty : new RectangleF(ImgLocation.X, ImgLocation.Y, Scale / 10, Scale) }
        };
        public Dictionary<Direction, (RectangleF left, RectangleF right)> CornerBounds => new Dictionary<Direction, (RectangleF left, RectangleF right)>
        {
            { Direction.Up, (new RectangleF(ImgLocation.X, ImgLocation.Y, Scale / 10, Scale / 10),
                             new RectangleF(ImgLocation.X + Scale * 9 / 10, ImgLocation.Y, Scale / 10, Scale / 10)) },
            { Direction.Down, (new RectangleF(ImgLocation.X, ImgLocation.Y + Scale * 9 / 10, Scale / 10, Scale / 10),
                               new RectangleF(ImgLocation.X + Scale * 9 / 10, ImgLocation.Y + Scale * 9 / 10, Scale / 10, Scale / 10)) }
        };

        public Button Button { get; protected set; }
        public void AddButton(Button button)
        {
            if (Button == null)
            {
                Button = button;
                Owner.UpdateEvent += (sender, e) =>
                {
                    Rectangle rect = Rectangle.Round(Button.Bounds);
                    var buttonRect = new RectangleF(ImgLocation.X + rect.X, ImgLocation.Y + rect.Y, rect.Width, rect.Height);
                    if (Owner.Player.Intersect(buttonRect))
                        Button.Activate();
                };
            }
        }

        private PointF ImgLocation => new PointF(TblPosition.Y * Scale, TblPosition.X * Scale);
        private SizeF Size => Owner.SectorScale;
        public float Scale => Owner.SectorScaleValue;
        public RectangleF Bounds => new RectangleF(ImgLocation, Size);
        public Image Background { get; private set; }

        public void PaintBackground()
        {
            foreach (Door door in GetDoors.Select(D => D.door))
                door.Render(Scale, Owner.PParametrs.DoorOpenSpeed);
            if (Button != null)
                Button.Render(Scale);
            (int W, int D) = GetWallDir();
            int ImgSectorSize = 200;
            Size ImgSize = new Size(ImgSectorSize, ImgSectorSize);
            Background = Images.GetFragment(Properties.Resources.Standart, ImgSize, new RectangleF(new PointF(D * ImgSectorSize, (4 - W) * ImgSectorSize), ImgSize), System.Drawing.Size.Truncate(Size));
        }

        public Sector(GameTable table, Point tblPosition, bool RW, bool DW, Direction voidPassage = Direction.None)
        {
            Owner = table;
            TblPosition = tblPosition;
            RightWall = RW;
            DownWall = DW;
            VoidPassage = voidPassage;
            Doors = new Dictionary<Direction, (Door door, bool blockState)>();
            Button = null;
            PaintBackground();
        }
        private (int W, int D) GetWallDir()
        {
            return string.Join("", CanGoWithoutDoors.Select(b => b.Value ? 1 : 0)) switch
            {
                "1111" => (0, 0),
                "1101" => (1, 0),
                "1110" => (1, 1),
                "0111" => (1, 2),
                "1011" => (1, 3),
                "1100" => (2, 0),
                "0110" => (2, 1),
                "0011" => (2, 2),
                "1001" => (2, 3),
                "1010" => (2, 4),
                "0101" => (2, 5),
                "1000" => (3, 0),
                "0100" => (3, 1),
                "0010" => (3, 2),
                "0001" => (3, 3),
                "0000" => (4, 0),
                _ => throw new Exception()
            };
        }
    }
    public class GameTable
    {
        public PhisicParametrs PParametrs;
        public BoundsParametrs BParametrs;

        protected Sector[,] Sectors;
        protected Sector this[int X, int Y] => X < TblSize.Width && Y < TblSize.Height &&
                                       X >= 0 && Y >= 0 ?
                                       Sectors[X, Y] : null;
        public Sector this[Point p] => this[p.X, p.Y];
        public Size TblSize => new Size(Sectors.GetLength(0), Sectors.GetLength(1));
        public Size ImgSize { get; protected set; }
        public float SectorScaleValue { get; protected set; }
        public SizeF SectorScale => new SizeF(SectorScaleValue, SectorScaleValue);
        public Door[] GetAllDoors()
        {
            return Sectors.Cast<Sector>().SelectMany(S => S.GetDoors).Select(D => D.door).GroupBy(D => D.ID).Select(g => g.First()).ToArray();
        }

        private bool started = false;
        public bool Started
        {
            get => started;
            private set
            {
                started = value;
                PaintUpdate?.Change(started ? 0 : -1, 100);
                PhisicUpdate?.Change(started ? 0 : -1, 100);
            }
        }

        public Guid Level { get; }

        public Player Player;

        protected (Point position, Direction direction)? StartSector;
        protected (Point position, Direction direction)? FinishSector;

        protected GameTable() { }
        public GameTable(Level level, Player.PlayerArgs player, Size formSize, IScreen box)
        {
            Level = level.GUID;
            Sectors = new Sector[level.Size.Width, level.Size.Height];
            SectorScaleValue = GetSectorScale(formSize);
            PParametrs = new PhisicParametrs(SectorScaleValue);
            BParametrs = new BoundsParametrs(SectorScaleValue);
            ImgSize = Size.Truncate(new SizeF(TblSize.Height * SectorScaleValue, TblSize.Width * SectorScaleValue));
            box.UpdateImage(new Bitmap(ImgSize.Width, ImgSize.Height));
            StartSector = level.Start;
            FinishSector = level.Exit;
            for (int x = 0; x < TblSize.Width; x++)
                for (int y = 0; y < TblSize.Height; y++)
                {
                    Direction voidPassage = Direction.None;
                    if (StartSector.Value.position == new Point(x, y))
                        voidPassage = StartSector.Value.direction;
                    if (FinishSector.Value.position == new Point(x, y))
                        voidPassage = FinishSector.Value.direction;
                    Sectors[x, y] = new Sector(this, new Point(x, y), level.Structure[x, y].RW, level.Structure[x, y].DW, voidPassage);
                }
            foreach (Door door in level.Doors)
            {
                door.Render(SectorScaleValue, PParametrs.DoorOpenSpeed);
                this[door.TblPosition].AddDoor(door);
            }
            foreach (var button in level.Buttons)
            {
                button.SetOwner(this);
                button.Render(SectorScaleValue);
                this[button.TblPosition].AddButton(button);
            }
            Player = new Player(this, player);
            Player.Render();
            UpdateEvent += Player.Update;
            OnPaint(box);
            PaintUpdate = new Timer(OnPaint, box, -1, 100);
            PhisicUpdate = new Timer(Update, null, -1, 100);

            SyncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        protected SynchronizationContext SyncContext;

        protected Timer PaintUpdate;
        private bool PaintFrameStart = false;
        protected void OnPaint(object state)
        {
            if (!PaintFrameStart)
            {
                PaintFrameStart = true;
                Image img = PaintProcess();
                (state as IScreen).UpdateImage(img);
                img.Dispose();
                PaintFrameStart = false;
            }
        }
        protected virtual Image PaintProcess()
        {
            Image img = new Bitmap(ImgSize.Width, ImgSize.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                g.Clear(Color.White);
                List<int> usingID = new List<int>();
                for (int x = 0; x < TblSize.Width; x++)
                    for (int y = 0; y < TblSize.Height; y++)
                    {
                        foreach (var door in from (Door door, bool blockState) D in Sectors[x, y].GetDoors
                                             where !usingID.Contains(D.door.ID) && D.blockState
                                             select D.door)
                        {
                            usingID.Add(door.ID);
                            g.DrawImage(door.Texture,
                                        door.Bounds.X + Sectors[x, y].Bounds.Location.X, door.Bounds.Y + Sectors[x, y].Bounds.Location.Y,
                                        door.Bounds.Width, door.Bounds.Height);
                        }
                        g.DrawImage(Sectors[x, y].Background, Sectors[x, y].Bounds);
                        if (Sectors[x, y].Button != null)
                        {
                            Button button = Sectors[x, y].Button;
                            g.DrawImage(button.Texture,
                                        button.Bounds.X + Sectors[x, y].Bounds.Location.X, button.Bounds.Y + Sectors[x, y].Bounds.Location.Y,
                                        button.Bounds.Width, button.Bounds.Height);
                        }
                    }
                if (Player != null)
                    g.DrawImage(Player.Texture, Player.ImgLocation);
            }
            return img;
        }

        protected Timer PhisicUpdate;
        public event EventHandler UpdateEvent;
        private bool PhisicFrameStart = false;

        protected void Update(object state)
        {
            if (!PhisicFrameStart)
            {
                PhisicFrameStart = true;
                EventHandler UpdateEnd = (sender, e) => PhisicFrameStart = false;
                UpdateEvent += UpdateEnd;
                UpdateEvent(this, EventArgs.Empty);
                UpdateEvent -= UpdateEnd;
            }
        }

        protected void PlayerToStart()
        {
            RectangleF startWall = this[StartSector.Value.position].Bounds;
            Player.TpTo(StartSector.Value.direction switch
            {
                Direction.Up => new PointF(startWall.Left + startWall.Width / 2, startWall.Top + BParametrs.Offset),
                Direction.Right => new PointF(startWall.Right - BParametrs.Offset, startWall.Top + startWall.Height / 2),
                Direction.Down => new PointF(startWall.Left + startWall.Width / 2, startWall.Bottom - BParametrs.Offset),
                Direction.Left => new PointF(startWall.Left + BParametrs.Offset, startWall.Top + startWall.Height / 2),
                _ => throw StartSector.Value.direction.ThrowInvalidDirectionException()
            });
        }
        public void GameStart()
        {
            PlayerToStart();
            Started = true;
        }
        public void GamePause()
        {
            Started = false;
        }
        public  void GameContinue()
        {
            Started = true;
        }
        public void CheckFinish((Point position, Direction leave) sector)
        {
            if (sector.leave == FinishSector.Value.direction)
            {
                var checkSector = sector.position + sector.leave.ToSizeOrEmpty();
                if (checkSector == FinishSector.Value.position)
                   SyncContext.Post(GameOver, null);
            }
        }
        private void GameOver(object state)
        {
            Started = false;
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler OnGameOver;

        protected float GetSectorScale(Size formSize)
        {
            float Y = (float)formSize.Height / TblSize.Height;
            float X = (float)formSize.Width / TblSize.Width;
            return X > Y ? Y : X;
        }
        public (Sector sector, Point position) GetPositionSector(Point imgLocation)
        {
            float sectorSize = SectorScaleValue;
            int x = imgLocation.Y / sectorSize.Round();
            int y = imgLocation.X / sectorSize.Round();
            if (imgLocation.Y < 0 && x == 0)
                x = -1;
            if (imgLocation.X < 0 && y == 0)
                y = -1;
            Point p = new Point(x, y);
            return (this[p], p);
        }

        public string Debug()
        {
            return $"JumpPower {PParametrs.JumpPower}\nGravity {PParametrs.MaxGravity}\nMaxSpeed {PParametrs.MaxSpeed}\nDoorOpenSpeed {PParametrs.DoorOpenSpeed}\n" +
                   $"Ground {BParametrs.Ground}\nWallBound {BParametrs.WallBound}\nOffset {BParametrs.Offset}";
        }

        public struct PhisicParametrs
        {
            public float JumpPower { get; }
            public int MaxGravity { get; }
            public float Gravity { get; }
            public int MaxSpeed { get; }
            public float Acceleration { get; }
            public float Deceleration => Acceleration * 2;
            public int DoorOpenSpeed { get; }

            public PhisicParametrs(float sectorScale)
            {
                JumpPower = sectorScale * 0.14f;
                MaxGravity = (sectorScale * 0.15f).Round();
                Gravity = sectorScale * 9.8f / 1000;
                MaxSpeed = (sectorScale * 0.09f).Round();
                Acceleration = sectorScale * 0.0075f;
                DoorOpenSpeed = (sectorScale * 0.09f).Round();
            }
        }
        public struct BoundsParametrs
        {
            public float Ground { get; }
            public float WallBound { get; }
            public float Offset { get; }

            public BoundsParametrs(float sectorScale)
            {
                Ground = sectorScale * 0.03f;
                WallBound = sectorScale * 0.06f;
                Offset = sectorScale * 0.03f;
            }
        }
    }

    [Serializable]
    public struct Level : ISerializable
    {
        public static readonly Level Empty = new Level();

        public Guid GUID { get; private set; }
        public void ChangeGUID(Guid guid) => GUID = guid;
        public (bool RW, bool DW)[,] Structure { get; }
        public Size Size => new Size(Structure.GetLength(0), Structure.GetLength(1));
        public (Point Position, Direction IN) Start { get; }
        public (Point Position, Direction OUT) Exit { get; }
        public Door[] Doors { get; }
        public Button[] Buttons { get; }

        public Level((bool RW, bool DW)[,] structure,
                     (Point Position, Direction IN) StartSector, (Point Position, Direction OUT) FinishSector,
                     Door[] doors, Button[] buttons)
        {
            GUID = Guid.NewGuid();
            Structure = structure;
            Start = StartSector;
            Exit = FinishSector;
            Doors = doors;
            Buttons = buttons;
        }

        public void Save(string path)
        {
            BinaryFormatter F = new BinaryFormatter();
            using FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            F.Serialize(fs, this);
        }
        public static Level Load(string path)
        {
            BinaryFormatter F = new BinaryFormatter();
            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return (Level)F.Deserialize(fs);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GUID", GUID.ToString());
            info.AddValue("Width", Size.Width);
            info.AddValue("Height", Size.Height);
            for (int x = 0; x < Size.Width; x++)
                for (int y = 0; y < Size.Height; y++)
                    info.AddValue($"{x},{y}", (Structure[x, y].RW ? 10 : 0) + (Structure[x, y].DW ? 1 : 0));
            info.AddValue("Start", $"{Start.Position.X},{Start.Position.Y},{(int)Start.IN}");
            info.AddValue("Exit", $"{Exit.Position.X},{Exit.Position.Y},{(int)Exit.OUT}");
            info.AddValue("DoorsCount", Doors.Length);
            for (int i = 0; i < Doors.Length; i++)
                info.AddValue("Door" + Doors[i].ID, Doors[i].Serialize);
            info.AddValue("ButtonsCount", Buttons.Length);
            for (int i = 0; i < Buttons.Length; i++)
            {
                info.AddValue("Button" + i, Buttons[i].Serialize);
            }
        }
        private Level(SerializationInfo info, StreamingContext context)
        {
            GUID = Guid.Parse(info.GetString("GUID"));
            int width = info.GetInt32("Width");
            int height = info.GetInt32("Height");
            int[,] structure = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    structure[x, y] = info.GetInt32($"{x},{y}");
            Structure = new (bool LW, bool UW)[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Structure[x, y] = (RW: structure[x, y] / 10 != 0, DW: structure[x, y] % 10 != 0);
            int[] start = info.GetString("Start").Split(',').Select(S => int.Parse(S)).ToArray();
            Start = (new Point(start[0], start[1]), (Direction)start[2]);
            int[] exit = info.GetString("Exit").Split(',').Select(S => int.Parse(S)).ToArray();
            Exit = (new Point(exit[0], exit[1]), (Direction)exit[2]);
            Doors = new Door[info.GetInt32("DoorsCount")];
            for (int i = 0; i < Doors.Length; i++)
            {
                int id = 1;
                try
                {
                    int[] value = info.GetString("Door" + id).Split(',').Select(S => int.Parse(S)).ToArray();
                    Doors[i] = new Door(id, new Point(value[0], value[1]), (Direction)value[2], (Direction)value[3]);
                }
                catch (SerializationException)
                {
                    id++;
                    i--;
                }
            }
            Buttons = new Button[info.GetInt32("ButtonsCount")];
            for (int i = 0; i < Buttons.Length; i++)
            {
                int[] value = info.GetString("Button" + i).Split(',').Select(S => int.Parse(S)).ToArray();
                Buttons[i] = new Button(new Point(value[0], value[1]), (Direction)value[2], value[3]);
            }
        }
    }

    public interface IScreen
    {
        public Image Image {get;}
        public void UpdateImage(Image img);
    }
}