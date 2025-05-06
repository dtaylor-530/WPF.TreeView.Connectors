using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Connectors
{
    public abstract class Connector(FrameworkElement element, ConnectorOrientation orientation)
    {
        public readonly FrameworkElement element = element;
        public const double margin = 6, width = 14, height = 14, halfWidth = width / 2, halfHeight = height / 2;

        public ConnectorOrientation Orientation { get; } = orientation;

        public abstract Rect Rect();

    }

    public class TopConnector(FrameworkElement element) : Connector(element, ConnectorOrientation.Top)
    {
        public override Rect Rect()
        {
            return new Rect(element.ActualWidth / 2 - halfWidth, margin, width, height);
        }
    }

    public class RightConnector(FrameworkElement element) : Connector(element, ConnectorOrientation.Right)
    {
        public override Rect Rect()
        {
            return new Rect(element.ActualWidth - width - margin, element.ActualHeight / 2 - halfHeight, width, height);
        }
    }

    public class BottomConnector(FrameworkElement element) : Connector(element, ConnectorOrientation.Bottom)
    {
        public override Rect Rect()
        {
            return new Rect(element.ActualWidth / 2 - halfWidth, element.ActualHeight - height - margin, width, height);
        }
    }

    public class LeftConnector(FrameworkElement element) : Connector(element, ConnectorOrientation.Left)
    {
        public override Rect Rect()
        {
            return new Rect(margin, element.ActualHeight / 2 - halfHeight, width, height);
        }
    }
}