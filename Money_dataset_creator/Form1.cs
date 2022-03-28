using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace dataset_creator
{
    public partial class Form1 : Form
    {
        string name;

        TextBox numberTestSamples = new TextBox();
        TextBox sampleNames = new TextBox();
        Button start = new Button();

        Button rotate = new Button();
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(400, 200);

            sampleNames.Size = new Size(100, 35);
            sampleNames.Location = new Point(10, 10);
            sampleNames.Text = "Name_Of_samples";
            this.Controls.Add(sampleNames);

            numberTestSamples.Size = new Size(100, 35);
            numberTestSamples.Location = new Point(110 + 100, 10);
            numberTestSamples.Text = "16";
            this.Controls.Add(numberTestSamples);

            start.Size = new Size(100, 30);
            start.Location = new Point(this.ClientSize.Width - start.Width - 10, this.ClientSize.Height - start.Height - 10);
            start.Text = "create samples";
            start.Click += createSamples;
            this.Controls.Add(start);

            rotate.Size = new Size(100, 30);
            rotate.Location = new Point(this.ClientSize.Width - start.Width - 10, this.ClientSize.Height - start.Height - 110);
            rotate.Text = "Rotate samples";
            rotate.Click += flipPictures;
            this.Controls.Add(rotate);

            //createNewSet("test",1);
        }

        private void PictureCreate()
        {
            int samples = Convert.ToInt32(numberTestSamples.Text);
            int numCores = Environment.ProcessorCount;
            int coreCalcRange = samples / numCores;
            Thread thread = Thread.CurrentThread;
            int id = thread.ManagedThreadId;

            for (int i = coreCalcRange * (id - 1); i < (coreCalcRange * (id - 1)) + coreCalcRange; i++)
            {
                createNewSet(name, i, false);
            }
        }

        public void createSamples(object sender, EventArgs e)
        {
            name = sampleNames.Text;
            int samples = Convert.ToInt32(numberTestSamples.Text);
            int numCores = Environment.ProcessorCount;

            for (int i = 0; i < numCores; i++)
            {
                Thread t = new Thread(new ThreadStart(PictureCreate));
                t.Start();
            }

        }

        private void flipPictures(object sender, EventArgs e)
        {
            int imWidth = 0;
            int imHeight = 0;
            int counter = 0;
            string sorucePath = @"C:\Users\Mick\Desktop\geldbilder\Reale_copy";
            string storePath = @"C:\Users\Mick\Desktop\geldbilder\real_rotaed";
            string[] fileNames = Directory.GetFiles(sorucePath, "*");
            List<string> allPicturesPath = new List<string>();
            List<string> allTextPath = new List<string>();

            foreach (string file in fileNames)
            {
                if (file.EndsWith(".jpg"))
                {
                    allPicturesPath.Add(file);
                }
                else if (file.EndsWith(".txt"))
                {
                    allTextPath.Add(file);
                }
            }
               

            foreach (string pictureToRotate in allPicturesPath)
            {
                Bitmap bm = new Bitmap(pictureToRotate);
                imWidth = bm.Width;
                imHeight = bm.Height;
                int tmpHeight = bm.Width;
                int tmpWidth = bm.Height;
                //int tmpHeight = bm.Height;
                //int tmpWidth = bm.Width;
                int rotation = 90;
                string saveNamePrefix = pictureToRotate.Remove(0, sorucePath.Length);
                string[] splittedName = saveNamePrefix.Split('.');
                string[] lines = File.ReadAllLines(allTextPath[counter]);
                string[] allFileNames = new string[4];
                string[] allTextFileNames = new string[4];
                allFileNames[0] = pictureToRotate;
                allTextFileNames[0] = allTextPath[counter];
                Random rnd = new Random();
                for (int i = 0; i < 3; i++)
                {
                    string[] writeLines = new string[lines.Length];
                    Bitmap tmpBm = new Bitmap(tmpWidth, tmpHeight);
                    using (Graphics g = Graphics.FromImage(tmpBm))
                    {
                        g.RotateTransform(rotation);
                        if (rotation == 90)
                        {
                            g.DrawImage(bm, new Point(0, -tmpWidth));
                            //g.DrawImage(bm, new Point(0, 0));
                            for (int j = 0; j < writeLines.Length; j++)
                            {
                                string[] splitedLine = lines[j].Split(' ');
                                float xpos = float.Parse(splitedLine[1]) / 1000000;
                                float ypos = float.Parse(splitedLine[2]) / 1000000;

                                ypos = 1.0f - ypos;
                                writeLines[j] += splitedLine[0] + " " + ypos + " " + xpos + " " + splitedLine[4] + " " + splitedLine[3];
                                writeLines[j] = writeLines[j].Replace(',', '.');
                            }
                        }
                        else if (rotation == 180)
                        {
                            g.DrawImage(bm, new Point(-tmpWidth, -tmpHeight));
                            for (int j = 0; j < writeLines.Length; j++)
                            {
                                string[] splitedLine = lines[j].Split(' ');
                                float xpos = float.Parse(splitedLine[1]) / 1000000;
                                float ypos = float.Parse(splitedLine[2]) / 1000000;

                                xpos = 1.0f - xpos;
                                ypos = 1.0f - ypos;
                                writeLines[j] += splitedLine[0] + " " + xpos + " " + ypos + " " + splitedLine[3] + " " + splitedLine[4];
                                writeLines[j] = writeLines[j].Replace(',', '.');
                            }
                        }
                        else if (rotation == 270)
                        {
                            g.DrawImage(bm, new Point(-tmpHeight, 0));
                            for (int j = 0; j < writeLines.Length; j++)
                            {
                                string[] splitedLine = lines[j].Split(' ');
                                float xpos = float.Parse(splitedLine[1]) / 1000000;
                                float ypos = float.Parse(splitedLine[2]) / 1000000;

                                xpos = 1.0f - xpos;
                                //ypos = 1.0f - ypos;
                                writeLines[j] += splitedLine[0] + " " + ypos + " " + xpos + " " + splitedLine[4] + " " + splitedLine[3];
                                writeLines[j] = writeLines[j].Replace(',', '.');
                            }
                        }
                    }
                    tmpBm.Save(storePath + splittedName[0] + "_rotatet_" + rotation + ".jpg");
                    allFileNames[i + 1] = storePath + splittedName[0] + "_rotatet_" + rotation + ".jpg";
                    //tmpBm.Save(storePath + splittedName[0] + ".jpg");
                    for (int j = 0; j < writeLines.Length; j++)
                    {
                        File.AppendAllText(storePath + splittedName[0] + "_rotatet_" + rotation + ".txt", writeLines[j] + "\n");
                    }
                    allTextFileNames[i + 1] = storePath + splittedName[0] + "_rotatet_" + rotation + ".txt";
                    rotation += 90;
                    int tmpSave = tmpHeight;
                    tmpHeight = tmpWidth;
                    tmpWidth = tmpSave;
                }
                counter++;

                int addFeaturesToPicture = rnd.Next(0, 4);
                if(addFeaturesToPicture == 3)
                {
                    int numEffects = rnd.Next(1, 10);
                    for (int i = 0; i < numEffects; i++)
                    {
                        int selectPicture = rnd.Next(0, allFileNames.Length);
                        int effect = rnd.Next(0, 3);
                        Bitmap tmpBm = new Bitmap(allFileNames[selectPicture]);
                        string suffix = "effect_" + i;
                        using (Graphics g = Graphics.FromImage(tmpBm))
                        {
                            switch(effect)
                            {
                                case 0:
                                    int alpha = rnd.Next(50, 200);
                                    Brush fill = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                                    g.FillRectangle(fill, new Rectangle(0, 0, tmpBm.Width, tmpBm.Height));
                                    break;
                                case 1:
                                    int alphaR = rnd.Next(50, 200);
                                    int r = rnd.Next(0, 256);
                                    int G = rnd.Next(0, 256);
                                    int b = rnd.Next(0, 256);
                                    Brush fillRnd = new SolidBrush(Color.FromArgb(alphaR, r, G, b));
                                    g.FillRectangle(fillRnd, new Rectangle(0, 0, tmpBm.Width, tmpBm.Height));
                                    break;
                                case 2:
                                    int numRects = rnd.Next(1, 25);
                                    int rectWidth = tmpBm.Width / (2 * numRects);
                                    int rectHeight = tmpBm.Height / (2 * numRects);
                                    Brush black = new SolidBrush(Color.Black);
                                    for (int k = 0; k < numRects; k++)
                                    {
                                        for (int j = 0; j < numRects; j++)
                                        {
                                            g.FillRectangle(black, new Rectangle(rectWidth * k * 2, rectWidth * j * 2, rectWidth, rectHeight));
                                        }
                                    }
                                    break;
                            }
                        }
                        string[] tmpLines = File.ReadAllLines(allTextFileNames[selectPicture]);
                        string textSavePath = allTextFileNames[selectPicture].Remove(allTextFileNames[selectPicture].Length - 4, 4);
                        string pictrueSavePath = allFileNames[selectPicture].Remove(allFileNames[selectPicture].Length - 4, 4);
                        tmpBm.Save(pictrueSavePath + suffix + ".jpg");
                        for (int j = 0; j < tmpLines.Length; j++)
                        {
                            File.AppendAllText(pictrueSavePath + suffix + ".txt", tmpLines[j] + "\n");
                        }
                    }
                }

            }

        }

        private void createNewSet(string name, int offsetNumber, bool generateBBoxFiles)
        {
            int imWidth = 0;
            int imHeight = 0;
            int format = 0;
            string suffixPath = @"C:\Users\Mick\Desktop\geldbilder";
            string backgroundPath = @"C:\Users\Mick\Desktop\backgrounds";
            string[] fileNames = Directory.GetFiles(suffixPath, "*");
            string[] backgrounds = Directory.GetFiles(backgroundPath, "*");
            Random rnd = new Random(DateTime.Now.Millisecond);
            List<Bitmap> objects = new List<Bitmap>();
            List<int> posX = new List<int>();
            List<int> posY = new List<int>();
            List<int> boxWidth = new List<int>();
            List<int> boxHeight = new List<int>();
            List<string> texts = new List<string>();
            List<string> outText = new List<string>();

            //this.Text = "create immage nr " + offsetNumber;
            int backGroundNumber = rnd.Next(backgrounds.Length);
            Bitmap bm = new Bitmap(backgrounds[backGroundNumber]);
            if (bm.Width < 1900 && bm.Height < 1900)
            {
                format = rnd.Next(0, 3);
                if (format == 0)
                {
                    imWidth = rnd.Next(3000, 5000);
                    imHeight = imWidth / 16 * 9;
                    bm = new Bitmap(bm, new Size(imWidth, imHeight));
                }
                else if (format == 1)
                {
                    imHeight = rnd.Next(3000, 5000);
                    imWidth = imHeight / 9 * 16;
                    bm = new Bitmap(bm, new Size(imWidth, imHeight));
                }
                else
                {
                    int m = rnd.Next(4, 16);
                    imWidth = bm.Width * m;
                    imHeight = bm.Height * m;
                    while (imWidth < 1900 || imHeight < 1900)
                    {
                        imWidth += imWidth;
                        imHeight += imHeight;
                    }
                    bm = new Bitmap(bm, new Size(imWidth, imHeight));
                }
            }

            imWidth = bm.Width;
            imHeight = bm.Height;
            float objectScale = (float)(rnd.Next(100, 401)) / 100.0f;
            int scaleTrue = rnd.Next(0, 11) % 2;


            int numObjects = rnd.Next(1, 10);
            using (Graphics g = Graphics.FromImage(bm))
            {
                //g.DrawImage(tmpImage,0,0);
                SolidBrush sliderBrush = new SolidBrush(Color.Purple);

                for (int k = 0; k < numObjects; k++)
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        for (int j = 0; j < 10000; j++)
                        {
                            int b = i * j + rnd.Next(0, i);
                        }
                    }
                    rnd = new Random(DateTime.Now.Millisecond + imHeight);
                    int randomFile = rnd.Next(0, fileNames.Length);
                    texts.Add(fileNames[randomFile]);

                    objects.Add(new Bitmap(fileNames[randomFile]));
                    if (scaleTrue == 0)
                    {
                        objects[k] = new Bitmap(objects[k], new Size((int)((float)objects[k].Width / objectScale), (int)((float)objects[k].Height / objectScale)));
                    }

                    int randomX = -1;
                    int randomY = -1;

                    int rotate = rnd.Next(0, 4);
                    RotateFlipType r;
                    switch (rotate)
                    {
                        case 0:
                            r = RotateFlipType.Rotate90FlipNone;
                            break;
                        case 1:
                            r = RotateFlipType.Rotate180FlipNone;
                            break;
                        case 2:
                            r = RotateFlipType.Rotate270FlipNone;
                            break;
                        default:
                            r = RotateFlipType.RotateNoneFlipNone;
                            break;
                    }
                    objects[k].RotateFlip(r);

                    int rotateEnable = rnd.Next(0, 5);

                    while (randomX < 0 && randomY < 0)
                    {
                        randomX = rnd.Next(0, imWidth);
                        randomY = rnd.Next(0, imHeight);
                    }

                    posX.Add(randomX);
                    posY.Add(randomY);
                    boxWidth.Add(objects[k].Width);
                    boxHeight.Add(objects[k].Height);
                    if (rotateEnable == 3)
                    {
                        int rotateAngleInt = rnd.Next(0, 450);
                        float rotateAngle = (float)rotateAngleInt / 10.0f;
                        g.RotateTransform(rotateAngle);
                    }
                    g.DrawImage(objects[k], new Point(randomX, randomY));

                }
                g.RotateTransform(0.0f);
                int shadowEnable = rnd.Next(0, 5);
                if (shadowEnable == 3)
                {
                    int alpha = rnd.Next(125, 200);
                    Brush shadowbox = null;
                    int color = rnd.Next(0, 4);
                    switch (color)
                    {
                        case (0):
                            shadowbox = new SolidBrush(Color.FromArgb(alpha, 145, 128, 0));
                            break;
                        case (1):
                            shadowbox = new SolidBrush(Color.FromArgb(alpha, 20, 0, 145));
                            break;
                        case (2):
                            shadowbox = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                            break;
                        case (3):
                            shadowbox = new SolidBrush(Color.FromArgb(alpha, 204, 27, 0));
                            break;
                    }
                    g.FillRectangle(shadowbox, new Rectangle(0, 0, bm.Width, bm.Height));
                }
            }
            if (generateBBoxFiles)
            {
                for (int i = 0; i < posX.Count; i++)
                {
                    bool add = true;
                    for (int j = i; j < posX.Count; j++)
                    {

                        if (posX[i] > posX[j] && posX[i] < posX[j] + boxWidth[j] && boxWidth[i] < boxWidth[j]
                                && posY[i] > posY[j] && posY[i] < posY[j] + boxHeight[j] && boxHeight[i] < boxHeight[j])
                        {
                            add = false;
                            //object under another
                        }
                        else
                        {
                            add = true;
                        }
                    }
                    if (add)
                    {
                        string input = getClassNumber(texts[i]) + " " + (float)(posX[i] + (boxWidth[i] / 2 + 1)) / (float)imWidth + " " + (float)(posY[i] + (boxHeight[i] / 2 + 1)) / (float)imHeight + " " + (float)boxWidth[i] / (float)imWidth + " " + (float)boxHeight[i] / (float)imHeight;
                        if (!outText.Contains(input))
                        {
                            outText.Add(input);
                        }
                    }
                }
            }
            bm.Save(suffixPath + "\\Dataset" + "\\" + name + "_" + offsetNumber + ".png");
            for (int i = 0; i < outText.Count; i++)
            {
                File.AppendAllText(suffixPath + "\\Dataset" + "\\" + name + "_" + offsetNumber + ".txt", outText[i] + "\n");
            }
            File.AppendAllText(suffixPath + "\\Dataset" + "\\" + "train.txt", "TO_BE_CHANGED/" + name + "_" + offsetNumber + ".png" + "\n");
        }

        public int getClassNumber(string objectClass)
        {
            if (objectClass.Contains("euro_100"))
                return 0;
            else if (objectClass.Contains("euro_50"))
                return 1;
            else if (objectClass.Contains("euro_20"))
                return 2;
            else if (objectClass.Contains("euro_10"))
                return 3;
            else if (objectClass.Contains("euro_5"))
                return 4;
            else if (objectClass.Contains("euro_2"))
                return 5;
            else if (objectClass.Contains("euro_1"))
                return 6;
            else if (objectClass.Contains("cent_50"))
                return 7;
            else if (objectClass.Contains("cent_20"))
                return 8;
            else if (objectClass.Contains("cent_10"))
                return 9;
            else if (objectClass.Contains("cent_5"))
                return 10;
            else if (objectClass.Contains("cent_2"))
                return 11;
            else if (objectClass.Contains("cent_1"))
                return 12;

            return -1;
        }
    }
}
