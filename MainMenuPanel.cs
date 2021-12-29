using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public partial class MainMenuPanel : UserControl
    {
        private MainForm Form => FindForm() as MainForm;

        public MainMenuPanel()
        {
            InitializeComponent();
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            Form.SelectLevel();
        }
        private void LoadGame_Click(object sender, EventArgs e)
        {
            
        }
        private void ConstructorButton_Click(object sender, EventArgs e)
        {
            Form.OpenConstructor();
        }
        private void Settings_Click(object sender, EventArgs e)
        {

        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Form.Close();
        }
    }
}
