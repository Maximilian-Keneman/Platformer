using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public partial class MainForm : Form
    {
        private MainMenuPanel MainMenu;
        private ProfilesPanel ProfilesPanel;
        private ConstructorForm Constructor;
        private LevelsSelector Selector;
        private PictureBox GameBox;
        private enum FormState
        {
            Menu,
            SelectLevel,
            Game,
            Construct
        }
        private FormState state;
        private FormState State
        {
            get => state;
            set
            {
                state = value;
                (MainMenu.Visible, Constructor.Visible, Selector.Visible) =
                (MainMenu.Enabled, Constructor.Enabled, Selector.Enabled) =
                state switch
                {
                    FormState.Menu => (true, false, false),
                    FormState.SelectLevel => (false, false, true),
                    FormState.Game => (false, false, false),
                    FormState.Construct => (false, true, false),
                };
                if (state == FormState.Game)
                    ProfilesPanel.Block(true);
                else
                    ProfilesPanel.Block(false);
            }
        }

        private GameTable Game;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ProfilesList = ProfilesList.Load();
            MainMenu = new MainMenuPanel { Name = "MainMenu" };
            Selector = new LevelsSelector { Name = "Selector" };
            Constructor = new ConstructorForm { Name = "Constructor" };
            ProfilesPanel = new ProfilesPanel { Name = "ProfilesPanel" };
            Controls.AddRange(new Control[] { Constructor, MainMenu, ProfilesPanel, Selector });
            ProfilesPanel.Init();
            State = FormState.Menu;
            DebugTimer.Start();
        }
        private void ToCenter(Control control)
        {
            control.Location = new Point((Width - control.Width) / 2, (Height - control.Height) / 2);
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            ToCenter(MainMenu);
            ToCenter(Constructor);
            ToCenter(Selector);
            ProfilesPanel.Location = new Point(Right - ProfilesPanel.Width, (Height - ProfilesPanel.Height) / 2);
        }

        public void SelectLevel()
        {
            State = FormState.SelectLevel;
        }
        public void StartNewGame(Level level)
        {
            State = FormState.Game;
            GameBox = new PictureBox();

            //Level level = Level.Load("MyLevels\\Tut_1.lvdat");

            Player.PlayerArgs player = new Player.PlayerArgs(20);
            Game = new GameTable(level, player, Size, (ScreenBox)GameBox);
            Game.OnGameOver += Game_GameOver;

            GameBox.Size = Game.ImgSize;
            ToCenter(GameBox);
            Controls.Add(GameBox);

            Game.GameStart();
        }
        public void ContinueGame()
        {
            State = FormState.Game;
            Game.GameContinue();
        }
        public void OpenConstructor()
        {
            Constructor.Init();
            State = FormState.Construct;
        }
        public void ReturnToMenu()
        {
            State = FormState.Menu;
        }

        private string Profile { get; set; }
        public ProfilesList ProfilesList { get; private set; }
        private void SaveLevel(Guid level)
        {
            if (Profile != null)
            {
                ProfilesList[Profile].CompleteLevel(level);
                ProfilesList.Save();
            }
        }
        public void LoadProfile(string name)
        {
            Profile = name;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (State == FormState.Game && Game?.Started == true)
            {
                if (e.KeyCode == Keys.D)
                    Game.Player.Speed.horizontal = Game.PParametrs.MaxSpeed;
                else if (e.KeyCode == Keys.A)
                    Game.Player.Speed.horizontal = -Game.PParametrs.MaxSpeed;
                if (e.KeyCode == Keys.Space && Game.Player.IsGrounded)
                    Game.Player.Jump(Game.PParametrs.JumpPower);
            }
        }
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (State == FormState.Game && Game?.Started == true)
            {
                Game.Player.Speed.horizontal = 0;
                if (e.KeyCode == Keys.Escape)
                {
                    State = FormState.Menu;
                    Game.GamePause();
                }
            }
        }
        private void Game_GameOver(object sender, EventArgs e)
        {
            SaveLevel(Game.Level);
            Controls.Remove(GameBox);
            State = FormState.Menu;
            GameBox = null;
            Game = null;
        }
        private void DebugTimer_Tick(object sender, EventArgs e)
        {
            DebugInfo.Text = $"Box\n{Debug()}\nGame\n{Game?.Debug() ?? ""}\nPlayer\n{Game?.Player.Debug(Game) ?? ""}";
        }
        private string Debug()
        {
            (int up, int right, int down, int left)? intervals = null;
            if (GameBox != null)
            {
                Rectangle rect = GameBox.Bounds;
                intervals = (rect.Top - Top, Right - rect.Right, Bottom - rect.Bottom, rect.Left - Left);
            }
            if (intervals.HasValue)
            {
                var (up, right, down, left) = intervals.Value;
                return $"Up {up}, Down {down}\nLeft {left}, Right {right}";
            }
            else
                return "";
        }

        private void DebugModeButton_Click(object sender, EventArgs e)
        {
            DebugInfo.Visible = !DebugInfo.Visible;
            if (DebugInfo.Visible)
                DebugModeButton.BackColor = SystemColors.ActiveCaption;
            else
                DebugModeButton.BackColor = SystemColors.Control;
        }

        private void DebugModeButton_MouseEnter(object sender, EventArgs e)
        {
            DebugModeButton.BorderStyle = BorderStyle.FixedSingle;
        }

        private void DebugModeButton_MouseLeave(object sender, EventArgs e)
        {
            DebugModeButton.BorderStyle = BorderStyle.None;
        }
    }

    public class ScreenBox : IScreen
    {
        private ScreenBox(PictureBox box) => Box = box;
        private PictureBox Box;
        public Image Image => Image;
        public void UpdateImage(Image img)
        {
            Box.Image = img.Clone() as Image;
        }

        public static explicit operator ScreenBox(PictureBox box) => new ScreenBox(box);
    }
}