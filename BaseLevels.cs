using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public static class BaseLevels
    {
        //System.Windows.Forms.MessageBox.Show(level.GUID.ToString());
        public static Level Tutorial_1
        {
            get
            {
                Level level = new Level(new (bool RW, bool DW)[,]
                {
                    { (0, 1).ToBool(), (0, 1).ToBool(), (0, 0).ToBool(), (0, 1).ToBool(), (1, 1).ToBool() },
                    { (0, 1).ToBool(), (1, 0).ToBool(), (0, 1).ToBool(), (0, 1).ToBool(), (1, 0).ToBool() },
                    { (1, 1).ToBool(), (0, 1).ToBool(), (0, 1).ToBool(), (0, 1).ToBool(), (1, 0).ToBool() },
                    { (0, 1).ToBool(), (0, 1).ToBool(), (0, 1).ToBool(), (0, 1).ToBool(), (1, 1).ToBool() },
                },
                (new Point(0, 0), Direction.Up), (new Point(1, 0), Direction.Left),
                new Door[]
                {
                    new Door(1, new Point(1, 3), Direction.Left, Direction.Down)
                },
                new Button[]
                {
                    new Button(new Point(0, 4), Direction.Right, 1),
                    new Button(new Point(3, 0), Direction.Left, 1),
                });
                level.ChangeGUID(Guid.Parse("2255017b-84e2-4a05-af75-5f57642d5b9c"));
                return level;
            }
        }
    }
}
