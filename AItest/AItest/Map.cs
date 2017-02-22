using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AItest
{
    public class Map
    {
        int WorldX;
        int WorldY;
        List<Square> WorldObjects = new List<Square>();

        public Map(int x, int y)
        {
            WorldX = x;
            WorldY = y;
        }

        public void AddSquare(int xPos, int yPos, int xSize, int ySize)
        {
            if((xPos > 0 && xPos < WorldX) && (yPos > 0 && yPos < WorldY) )
            {
                WorldObjects.Add(new Square(xPos, yPos, xSize, ySize));
            }
            else
            {
                throw new ArgumentException("Square out of world");
            }
        }

        public uint GetDistance(int xPos, int yPos, int orientation)
        {
            foreach(var obj in WorldObjects)
            {

            }
            return 0;
        }
    }

    public class Square
    {
        public int xSize;
        public int ySize;
        public int xPos;
        public int yPos;

        public Square(int xPos, int yPos, int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            this.xPos = xPos;
            this.yPos = yPos;
        }
    }
}
