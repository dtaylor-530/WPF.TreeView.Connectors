using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Policy;
using System;
using System.Diagnostics;

namespace WPF.Connectors
{
    public class ConnectionAdorner : Adorner
    {
        private PathGeometry pathGeometry;
        private Canvas designerCanvas;
        private readonly ConnectorAdorner sourceConnectorAdorner;
        private Pen drawingPen;

        FrameworkElement hitItem = null;
        private bool isComplete;
        private Connector sourceConnector, sinkConnector;

        public ConnectionAdorner(Canvas designer, Connector sourceConnector, ConnectorAdorner sourceConnectorAdorner)
            : base(designer)
        {
            //this.IsHitTestVisible = false;
            this.designerCanvas = designer;
            this.sourceConnector = sourceConnector;
            this.sourceConnectorAdorner = sourceConnectorAdorner;
            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;
            this.Cursor = Cursors.Cross;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (hitItem != null)
            {
                AdornerLayer _adornerLayer = AdornerLayer.GetAdornerLayer(this.hitItem);
                if (_adornerLayer != null)
                {

                    if (_adornerLayer.GetAdorners(this.hitItem) is Adorner[] adornersOfStackPanel)
                    {

                        var hitPoint = e.GetPosition(this);

                        foreach (var adorner in adornersOfStackPanel)
                        {
                            if (adorner is ConnectorAdorner connAdorner)
                            {
                                //_tuple = connAdorner.Match(this.hitItem, designerCanvas, hitPoint);
                                var hitConnector = connAdorner;
                                sinkConnector = hitConnector.Match(this.hitItem, designerCanvas, hitPoint);
                            }
                        }
                    }
                }
            }

            if (sinkConnector != null)
            {
                update();
                sourceConnector.element.SizeChanged += (s, _e) =>
                {

                    update();

                };
                sinkConnector.element.SizeChanged += (s, _e) =>
                {

                    update();
                };

                sourceConnector.element.IsVisibleChanged += (s, _e) =>
                {

                    update();
                };

                sinkConnector.element.IsVisibleChanged += (s, _e) =>
                {

                    update();
                };

                sourceConnectorAdorner.Remove();
                this.IsHitTestVisible = false;
                isComplete = true;
            }
            else
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
            InvalidateVisual();


            if (this.IsMouseCaptured)
                this.ReleaseMouseCapture();


        }

        void update()
        {
            this.pathGeometry = GetPathGeometry(sourceCentre(), sinkCentre());
            this.InvalidateVisual();
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                var sinkCentre = e.GetPosition(this);
                HitTesting(sinkCentre);
                this.pathGeometry = GetPathGeometry(sourceCentre(), sinkCentre);
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }
        }

        private Point sourceCentre()
        {
            return sourceConnector.element.TransformToAncestor(designerCanvas).Transform(sourceConnector.Rect().Centre());
        }

        private Point sinkCentre()
        {
            return sinkConnector.element.TransformToAncestor(designerCanvas).Transform(sinkConnector.Rect().Centre());
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (sinkConnector != null && sinkConnector.element.IsVisible == false || sourceConnector.element.IsVisible == false)
                return;

            dc.DrawGeometry(null, drawingPen, this.pathGeometry);

            if (sinkConnector != null)
                dc.DrawEllipse(Brushes.LightBlue, new Pen(Brushes.Black, 1), sinkCentre(), 2, 2);

            if (sourceConnector != null)
                dc.DrawEllipse(Brushes.Pink, new Pen(Brushes.Black, 1), sourceCentre(), 2, 2);

            // without a background the OnMouseMove event would not be fired
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            if (this.isComplete == false)
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

        }

        private PathGeometry GetPathGeometry(Point sourcePosition, Point sinkPosition)
        {
            PathGeometry geometry = new PathGeometry();

            ConnectorOrientation targetOrientation;
            if (sinkConnector != null)
                targetOrientation = sinkConnector.Orientation;
            else
                targetOrientation = ConnectorOrientation.None;

            List<Point> pathPoints = StraightPathFinder.GetConnectionLine(sourcePosition, sinkPosition, targetOrientation);

            if (pathPoints.Count > 0)
            {
                PathFigure figure = new PathFigure
                {
                    StartPoint = pathPoints[0]
                };
                pathPoints.Remove(pathPoints[0]);
                figure.Segments.Add(new PolyLineSegment(pathPoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }


        private void HitTesting(Point hitPoint)
        {
            DependencyObject hitObject = designerCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != sourceConnectorAdorner.AdornedElement &&
                   hitObject.GetType() != typeof(Canvas))
            {
                if (hitObject is ConnectorAdorner)
                    return;

                if (hitObject is TreeViewItem tvitem)
                {
                    Debug.WriteLine(hitObject.GetType().Name);
                }
                //if (hitObject is DesignerItem ditem)
                //{


                //    hitItem = ditem;
                //    add(ditem, Position.All);
                //    return;

                //    //if (!hitConnectorFlag)
                //    //    HitConnector = null;

                //}
                if (hitObject is FrameworkElement item)
                {
                    if (item.GetValue(Ex.IsConnectableProperty) is true)
                    {
                        var pos = (Position?)item.GetValue(Ex.PositionProperty);

                        if (hitItem != null)
                            Ex.Remove(hitItem);
                        hitItem = item;
                        Ex.Add(item, pos);
                        return;
                    }


                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }
            if (hitItem != null)
            {

                Ex.Remove(hitItem);
                hitItem = null;
            }
        }

        //public void add(FrameworkElement element, Position? pos = null)
        //{
        //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
        //    if (adornerLayer != null)
        //    {

        //        if (adornerLayer.GetAdorners(this.hitItem) is Adorner[] adornersOfStackPanel)
        //        {


        //            foreach (var _adorner in adornersOfStackPanel)
        //            {
        //                if (_adorner is ConnectorAdorner connAdorner)
        //                {
        //                    adornerLayer.Remove(_adorner);
        //                }
        //            }
        //        }
        //        //remove();
        //        var adorner = new ConnectorAdorner(element, pos);
        //        adornerLayer.Add(adorner);
        //        //e.Handled = true;
        //    }
        //}
    }
}
