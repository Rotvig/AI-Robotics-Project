using System;
using System.Collections.Generic;
using System.Linq;

//Information:
// http://stackoverflow.com/questions/2138642/how-to-implement-an-a-algorithm
// http://web.mit.edu/eranki/www/tutorials/search/
namespace AItest
{
    public class Astar
    {
        public Node AStar(char[,] matrix, int fromX, int fromY, int toX, int toY)
        {
            //Find max Size of map
            int maxX = matrix.GetLength(0);
            if (maxX == 0)
                return null;
            int maxY = matrix.Length;

            //the keys for open and closed are x.ToString() + y.ToString() of the Node 
            Dictionary<string, Node> open = new Dictionary<string, Node>();
            Dictionary<string, Node> closed = new Dictionary<string, Node>();

            //put the starting node on the open list (you can leave its f at zero)
            Node startNode = new Node { x = fromX, y = fromY };
            string key = startNode.x.ToString() + startNode.y.ToString();
            open.Add(key, startNode);

            // generate startNode's 8 successors and set their parents to startNode
            List<Node> successor = new List<Node>()
                            {
                             new Node {x = -1, y = 0, parent = startNode}, // UP
                             new Node {x = 0, y = 1, parent = startNode }, // RIGHT
                             new Node {x = 1, y = 0, parent = startNode }, // DOWN
                             new Node {x = 0, y = -1, parent = startNode }}; // LEFT

            while (true)
            {
                if (open.Count <= 0)
                    break;

                // find the node with the least f on the open list, call it "smallest"
                // pop smallest off the open list
                var smallest = FindSmallestOpenNode(open);

                if (smallest.Value.x == toX && smallest.Value.y == toY)
                    return smallest.Value;

                open.Remove(smallest.Key);
                closed.Add(smallest.Key, smallest.Value);

                foreach (var suc in successor)
                {
                    int nbrX = smallest.Value.x + suc.x;
                    int nbrY = smallest.Value.y + suc.y;
                    string nbrKey = nbrX.ToString() + nbrY.ToString();

                    // if a node with the same position as successor is in the CLOSED list \ 
                    // which has a lower f than successor, skip this successor
                    if (nbrX < 0 || nbrY < 0 || nbrX >= maxX || nbrY >= maxY
                        || matrix[nbrX, nbrY] == 'X' //obstacles marked by 'X'
                        || closed.ContainsKey(nbrKey))
                        continue;


                    // if a node with the same position as successor is in the OPEN list \
                    // which has a lower f than successor, skip this successor
                    if (open.ContainsKey(nbrKey))
                    {
                        Node curNbr = open[nbrKey];
                        // successor.g = q.g + distance between successor and q
                        int g = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                        // successor.h = distance from goal to successor
                        var h = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                        var f = g + h;
                        if (f < curNbr.f)
                        {
                            curNbr.f = f;
                            curNbr.parent = smallest.Value;
                        }
                    }
                    else
                    {
                        // otherwise, add the node to the open list
                        Node curNbr = new Node { x = nbrX, y = nbrY };
                        // successor.g = q.g + distance between successor and q
                        curNbr.g = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                        // successor.h = distance from goal to successor
                        curNbr.h = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                        // successor.f = successor.g + successor.h
                        curNbr.f = curNbr.g + curNbr.h;
                        curNbr.parent = smallest.Value;
                        open.Add(nbrKey, curNbr);
                    }
                }
            }

            return null;
        }

        private KeyValuePair<string, Node> FindSmallestOpenNode(Dictionary<string, Node> open)
        {
            //find the node with the least f on the open list, call it "smallest"
            var smallest = open.First();

            foreach (var node in open)
            {
                if (node.Value.f < smallest.Value.f)
                    smallest = node;
                else if (node.Value.f == smallest.Value.f
                        && node.Value.h < smallest.Value.h)
                    smallest = node;
            }

            return smallest;
        }

        public void unitTest_AStar()
        {
            char[,] matrix = new char[,] { {'-', 'S', '-', '-', 'X'},
                                           {'-', 'X', '-', '-', '-'},
                                           {'-', '-', 'X', '-', 'X'},
                                           {'X', '-', 'X', 'E', '-'},
                                           {'-', '-', '-', '-', 'X'}};

            //looking for shortest path from 'S' at (0,1) to 'E' at (3,3)
            //obstacles marked by 'X'
            int fromX = 0, fromY = 1, toX = 3, toY = 3;
            var endNode = new Astar().AStar(matrix, fromX, fromY, toX, toY);

            //looping through the Parent nodes until we get to the start node
            Stack<Node> path = new Stack<Node>();

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
                Node node = path.Pop();
                Console.WriteLine("(" + node.x + "," + node.y + ")");
            }
        }

        public void PrintPath(Node endNode, int fromX, int fromY, int toX, int toY)
        {
            //looping through the Parent nodes until we get to the start node
            Stack<Node> path = new Stack<Node>();

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
                Node node = path.Pop();
                Console.WriteLine("(" + node.x + "," + node.y + ")");
            }
        }
    }

    public class Node
    {
        public int g = 0, h = 0, f = 0;
        public int x, y;
        public Node parent;
    }
}

