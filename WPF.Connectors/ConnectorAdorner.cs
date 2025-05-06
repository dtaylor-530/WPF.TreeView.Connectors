using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;


[assembly: XmlnsDefinition("urn:diagram-designer-ns", "WPF.Connectors")]

namespace WPF.Connectors
{

    public class ConnectorAdorner : Adorner
    {
        // drag start point, relative to the DesignerCanvas
        private Point? dragStartPoint = null;
        private Canvas _element;
        private Position? _position;
        private Connector x;


        public ConnectorAdorner(UIElement adornedElement, Position? position = default) : base(adornedElement)
        {
            this.Width = Connector.width; this.Height = Connector.height;
            this._position = position.HasValue ? position : WPF.Connectors.Position.All;


            // fired when layout changes
            adornedElement.LayoutUpdated += Connector_LayoutUpdated;
        }


        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Canvas canvas = GetDesignerCanvas(this.AdornedElement);
            if (canvas != null)
            {
                this.dragStartPoint = new Point?(e.GetPosition(canvas));

                if (AdornedElement is FrameworkElement frameworkElement && dragStartPoint is Point point)
                {
                    x = Match(frameworkElement, canvas, point);
                }
                e.Handled = true;
            }
        }

        public Connector Match(FrameworkElement frameworkElement, Canvas canvas, Point point)
        {

            foreach (var x in rects(frameworkElement))
            {
                var pos = x.Rect();
                var rect = new Rect(this.AdornedElement.TransformToAncestor(canvas).Transform(new Point(pos.X, pos.Y)), pos.Size);
                if (rect.Contains(point))
                {
                    return x;
                }
            }
            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            // but if mouse button is pressed and start point value is set we do have one
            if (this.dragStartPoint.HasValue)
            {
                Canvas canvas = GetDesignerCanvas(this.AdornedElement);
                if (canvas != null)
                {

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    if (adornerLayer != null)
                    {

                        var connector = Match(AdornedElement as FrameworkElement, canvas, dragStartPoint.Value);
                        ConnectionAdorner adorner = new ConnectionAdorner(canvas, connector, this);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (AdornedElement is FrameworkElement frameworkElement)
            {
                foreach (var r in rects(frameworkElement))
                    dc.DrawRectangle(Brushes.LightGray, new Pen(Brushes.LightCyan, 1), r.Rect());
            }
        }


        private IEnumerable<Connector> rects(FrameworkElement frameworkElement)
        {
            foreach (var x in EnumHelper.SeparateFlags(_position.Value))
            {
                switch (x)
                {
                    case WPF.Connectors.Position.Top:
                        yield return new TopConnector(frameworkElement);
                        break;
                    case WPF.Connectors.Position.Right:
                        yield return new RightConnector(frameworkElement);
                        break;
                    case WPF.Connectors.Position.Bottom:
                        yield return new BottomConnector(frameworkElement);
                        break;
                    case WPF.Connectors.Position.Left:
                        yield return new LeftConnector(frameworkElement);
                        break;
                }
            }
        }

        //internal ConnectorInfo GetInfo()
        //{
        //    ConnectorInfo info = new ConnectorInfo();
        //    //info.DesignerItemLeft = DesignerCanvas.GetLeft(this.AdornedElement ) + 300;
        //    //info.DesignerItemTop = DesignerCanvas.GetTop(this.AdornedElement) + 100; ;
        //    //info.DesignerItemSize = new Size(this.AdornedElement.ActualWidth, this.ParentDesignerItem.ActualHeight);
        //    info.Orientation = this.Orientation;
        //    info.Position = this.x.Rect().Centre();
        //    return info;
        //}

        // iterate through visual tree to get parent DesignerCanvas
        private Canvas GetDesignerCanvas(DependencyObject element)
        {
            if (_element != null)
                return _element;
            while (element != null && !(element is Canvas))
                element = VisualTreeHelper.GetParent(element);

            return _element = element as Canvas;
        }


        public void Remove()
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }
    } 
}
