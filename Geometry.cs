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

        // Here we see if the player moving the token (startX,startY) can move to (targetX,targetY)
        private bool canMoveTo(int startX, int startY, int targetX, int targetY)
        {
            int adjacent = 0;
            if ((startX == -1) && (targetX == 1))
                adjacent = 1;
            if ((startX == 1) && (targetX == -1))
                adjacent = 1;
            if ((startY == targetY) && (startX + 1 == targetX))
                adjacent = 1;
            if ((startY == targetY) && (targetX + 1 == startX))
                adjacent = 1;
            if ((startX == targetX) && ((targetY + 1) % 16 == startY) && ((startX % 2 == 0) || (startX == 7)))
                adjacent = 1;
            if ((startX == targetX) && ((startY + 1) % 16 == targetY) && (startX % 2 == 1))
                adjacent = 1;
            if ((startX == 0) || (targetX == 0)) return false;
            if (adjacent == 0) return false;
            if (isFree(targetX, targetY)) return true;
            if (isEnemy(startX, startY) && isPlayer(targetX, targetY)) return true;
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

        // Here we see if on the path to enemy (u,v) two positions (startX,startY) and (targetX,targetY)
        // are consecutive and also valid for the movement of the enemy from (u,v)
        private bool canMoveToEx(int startX, int startY, int targetX, int targetY, int u, int v)
        {
            int adjacent = 0;
            if ((startX == 0) && (targetX == 1))
                adjacent = 1;
            if ((startX == 1) && (targetX == 0))
                adjacent = 1;
            if ((startY == targetY) && (startX + 1 == targetX))
                adjacent = 1;
            if ((startY == targetY) && (targetX + 1 == startX))
                adjacent = 1;
            if ((startX == targetX) && ((startY + 1) % 16 == targetY) && ((startX % 2 == 0) || (startX == 7)))
                adjacent = 1;
            if ((startX == targetX) && ((targetY + 1) % 16 == startY) && (startX % 2 == 1))
                adjacent = 1;
            if (((startX == 0) && (startY > 0)) || ((targetX == 0) && (targetY > 0))) return false;
            if (adjacent == 0) return false;
            if ((isFree(targetX, targetY) || isPlayer(targetX, targetY)) && (isFree(startX, startY) || isPlayer(startX, startY))) return true;
            if ((targetX == u) && (targetY == v) && (isFree(startX, startY) || isPlayer(startX, startY))) return true;
            return false;
        }

    }
}
