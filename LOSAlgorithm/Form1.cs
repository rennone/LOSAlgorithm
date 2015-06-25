using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOSAlgorithm
{
    public partial class Form1 : Form
    {

        class Vertex
        {
            public Point v;

            public bool Touched(Point p)
            {
                var dx = v.X - p.X;
                var dy = v.Y - p.Y;

                return dx * dx + dy * dy < 20 * 20;
            }

            public Vertex(Point p)
            {
                v = p;
            }
        }

        class Cell
        {
           public Point p;

            public Cell(Point v)
            {
                p = v;
            }
        }

        int CanvasSize
        {
            get
            {
                return Math.Min(Canvas.Width, Canvas.Height) - 1;
            }
        }

        Point lastTouched = new Point();

        Vertex st, en;

        Vertex selected;

        List<Cell> Cells = new List<Cell>();

        int SpritNum { get { return (int)Sprit.Value; } }

        public Form1()
        {
            InitializeComponent();

            st = new Vertex(new Point(50,50));
            en = new Vertex(new Point(100, 50));
        }

        PointF PointToCell(Point p)
        {

            return new PointF(1.0f * p.X / CanvasSize * SpritNum, 1.0f * p.Y / CanvasSize * SpritNum);
        }

        Rectangle CellToRect(Point cell)
        {
            return new Rectangle(cell.X * CanvasSize / SpritNum, cell.Y * CanvasSize / SpritNum, CanvasSize / SpritNum, CanvasSize / SpritNum);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastTouched = e.Location;

            selected = null;

            if (st.Touched(lastTouched))
                selected = st;

            if (en.Touched(lastTouched))
                selected = en;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected == null || e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            selected.v.X += e.X - lastTouched.X;
            selected.v.Y += e.Y - lastTouched.Y;

            lastTouched = e.Location;

            Cells = LOS(PointToCell(st.v), PointToCell(en.v));
            Canvas.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.White);

            foreach(var i in Enumerable.Range(0, SpritNum+1) )
            {
                Point p1 = new Point(i * CanvasSize / SpritNum, 0);
                Point p2 = new Point(i * CanvasSize / SpritNum, CanvasSize);

                g.DrawLine(Pens.Black, p1, p2);


                Point l = new Point(0,i * CanvasSize / SpritNum);
                Point r = new Point(CanvasSize, i * CanvasSize / SpritNum);

                g.DrawLine(Pens.Black, l, r);
            }
;
            if(Cells.Count > 0)
                g.FillRectangles(new SolidBrush(Color.FromArgb(100, 25, 0, 0)), Cells.Select( (c) => CellToRect(c.p)).ToArray());

            g.DrawLine(Pens.Red, st.v, en.v);

            g.DrawString(Cells.Count + "", new Font("MsGothic", 12), Brushes.Red, new PointF());
        }

        private void Sprit_ValueChanged(object sender, EventArgs e)
        {
            Canvas.Invalidate();
        }


        private List<Cell> LOS(PointF st, PointF en)
        {
            bool bresenham = true;
            List<Cell> res = new List<Cell>();

            Point stInt = new Point((int)st.X,(int) st.Y);
            Point enInt = new Point((int)en.X,(int) en.Y);

            Point next = stInt;

            PointF delta = new PointF(en.X - st.X, en.Y - st.Y);

            Point step = new Point(delta.X < 0 ? -1 : 1, delta.Y < 0 ? -1 : 1);

            delta.X = Math.Abs(delta.X);
            delta.Y = Math.Abs(delta.Y);

            PointF fmod = new PointF(Math.Abs(st.X - stInt.X), Math.Abs(st.Y - stInt.Y));

            if (step.X < 0)
                fmod.X = 1.0f - fmod.X;

            if (step.Y < 0)
                fmod.Y = 1.0f - fmod.Y;

             res.Add(new Cell(next));
            if(delta.X > delta.Y)
            {
                float fraction = -delta.X * (1 - fmod.Y) + delta.Y * (1 - fmod.X);

                while(next.X != enInt.X)
                {
                    if(fraction >= 0)
                    {
                        next.Y += step.Y;
                        fraction -= delta.X;
                        if (Math.Abs(fraction) != delta.X && bresenham == false)
                            res.Add(new Cell(next));
                    }
                    next.X += step.X;
                    fraction += delta.Y;
                    res.Add(new Cell(next));
                }
            }
            else
            {
                float fraction = -delta.Y * (1 - fmod.X) + delta.X * (1 - fmod.Y);
                while(next.Y != enInt.Y)
                {
                    if(fraction >= 0)
                    {
                        next.X += step.X;
                        fraction -= delta.Y;
                        if (Math.Abs(fraction) != delta.Y && bresenham == false)
                            res.Add(new Cell(next));
                    }
                    next.Y += step.Y;
                    fraction += delta.X;
                    res.Add(new Cell(next));
                }
            }

            if( res[res.Count-1].p != enInt)
                 res.Add(new Cell(enInt));
            return res;
        }
    }
}
