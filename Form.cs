﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WoTGame
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        enum GameStates
        {
            StatePlayerOneRolling = 1,
            StatePlayerOneMoving = 2,
            StatePlayerOneSnakes = 3,
            StatePlayerOneFoxes = 4,
            StatePlayerTwoRolling = 5,
            StatePlayerTwoMoving = 6,
            StatePlayerTwoSnakes = 7,
            StatePlayerTwoFoxes = 8
        }

        // Structure encapsulating the current game's data
        struct GameState
        {
            public int pOneX, pOneY;
            public int pTwoX, pTwoY;
            public int stateOne, stateTwo;
            public int[] SnakesX, SnakesY;
            public int[] FoxesX, FoxesY;
            public int StateID;
        }

        private int windowSize, circleStep;
        private double sensitivity;

        private Point coords;
        private GameState gameState;

        private int noPlayers, isSimulated;
        private int rolledMoves, rolledSnakes, rolledFoxes;

        // "Capture" here refers to the selected enemy token
        private int isCaptured, capturedID;
        private Point capture;

        private void frmMainSizeChanged(object sender, EventArgs e)
        {
            int height = this.Size.Height;
            if (height >= 200)
                this.windowSize = height;
            else
                this.windowSize = 600;
            this.Size = new Size(windowSize + 200, windowSize);
            this.Invalidate();
        }

        /* Handler for the timer's tick (at small, <1 second intervals)
         * The purpose of this timer is to periodically convert the
         * mouse cursor's position from screen space to board space */
      
        private void tmrTimerTick(object sender, System.EventArgs e)
        {
            int x = this.PointToClient(Cursor.Position).X;
            int y = this.PointToClient(Cursor.Position).Y;
            if (x < 0 || y < 0 || x > windowSize || y > windowSize)
            {
                coords = new Point(0, 0);
            } 
            else
            {
                int relx = x - this.windowSize / 2 + 15;
                int rely = y - this.windowSize / 2 + 15;
                int dist2 = relx * relx + rely * rely;
                int i, xres = 0, yres = 0;
                if (dist2 < circleStep * circleStep * (1 + sensitivity) * (1 + sensitivity))
                {
                    xres = -1;
                    yres = 0;
                }
                else
                {
                    for (i = 4; i <= 16; i += 2)
                        if (dist2 < circleStep * circleStep * (i + sensitivity) * (i + sensitivity))
                            if (dist2 > circleStep * circleStep * (i - sensitivity) * (i - sensitivity))
                                xres = i / 2 - 1;
                    double angle = Math.Atan2(-rely, relx) + Math.PI - Math.PI / 16;
                    for (i = 0; i <= 15; ++i)
                        if (angle > (Math.PI / 8) * (i - sensitivity))
                            if (angle < (Math.PI / 8) * (i + sensitivity))
                                yres = i;
                }
                Text = xres.ToString() + " " + yres.ToString();
                coords = new Point(xres, yres);
            }
        }

        // This procedure paints the board and the tokens with vector
        // graphics, where the size can vary as a function of form's size
        protected void OnPaint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.Blue, 2);
            Image fox = Image.FromFile("assets\\fox.png");
            Image snake = Image.FromFile("assets\\snake.png");
            Image pOne = Image.FromFile("assets\\pOne.png");
            Image pTwo = Image.FromFile("assets\\pTwo.png");
            try
            {
                int center = this.windowSize / 2 - 15;
                int i, size, beginX, beginY, endX, endY;
                circleStep = (int)Math.Floor((double)(this.windowSize - 30) * 0.025);
                int diff = center - circleStep;
                e.Graphics.DrawEllipse(p, diff, diff, circleStep * 2, circleStep * 2);
                diff = center - circleStep * 16;
                e.Graphics.DrawEllipse(p, diff, diff, circleStep * 32, circleStep * 32);
                double angle;
                for (i = 1; i <= 16; ++i)
                {
                    angle = Math.PI / 16 + i * Math.PI / 8;
                    beginX = (int)Math.Floor(Math.Sin(angle) * circleStep);
                    beginY = (int)Math.Floor(Math.Cos(angle) * circleStep);
                    endX = (int)Math.Floor(Math.Sin(angle) * circleStep * 16);
                    endY = (int)Math.Floor(Math.Cos(angle) * circleStep * 16);
                    e.Graphics.DrawLine(p, center + beginX, center + beginY, center + endX, center + endY);
                }
                p = new Pen(Color.Red, 2);
                for (i = 1; i <= 3; ++i) {
                    size = circleStep * 4 * i;
                    e.Graphics.DrawEllipse(p, center - size, center - size, size * 2, size * 2);
                }
                p = new Pen(Color.Green, 2);
                for (i = 1; i <= 3; ++i) {
                    size = circleStep * (4 * i + 2);
                    e.Graphics.DrawEllipse(p, center - size, center - size, size * 2, size * 2);
                }
                Point at;
                for (i = 0; i < 8; ++i) {
                    at = getPointForCoords(gameState.FoxesX[i], gameState.FoxesY[i]);
                    at.X -= 16; at.Y -= 16;
                    e.Graphics.DrawImage(fox, at);
                }
                for (i = 0; i < 8; ++i)
                {
                    at = getPointForCoords(gameState.SnakesX[i], gameState.SnakesY[i]);
                    at.X -= 16; at.Y -= 16;
                    e.Graphics.DrawImage(snake, at);
                }
                if (noPlayers == 2)
                {
                    if ((gameState.stateOne == 0) && (gameState.stateTwo == 0) && (gameState.pOneX == -1) && (gameState.pTwoX == -1))
                    {
                        at = getPointForCoords(-1, 0);
                        at.X -= 40; at.Y -= 16;
                        e.Graphics.DrawImage(pOne, at);
                        at.X += 48;
                        e.Graphics.DrawImage(pTwo, at);
                    }
                    else
                    {
                        if (gameState.stateOne < 2)
                        {
                            at = getPointForCoords(gameState.pOneX, gameState.pOneY);
                            at.X -= 16; at.Y -= 16;
                            e.Graphics.DrawImage(pOne, at);
                        }
                        if (gameState.stateTwo < 2)
                        {
                            at = getPointForCoords(gameState.pTwoX, gameState.pTwoY);
                            at.X -= 16; at.Y -= 16;
                            e.Graphics.DrawImage(pTwo, at);
                        }
                    }
                }
                else
                {
                    at = getPointForCoords(gameState.pOneX, gameState.pOneY);
                    at.X -= 16; at.Y -= 16;
                    e.Graphics.DrawImage(pOne, at);
                }
                this.btnRoll.Location = new Point(windowSize * 19 / 20 + 50, windowSize * 3 / 4);
                this.lblRoll.Location = new Point(windowSize * 19 / 20 + 50, windowSize * 1 / 4);
                this.lblGameState.Location = new Point(windowSize * 19 / 20 + 50, windowSize * 1 / 4 - 20);
                this.lblScore.Location = new Point(windowSize * 19 / 20 + 50, windowSize * 3 / 4 + 25);
            }
            finally
            {
                p.Dispose();
                fox.Dispose();
                snake.Dispose();
                pOne.Dispose();
                pTwo.Dispose();
            }
        }

        // This is where moves are handled per se. According to the current game state, it is checked
        // whether the token (player or enemy) can move to that space and, in the affirmative, moved.
        private void frmMainMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (gameState.StateID == 2)
                {
                    if (canMoveTo(gameState.pOneX, gameState.pOneY, coords.X, coords.Y))
                    {
                        gameState.pOneX = coords.X;
                        gameState.pOneY = coords.Y;
                        validateBoundaries();
                        rolledMoves--;
                        updateGameState();
                    }
                }
                else if ((gameState.StateID == 3) || (gameState.StateID == 7))
                {
                    if ((isCaptured == 1) && canMoveTo(capture.X, capture.Y, coords.X, coords.Y))
                    {
                        gameState.SnakesX[capturedID] = coords.X;
                        gameState.SnakesY[capturedID] = coords.Y;
                        rolledSnakes--;
                        isCaptured = 0;
                        updateGameState();
                    }
                    else if (isCaptured == 0)
                    {
                        int en = -1;
                        for (int i = 0; i < 8; ++i)
                            if ((gameState.SnakesX[i] == coords.X) && (gameState.SnakesY[i] == coords.Y))
                            {
                                en = i;
                                break;
                            }
                        if (en > -1)
                        {
                            isCaptured = 1;
                            capturedID = en;
                            capture = new Point(coords.X, coords.Y);
                        }
                        updateLabel();
                    }
                }
                else if ((gameState.StateID == 4) || (gameState.StateID == 8))
                {
                    if ((isCaptured == 1) && canMoveTo(capture.X, capture.Y, coords.X, coords.Y))
                    {
                        gameState.FoxesX[capturedID] = coords.X;
                        gameState.FoxesY[capturedID] = coords.Y;
                        rolledFoxes--;
                        isCaptured = 0;
                        updateGameState();
                    }
                    else if (isCaptured == 0)
                    {
                        int en = -1;
                        for (int i = 0; i < 8; ++i)
                            if ((gameState.FoxesX[i] == coords.X) && (gameState.FoxesY[i] == coords.Y))
                            {
                                en = i;
                                break;
                            }
                        if (en > -1)
                        {
                            isCaptured = 1;
                            capturedID = en;
                            capture = new Point(coords.X, coords.Y);
                        }
                    }
                    updateLabel();
                }
                else if (gameState.StateID == 6)
                {
                    if (canMoveTo(gameState.pTwoX, gameState.pTwoY, coords.X, coords.Y))
                    {
                        gameState.pTwoX = coords.X;
                        gameState.pTwoY = coords.Y;
                        validateBoundaries();
                        rolledMoves--;
                        updateGameState();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                isCaptured = 0;
                updateLabel();
            }
            updateScore();
            this.Invalidate();
        }

        // This is the function where the dice are rolled
        private void btnRollClick(object sender, EventArgs e)
        {
            if ((gameState.StateID == 1) || (gameState.StateID == 5))
            {
                int val;
                Random rnd = new Random();
                rolledMoves = 0;
                rolledSnakes = 0;
                rolledFoxes = 0;
                for (int i = 0; i < 6; ++i)
                {
                    val = rnd.Next(1, 7);
                    if ((val == 1) || (val == 2))
                        rolledMoves++;
                    if ((val == 3) || (val == 4))
                        rolledSnakes++;
                    if ((val == 5) || (val == 6))
                        rolledFoxes++;
                }
                string rollText = rolledMoves.ToString() + " move(s), \n" + rolledSnakes + " snake(s) and " + rolledFoxes + " fox(es)";
                lblRoll.Text = getPlayerString() + " rolled " + rollText;
                gameState.StateID++;
                updateGameState();
            }
        }

        // This function reads the game parameters from the "game.xml"
        // file and sets them accordingly.
        private void frmMainLoad(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("game.xml");
            XmlNode node = doc.DocumentElement.SelectSingleNode("display");
            windowSize = int.Parse(node.Attributes["size"]?.InnerText);
            sensitivity = (double)int.Parse(node.Attributes["sensitivity"]?.InnerText) / 100;
            this.Size = new Size(windowSize + 200, windowSize);
            node = doc.DocumentElement.SelectSingleNode("playercount");
            noPlayers = int.Parse(node.Attributes["size"]?.InnerText);
            node = doc.DocumentElement.SelectSingleNode("simulation");
            if (node.Attributes["value"]?.InnerText == "true")
                isSimulated = 1;
            else
                isSimulated = 0;
            doc = null;
            newGame();
        }
    }
}
