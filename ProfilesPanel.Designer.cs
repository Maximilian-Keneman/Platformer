namespace Platformer
{
    partial class ProfilesPanel
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
            this.ProfilesList = new System.Windows.Forms.ComboBox();
            this.AcceptProfileButton = new System.Windows.Forms.Button();
            this.VisitorButton = new System.Windows.Forms.Button();
            this.CancelChangeButton = new System.Windows.Forms.Button();
            this.ProfileLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProfilesList
            // 
            this.ProfilesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfilesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.ProfilesList.FormattingEnabled = true;
            this.ProfilesList.Location = new System.Drawing.Point(0, 25);
            this.ProfilesList.Name = "ProfilesList";
            this.ProfilesList.Size = new System.Drawing.Size(243, 215);
            this.ProfilesList.TabIndex = 1;
            this.ProfilesList.Visible = false;
            this.ProfilesList.TextChanged += new System.EventHandler(this.ProfilesList_TextChanged);
            // 
            // AcceptProfileButton
            // 
            this.AcceptProfileButton.Enabled = false;
            this.AcceptProfileButton.Location = new System.Drawing.Point(84, 242);
            this.AcceptProfileButton.Name = "AcceptProfileButton";
            this.AcceptProfileButton.Size = new System.Drawing.Size(75, 23);
            this.AcceptProfileButton.TabIndex = 3;
            this.AcceptProfileButton.Text = "Создать";
            this.AcceptProfileButton.UseVisualStyleBackColor = true;
            this.AcceptProfileButton.Visible = false;
            this.AcceptProfileButton.Click += new System.EventHandler(this.AcceptProfileButton_Click);
            // 
            // VisitorButton
            // 
            this.VisitorButton.Location = new System.Drawing.Point(3, 242);
            this.VisitorButton.Name = "VisitorButton";
            this.VisitorButton.Size = new System.Drawing.Size(75, 23);
            this.VisitorButton.TabIndex = 2;
            this.VisitorButton.Text = "Гость";
            this.VisitorButton.UseVisualStyleBackColor = true;
            this.VisitorButton.Visible = false;
            this.VisitorButton.Click += new System.EventHandler(this.VisitorButton_Click);
            // 
            // CancelChangeButton
            // 
            this.CancelChangeButton.Location = new System.Drawing.Point(165, 242);
            this.CancelChangeButton.Name = "CancelChangeButton";
            this.CancelChangeButton.Size = new System.Drawing.Size(75, 23);
            this.CancelChangeButton.TabIndex = 4;
            this.CancelChangeButton.Text = "Отмена";
            this.CancelChangeButton.UseVisualStyleBackColor = true;
            this.CancelChangeButton.Visible = false;
            this.CancelChangeButton.Click += new System.EventHandler(this.CancelChangeButton_Click);
            // 
            // ProfileLabel
            // 
            this.ProfileLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProfileLabel.Location = new System.Drawing.Point(0, 0);
            this.ProfileLabel.Name = "ProfileLabel";
            this.ProfileLabel.Size = new System.Drawing.Size(243, 25);
            this.ProfileLabel.TabIndex = 4;
            this.ProfileLabel.Text = "Гость";
            this.ProfileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProfileLabel.Click += new System.EventHandler(this.ProfileLabel_Click);
            // 
            // ProfilesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProfilesList);
            this.Controls.Add(this.ProfileLabel);
            this.Controls.Add(this.CancelChangeButton);
            this.Controls.Add(this.VisitorButton);
            this.Controls.Add(this.AcceptProfileButton);
            this.Name = "ProfilesPanel";
            this.Size = new System.Drawing.Size(243, 268);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ProfilesList;
        private System.Windows.Forms.Button AcceptProfileButton;
        private System.Windows.Forms.Button VisitorButton;
        private System.Windows.Forms.Button CancelChangeButton;
        private System.Windows.Forms.Label ProfileLabel;
    }
}
