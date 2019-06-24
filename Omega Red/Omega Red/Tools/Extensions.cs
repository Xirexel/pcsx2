using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Omega_Red.Tools
{
    public partial class Extensions
    {
        public static readonly DependencyProperty ValueChangedCommandProperty = DependencyProperty.RegisterAttached("ValueChangedCommand", typeof(ICommand), typeof(Extensions), new UIPropertyMetadata((s, e) =>
        {
            var element = s as System.Windows.Controls.Slider;

            if (element != null)
            {
                element.ValueChanged -= OnSingleValueChanged;

                element.TouchDown -= OnTouchDown;


                if (e.NewValue != null)
                {
                    element.ValueChanged += OnSingleValueChanged;

                    element.TouchDown += OnTouchDown;
                }
            }
        }));

        private static void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

        public static ICommand GetValueChangedCommand(UIElement element)
        {
            return (ICommand)element.GetValue(ValueChangedCommandProperty);
        }

        public static void SetValueChangedCommand(UIElement element, ICommand value)
        {
            element.SetValue(ValueChangedCommandProperty, value);
        }

        private static void OnSingleValueChanged(object sender, RoutedEventArgs e)
        {
            var element = sender as System.Windows.FrameworkElement;
            var command = element.GetValue(ValueChangedCommandProperty) as ICommand;

            if (command != null && command.CanExecute(element))
            {
                command.Execute(element.Tag);
                e.Handled = true;
            }
        }
    }
}
