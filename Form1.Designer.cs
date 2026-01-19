namespace CS2ServerPicker
{
    partial class Form1 : System.Windows.Forms.Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoad = new Button();
            dataGridView1 = new DataGridView();
            comboLimbaj = new ComboBox();
            btnApply = new Button();
            btnReset = new Button();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(57, 33);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(151, 23);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load Servers";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(57, 63);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(683, 312);
            dataGridView1.TabIndex = 1;
            // 
            // comboLimbaj
            // 
            comboLimbaj.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLimbaj.FormattingEnabled = true;
            comboLimbaj.Location = new Point(619, 34);
            comboLimbaj.Name = "comboLimbaj";
            comboLimbaj.Size = new Size(121, 23);
            comboLimbaj.TabIndex = 2;
            comboLimbaj.SelectedIndexChanged += comboLimbaj_SelectedIndexChanged;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(57, 381);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(151, 23);
            btnApply.TabIndex = 3;
            btnApply.Text = "Apply Changes";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(595, 381);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(145, 23);
            btnReset.TabIndex = 4;
            btnReset.Text = "Unblock All";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(39, 17);
            lblStatus.Text = "Ready";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip1);
            Controls.Add(btnReset);
            Controls.Add(btnApply);
            Controls.Add(comboLimbaj);
            Controls.Add(dataGridView1);
            Controls.Add(btnLoad);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLoad;
        private DataGridView dataGridView1;
        private ComboBox comboLimbaj;
        private Button btnApply;
        private Button btnReset;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
    }
}
