namespace jinjieapp
{
    partial class Form2
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
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button8 = new System.Windows.Forms.Button();
            this.bchon = new System.Windows.Forms.Button();
            this.bchoff = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 129);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 46);
            this.button1.TabIndex = 0;
            this.button1.Text = "登录获取token";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(413, 205);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(256, 46);
            this.button3.TabIndex = 2;
            this.button3.Text = "计算污染率and 加入待吹灰列表";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(413, 129);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(256, 46);
            this.button4.TabIndex = 5;
            this.button4.Text = "吹灰列表执行";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(44, 35);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(134, 46);
            this.button7.TabIndex = 6;
            this.button7.Text = "调接口(>>)";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(225, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 46);
            this.button2.TabIndex = 7;
            this.button2.Text = "空预器吹灰列表执行";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(44, 205);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(134, 46);
            this.button5.TabIndex = 8;
            this.button5.Text = "获取负荷";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(225, 205);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(144, 46);
            this.button6.TabIndex = 9;
            this.button6.Text = "吹灰器状态更新";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(214, 35);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(134, 46);
            this.button8.TabIndex = 10;
            this.button8.Text = "调接口(||)";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // bchon
            // 
            this.bchon.Location = new System.Drawing.Point(389, 38);
            this.bchon.Name = "bchon";
            this.bchon.Size = new System.Drawing.Size(134, 43);
            this.bchon.TabIndex = 11;
            this.bchon.Text = "炉膛吹灰器on";
            this.bchon.UseVisualStyleBackColor = true;
            this.bchon.Click += new System.EventHandler(this.Bchon_Click);
            // 
            // bchoff
            // 
            this.bchoff.Location = new System.Drawing.Point(558, 38);
            this.bchoff.Name = "bchoff";
            this.bchoff.Size = new System.Drawing.Size(134, 43);
            this.bchoff.TabIndex = 12;
            this.bchoff.Text = "炉膛吹灰器off";
            this.bchoff.UseVisualStyleBackColor = true;
            this.bchoff.Click += new System.EventHandler(this.Bchoff_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 10000;
            this.timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(214, 282);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(134, 46);
            this.button9.TabIndex = 14;
            this.button9.Text = "调接口(||)";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(44, 282);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(134, 46);
            this.button10.TabIndex = 13;
            this.button10.Text = "调接口(>>)";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 5000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(431, 282);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(124, 46);
            this.button11.TabIndex = 15;
            this.button11.Text = "button11";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.Button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(623, 282);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(104, 46);
            this.button12.TabIndex = 16;
            this.button12.Text = "button12";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 380);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.bchoff);
            this.Controls.Add(this.bchon);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button bchon;
        private System.Windows.Forms.Button bchoff;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
    }
}

