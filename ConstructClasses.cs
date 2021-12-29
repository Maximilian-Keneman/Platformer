using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Platformer;
using Button = Platformer.Button;
using Timer = System.Threading.Timer;

namespace ConstructPlatformer
{
    public class ConstructSector : Sector
    {
        private new Dictionary<Direction, ConstructSector> NeighborSector => base.NeighborSector.ToDictionary(N => N.Key, N => N.Value as ConstructSector);
        public new ConstructButton Button
        {
            get => base.Button as ConstructButton;
            private set => base.Button = value;
        }

        public ConstructSector(GameTable table, Point tblPosition, bool RW, bool DW, Direction voidPassage = Direction.None) :
            base(table, tblPosition, RW, DW, voidPassage)
        { }

        private Dictionary<Direction, RectangleF> SelfConstructWallBounds
        {
            get
            {
                float wallWidth = Scale / 20;
                return new Dictionary<Direction, RectangleF>
                {
                    { Direction.Up, new RectangleF(Bounds.Left + wallWidth, Bounds.Top, Scale - 2 * wallWidth, wallWidth) },
                    { Direction.Right, new RectangleF(Bounds.Left + Scale - wallWidth, Bounds.Top + wallWidth, wallWidth, Scale - 2 * wallWidth) },
                    { Direction.Down, new RectangleF(Bounds.Left + wallWidth, Bounds.Top + Scale - wallWidth, Scale - 2 * wallWidth, wallWidth) },
                    { Direction.Left, new RectangleF(Bounds.Left, Bounds.Top + wallWidth, wallWidth, Scale - 2 * wallWidth) }
                };
            }
        }
        public Dictionary<Direction, RectangleF> ConstructWallBounds => new Dictionary<Direction, RectangleF>
        {
            { Direction.Up, NeighborSector[Direction.Up] == null ? RectangleF.Empty :
                RectangleF.Union(SelfConstructWallBounds[Direction.Up],
                    NeighborSector[Direction.Up].SelfConstructWallBounds[Direction.Down]) },
            { Direction.Right, NeighborSector[Direction.Right] == null ? RectangleF.Empty :
                RectangleF.Union(SelfConstructWallBounds[Direction.Right],
                    NeighborSector[Direction.Right].SelfConstructWallBounds[Direction.Left]) },
            { Direction.Down, NeighborSector[Direction.Down] == null ? RectangleF.Empty :
                RectangleF.Union(SelfConstructWallBounds[Direction.Down],
                    NeighborSector[Direction.Down].SelfConstructWallBounds[Direction.Up]) },
            { Direction.Left, NeighborSector[Direction.Left] == null ? RectangleF.Empty :
                RectangleF.Union(SelfConstructWallBounds[Direction.Left],
                    NeighborSector[Direction.Left].SelfConstructWallBounds[Direction.Right]) },
        };

        public void ChangeWall(Direction direction, bool ignoreVoid)
        {
            if (!ignoreVoid)
            {
                ConstructSector neighbor = NeighborSector[direction];
                if (neighbor == null)
                    return;
                switch (direction)
                {
                    case Direction.Up:
                        neighbor.ChangeWall(Direction.Down, ignoreVoid);
                        break;
                    case Direction.Right:
                        RightWall = !RightWall;
                        neighbor.PaintBackground();
                        break;
                    case Direction.Down:
                        DownWall = !DownWall;
                        neighbor.PaintBackground();
                        break;
                    case Direction.Left:
                        neighbor.ChangeWall(Direction.Right, ignoreVoid);
                        break;
                }
            }
            else
                switch (direction)
                {
                    case Direction.Up:
                        if (VoidPassage != Direction.Up)
                            VoidPassage = Direction.Up;
                        else
                            VoidPassage = Direction.None;
                        break;
                    case Direction.Right:
                        RightWall = !RightWall;
                        break;
                    case Direction.Down:
                        DownWall = !DownWall;
                        break;
                    case Direction.Left:
                        if (VoidPassage != Direction.Left)
                            VoidPassage = Direction.Left;
                        else
                            VoidPassage = Direction.None;
                        break;
                }
            PaintBackground();
        }

        public void RemoveDoor(Direction doorWall)
        {
            if (Doors.ContainsKey(doorWall))
            {
                NeighborSector[Doors[doorWall].door.OpenDirection]?.Doors.Remove(doorWall);
                Doors.Remove(doorWall);
            }
        }

        public void ChangeButtonWall(Direction wall)
        {
            Button.ChangeWall(wall, Scale);
        }
        public void RemoveButton()
        {
            Button = null;
        }

        public (bool RW, bool DW) GetStruct => (RightWall, DownWall);
    }
    public class ConstructGameTable : GameTable
    {
        private new ConstructSector this[int x, int y] => base[x, y] as ConstructSector;
        private new ConstructSector this[Point position] => base[position] as ConstructSector;

        public ConstructGameTable(Size imgSize)
        {
            Sectors = new ConstructSector[1, 1];
            SectorScaleValue = GetSectorScale(imgSize);
            PParametrs = new PhisicParametrs(SectorScaleValue);
            BParametrs = new BoundsParametrs(SectorScaleValue);
            Sectors[0, 0] = new ConstructSector(this, new Point(0, 0), true, true);
            ImgSize = imgSize;
        }
        public ConstructGameTable(Level level, Size imgSize)
        {
            Sectors = new ConstructSector[level.Size.Width, level.Size.Height];
            SectorScaleValue = GetSectorScale(imgSize);
            PParametrs = new PhisicParametrs(SectorScaleValue);
            BParametrs = new BoundsParametrs(SectorScaleValue);
            ImgSize = Size.Truncate(new SizeF(TblSize.Height * SectorScaleValue, TblSize.Width * SectorScaleValue));
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
                    Sectors[x, y] = new ConstructSector(this, new Point(x, y), level.Structure[x, y].RW, level.Structure[x, y].DW, voidPassage);
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
        }

        public void InitStart(Player.PlayerArgs player, IScreen box)
        {
            Player = new Player(this, player);
            Player.Render();
            UpdateEvent += Player.Update;
            OnPaint(box);
            PaintUpdate = new Timer(OnPaint, box, -1, 100);
            PhisicUpdate = new Timer(Update, null, -1, 100);

            SyncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            GameStart();
        }
        public void Deinit()
        {
            if (Started)
                GamePause();
            SyncContext = null;
            PaintUpdate = null;
            PhisicUpdate = null;
            UpdateEvent -= Player.Update;
            Player = null;
        }

        public enum Tool
        {
            None,
            Wall,
            Door,
            DoorDirection,
            Button,
            ButtonDoor,
            Enter,
            Exit
        }

        private Tool selectedTool;
        public Tool SelectedTool
        {
            get => selectedTool;
            set
            {
                if (value != Tool.Button && value != Tool.ButtonDoor)
                    ProjectButton = null;
                if (value != Tool.DoorDirection)
                    ProjectDoor = null;
                selectedTool = value;
            }
        }
        public Point Position { get; set; }
        public Direction Direction { get; set; }
        public (Direction vertical, Direction horizontal) Quarter { get; set; }
        private ConstructDoor ProjectDoor;
        private void DesignDoor()
        {
            if (ProjectDoor == null)
            {
                ProjectDoor = new ConstructDoor(Position, Direction, Direction.None);
                ProjectDoor.Render(SectorScaleValue, PParametrs.DoorOpenSpeed);
            }
            else
            {
                ProjectDoor.ChangeDirection(Position, Direction, Quarter);
                ProjectDoor.Render(SectorScaleValue, PParametrs.DoorOpenSpeed);
            }
        }
        private ConstructButton ProjectButton;
        private void DesignButton()
        {
            Direction wall = Direction == Direction.None ? Direction.None : GetButtonWall(Position, Direction, Quarter);
            if (wall == Direction.None)
                ProjectButton = null;
            else if (ProjectButton == null)
            {
                ProjectButton = new ConstructButton(this, Position, wall, -1);
                ProjectButton.Render(SectorScaleValue);
            }
            else
            {
                ProjectButton.ChangePosition(Position);
                ProjectButton.ChangeWall(wall, SectorScaleValue);
            }
        }
        public void OnPaint(IScreen box) => base.OnPaint(box);

        protected override Image PaintProcess()
        {
            if (Started)
                return base.PaintProcess();
            Image img = base.PaintProcess();
            using (Graphics g = Graphics.FromImage(img))
            {
                foreach (var button in from ConstructSector S in Sectors
                                       where S.Button != null
                                       select S.Button)
                    g.DrawLine(new Pen(Color.DarkMagenta, SectorScaleValue / 30),
                               this[button.TblPosition].Bounds.Location + new SizeF(button.Bounds.Center()),
                               this[button.Door.TblPosition].Bounds.Location + new SizeF(button.Door.Bounds.Center()));
                foreach (var door in Sectors.Cast<ConstructSector>()
                                            .SelectMany(S => S.GetDoors)
                                            .Select(D => D.door))
                {
                    RectangleF bounds = new RectangleF(this[door.TblPosition].Bounds.Location + new SizeF(door.Bounds.Location), door.Bounds.Size);
                    g.DrawArrow(door.OpenState ? Color.Orange : Color.LimeGreen,
                                bounds, SectorScaleValue / 4, SectorScaleValue / 36,
                                door.OpenState ? door.OpenDirection.Inverse() : door.OpenDirection);
                }
            }
            if (SelectedTool == Tool.Button)
                DesignButton();
            if (SelectedTool == Tool.DoorDirection)
                DesignDoor();
            if (this[Position] != null && Direction != Direction.None)
            {
                using Graphics g = Graphics.FromImage(img);
                switch (SelectedTool)
                {
                    case Tool.Wall:
                    {
                        RectangleF bounds = this[Position].ConstructWallBounds[Direction];
                        g.FillRectangle(Brushes.White, bounds);
                        Color color = this[Position].CanGoWithoutDoors[Direction] ? Color.FromArgb(100, 80, 0, 0)
                                                                                  : Color.FromArgb(100, 0, 80, 0);
                        g.FillRectangle(new SolidBrush(color), bounds);
                        break;
                    }
                    case Tool.DoorDirection:
                    {
                        RectangleF bounds = new RectangleF(this[ProjectDoor.TblPosition].Bounds.Location + new SizeF(ProjectDoor.Bounds.Location), ProjectDoor.Bounds.Size);
                        g.DrawImage(ProjectDoor.Texture.AdjustAlpha(0.5f), bounds);
                        if (ProjectDoor.OpenDirection != Direction.None)
                            g.DrawArrow(ProjectDoor.OpenState ? Color.Orange : Color.LimeGreen,
                                        bounds, SectorScaleValue / 4, SectorScaleValue / 36,
                                        ProjectDoor.OpenState ? ProjectDoor.OpenDirection.Inverse() : ProjectDoor.OpenDirection);
                        break;
                    }
                    case Tool.Button:
                    {
                        if (ProjectButton != null && this[ProjectButton.TblPosition].Button == null)
                            g.DrawImage(ProjectButton.Texture.AdjustAlpha(0.5f),
                                        ProjectButton.Bounds.X + this[ProjectButton.TblPosition].Bounds.Location.X, ProjectButton.Bounds.Y + this[ProjectButton.TblPosition].Bounds.Location.Y,
                                        ProjectButton.Bounds.Width, ProjectButton.Bounds.Height);
                        break;
                    }
                    case Tool.ButtonDoor:
                    {
                        Direction direction = CheckDoors(Position, Direction, Quarter);
                        if (direction != Direction.None)
                        {
                            PointF p1 = this[ProjectButton.TblPosition].Bounds.Location + new SizeF(ProjectButton.Bounds.Location + new SizeF(ProjectButton.Bounds.Width / 2, ProjectButton.Bounds.Height / 2));
                            PointF p2 = this[Position].ConstructWallBounds[direction].Center();
                            g.DrawLine(new Pen(Color.LightBlue, SectorScaleValue / 23), p1, p2);
                        }
                        goto case Tool.Button;
                    }
                    case Tool.Enter:
                    case Tool.Exit:
                    {
                        if (Direction != Direction.None)
                        {
                            Direction direction = (Position.X == 0, Position.Y == 0, Position.X == TblSize.Width - 1, Position.Y == TblSize.Height - 1) switch
                            {
                                (true, true, false, false) => Direction == Direction.Up || Direction == Direction.Right ? Direction.Up : Direction.Left,
                                (true, false, false, false) => Direction.Up,
                                (true, false, false, true) => Direction == Direction.Down || Direction == Direction.Right ? Direction.Right : Direction.Up,
                                (false, false, false, true) => Direction.Right,
                                (false, false, true, true) => Direction == Direction.Down || Direction == Direction.Left ? Direction.Down : Direction.Right,
                                (false, false, true, false) => Direction.Down,
                                (false, true, true, false) => Direction == Direction.Up || Direction == Direction.Left ? Direction.Left : Direction.Down,
                                (false, true, false, false) => Direction.Left,
                                _ => Direction.None,
                            };
                            float width = SectorScaleValue / 23;
                            float delta = SectorScaleValue / 8;
                            var rect = direction switch
                            {
                                Direction.Up => new RectangleF(this[Position].Bounds.Left + SectorScaleValue * 5 / 11, this[Position].Bounds.Top + SectorScaleValue / 8, SectorScaleValue / 11, SectorScaleValue * 3 / 8),
                                Direction.Right => new RectangleF(this[Position].Bounds.Left + SectorScaleValue / 2, this[Position].Bounds.Top + SectorScaleValue * 5 / 11, SectorScaleValue * 3 / 8, SectorScaleValue / 11),
                                Direction.Down => new RectangleF(this[Position].Bounds.Left + SectorScaleValue * 5 / 11, this[Position].Bounds.Top + SectorScaleValue / 2, SectorScaleValue / 11, SectorScaleValue * 3 / 8),
                                Direction.Left => new RectangleF(this[Position].Bounds.Left + SectorScaleValue / 8, this[Position].Bounds.Top + SectorScaleValue * 5 / 11, SectorScaleValue * 3 / 8, SectorScaleValue / 11),
                                _ => RectangleF.Empty,
                            };
                            if (!rect.IsEmpty)
                                if (SelectedTool == Tool.Exit)
                                    g.DrawArrow(Color.Red, rect, delta, width, direction);
                                else if (SelectedTool == Tool.Enter)
                                    g.DrawArrow(Color.Green, rect, delta, width, direction.Inverse());
                        }
                        break;
                    }
                }
            }
            return img;
        }

        public void ChangeSize(Size newSize, int sectorScale)
        {
            SectorScaleValue = sectorScale;
            PParametrs = new PhisicParametrs(SectorScaleValue);
            BParametrs = new BoundsParametrs(SectorScaleValue);
            ConstructSector[,] newSectors = new ConstructSector[newSize.Width, newSize.Height];
            for (int x = 0; x < newSize.Width; x++)
                for (int y = 0; y < newSize.Height; y++)
                {
                    if (x < TblSize.Width && y < TblSize.Height)
                    {
                        newSectors[x, y] = this[x, y];
                        if (x == newSize.Width - 1)
                        {
                            if (this[x, y].CanGo[Direction.Down])
                                this[x, y].ChangeWall(Direction.Down, true);
                            if (TblSize.Width > newSize.Width)
                                foreach (Direction doorWall in from (Door door, bool blockState) D in this[x, y].GetDoors
                                                               where D.door.OpenDirection == (D.blockState ? Direction.Down : Direction.Up)
                                                               select D.door.Wall)
                                    this[x, y].RemoveDoor(doorWall);
                        }
                        if (y == newSize.Height - 1)
                        {
                            if (this[x, y].CanGo[Direction.Right])
                                this[x, y].ChangeWall(Direction.Right, true);
                            if (TblSize.Height > newSize.Height)
                                foreach (Direction doorWall in from (Door door, bool blockState) D in this[x, y].GetDoors
                                                               where D.door.OpenDirection == (D.blockState ? Direction.Right : Direction.Left)
                                                               select D.door.Wall)
                                    this[x, y].RemoveDoor(doorWall);
                        }
                        newSectors[x, y].PaintBackground();
                    }
                    else
                        newSectors[x, y] = new ConstructSector(this, new Point(x, y), true, true);
                }
            Sectors = newSectors;
            var doors = Sectors.Cast<ConstructSector>().SelectMany(S => S.GetDoors).Select(D => D.door.ID);
            foreach (var sector in from ConstructSector S in Sectors
                                   let B = S.Button
                                   where B != null && !doors.Contains(B.Door.ID)
                                   select B.TblPosition)
                this[sector].RemoveButton();
        }
        public void ChangeSector(Point position, Direction direction)
        {
            if (this[position].Button != null && this[position].Button.Wall == direction)
            {
                Direction wall = NextButtonWall(position, this[position].Button.Wall);
                if (wall == Direction.None)
                    switch (MessageBox.Show("Других стен для кнопки нет. Если продолжить, то кнопка будет удалена. Продолжить?",
                                            "Нет стен", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Yes:
                            RemoveButton(position);
                            break;
                        case DialogResult.No:
                            return;
                    }
                else
                    this[position].ChangeButtonWall(wall);
            }
            this[position].ChangeWall(direction, false);
        }

        public void AddDoor(Point position, Direction wall, Direction openDirection)
        {
            var doors = Sectors.Cast<Sector>().SelectMany(S => S.GetDoors);
            int id = doors.Count() == 0 ? 1 : doors.Max(D => D.door.ID) + 1;
            Door door = new Door(id, position, wall, openDirection);
            door.Render(SectorScaleValue, PParametrs.DoorOpenSpeed);
            this[position].AddDoor(door);
        }
        private Direction CheckDoors(Point position, Direction direction, (Direction vertical, Direction horizontal) quarter)
        {
            Direction[] doors = this[position].GetDoors.Select(D => D.door.Wall).OrderBy(D => D).ToArray();
            Direction doorWall;
            switch (doors.Count())
            {
                case 0:
                    doorWall = Direction.None;
                    break;
                case 1:
                    doorWall = doors[0];
                    break;
                case 2:
                    doorWall = (doors[0], doors[1], direction) switch
                    {
                        (Direction.Up, Direction.Right, Direction.Up) => Direction.Up,
                        (Direction.Up, Direction.Right, Direction.Right) => Direction.Right,
                        (Direction.Up, Direction.Right, Direction.Down) => Direction.Right,
                        (Direction.Up, Direction.Right, Direction.Left) => Direction.Up,
                        (Direction.Up, Direction.Down, _) => quarter.vertical,
                        (Direction.Up, Direction.Left, Direction.Up) => Direction.Up,
                        (Direction.Up, Direction.Left, Direction.Right) => Direction.Up,
                        (Direction.Up, Direction.Left, Direction.Down) => Direction.Left,
                        (Direction.Up, Direction.Left, Direction.Left) => Direction.Left,
                        (Direction.Right, Direction.Down, Direction.Up) => Direction.Right,
                        (Direction.Right, Direction.Down, Direction.Right) => Direction.Right,
                        (Direction.Right, Direction.Down, Direction.Down) => Direction.Down,
                        (Direction.Right, Direction.Down, Direction.Left) => Direction.Down,
                        (Direction.Right, Direction.Left, _) => quarter.horizontal,
                        (Direction.Down, Direction.Left, Direction.Up) => Direction.Left,
                        (Direction.Down, Direction.Left, Direction.Right) => Direction.Down,
                        (Direction.Down, Direction.Left, Direction.Down) => Direction.Down,
                        (Direction.Down, Direction.Left, Direction.Left) => Direction.Left,
                        _ => throw new Exception()
                    };
                    break;
                case 3:
                    if (!doors.Contains(Direction.Up))
                        doorWall = direction == Direction.Up ? quarter.horizontal : direction;
                    else if (!doors.Contains(Direction.Right))
                        doorWall = direction == Direction.Right ? quarter.vertical : direction;
                    else if (!doors.Contains(Direction.Down))
                        doorWall = direction == Direction.Down ? quarter.horizontal : direction;
                    else if (!doors.Contains(Direction.Left))
                        doorWall = direction == Direction.Left ? quarter.vertical : direction;
                    else
                        throw new Exception();
                    break;
                case 4:
                    doorWall = direction;
                    break;
                default:
                    throw new Exception();
            }
            return doorWall;
        }
        public void RemoveDoor(Point position, Direction direction, (Direction vertical, Direction horizontal) quarter)
        {
            if (this[position].GetDoors.Length > 0)
            {
                Direction doorWall = CheckDoors(position, direction, quarter);
                foreach (var buttonPosition in from ConstructSector sector in Sectors
                                               let door = this[position].GetDoors.Single(D => D.door.Wall == doorWall).door
                                               where sector.Button?.Door.ID == door.ID
                                               select sector.Button.TblPosition)
                    RemoveButton(buttonPosition);
                this[position].RemoveDoor(doorWall);
            }
        }
        public void DoorActivate(Point position, Direction direction, (Direction vertical, Direction horizontal) quarter)
        {
            Direction doorWall = CheckDoors(position, direction, quarter);
            this[position].DoorActivate(doorWall, false);
        }

        public bool AddButton((Point position, Direction wall) button,
                              (Point position, Direction direction, (Direction vertical, Direction horizontal) quarter) door)
        {
            Direction doorWall = CheckDoors(door.position, door.direction, door.quarter);
            var buttonDoor = this[door.position].GetDoors.Where(D => D.door.Wall == doorWall);
            if (buttonDoor.Count() == 0)
                return false;
            else
            {
                ConstructButton NewButton = new ConstructButton(this, button.position, button.wall, buttonDoor.Single().door.ID);
                NewButton.Render(SectorScaleValue);
                this[button.position].AddButton(NewButton);
                return true;
            }
        }
        public bool RemoveButton(Point position)
        {
            if (this[position].Button != null)
            {
                this[position].RemoveButton();
                return true;
            }
            else
                return false;
        }
        public Direction GetButtonWall(Point position, Direction direction, (Direction vertical, Direction horizontal) quarter)
        {
            Dictionary<Direction, bool> go = this[position].CanGoWithoutDoors;
            var doors = this[position].GetDoors.Select(D => D.door.Wall);
            return checkWall(go[Direction.Up], go[Direction.Right], go[Direction.Down], go[Direction.Left], direction);

            Direction checkWall(bool up, bool right, bool down, bool left, Direction direction)
            {
                switch (up, right, down, left, direction)
                {
                    case (_, _, _, _, Direction.None):
                        throw direction.ThrowInvalidDirectionException();
                    case (false, false, false, false, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, false, false, false, Direction.Up);
                    case (false, false, false, true, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, false, false, true, Direction.Up);
                    case (false, false, true, false, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, false, true, false, Direction.Up);
                    case (false, false, true, true, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, false, true, true, Direction.Up);
                    case (false, true, false, false, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, true, false, false, Direction.Up);
                    case (false, true, false, true, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, true, false, true, Direction.Up);
                    case (false, true, true, false, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, true, true, false, Direction.Up);
                    case (false, true, true, true, Direction.Up):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, true, true, true, Direction.Up);
                    case (false, false, false, false, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(false, true, false, false, Direction.Right);
                    case (false, false, false, true, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(false, true, false, true, Direction.Right);
                    case (false, false, true, false, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(false, true, true, false, Direction.Right);
                    case (false, false, true, true, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(false, true, true, true, Direction.Right);
                    case (true, false, false, false, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(true, true, false, false, Direction.Right);
                    case (true, false, false, true, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(true, true, false, true, Direction.Right);
                    case (true, false, true, false, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(true, true, true, false, Direction.Right);
                    case (true, false, true, true, Direction.Right):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(true, true, true, true, Direction.Right);
                    case (false, false, false, false, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(false, false, true, false, Direction.Down);
                    case (false, true, false, false, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(false, true, true, false, Direction.Down);
                    case (true, false, false, false, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, false, true, false, Direction.Down);
                    case (true, true, false, false, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, true, true, false, Direction.Down);
                    case (false, false, false, true, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(false, false, true, true, Direction.Down);
                    case (false, true, false, true, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(false, true, true, true, Direction.Down);
                    case (true, false, false, true, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, false, true, true, Direction.Down);
                    case (true, true, false, true, Direction.Down):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, true, true, true, Direction.Down);
                    case (false, false, false, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(false, false, false, true, Direction.Left);
                    case (false, false, true, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(false, false, true, true, Direction.Left);
                    case (false, true, false, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(false, true, false, true, Direction.Left);
                    case (false, true, true, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(false, true, true, true, Direction.Left);
                    case (true, false, false, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(true, false, false, true, Direction.Left);
                    case (true, false, true, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(true, false, true, true, Direction.Left);
                    case (true, true, false, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(true, true, false, true, Direction.Left);
                    case (true, true, true, false, Direction.Left):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(true, true, true, true, Direction.Left);
                    case (true, false, false, true, Direction.Up):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(true, true, false, true, Direction.Up);
                    case (true, true, false, false, Direction.Up):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(true, true, false, true, Direction.Up);
                    case (false, true, true, false, Direction.Right):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, true, true, false, Direction.Right);
                    case (true, true, false, false, Direction.Right):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, true, true, false, Direction.Right);
                    case (false, false, true, true, Direction.Down):
                        return !doors.Contains(Direction.Right) ? Direction.Right : checkWall(false, true, true, true, Direction.Down);
                    case (false, true, true, false, Direction.Down):
                        return !doors.Contains(Direction.Left) ? Direction.Left : checkWall(false, true, true, true, Direction.Down);
                    case (false, false, true, true, Direction.Left):
                        return !doors.Contains(Direction.Up) ? Direction.Up : checkWall(true, false, true, true, Direction.Left);
                    case (true, false, false, true, Direction.Left):
                        return !doors.Contains(Direction.Down) ? Direction.Down : checkWall(true, false, true, true, Direction.Left);
                    case (true, false, false, false, Direction.Up):
                        if (quarter.horizontal == Direction.Right)
                        {
                            if (!doors.Contains(Direction.Right))
                                return Direction.Right;
                            else
                                return checkWall(true, true, false, false, Direction.Up);
                        }
                        else if (quarter.horizontal == Direction.Left)
                        {
                            if (!doors.Contains(Direction.Left))
                                return Direction.Left;
                            else
                                return checkWall(true, false, false, true, Direction.Up);
                        }
                        else
                            throw quarter.horizontal.ThrowInvalidDirectionException();
                    case (true, false, true, false, Direction.Up):
                        if (quarter.horizontal == Direction.Right)
                        {
                            if (!doors.Contains(Direction.Right))
                                return Direction.Right;
                            else
                                return checkWall(true, true, true, false, Direction.Up);
                        }
                        else if (quarter.horizontal == Direction.Left)
                        {
                            if (!doors.Contains(Direction.Left))
                                return Direction.Left;
                            else
                                return checkWall(true, false, true, true, Direction.Up);
                        }
                        else
                            throw quarter.horizontal.ThrowInvalidDirectionException();
                    case (false, true, false, false, Direction.Right):
                        if (quarter.vertical == Direction.Up)
                        {
                            if (!doors.Contains(Direction.Up))
                                return Direction.Up;
                            else
                                return checkWall(true, true, false, false, Direction.Right);
                        }
                        else if (quarter.vertical == Direction.Down)
                        {
                            if (!doors.Contains(Direction.Down))
                                return Direction.Down;
                            else
                                return checkWall(false, true, true, false, Direction.Right);
                        }
                        else
                            throw quarter.vertical.ThrowInvalidDirectionException();
                    case (false, true, false, true, Direction.Right):
                        if (quarter.vertical == Direction.Up)
                        {
                            if (!doors.Contains(Direction.Up))
                                return Direction.Up;
                            else
                                return checkWall(true, true, false, true, Direction.Right);
                        }
                        else if (quarter.vertical == Direction.Down)
                        {
                            if (!doors.Contains(Direction.Down))
                                return Direction.Down;
                            else
                                return checkWall(false, true, true, true, Direction.Right);
                        }
                        else
                            throw quarter.vertical.ThrowInvalidDirectionException();
                    case (false, false, true, false, Direction.Down):
                        if (quarter.horizontal == Direction.Right)
                        {
                            if (!doors.Contains(Direction.Right))
                                return Direction.Right;
                            else
                                return checkWall(false, true, true, false, Direction.Down);
                        }
                        else if (quarter.horizontal == Direction.Left)
                        {
                            if (!doors.Contains(Direction.Left))
                                return Direction.Left;
                            else
                                return checkWall(false, false, true, true, Direction.Down);
                        }
                        else
                            throw quarter.horizontal.ThrowInvalidDirectionException();
                    case (true, false, true, false, Direction.Down):
                        if (quarter.horizontal == Direction.Right)
                        {
                            if (!doors.Contains(Direction.Right))
                                return Direction.Right;
                            else
                                return checkWall(true, true, true, false, Direction.Down);
                        }
                        else if (quarter.horizontal == Direction.Left)
                        {
                            if (!doors.Contains(Direction.Left))
                                return Direction.Left;
                            else
                                return checkWall(true, false, true, true, Direction.Down);
                        }
                        else
                            throw quarter.horizontal.ThrowInvalidDirectionException();
                    case (false, true, false, true, Direction.Left):
                        if (quarter.vertical == Direction.Up)
                        {
                            if (!doors.Contains(Direction.Up))
                                return Direction.Up;
                            else
                                return checkWall(true, true, false, true, Direction.Left);
                        }
                        else if (quarter.vertical == Direction.Down)
                        {
                            if (!doors.Contains(Direction.Down))
                                return Direction.Down;
                            else
                                return checkWall(false, true, true, true, Direction.Left);
                        }
                        else
                            throw quarter.vertical.ThrowInvalidDirectionException();
                    case (false, false, false, true, Direction.Left):
                        if (quarter.vertical == Direction.Up)
                        {
                            if (!doors.Contains(Direction.Up))
                                return Direction.Up;
                            else
                                return checkWall(true, false, false, true, Direction.Left);
                        }
                        else if (quarter.vertical == Direction.Down)
                        {
                            if (!doors.Contains(Direction.Down))
                                return Direction.Down;
                            else
                                return checkWall(false, false, true, true, Direction.Left);
                        }
                        else
                            throw quarter.vertical.ThrowInvalidDirectionException();
                    case (false, true, true, true, _):
                        return !doors.Contains(Direction.Up) ? Direction.Up : Direction.None;
                    case (true, false, true, true, _):
                        return !doors.Contains(Direction.Right) ? Direction.Right : Direction.None;
                    case (true, true, false, true, _):
                        return !doors.Contains(Direction.Down) ? Direction.Down : Direction.None;
                    case (true, true, true, false, _):
                        return !doors.Contains(Direction.Left) ? Direction.Left : Direction.None;
                    case (true, true, true, true, _):
                        return Direction.None;
                    default:
                        throw new Exception("Что-то не учёл");
                }
            }
        }
        private Direction NextButtonWall(Point position, Direction start)
        {
            Dictionary<Direction, bool> go = this[position].CanGoWithoutDoors;
            go[start] = true;
            Direction direction = start == Direction.Left ? Direction.Up : start + 1;
            while (go[direction])
            {
                direction++;
                if ((int)direction == 4)
                    direction = Direction.Up;
                if (direction == start)
                    return Direction.None;
            }
            return direction;
        }

        public bool? CreatePassage((Point position, Direction direction) passage, bool isExit)
        {
            if (isExit && StartSector != passage)
            {
                if (!FinishSector.HasValue)
                {
                    FinishSector = passage;
                }
                else if (FinishSector == passage)
                {
                    FinishSector = null;
                }
                else
                {
                    this[FinishSector.Value.position].ChangeWall(FinishSector.Value.direction, true);
                    FinishSector = passage;
                }
            }
            else if (!isExit && FinishSector != passage)
            {
                if (!StartSector.HasValue)
                {
                    StartSector = passage;
                }
                else if (StartSector == passage)
                {
                    StartSector = null;
                }
                else
                {
                    this[StartSector.Value.position].ChangeWall(StartSector.Value.direction, true);
                    StartSector = passage;
                }
            }
            else
                return null;
            this[passage.position].ChangeWall(passage.direction, true);
            return isExit ? FinishSector.HasValue : StartSector.HasValue;
        }

        public string[] GetSectorInfo(Point position)
        {
            ConstructSector sector = this[position];
            string tblLocation = $"{{{position.X}, {position.Y}}}";
            string walls = string.Join(", ", sector.CanGoWithoutDoors.Select(b => b.Key + "-" + (b.Value ? 1 : 0)));
            string[] doors = sector.GetDoors.Select(D => D.door).Select(D => $"D{D.ID}-{D.Wall} open to {D.OpenDirection}, state {D.OpenState}").ToArray();
            ConstructButton B = sector.Button;
            string button = B != null ? $"B-{B.Wall} activate {B.DoorInfo}" : "";
            List<string> info = new List<string>();
            info.Add("Sector " + tblLocation);
            info.Add(walls);
            info.AddRange(doors);
            info.Add(button);
            return info.ToArray();
        }

        public void SaveLevel(string name)
        {
            (bool RW, bool DW)[,] structure = new (bool RW, bool DW)[TblSize.Width, TblSize.Height];
            List<Door> doors = new List<Door>();
            List<Button> buttons = new List<Button>();
            for (int x = 0; x < TblSize.Width; x++)
                for (int y = 0; y < TblSize.Height; y++)
                {
                    structure[x, y] = this[x, y].GetStruct;
                    if (this[x, y].GetDoors.Length != 0)
                        doors.AddRange(this[x, y].GetDoors.Select(D => D.door));
                    if (this[x, y].Button != null)
                        buttons.Add(this[x, y].Button);
                }
            new Level(structure, StartSector.Value, FinishSector.Value,
                      doors.GroupBy(D => D.ID).Select(g => g.First()).ToArray(), buttons.ToArray())
                .Save($"MyLevels\\{name}.lvdat");
        }
    }
    public class ConstructButton : Button
    {
        public ConstructButton(ConstructGameTable owner, Point position, Direction wall, int doorId) : base(position, wall, doorId)
        {
            SetOwner(owner);
        }

        public void ChangeWall(Direction wall, float scale)
        {
            Wall = wall;
            Render(scale);
        }
        public void ChangePosition(Point position)
        {
            TblPosition = position;
        }

        public Door Door => Owner.GetAllDoors().First(D => D.ID == DoorID);
        public string DoorInfo => Door.GetInfo;
    }
    public class ConstructDoor : Door
    {
        public ConstructDoor(Point position, Direction wall, Direction openDirection) : base(-1, position, wall, openDirection) { }

        public void ChangeDirection(Point position, Direction direction, (Direction vertical, Direction horizontal) quarter)
        {
            Direction DoorOpen = (position.X - TblPosition.X, position.Y - TblPosition.Y) switch
            {
                (-1, 0) => Direction.Up,
                (0, 1) => Direction.Right,
                (1, 0) => Direction.Down,
                (0, -1) => Direction.Left,
                _ => Direction.None,
            };
            Direction wall = DoorOpen switch
            {
                Direction.Up => quarter.horizontal,
                Direction.Right => quarter.vertical,
                Direction.Down => quarter.horizontal,
                Direction.Left => quarter.vertical,
                _ => direction,
            };
            Wall = wall;
            OpenDirection = DoorOpen;
        }
    }
}