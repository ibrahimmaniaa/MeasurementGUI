using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace MeasurementGUI
{
    public partial class mainForm : Form
    {
        private string imgFilePath;
        private Image img;
        private bool zoomInEnabled = false;
        private bool zoomOutEnabled = false;
        private double zoomFactor;
        private int[] idx = new int[2];
        private double BoehlerAngle;
        private Cursor zoomCursor;
        private int numFilesInDir;
        private bool linesShown = false;

        private List<Tuple<Rectangle, Rectangle, Color>> endpoints = new List<Tuple<Rectangle, Rectangle, Color>>();
        private List<Tuple<Rectangle, Rectangle, Color>> originEndpoints;
        

        private bool isMouseDown = false;
        private bool started = false;

        private Size formOriginSize;
        private Rectangle panelOriginRect;
        private Rectangle pictureBoxOriginRect;
        private Rectangle uploadBtnOriginRect;
        private Rectangle startBtnOriginRect;
        private Rectangle homeBtnOriginRect;
        private Rectangle zoomInBtnOriginRect;
        private Rectangle zoomOutBtnOriginRect;
        private Rectangle saveBtnOriginRect;
        private Rectangle anglePictureBoxOriginRect;
        private Rectangle showLinesOriginRect;


        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.Text = "MeasurementGUI";
            pictureBox.AllowDrop = true;

            this.MinimumSize = this.Size;

            //GetInitialLinePosition();

            formOriginSize = this.Size;
            panelOriginRect = new Rectangle(panel1.Location.X, panel1.Location.Y, panel1.Width, panel1.Height);
            pictureBoxOriginRect = new Rectangle(pictureBox.Location.X, pictureBox.Location.Y, pictureBox.Width, pictureBox.Height);
            uploadBtnOriginRect = new Rectangle(uploadImgBtn.Location.X, uploadImgBtn.Location.Y, uploadImgBtn.Width, uploadImgBtn.Height);
            startBtnOriginRect = new Rectangle(startBtn.Location.X, startBtn.Location.Y, startBtn.Width, startBtn.Height);
            homeBtnOriginRect = new Rectangle(homeBtn.Location.X, homeBtn.Location.Y, homeBtn.Width, homeBtn.Height);
            zoomInBtnOriginRect = new Rectangle(zoomInBtn.Location.X, zoomInBtn.Location.Y, zoomInBtn.Width, zoomInBtn.Height);
            zoomOutBtnOriginRect = new Rectangle(zoomOutBtn.Location.X, zoomOutBtn.Location.Y, zoomOutBtn.Width, zoomOutBtn.Height);
            saveBtnOriginRect = new Rectangle(saveBtn.Location.X, saveBtn.Location.Y, saveBtn.Width, saveBtn.Height);
            anglePictureBoxOriginRect = new Rectangle(anglePictureBox.Location.X, anglePictureBox.Location.Y, anglePictureBox.Width, anglePictureBox.Height);
            showLinesOriginRect = new Rectangle(showLinesCheckBox.Location.X, showLinesCheckBox.Location.Y, showLinesCheckBox.Width, showLinesCheckBox.Height);

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("MeasurementGUI.res.zoom.cur");
            
            zoomCursor = new Cursor(myStream);

        }

        /**************************************************************************************************************************************
         ************************************************       upload image        ***********************************************************
         *************************************************************************************************************************************/

        private void uploadImgBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "Image Files(*.png; *.jpg; *.jpeg; *.bmp)|*.png; *.jpg; *.jpeg; *.bmp",
                Title = "Select image"
            };

            if (open.ShowDialog() == DialogResult.OK)
            {
                img = Image.FromFile(open.FileName);
                pictureBox.Image = img;
                imgFilePath = open.FileName;
                this.Text = "";
                this.Text += "MeasurementGUI -- " + imgFilePath;
                pictureBox.BackColor = this.BackColor;
                zoomFactor = 1;
                zoomInBtn.Enabled = true;
                zoomOutBtn.Enabled = true;
                startBtn.Focus();

                string folderName = Path.GetDirectoryName(imgFilePath);
                numFilesInDir = Directory.GetFiles(folderName, "*.*").Length;
                checkLastImgTimer.Enabled = true;

                endpoints.Clear();
                linesShown = false;
                started = false;

            }
        }

        private void pictureBox_DragDrop(object sender, DragEventArgs e)
        {
            zoomFactor = 1;
            zoomInBtn.Enabled = true;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            string temp = files[0];
            if (temp.EndsWith("jpg") || temp.EndsWith("jpeg") || temp.EndsWith("png") || temp.EndsWith("bmp"))
            {
                imgFilePath = temp;
                img = Image.FromFile(imgFilePath);
                pictureBox.Image = img;
                this.Text = "";
                this.Text += "MeasurementGUI -- " + imgFilePath;
                pictureBox.BackColor = this.BackColor;
                zoomFactor = 1;
                zoomInBtn.Enabled = true;

                string folderName = Path.GetDirectoryName(imgFilePath);
                numFilesInDir = Directory.GetFiles(folderName, "*.*").Length;
                checkLastImgTimer.Enabled = true;

                endpoints.Clear();
                linesShown = false;
                started = false;
            }
                
        }

        private void pictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap) &&
            (e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                // Allow this.
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Don't allow any other drop.
                e.Effect = DragDropEffects.None;
            }
            try
            {
                e.Effect = DragDropEffects.Copy;
            }
            catch { }
      
        }

        /**************************************************************************************************************************************
         **********************************************         save button       *************************************************************
         *************************************************************************************************************************************/

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png",
                Title = "Save an Image File"
            };

            string[] subs1 = imgFilePath.Split('\\');
            string[] subs2 = subs1[subs1.Length - 1].Split('.');
            sfd.FileName = subs2[0] + "_" + BoehlerAngle + "_degrees";

            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    //pictureBox.Image.Save(sfd.FileName, format);

                    //Bitmap bmp = new Bitmap(Convert.ToInt32(img.Width * zoomFactor), Convert.ToInt32(img.Height * zoomFactor));
                    //pictureBox.DrawToBitmap(bmp, new Rectangle(0, 0, Convert.ToInt32(img.Width * zoomFactor), Convert.ToInt32(img.Height * zoomFactor)));

                    Bitmap bmp = new Bitmap(Convert.ToInt32(pictureBox.Width), Convert.ToInt32(pictureBox.Height));
                    pictureBox.DrawToBitmap(bmp, new Rectangle(0, 0, Convert.ToInt32(pictureBox.Width), Convert.ToInt32(pictureBox.Height)));


                    Bitmap bmp1 = new Bitmap(img.Width, img.Height);
                    var graph = Graphics.FromImage(bmp1);
                    graph.DrawImage(bmp, new Rectangle(0, 0, bmp1.Width, bmp1.Height));

                    bmp1.Save(sfd.FileName);
                }                
            }

        }
            

        /**************************************************************************************************************************************
         *********************************************         zoom functions       ***********************************************************
         *************************************************************************************************************************************/
        

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (zoomInEnabled)
            {
                if (zoomFactor <= 3) { zoomFactor += 0.5;  pictureBox.Image = Zoom(img); }
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox.Dock = DockStyle.None;
                UpdateLinePosition();
            }

            if (zoomOutEnabled)
            {
                if (zoomFactor > 1.5)
                {
                    zoomFactor -= 0.5;
                    pictureBox.Image = Zoom(img);
                    pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox.Dock = DockStyle.None;
                    UpdateLinePosition();
                }
                else if (zoomFactor == 1.5) { homeBtn.PerformClick(); }
                
            }
            //pictureBox.Refresh();
            Refresh();
        }

        private Image Zoom(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            Bitmap zoomed = new Bitmap(Convert.ToInt32(img.Width * zoomFactor), Convert.ToInt32(img.Height * zoomFactor));

            using (Graphics g = Graphics.FromImage(zoomed))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.DrawImage(bmp, new Rectangle(0, 0, zoomed.Width, zoomed.Height));
            }
            return zoomed;
        }


        /**************************************************************************************************************************************
         **************************************************         buttons      **************************************************************
         *************************************************************************************************************************************/

        private void homeBtn_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Dock = DockStyle.Fill;
                HomeLinePosition();     
                pictureBox.Image = img;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            zoomInBtn.FlatStyle = FlatStyle.Popup;
            pictureBox.Cursor = Cursors.Default;
            pictureBox.Size = panel1.Size;
            zoomInEnabled = false;
            zoomOutEnabled = false;
            zoomFactor = 1;

            pictureBox.Refresh();
        }

        private void zoomInBtn_Click(object sender, EventArgs e)
        {
            if (zoomInBtn.FlatStyle == FlatStyle.Flat)
            {
                zoomInBtn.FlatStyle = FlatStyle.Popup;
                pictureBox.Cursor = Cursors.Default;
                zoomInEnabled = false;
                startBtn.Focus();
            }
            else
            {
                zoomInBtn.FlatStyle = FlatStyle.Flat;
                pictureBox.Cursor = zoomCursor;
                zoomInEnabled = true;
                zoomOutEnabled = false;
                zoomOutBtn.FlatStyle = FlatStyle.Popup;
                startBtn.Focus();
            }

        }

        private void zoomOutBtn_Click(object sender, EventArgs e)
        {
            if (zoomOutBtn.FlatStyle == FlatStyle.Flat)
            {
                zoomOutBtn.FlatStyle = FlatStyle.Popup;
                pictureBox.Cursor = Cursors.Default;
                zoomOutEnabled = false;
                startBtn.Focus();
            }
            else
            {
                zoomOutBtn.FlatStyle = FlatStyle.Flat;
                pictureBox.Cursor = zoomCursor;
                zoomOutEnabled = true;
                zoomInEnabled = false;
                zoomInBtn.FlatStyle = FlatStyle.Popup;
                startBtn.Focus();
            }
            
        }

        /**************************************************************************************************************************************
         **************************************************      Start button    **************************************************************
         *************************************************************************************************************************************/

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                saveBtn.Enabled = true;
                started = true;
                if (zoomInEnabled | zoomOutEnabled)
                {
                    UpdateLinePosition();
                }
                else { HomeLinePosition(); }
                
                pictureBox.Refresh();

                anglePictureBox.Refresh();
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (started)
            {
                if (showLinesCheckBox.Checked)
                {
                    foreach (var line in endpoints)
                    {
                        try
                        {
                            e.Graphics.FillRectangle(new SolidBrush(line.Item3), line.Item1);
                            e.Graphics.FillRectangle(new SolidBrush(line.Item3), line.Item2);
                        }
                        catch { }

                    }

                    for (int i = 0; i < endpoints.Count; i++)
                    {
                        Pen p = new Pen(endpoints[i].Item3);
                        e.Graphics.DrawLine(p, endpoints[i].Item1.Location, endpoints[i].Item2.Location);
                    }
                }
            }

        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            idx = GetClickedEndPoint(e.Location);
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown && idx[0] != 999 && !zoomInEnabled && !zoomOutEnabled)
            {
                Rectangle rect;
                if (idx[1] == 0) { rect = endpoints[idx[0]].Item1; }
                else { rect = endpoints[idx[0]].Item2; }
                
                rect.Location = e.Location;

                if (rect.Right > pictureBox.Width)
                {
                    rect.X = pictureBox.Width - rect.Width;
                }
                if (rect.Top < 0)
                {
                    rect.Y = 0;
                }
                if (rect.Left < 0)
                {
                    rect.X = 0;
                }
                if (rect.Bottom > pictureBox.Height)
                {
                    rect.Y = pictureBox.Height - rect.Height;
                }

                if (idx[1] == 0) { endpoints[idx[0]] = Tuple.Create(rect, endpoints[idx[0]].Item2, endpoints[idx[0]].Item3); }
                else { endpoints[idx[0]] = Tuple.Create(endpoints[idx[0]].Item1, rect, endpoints[idx[0]].Item3); }

                GetPerpendicularLine();
                Refresh();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }


        /**************************************************************************************************************************************
         ****************************************************      helpers    *****************************************************************
         *************************************************************************************************************************************/

        private int[] GetClickedEndPoint(Point location)
        {
            int[] idx = { 0, 0 };
            foreach (var line in endpoints)
            {
                if (location.X >= line.Item1.X && location.Y >= line.Item1.Y && location.X <= line.Item1.Right && location.Y <= line.Item1.Bottom)
                {
                    idx[1] = 0;
                    return idx;
                }

                if (location.X >= line.Item2.X && location.Y >= line.Item2.Y && location.X <= line.Item2.Right && location.Y <= line.Item2.Bottom)
                {
                    idx[1] = 1;
                    return idx;
                }

                idx[0] += 1;
            }

            idx = new int[]{ 999, 999};
            return idx;
        }

        private void anglePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (started & linesShown)
            {
                Point p11 = endpoints[0].Item1.Location;
                Point p12 = endpoints[0].Item2.Location;
                Point p21 = endpoints[2].Item2.Location;
                Point p22 = endpoints[2].Item1.Location;

                double[] vector1 = { p11.X - p12.X, p11.Y - p12.Y };
                double[] vector2 = { p21.X - p22.X, p21.Y - p22.Y };
        
                BoehlerAngle = Math.Acos(DotProduct(vector1, vector2)/(Magnitude(vector1)*Magnitude(vector2)) );

                BoehlerAngle = BoehlerAngle * 180 / Math.PI;
                BoehlerAngle = Math.Round(BoehlerAngle, 2);
                BoehlerAngle = Math.Min(BoehlerAngle, 180-BoehlerAngle);

                using (Font myFont = new Font("Arial", 12))
                {
                    e.Graphics.DrawString("Böhler Angle =", myFont, Brushes.Black, new Point(0, 0));
                }

                using (Font myFont = new Font("Arial", 30, FontStyle.Bold))
                {
                    e.Graphics.DrawString(BoehlerAngle + "°", myFont, Brushes.Black, new Point(45, 15));
                }
            }
            
        }

        private  double DotProduct(double[] vec1, double[] vec2)
        {
            double tVal = 0;
            for (int x = 0; x < vec1.Length; x++)
            {
                tVal += vec1[x] * vec2[x];
            }
            return tVal;
        }

        private double Magnitude(double[] vec)
        {
            double tVal = 0;
            for (int x = 0; x < vec.Length; x++)
            {
                tVal += Math.Pow(vec[x], 2);
            }
            return Math.Sqrt(tVal);
        }

        private void HomeLinePosition()
        {
            if (linesShown) { endpoints = new List<Tuple<Rectangle, Rectangle, Color>>(originEndpoints); }
            else if (started){ GetInitialLinePosition(); }
            else { return; }
            

            //linesShown = true;

            //double imgAspectRatio = (double) img.Width / img.Height;
            //double pictureBoxAspectRatio = (double) pictureBox.Width / pictureBox.Height;

            double xOffset = 0;
            double yOffset = 0;
            //double ratio;

            //if (pictureBoxAspectRatio > imgAspectRatio)
            //{
            //    ratio = (double)pictureBox.Height / img.Height;
                
            //    xOffset = pictureBox.Width - img.Width * ratio;
            //    xOffset /= 2;
                
            //    if (pictureBox.Height > img.Height) { ratio = 1 / ratio; xOffset = 0; }
            //}
            //else
            //{
            //    ratio = (double)pictureBox.Width / img.Width; xOffset = 35;
               
            //    yOffset = pictureBox.Height - img.Height * ratio;
            //    yOffset /= 2;
                
            //    if (pictureBox.Width > img.Width) { ratio = 1 / ratio; yOffset = 0; }
            //}

            double ratioX = (double)pictureBox.Width / img.Width;
            double ratioY = (double)pictureBox.Height / img.Height;

            for (int i=0; i<endpoints.Count(); i++)
            {
                double x = ratioX * endpoints[i].Item1.Location.X;
                double y = ratioY * endpoints[i].Item1.Location.Y;
                Rectangle rect1 = new Rectangle((int)(x + xOffset), (int)(y + yOffset), endpoints[i].Item1.Width, endpoints[i].Item1.Height);

                x = ratioX * endpoints[i].Item2.Location.X;
                y = ratioY * endpoints[i].Item2.Location.Y;
                Rectangle rect2 = new Rectangle((int)(x + xOffset), (int)(y + yOffset), endpoints[i].Item2.Width, endpoints[i].Item2.Height);

                endpoints[i] = Tuple.Create(rect1, rect2, endpoints[i].Item3);
            }
            GetPerpendicularLine();
        }

        private void GetInitialLinePosition()
        {
            //this.UseWaitCursor = true; 
            
            var psi = new ProcessStartInfo();

            IniFile iniFile = new IniFile("config.ini");
            psi.FileName = @iniFile.Read("pythonInterpreter", "input");

            var script = @"predict.py";

            psi.Arguments = $"\"{script}\" \"{imgFilePath}\"";

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var error = "";
            var results = "";

            using (var process = Process.Start(psi))
            {
                results = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                linesShown = false;
                return;
            }

            string[] subs = results.Split(' ');
            Rectangle rectJointLineL = new Rectangle(Convert.ToInt16(subs[0]), Convert.ToInt16(subs[1]), 10, 10);
            Rectangle rectJointLineR = new Rectangle(Convert.ToInt16(subs[2]), Convert.ToInt16(subs[3]), 10, 10);
            Rectangle centerLineDown = new Rectangle(Convert.ToInt16(subs[4]), Convert.ToInt16(subs[5]), 10, 10);
            Rectangle centerLineUp = new Rectangle(Convert.ToInt16(subs[6]), Convert.ToInt16(subs[7]), 10, 10);

            //Rectangle rectJointLineL = new Rectangle(225, 456, 10, 10);
            //Rectangle rectJointLineR = new Rectangle(360, 519, 10, 10);
            //Rectangle centerLineDown = new Rectangle(267, 950, 10, 10);
            //Rectangle centerLineUp = new Rectangle(284, 587, 10, 10);

            endpoints.Clear();
            endpoints.Add(Tuple.Create(rectJointLineL, rectJointLineR, Color.Yellow));
            endpoints.Add(Tuple.Create(centerLineDown, centerLineUp, Color.Blue));

            originEndpoints = new List<Tuple<Rectangle, Rectangle, Color>>(endpoints);
            linesShown = true;
            //this.UseWaitCursor = false;
        }

        private void UpdateLinePosition()
        {
            if (linesShown)
            {
                endpoints = new List<Tuple<Rectangle, Rectangle, Color>>(originEndpoints);

                for (int i = 0; i < endpoints.Count(); i++)
                {
                    double x = zoomFactor * endpoints[i].Item1.Location.X;
                    double y = zoomFactor * endpoints[i].Item1.Location.Y;
                    Rectangle rect1 = new Rectangle((int)x, (int)y, endpoints[i].Item1.Width, endpoints[i].Item1.Height);

                    x = zoomFactor * endpoints[i].Item2.Location.X;
                    y = zoomFactor * endpoints[i].Item2.Location.Y;
                    Rectangle rect2 = new Rectangle((int)x, (int)y, endpoints[i].Item2.Width, endpoints[i].Item2.Height);

                    endpoints[i] = Tuple.Create(rect1, rect2, endpoints[i].Item3);
                }
                GetPerpendicularLine();
            }
            
        }

        private void GetPerpendicularLine()
        {
            if (linesShown)
            {
                Point p1 = endpoints[1].Item1.Location;
                Point p2 = endpoints[1].Item2.Location;
                double slope;

                slope = ((double)(p2.Y - p1.Y) / (double)(p2.X - p1.X));
                if (slope != 0) { slope = -1 / slope; }
                else { slope = 1; }

                Point upperPoint;

                if (p1.Y > p2.Y) { upperPoint = p2; }
                else { upperPoint = p1; }

                double b = upperPoint.Y - slope * upperPoint.X;

                Point L = new Point((int)upperPoint.X + 100, (int)(slope * (upperPoint.X + 100) + b));
                Point R = new Point((int)upperPoint.X - 100, (int)(slope * (upperPoint.X - 100) + b));

                Rectangle perpendicularLineL = new Rectangle(L.X, L.Y, 1, 1);
                Rectangle perpendicularLineR = new Rectangle(R.X, R.Y, 1, 1);

                if (endpoints.Count == 3)
                {
                    endpoints[2] = Tuple.Create(perpendicularLineL, perpendicularLineR, endpoints[1].Item3);
                }
                else
                {
                    endpoints.Add(Tuple.Create(perpendicularLineL, perpendicularLineR, endpoints[1].Item3));
                }
            }
            
            
        }

        /*##################################################################################################################################*/
        /*##########################################     Form Resizing     #################################################################*/
        /*##################################################################################################################################*/

        private void ResizeControl(Rectangle originCtrlRect, Control ctrl)
        {
            float xRatio = (float)(this.Width) / (formOriginSize.Width);
            float yRatio = (float)(this.Height) / (formOriginSize.Height);

            int newX = (int)(originCtrlRect.X * xRatio);
            int newY = (int)(originCtrlRect.Y * yRatio);
            int newWidth = (int)(originCtrlRect.Width * xRatio);
            int newHeight = (int)(originCtrlRect.Height * yRatio);

            ctrl.Location = new Point(newX, newY);
            ctrl.Size = new Size(newWidth, newHeight);
        }

        private void ResizeChildrenControls()
        {
            ResizeControl(panelOriginRect, panel1);
            ResizeControl(pictureBoxOriginRect, pictureBox);
            ResizeControl(uploadBtnOriginRect, uploadImgBtn);
            ResizeControl(startBtnOriginRect, startBtn);
            ResizeControl(homeBtnOriginRect, homeBtn);
            ResizeControl(zoomInBtnOriginRect, zoomInBtn);
            ResizeControl(zoomOutBtnOriginRect, zoomOutBtn);
            ResizeControl(saveBtnOriginRect, saveBtn);
            ResizeControl(anglePictureBoxOriginRect, anglePictureBox);
            ResizeControl(showLinesOriginRect, showLinesCheckBox);
         
        }

        private void mainForm_SizeChanged(object sender, EventArgs e)
        {
            ResizeChildrenControls();
            if (pictureBox.Image != null)
            {
                HomeLinePosition();
                pictureBox.Refresh();
            }
            
        }


        /*##################################################################################################################################*/
        /*##################################################     Timer     #################################################################*/
        /*##################################################################################################################################*/

        private void checkLastImgTimer_Tick(object sender, EventArgs e)
        {
            string folderName = Path.GetDirectoryName(imgFilePath);

            int currNumFilesInDir = Directory.GetFiles(folderName, "*.*").Length;

            if (currNumFilesInDir > numFilesInDir)
            {
                numFilesInDir = currNumFilesInDir;

                var directory = new DirectoryInfo(folderName);
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

                string temp = myFile.FullName;

                if (temp.EndsWith("jpg") || temp.EndsWith("jpeg") || temp.EndsWith("png") || temp.EndsWith("bmp"))
                {
                    imgFilePath = temp;

                    img = Image.FromFile(imgFilePath);
                    pictureBox.Image = img;
                    this.Text = "";
                    this.Text += "MeasurementGUI -- " + imgFilePath;
                    pictureBox.BackColor = this.BackColor;
                    zoomFactor = 1;

                    endpoints.Clear();
                    linesShown = false;

                    homeBtn.PerformClick();
                    startBtn.PerformClick();
                    //homeBtn.PerformClick();
                }
            }
        }


        private void showLinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (started)
            {
                if (showLinesCheckBox.Checked) { pictureBox.Refresh(); }
                else
                {
                    if (zoomInEnabled || zoomOutEnabled) { pictureBox.Image = Zoom(img); }
                    else { pictureBox.Image = img; }
                    
                }
            }
        }


        /*##################################################################################################################################*/
        /*##################################################      End     ##################################################################*/
        /*##################################################################################################################################*/


    }
}
