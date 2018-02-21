﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cef
{
    public class InvertableBooleanToVisibilityConverter : IValueConverter
    {
        enum Parameters
        {
            Normal, Inverted
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            var direction = parameter != null ? (Parameters)Enum.Parse(typeof(Parameters), (string)parameter) : Parameters.Normal;

            if (direction == Parameters.Inverted)
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}