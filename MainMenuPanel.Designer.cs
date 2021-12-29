namespace Platformer
{
    partial class MainMenuPanel
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
            this.ConstructorButton = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.Button();
            this.Settings = new System.Windows.Forms.Button();
            this.LoadGame = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NewGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConstructorButton
            // 
            this.ConstructorButton.Location = new System.Drawing.Point(61, 121);
            this.ConstructorButton.Name = "ConstructorButton";
            this.ConstructorButton.Size = new System.Drawing.Size(110, 23);
            this.ConstructorButton.TabIndex = 11;
            this.ConstructorButton.Text = "Создать уровень";
            this.ConstructorButton.UseVisualStyleBackColor = true;
            this.ConstructorButton.Click += new System.EventHandler(this.ConstructorButton_Click);
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(61, 183);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(110, 25);
            this.Exit.TabIndex = 10;
            this.Exit.Text = "Выход";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Settings
            // 
            this.Settings.Location = new System.Drawing.Point(61, 151);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(110, 25);
            this.Settings.TabIndex = 9;
            this.Settings.Text = "Настройки";
            this.Settings.UseVisualStyleBackColor = true;
            this.Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // LoadGame
            // 
            this.LoadGame.Location = new System.Drawing.Point(61, 89);
            this.LoadGame.Name = "LoadGame";
            this.LoadGame.Size = new System.Drawing.Size(110, 25);
            this.LoadGame.TabIndex = 8;
            this.LoadGame.Text = "Загрузить";
            this.LoadGame.UseVisualStyleBackColor = true;
            this.LoadGame.Click += new System.EventHandler(this.LoadGame_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label1.Location = new System.Drawing.Point(66, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "Platformer";
            // 
            // NewGame
            // 
            this.NewGame.Location = new System.Drawing.Point(61, 57);
            this.NewGame.Name = "NewGame";
            this.NewGame.Size = new System.Drawing.Size(110, 25);
            this.NewGame.TabIndex = 6;
            this.NewGame.Text = "Новая игра";
            this.NewGame.UseVisualStyleBackColor = true;
            this.NewGame.Click += new System.EventHandler(this.NewGame_Click);
            // 
            // MainMenuPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ConstructorButton);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.Settings);
            this.Controls.Add(this.LoadGame);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewGame);
            this.Name = "MainMenuPanel";
            this.Size = new System.Drawing.Size(233, 242);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConstructorButton;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button Settings;
        private System.Windows.Forms.Button LoadGame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button NewGame;
    }
}
