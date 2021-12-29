using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ConstructPlatformer;

namespace Platformer
{
    public partial class ConstructorForm : UserControl
    {
        private MainForm Form => FindForm() as MainForm;
        public ConstructorForm()
        {
            InitializeComponent();
        }
        public void Init()
        {
            HeightNumeric.Value = 1;
            WidthNumeric.Value = 1;
            WallButton.Checked = true;
            EnterCheck.Checked = false;
            ExitCheck.Checked = false;
            NameBox.Text = "Name";
            IsPassable = false;
            CheckSaveConditions();
            Table = new ConstructGameTable(GameBox.Size);
            TablePaint();
        }
        public void Init(Level level, string name)
        {
            HeightNumeric.Value = level.Size.Width;
            WidthNumeric.Value = level.Size.Height;
            WallButton.Checked = true;
            EnterCheck.Checked = true;
            ExitCheck.Checked = true;
            NameBox.Text = name;
            IsPassable = false;
            CheckSaveConditions();
            Table = new ConstructGameTable(level, GameBox.Size);
            Table.SelectedTool = ConstructGameTable.Tool.Wall;
            TablePaint();
        }

        private void TablePaint()
        {
            Table.OnPaint((ScreenBox)GameBox);
        }

        private ConstructGameTable Table;
        private Size TblSize => new Size((int)HeightNumeric.Value, (int)WidthNumeric.Value);

        private void Numeric_ValueChanged(object sender, EventArgs e)
        {
            int sectorScale = (int)Math.Min((float)GameBox.Width / TblSize.Height, (float)GameBox.Height / TblSize.Width);
            Table.ChangeSize(TblSize, sectorScale);
            TablePaint();
        }
        private void Tool_CheckedChanged(object sender, EventArgs e)
        {
            ActivateBox.Enabled = DoorButton.Checked;
            ActivateBox.Checked = false;
            Dictionary<Tool, RadioButton> dict = new Dictionary<Tool, RadioButton>()
            {
                { Tool.Wall, WallButton },
                { Tool.Door, DoorButton },
                { Tool.Button, ButtonButton },
                { Tool.Enter, EnterButton },
                { Tool.Exit, ExitButton },
                { Tool.Analize, AnalizeButton }
            };
            SelectedTool = dict.Single(T => T.Value.Checked).Key;
            Table.SelectedTool = SelectedTool switch
            {
                Tool.Wall => ConstructGameTable.Tool.Wall,
                Tool.Door => ConstructGameTable.Tool.Door,
                Tool.Button => ConstructGameTable.Tool.Button,
                Tool.Enter => ConstructGameTable.Tool.Enter,
                Tool.Exit => ConstructGameTable.Tool.Exit,
                _ => ConstructGameTable.Tool.None,
            };
        }

        private enum Tool
        {
            Wall,
            Door,
            Button,
            Enter,
            Exit,
            Analize
        }
        private Tool SelectedTool = Tool.Wall;
        private (Point position, Direction direction, Direction passageDirection, (Direction vertical, Direction horizontal) quarter) AnalizeSector(Point imgLocation)
        {
            float sectorSize = Table.SectorScaleValue;
            int x = imgLocation.X / sectorSize.Round();
            int y = imgLocation.Y / sectorSize.Round();
            if (x >= TblSize.Height || y >= TblSize.Width)
                return (Point.Empty, Direction.None, Direction.None, (Direction.None, Direction.None));
            else
            {
                Point p = new Point(y, x);
                float propX = imgLocation.X % sectorSize / sectorSize;
                float propY = imgLocation.Y % sectorSize / sectorSize;
                Direction dir = (propX < propY, propX < 1 - propY) switch
                {
                    (false, true) => Direction.Up,
                    (false, false) => Direction.Right,
                    (true, false) => Direction.Down,
                    (true, true) => Direction.Left
                };
                (Direction q1, Direction q2) quarter = (propX < 0.5, propY < 0.5) switch
                {
                    (false, true) => (Direction.Up, Direction.Right),
                    (false, false) => (Direction.Down, Direction.Right),
                    (true, false) => (Direction.Down, Direction.Left),
                    (true, true) => (Direction.Up, Direction.Left),
                };
                return (p, dir, CheckPosition(p, dir), quarter);
            }
        }
        private Direction CheckPosition(Point position, Direction direction)
        {
            if (direction != Direction.None)
                direction = ((position.X == 0, position.Y == 0, position.X == TblSize.Width - 1, position.Y == TblSize.Height - 1) switch
                {
                    (true, true, false, false) => direction == Direction.Up || direction == Direction.Right ? Direction.Up : Direction.Left,
                    (true, false, false, false) => Direction.Up,
                    (true, false, false, true) => direction == Direction.Down || direction == Direction.Right ? Direction.Right : Direction.Up,
                    (false, false, false, true) => Direction.Right,
                    (false, false, true, true) => direction == Direction.Down || direction == Direction.Left ? Direction.Down : Direction.Right,
                    (false, false, true, false) => Direction.Down,
                    (false, true, true, false) => direction == Direction.Up || direction == Direction.Left ? Direction.Left : Direction.Down,
                    (false, true, false, false) => Direction.Left,
                    _ => Direction.None,
                });
            return direction;
        }

        private Point? DoorState = null;
        private Direction DoorOpen = Direction.None;
        private Direction DoorReset()
        {
            DoorState = null;
            Table.SelectedTool = ConstructGameTable.Tool.Door;
            return Direction.None;
        }

        private (Point position, Direction wall)? NewButton = null;

        private Point? ToolTipSector = null;

        private void GameBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (SelectedTool == Tool.Door && !ActivateBox.Checked)
            {
                DoorState = AnalizeSector(e.Location).position;
                Table.SelectedTool = ConstructGameTable.Tool.DoorDirection;
            }
        }

        private void GameBox_MouseMove(object sender, MouseEventArgs e)
        {
            var (position, direction, passageDirection, quarter) = AnalizeSector(e.Location);
            if (AnalizeTip.Active && ToolTipSector != position)
                AnalizeTip.Hide(GameBox);
            if (DoorState != null)
            {
                DoorOpen = (position.X - DoorState.Value.X, position.Y - DoorState.Value.Y) switch
                {
                    (0, 0) => Direction.None,
                    (-1, 0) => Direction.Up,
                    (0, 1) => Direction.Right,
                    (1, 0) => Direction.Down,
                    (0, -1) => Direction.Left,
                    _ => DoorReset(),
                };
            }
            if ((Table.Position, Table.Direction, Table.Quarter) != (position, direction, quarter))
            {
                (Table.Position, Table.Direction, Table.Quarter) = (position, direction, quarter);
                TablePaint();
            }
            SectorUnderMouseStatus.Text = $"Sector {{{position.Y}, {position.X}}}";
            ToolStatus.Text = SelectedTool switch
            {
                Tool.Wall => direction.ToString(),
                Tool.Door => ActivateBox.Checked ? "Select door"
                                                 : ((DoorState?.ToString() ?? "Select sector")
                                                 + (DoorOpen != Direction.None ? " " + DoorOpen.ToString() : "")),
                Tool.Button => NewButton.HasValue ? "Select door" : "Select sector",
                Tool.Enter => passageDirection == Direction.None ? "No good for Enter" : passageDirection.ToString(),
                Tool.Exit => passageDirection == Direction.None ? "No good for Exit" : passageDirection.ToString(),
                Tool.Analize => $"direction {direction}, quarter: {{{quarter.vertical}, {quarter.horizontal}}}",
                _ => ""
            };
        }

        private void GameBox_MouseUp(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(counter.ToString());
            var (position, direction, passageDirection, quarter) = AnalizeSector(e.Location);
            switch (SelectedTool)
            {
                case Tool.Wall:
                    Table.ChangeSector(position, direction);
                    break;
                case Tool.Door:
                    if (DoorState != null)
                    {
                        if (DoorOpen == Direction.None)
                            Table.RemoveDoor(DoorState.Value, direction, quarter);
                        else
                        {
                            Direction wall = DoorOpen switch
                            {
                                Direction.Up => quarter.horizontal,
                                Direction.Right => quarter.vertical,
                                Direction.Down => quarter.horizontal,
                                Direction.Left => quarter.vertical,
                                _ => Direction.None,
                            };
                            Table.AddDoor(DoorState.Value, wall, DoorOpen);
                        }
                        DoorOpen = DoorReset();
                    }
                    else if (ActivateBox.Checked)
                    {
                        Table.DoorActivate(position, direction, quarter);
                    }
                    break;
                case Tool.Button:
                    if (NewButton.HasValue)
                    {
                        if (Table.AddButton(NewButton.Value, (position, direction, quarter)))
                        {
                            NewButton = null;
                            Table.SelectedTool = ConstructGameTable.Tool.Button;
                        }
                    }
                    else
                    {
                        if (!Table.RemoveButton(position))
                        {
                            Direction buttonWall = Table.GetButtonWall(position, direction, quarter);
                            if (buttonWall == Direction.None)
                                ToolStatus.Text = "No wall for button";
                            else
                            {
                                NewButton = (position, buttonWall);
                                Table.SelectedTool = ConstructGameTable.Tool.ButtonDoor;
                            }
                        }
                    }
                    break;
                case Tool.Enter:
                    if (passageDirection != Direction.None)
                        EnterCheck.Checked = Table.CreatePassage((position, passageDirection), false).GetValueOrDefault(EnterCheck.Checked);
                    break;
                case Tool.Exit:
                    if (passageDirection != Direction.None)
                        ExitCheck.Checked = Table.CreatePassage((position, passageDirection), true).GetValueOrDefault(ExitCheck.Checked);
                    break;
                case Tool.Analize:
                    ToolTipSector = position;
                    AnalizeTip.Show(string.Join("\n", Table.GetSectorInfo(position)), GameBox);
                    break;
            }
            TablePaint();
            CheckSaveConditions();
        }
        private int counter = 0;
        private bool IsPassable = false;
        private void TestButton_Click(object sender, EventArgs e)
        {
            if (!Table.Started)
            {
                Table.InitStart(new Player.PlayerArgs(20), (ScreenBox)GameBox);
                TestButton.Enabled = false;
                SaveButton.Enabled = false;

                Form.KeyDown += ConstructorForm_KeyDown;
                Form.KeyUp += ConstructorForm_KeyUp;

                Table.OnGameOver += TestGame_GameOver;
            }
            else
            {
                Table.Deinit();

                Table.OnGameOver -= TestGame_GameOver;

                Form.KeyDown -= ConstructorForm_KeyDown;
                Form.KeyUp -= ConstructorForm_KeyUp;

                CheckSaveConditions();
                TablePaint();
            }
            WidthNumeric.Enabled = !WidthNumeric.Enabled;
            HeightNumeric.Enabled = !HeightNumeric.Enabled;

            WallButton.Enabled = !WallButton.Enabled;
            DoorButton.Enabled = !DoorButton.Enabled;
            ButtonButton.Enabled = !ButtonButton.Enabled;
            EnterButton.Enabled = !EnterButton.Enabled;
            ExitButton.Enabled = !ExitButton.Enabled;
            AnalizeButton.Enabled = !AnalizeButton.Enabled;

            NameBox.Enabled = !NameBox.Enabled;
            CloseButton.Enabled = !CloseButton.Enabled;
            counter++;
        }

        private void ConstructorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (Table.Started)
            {
                if (e.KeyCode == Keys.D)
                    Table.Player.Speed.horizontal = Table.PParametrs.MaxSpeed;
                else if (e.KeyCode == Keys.A)
                    Table.Player.Speed.horizontal = -Table.PParametrs.MaxSpeed;
                if (e.KeyCode == Keys.Space && Table.Player.IsGrounded)
                    Table.Player.Jump(Table.PParametrs.JumpPower);
            }
        }
        private void ConstructorForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (Table.Started)
            {
                Table.Player.Speed.horizontal = 0;
                if (e.KeyCode == Keys.Escape)
                    TestButton_Click(sender, e);
            }
        }
        private void TestGame_GameOver(object sender, EventArgs e)
        {
            IsPassable = true;
            TestButton_Click(sender, e);
        }

        private void CheckSaveConditions()
        {
            TestButton.Enabled = EnterCheck.Checked && ExitCheck.Checked;
            SaveButton.Enabled = TestButton.Enabled && IsPassable && string.IsNullOrWhiteSpace(NameBox.Text);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Table.SaveLevel(NameBox.Text);
            CloseButton_Click(sender, e);
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Table = null;
            Form.ReturnToMenu();
        }
    }
}