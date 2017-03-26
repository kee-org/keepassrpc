namespace KeePassRPC.Forms
{
    partial class KeeFoxURLForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonMatch = new System.Windows.Forms.RadioButton();
            this.radioButtonBlock = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxRegEx = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(260, 20);
            this.textBox1.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(116, 98);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(197, 98);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // radioButtonMatch
            // 
            this.radioButtonMatch.AutoSize = true;
            this.radioButtonMatch.Location = new System.Drawing.Point(12, 78);
            this.radioButtonMatch.Name = "radioButtonMatch";
            this.radioButtonMatch.Size = new System.Drawing.Size(55, 17);
            this.radioButtonMatch.TabIndex = 3;
            this.radioButtonMatch.TabStop = true;
            this.radioButtonMatch.Text = "Match";
            this.radioButtonMatch.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlock
            // 
            this.radioButtonBlock.AutoSize = true;
            this.radioButtonBlock.Location = new System.Drawing.Point(12, 101);
            this.radioButtonBlock.Name = "radioButtonBlock";
            this.radioButtonBlock.Size = new System.Drawing.Size(52, 17);
            this.radioButtonBlock.TabIndex = 4;
            this.radioButtonBlock.TabStop = true;
            this.radioButtonBlock.Text = "Block";
            this.radioButtonBlock.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL / pattern:";
            // 
            // checkBoxRegEx
            // 
            this.checkBoxRegEx.AutoSize = true;
            this.checkBoxRegEx.Location = new System.Drawing.Point(12, 55);
            this.checkBoxRegEx.Name = "checkBoxRegEx";
            this.checkBoxRegEx.Size = new System.Drawing.Size(117, 17);
            this.checkBoxRegEx.TabIndex = 2;
            this.checkBoxRegEx.Text = "Regular Expression";
            this.checkBoxRegEx.UseVisualStyleBackColor = true;
            // 
            // KeeFoxURLForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(284, 130);
            this.Controls.Add(this.checkBoxRegEx);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonBlock);
            this.Controls.Add(this.radioButtonMatch);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeeFoxURLForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeeFoxURLForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonMatch;
        private System.Windows.Forms.RadioButton radioButtonBlock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxRegEx;
    }
}