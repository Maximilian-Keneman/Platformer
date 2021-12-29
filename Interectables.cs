using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Button
    {
        protected GameTable Owner { get; private set; }
        protected int DoorID { get; }
        private bool Activated = false;
        public Point TblPosition { get; protected set; }
        public Direction Wall { get; protected set; }
        public RectangleF Bounds { get; private set; }
        private float SectorScale;

        public Button(Point position, Direction wall, int doorId)
        {
            DoorID = doorId;
            TblPosition = position;
            Wall = wall;
        }
        public void SetOwner(GameTable owner) => Owner = owner;
        public void Render(float sectorScale)
        {
            SectorScale = sectorScale;
            float delta = sectorScale / 20 * 3;
            Bounds = Wall switch
            {
                Direction.Up => new RectangleF(0, 0, sectorScale, delta),
                Direction.Right => new RectangleF(sectorScale - delta, 0, delta, sectorScale),
                Direction.Down => new RectangleF(0, sectorScale - delta, sectorScale, delta),
                Direction.Left => new RectangleF(0, 0, delta, sectorScale),
                _ => throw Wall.ThrowInvalidDirectionException()
            };
            float ImgSectorScale = 200;
            float ImgBase = (Activated ? 2 : 1) * ImgSectorScale;
            float ImgDelta = ImgSectorScale / 20 * 3;
            RectangleF ImgRect = Wall switch
            {
                Direction.Up => new RectangleF(ImgBase, 0, ImgSectorScale, ImgDelta),
                Direction.Right => new RectangleF(ImgBase + ImgSectorScale - ImgDelta, 0, ImgDelta, ImgSectorScale),
                Direction.Down => new RectangleF(ImgBase, ImgSectorScale - ImgDelta, ImgSectorScale, ImgDelta),
                Direction.Left => new RectangleF(ImgBase, 0, ImgDelta, ImgSectorScale),
                _ => throw Wall.ThrowInvalidDirectionException()
            };
            Texture = Images.GetFragment(Properties.Resources.Standart, Size.Truncate(ImgRect.Size), ImgRect, Size.Truncate(new SizeF(sectorScale, sectorScale)));
        }
        public Image Texture { get; private set; }

        public void Activate()
        {
            if (!Activated)
            {
                Activated = !Activated;
                Render(SectorScale);
                Owner.GetAllDoors().First(D => D.ID == DoorID).Activate(true);
            }
        }

        public string Serialize => $"{TblPosition.X},{TblPosition.Y},{(int)Wall},{DoorID}";
    }
    public class Door
    {
        public int ID { get; }
        public Point TblPosition { get; }
        public Direction Wall { get; protected set; }
        private RectangleF bounds;
        public RectangleF Bounds => bounds;
        public Direction OpenDirection { get; protected set; }
        public bool OpenState { get; private set; } = false;
        private float SectorScale;
        public Door(int id, Point position, Direction wall, Direction openDirection)
        {
            ID = id;
            TblPosition = position;
            Wall = wall;
            OpenDirection = openDirection;
        }
        public void Render(float sectorScale, int openSpeed)
        {
            OpenSpeed = openSpeed;
            SectorScale = sectorScale;
            float delta = sectorScale / 10f;
            bounds = Wall switch
            {
                Direction.Up => new RectangleF(new PointF(0, 0), new SizeF(sectorScale, delta)),
                Direction.Right => new RectangleF(new PointF(sectorScale - delta, 0), new SizeF(delta, sectorScale)),
                Direction.Down => new RectangleF(new PointF(0, sectorScale - delta), new SizeF(sectorScale, delta)),
                Direction.Left => new RectangleF(new PointF(0, 0), new SizeF(delta, sectorScale)),
                _ => throw Wall.ThrowInvalidDirectionException()
            };
            if (OpenState)
            {
                OpenState = false;
                Activate(false);
            }
            int ImgSectorScale = 200;
            Size ImgSize = Wall switch
            {
                Direction.Up => new Size(ImgSectorScale, ImgSectorScale / 10),
                Direction.Right => new Size(ImgSectorScale / 10, ImgSectorScale),
                Direction.Down => new Size(ImgSectorScale, ImgSectorScale / 10),
                Direction.Left => new Size(ImgSectorScale / 10, ImgSectorScale),
                _ => throw Wall.ThrowInvalidDirectionException()
            };
            Texture = Images.GetFragment(Properties.Resources.Standart, ImgSize, new RectangleF(new PointF(3 * ImgSectorScale, 0), ImgSize), Size.Truncate(new SizeF(sectorScale, sectorScale)));
        }

        public Image Texture { get; private set; }

        public void Activate(bool animated)
        {
            if (animated)
                Task.Run(Move);
            else
                Swap();
        }

        private int OpenSpeed;
        private async void Move()
        {
            Size delta = OpenDirection switch
            {
                Direction.Up => OpenState ? new Size(0, 1) : new Size(0, -1),
                Direction.Right => OpenState ? new Size(-1, 0) : new Size(1, 0),
                Direction.Down => OpenState ? new Size(0, -1) : new Size(0, 1),
                Direction.Left => OpenState ? new Size(1, 0) : new Size(-1, 0),
                _ => throw OpenDirection.ThrowInvalidDirectionException()
            };
            float Path = 0,
                  End = SectorScale;
            while (Path < End)
            {
                bounds.Location += delta;
                Path++;
                await Task.Delay(OpenSpeed);
            }
            OpenState = !OpenState;
        }
        private void Swap()
        {
            SizeF delta = OpenDirection switch
            {
                Direction.Up => OpenState ? new SizeF(0, SectorScale) : new SizeF(0, -SectorScale),
                Direction.Right => OpenState ? new SizeF(-SectorScale, 0) : new SizeF(SectorScale, 0),
                Direction.Down => OpenState ? new SizeF(0, -SectorScale) : new SizeF(0, SectorScale),
                Direction.Left => OpenState ? new SizeF(SectorScale, 0) : new SizeF(-SectorScale, 0),
                _ => throw OpenDirection.ThrowInvalidDirectionException()
            };
            bounds.Location += delta;
            OpenState = !OpenState;
        }

        public string Serialize => $"{TblPosition.X},{TblPosition.Y},{(int)Wall},{(int)OpenDirection}";

        public string GetInfo => $"D{ID}-{Wall} at {{{TblPosition.X}, {TblPosition.Y}}}";
    }
}
