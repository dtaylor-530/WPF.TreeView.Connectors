
using DiagramDesigner;
using System.Windows;

namespace DiagramDesigner
{
    public struct ConnectorInfo
    {
        public double DesignerItemLeft { get; set; }
        public double DesignerItemTop { get; set; }
        public Size DesignerItemSize { get; set; }
        public Point Position { get; set; }
        public ConnectorOrientation Orientation { get; set; }
    }

}
