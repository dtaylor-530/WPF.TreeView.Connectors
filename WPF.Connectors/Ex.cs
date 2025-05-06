using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.ComponentModel;
using System.Xml.Linq;

namespace WPF.Connectors
{
    [Flags]
    public enum Position
    {
        Left = 1, Top = 2, Right = 4, Bottom = 8, All = Left | Top | Right | Bottom
    }

    public class Ex
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Position), typeof(Ex), new PropertyMetadata(position_changed));
        public static readonly DependencyProperty IsConnectableProperty =
    DependencyProperty.RegisterAttached("IsConnectable", typeof(bool), typeof(Ex), new PropertyMetadata(is_connectable_changed));


        public static Position GetPosition(UIElement element)
        {
            return (Position)element.GetValue(PositionProperty);
        }

        public static void SetPosition(UIElement element, Position value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static bool GetIsConnectable(UIElement element)
        {
            return (bool)element.GetValue(IsConnectableProperty);
        }

        public static void SetIsConnectable(UIElement element, bool value)
        {
            element.SetValue(IsConnectableProperty, value);
        }


        private static void position_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Position p)
            {
                if (d is FrameworkElement element)
                {

                    watch(p, element);
                    element.SetValue(IsConnectableProperty, true);
                }
            }
        }
        private static void is_connectable_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                if (d is FrameworkElement element)
                {
                    if (element.GetValue(PositionProperty) is null)
                    {
                        watch(Position.All, element);
                    }

                }
            }
        }

        public static void watch(Position p, FrameworkElement _element)
        {
            var disposable = _element
                .Observe(UIElement.IsMouseOverProperty)
                .Subscribe(x =>
                {
                    if (_element.IsMouseOver is true)
                    {
                        Add(_element, p);
                    }
                    else
                        Remove(_element);
                });
        }


        public static void Add(UIElement _element, Position? position = Position.All)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_element);
            if (adornerLayer != null)
            {

                //remove(_element);
                var adorner = new ConnectorAdorner(_element, position);
                adornerLayer.Add(adorner);

            }
        }

        public static void Remove(UIElement _element)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_element);
            if (adornerLayer != null)
            {
                if (adornerLayer.GetAdorners(_element) is Adorner[] adornersOfStackPanel)

                    foreach (var adorner in adornersOfStackPanel)
                    {
                        if (adorner?.IsMouseOver != false)
                        {
                            adorner.Observe(Adorner.IsMouseOverProperty).Subscribe(a =>
                            {
                                if (adorner.IsMouseOver == false)
                                {
                                    adornerLayer.Remove(adorner);

                                }
                            });
                        }
                        else
                        {
                            adornerLayer.Remove(adorner);

                        }
                    }
            }
        }


    }
}
