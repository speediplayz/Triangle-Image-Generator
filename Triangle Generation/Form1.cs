using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Triangle_Generation
{
    public partial class Form1 : Form
    {
        int ScreenWidth = 15;
        int ScreenHeight = 15;
        
        int gridSize = 50;

        int maxOffset = 10;

        int lineWidth = 3;
        Color lineColor;

        Color[] colorList;

        Bitmap result;

        public Form1()
        {
            InitializeComponent();
            cmdRestart.Visible = false;
        }

        private void GenerateMesh()
        {
            int width = ScreenWidth + 2;
            int height = ScreenHeight + 2;

            Graphics g = CreateGraphics();
            Bitmap image = new Bitmap((ScreenWidth - 1) * gridSize, (ScreenHeight - 1) * gridSize);
            Graphics img = Graphics.FromImage(image);
            List<Vertex> vertices = new List<Vertex>();
            List<Triangle> triangles = new List<Triangle>();
            Random r = new Random(DateTime.Now.Millisecond);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int vX = ((x - 1) * gridSize) + (r.Next(0, maxOffset * 2 + 2) - maxOffset);
                    int vY = ((y - 1) * gridSize) + (r.Next(0, maxOffset * 2 + 2) - maxOffset);
                    
                    Vertex v = new Vertex(vX, vY);
                    vertices.Add(v);
                }
            }

            for (int i = 0, y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    Vertex a = vertices[i];
                    Vertex b = vertices[i + 1];
                    Vertex c = vertices[i + width];
                    Vertex d = vertices[i + width + 1];

                    Triangle t1 = new Triangle(a, b, c);
                    Triangle t2 = new Triangle(b, c, d);
                    triangles.Add(t1);
                    triangles.Add(t2);
                    i++;
                }
                i++;
            }

            foreach (Triangle t in triangles)
            {
                t.Fill(colorList[r.Next(0, colorList.Length)], g);
                t.Draw(lineColor, g, lineWidth);

                t.Fill(colorList[r.Next(0, colorList.Length)], img);
                t.Draw(lineColor, img, lineWidth);
            }

            result = image;
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            string sLineWidth = Interaction.InputBox("Line Width?", "Width", "2");
            int.TryParse(sLineWidth, out lineWidth);
            string sLineColor = Interaction.InputBox("Line Color Hex?", "Color", "#FFFFFF");
            lineColor = ColorTranslator.FromHtml(sLineColor);

            string sColors = Interaction.InputBox("How Many Colors?", "Colors");
            int colorCount;
            if(int.TryParse(sColors, out colorCount))
            {
                colorList = new Color[colorCount];

                for(int i = 0; i < colorCount; i++)
                {
                    string input = Interaction.InputBox("Hex Code Of Color " + (i + 1));
                    colorList[i] = ColorTranslator.FromHtml(input);
                }

                string sScreenWidth = Interaction.InputBox("Width?", "Width", "25");
                string sScreenHeight = Interaction.InputBox("Height?", "Height", "25");
                string sSize = Interaction.InputBox("Triangle Size?", "Size", "25");
                string sMaxOff = Interaction.InputBox("Max Offset?", "Offset", "10");

                int.TryParse(sScreenWidth, out ScreenWidth);
                int.TryParse(sScreenHeight, out ScreenHeight);
                int.TryParse(sSize, out gridSize);
                int.TryParse(sMaxOff, out maxOffset);
                
                cmdStart.Visible = false;
                Width = (ScreenWidth - 1) * gridSize + 16;
                Height = (ScreenHeight - 1) * gridSize + 39;
                GenerateMesh();

                CreateGraphics().Clear(BackColor);
                FolderBrowserDialog open = new FolderBrowserDialog();
                if(open.ShowDialog() == DialogResult.OK)
                {
                    string path = open.SelectedPath;
                    result.Save(path + @"\" + Guid.NewGuid() + ".png");
                }
                cmdRestart.Visible = true;
            }
        }

        private void cmdRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }
    }

    public struct Triangle
    {
        Vertex a;
        Vertex b;
        Vertex c;

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            this.a = new Vertex(new Point(x1, y1));
            this.b = new Vertex(new Point(x2, y2));
            this.c = new Vertex(new Point(x3, y3));
        }

        public void Draw(Color color, Graphics g, int width)
        {
            Pen pen = new Pen(color);
            pen.Alignment = PenAlignment.Center;
            pen.Width = width;

            g.DrawLine(pen, a.point, b.point);
            g.DrawLine(pen, b.point, c.point);
            g.DrawLine(pen, c.point, a.point);
        }

        public void Fill(Color color, Graphics g)
        {
            Point[] points = new Point[] { a.point, b.point, c.point };
            g.FillPolygon(new SolidBrush(color), points);
        }
    }

    public struct Vertex
    {
        public Point point;

        public Vertex(Point a)
        {
            point = a;
        }
        public Vertex(int x, int y)
        {
            point = new Point(x, y);
        }
    }
}
