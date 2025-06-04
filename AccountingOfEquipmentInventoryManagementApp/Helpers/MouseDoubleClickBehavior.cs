using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace AccountingOfEquipmentInventoryManagementApp.Helpers
{
    public static class MouseDoubleClickBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(MouseDoubleClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, ICommand value) =>
            element.SetValue(CommandProperty, value);

        public static ICommand GetCommand(UIElement element) =>
            (ICommand)element.GetValue(CommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    uiElement.PreviewMouseLeftButtonDown += UiElement_PreviewMouseLeftButtonDown;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    uiElement.PreviewMouseLeftButtonDown -= UiElement_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void UiElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var uiElement = sender as UIElement;
                var command = GetCommand(uiElement);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
