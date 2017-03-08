using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AI_In_Robotics.Classes;

namespace AI_In_Robotics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var pathFinding = new Astar();
            Map myMap = new Map(20, 20);

            myMap.AddSquare(1, 1, 2, 3, 0);
            myMap.AddSquare(12, 12, 5, 5, 45);

            int fromX = 0, fromY = 0, toX = 19, toY = 19;
            var roadMap = myMap.GetAStarRoadMap(fromX, fromY, toX, toY);


            var endNode = pathFinding.AStar(roadMap, fromX, fromY, toX, toY);
            pathFinding.PrintPath(endNode, fromX, fromY, toX, toY);

            myMap.PrintRoadMap(roadMap, endNode, fromX, fromY, toX, toY);
        }
    }
}
