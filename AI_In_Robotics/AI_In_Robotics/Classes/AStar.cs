﻿using System;
using System.Collections.Generic;
using System.Linq;

//Information:
// http://stackoverflow.com/questions/2138642/how-to-implement-an-a-algorithm
// http://web.mit.edu/eranki/www/tutorials/search/
//http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
namespace AI_In_Robotics.Classes
{
    public class Astar
    {
        public Node AStar(char[,] matrix, int startX, int startY, int goalX, int goalY, int fieldsToApply = 0)
        {
            //Find max Size of map
            var maxX = matrix.GetLength(0);
            if (maxX == 0)
                return null;
            var maxY = matrix.GetLength(1);

            var enLargedObjectMatrix = EnLargeObjects(matrix, fieldsToApply);

            //the keys for open and closed are x.ToString() + y.ToString() of the Node 
            var open = new Dictionary<string, Node>();
            var closed = new Dictionary<string, Node>();

            //put the starting node on the open list (you can leave its f at zero)
            var startNode = new Node {x = startX, y = startY};
            var key = startNode.x.ToString() + startNode.y;
            open.Add(key, startNode);

            // generate startNode's 8 successors and set their parents to startNode
            var successor = new List<Node>
            {
                new Node {x = -1, y = 0, parent = startNode}, // UP
                new Node {x = 0, y = 1, parent = startNode}, // RIGHT
                new Node {x = 1, y = 0, parent = startNode}, // DOWN
                new Node {x = 0, y = -1, parent = startNode} // LEFT
            }; 

            while (true)
            {
                if (open.Count <= 0)
                    break;

                // find the node with the least f on the open list, call it "smallest"
                // pop smallest off the open list
                var smallest = FindSmallestOpenNode(open);

                if (smallest.Value.x == goalX && smallest.Value.y == goalY)
                    return smallest.Value;

                open.Remove(smallest.Key);
                closed.Add(smallest.Key, smallest.Value);

                foreach (var suc in successor)
                {
                    var nbrX = smallest.Value.x + suc.x;
                    var nbrY = smallest.Value.y + suc.y;
                    var nbrKey = nbrX.ToString() + nbrY;

                    // if a node with the same position as successor is in the CLOSED list \ 
                    // which has a lower f than successor, skip this successor
                    if (nbrX < 0 || nbrY < 0 || nbrX >= maxX || nbrY >= maxY
                        || enLargedObjectMatrix[nbrX, nbrY] == 'X' //obstacles marked by 'X'
                        || closed.ContainsKey(nbrKey))
                        continue;


                    // if a node with the same position as successor is in the OPEN list \
                    // which has a lower f than successor, skip this successor
                    if (open.ContainsKey(nbrKey))
                    {
                        var curNbr = open[nbrKey];
                        //Distance from start to node
                        var g = Math.Abs(Math.Sqrt((nbrX - goalX) ^ 2 + (nbrY - goalY) ^ 2));
                        // successor.h = heuristic from goal to successor
                        var h = Calculateheuristic(curNbr, startX, startY);
                        var f = g + h;

                        if (!(f < curNbr.f)) continue;

                        curNbr.f = f;
                        curNbr.g = g;
                        curNbr.h = h;
                        curNbr.parent = smallest.Value;
                    }
                    else
                    {
                        // otherwise, add the node to the open list
                        var curNbr = new Node {x = nbrX, y = nbrY};
                        //Distance from start to node
                        curNbr.g = Math.Abs(Math.Sqrt((nbrX - goalX) ^ 2 + (nbrY - goalY) ^ 2));
                        // successor.h = heuristic from goal to successor
                        curNbr.h = Calculateheuristic(curNbr, startX, startY);
                        // successor.f = successor.g + successor.h
                        curNbr.f = curNbr.g + curNbr.h;
                        curNbr.parent = smallest.Value;
                        open.Add(nbrKey, curNbr);
                    }
                }
            }

            return null;
        }

        private int Calculateheuristic(Node node, int x, int y, int D = 1)
        {
            var dx = Math.Abs(node.x - x);
            var dy = Math.Abs(node.y - y);
            return D*(dx + dy);
        }

        public static char[,] EnLargeObjects(char[,] matrix, int fieldsToApply = 0)
        {

            var maxX = matrix.GetLength(0);
            if (maxX == 0)
                return null;
            var maxY = matrix.GetLength(1);

            var enLargedMap = (char[,])matrix.Clone();
            for (var xIndex = 0; xIndex < maxX; xIndex++)
            {
                for (var yIndex = 0; yIndex < maxY; yIndex++)
                {
                    if (matrix[xIndex, yIndex] != 'X') continue;

                    if (CheckIfItsInsideMap(xIndex + fieldsToApply, yIndex, maxX, maxY) && matrix[xIndex + fieldsToApply, yIndex] != 'X')
                    {
                        enLargedMap[xIndex + fieldsToApply, yIndex] = 'X';
                    }
                    if(CheckIfItsInsideMap(xIndex, yIndex + fieldsToApply, maxX, maxY) && matrix[xIndex, yIndex + fieldsToApply] != 'X' )
                    {
                        enLargedMap[xIndex, yIndex + fieldsToApply] = 'X';
                    }
                    if (CheckIfItsInsideMap(xIndex - fieldsToApply, yIndex, maxX, maxY) && matrix[xIndex - fieldsToApply, yIndex] != 'X')
                    {
                        enLargedMap[xIndex - fieldsToApply, yIndex] = 'X';
                    }
                    if (CheckIfItsInsideMap(xIndex, yIndex - fieldsToApply, maxX, maxY) && matrix[xIndex, yIndex - fieldsToApply] != 'X')
                    {
                        enLargedMap[xIndex, yIndex - fieldsToApply] = 'X';
                    }

                    if (CheckIfItsInsideMap(xIndex + fieldsToApply, yIndex + fieldsToApply, maxX, maxY) && matrix[xIndex + fieldsToApply, yIndex + fieldsToApply] != 'X')
                    {
                        enLargedMap[xIndex + fieldsToApply, yIndex + fieldsToApply] = 'X';
                    }
                    if (CheckIfItsInsideMap(xIndex - fieldsToApply, yIndex - fieldsToApply, maxX, maxY) && matrix[xIndex - fieldsToApply, yIndex - fieldsToApply] != 'X')
                    {
                        enLargedMap[xIndex - fieldsToApply, yIndex - fieldsToApply] = 'X';
                    }
                    if (CheckIfItsInsideMap(xIndex - fieldsToApply, yIndex + fieldsToApply, maxX, maxY) && matrix[xIndex - fieldsToApply, yIndex + fieldsToApply] != 'X')
                    {
                        enLargedMap[xIndex - fieldsToApply, yIndex + fieldsToApply] = 'X';
                    }
                    if (CheckIfItsInsideMap(xIndex + fieldsToApply, yIndex - fieldsToApply, maxX, maxY) && matrix[xIndex + fieldsToApply, yIndex - fieldsToApply] != 'X')
                    {
                        enLargedMap[xIndex + fieldsToApply, yIndex - fieldsToApply] = 'X';
                    }
                }
            }
            return enLargedMap;
        }

        private static bool CheckIfItsInsideMap(int xIndex, int yIndex, int maxX, int maxY)
        {
            return (xIndex >= 0 && xIndex < maxX) && (yIndex >= 0 && yIndex < maxY);
        }

        private KeyValuePair<string, Node> FindSmallestOpenNode(Dictionary<string, Node> open)
        {
            //find the node with the least f on the open list
            return open.OrderBy(x => x.Value.f).First();
        }

        public void unitTest_AStar()
        {
            char[,] matrix =
            {
                {'-', 'S', '-', '-', 'X'},
                {'-', 'X', '-', '-', '-'},
                {'-', '-', 'X', '-', 'X'},
                {'X', '-', 'X', 'E', '-'},
                {'-', '-', '-', '-', 'X'}
            };

            //looking for shortest path from 'S' at (0,1) to 'E' at (3,3)
            //obstacles marked by 'X'
            int fromX = 0, fromY = 1, toX = 3, toY = 3;
            var endNode = new Astar().AStar(matrix, fromX, fromY, toX, toY);

            //looping through the Parent nodes until we get to the start node
            var path = new Stack<Node>();

            while (endNode.x != fromX || endNode.y != fromY)
            {
                path.Push(endNode);
                endNode = endNode.parent;
            }

            path.Push(endNode);

            Console.WriteLine("The shortest path from  " +
                              "(" + fromX + "," + fromY + ")  to " +
                              "(" + toX + "," + toY + ")  is:  \n");

            while (path.Count > 0)
            {
                var node = path.Pop();
                Console.WriteLine("(" + node.x + "," + node.y + ")");
            }
        }

        public void PrintPath(Node endNode, int fromX, int fromY, int toX, int toY)
        {
            //looping through the Parent nodes until we get to the start node
            var path = new Stack<Node>();

            while (endNode.x != fromX || endNode.y != fromY)
            {
                path.Push(endNode);
                endNode = endNode.parent;
            }

            path.Push(endNode);

            Console.WriteLine("The shortest path from  " +
                              "(" + fromX + "," + fromY + ")  to " +
                              "(" + toX + "," + toY + ")  is:  \n");

            while (path.Count > 0)
            {
                var node = path.Pop();
                Console.WriteLine("(" + node.x + "," + node.y + ")");
            }
        }
    }

    public class Node
    {
        public double g, h, f;
        public int x, y;
        public Node parent;
    }
}

