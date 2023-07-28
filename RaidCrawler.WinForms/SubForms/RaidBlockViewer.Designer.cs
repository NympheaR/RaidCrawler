namespace RaidCrawler.WinForms.SubForms
{
    partial class RaidBlockViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RaidBlockViewer));
            RAM = new TextBox();
            AbsoluteAddress = new TextBox();
            LabelAbsoluteAddress = new Label();
            SuspendLayout();
            // 
            // RAM
            // 
            RAM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RAM.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            RAM.Location = new Point(12, 12);
            RAM.Multiline = true;
            RAM.Name = "RAM";
            RAM.Size = new Size(342, 342);
            RAM.TabIndex = 0;
            RAM.Text = "00 11 22 33 44 55 66 77 88 99 AA BB CC DD EE FF";
            // 
            // AbsoluteAddress
            // 
            AbsoluteAddress.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AbsoluteAddress.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            AbsoluteAddress.Location = new Point(235, 360);
            AbsoluteAddress.Name = "AbsoluteAddress";
            AbsoluteAddress.ReadOnly = true;
            AbsoluteAddress.Size = new Size(119, 22);
            AbsoluteAddress.TabIndex = 1;
            AbsoluteAddress.Text = "0123456789ABCDEF";
            // 
            // LabelAbsoluteAddress
            // 
            LabelAbsoluteAddress.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LabelAbsoluteAddress.AutoSize = true;
            LabelAbsoluteAddress.Location = new Point(69, 362);
            LabelAbsoluteAddress.Name = "LabelAbsoluteAddress";
            LabelAbsoluteAddress.Size = new Size(160, 15);
            LabelAbsoluteAddress.TabIndex = 2;
            LabelAbsoluteAddress.Text = "Raid Block Absolute Address:";
            // 
            // RaidBlockViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(367, 391);
            Controls.Add(LabelAbsoluteAddress);
            Controls.Add(AbsoluteAddress);
            Controls.Add(RAM);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "RaidBlockViewer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "RaidBlockViewer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox RAM;
        private TextBox AbsoluteAddress;
        private Label LabelAbsoluteAddress;
    }
}