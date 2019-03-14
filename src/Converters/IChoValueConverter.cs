namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Globalization;

	#endregion NameSpaces

	public interface IChoValueConverter
	{
		object Convert(object value, Type targetType, object parameter, CultureInfo culture);
		object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}

	public interface IChoMultiValueConverter
	{
		object Convert(object[] value, Type targetType, object parameter, CultureInfo culture);
		object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}
}
