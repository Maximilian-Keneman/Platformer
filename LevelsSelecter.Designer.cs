namespace Platformer
{
    partial class LevelsSelector
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Tutorial levels", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("My levels", System.Windows.Forms.HorizontalAlignment.Left);
            this.LevelsListView = new System.Windows.Forms.ListView();
            this.CancelSelectButton = new System.Windows.Forms.Button();
            this.OpenMyLevelsDirectoryButton = new System.Windows.Forms.Button();
            this.LevelsWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.LevelsWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // LevelsListView
            // 
            listViewGroup1.Header = "Tutorial levels";
            listViewGroup1.Name = "TutorialLevelsGroup";
            listViewGroup2.Header = "My levels";
            listViewGroup2.Name = "MyLevelsGroup";
            this.LevelsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.LevelsListView.HideSelection = false;
            this.LevelsListView.Location = new System.Drawing.Point(3, 3);
            this.LevelsListView.Name = "LevelsListView";
            this.LevelsListView.Size = new System.Drawing.Size(519, 195);
            this.LevelsListView.TabIndex = 2;
            this.LevelsListView.UseCompatibleStateImageBehavior = false;
            this.LevelsListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseDoubleClick);
            // 
            // CancelSelectButton
            // 
            this.CancelSelectButton.Location = new System.Drawing.Point(3, 204);
            this.CancelSelectButton.Name = "CancelSelectButton";
            this.CancelSelectButton.Size = new System.Drawing.Size(75, 23);
            this.CancelSelectButton.TabIndex = 3;
            this.CancelSelectButton.Text = "Отмена";
            this.CancelSelectButton.UseVisualStyleBackColor = true;
            this.CancelSelectButton.Click += new System.EventHandler(this.CancelSelectButton_Click);
            // 
            // OpenMyLevelsDirectoryButton
            // 
            this.OpenMyLevelsDirectoryButton.Location = new System.Drawing.Point(326, 204);
            this.OpenMyLevelsDirectoryButton.Name = "OpenMyLevelsDirectoryButton";
            this.OpenMyLevelsDirectoryButton.Size = new System.Drawing.Size(196, 23);
            this.OpenMyLevelsDirectoryButton.TabIndex = 4;
            this.OpenMyLevelsDirectoryButton.Text = "Открыть папку с Моими уровнями";
            this.OpenMyLevelsDirectoryButton.UseVisualStyleBackColor = true;
            this.OpenMyLevelsDirectoryButton.Click += new System.EventHandler(this.OpenMyLevelsDirectoryButton_Click);
            // 
            // LevelsWatcher
            // 
            this.LevelsWatcher.EnableRaisingEvents = true;
            this.LevelsWatcher.Filter = "*.lvdat";
            this.LevelsWatcher.Path = "MyLevels";
            this.LevelsWatcher.SynchronizingObject = this;
            this.LevelsWatcher.Changed += new System.IO.FileSystemEventHandler(this.LevelsWatcher_Changed);
            // 
            // LevelsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OpenMyLevelsDirectoryButton);
            this.Controls.Add(this.CancelSelectButton);
            this.Controls.Add(this.LevelsListView);
            this.Name = "LevelsSelector";
            this.Size = new System.Drawing.Size(525, 242);
            ((System.ComponentModel.ISupportInitialize)(this.LevelsWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LevelsListView;
        private System.Windows.Forms.Button CancelSelectButton;
        private System.Windows.Forms.Button OpenMyLevelsDirectoryButton;
        private System.IO.FileSystemWatcher LevelsWatcher;
    }
}
