namespace User_App_Test
{
    partial class Form1
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
            button1 = new Button();
            rich_txt = new RichTextBox();
            button2 = new Button();
            label1 = new Label();
            plan_lbl = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(32, 12);
            button1.Name = "button1";
            button1.Size = new Size(174, 48);
            button1.TabIndex = 0;
            button1.Text = "new Form";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // rich_txt
            // 
            rich_txt.Location = new Point(383, 55);
            rich_txt.Name = "rich_txt";
            rich_txt.Size = new Size(364, 347);
            rich_txt.TabIndex = 1;
            rich_txt.Text = "";
            // 
            // button2
            // 
            button2.Location = new Point(32, 91);
            button2.Name = "button2";
            button2.Size = new Size(174, 48);
            button2.TabIndex = 2;
            button2.Text = "Chagnge Theme";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(597, 421);
            label1.Name = "label1";
            label1.Size = new Size(81, 20);
            label1.TabIndex = 3;
            label1.Text = "Your Plan : ";
            // 
            // plan_lbl
            // 
            plan_lbl.AutoSize = true;
            plan_lbl.Location = new Point(684, 421);
            plan_lbl.Name = "plan_lbl";
            plan_lbl.Size = new Size(40, 20);
            plan_lbl.TabIndex = 4;
            plan_lbl.Text = " N/A";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(plan_lbl);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(rich_txt);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private RichTextBox rich_txt;
        private Button button2;
        private Label label1;
        private Label plan_lbl;
    }
}
