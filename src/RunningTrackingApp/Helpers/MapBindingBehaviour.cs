using Mapsui.UI.Wpf;
using Mapsui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RunningTrackingApp.Helpers
{
    /// <summary>
    /// A workaround to ensure a pure MVVM pattern so code-behind is not required.
    /// Necessary because the Mapsui.Maps object is not bindable.
    /// ps this took a fair bit of research to work out...
    /// </summary>
    internal static class MapBindingBehaviour
    {
        public static readonly DependencyProperty BindableMapProperty =
            DependencyProperty.RegisterAttached(
                "BindableMap",
                typeof(Map),
                typeof(MapBindingBehaviour),
                new PropertyMetadata(null, OnMapChanged));

        public static void SetBindableMap(UIElement element, Map value)
        {
            element.SetValue(BindableMapProperty, value);
        }

        public static Map GetBindableMap(UIElement element)
        {
            return (Map)element.GetValue(BindableMapProperty);
        }

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MapControl mapControl)
            {
                mapControl.Map = e.NewValue as Map;
            }
        }
    }
}
