﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Webp;  // Webp.cs

/* vanilla usings:
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using WebPWrapper; */

namespace SakuraView
{
    public partial class SakuraView : Form
    {
        static readonly string execPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
        static string upscaleMode;
        static string windowPosition;
        static string upscaleAlgorithm;
        static int width;
        static int height;
        static int widthSpan;
        static int heightSpan;
        static int bottomSpace = 0;
        static int rightSpace = 0;
        static int screenWidth;
        static int screenHeight;
        static int currentImage;
        static int bannerHeight;
        static int x;
        static int y;
        static bool windowFound;
        static bool help = false;
        static bool info = false;
        static bool banner = false;
        static byte currentScreen = 0;
        static List<string> imagePaths = new List<string>();
        static List<Image> images = new List<Image>();
        static string[] txt = { "SakuraView v1.0", "Scale Algorithm {Bicubic, Bilinear, Default, High, HighQualityBicubic, HighQualityBilinear, Low, NearestNeighbor}", "High",  // config[2]
            "Scale mode {Fill, Fit, Stretch, VanillaFit, VanillaFill, Center}","VanillaFit",  // config[4]
            "Window Location Window Location {Minimized, Normal, Maximized, Normal2, Maximized2}", "Normal",  // config[6]
            "Text Colour (System.Drawing.Color)", "White",  // config[8]
            "Background Colour", "Black",  // config[10]
            "Banner {View, Hide}", "View",  // config[12]
            "Help {View, Hide}", "View",  // config[14]
            "Info {View, Hide}", "View",  // config[16]
            "Always On Top {True, False}", "False"};  // config[18]
        // when escape is pressed 
        // Environment.Exit(0);
        public SakuraView()
        {
            InitializeComponent();
            try
            {
                // read the config file
                txt = System.IO.File.ReadAllLines(execPath + "SakuraView.txt");
                upscaleAlgorithm = txt[2].ToLower();
                upscaleMode = txt[4].ToLower();
                windowPosition = txt[6].ToLower();
                SetAlgorithm();
                SetSizeMode();
                SetTextColour(txt[8]);
                SetBackgroundColour(txt[10]);
                banner = txt[12].ToLower() == "view";
                help = txt[14].ToLower() == "view";
                info = txt[16].ToLower() == "view";
                SetBanner();
                SetHelp(true);
                SetInfo(true);
                SetWindowPosition();
                this.TopMost = txt[18].ToLower() == "true";
            }
            catch
            {
                // if the program doesn't have the rights to read, then load default config
                upscaleMode = "vanillafit";
                // this.WindowState = FormWindowState.Normal;
            }
            string[] args = Environment.GetCommandLineArgs();
            Console.WriteLine(args);
            if (args.Length > 1)
            {
                LoadImage(args[1]);
            }

        }
        private void SetBackgroundColour(string backgroundColour)
        {
            this.BackColor = System.Drawing.Color.FromName(backgroundColour);
            SakuraBox.BackColor = System.Drawing.Color.FromName(backgroundColour);
        }
        private void SetTextColour(string textColour)
        {

            SakuraInfo.ForeColor = System.Drawing.Color.FromName(textColour);
            SakuraSideHelp.ForeColor = System.Drawing.Color.FromName(textColour);
            SakuraHelp.ForeColor = System.Drawing.Color.FromName(textColour);
        }
        private void SetBanner()
        {
            if (banner)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
            }
        }
        private void SetHelp(bool init = false)
        {
            if (help)
            {
                SakuraSideHelp.Visible = true;
                SakuraHelp.Visible = true;
                if (!info)
                {
                    bottomSpace += SakuraHelp.Height;  // add the height of SakuraHelp
                }
                else
                {
                    bottomSpace += SakuraHelp.Height + 35;  // add the height of SakuraHelp + padding
                    rightSpace += SakuraSideHelp.Width;
                }
            }
            else
            {
                SakuraSideHelp.Visible = false;
                SakuraHelp.Visible = false;
                if (!init)
                {
                    if (!info)
                    {
                        bottomSpace -= SakuraHelp.Height;  // sub the height of SakuraHelp
                    }
                    else
                    {
                        bottomSpace -= SakuraHelp.Height + 35;  // sub the height of SakuraHelp + padding
                        rightSpace -= SakuraSideHelp.Width;
                    }
                }
            }
        }
        private void SetInfo(bool init = false)
        {
            if (info)
            {
                SakuraInfo.Visible = true;
                bottomSpace += SakuraInfo.Height;
            }
            else
            {
                SakuraInfo.Visible = false;
                if (!init)
                {
                    if (help)
                    {
                        bottomSpace -= SakuraInfo.Height + 35;
                    }
                    else
                    {
                        bottomSpace -= SakuraInfo.Height;
                    }
                }
            }
        }
        private void SetAlgorithm()
        {
            switch (upscaleAlgorithm)
            {
                case "bicubic":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                    break;
                case "bilinear":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    break;
                case "default":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                    break;
                case "high":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    break;
                case "highqualitybicubic":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    break;
                case "highqualitybilinear":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    break;
                case "low":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    break;
                case "nearestneighbor":
                    SakuraBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    break;
            }
        }
        private void SetWindowPosition()
        {
            switch (windowPosition.ToLower())
            {
                case "minimized":
                    this.WindowState = FormWindowState.Minimized;
                    break;
                case "normal":
                    WindowState = FormWindowState.Normal;
                    for (byte i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        if (Screen.AllScreens[i].Bounds.X == 0)
                        {
                            this.Location = Screen.AllScreens[i].Bounds.Location;
                            currentScreen = i;
                            break;
                        }
                    }
                    break;
                case "normal2":
                    WindowState = FormWindowState.Normal;
                    for (byte i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        if (Screen.AllScreens[i].Bounds.X != 0)
                        {
                            this.Location = Screen.AllScreens[i].Bounds.Location;
                            currentScreen = i;
                            break;
                        }
                    }
                    break;
                case "maximized":
                    if (this.WindowState == FormWindowState.Maximized) { this.WindowState = FormWindowState.Normal; }
                    for (byte i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        if (Screen.AllScreens[i].Bounds.X == 0)
                        {
                            currentScreen = i;
                            UpdateLayoutMaximized();
                            break;
                        }
                    }
                    WindowState = FormWindowState.Maximized;
                    break;
                case "maximized2":
                    if (this.WindowState == FormWindowState.Maximized) { this.WindowState = FormWindowState.Normal; }
                    for (byte i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        if (Screen.AllScreens[i].Bounds.X != 0)
                        {
                            currentScreen = i;
                            UpdateLayoutMaximized();
                            break;
                        }
                    }
                    WindowState = FormWindowState.Maximized;
                    break;
            }
            ScaleImage();
        }
        private void LoadImage(String filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    byte[] id = new byte[4];
                    using (System.IO.FileStream file = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        file.Position = 0x08;
                        file.Read(id, 0, 4);
                    }
                    if (id[0] == 87 && id[1] == 69 && id[2] == 66 && id[3] == 80) // Webp
                    {
                        using (WebP webp = new WebP())
                        {
                            SakuraBox.Image = webp.Load(filePath);
                        }
                    }
                    else
                    {
                        SakuraBox.Image = LoadImageFromFile(filePath);
                    }
                    while (images.Count < currentImage)
                    {
                        images.Add(null);
                    }
                    images.Add(SakuraBox.Image);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid input Image");
                    throw e;
                }
            }
            //string extension = Path.GetExtension(filePath).ToLower();
            //string baseName = Path.GetFileNameWithoutExtension(filePath);
            // Extract icon from executable and set as picture box image
            //Icon icon = Icon.ExtractAssociatedIcon(filePath);
            //SakuraBox.Image = icon.ToBitmap();

            // Load cursor into picture box using IconLib library
            /*
            using (IconReader iconReader = new IconReader())
            {
                IconInfo cursorInfo = iconReader.ReadIcon(filePath, IconReaderFormat.DEFAULT, false)[0];
                pictureBox1.Image = cursorInfo.ToBitmap();
            }*/ /*
            Dynamicweb.Imaging.PdfImageConverter pdf = new Dynamicweb.Imaging.PdfImageConverter();
            IEnumerable<byte[]> imageCollection = pdf.ConvertToImages(filePath);
            foreach (byte[] image in imageCollection)
            {
                SakuraBox.Image = byteArrayToImage(image);
            }*/

            // Load image
            // SakuraBox.Image = Image.FromFile(baseName + ".png");

            // Load WebP image into picture box using Google.Webp decoder library
            //SixLabors.ImageSharp.Image.Load(filePath);
            /*
            Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(filePath);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, new PngOptions());
                SakuraBox.Image = Image.FromStream(ms);
            }*/


            Console.WriteLine("hello");
            ScaleImage();
        }
        private Image LoadImageFromFile(string path)  // allows the image not to be locked by the program
        {
            byte[] bytes = File.ReadAllBytes(path);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
        private void SetSizeMode()
        {
            if (upscaleMode == "fill" || upscaleMode == "stretch" || upscaleMode == "vanillafill")
            {
                SakuraBox.SizeMode = PictureBoxSizeMode.StretchImage;

            }
            else if (upscaleMode == "fit" || upscaleMode == "vanillafit")
            {
                SakuraBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                SakuraBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }
        private void ScaleImage()
        {
            if (SakuraBox.Image == null) { return; }
            width = SakuraBox.Image.Size.Width;
            height = SakuraBox.Image.Size.Height;
            widthSpan = 0;
            heightSpan = 0;
            windowFound = false;
            for (byte i = 0; i < Screen.AllScreens.Length; i++)
            {
                screenWidth = Screen.AllScreens[i].Bounds.Width;
                screenHeight = Screen.AllScreens[i].Bounds.Height;
                bannerHeight = screenHeight - this.ClientSize.Height;
                x = Screen.AllScreens[i].Bounds.Location.X;
                y = Screen.AllScreens[i].Bounds.Location.Y;
                if (x <= this.Location.X && this.Location.X <= (x + screenWidth) && y <= this.Location.Y && this.Location.Y <= (y + screenHeight))  // finds the screen boundaries the window is currently in
                {
                    windowFound = true;
                    break;
                }
            }
            if (!windowFound)
            {
                screenWidth = Screen.AllScreens[currentScreen].Bounds.Width;
                screenHeight = Screen.AllScreens[currentScreen].Bounds.Height;
                x = Screen.AllScreens[currentScreen].Bounds.Location.X;
                y = Screen.AllScreens[currentScreen].Bounds.Location.Y;
                bannerHeight = screenHeight - this.ClientSize.Height;
            }
            if (upscaleMode == "fill")
            {
                setImageBounds();
                Fill();
            }
            else if (upscaleMode == "fit")  // the highest value becomes the screen bounds
            {
                setImageBounds();
                Fit();
            }
            else if (upscaleMode == "stretch")
            {
                setImageBounds();
                height = screenHeight;
                width = screenWidth;
            }
            else if (upscaleMode != "none" && (width > screenWidth || height > screenHeight - bannerHeight - bottomSpace))
            { // upscaleMode == "none" - we downscale the image to the "fit" algorithm
                if (upscaleMode == "vanillafill")
                {
                    setImageBounds();
                    Fill();
                }
                if (upscaleMode == "vanillafit")
                {
                    setImageBounds();
                    Fit();
                }
            }
            SakuraBox.Location = new Point(widthSpan, heightSpan);
            SakuraBox.Size = new System.Drawing.Size(width + 1, height + 1);  // for some reason there's a pixel of margin.
            Console.WriteLine(SakuraBox.Size);
            Console.WriteLine(SakuraBox.Location);
            if (this.WindowState != FormWindowState.Maximized)
            {
                UpdateLayout();
            } else { UpdateLayoutMaximized(); }

        }
        private void setImageBounds()
        {
            if (banner)
                screenHeight -= bannerHeight + bottomSpace;
            else
                screenHeight -= bottomSpace;
            screenWidth -= rightSpace;
        }
        private void UpdateLayout()
        {
            this.ClientSize = new System.Drawing.Size(width + rightSpace, height + bannerHeight + bottomSpace);
            SakuraSideHelp.Location = new Point(width, 0);
            SakuraHelp.Location = new Point(0, height);
            SakuraInfo.Location = new Point(0, height + bottomSpace - SakuraInfo.Height);
        }
        private void UpdateLayoutMaximized()
        {
            this.Location = Screen.AllScreens[currentScreen].Bounds.Location;
            if (banner)
            {
                SakuraHelp.Location = new Point(0, Screen.AllScreens[currentScreen].Bounds.Height - SakuraInfo.Height - 35 - SakuraHelp.Height - bannerHeight);
                SakuraInfo.Location = new Point(0, Screen.AllScreens[currentScreen].Bounds.Height - SakuraInfo.Height - bannerHeight);
            }
            else
            {
                SakuraHelp.Location = new Point(0, Screen.AllScreens[currentScreen].Bounds.Height - SakuraInfo.Height - 35 - SakuraHelp.Height);
                SakuraInfo.Location = new Point(0, Screen.AllScreens[currentScreen].Bounds.Height - SakuraInfo.Height);
            }
            SakuraSideHelp.Location = new Point(Screen.AllScreens[currentScreen].Bounds.Width - SakuraSideHelp.Width, 0);
        }
        private void Fill()
        {
            decimal ratio = (decimal)screenHeight / (decimal)height;
            height = screenHeight;
            widthSpan = width;
            width = (int)(width * ratio);
            widthSpan = Math.Abs(widthSpan - width) >> 1;
        }
        private void Fit()
        {
            decimal ratio = (decimal)screenHeight / (decimal)height;
            height = screenHeight;
            width = (int)(width * ratio);
        }
        private void SakuraViewClass_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);  // gets all the files or folders name that were dragged in an array, but I'll only use the first
            if (file != null) // prevent crashes if it's for example a google chrome favourite that was dragged
            {
                bool isFolder = System.IO.File.GetAttributes(file[0]).HasFlag(System.IO.FileAttributes.Directory);
                if (!isFolder)  // that means it's a file.
                {
                    LoadImage(file[0]);
                }
            }
        }

        private void SakuraViewClass_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void ViewImage(int imageNumber)
        {

        }

        private void SakuraView_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keydown!" + e.KeyCode);
            if (e.KeyCode == Keys.D1) { ViewImage(1); }
            else if (e.KeyCode == Keys.D2) { ViewImage(2); }
            else if (e.KeyCode == Keys.D3) { ViewImage(3); }
            else if (e.KeyCode == Keys.D4) { ViewImage(4); }
            else if (e.KeyCode == Keys.D5) { ViewImage(5); }
            else if (e.KeyCode == Keys.D6) { ViewImage(6); }
            else if (e.KeyCode == Keys.D7) { ViewImage(7); }
            else if (e.KeyCode == Keys.D8) { ViewImage(8); }
            else if (e.KeyCode == Keys.D9) { ViewImage(9); }
            else if (e.KeyCode == Keys.D0) { ViewImage(imagePaths.Count >> 1); }
            else if (e.KeyCode == Keys.F1)
            {
                if (!help)
                {
                    help = true;
                    SetHelp();
                }
                else
                {
                    help = false;
                    SetHelp();
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                upscaleMode = "fill";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F3)
            {
                upscaleMode = "fit";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F4)
            {
                upscaleMode = "stretch";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F5)
            {
                upscaleMode = "vanillafit";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F6)
            {
                upscaleMode = "vanillafill";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F7)
            {
                upscaleMode = "center";
                SetSizeMode();
                ScaleImage();
            }
            else if (e.KeyCode == Keys.F8)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else if (e.KeyCode == Keys.F9)
            {
                upscaleAlgorithm = "normal";
                SetWindowPosition();
            }
            else if (e.KeyCode == Keys.F10)
            {
                upscaleAlgorithm = "normal2";
                SetWindowPosition();
            }
            else if (e.KeyCode == Keys.F11)
            {
                upscaleAlgorithm = "maximized";
                SetWindowPosition();
            }
            else if (e.KeyCode == Keys.F12)
            {
                upscaleAlgorithm = "maximized2";
                SetWindowPosition();
            }
            else if (e.KeyCode == Keys.B)
            {
                upscaleAlgorithm = "bicubic";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.F)
            {
                upscaleAlgorithm = "bilinear";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.D)
            {
                upscaleAlgorithm = "default";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.H)
            {
                upscaleAlgorithm = "high";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.Q)
            {
                upscaleAlgorithm = "highqualitybicubic";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.J)
            {
                upscaleAlgorithm = "highqualitybilinear";
                SetAlgorithm();
                SakuraBox.Refresh();

            }
            else if (e.KeyCode == Keys.L)
            {
                upscaleAlgorithm = "low";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.N)
            {
                upscaleAlgorithm = "nearestneighbor";
                SetAlgorithm();
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Environment.Exit(0);
            }
            else if (e.KeyCode == Keys.E)
            {
                images.Clear();
                SakuraInfo.Text = "";
                GC.Collect(2, GCCollectionMode.Forced, false, false);
            }
            else if (e.KeyCode == Keys.T)
            {
                banner = banner != true;
                SetBanner();
            }
            else if (e.KeyCode == Keys.I)
            {
                info = info != true;
                SetInfo();
            }
            else if (e.KeyCode == Keys.A)
            {
                this.TopMost = this.TopMost != true;
            }
            else if (e.KeyCode == Keys.X)
            {
                SakuraBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.Y)
            {
                SakuraBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                SakuraBox.Refresh();
            }
            else if (e.KeyCode == Keys.R)
            {
                SakuraBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                SakuraBox.Refresh();
                //ScaleImage();
            }
            else if (e.KeyCode == Keys.S)
            {
                txt[2] = upscaleAlgorithm;
                txt[4] = upscaleMode;
                txt[6] = windowPosition;
                // txt[8] is only edited through the config
                // txt[10] is only edited through the config
                if (banner) { txt[12] = "view"; } else { txt[12] = "hide"; }
                if (help) { txt[14] = "view"; } else { txt[14] = "hide"; }
                if (info) { txt[16] = "view"; } else { txt[16] = "hide"; }
                if (this.TopMost) { txt[18] = "true"; } else { txt[18] = "false"; }
                try { System.IO.File.WriteAllLines(execPath + "SakuraView.txt", txt); }
                catch { } // continue execution without saving
            }
            else if (e.KeyCode == Keys.Left)
            {
                ViewImage(currentImage - 1);
            }
            else if (e.KeyCode == Keys.Right)
            {
                ViewImage(currentImage - 1);
            }
            else if (e.KeyCode == Keys.Up)
            {
                ViewImage(1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                ViewImage(imagePaths.Count);
            }
        }
    }
}