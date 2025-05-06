using WPF.Connectors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Connectors
{
    internal class StraightPathFinder
    {

        public static List<Point> GetConnectionLine(Point source, Point sinkPoint, ConnectorOrientation preferredOrientation)
        {
            List<Point> linePoints = [source, sinkPoint];
            return linePoints;
        }

    }
}
