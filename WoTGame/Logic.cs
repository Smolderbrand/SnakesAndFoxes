using System;
using System.Drawing;
using System.Windows.Forms;

namespace WoTGame
{
    public partial class frmMain
    {
        private void newGame()
        {
            gameState.pOneX = -1; gameState.pOneY = 0;
            gameState.pTwoX = -1; gameState.pTwoY = 0;
            gameState.stateOne = 0; gameState.stateTwo = 0;
            gameState.SnakesX = new int[8];
            gameState.SnakesY = new int[8];
            for (int i = 0; i < 8; ++i)
            {
                gameState.SnakesX[i] = 7;
                gameState.SnakesY[i] = 2 * i;
            }
            gameState.FoxesX = new int[8];
            gameState.FoxesY = new int[8];
            for (int i = 0; i < 8; ++i)
            {
                gameState.FoxesX[i] = 7;
                gameState.FoxesY[i] = 2 * i + 1;
            }
            gameState.StateID = (int)GameStates.StatePlayerOneRolling;
        }


        private bool isOnACircle(int x, int y)
        {
            return x > 0 && x < 8 && y >= 0 && y < 16;
        }

        private Point getPointForCoords(int x, int y)
        {
            int center = this.windowSize / 2 - 15;
            if (x == -1)
            {
                return new Point(center, center);
            }
            if (isOnACircle(x, y))
            {
                double angle = -Math.PI + Math.PI / 16 + y * Math.PI / 8;
                int diffX = (int)Math.Floor(Math.Cos(angle) * circleStep * 2 * (1 + x));
                int diffY = -(int)Math.Floor(Math.Sin(angle) * circleStep * 2 * (1 + x));
                return new Point(center + diffX, center + diffY);
            }
            return new Point(0, 0);
        }


        private bool isEnemy(int x, int y)
        {
            for (int i = 0; i < 8; ++i)
                if ((gameState.SnakesX[i] == x) && (gameState.SnakesY[i] == y))
                    return true;
            for (int i = 0; i < 8; ++i)
                if ((gameState.FoxesX[i] == x) && (gameState.FoxesY[i] == y))
                    return true;
            return false;
        }

        private bool isPlayer(int x, int y)
        {
            if (noPlayers == 2)
                return ((gameState.pOneX == x) && (gameState.pOneY == y)) || ((gameState.pTwoX == x) && (gameState.pTwoY == y));
            else
                return (gameState.pOneX == x) && (gameState.pOneY == y);
        }

        private bool isFree(int x, int y)
        {
            return (!isEnemy(x, y)) && (!isPlayer(x, y));
        }

        private bool canMoveTo(int x, int y, int z, int w)
        {
            int adjacent = 0;
            if ((x == -1) && (z == 1))
                adjacent = 1;
            if ((x == 1) && (z == -1))
                adjacent = 1;
            if ((y == w) && (x + 1 == z))
                adjacent = 1;
            if ((y == w) && (z + 1 == x))
                adjacent = 1;
            if ((x == z) && ((w + 1) % 16 == y) && ((x % 2 == 0) || (x == 7)))
                adjacent = 1;
            if ((x == z) && ((y + 1) % 16 == w) && (x % 2 == 1))
                adjacent = 1;
            if ((x == 0) || (z == 0)) return false;
            if (adjacent == 0) return false;
            if (isFree(z, w)) return true;
            if (isEnemy(x, y) && isPlayer(z, w)) return true;
            return false;
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
            if (noPlayers == 1)
            {
                if (gameState.stateOne == 0)
                    s += "Player in progress.";
                else if (gameState.stateOne == 1)
                    s += "Player returning.";
                else if (gameState.stateOne == 2)
                    s += "Player has escaped!";
                else
                    s += "Player captured!";
            }
            else
            {
                if (gameState.stateOne == 0)
                    s += "Player 1: in progress; \n";
                else if (gameState.stateOne == 1)
                    s += "Player 1: returning; \n";
                else if (gameState.stateOne == 2)
                    s += "Player 1: has escaped; \n";
                else
                    s += "Player 1: captured; \n";
                if (gameState.stateTwo == 0)
                    s += "Player 2: in progress.";
                else if (gameState.stateTwo == 1)
                    s += "Player 2: returning.";
                else if (gameState.stateTwo == 2)
                    s += "Player 2: has escaped.";
                else
                    s += "Player 2: captured. ";
            }
            lblScore.Text = s;
        }

        private void updateLabel()
        {
            if (noPlayers == 1)
            {
                if (gameState.StateID == 1)
                    lblGameState.Text = "The player rolls";
                else if (gameState.StateID == 2)
                    lblGameState.Text = "The player moves";
                else if (gameState.StateID == 3)
                    lblGameState.Text = "The snakes move";
                else
                    lblGameState.Text = "The foxes move";
            }
            else
            {
                if (gameState.StateID == 1)
                    lblGameState.Text = "Player 1 rolls";
                else if (gameState.StateID == 2)
                    lblGameState.Text = "Player 1 moves";
                else if (gameState.StateID == 3)
                {
                    if (isCaptured == 1)
                        lblGameState.Text = "Player 2 moves snake " + capturedID.ToString();
                    else
                        lblGameState.Text = "Player 2 selects a snake";
                }
                else if (gameState.StateID == 4)
                {
                    if (isCaptured == 1)
                        lblGameState.Text = "Player 2 moves fox " + capturedID.ToString();
                    else
                        lblGameState.Text = "Player 2 selects a fox";
                }
                else if (gameState.StateID == 5)
                    lblGameState.Text = "Player 2 rolls";
                else if (gameState.StateID == 6)
                    lblGameState.Text = "Player 2 moves";
                else if (gameState.StateID == 7)
                {
                    if (isCaptured == 1)
                        lblGameState.Text = "Player 1 moves snake " + capturedID.ToString();
                    else
                        lblGameState.Text = "Player 1 selects a snake";
                }
                else if (gameState.StateID == 8)
                {
                    if (isCaptured == 1)
                        lblGameState.Text = "Player 1 moves fox " + capturedID.ToString();
                    else
                        lblGameState.Text = "Player 1 selects a fox";
                }
            }
        }

        private bool canMoveToEx(int x, int y, int z, int w, int u, int v)
        {
            int adjacent = 0;
            if ((x == 0) && (z == 1))
                adjacent = 1;
            if ((x == 1) && (z == 0))
                adjacent = 1;
            if ((y == w) && (x + 1 == z))
                adjacent = 1;
            if ((y == w) && (z + 1 == x))
                adjacent = 1;
            if ((x == z) && ((y + 1) % 16 == w) && ((x % 2 == 0) || (x == 7)))
                adjacent = 1;
            if ((x == z) && ((w + 1) % 16 == y) && (x % 2 == 1))
                adjacent = 1;
            if (((x == 0) && (y > 0)) || ((z == 0) && (w > 0))) return false;
            if (adjacent == 0) return false;
            if ((isFree(z, w) || isPlayer(z, w)) && (isFree(x, y) || isPlayer(x, y))) return true;
            if ((z == u) && (w == v) && (isFree(x, y) || isPlayer(x, y))) return true;
            return false;
        }

        private void runSimulationFor(int player)
        {
            int[] snakes = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            int[] snakeDistances = new int[8];
            int[] foxes = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            int[] foxDistances = new int[8];
            int[] nextStepX = new int[8];
            int[] nextStepY = new int[8];
            int[,,,] adj = new int[8, 16, 8, 16];
            int[,] dist = new int[8, 16];
            bool[,] sptSet = new bool[8, 16];
            int aux;
            for (int s = 0; s < 8; ++s)
            {
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 16; ++j)
                        for (int k = 0; k < 8; ++k)
                            for (int l = 0; l < 16; ++l)
                                if (canMoveToEx(i, j, k, l, gameState.SnakesX[s], gameState.SnakesY[s]))
                                    adj[i, j, k, l] = 1;
                                else
                                    adj[i, j, k, l] = 0;
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 16; ++j)
                    {
                        dist[i, j] = 30;
                        sptSet[i, j] = false;
                    }
                if (player == 1)
                    dist[(gameState.pOneX == -1) ? 0 : gameState.pOneX, gameState.pOneY] = 0;
                else
                    dist[(gameState.pTwoX == -1) ? 0 : gameState.pTwoX, gameState.pTwoY] = 0;
                for (int count = 0; count < 112; ++count)
                {
                    int min = 30, u1 = 0, u2 = 0;
                    for (int v = 0; v < 8; ++v)
                        for (int w = 0; w < 16; ++w)
                            if (sptSet[v, w] == false && dist[v, w] <= min && (v > 0 || w == 0))
                            {
                                min = dist[v, w];
                                u1 = v;
                                u2 = w;
                            }
                    sptSet[u1, u2] = true;
                    for (int v = 0; v < 8; ++v)
                        for (int w = 0; w < 16; ++w)
                            if ((v > 0 || w == 0) && !sptSet[v, w] && (adj[u1, u2, v, w] > 0) && dist[u1, u2] != 30 && dist[u1, u2] + adj[u1, u2, v, w] < dist[v, w])
                                dist[v, w] = dist[u1, u2] + adj[u1, u2, v, w];
                }
                for (int v = 0; v < 8; ++v)
                    for (int w = 0; w < 16; ++w)
                        if ((v > 0 || w == 0) && (dist[v, w] == dist[gameState.SnakesX[s], gameState.SnakesY[s]] - 1))
                            if (canMoveToEx(v, w, gameState.SnakesX[s], gameState.SnakesY[s], gameState.SnakesX[s], gameState.SnakesY[s]))
                            {
                                nextStepX[s] = v;
                                nextStepY[s] = w;
                            }
                snakeDistances[s] = dist[gameState.SnakesX[s], gameState.SnakesY[s]];
            }
            for (int i = 0; i < 7; ++i)
                for (int j = i + 1; j < 8; ++j)
                    if (snakeDistances[i] > snakeDistances[j])
                    {
                        aux = snakes[i]; snakes[i] = snakes[j]; snakes[j] = aux;
                        aux = snakeDistances[i]; snakeDistances[i] = snakeDistances[j]; snakeDistances[j] = aux;
                        aux = nextStepX[i]; nextStepX[i] = nextStepX[j]; nextStepX[j] = aux;
                        aux = nextStepY[i]; nextStepY[i] = nextStepY[j]; nextStepY[j] = aux;
                    }
            for (int i = 0; i < rolledSnakes; ++i)
            {
                gameState.SnakesX[snakes[i]] = (nextStepX[i] == 0) ? -1 : nextStepX[i];
                gameState.SnakesY[snakes[i]] = nextStepY[i];
                updateLabel();
                this.Invalidate();
            }
            rolledSnakes = 0;
            for (int s = 0; s < 8; ++s)
            {
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 16; ++j)
                        for (int k = 0; k < 8; ++k)
                            for (int l = 0; l < 16; ++l)
                                if (canMoveToEx(i, j, k, l, gameState.FoxesX[s], gameState.FoxesY[s]))
                                    adj[i, j, k, l] = 1;
                                else
                                    adj[i, j, k, l] = 0;
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 16; ++j)
                    {
                        dist[i, j] = 30;
                        sptSet[i, j] = false;
                    }
                if (player == 1)
                    dist[(gameState.pOneX == -1) ? 0 : gameState.pOneX, gameState.pOneY] = 0;
                else
                    dist[(gameState.pTwoX == -1) ? 0 : gameState.pTwoX, gameState.pTwoY] = 0;
                for (int count = 0; count < 112; ++count)
                {
                    int min = 30, u1 = 0, u2 = 0;
                    for (int v = 0; v < 8; ++v)
                        for (int w = 0; w < 16; ++w)
                            if (sptSet[v, w] == false && dist[v, w] <= min && (v > 0 || w == 0))
                            {
                                min = dist[v, w];
                                u1 = v;
                                u2 = w;
                            }
                    sptSet[u1, u2] = true;
                    for (int v = 0; v < 8; ++v)
                        for (int w = 0; w < 16; ++w)
                            if ((v > 0 || w == 0) && !sptSet[v, w] && (adj[u1, u2, v, w] > 0) && dist[u1, u2] != 30 && dist[u1, u2] + adj[u1, u2, v, w] < dist[v, w])
                                dist[v, w] = dist[u1, u2] + adj[u1, u2, v, w];
                }
                for (int v = 0; v < 8; ++v)
                    for (int w = 0; w < 16; ++w)
                        if ((v > 0 || w == 0) && (dist[v, w] == dist[gameState.FoxesX[s], gameState.FoxesY[s]] - 1))
                            if (canMoveToEx(v, w, gameState.FoxesX[s], gameState.FoxesY[s], gameState.FoxesX[s], gameState.FoxesY[s]))
                            {
                                nextStepX[s] = v;
                                nextStepY[s] = w;
                            }
                Point p1 = getPointForCoords(gameState.FoxesX[s], gameState.FoxesY[s]);
                Point p2;
                if (player == 1)
                    p2 = getPointForCoords(gameState.pOneX, gameState.pOneY);
                else
                    p2 = getPointForCoords(gameState.pTwoX, gameState.pTwoY);
                foxDistances[s] = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
            }
            for (int i = 0; i < 7; ++i)
                for (int j = i + 1; j < 8; ++j)
                    if (foxDistances[i] > foxDistances[j])
                    {
                        aux = foxes[i]; foxes[i] = foxes[j]; foxes[j] = aux;
                        aux = foxDistances[i]; foxDistances[i] = foxDistances[j]; foxDistances[j] = aux;
                        aux = nextStepX[i]; nextStepX[i] = nextStepX[j]; nextStepX[j] = aux;
                        aux = nextStepY[i]; nextStepY[i] = nextStepY[j]; nextStepY[j] = aux;
                    }
            for (int i = 0; i < rolledFoxes; ++i)
            {
                gameState.FoxesX[foxes[i]] = (nextStepX[i] == 0) ? -1 : nextStepX[i];
                gameState.FoxesY[foxes[i]] = nextStepY[i];
                updateLabel();
                this.Invalidate();
            }
            rolledFoxes = 0;
            updateGameState();
        }

        private void updateGameState()
        {
            int hasChanged = 1;
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
                for (int i = 0; i < 8; ++i)
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
                DialogResult d = MessageBox.Show(lblScore.Text + "\nNew game?", "Game over!", MessageBoxButtons.YesNo);
                if (d == DialogResult.Yes)
                {
                    newGame();
                    updateLabel();
                    this.lblRoll.Text = "Ready";
                    this.Invalidate();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            if ((gameState.StateID == 3) || (gameState.StateID == 4))
                if (isSimulated == 1)
                    runSimulationFor(1);
            if ((gameState.StateID == 7) || (gameState.StateID == 8))
                if (isSimulated == 1)
                    runSimulationFor(2);
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
    }
}
