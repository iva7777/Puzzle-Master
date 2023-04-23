using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleMaster
{
    public partial class Form1 : Form
    {
        List<PictureBox> pictureBoxList = new List<PictureBox>();
        List<Bitmap> images = new List<Bitmap>();
        List<string> locations = new List<string>();
        List<string> currentLocations = new List<string>();

        private string winPosition;
        private string currentPosition;

        private Bitmap mainBitmap;

        public Form1()
        {
            InitializeComponent();
            menuStrip1.BackColor = Color.FromArgb(122, 173, 243);
        }

        private void OpenFileEvent(object sender, EventArgs e)
        {
            if (pictureBoxList != null)
            {
                foreach (PictureBox picture in pictureBoxList.ToList())
                {
                    this.Controls.Remove(picture);
                }
                pictureBoxList.Clear();
                images.Clear();
                locations.Clear();
                currentLocations.Clear();
                winPosition = string.Empty;
                currentPosition = string.Empty;
                label2.Text = string.Empty;
            }
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files Only | *.jpg; *.jpeg; *.gif; *.png;";
            if (open.ShowDialog() == DialogResult.OK)
            {
                mainBitmap = new Bitmap(open.FileName);
                CreatePictureBoxes();
                AddImages();
            }
        }

        private void CreatePictureBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                PictureBox tempPic = new PictureBox();
                tempPic.Size = new Size(130, 130);
                tempPic.Tag = i.ToString();
                tempPic.Click += OnPicClick;
                pictureBoxList.Add(tempPic);
                locations.Add(tempPic.Tag.ToString());
            }
        }

        private void OnPicClick(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            PictureBox emptyBox = pictureBoxList.Find(x => x.Tag.ToString() == "0");

            Point firstPic = new Point(pictureBox.Location.X, pictureBox.Location.Y);
            Point secondPic = new Point(emptyBox.Location.X, emptyBox.Location.Y);

            var firstIndex = this.Controls.IndexOf(pictureBox);
            var secondIndex = this.Controls.IndexOf(emptyBox);

            if (pictureBox.Right == emptyBox.Left && pictureBox.Location.Y == emptyBox.Location.Y
                || pictureBox.Left == emptyBox.Right && pictureBox.Location.Y == emptyBox.Location.Y
                || pictureBox.Top == emptyBox.Bottom && pictureBox.Location.X == emptyBox.Location.X
                || pictureBox.Bottom == emptyBox.Top && pictureBox.Location.X == emptyBox.Location.X)
            {
                pictureBox.Location = secondPic;
                emptyBox.Location = firstPic;

                this.Controls.SetChildIndex(pictureBox, secondIndex);
                this.Controls.SetChildIndex(emptyBox, firstIndex);
            }

            label2.Text = "";
            currentLocations.Clear();
            CheckGame();
        }

        private void CropImage(Bitmap mainBitmap, int height, int width)
        {
            int x = 0;
            int y = 0;

            for (int blocks = 0; blocks < 9; blocks++)
            {
                Bitmap croppedImage = new Bitmap(height, width);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        croppedImage.SetPixel(i, j, mainBitmap.GetPixel((i + x), (j + y)));
                    }
                }

                images.Add(croppedImage);
                x += 130;
                if (x == 390)
                {
                    x = 0;
                    y += 130;
                }
            }
        }

        private void AddImages()
        {
            Bitmap tempBitmap = new Bitmap(mainBitmap, new Size(390, 390));
            CropImage(tempBitmap, 130, 130);

            for (int i = 1; i < pictureBoxList.Count; i++)
            {
                pictureBoxList[i].BackgroundImage = (Image)images[i];
            }

            PlacePictureBoxesToForm();
        }

        private void PlacePictureBoxesToForm()
        {
            //shuffle the parts of the puzzle
            var shuffleImages = pictureBoxList.OrderBy(a => Guid.NewGuid()).ToList();
            pictureBoxList = shuffleImages;

            int x = 200;
            int y = 25;

            for (int i = 0; i < pictureBoxList.Count; i++)
            {
                pictureBoxList[i].BackColor = Color.Gold;

                if (i == 3 || i == 6)
                {
                    y += 130;
                    x = 200;
                }

                pictureBoxList[i].BorderStyle = BorderStyle.FixedSingle;
                pictureBoxList[i].Location = new Point(x, y);

                this.Controls.Add(pictureBoxList[i]);
                x += 130;
                winPosition += locations[i];
            }
        }

        private void CheckGame()
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    currentLocations.Add(x.Tag.ToString());
                }
            }
            currentPosition = string.Join("", currentLocations);
            label1.Text = winPosition;
            label2.Text = currentPosition;

            if (winPosition == currentPosition)
            {
                label2.Text = "Matched!";
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }
    }
}
