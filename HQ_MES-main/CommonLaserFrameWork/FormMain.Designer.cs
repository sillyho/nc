namespace CommonLaserFrameWork
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.textBox_model = new System.Windows.Forms.TextBox();
            this.button_load = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.button_hand = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_SN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_load2 = new System.Windows.Forms.Button();
            this.textBox_model2 = new System.Windows.Forms.TextBox();
            this.label_status = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_set = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label_io = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_model
            // 
            this.textBox_model.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_model.ForeColor = System.Drawing.Color.MidnightBlue;
            this.textBox_model.Location = new System.Drawing.Point(101, 12);
            this.textBox_model.Name = "textBox_model";
            this.textBox_model.ReadOnly = true;
            this.textBox_model.Size = new System.Drawing.Size(480, 21);
            this.textBox_model.TabIndex = 15;
            // 
            // button_load
            // 
            this.button_load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_load.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_load.BackgroundImage")));
            this.button_load.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_load.FlatAppearance.BorderSize = 0;
            this.button_load.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_load.Location = new System.Drawing.Point(591, 7);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(26, 26);
            this.button_load.TabIndex = 14;
            this.button_load.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Click += new System.EventHandler(this.button_load_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "工位1模板路径:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.Location = new System.Drawing.Point(10, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(612, 467);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_Log.Location = new System.Drawing.Point(631, 136);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.ReadOnly = true;
            this.richTextBox_Log.Size = new System.Drawing.Size(319, 369);
            this.richTextBox_Log.TabIndex = 18;
            this.richTextBox_Log.Text = "";
            // 
            // button_hand
            // 
            this.button_hand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_hand.FlatAppearance.BorderSize = 0;
            this.button_hand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_hand.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_hand.Image = ((System.Drawing.Image)(resources.GetObject("button_hand.Image")));
            this.button_hand.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_hand.Location = new System.Drawing.Point(801, 98);
            this.button_hand.Name = "button_hand";
            this.button_hand.Size = new System.Drawing.Size(71, 35);
            this.button_hand.TabIndex = 19;
            this.button_hand.Text = "标刻";
            this.button_hand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_hand.UseVisualStyleBackColor = true;
            this.button_hand.Click += new System.EventHandler(this.button_hand_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Stop.FlatAppearance.BorderSize = 0;
            this.button_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Stop.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Stop.Image = ((System.Drawing.Image)(resources.GetObject("button_Stop.Image")));
            this.button_Stop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_Stop.Location = new System.Drawing.Point(878, 98);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(71, 35);
            this.button_Stop.TabIndex = 20;
            this.button_Stop.Text = "停止";
            this.button_Stop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(630, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 22);
            this.label2.TabIndex = 21;
            this.label2.Text = "DSN:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // textBox_SN
            // 
            this.textBox_SN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SN.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SN.Location = new System.Drawing.Point(632, 58);
            this.textBox_SN.Name = "textBox_SN";
            this.textBox_SN.Size = new System.Drawing.Size(307, 29);
            this.textBox_SN.TabIndex = 22;
            this.textBox_SN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_SN_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label3.Location = new System.Drawing.Point(4, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "工位2模板路径:";
            // 
            // button_load2
            // 
            this.button_load2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_load2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_load2.BackgroundImage")));
            this.button_load2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_load2.FlatAppearance.BorderSize = 0;
            this.button_load2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_load2.Location = new System.Drawing.Point(591, 38);
            this.button_load2.Name = "button_load2";
            this.button_load2.Size = new System.Drawing.Size(26, 26);
            this.button_load2.TabIndex = 14;
            this.button_load2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_load2.UseVisualStyleBackColor = true;
            this.button_load2.Click += new System.EventHandler(this.button_load2_Click);
            // 
            // textBox_model2
            // 
            this.textBox_model2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_model2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.textBox_model2.Location = new System.Drawing.Point(101, 41);
            this.textBox_model2.Name = "textBox_model2";
            this.textBox_model2.ReadOnly = true;
            this.textBox_model2.Size = new System.Drawing.Size(480, 21);
            this.textBox_model2.TabIndex = 15;
            // 
            // label_status
            // 
            this.label_status.BackColor = System.Drawing.SystemColors.Control;
            this.label_status.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_status.ForeColor = System.Drawing.Color.Lime;
            this.label_status.Location = new System.Drawing.Point(721, 95);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(62, 39);
            this.label_status.TabIndex = 23;
            this.label_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(635, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 22);
            this.label4.TabIndex = 21;
            this.label4.Text = "校验结果:";
            this.label4.Click += new System.EventHandler(this.label2_Click);
            // 
            // button_set
            // 
            this.button_set.FlatAppearance.BorderSize = 0;
            this.button_set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_set.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_set.Image = ((System.Drawing.Image)(resources.GetObject("button_set.Image")));
            this.button_set.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_set.Location = new System.Drawing.Point(867, 7);
            this.button_set.Name = "button_set";
            this.button_set.Size = new System.Drawing.Size(75, 46);
            this.button_set.TabIndex = 24;
            this.button_set.Text = "设置";
            this.button_set.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_set.UseVisualStyleBackColor = true;
            this.button_set.Click += new System.EventHandler(this.button_set_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(630, 518);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 14);
            this.label5.TabIndex = 25;
            this.label5.Text = "IO状态(0-15):";
            // 
            // label_io
            // 
            this.label_io.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_io.AutoSize = true;
            this.label_io.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_io.ForeColor = System.Drawing.Color.Blue;
            this.label_io.Location = new System.Drawing.Point(746, 518);
            this.label_io.Name = "label_io";
            this.label_io.Size = new System.Drawing.Size(135, 14);
            this.label_io.TabIndex = 25;
            this.label_io.Text = "0000000000000000";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(955, 541);
            this.Controls.Add(this.label_io);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_set);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.textBox_SN);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_hand);
            this.Controls.Add(this.richTextBox_Log);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox_model2);
            this.Controls.Add(this.button_load2);
            this.Controls.Add(this.textBox_model);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_load);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "首镭华勤MES打标软件20211017";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_model;
        private System.Windows.Forms.Button button_load;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox richTextBox_Log;
        private System.Windows.Forms.Button button_hand;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_SN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_load2;
        private System.Windows.Forms.TextBox textBox_model2;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_set;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_io;
    }
}

