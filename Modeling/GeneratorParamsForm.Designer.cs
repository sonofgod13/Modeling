namespace Modeling
{
    partial class GeneratorParamsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Z_comboBox = new System.Windows.Forms.ComboBox();
            this.Z_param2_label = new System.Windows.Forms.Label();
            this.Z_param1_textBox = new System.Windows.Forms.TextBox();
            this.Z_param1_label = new System.Windows.Forms.Label();
            this.Z_param2_textBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Z_comboBox);
            this.groupBox1.Controls.Add(this.Z_param2_label);
            this.groupBox1.Controls.Add(this.Z_param1_textBox);
            this.groupBox1.Controls.Add(this.Z_param1_label);
            this.groupBox1.Controls.Add(this.Z_param2_textBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 115);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Выбор параметров генератора";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "тип:";
            // 
            // Z_comboBox
            // 
            this.Z_comboBox.FormattingEnabled = true;
            this.Z_comboBox.Items.AddRange(new object[] {
            "Равномерное",
            "Нормальное",
            "Рэлея",
            "Гамма-распределение",
            "Экспоненциальное"});
            this.Z_comboBox.Location = new System.Drawing.Point(39, 19);
            this.Z_comboBox.Name = "Z_comboBox";
            this.Z_comboBox.Size = new System.Drawing.Size(211, 21);
            this.Z_comboBox.TabIndex = 0;
            this.Z_comboBox.SelectedIndexChanged += new System.EventHandler(this.Z_comboBox_SelectedIndexChanged);
            // 
            // Z_param2_label
            // 
            this.Z_param2_label.AutoSize = true;
            this.Z_param2_label.Location = new System.Drawing.Point(5, 88);
            this.Z_param2_label.Name = "Z_param2_label";
            this.Z_param2_label.Size = new System.Drawing.Size(16, 13);
            this.Z_param2_label.TabIndex = 4;
            this.Z_param2_label.Text = "b:";
            // 
            // Z_param1_textBox
            // 
            this.Z_param1_textBox.Location = new System.Drawing.Point(79, 55);
            this.Z_param1_textBox.Name = "Z_param1_textBox";
            this.Z_param1_textBox.Size = new System.Drawing.Size(171, 20);
            this.Z_param1_textBox.TabIndex = 1;
            // 
            // Z_param1_label
            // 
            this.Z_param1_label.AutoSize = true;
            this.Z_param1_label.Location = new System.Drawing.Point(5, 58);
            this.Z_param1_label.Name = "Z_param1_label";
            this.Z_param1_label.Size = new System.Drawing.Size(16, 13);
            this.Z_param1_label.TabIndex = 3;
            this.Z_param1_label.Text = "a:";
            // 
            // Z_param2_textBox
            // 
            this.Z_param2_textBox.Location = new System.Drawing.Point(79, 85);
            this.Z_param2_textBox.Name = "Z_param2_textBox";
            this.Z_param2_textBox.Size = new System.Drawing.Size(171, 20);
            this.Z_param2_textBox.TabIndex = 2;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 133);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "Задать";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // generator_params
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Name = "generator_params";
            this.Text = "generator_params";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Z_param2_label;
        private System.Windows.Forms.Label Z_param1_label;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox Z_param1_textBox;
        private System.Windows.Forms.TextBox Z_param2_textBox;

        public System.Windows.Forms.ComboBox Z_comboBox;
        public double fGeneratorParamFirst;
        public double fGeneratorParamSecond;

        private double fGeneratorParamFirst_BASE;
        private double fGeneratorParamSecond_BASE;

    }
}