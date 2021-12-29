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
using System.Diagnostics;

namespace Platformer
{
    public partial class LevelsSelector : UserControl
    {
        private MainForm Form => FindForm() as MainForm;
        public LevelsSelector()
        {
            InitializeComponent();
            Levels = new Dictionary<Guid, (Level level, string group)>();
            AddLevel("Tutorial 1", BaseLevels.Tutorial_1, LevelsListView.Groups["TutorialLevelsGroup"]);
            FullMyLevels();
        }
        private void FullMyLevels()
        {
            var levels = Directory.GetFiles("MyLevels", "*.lvdat").Select(p => (p.GetFileName(), Level.Load(p)));
            foreach (var (name, level) in levels)
                AddLevel(name, level, LevelsListView.Groups["MyLevelsGroup"]);
        }

        private Dictionary<Guid, (Level level, string group)> Levels;
        private void AddLevel(string name, Level level, ListViewGroup group)
        {
            Levels.Add(level.GUID, (level, group.Name));
            LevelsListView.Items.Add(new ListViewItem(new string[] { name, level.GUID.ToString() }, group));
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = LevelsListView.GetItemAt(e.X, e.Y);
            Form.StartNewGame(Levels[Guid.Parse(item.SubItems[1].Text)].level);
        }

        private void CancelSelectButton_Click(object sender, EventArgs e)
        {
            Form.ReturnToMenu();
        }

        private void LevelsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LevelsListView.Groups["MyLevelsGroup"].Items.Clear();
            foreach (var guid in Levels.Where(L => L.Value.group == "MyLevelsGroup").Select(L => L.Key))
                Levels.Remove(guid);
            FullMyLevels();
        }

        private void OpenMyLevelsDirectoryButton_Click(object sender, EventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + "\\MyLevels");
        }
    }
}
