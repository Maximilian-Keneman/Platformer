using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Platformer
{
    public partial class ProfilesPanel : UserControl
    {
        private MainForm Form => FindForm() as MainForm;

        public ProfilesPanel()
        {
            InitializeComponent();
        }
        public void Init()
        {
            ProfilesList.Items.AddRange(Form.ProfilesList.GetNames);
        }

        public void Block(bool state)
        {
            if (state)
            {
                Close();
                ProfileLabel.Click -= ProfileLabel_Click;
            }
            else
                ProfileLabel.Click += ProfileLabel_Click;
        }

        private void ProfileLabel_Click(object sender, EventArgs e)
        {
            ProfileLabel.BorderStyle = BorderStyle.FixedSingle;
            ProfileLabel.TextAlign = ContentAlignment.MiddleCenter;
            ProfilesList.Visible = true;
            VisitorButton.Visible = true;
            AcceptProfileButton.Visible = true;
            CancelChangeButton.Visible = true;
        }

        private void VisitorButton_Click(object sender, EventArgs e)
        {
            ProfileLabel.Text = "Гость";
            Form.LoadProfile(null);
            Close();
        }

        private void ProfilesList_TextChanged(object sender, EventArgs e)
        {
            AcceptProfileButton.Text = ProfilesList.Items.Contains(ProfilesList.Text) ? "Выбрать" : "Создать";
            AcceptProfileButton.Enabled = !string.IsNullOrWhiteSpace(ProfilesList.Text);
        }

        private void CancelChangeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Close()
        {
            ProfileLabel.BorderStyle = BorderStyle.None;
            ProfileLabel.TextAlign = ContentAlignment.MiddleRight;
            ProfilesList.Visible = false;
            ProfilesList.Text = "";
            VisitorButton.Visible = false;
            AcceptProfileButton.Visible = false;
            CancelChangeButton.Visible = false;
        }

        private void AcceptProfileButton_Click(object sender, EventArgs e)
        {
            if (!ProfilesList.Items.Contains(ProfilesList.Text))
            {
                Profile profile = new Profile(ProfilesList.Text);
                Form.ProfilesList.NewProfile(profile);
                ProfilesList.Items.Add(ProfilesList.Text);
            }
            ProfileLabel.Text = ProfilesList.Text;
            Form.LoadProfile(ProfilesList.Text);
            Close();
        }
    }

    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class Profile : ISerializable
    {
        public string Name { get; }
        private List<Guid> CompletedLevels;
        public void CompleteLevel(Guid level)
        {
            if (!CompletedLevels.Contains(level))
                CompletedLevels.Add(level);
        }

        public Profile(string name)
        {
            Name = name;
            CompletedLevels = new List<Guid>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            for (int i = 0; i < CompletedLevels.Count; i++)
                info.AddValue("Level" + i, CompletedLevels[i].ToString());
        }
        private Profile(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            CompletedLevels = new List<Guid>();
            int i = 0;
            while (true)
            {
                try
                {
                    CompletedLevels.Add(Guid.Parse(info.GetString("Level" + i)));
                    i++;
                }
                catch (SerializationException)
                {
                    break;
                }
            }
        }
    }

    [Serializable]
    [DebuggerDisplay("Count = {Profiles.Count}")]
    public class ProfilesList : ISerializable
    {
        private Dictionary<string, Profile> Profiles;
        public Profile this[string key]
        {
            get => Profiles[key];
            set => Profiles[key] = value;
        }

        private ProfilesList()
        {
            Profiles = new Dictionary<string, Profile>();
            Save();
        }
        public static ProfilesList Load()
        {
            //_ = new ProfilesList();
            BinaryFormatter F = new BinaryFormatter();
            using (FileStream fs = new FileStream("profiles.dat", FileMode.OpenOrCreate, FileAccess.Read))
                return (ProfilesList)F.Deserialize(fs);
        }

        public string[] GetNames => Profiles.Select(V => V.Key).ToArray();
        public void NewProfile(Profile profile)
        {
            Profiles.Add(profile.Name, profile);
            Save();
        }
        public void DeleteProfile(string name)
        {
            Profiles.Remove(name);
            Save();
        }
        public void Save()
        {
            BinaryFormatter F = new BinaryFormatter();
            using (FileStream fs = new FileStream("profiles.dat", FileMode.OpenOrCreate, FileAccess.Write))
                F.Serialize(fs, this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Count", Profiles.Count);
            int i = 0;
            foreach (Profile profile in Profiles.Select(V => V.Value))
            {
                info.AddValue("Profile" + i, profile);
                i++;
            }
                
        }
        private ProfilesList(SerializationInfo info, StreamingContext context)
        {
            int count = info.GetInt32("Count");
            List<Profile> profiles = new List<Profile>(count);
            for (int i = 0; i < count; i++)
                profiles.Add(info.GetValue("Profile" + i, typeof(Profile)) as Profile);
            Profiles = profiles.ToDictionary(P => P.Name);
        }
    }
}
