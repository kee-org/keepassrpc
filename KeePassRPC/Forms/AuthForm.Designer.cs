namespace KeePassRPC.Forms
{
    partial class AuthForm
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
            this.buttonDeny = new System.Windows.Forms.Button();
            this.richTextBoxClientID = new System.Windows.Forms.RichTextBox();
            this.richTextBoxPassword = new System.Windows.Forms.RichTextBox();
            this.richTextBoxSecurityLevel = new System.Windows.Forms.RichTextBox();
            this.richTextBoxConfirmInstruction = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDeny
            // 
            this.buttonDeny.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDeny.Location = new System.Drawing.Point(12, 386);
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.Size = new System.Drawing.Size(280, 23);
            this.buttonDeny.TabIndex = 0;
            this.buttonDeny.Text = "Deny this request";
            this.buttonDeny.UseVisualStyleBackColor = true;
            this.buttonDeny.Click += new System.EventHandler(this.buttonDeny_Click);
            // 
            // richTextBoxClientID
            // 
            this.richTextBoxClientID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxClientID.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxClientID.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxClientID.Name = "richTextBoxClientID";
            this.richTextBoxClientID.ReadOnly = true;
            this.richTextBoxClientID.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxClientID.Size = new System.Drawing.Size(556, 138);
            this.richTextBoxClientID.TabIndex = 2;
            this.richTextBoxClientID.Text = "This is a test";
            // 
            // richTextBoxPassword
            // 
            this.richTextBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxPassword.DetectUrls = false;
            this.richTextBoxPassword.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxPassword.ForeColor = System.Drawing.Color.Red;
            this.richTextBoxPassword.Location = new System.Drawing.Point(400, 13);
            this.richTextBoxPassword.Name = "richTextBoxPassword";
            this.richTextBoxPassword.ReadOnly = true;
            this.richTextBoxPassword.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxPassword.Size = new System.Drawing.Size(151, 31);
            this.richTextBoxPassword.TabIndex = 3;
            this.richTextBoxPassword.Text = "222222222";
            // 
            // richTextBoxSecurityLevel
            // 
            this.richTextBoxSecurityLevel.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxSecurityLevel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxSecurityLevel.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxSecurityLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxSecurityLevel.Location = new System.Drawing.Point(12, 242);
            this.richTextBoxSecurityLevel.Name = "richTextBoxSecurityLevel";
            this.richTextBoxSecurityLevel.ReadOnly = true;
            this.richTextBoxSecurityLevel.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxSecurityLevel.Size = new System.Drawing.Size(556, 138);
            this.richTextBoxSecurityLevel.TabIndex = 4;
            this.richTextBoxSecurityLevel.Text = "";
            // 
            // richTextBoxConfirmInstruction
            // 
            this.richTextBoxConfirmInstruction.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxConfirmInstruction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxConfirmInstruction.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxConfirmInstruction.Location = new System.Drawing.Point(15, 13);
            this.richTextBoxConfirmInstruction.Name = "richTextBoxConfirmInstruction";
            this.richTextBoxConfirmInstruction.ReadOnly = true;
            this.richTextBoxConfirmInstruction.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxConfirmInstruction.Size = new System.Drawing.Size(379, 58);
            this.richTextBoxConfirmInstruction.TabIndex = 5;
            this.richTextBoxConfirmInstruction.Text = "To authorise KeeFox to access your passwords please enter this password into the " +
    "box KeeFox has presented to you.";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(364, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "This dialog will automatically close when the connection is authorised or denied";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.richTextBoxConfirmInstruction);
            this.panel1.Controls.Add(this.richTextBoxPassword);
            this.panel1.ForeColor = System.Drawing.Color.Red;
            this.panel1.Location = new System.Drawing.Point(12, 156);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 80);
            this.panel1.TabIndex = 8;
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 427);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxSecurityLevel);
            this.Controls.Add(this.richTextBoxClientID);
            this.Controls.Add(this.buttonDeny);
            this.Name = "AuthForm";
            this.Text = "Authorise a new connection";
            this.Load += new System.EventHandler(this.AuthForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDeny;
        private System.Windows.Forms.RichTextBox richTextBoxClientID;
        private System.Windows.Forms.RichTextBox richTextBoxPassword;
        private System.Windows.Forms.RichTextBox richTextBoxSecurityLevel;
        private System.Windows.Forms.RichTextBox richTextBoxConfirmInstruction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}