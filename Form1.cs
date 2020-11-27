using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;

namespace MaslovaSasha_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        int currentLayer = 0;
        bool needReload = false;
        [Obsolete]
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (Quads.Checked == true)
                {
                    view.DrawQuadStrip(currentLayer);
                    glControl1.SwapBuffers();
                }
                if (QuadStrip.Checked == true)
                {
                    view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
                if (Texture.Checked == true)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                    glControl1.SwapBuffers();
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        void Application_Idle(object sender, EventArgs e) //проверяет, занято ли OpenGL окно работой
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate(); //если нет,то вызывается функция Invalidate, которая заставляет кадр рендериться заново
            }
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS() //функция обновления
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.min = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.max = trackBar3.Value;
            needReload = true;
        }
    }  
}

