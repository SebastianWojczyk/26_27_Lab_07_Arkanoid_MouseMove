using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid_MouseMove
{
    public partial class FormArkanoid : Form
    {
        RectangleF player;
        RectangleF ball;
        PointF ballSpeed;

        List<RectangleF> bricks;
        Timer timer;
        public FormArkanoid()
        {
            InitializeComponent();
            DoubleBuffered = true;

            PrepareGame();
        }

        private void PrepareGame()
        {
            player = new RectangleF(new PointF(45, 90), new SizeF(10, 5));
            ball = new RectangleF(new PointF(50, 50), new SizeF(5, 5));
            ballSpeed = new PointF(1, 1);
            Form1_SizeChanged(null, null);//do ustalenia kształtu piłki

            bricks = new List<RectangleF>();
            SizeF BRICK_SIZE = new SizeF(10, 5);
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 100 / BRICK_SIZE.Width; col++)
                {
                    bricks.Add(new RectangleF(new PointF(col * BRICK_SIZE.Width,
                                                         row * BRICK_SIZE.Height + 10),
                                              BRICK_SIZE));
                }
            }

            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ball.X += ballSpeed.X;
            ball.Y += ballSpeed.Y;
            Invalidate();

            foreach (RectangleF brick in bricks)
            {
                if (ball.IntersectsWith(brick))
                {
                    bricks.Remove(brick);
                    ballSpeed.Y = -ballSpeed.Y;
                    break;
                }
            }
            if (bricks.Count == 0)
            {
                timer.Stop();
                MessageBox.Show("Wygrałeś");
                PrepareGame();
            }
            if (ball.IntersectsWith(player))
            {
                ballSpeed.Y = -ballSpeed.Y;
            }
            //lewa i prawa sciana
            if (ball.X < 0 || ball.X + ball.Width > 100)
            {
                ballSpeed.X = -ballSpeed.X;
            }
            //sufit
            if (ball.Y < 0)
            {
                ballSpeed.Y = -ballSpeed.Y;
            }
            else if (ball.Y + ball.Height > 100)
            {
                timer.Stop();
                MessageBox.Show("Przegrałeś");
                PrepareGame();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightPink);
            e.Graphics.DrawRectangle(new Pen(Color.Black, 4),
                                     recalculateRectangle(player));

            e.Graphics.DrawImage(Properties.Resources.ball_image,
                                     recalculateRectangle(ball));

            foreach (RectangleF brick in bricks)
            {
                e.Graphics.DrawRectangle(new Pen(Color.DarkRed, 4),
                                     recalculateRectangle(brick));
            }
        }

        private Rectangle recalculateRectangle(RectangleF player)
        {
            return new Rectangle(
                                 (int)(player.X * ClientSize.Width / 100),
                                 (int)(player.Y * ClientSize.Height / 100),
                                 (int)(player.Width * ClientSize.Width / 100),
                                 (int)(player.Height * ClientSize.Height / 100)
                                );
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            ball.Width = ball.Height * ClientSize.Height / ClientSize.Width;
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            player.X = (float)e.Location.X / ClientSize.Width * 100 - player.Width / 2;
            Invalidate();
        }
    }
}
