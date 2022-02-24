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

        ProgressBar pb = new ProgressBar();
        TextBox numberTestSamples = new TextBox();
        TextBox sampleNames = new TextBox();
        Button start = new Button();
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(400, 200);

            sampleNames.Size = new Size(100, 35);
            sampleNames.Location = new Point(10, 10);
            this.Controls.Add(sampleNames);

            numberTestSamples.Size = new Size(100, 35);
            numberTestSamples.Location = new Point(110 + 100, 10);
            this.Controls.Add(numberTestSamples);

            pb.Size = new Size(this.ClientSize.Width, 30);
            pb.Location = new Point(0, 70);
            this.Controls.Add(pb);

            start.Size = new Size(100, 30);
            start.Location = new Point(this.ClientSize.Width - start.Width - 10, this.ClientSize.Height - start.Height - 10);
            start.Text = "create samples";
            start.Click += createSamples;
            this.Controls.Add(start);

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
                createNewSet(name, i);
            }
        }

        public void createSamples(object sender, EventArgs e)
        {
            name = sampleNames.Text;
            int samples = Convert.ToInt32(numberTestSamples.Text);
            int numCores = Environment.ProcessorCount;
 
            pb.Maximum = samples;
            pb.Value = 0;
            pb.Step = samples / numCores;

            for (int i = 0; i < numCores; i++)
            {
                Thread t = new Thread(new ThreadStart(PictureCreate));
                t.Start();
            }

        }

        private void createNewSet(string name, int offsetNumber)
        {
            int imWidth = 0;
            int imHeight = 0;
            int format = 0;
            string suffixPath = @"PATH_TO_SAVE";
            string backgroundPath = @"PATH_TO_BACKGROUNDS";
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

                while (randomX < 0 && randomY < 0)
                {
                    randomX = rnd.Next(0, imWidth - objects[k].Width);
                    randomY = rnd.Next(0, imHeight - objects[k].Height);
                }

                posX.Add(randomX);
                posY.Add(randomY);
                boxWidth.Add(objects[k].Width);
                boxHeight.Add(objects[k].Height);
                for (int i = 0; i < objects[k].Width; i++)
                {
                    for (int j = 0; j < objects[k].Height; j++)
                    {
                        try
                        {
                            if (objects[k].GetPixel(i, j).A == 0)
                                bm.SetPixel(randomX + i, randomY + j, bm.GetPixel(randomX + i, randomY + j));
                            else
                            {
                                bm.SetPixel(randomX + i, randomY + j, objects[k].GetPixel(i, j));
                            }

                        }
                        catch (Exception e)
                        {
                            break;
                        }
                    }
                }
            }
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
            bm.Save(suffixPath + "\\Dataset" + "\\" + name + "_" + offsetNumber + ".png");
            for (int i = 0; i < outText.Count; i++)
            {
                File.AppendAllText(suffixPath + "\\Dataset" + "\\" + name + "_" + offsetNumber + ".txt", outText[i] + "\n");
            }
            File.AppendAllText(suffixPath + "\\Dataset" + "\\" + "train.txt", "TO_BE_CHANGED/" + name + "_" + offsetNumber + ".png" + "\n");
        }

        public int getClassNumber(string objectClass)
        {
            if (objectClass.Contains("OBJECT_1"))
                return 0;
            else if (objectClass.Contains("OBJECT_2"))
                return 1;
            else if (objectClass.Contains("OBJECT_3"))
                return 2;
            else if (objectClass.Contains("OBJECT_4"))
                return 3;
            else if (objectClass.Contains("OBJECT_5"))
                return 4;
            else if (objectClass.Contains("OBJECT_6"))
                return 5;
            else if (objectClass.Contains("OBJECT_7"))
                return 6;
            else if (objectClass.Contains("OBJECT_8"))
                return 7;
            else if (objectClass.Contains("OBJECT_9"))
                return 8;
            else if (objectClass.Contains("OBJECT_10"))
                return 9;
            else if (objectClass.Contains("OBJECT_11"))
                return 10;
            else if (objectClass.Contains("OBJECT_12"))
                return 11;
            else if (objectClass.Contains("OBJECT_13"))
                return 12;

            return -1;
        }
    }
}
