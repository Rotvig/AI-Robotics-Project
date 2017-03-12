using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace AI_In_Robotics.Classes
{
    public class Map
    {
        public readonly int WorldSizeX;
        public readonly int WorldSizeY;
        private readonly List<Line2D> WorldLines = new List<Line2D>();
        private readonly List<Square> WorldObjects = new List<Square>();

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
            if ((xPos > 0 && xPos < WorldSizeX - xSize) && (yPos > 0 && yPos < WorldSizeY - ySize))
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
            var MinLengthToObstacle = observerDistance;

            //Make observar line from vector
            var observerLine = CalcLine(position, observerDistance, orientationDeg);

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
            var EndPoint = new Point2D(lineLength, new Angle(lineOrientationDeg, new Degrees()));
            //Move end point to correct position
            var EndPointMoved = new Point2D(EndPoint.X + StartPoint.X, EndPoint.Y + StartPoint.X);

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
            if (180 < orientationDeg && orientationDeg < 360)
            {
                return observerPoint.Y > intersectPoint.Y;
            }
            if (orientationDeg == 0)
            {
                return observerPoint.X < intersectPoint.X;
            }
            if (orientationDeg == 180)
            {
                return observerPoint.X > intersectPoint.X;
            }
            return false;
        }

        private bool IntersectionIsOnLine(Point2D intersectPoint, Line2D objectLine)
        {
            var margin = 0.00001;

            //Check if the two X values are equal with small margin
            if (Math.Abs(objectLine.StartPoint.X - objectLine.EndPoint.X) < margin)
            {
                //Check if the two Y values are equal with small margin
                if (Math.Abs(objectLine.StartPoint.Y - objectLine.EndPoint.Y) < margin)
                {
                    throw new ArgumentException("Line have same start and end point");
                }
                if (objectLine.StartPoint.Y < objectLine.EndPoint.Y)
                {
                    return intersectPoint.Y >= objectLine.StartPoint.Y && intersectPoint.Y <= objectLine.EndPoint.Y;
                }
                if (objectLine.StartPoint.Y > objectLine.EndPoint.Y)
                {
                    return intersectPoint.Y <= objectLine.StartPoint.Y && intersectPoint.Y >= objectLine.EndPoint.Y;
                }
                return false;
            }
            if (objectLine.StartPoint.X < objectLine.EndPoint.X)
            {
                return intersectPoint.X >= objectLine.StartPoint.X && intersectPoint.X <= objectLine.EndPoint.X;
            }
            if (objectLine.StartPoint.X > objectLine.EndPoint.X)
            {
                return intersectPoint.X <= objectLine.StartPoint.X && intersectPoint.X >= objectLine.EndPoint.X;
            }
            return false;
        }

        public void testMap(Map myMap)
        {
            myMap.AddSquare(3, 3, 2, 2, 0);

            myMap.GetDistance(new Point2D(1, 1), 0, 10);
            myMap.GetDistance(new Point2D(1, 1), 90, 10);
            myMap.GetDistance(new Point2D(4, 1), 90, 10);
        }

        public char[,] GetAStarRoadMap(int fromX, int fromY, int toX, int toY)
        {
            var roadMap = new char[WorldSizeX, WorldSizeY];
            for (var xIndex = 0; xIndex < WorldSizeX; xIndex++)
            {
                for (var yIndex = 0; yIndex < WorldSizeY; yIndex++)
                {
                    //Position of robot
                    if (xIndex == fromX && yIndex == fromY)
                    {
                        roadMap[xIndex, yIndex] = 'S';
                    }
                    else if (xIndex == toX && yIndex == toY)
                    {
                        roadMap[xIndex, yIndex] = 'E';
                    }
                    else if (IsPointInSquare(new Point2D(xIndex, yIndex)))
                    {
                        roadMap[xIndex, yIndex] = 'X';
                    }
                    else
                    {
                        roadMap[xIndex, yIndex] = '-';
                    }
                }
            }

            return roadMap;
        }

        public void PrintRoadMap(char[,] roadMap, Node endNode, int fromX, int fromY, int toX, int toY)
        {
            var rowLength = roadMap.GetLength(0);
            var colLength = roadMap.GetLength(1);

            if (endNode != null)
                roadMap = AddRouteToMap(roadMap, endNode, fromX, fromY, toX, toY);

            for (var yIndex = colLength - 1; yIndex >= 0; yIndex--)
            {
                for (var xIndex = 0; xIndex < rowLength; xIndex++)
                {
                    Console.Write("{0} ", roadMap[xIndex, yIndex]);
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        private char[,] AddRouteToMap(char[,] roadMap, Node endNode, int fromX, int fromY, int toX, int toY)
        {
            var path = new List<Node>();
            while (endNode.x != fromX || endNode.y != fromY)
            {
                path.Add(endNode);
                endNode = endNode.parent;
            }

            path.Add(endNode);

            for (var i = 0; i < path.Count; i++)
            {
                var val = path[i];
                if (roadMap[val.x, val.y] == 'S' || roadMap[val.x, val.y] == 'E')
                    continue;


                if (path[i - 1].y == val.y)
                {
                    if (path[i - 1].x < val.x)
                    {
                        //LEFT
                        roadMap[val.x, val.y] = '←';
                    }
                    else
                    {
                        //RIGHT
                        roadMap[val.x, val.y] = '→';
                    }
                }
                else
                {
                    if (path[i - 1].y < val.y)
                    {
                        //DOWN
                        roadMap[val.x, val.y] = '↓';
                    }
                    else
                    {
                        //UP
                        roadMap[val.x, val.y] = '↑';
                    }
                }
            }

            return roadMap;
        }

        public bool IsPointInSquare(Point2D Point)
        {
            var margin = 0.00001;
            var pointInSqaure = false;

            foreach (var obj in WorldObjects)
            {
                double sumOfArea = 0;
                foreach (var line in obj.SquareEdgeLines)
                {
                    if (CheckIfPointIsOnLine(line, Point))
                    {
                        if(IntersectionIsOnLine(Point, line))
                        {
                            pointInSqaure = true;
                            break;
                        }
                        else
                        {
                            pointInSqaure = false;
                            sumOfArea = 0;
                            break;
                        }
                    }

                    sumOfArea += CalculateTriangleArea(line, Point);
                }

                if (Math.Abs(sumOfArea - obj.area) < margin && sumOfArea != 0)
                    pointInSqaure = true;

                if (pointInSqaure)
                {
                    return true;
                }
                else
                {
                    sumOfArea = 0;
                }
            }

            return false;
        }

        private double CalculateTriangleArea(Line2D line, Point2D P)
        {
            var h = line.LineTo(P, false).Length;
            return 0.5*h*line.Length;
        }

        private bool CheckIfPointIsOnLine(Line2D line, Point2D P)
        {
            var dxc = P.X - line.StartPoint.X;
            var dyc = P.Y - line.StartPoint.Y;
            var dxl = line.EndPoint.X - line.StartPoint.X;
            var dyl = line.EndPoint.Y - line.StartPoint.Y;

            var cross = dxc*dyl - dyc*dxl;
            var margin = 0.00001;

            return cross <= margin && cross >= -margin;
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
            area = xSize*ySize;

            //Make start point of sqaure
            var StartPoint = new Point2D(xPos, yPos);

            //Make 4 line segments of sqaure
            SquareEdgeLines.Add(CalcSquareLine(StartPoint, xSize, lineOrientationDeg));
            SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[0].EndPoint, ySize, lineOrientationDeg + 90));
            SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[1].EndPoint, xSize, lineOrientationDeg + 180));
            SquareEdgeLines.Add(CalcSquareLine(StartPoint, ySize, lineOrientationDeg + 90));
            //SquareEdgeLines.Add(CalcSquareLine(SquareEdgeLines[2].EndPoint, ySize, lineOrientationDeg + 270));
        }

        private Line2D CalcSquareLine(Point2D StartPoint, double lineLength, double lineOrientationDeg)
        {
            //Make end point of observer line
            var EndPoint = new Point2D(lineLength, new Angle(lineOrientationDeg, new Degrees()));
            //Move end point to correct position
            var EndPointMoved = new Point2D(EndPoint.X + StartPoint.X, EndPoint.Y + StartPoint.Y);

            //Make observar line from vector
            return new Line2D(StartPoint, EndPointMoved);
        }
    }
}
