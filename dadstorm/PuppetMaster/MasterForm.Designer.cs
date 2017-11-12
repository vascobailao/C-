namespace DADSTORM
{
    partial class MasterForm
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
            this.LogBox = new System.Windows.Forms.TextBox();
            this.send_button = new System.Windows.Forms.Button();
            this.Insert_command = new System.Windows.Forms.Label();
            this.Run_script = new System.Windows.Forms.Label();
            this.runBox = new System.Windows.Forms.TextBox();
            this.run_button = new System.Windows.Forms.Button();
            this.sendBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(393, 43);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogBox.Size = new System.Drawing.Size(320, 376);
            this.LogBox.TabIndex = 4;
            this.LogBox.TextChanged += new System.EventHandler(this.LogBox_TextChanged);
            // 
            // send_button
            // 
            this.send_button.Location = new System.Drawing.Point(310, 45);
            this.send_button.Name = "send_button";
            this.send_button.Size = new System.Drawing.Size(75, 20);
            this.send_button.TabIndex = 5;
            this.send_button.Text = "Send";
            this.send_button.UseVisualStyleBackColor = true;
            this.send_button.Click += new System.EventHandler(this.send_button_Click);
            // 
            // Insert_command
            // 
            this.Insert_command.AutoSize = true;
            this.Insert_command.Location = new System.Drawing.Point(12, 29);
            this.Insert_command.Name = "Insert_command";
            this.Insert_command.Size = new System.Drawing.Size(82, 13);
            this.Insert_command.TabIndex = 7;
            this.Insert_command.Text = "Insert command";
            // 
            // Run_script
            // 
            this.Run_script.AutoSize = true;
            this.Run_script.Location = new System.Drawing.Point(12, 106);
            this.Run_script.Name = "Run_script";
            this.Run_script.Size = new System.Drawing.Size(55, 13);
            this.Run_script.TabIndex = 8;
            this.Run_script.Text = "Run script";
            // 
            // runBox
            // 
            this.runBox.Location = new System.Drawing.Point(12, 122);
            this.runBox.Name = "runBox";
            this.runBox.Size = new System.Drawing.Size(289, 20);
            this.runBox.TabIndex = 9;
            this.runBox.Text = "dadstorm.config";
            // 
            // run_button
            // 
            this.run_button.Location = new System.Drawing.Point(307, 122);
            this.run_button.Name = "run_button";
            this.run_button.Size = new System.Drawing.Size(75, 20);
            this.run_button.TabIndex = 10;
            this.run_button.Text = "Run";
            this.run_button.UseVisualStyleBackColor = true;
            this.run_button.Click += new System.EventHandler(this.run_button_Click);
            // 
            // sendBox
            // 
            this.sendBox.Location = new System.Drawing.Point(12, 46);
            this.sendBox.Name = "sendBox";
            this.sendBox.Size = new System.Drawing.Size(289, 20);
            this.sendBox.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(367, 41);
            this.button1.TabIndex = 12;
            this.button1.Text = "Step-by-step";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 442);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sendBox);
            this.Controls.Add(this.run_button);
            this.Controls.Add(this.runBox);
            this.Controls.Add(this.Run_script);
            this.Controls.Add(this.Insert_command);
            this.Controls.Add(this.send_button);
            this.Controls.Add(this.LogBox);
            this.Name = "MasterForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MasterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button send_button;
        private System.Windows.Forms.Label Insert_command;
        private System.Windows.Forms.Label Run_script;
        private System.Windows.Forms.TextBox runBox;
        private System.Windows.Forms.Button run_button;
        private System.Windows.Forms.TextBox sendBox;
        private System.Windows.Forms.Button button1;
    }
}

