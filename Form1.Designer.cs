namespace Case1
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
            toolPanel = new Panel();
            btnSave = new Button();
            btnOpen = new Button();
            btnYellow = new Button();
            btnGreen = new Button();
            btnBlue = new Button();
            btnRed = new Button();
            btnBlack = new Button();
            btnHexagon = new Button();
            btnTriangle = new Button();
            btnCircle = new Button();
            btnRectangle = new Button();
            btnDeleting = new Button();
            btnMoving = new Button();
            btnCreating = new Button();
            toolPanel.SuspendLayout();
            SuspendLayout();
            // 
            // toolPanel
            // 
            toolPanel.BackColor = Color.LightGray;
            toolPanel.Controls.Add(btnSave);
            toolPanel.Controls.Add(btnOpen);
            toolPanel.Controls.Add(btnYellow);
            toolPanel.Controls.Add(btnGreen);
            toolPanel.Controls.Add(btnBlue);
            toolPanel.Controls.Add(btnRed);
            toolPanel.Controls.Add(btnBlack);
            toolPanel.Controls.Add(btnHexagon);
            toolPanel.Controls.Add(btnTriangle);
            toolPanel.Controls.Add(btnCircle);
            toolPanel.Controls.Add(btnRectangle);
            toolPanel.Controls.Add(btnDeleting);
            toolPanel.Controls.Add(btnMoving);
            toolPanel.Controls.Add(btnCreating);
            toolPanel.Dock = DockStyle.Right;
            toolPanel.Location = new Point(880, 0);
            toolPanel.Name = "toolPanel";
            toolPanel.Size = new Size(120, 553);
            toolPanel.TabIndex = 0;
            toolPanel.Paint += toolPanel_Paint;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(10, 401);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 30);
            btnSave.TabIndex = 13;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(10, 361);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(100, 30);
            btnOpen.TabIndex = 12;
            btnOpen.Text = "Open";
            btnOpen.UseVisualStyleBackColor = true;
            // 
            // btnYellow
            // 
            btnYellow.BackColor = Color.Yellow;
            btnYellow.Location = new Point(13, 321);
            btnYellow.Name = "btnYellow";
            btnYellow.Size = new Size(20, 20);
            btnYellow.TabIndex = 11;
            btnYellow.UseVisualStyleBackColor = false;
            // 
            // btnGreen
            // 
            btnGreen.BackColor = Color.Green;
            btnGreen.Location = new Point(88, 296);
            btnGreen.Name = "btnGreen";
            btnGreen.Size = new Size(20, 20);
            btnGreen.TabIndex = 10;
            btnGreen.UseVisualStyleBackColor = false;
            // 
            // btnBlue
            // 
            btnBlue.BackColor = Color.Blue;
            btnBlue.Location = new Point(63, 296);
            btnBlue.Name = "btnBlue";
            btnBlue.Size = new Size(20, 20);
            btnBlue.TabIndex = 9;
            btnBlue.UseVisualStyleBackColor = false;
            // 
            // btnRed
            // 
            btnRed.BackColor = Color.Red;
            btnRed.Location = new Point(38, 296);
            btnRed.Name = "btnRed";
            btnRed.Size = new Size(20, 20);
            btnRed.TabIndex = 8;
            btnRed.UseVisualStyleBackColor = false;
            // 
            // btnBlack
            // 
            btnBlack.BackColor = Color.Black;
            btnBlack.Location = new Point(13, 296);
            btnBlack.Name = "btnBlack";
            btnBlack.Size = new Size(20, 20);
            btnBlack.TabIndex = 7;
            btnBlack.UseVisualStyleBackColor = false;
            // 
            // btnHexagon
            // 
            btnHexagon.Location = new Point(10, 260);
            btnHexagon.Name = "btnHexagon";
            btnHexagon.Size = new Size(100, 30);
            btnHexagon.TabIndex = 6;
            btnHexagon.Text = "Hexagon";
            btnHexagon.UseVisualStyleBackColor = true;
            // 
            // btnTriangle
            // 
            btnTriangle.Location = new Point(10, 220);
            btnTriangle.Name = "btnTriangle";
            btnTriangle.Size = new Size(100, 30);
            btnTriangle.TabIndex = 5;
            btnTriangle.Text = "Triangle";
            btnTriangle.UseVisualStyleBackColor = true;
            // 
            // btnCircle
            // 
            btnCircle.Location = new Point(10, 180);
            btnCircle.Name = "btnCircle";
            btnCircle.Size = new Size(100, 30);
            btnCircle.TabIndex = 4;
            btnCircle.Text = "Circle";
            btnCircle.UseVisualStyleBackColor = true;
            // 
            // btnRectangle
            // 
            btnRectangle.Location = new Point(10, 140);
            btnRectangle.Name = "btnRectangle";
            btnRectangle.Size = new Size(100, 30);
            btnRectangle.TabIndex = 3;
            btnRectangle.Text = "Rectangle";
            btnRectangle.UseVisualStyleBackColor = true;
            // 
            // btnDeleting
            // 
            btnDeleting.Location = new Point(10, 90);
            btnDeleting.Name = "btnDeleting";
            btnDeleting.Size = new Size(100, 30);
            btnDeleting.TabIndex = 2;
            btnDeleting.Text = "Delete";
            btnDeleting.UseVisualStyleBackColor = true;
            // 
            // btnMoving
            // 
            btnMoving.Location = new Point(10, 50);
            btnMoving.Name = "btnMoving";
            btnMoving.Size = new Size(100, 30);
            btnMoving.TabIndex = 1;
            btnMoving.Text = "Move";
            btnMoving.UseVisualStyleBackColor = true;
            // 
            // btnCreating
            // 
            btnCreating.BackColor = Color.LightBlue;
            btnCreating.Location = new Point(10, 10);
            btnCreating.Name = "btnCreating";
            btnCreating.Size = new Size(100, 30);
            btnCreating.TabIndex = 0;
            btnCreating.Text = "Create";
            btnCreating.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 553);
            Controls.Add(toolPanel);
            Name = "Form1";
            Text = "Creating Mode";
            toolPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel toolPanel;
        private Button btnCreating;
        private Button btnMoving;
        private Button btnDeleting;
        private Button btnRectangle;
        private Button btnCircle;
        private Button btnTriangle;
        private Button btnHexagon;
        private Button btnBlack;
        private Button btnRed;
        private Button btnBlue;
        private Button btnGreen;
        private Button btnYellow;
        private Button btnSave;
        private Button btnOpen;
    }
}
