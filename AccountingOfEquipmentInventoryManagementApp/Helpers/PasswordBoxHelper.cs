using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace AccountingOfEquipmentInventoryManagementApp.Helpers
{
    public static class PasswordBoxHelper
    {
        // Присоединяемое свойство BoundPassword для хранения значения пароля
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        // Флаг, чтобы избежать рекурсивного обновления
        private static bool _isUpdating = false;

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // Удаляем обработчик, чтобы избежать повторной подписки
                passwordBox.PasswordChanged -= PasswordChanged;
                if (!_isUpdating)
                {
                    passwordBox.Password = e.NewValue as string;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        // Метод-обработчик для обновления привязанного значения при изменении пароля в PasswordBox
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _isUpdating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _isUpdating = false;
            }
        }

        // Присоединяемое свойство BindPassword – флаг, чтобы включить привязку
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPasswordProperty);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPasswordProperty, value);
        }

        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                bool wasBound = (bool)e.OldValue;
                bool needToBind = (bool)e.NewValue;

                if (wasBound)
                {
                    passwordBox.PasswordChanged -= PasswordChanged;
                }
                if (needToBind)
                {
                    passwordBox.PasswordChanged += PasswordChanged;
                }
            }
        }
    }
}
