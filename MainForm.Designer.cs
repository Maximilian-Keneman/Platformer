namespace Platformer
{
    partial class MainForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DebugInfo = new System.Windows.Forms.Label();
            this.DebugTimer = new System.Windows.Forms.Timer(this.components);
            this.DebugModeButton = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // DebugInfo
            // 
            this.DebugInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DebugInfo.BackColor = System.Drawing.Color.Black;
            this.DebugInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DebugInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DebugInfo.ForeColor = System.Drawing.Color.Lime;
            this.DebugInfo.Location = new System.Drawing.Point(12, 9);
            this.DebugInfo.Name = "DebugInfo";
            this.DebugInfo.Size = new System.Drawing.Size(129, 432);
            this.DebugInfo.TabIndex = 0;
            this.DebugInfo.Text = "Debug";
            this.DebugInfo.Visible = false;
            // 
            // DebugTimer
            // 
            this.DebugTimer.Tick += new System.EventHandler(this.DebugTimer_Tick);
            // 
            // DebugModeButton
            // 
            this.DebugModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugModeButton.BackColor = System.Drawing.SystemColors.Control;
            this.DebugModeButton.Location = new System.Drawing.Point(759, 12);
            this.DebugModeButton.Name = "DebugModeButton";
            this.DebugModeButton.Size = new System.Drawing.Size(29, 29);
            this.DebugModeButton.TabIndex = 1;
            this.DebugModeButton.Click += new System.EventHandler(this.DebugModeButton_Click);
            this.DebugModeButton.MouseEnter += new System.EventHandler(this.DebugModeButton_MouseEnter);
            this.DebugModeButton.MouseLeave += new System.EventHandler(this.DebugModeButton_MouseLeave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DebugModeButton);
            this.Controls.Add(this.DebugInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Platformer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label DebugInfo;
        private System.Windows.Forms.Timer DebugTimer;
        private System.Windows.Forms.Panel DebugModeButton;
    }
}

