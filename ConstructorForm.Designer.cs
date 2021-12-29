namespace Platformer
{
    partial class ConstructorForm
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
            this.components = new System.ComponentModel.Container();
            this.GameBox = new System.Windows.Forms.PictureBox();
            this.ControlsPanel = new System.Windows.Forms.Panel();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.TestButton = new System.Windows.Forms.Button();
            this.AnalizeButton = new System.Windows.Forms.RadioButton();
            this.ActivateBox = new System.Windows.Forms.CheckBox();
            this.ExitButton = new System.Windows.Forms.RadioButton();
            this.EnterButton = new System.Windows.Forms.RadioButton();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ExitCheck = new System.Windows.Forms.CheckBox();
            this.EnterCheck = new System.Windows.Forms.CheckBox();
            this.ButtonButton = new System.Windows.Forms.RadioButton();
            this.DoorButton = new System.Windows.Forms.RadioButton();
            this.WallButton = new System.Windows.Forms.RadioButton();
            this.HeightNumeric = new System.Windows.Forms.NumericUpDown();
            this.WidthNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SectorUnderMouseStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.AnalizeTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.GameBox)).BeginInit();
            this.ControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthNumeric)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameBox
            // 
            this.GameBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameBox.Location = new System.Drawing.Point(0, 0);
            this.GameBox.Name = "GameBox";
            this.GameBox.Size = new System.Drawing.Size(573, 399);
            this.GameBox.TabIndex = 0;
            this.GameBox.TabStop = false;
            this.GameBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameBox_MouseDown);
            this.GameBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameBox_MouseMove);
            this.GameBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GameBox_MouseUp);
            // 
            // ControlsPanel
            // 
            this.ControlsPanel.Controls.Add(this.NameBox);
            this.ControlsPanel.Controls.Add(this.HeightLabel);
            this.ControlsPanel.Controls.Add(this.WidthLabel);
            this.ControlsPanel.Controls.Add(this.TestButton);
            this.ControlsPanel.Controls.Add(this.AnalizeButton);
            this.ControlsPanel.Controls.Add(this.ActivateBox);
            this.ControlsPanel.Controls.Add(this.ExitButton);
            this.ControlsPanel.Controls.Add(this.EnterButton);
            this.ControlsPanel.Controls.Add(this.CloseButton);
            this.ControlsPanel.Controls.Add(this.SaveButton);
            this.ControlsPanel.Controls.Add(this.ExitCheck);
            this.ControlsPanel.Controls.Add(this.EnterCheck);
            this.ControlsPanel.Controls.Add(this.ButtonButton);
            this.ControlsPanel.Controls.Add(this.DoorButton);
            this.ControlsPanel.Controls.Add(this.WallButton);
            this.ControlsPanel.Controls.Add(this.HeightNumeric);
            this.ControlsPanel.Controls.Add(this.WidthNumeric);
            this.ControlsPanel.Controls.Add(this.label1);
            this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ControlsPanel.Location = new System.Drawing.Point(573, 0);
            this.ControlsPanel.Name = "ControlsPanel";
            this.ControlsPanel.Size = new System.Drawing.Size(211, 421);
            this.ControlsPanel.TabIndex = 1;
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(3, 354);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(75, 20);
            this.NameBox.TabIndex = 19;
            this.NameBox.Text = "Name";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(57, 42);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(45, 13);
            this.HeightLabel.TabIndex = 18;
            this.HeightLabel.Text = "Высота";
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(6, 42);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(46, 13);
            this.WidthLabel.TabIndex = 17;
            this.WidthLabel.Text = "Ширина";
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(47, 316);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(92, 23);
            this.TestButton.TabIndex = 16;
            this.TestButton.Text = "Тестировать";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // AnalizeButton
            // 
            this.AnalizeButton.AutoSize = true;
            this.AnalizeButton.Location = new System.Drawing.Point(33, 244);
            this.AnalizeButton.Name = "AnalizeButton";
            this.AnalizeButton.Size = new System.Drawing.Size(106, 17);
            this.AnalizeButton.TabIndex = 15;
            this.AnalizeButton.TabStop = true;
            this.AnalizeButton.Text = "Анализ сектора";
            this.AnalizeButton.UseVisualStyleBackColor = true;
            // 
            // ActivateBox
            // 
            this.ActivateBox.AutoSize = true;
            this.ActivateBox.Enabled = false;
            this.ActivateBox.Location = new System.Drawing.Point(96, 152);
            this.ActivateBox.Name = "ActivateBox";
            this.ActivateBox.Size = new System.Drawing.Size(97, 17);
            this.ActivateBox.TabIndex = 14;
            this.ActivateBox.Text = "Активировать";
            this.ActivateBox.UseVisualStyleBackColor = true;
            // 
            // ExitButton
            // 
            this.ExitButton.AutoSize = true;
            this.ExitButton.Location = new System.Drawing.Point(33, 221);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(57, 17);
            this.ExitButton.TabIndex = 13;
            this.ExitButton.TabStop = true;
            this.ExitButton.Text = "Выход";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.CheckedChanged += new System.EventHandler(this.Tool_CheckedChanged);
            // 
            // EnterButton
            // 
            this.EnterButton.AutoSize = true;
            this.EnterButton.Location = new System.Drawing.Point(33, 197);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(49, 17);
            this.EnterButton.TabIndex = 12;
            this.EnterButton.TabStop = true;
            this.EnterButton.Text = "Вход";
            this.EnterButton.UseVisualStyleBackColor = true;
            this.EnterButton.CheckedChanged += new System.EventHandler(this.Tool_CheckedChanged);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(133, 380);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 9;
            this.CloseButton.Text = "Отмена";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Enabled = false;
            this.SaveButton.Location = new System.Drawing.Point(3, 380);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Сохранить";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ExitCheck
            // 
            this.ExitCheck.AutoSize = true;
            this.ExitCheck.Enabled = false;
            this.ExitCheck.Location = new System.Drawing.Point(96, 222);
            this.ExitCheck.Name = "ExitCheck";
            this.ExitCheck.Size = new System.Drawing.Size(58, 17);
            this.ExitCheck.TabIndex = 11;
            this.ExitCheck.Text = "Выход";
            this.ExitCheck.UseVisualStyleBackColor = true;
            // 
            // EnterCheck
            // 
            this.EnterCheck.AutoSize = true;
            this.EnterCheck.Enabled = false;
            this.EnterCheck.Location = new System.Drawing.Point(96, 198);
            this.EnterCheck.Name = "EnterCheck";
            this.EnterCheck.Size = new System.Drawing.Size(50, 17);
            this.EnterCheck.TabIndex = 10;
            this.EnterCheck.Text = "Вход";
            this.EnterCheck.UseVisualStyleBackColor = true;
            // 
            // ButtonButton
            // 
            this.ButtonButton.AutoSize = true;
            this.ButtonButton.Location = new System.Drawing.Point(33, 174);
            this.ButtonButton.Name = "ButtonButton";
            this.ButtonButton.Size = new System.Drawing.Size(62, 17);
            this.ButtonButton.TabIndex = 5;
            this.ButtonButton.Text = "Кнопка";
            this.ButtonButton.UseVisualStyleBackColor = true;
            this.ButtonButton.CheckedChanged += new System.EventHandler(this.Tool_CheckedChanged);
            // 
            // DoorButton
            // 
            this.DoorButton.AutoSize = true;
            this.DoorButton.Location = new System.Drawing.Point(33, 151);
            this.DoorButton.Name = "DoorButton";
            this.DoorButton.Size = new System.Drawing.Size(58, 17);
            this.DoorButton.TabIndex = 4;
            this.DoorButton.Text = "Дверь";
            this.DoorButton.UseVisualStyleBackColor = true;
            this.DoorButton.CheckedChanged += new System.EventHandler(this.Tool_CheckedChanged);
            // 
            // WallButton
            // 
            this.WallButton.AutoSize = true;
            this.WallButton.Checked = true;
            this.WallButton.Location = new System.Drawing.Point(33, 128);
            this.WallButton.Name = "WallButton";
            this.WallButton.Size = new System.Drawing.Size(62, 17);
            this.WallButton.TabIndex = 3;
            this.WallButton.TabStop = true;
            this.WallButton.Text = "Проход";
            this.WallButton.UseVisualStyleBackColor = true;
            this.WallButton.CheckedChanged += new System.EventHandler(this.Tool_CheckedChanged);
            // 
            // HeightNumeric
            // 
            this.HeightNumeric.Location = new System.Drawing.Point(60, 58);
            this.HeightNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightNumeric.Name = "HeightNumeric";
            this.HeightNumeric.Size = new System.Drawing.Size(47, 20);
            this.HeightNumeric.TabIndex = 2;
            this.HeightNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightNumeric.ValueChanged += new System.EventHandler(this.Numeric_ValueChanged);
            // 
            // WidthNumeric
            // 
            this.WidthNumeric.Location = new System.Drawing.Point(9, 58);
            this.WidthNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthNumeric.Name = "WidthNumeric";
            this.WidthNumeric.Size = new System.Drawing.Size(45, 20);
            this.WidthNumeric.TabIndex = 1;
            this.WidthNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthNumeric.ValueChanged += new System.EventHandler(this.Numeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Размер";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SectorUnderMouseStatus,
            this.ToolStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 399);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(573, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SectorUnderMouseStatus
            // 
            this.SectorUnderMouseStatus.Name = "SectorUnderMouseStatus";
            this.SectorUnderMouseStatus.Size = new System.Drawing.Size(81, 17);
            this.SectorUnderMouseStatus.Text = "Sector {00, 00}";
            // 
            // ToolStatus
            // 
            this.ToolStatus.Name = "ToolStatus";
            this.ToolStatus.Size = new System.Drawing.Size(36, 17);
            this.ToolStatus.Text = "None";
            // 
            // ConstructorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.GameBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ControlsPanel);
            this.Name = "ConstructorForm";
            this.Size = new System.Drawing.Size(784, 421);
            ((System.ComponentModel.ISupportInitialize)(this.GameBox)).EndInit();
            this.ControlsPanel.ResumeLayout(false);
            this.ControlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthNumeric)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox GameBox;
        private System.Windows.Forms.Panel ControlsPanel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.RadioButton ButtonButton;
        private System.Windows.Forms.RadioButton DoorButton;
        private System.Windows.Forms.RadioButton WallButton;
        private System.Windows.Forms.NumericUpDown HeightNumeric;
        private System.Windows.Forms.NumericUpDown WidthNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel SectorUnderMouseStatus;
        private System.Windows.Forms.CheckBox ExitCheck;
        private System.Windows.Forms.CheckBox EnterCheck;
        private System.Windows.Forms.RadioButton ExitButton;
        private System.Windows.Forms.RadioButton EnterButton;
        private System.Windows.Forms.CheckBox ActivateBox;
        private System.Windows.Forms.ToolStripStatusLabel ToolStatus;
        private System.Windows.Forms.RadioButton AnalizeButton;
        private System.Windows.Forms.ToolTip AnalizeTip;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.Label WidthLabel;
    }
}
