using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace AItest
{
    public class Map
    {
        int WorldSizeX;
        int WorldSizeY;
        List<Line2D> WorldLines = new List<Line2D>();
        List<Square> WorldObjects = new List<Square>();

        public Map(int x, int y)
        {
            WorldSizeX = x;
            WorldSizeY = y;

            WorldLines.Add(new Line2D(new Point2D(0, 0), new Point2D(WorldSizeX, 0)));
            WorldLines.Add(new Line2D(new Point2D(WorldSizeX, 0), new Point2D(WorldSizeX, WorldSizeY)));
            WorldLines.Add(new Line2D(new Point2D(WorldSizeX, WorldSizeY), new Point2D(0, WorldSizeY)));
            WorldLines.Add(new Line2D(new Point2D(0, WorldSizeY), new Point2D(0, 0)));
        }

        public void AddSquare(int xPos, int yPos, int xSize, int ySize, double orientationDeg)
        {
            if((xPos > 0 && xPos < WorldSizeX - xSize) && (yPos > 0 && yPos < WorldSizeY - ySize))
            {
                WorldObjects.Add(new Square(xPos, yPos, xSize, ySize, orientationDeg));

                foreach (var objLines in WorldObjects.Last().SquareEdgeLines)
                {
                    WorldLines.Add(objLines);
                }
            }
            else
            {
                throw new ArgumentException("Square out of world");
            }
        }

        public double GetDistance(Point2D position, double orientationDeg, double observerDistance)
        {
            double MinLengthToObstacle = observerDistance;

            //Make observar line from vector
            Line2D observerLine = CalcLine(position, observerDistance, orientationDeg);

            //Calculate observer line intersections with world lines
            var IntersectPoints = CalcLineIntersections(observerLine, WorldLines, orientationDeg);

            //Calculate length to intersection and find smalles length
            foreach (var instPjt in IntersectPoints)
            {
                var LengthToObstacle = new Line2D(position, instPjt).Length;

                MinLengthToObstacle = (LengthToObstacle < MinLengthToObstacle) ? LengthToObstacle : MinLengthToObstacle;
            }

            return MinLengthToObstacle;
        }

        private Line2D CalcLine(Point2D StartPoint, double lineLength, double lineOrientationDeg)
        {
            //Make end point of observer line
            Point2D EndPoint = new Point2D(lineLength, new Angle(lineOrientationDeg, new Degrees()));
            //Move end point to correct position
            Point2D EndPointMoved = new Point2D(EndPoint.X + StartPoint.X, EndPoint.Y + StartPoint.X);

            //Make observar line from vector
            return new Line2D(StartPoint, EndPointMoved);
        }

        private List<Point2D> CalcLineIntersections(Line2D observerLine, List<Line2D> objectLines, double orientationDeg)
        {
            var IntersectPoints = new List<Point2D>();

            //Calculate observer line intersections with map lines
            foreach (var objLines in objectLines)
            {
                var IntersectPoint = observerLine.IntersectWith(objLines);

                //Check if lines are parallel
                if (IntersectPoint.HasValue)
                {
                    //Check if lines are in correct direction of observer point
                    if (IntersectionIsInRightDirection(observerLine.StartPoint, IntersectPoint.Value, orientationDeg))
                    {
                        //Check if intersection point is on the line
                        if (IntersectionIsOnLine(IntersectPoint.Value, objLines))
                        {
                            IntersectPoints.Add(IntersectPoint.Value);
                        }
                    }
                }
            }

            return IntersectPoints;
        }

        private bool IntersectionIsInRightDirection(Point2D observerPoint, Point2D intersectPoint, double orientationDeg)
        {
            //Check if lines are in correct direction of observer point
            if (0 < orientationDeg && orientationDeg < 180)
            {
                return observerPoint.Y < intersectPoint.Y;
            }
            else if (180 < orientationDeg && orientationDeg < 360)
            {
                return observerPoint.Y > intersectPoint.Y;
            }
            else if (orientationDeg == 0)
            {
                return observerPoint.X < intersectPoint.X;
            }
            else if (orientationDeg == 180)
            {
                return observerPoint.X > intersectPoint.X;
            }
            else
            {
                return false;
            }
        }

        private bool IntersectionIsOnLine(Point2D intersectPoint, Line2D objectLine)
        {
            if (objectLine.StartPoint.X < objectLine.EndPoint.X)
            {
                return intersectPoint.X > objectLine.StartPoint.X && intersectPoint.X < objectLine.EndPoint.X;
            }
            else if (objectLine.StartPoint.X > objectLine.EndPoint.X)
            {
                return intersectPoint.X < objectLine.StartPoint.X && intersectPoint.X > objectLine.EndPoint.X;
            }
            else if (objectLine.StartPoint.X == objectLine.EndPoint.X)
            {
                if (objectLine.StartPoint.Y < objectLine.EndPoint.Y)
                {
                    return intersectPoint.Y > objectLine.StartPoint.Y && intersectPoint.Y < objectLine.EndPoint.Y;
                }
                else if (objectLine.StartPoint.Y > objectLine.EndPoint.Y)
                {
                    return intersectPoint.Y < objectLine.StartPoint.Y && intersectPoint.Y > objectLine.EndPoint.Y;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void testMap(Map myMap)
        {
            myMap.AddSquare(3, 3, 2, 2, 0);

            myMap.GetDistance(new Point2D(1, 1), 0, 10);
            myMap.GetDistance(new Point2D(1, 1), 90, 10);
            myMap.GetDistance(new Point2D(4, 1), 90, 10);
        }

        public char[,] GetAStarRoadMap(int resolution, int fromX, int fromY, int toX, int toY)
        {

            var roadMap = new char[WorldSizeX * resolution, WorldSizeY * resolution];
            for(var i = 0; i < WorldSizeX*resolution; i++)
            {
                for(var j = 0; j < WorldSizeY*resolution; j++)
                {
                    //Position of robot
                    if(i == fromX && j == fromY)
                    {
                        roadMap[i, j] = 'S';
                    }
                    else if (i == toX && j == toY)
                    {
                        roadMap[i, j] = 'E';
                    }
                    else if (IsPointInSquare(new Point2D(i,j)))
                    {
                        roadMap[i, j] = 'X';
                    }
                    else
                    {
                        roadMap[i, j] = '-';
                    }
                }
            }

            return roadMap;
        }

        private bool IsPointInSquare(Point2D StartPoint)
        {
            foreach(var obj in WorldObjects)
            {
                double sumOfArea = 0;
                foreach(var line in obj.SquareEdgeLines)
                {
                    if (CheckIfPointIsOnLine(line, StartPoint))
                        return true;

                    sumOfArea += CalculateTriangleArea(line, StartPoint);
                }

                if(sumOfArea <= obj.area)
                    return true;
            }

            return false;
        }

        private double CalculateTriangleArea(Line2D line, Point2D P)
        {
            var h = line.LineTo(P, false).Length;
            return 0.5 * h * line.Length;
        }

        private bool CheckIfPointIsOnLine(Line2D line, Point2D P)
        {
            var dxc = P.X - line.StartPoint.X;
            var dyc = P.Y - line.StartPoint.Y;
            var dxl = line.EndPoint.X - line.StartPoint.X;
            var dyl = line.EndPoint.Y - line.StartPoint.Y;

            var cross = dxc * dyl - dyc * dxl;
            var margin = 0.001;

            return cross <= margin && cross >= -margin || cross == 0;
        }
    }

    public class Square
    {
        public double xSize;
        public double ySize;
        public double xPos;
        public double yPos;
        public double lineOrientationDeg;
        public List<Line2D> SquareEdgeLines = new List<Line2D>();
        public double area;

        public Square(double xPos, double yPos, double xSize, double ySize, double lineOrientationDeg)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            this.xPos = xPos;
            this.yPos = yPos;
            this.lineOrientationDeg = lineOrientationDeg;
            area = xSize * ySize;

            //Make 4 line segments of sqaure
            SquareEdgeLines.Add(CalcSquareLine(xPos, yPos, xSize, lineOrientationDeg));
            SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[0].EndPoint.X, SquareEdgeLines[0].EndPoint.Y, ySize, lineOrientationDeg + 90));
            SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[1].EndPoint.X, SquareEdgeLines[1].EndPoint.Y, xSize, lineOrientationDeg + 180));
            SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[2].EndPoint.X, SquareEdgeLines[2].EndPoint.Y, ySize, lineOrientationDeg + 270));
        }

        private Line2D CalcSquareLine(double xPos, double yPos, double lineLength, double lineOrientationDeg)
        {
            //Make start point of observer line
            Point2D StartPoint = new Point2D(xPos, yPos);

            //Make end point of observer line
            Point2D EndPoint = new Point2D(lineLength, new Angle(lineOrientationDeg, new Degrees()));
            //Move end point to correct position
            Point2D EndPointMoved = new Point2D(EndPoint.X + xPos, EndPoint.Y + yPos);

            //Make observar line from vector
            return new Line2D(StartPoint, EndPointMoved);
        }
    }
}
