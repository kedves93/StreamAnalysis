namespace FakeSensor
{
    partial class SensorForm
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
            this.Start_Btn = new System.Windows.Forms.Button();
            this.Stop_Btn = new System.Windows.Forms.Button();
            this.Parameters_GroupBox = new System.Windows.Forms.GroupBox();
            this.FrequencyType_ComboBox = new System.Windows.Forms.ComboBox();
            this.Frequency_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Frequency_Label = new System.Windows.Forms.Label();
            this.SensorType_Label = new System.Windows.Forms.Label();
            this.SensorType_ComboBox = new System.Windows.Forms.ComboBox();
            this.Parameters_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Frequency_NumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Start_Btn
            // 
            this.Start_Btn.Location = new System.Drawing.Point(156, 307);
            this.Start_Btn.Name = "Start_Btn";
            this.Start_Btn.Size = new System.Drawing.Size(88, 33);
            this.Start_Btn.TabIndex = 0;
            this.Start_Btn.Text = "Start";
            this.Start_Btn.UseVisualStyleBackColor = true;
            // 
            // Stop_Btn
            // 
            this.Stop_Btn.Location = new System.Drawing.Point(250, 307);
            this.Stop_Btn.Name = "Stop_Btn";
            this.Stop_Btn.Size = new System.Drawing.Size(88, 33);
            this.Stop_Btn.TabIndex = 1;
            this.Stop_Btn.Text = "Stop";
            this.Stop_Btn.UseVisualStyleBackColor = true;
            // 
            // Parameters_GroupBox
            // 
            this.Parameters_GroupBox.Controls.Add(this.SensorType_ComboBox);
            this.Parameters_GroupBox.Controls.Add(this.SensorType_Label);
            this.Parameters_GroupBox.Controls.Add(this.FrequencyType_ComboBox);
            this.Parameters_GroupBox.Controls.Add(this.Frequency_NumericUpDown);
            this.Parameters_GroupBox.Controls.Add(this.Frequency_Label);
            this.Parameters_GroupBox.Location = new System.Drawing.Point(41, 26);
            this.Parameters_GroupBox.Name = "Parameters_GroupBox";
            this.Parameters_GroupBox.Size = new System.Drawing.Size(391, 236);
            this.Parameters_GroupBox.TabIndex = 2;
            this.Parameters_GroupBox.TabStop = false;
            this.Parameters_GroupBox.Text = "Parameters";
            // 
            // FrequencyType_ComboBox
            // 
            this.FrequencyType_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FrequencyType_ComboBox.FormattingEnabled = true;
            this.FrequencyType_ComboBox.Items.AddRange(new object[] {
            "milliseconds",
            "seconds",
            "minutes",
            "hours"});
            this.FrequencyType_ComboBox.SelectedIndex = 1;
            this.FrequencyType_ComboBox.Location = new System.Drawing.Point(186, 78);
            this.FrequencyType_ComboBox.MaxDropDownItems = 4;
            this.FrequencyType_ComboBox.MaxLength = 4;
            this.FrequencyType_ComboBox.Name = "FrequencyType_ComboBox";
            this.FrequencyType_ComboBox.Size = new System.Drawing.Size(86, 21);
            this.FrequencyType_ComboBox.TabIndex = 3;
            // 
            // Frequency_NumericUpDown
            // 
            this.Frequency_NumericUpDown.Location = new System.Drawing.Point(134, 78);
            this.Frequency_NumericUpDown.Name = "Frequency_NumericUpDown";
            this.Frequency_NumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.Frequency_NumericUpDown.TabIndex = 2;
            // 
            // Frequency_Label
            // 
            this.Frequency_Label.AutoSize = true;
            this.Frequency_Label.Location = new System.Drawing.Point(64, 78);
            this.Frequency_Label.Name = "Frequency_Label";
            this.Frequency_Label.Size = new System.Drawing.Size(64, 15);
            this.Frequency_Label.TabIndex = 1;
            this.Frequency_Label.Text = "Frequency";
            // 
            // SensorType_Label
            // 
            this.SensorType_Label.AutoSize = true;
            this.SensorType_Label.Location = new System.Drawing.Point(53, 43);
            this.SensorType_Label.Name = "SensorType_Label";
            this.SensorType_Label.Size = new System.Drawing.Size(75, 15);
            this.SensorType_Label.TabIndex = 4;
            this.SensorType_Label.Text = "Sensor Type";
            // 
            // SensorType_ComboBox
            // 
            this.SensorType_ComboBox.FormattingEnabled = true;
            this.SensorType_ComboBox.Items.AddRange(new object[] {
            "Temperature",
            "Pressure"});
            this.SensorType_ComboBox.SelectedIndex = 0;
            this.SensorType_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SensorType_ComboBox.Location = new System.Drawing.Point(134, 43);
            this.SensorType_ComboBox.Name = "SensorType_ComboBox";
            this.SensorType_ComboBox.Size = new System.Drawing.Size(103, 21);
            this.SensorType_ComboBox.TabIndex = 5;
            // 
            // SensorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 377);
            this.Controls.Add(this.Parameters_GroupBox);
            this.Controls.Add(this.Stop_Btn);
            this.Controls.Add(this.Start_Btn);
            this.Name = "SensorForm";
            this.Text = "Sensor";
            this.Parameters_GroupBox.ResumeLayout(false);
            this.Parameters_GroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Frequency_NumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Start_Btn;
        private System.Windows.Forms.Button Stop_Btn;
        private System.Windows.Forms.GroupBox Parameters_GroupBox;
        private System.Windows.Forms.Label Frequency_Label;
        private System.Windows.Forms.NumericUpDown Frequency_NumericUpDown;
        private System.Windows.Forms.ComboBox FrequencyType_ComboBox;
        private System.Windows.Forms.ComboBox SensorType_ComboBox;
        private System.Windows.Forms.Label SensorType_Label;
    }
}

