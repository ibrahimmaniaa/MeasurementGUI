namespace MeasurementGUI
{
    partial class mainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.uploadImgBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.checkLastImgTimer = new System.Windows.Forms.Timer(this.components);
            this.showLinesCheckBox = new System.Windows.Forms.CheckBox();
            this.anglePictureBox = new System.Windows.Forms.PictureBox();
            this.zoomOutBtn = new System.Windows.Forms.Button();
            this.homeBtn = new System.Windows.Forms.Button();
            this.zoomInBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.anglePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // uploadImgBtn
            // 
            this.uploadImgBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uploadImgBtn.Location = new System.Drawing.Point(822, 37);
            this.uploadImgBtn.Name = "uploadImgBtn";
            this.uploadImgBtn.Size = new System.Drawing.Size(191, 46);
            this.uploadImgBtn.TabIndex = 5;
            this.uploadImgBtn.Text = "upload image";
            this.uploadImgBtn.UseVisualStyleBackColor = true;
            this.uploadImgBtn.Click += new System.EventHandler(this.uploadImgBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.startBtn.Location = new System.Drawing.Point(822, 606);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(191, 147);
            this.startBtn.TabIndex = 4;
            this.startBtn.Text = "START";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 768);
            this.panel1.TabIndex = 6;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(768, 768);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragDrop);
            this.pictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragEnter);
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // checkLastImgTimer
            // 
            this.checkLastImgTimer.Interval = 1000;
            this.checkLastImgTimer.Tick += new System.EventHandler(this.checkLastImgTimer_Tick);
            // 
            // showLinesCheckBox
            // 
            this.showLinesCheckBox.AutoSize = true;
            this.showLinesCheckBox.Checked = true;
            this.showLinesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLinesCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showLinesCheckBox.Location = new System.Drawing.Point(822, 556);
            this.showLinesCheckBox.Name = "showLinesCheckBox";
            this.showLinesCheckBox.Size = new System.Drawing.Size(110, 24);
            this.showLinesCheckBox.TabIndex = 9;
            this.showLinesCheckBox.Text = "Show Lines";
            this.showLinesCheckBox.UseVisualStyleBackColor = true;
            this.showLinesCheckBox.CheckedChanged += new System.EventHandler(this.showLinesCheckBox_CheckedChanged);
            // 
            // anglePictureBox
            // 
            this.anglePictureBox.Location = new System.Drawing.Point(822, 311);
            this.anglePictureBox.Name = "anglePictureBox";
            this.anglePictureBox.Size = new System.Drawing.Size(191, 149);
            this.anglePictureBox.TabIndex = 8;
            this.anglePictureBox.TabStop = false;
            this.anglePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.anglePictureBox_Paint);
            // 
            // zoomOutBtn
            // 
            this.zoomOutBtn.Enabled = false;
            this.zoomOutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.zoomOutBtn.Image = global::MeasurementGUI.Properties.Resources.zoomout;
            this.zoomOutBtn.Location = new System.Drawing.Point(155, 786);
            this.zoomOutBtn.Margin = new System.Windows.Forms.Padding(0);
            this.zoomOutBtn.Name = "zoomOutBtn";
            this.zoomOutBtn.Size = new System.Drawing.Size(50, 50);
            this.zoomOutBtn.TabIndex = 7;
            this.zoomOutBtn.UseVisualStyleBackColor = true;
            this.zoomOutBtn.Click += new System.EventHandler(this.zoomOutBtn_Click);
            // 
            // homeBtn
            // 
            this.homeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.homeBtn.Image = global::MeasurementGUI.Properties.Resources.home;
            this.homeBtn.Location = new System.Drawing.Point(11, 786);
            this.homeBtn.Margin = new System.Windows.Forms.Padding(0);
            this.homeBtn.Name = "homeBtn";
            this.homeBtn.Size = new System.Drawing.Size(50, 50);
            this.homeBtn.TabIndex = 1;
            this.homeBtn.UseVisualStyleBackColor = true;
            this.homeBtn.Click += new System.EventHandler(this.homeBtn_Click);
            // 
            // zoomInBtn
            // 
            this.zoomInBtn.Enabled = false;
            this.zoomInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.zoomInBtn.Image = global::MeasurementGUI.Properties.Resources.zoom;
            this.zoomInBtn.Location = new System.Drawing.Point(83, 786);
            this.zoomInBtn.Margin = new System.Windows.Forms.Padding(0);
            this.zoomInBtn.Name = "zoomInBtn";
            this.zoomInBtn.Size = new System.Drawing.Size(50, 50);
            this.zoomInBtn.TabIndex = 2;
            this.zoomInBtn.TabStop = false;
            this.zoomInBtn.UseVisualStyleBackColor = true;
            this.zoomInBtn.Click += new System.EventHandler(this.zoomInBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Enabled = false;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.saveBtn.Image = global::MeasurementGUI.Properties.Resources.save;
            this.saveBtn.Location = new System.Drawing.Point(227, 786);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(50, 50);
            this.saveBtn.TabIndex = 3;
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 842);
            this.Controls.Add(this.showLinesCheckBox);
            this.Controls.Add(this.anglePictureBox);
            this.Controls.Add(this.zoomOutBtn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.homeBtn);
            this.Controls.Add(this.zoomInBtn);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.uploadImgBtn);
            this.Font = new System.Drawing.Font("Symbol", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.SizeChanged += new System.EventHandler(this.mainForm_SizeChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.anglePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button uploadImgBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button zoomInBtn;
        private System.Windows.Forms.Button homeBtn;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button zoomOutBtn;
        private System.Windows.Forms.PictureBox anglePictureBox;
        private System.Windows.Forms.Timer checkLastImgTimer;
        private System.Windows.Forms.CheckBox showLinesCheckBox;
    }
}

