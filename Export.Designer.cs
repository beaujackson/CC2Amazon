namespace CC2Amazon
{
	partial class Export
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
			this.buttonExport = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textFile = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboUpdateDelete = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textLaunchDate = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonExport
			// 
			this.buttonExport.Location = new System.Drawing.Point(21, 125);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(104, 23);
			this.buttonExport.TabIndex = 1;
			this.buttonExport.Text = "Export from CC";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Export File:";
			// 
			// textFile
			// 
			this.textFile.Location = new System.Drawing.Point(99, 14);
			this.textFile.Name = "textFile";
			this.textFile.Size = new System.Drawing.Size(354, 20);
			this.textFile.TabIndex = 0;
			this.textFile.Text = "C:\\beau\\src\\CC2Amazon\\inventory.txt";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "UpdateDelete:";
			// 
			// comboUpdateDelete
			// 
			this.comboUpdateDelete.FormattingEnabled = true;
			this.comboUpdateDelete.Items.AddRange(new object[] {
            "Update",
            "UpdatePartial",
            "Delete"});
			this.comboUpdateDelete.Location = new System.Drawing.Point(99, 46);
			this.comboUpdateDelete.Name = "comboUpdateDelete";
			this.comboUpdateDelete.Size = new System.Drawing.Size(206, 21);
			this.comboUpdateDelete.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(25, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "LaunchDate:";
			// 
			// textLaunchDate
			// 
			this.textLaunchDate.Location = new System.Drawing.Point(100, 80);
			this.textLaunchDate.Name = "textLaunchDate";
			this.textLaunchDate.Size = new System.Drawing.Size(100, 20);
			this.textLaunchDate.TabIndex = 5;
			// 
			// Export
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(466, 157);
			this.Controls.Add(this.textLaunchDate);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboUpdateDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonExport);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Export";
			this.Text = "Export";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonExport;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboUpdateDelete;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLaunchDate;
	}
}

