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
        GameState gameState;

        private int noPlayers, isSimulated;
        private int rolledMoves, rolledSnakes, rolledFoxes;
        private int isCaptured, capturedID;
        private Point capture;

        private void NewGame()
        {
            gameState.pOneX = -1; gameState.pOneY = 0;
            gameState.pTwoX = -1; gameState.pTwoY = 0;
            gameState.stateOne = 0; gameState.stateTwo = 0;
            int i;
            gameState.SnakesX = new int[8];
            gameState.SnakesY = new int[8];
            for (i = 0; i < 8; i++)
            {
                gameState.SnakesX[i] = 7;
                gameState.SnakesY[i] = 2 * i;
            }
            gameState.FoxesX = new int[8];
            gameState.FoxesY = new int[8];
            for (i = 0; i < 8; i++)
            {
                gameState.FoxesX[i] = 7;
                gameState.FoxesY[i] = 2 * i + 1;
            }
            gameState.StateID = (int)GameStates.StatePlayerOneRolling;
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            int height = this.Size.Height;
            if (height >= 200)
                this.windowSize = height;
            else
                this.windowSize = 600;
            this.Size = new Size(windowSize + 200, windowSize);
            this.Invalidate();
        }

        private Point getPointForCoords(int x, int y)
        {
            int center = this.windowSize / 2 - 15;
            if (x == -1)
            {
                return new Point(center, center);
            }
            if ((x > 0) && (x < 8) && (y >= 0) && (y < 16))
            {
                double angle = -Math.PI + Math.PI / 16 + y * Math.PI / 8;
                int diffX = (int)Math.Floor(Math.Cos(angle) * circleStep * 2 * (1 + x));
                int diffY = -(int)Math.Floor(Math.Sin(angle) * circleStep * 2 * (1 + x));
                return new Point(center + diffX, center + diffY);
            }
            return new Point(0, 0);
        }

        private void TmrTimer_Tick(object sender, System.EventArgs e)
        {
            int x = this.PointToClient(Cursor.Position).X;
            int y = this.PointToClient(Cursor.Position).Y;
            if ((x < 0) || (y < 0) || (x > this.windowSize) || (y > this.windowSize))
                coords = new Point(0, 0);
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
                    for (i = 0; i <= 15; i++)
                        if (angle > (Math.PI / 8) * (i - sensitivity))
                            if (angle < (Math.PI / 8) * (i + sensitivity))
                                yres = i;
                }
                this.Text = xres.ToString() + " " + yres.ToString();
                coords = new Point(xres, yres);
            }
        }

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
                e.Graphics.DrawEllipse(p, center - circleStep, center - circleStep, circleStep * 2, circleStep * 2);
                e.Graphics.DrawEllipse(p, center - circleStep * 16, center - circleStep * 16, circleStep * 32, circleStep * 32);
                double angle;
                for (i = 1; i <= 16; i++)
                {
                    angle = Math.PI / 16 + i * Math.PI / 8;
                    beginX = (int)Math.Floor(Math.Sin(angle) * circleStep);
                    beginY = (int)Math.Floor(Math.Cos(angle) * circleStep);
                    endX = (int)Math.Floor(Math.Sin(angle) * circleStep * 16);
                    endY = (int)Math.Floor(Math.Cos(angle) * circleStep * 16);
                    e.Graphics.DrawLine(p, center + beginX, center + beginY, center + endX, center + endY);
                }
                p = new Pen(Color.Red, 2);
                for (i = 1; i <= 3; i++) {
                    size = circleStep * 4 * i;
                    e.Graphics.DrawEllipse(p, center - size, center - size, size * 2, size * 2);
                }
                p = new Pen(Color.Green, 2);
                for (i = 1; i <= 3; i++) {
                    size = circleStep * (4 * i + 2);
                    e.Graphics.DrawEllipse(p, center - size, center - size, size * 2, size * 2);
                }
                Point at;
                for (i = 0; i < 8; i++) {
                    at = getPointForCoords(gameState.FoxesX[i], gameState.FoxesY[i]);
                    at.X -= 16; at.Y -= 16;
                    e.Graphics.DrawImage(fox, at);
                }
                for (i = 0; i < 8; i++)
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
                } else
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

        private bool isEnemy(int x, int y)
        {
            int i;
            for (i = 0; i < 8; i++)
                if ((gameState.SnakesX[i] == x) && (gameState.SnakesY[i] == y))
                    return true;
            for (i = 0; i < 8; i++)
                if ((gameState.FoxesX[i] == x) && (gameState.FoxesY[i] == y))
                    return true;
            return false;
        }

        private bool isPlayer(int x, int y)
        {
            if ((gameState.pOneX == x) && (gameState.pOneY == y))
                return true;
            if ((gameState.pTwoX == x) && (gameState.pTwoY == y))
                return true;
            return false;
        }

        private bool isFree(int x, int y)
        {
            return (!isEnemy(x, y)) && (!isPlayer(x, y));
        }

        private bool canMoveTo(int x, int y, int z, int w)
        {
            int fl = 0;
            if ((x == -1) && (z == 1))
                fl = 1;
            if ((x == 1) && (z == -1))
                fl = 1;
            if ((y == w) && (x + 1 == z))
                fl = 1;
            if ((y == w) && (z + 1 == x))
                fl = 1;
            if ((x == z) && ((w + 1) % 16 == y) && ((x % 2 == 0) || (x == 7)))
                fl = 1;
            if ((x == z) && ((y + 1) % 16 == w) && (x % 2 == 1))
                fl = 1;
            if (fl == 0) return false;
            if (isFree(z, w)) return true;
            if (isEnemy(x, y) && isPlayer(z, w)) return true;
            return false;
        }

        private void frmMain_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (gameState.StateID)
                    {
                        case 2:
                            if (canMoveTo(gameState.pOneX, gameState.pOneY, coords.X, coords.Y))
                            {
                                gameState.pOneX = coords.X;
                                gameState.pOneY = coords.Y;
                                validateBoundaries();
                                rolledMoves--;
                                updateGameState();
                            }
                            break;
                        case 3:
                        case 7:
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
                                int en = -1, i;
                                for (i = 0; i < 8; i++)
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
                            break;
                        case 4:
                        case 8:
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
                                int en = -1, i;
                                for (i = 0; i < 8; i++)
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
                            break;
                        case 6:
                            if (canMoveTo(gameState.pTwoX, gameState.pTwoY, coords.X, coords.Y))
                            {
                                gameState.pTwoX = coords.X;
                                gameState.pTwoY = coords.Y;
                                validateBoundaries();
                                rolledMoves--;
                                updateGameState();
                            }
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    isCaptured = 0;
                    updateLabel();
                    break;
            }
            updateScore();
            this.Invalidate();
        }

        private void validateBoundaries()
        {
            if (gameState.pOneX == 7)
            {
                gameState.stateOne = 1;
            }
            if ((noPlayers == 2) && (gameState.pTwoX == 7))
            {
                gameState.stateTwo = 1;
            }
        }

        private void updateScore()
        {
            string s = "";
            switch (noPlayers)
            {
                case 1:
                    switch (gameState.stateOne)
                    {
                        case 0:
                            s += "Player in progress.";
                            break;
                        case 1:
                            s += "Player returning.";
                            break;
                        case 2:
                            s += "Player has escaped!";
                            break;
                        case 3:
                            s += "Player captured!";
                            break;
                    }
                    break;
                case 2:
                    switch (gameState.stateOne)
                    {
                        case 0:
                            s += "Player 1: in progress; \n";
                            break;
                        case 1:
                            s += "Player 1: returning; \n";
                            break;
                        case 2:
                            s += "Player 1: has escaped; \n";
                            break;
                        case 3:
                            s += "Player 1: captured; \n";
                            break;
                    }
                    switch (gameState.stateTwo)
                    {
                        case 0:
                            s += "Player 2: in progress.";
                            break;
                        case 1:
                            s += "Player 2: returning.";
                            break;
                        case 2:
                            s += "Player 2: has escaped.";
                            break;
                        case 3:
                            s += "Player 2: captured. ";
                            break;
                    }
                    break;
            }
            lblScore.Text = s;
        }

        private void updateLabel()
        {
            switch (isSimulated)
            {
                case 0:
                    switch (gameState.StateID)
                    {
                        case 1:
                            lblGameState.Text = "Player 1 rolls";
                            break;
                        case 2:
                            lblGameState.Text = "Player 1 moves";
                            break;
                        case 3:
                            if (isCaptured == 1)
                                lblGameState.Text = "Player 2 moves snake " + capturedID.ToString();
                            else
                                lblGameState.Text = "Player 2 selects a snake";
                            break;
                        case 4:
                            if (isCaptured == 1)
                                lblGameState.Text = "Player 2 moves fox " + capturedID.ToString();
                            else
                                lblGameState.Text = "Player 2 selects a fox";
                            break;
                        case 5:
                            lblGameState.Text = "Player 2 rolls";
                            break;
                        case 6:
                            lblGameState.Text = "Player 2 moves";
                            break;
                        case 7:
                            if (isCaptured == 1)
                                lblGameState.Text = "Player 1 moves snake " + capturedID.ToString();
                            else
                                lblGameState.Text = "Player 1 selects a snake";
                            break;
                        case 8:
                            if (isCaptured == 1)
                                lblGameState.Text = "Player 1 moves fox " + capturedID.ToString();
                            else
                                lblGameState.Text = "Player 1 selects a fox";
                            break;
                    }
                    break;
                case 1:
                    break;
            }
        }

        private void updateGameState()
        {
            int hasChanged = 1, i;
            while (hasChanged == 1)
            {
                hasChanged = 0;
                if ((gameState.StateID == 2) && (rolledMoves == 0))
                {
                    gameState.StateID++;
                    hasChanged = 1;
                }
                if ((gameState.StateID == 3) && (rolledSnakes == 0))
                {
                    gameState.StateID++;
                    hasChanged = 1;
                }
                if ((gameState.StateID == 4) && (rolledFoxes == 0))
                {
                    if ((gameState.stateTwo < 2) && (noPlayers == 2))
                        gameState.StateID = 5;
                    else
                        gameState.StateID = 1;
                    hasChanged = 1;
                }
                if ((gameState.StateID == 6) && (rolledMoves == 0))
                {
                    gameState.StateID++;
                    hasChanged = 1;
                }
                if ((gameState.StateID == 7) && (rolledSnakes == 0))
                {
                    gameState.StateID++;
                    hasChanged = 1;
                }
                if ((gameState.StateID == 8) && (rolledFoxes == 0))
                {
                    if (gameState.stateOne < 2)
                        gameState.StateID = 1;
                    else
                        gameState.StateID = 5;
                    hasChanged = 1;
                }
                if ((gameState.pOneX == -1) && (gameState.stateOne == 1))
                {
                    if (gameState.StateID < 5)
                        gameState.StateID = 3;
                    gameState.stateOne = 2;
                    gameState.pOneX = -2;
                    hasChanged = 1;
                    updateScore();
                }
                if ((gameState.pTwoX == -1) && (gameState.stateTwo == 1))
                {
                    if (gameState.StateID > 4)
                        gameState.StateID = 7;
                    gameState.stateTwo = 2;
                    gameState.pTwoX = -2;
                    hasChanged = 1;
                    updateScore();
                }
                for (i = 0; i < 8; i++)
                {
                    if ((gameState.SnakesX[i] == gameState.pOneX) && (gameState.SnakesY[i] == gameState.pOneY))
                    {
                        gameState.stateOne = 3;
                        hasChanged = 1;
                        gameState.pOneX = -2;
                        updateScore();
                    }
                    if ((gameState.SnakesX[i] == gameState.pTwoX) && (gameState.SnakesY[i] == gameState.pTwoY))
                    {
                        gameState.stateTwo = 3;
                        hasChanged = 1;
                        gameState.pTwoX = -2;
                        updateScore();
                    }
                    if ((gameState.FoxesX[i] == gameState.pOneX) && (gameState.FoxesY[i] == gameState.pOneY))
                    {
                        gameState.stateOne = 3;
                        hasChanged = 1;
                        gameState.pOneX = -2;
                        updateScore();
                    }
                    if ((gameState.FoxesX[i] == gameState.pTwoX) && (gameState.FoxesY[i] == gameState.pTwoY))
                    {
                        gameState.stateTwo = 3;
                        hasChanged = 1;
                        gameState.pTwoX = -2;
                        updateScore();
                    }
                }
            }
            updateLabel();
            if (((noPlayers == 2) && (gameState.stateOne > 1) && (gameState.stateTwo > 1)) || ((noPlayers == 1) && (gameState.stateOne > 1)))
            {
                MessageBox.Show(lblScore.Text, "Game over!", MessageBoxButtons.OK);
                Environment.Exit(0);
            }
        }

        string getPlayerString()
        {
            if (noPlayers == 1)
                return "The player";
            else if ((gameState.StateID < 5) && (isSimulated == 1))
                return "Player 1";
            else if ((gameState.StateID < 3) || (gameState.StateID == 7) || (gameState.StateID == 8))
                return "Player 1";
            else
                return "Player 2";
        }

        private void btnRoll_Click(object sender, EventArgs e)
        {
            if ((gameState.StateID == 1) || (gameState.StateID == 5))
            {
                int i, val;
                Random rnd = new Random();
                rolledMoves = 0;
                rolledSnakes = 0;
                rolledFoxes = 0;
                for (i = 0; i < 6; i++)
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

        private void frmMain_Load(object sender, EventArgs e)
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
            NewGame();
        }
    }
}