using System;
using System.Drawing;
using System.Windows.Forms;

namespace WoTGame
{
    public partial class frmMain
    {
        private bool isOnACircle(int x, int y)
        {
            return x > 0 && x < 8 && y >= 0 && y < 16;
        }

        // This is where translation from board space to screen space is done
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

        // Here we see if the player moving the token (x,y) can move to (z,w)
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

        // This procedure checks whether a player has reached the outer ring and
        // updates the player's state accordingly.
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

        // Here we see if on the path to enemy (u,v) two positions (x,y) and (z,w)
        // are consecutive and also valid for the movement of the enemy towards (u,v)
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

    }
}
