namespace ArcSoft_Face_Demo_X64
{
    partial class ArcSoft_Face_Demo_X64
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox0 = new System.Windows.Forms.PictureBox();
            this.imageBox0 = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox0)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(426, 277);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "注册人脸";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox0
            // 
            this.pictureBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox0.Location = new System.Drawing.Point(410, 24);
            this.pictureBox0.Name = "pictureBox0";
            this.pictureBox0.Size = new System.Drawing.Size(224, 179);
            this.pictureBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox0.TabIndex = 9;
            this.pictureBox0.TabStop = false;
            // 
            // imageBox0
            // 
            this.imageBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox0.Location = new System.Drawing.Point(12, 12);
            this.imageBox0.Name = "imageBox0";
            this.imageBox0.Size = new System.Drawing.Size(392, 344);
            this.imageBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox0.TabIndex = 8;
            this.imageBox0.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(408, 323);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "识别结果： ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(491, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "注册人脸图片";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(309, -154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "结果： ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(424, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "人脸用户名：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(507, 209);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(532, 277);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "识别人脸";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(424, 239);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 17;
            this.label5.Text = "相似度调节：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(507, 237);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(100, 20);
            this.comboBox1.TabIndex = 18;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(519, 332);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ArcSoft_Face_Demo_X64
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 377);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox0);
            this.Controls.Add(this.imageBox0);
            this.Name = "ArcSoft_Face_Demo_X64";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArcSoftFaceDemoX64_FormClosing);
            this.Load += new System.EventHandler(this.ArcSoft_Face_Demo_X64_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox0)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox0;
        private Emgu.CV.UI.ImageBox imageBox0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button3;
    }
}

