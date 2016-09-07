using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;

namespace IMGProcessor
{
    public partial class Form1 : Form
    {
        // Parametri
        Bitmap bitmap;
        Bitmap previousBitmap;
        Image image;
        Boolean opened = false;
        Boolean saved = false;
        int blurAmount = 1;
        int lastCol = 0;
        float contrast = 0;
        float gamma = 0;

        State state;

        // U konstruktoru radimo potrebnu inicijalizaciju
        public Form1()
        {
            InitializeComponent();

            saveToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            trackBar3.Enabled = false;
            trackBar4.Enabled = false;

            trackBar1.Value = 1;
            trackBar2.Value = 0;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
 
        }


        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
 
        }


        // Grayscale
        private void button3_Click(object sender, EventArgs e)
        {
            
            if (opened)
            {

                int i = 1;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color originalColor = bitmap.GetPixel(x, y);

                        //int grayscale = (int)((originalColor.R * .3) + (originalColor.G * .59) + (originalColor.B * .11));
                        //int grayscale = (int)((originalColor.R / .3) + (originalColor.G / .59) + (originalColor.B / .11));

                        int red = ((int)(originalColor.R / .7));
                        red = red > 255 ? 255 : red;

                        int blue = ((int)(originalColor.B / .41));
                        blue = blue > 255 ? 255 : blue;

                        int green = ((int)(originalColor.G / .89));
                        green = green > 255 ? 255 : green;

                        Color grayscaleColor = Color.FromArgb(red, green, blue);

                        bitmap.SetPixel(x, y, grayscaleColor);
                    }
                }

                pictureBox1.Image = bitmap;

                previousBitmap = new Bitmap(bitmap);

                state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
                undoToolStripMenuItem.Enabled = true;

            }
            else
            {
                MessageBox.Show("You need to open an image first");
            } 

        }

        // Blur
        private void button4_Click(object sender, EventArgs e)
        {
            if (opened)
            {
                for (int x = blurAmount; x <= bitmap.Width - blurAmount; x++)
                {
                    for (int y = blurAmount; y <= bitmap.Height - blurAmount; y++)
                    {
                        try
                        {
                            Color prevX = bitmap.GetPixel(x - blurAmount, y);
                            Color nextX = bitmap.GetPixel(x + blurAmount, y);
                            Color prevY = bitmap.GetPixel(x, y - blurAmount);
                            Color nextY = bitmap.GetPixel(x, y + blurAmount);

                            int averageR = (int)((prevX.R + nextX.R + prevY.R + nextY.R) / 4);
                            int averageG = (int)((prevX.G + nextX.G + prevY.G + nextY.G) / 4);
                            int averageB = (int)((prevX.B + nextX.B + prevY.B + nextY.B) / 4);

                            Color newColor = Color.FromArgb(averageR, averageG, averageB);

                            bitmap.SetPixel(x, y, newColor);
                        }
                        catch (Exception) { }
                    }
                }

                pictureBox1.Image = bitmap;

                previousBitmap = new Bitmap(bitmap);

                state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
                undoToolStripMenuItem.Enabled = true;
            }
            else
            {
                MessageBox.Show("You need to open an image first");
            }
       
        }

        private void updateBlur(object sender, EventArgs e)
        {
            if (!opened)
            {
                MessageBox.Show("You need to open an image first");
                return;
            }

            blurAmount = int.Parse(trackBar1.Value.ToString());
        }

        private void updateBrightness(object sender, EventArgs e)
        {
            if (!opened)
            {
                MessageBox.Show("You need to open an image first");
                return;
            }

            label3.Text = trackBar2.Value.ToString();

            bitmap = adjustBrightness(previousBitmap, trackBar2.Value);

            pictureBox1.Image = bitmap;

            state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
            undoToolStripMenuItem.Enabled = true;
        }

        public static Bitmap adjustBrightness (Bitmap image, int value)
        {
            float finalValue = (float)value / 255.0f;

            Bitmap newBitmap = new Bitmap(image.Width, image.Height);

            float[][] floatMatrix = {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {finalValue, finalValue, finalValue, 1, 1}
                        };

            ColorMatrix colorMatrix = new ColorMatrix(floatMatrix);

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            Graphics graphics = Graphics.FromImage(newBitmap);

            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes );

            graphics.Dispose();

            attributes.Dispose();

            return newBitmap;
        }

        // Negative
        private void button5_Click(object sender, EventArgs e)
        {
            if (opened)
            {
                for (int x = 0; x <= bitmap.Width - 1; x++)
                {
                    for (int y = 0; y <= bitmap.Height - 1; y++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);

                        int red = pixel.R;
                        int green = pixel.G;
                        int blue = pixel.B;

                        bitmap.SetPixel(x, y, Color.FromArgb(255 - red, 255 - green, 255 - blue));

                    }
                }

                pictureBox1.Image = bitmap;

                previousBitmap = new Bitmap(bitmap);

                state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
                undoToolStripMenuItem.Enabled = true;
            }
            else
            {
                MessageBox.Show("You need to open an image first");
            }
        }

        // Emboss
        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap nB = new Bitmap(bitmap.Width, bitmap.Height);

            for (int x = 1; x <= bitmap.Width - 1; x++)
            {
                for (int y = 1; y <= bitmap.Height - 1; y++)
                {
                    nB.SetPixel(x, y, Color.DarkGray);
                }
            }

            for (int x = 1; x <= bitmap.Width - 1; x++)
            {
                for (int y = 1; y <= bitmap.Height - 1; y++)
                {
                    try
                    {
                        Color pixel = bitmap.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }


                    }
                    catch (Exception) { }
                }
            }

            for (int y = 1; y <= bitmap.Height - 1; y++)
            {

                for (int x = 1; x <= bitmap.Width - 1; x++)
                {
                    try
                    {
                        Color pixel = bitmap.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }

                    }
                    catch (Exception) { }
                }

            }

            bitmap = nB;

            pictureBox1.Image = bitmap;

            previousBitmap = new Bitmap(bitmap);

            state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
            undoToolStripMenuItem.Enabled = true;
           
        }

        // Contrast
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label4.Text = trackBar3.Value.ToString();

            contrast = 0.04f * trackBar3.Value;

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            Graphics graphics = Graphics.FromImage(newBitmap);

            ImageAttributes attributes = new ImageAttributes();

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]{
                    new float[]{contrast, 0f, 0f, 0f, 0f},
                    new float[]{0f, contrast, 0f, 0f, 0f},
                    new float[]{0f, 0f, contrast, 0f, 0f},
                    new float[]{0f, 0f, 0f, 1f, 0f},
                    new float[]{0.001f, 0.001f, 0.001f, 0f, 1f}
                }
                );

            attributes.SetColorMatrix(colorMatrix);

            graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);

            graphics.Dispose();

            attributes.Dispose();

            pictureBox1.Image = newBitmap;

            previousBitmap = new Bitmap(newBitmap);

            state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
        }

        // Gamma
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label6.Text = trackBar4.Value.ToString();

            gamma = 0.04f * trackBar4.Value;

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            Graphics graphics = Graphics.FromImage(newBitmap);

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetGamma(gamma);

            graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);

            graphics.Dispose();

            attributes.Dispose();

            pictureBox1.Image = newBitmap;

            previousBitmap = new Bitmap(newBitmap);

            state.save(pictureBox1.Image, trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
            undoToolStripMenuItem.Enabled = true;
        }

        // Open
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.Image = image;
                bitmap = new Bitmap(openFileDialog1.FileName);
                previousBitmap = new Bitmap(bitmap);

                opened = true;
                saved = false;

                saveToolStripMenuItem.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                trackBar1.Enabled = true;
                trackBar2.Enabled = true;
                trackBar3.Enabled = true;
                trackBar4.Enabled = true;

                trackBar1.Value = trackBar1.Minimum;
                trackBar2.Value = trackBar2.Minimum;
                trackBar3.Value = 25;
                trackBar4.Value = 23;

                label3.Text = "0";
                label4.Text = "25";
                label6.Text = "23";

                State.undoStack.Clear();
                State.redoStack.Clear();

                state = new State(pictureBox1.Image,trackBar1.Value, trackBar2.Value, trackBar3.Value, trackBar4.Value);
            }
        }

        // Save
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (opened)
            {
                DialogResult dialogResult = saveFileDialog1.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    String imageFilter = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower();

                    if (imageFilter == "jpg")
                    {
                        image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                    }

                    if (imageFilter == "bmp")
                    {
                        image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                    }

                    if (imageFilter == "png")
                    {
                        image.Save(saveFileDialog1.FileName, ImageFormat.Png);
                    }

                    if (imageFilter == "gif")
                    {
                        image.Save(saveFileDialog1.FileName, ImageFormat.Gif);
                    }

                    saved = true;
                }
            }
            else
            {
                MessageBox.Show("You need to open an image first");
            }
        }

        // Undo
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            state.undo();
            pictureBox1.Image = state.Image;
            bitmap = new Bitmap(state.Image);
            trackBar1.Value = state.Blur;
            trackBar2.Value = state.Brightness;
            trackBar3.Value = state.Contrast;
            trackBar4.Value = state.Gamma;

            label3.Text = trackBar2.Value.ToString();
            label4.Text = trackBar3.Value.ToString();
            label6.Text = trackBar4.Value.ToString();

            if (State.undoStack.Count == 0) undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = true;
        }

        // Redo
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            state.redo();
            pictureBox1.Image = state.Image;
            bitmap = new Bitmap(state.Image);
            trackBar1.Value = state.Blur;
            trackBar2.Value = state.Brightness;
            trackBar3.Value = state.Contrast;
            trackBar4.Value = state.Gamma;

            label3.Text = trackBar2.Value.ToString();
            label4.Text = trackBar3.Value.ToString();
            label6.Text = trackBar4.Value.ToString();

            if (State.redoStack.Count == 0) redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = true;
        }

        // Help
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open an image, than enjoy!", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Close
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saved == false)
            {
                DialogResult result = MessageBox.Show("Are you sure?", "Exit",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }
            Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Author: Dragan Veljovic\nPrinciples of Modern Telecomunications\n" +
                "School of Electrical Engineering Belgrade\nVersion 1.0\n2013", "About", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
