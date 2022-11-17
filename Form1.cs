using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkenoid1
{


    public partial class Form1 : Form
    {

        Image redBrick;
        Image blueBrick;
        Image greenBrick;
        Image yellowBrick
            ;
        public Form1 ()
        {
            InitializeComponent();
            
            redBrick = Image.FromFile(Application.StartupPath + "\\brick_red.png");
            blueBrick = Image.FromFile(Application.StartupPath + "\\brick_yellow.png");
            greenBrick = Image.FromFile(Application.StartupPath + "\\brick_blue.png");
            yellowBrick = Image.FromFile(Application.StartupPath + "\\brick_green.png");

            drawingBox.BackColor = Color.Black;

            drawingBox.Invalidate();

            brickState = new bool[40];
            brickPoints = new int[40];
            Random rndGenerator = new Random();

            for (int i = 0; i < 40; i++) {
                brickState[i] = true;
                brickPoints[i] = rndGenerator.Next(1, 5);
            }
            //Timer
            timer1.Interval = 20;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            button1.Visible = false;
        }

        private void Timer1_Tick (object sender, EventArgs e)
        {
            //if (gameIsRestarted == false) {
            //    drawingBox.Invalidate();
            //} 


            // This method to paused of game.
            if (gameIsPaused == false) {
                drawingBox.Invalidate();
            }
        }

        const int ballSize = 10;
        const int ballSpeedX = 4;
        const int ballSpeedY = 4;
        const int brickWidth = 40;
        const int brickHeight = 20;
        const int brickSep = 4;
        int Score;

        bool gameIsRestarted = false;
        bool gameIsPaused = false;
        int ballX = 100;
        int ballY = 200;
        int ballVx = ballSpeedX;
        int ballVy = ballSpeedY;
        

        int padX = 0;
        int padY = 260;
        int padWidth = 70;
        int padHeight = 10;

        bool[] brickState;
        int[] brickPoints;


        private void drawingBox_Paint (object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            Pen whitePen = new Pen(Color.White);
            Brush brushplay = new SolidBrush(Color.White);

            Rectangle a = new Rectangle(30, 30, 50, 20);
            Rectangle b = new Rectangle(100, 30, 50, 20);

            Rectangle padRectangle = new Rectangle(padX, padY, padWidth, padHeight);

            e.Graphics.DrawRectangle(whitePen, padRectangle);
            e.Graphics.FillRectangle(brushplay, padRectangle);





            //g.DrawRectangle(whitePen, r);

            Color[] brickColors = { Color.Red, Color.SeaGreen, Color.SlateGray, Color.SkyBlue };
            Image[] brickImages= { yellowBrick, blueBrick, redBrick, greenBrick };


            Font font = new Font("Arial", 11);

            Brush textBrush = new SolidBrush(Color.White);
            int brickNo = 0;

            

            for (int i = 0; i < 4; i++) {
             //   Brush brush = new SolidBrush(brickColors[i % 4]);
                //Image Color = new Image(brickColor[i%4]);
                int x = 30;
                while ((x + brickWidth) < drawingBox.Width) {
                    int y = 50 + i * (brickHeight + brickSep);

                    if (brickState[brickNo]) {

                        //g.FillRectangle(brush, x, y, brickWidth, brickHeight);
                        g.DrawImage(brickImages[i%4],x,y,brickWidth,brickHeight);
                        //g.DrawString("" + brickPoints[brickNo], font, textBrush, x, y);


                        bool hitVertical = CheckIfHitVertical(ballX, ballY, x, y);
                        bool hitHorizontal = CheckIfHitHorizontal(ballX, ballY, x, y);
                        //Print a ScoreBoard.
                        Point nokta = new Point(10, 15);
                        Pen newPen = new Pen(Color.White);
                        Font newfont = new Font("Arial", 15);
                        Brush Brush = new SolidBrush(Color.White);
                        g.DrawString("Score : " + Score.ToString(), newfont, Brush, nokta);


                        //bool hit = CheckIfHit(ballX, ballY, x, y);
                        bool hit = hitVertical || hitHorizontal;
                        if (hit) {

                            brickState[brickNo] = false;
                            Score += brickPoints[brickNo];  // Score +=1; --> Score increase each hit is a 1.
                        }

                        if (hitHorizontal) {
                            ballVy = -ballVy;
                        }
                        if (hitVertical) {
                            ballVx = -ballVx;
                        }
                    }
                    
                    x += (brickWidth + brickSep);
                    brickNo++;
                }

            }

            Rectangle r = new Rectangle(ballX - ballSize / 2, ballY - ballSize / 2, ballSize, ballSize);

            bool isBallHitThePad = CheckIfPadHit(ballX, ballY);
            if (isBallHitThePad) {
                ballVy = -ballSpeedY;
            }

            e.Graphics.DrawEllipse(whitePen, r);
            e.Graphics.FillEllipse(textBrush, r);



            if (ballX > drawingBox.Width - ballSize) {

                ballVx = -ballSpeedX;

            }
            if (ballX < 0) {

                ballVx = ballSpeedX;

            }
            if (ballY > drawingBox.Height - ballSize) {

                gameIsPaused = true;
                String s = "...GAME İS OVER...";
                button1.Text = "PLAY ";
                button1.BackColor = Color.White;
                button1.ForeColor = Color.Black;
                button1.Font = new Font("Arial", 8);
                button1.Visible = true;

                g.DrawString(s, font, textBrush, 170, 150);
            }
            if (ballY < 0) {

                ballVy = ballSpeedY;
            }

            // When I want to stop our program I can use this method or timer method.
            //if (gameIsPaused == false)
            //{

            ballX += ballVx;
            ballY += ballVy;

            //}
        }

        bool CheckIfHitVertical (int ballX, int ballY, int brickX, int brickY)
        {
            int x1 = brickX;
            int y1 = brickY;

            int x2 = x1 + brickWidth;
            int y2 = y1;

            int x3 = x1 + brickWidth;
            int y3 = y1 + brickHeight;

            int x4 = x1;
            int y4 = y1 + brickHeight;

            //sol kenarı kontrol et.
            int ballCheckX = ballX + ballSize / 2;

            if ((ballY >= y1 && ballY <= y4) && (ballCheckX >= x1 && ballCheckX <= x1 + ballSpeedX)) {
                return true;

            }

            //sağ kenarı kontrol et.
            ballCheckX = ballCheckX - ballSize / 2;
            if ((ballY >= y2 && ballY <= y3) && (ballCheckX >= x2 && ballCheckX <= x2 + ballSpeedX)) {
                return true;

            }
            return false;
        }

        bool CheckIfHitHorizontal (int ballX, int ballY, int brickX, int brickY)
        {

            int x1 = brickX;
            int y1 = brickY;

            int x2 = x1 + brickWidth;
            int y2 = y1;

            int x3 = x1 + brickWidth;
            int y3 = y1 + brickHeight;

            int x4 = x1;
            int y4 = y1 + brickHeight;

            //Üst kenarı kontrol et
            int ballCheckY = ballY + ballSize / 2;  //Merkezden yarıçap kadar aşağısının Y koordinatı
            if ((ballX >= x1 && ballX <= x2) && (ballCheckY >= y1 && ballCheckY <= y1 + ballSpeedY)) {
                return true;

            }

            //Alt kenarı kontrol et
            ballCheckY = ballY - ballSize / 2; //Merkezden yarıçap kadar yukarısının Y koordinatı 
            if ((ballX >= x4 && ballX <= x3) && (ballCheckY <= y3 && ballCheckY >= y3 - ballSpeedY)) {
                return true;

            }
            return false;
        }

        bool CheckIfPadHit (int ballX, int ballY)
        {
            if (ballX >= padX && ballX <= padX + padWidth) {            // X ekseninde pad aralığında
                if (ballY >= padY && ballY <= padY + padHeight) {       // Y ekseninde pad aralığında
                    return true;
                }
            }

            return false;
        }

        void ResetBricks()
        {
            for (int i = 0; i < 40; i++) {
                brickState[i] = true;
            }
        }
        private void buttonReset_Click (object sender, EventArgs e)
        {
            ResetBricks();
        }

        private void buttonPause_Click (object sender, EventArgs e)
        {
            gameIsPaused = !gameIsPaused;
        }

        private void drawingBox_MouseMove (object sender, MouseEventArgs e)
        {
            padX = e.X;
            Console.WriteLine("X:" + e.X + "  Y:" + e.Y);
        }

        private void button1_Click (object sender, EventArgs e)
        {
            ResetBricks();
            Score = 0;
            ballX = 100;
            ballY = 200;
            ballVx = ballSpeedX;
            ballVy = ballSpeedY;
            gameIsPaused = false;
            button1.Visible = false;
        }
    }
}


