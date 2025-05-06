using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;


public static class DependencyObjectExtensions
{
    public static IObservable<EventArgs> Observe<T>(this T component, DependencyProperty dependencyProperty)
        where T : DependencyObject
    {
        return Observable.Create<EventArgs>(observer =>
        {
            EventHandler update = (sender, args) => observer.OnNext(args);
            var property = DependencyPropertyDescriptor.FromProperty(dependencyProperty, typeof(T));
            property.AddValueChanged(component, update);
            var disposable = Disposable.Create(() => property.RemoveValueChanged(component, update));
            component.Dispatcher.ShutdownStarted += (s, _) => disposable.Dispose();
            return disposable;
        });
    }
}