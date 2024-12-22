using Cinchoo.Core;
using Cinchoo.Core.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsingValueConverter
{
    public class ChoTimeoutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;

            int timeout = 0;
            if (String.Compare("INFINITE", value.ToString(), true) == 0)
                return -1;
            else if (int.TryParse(value.ToString(), out timeout))
                return timeout;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.ToString() : String.Empty;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoSleepConverterAttribute : ChoTypeConverterAttribute
    {
        public ChoSleepConverterAttribute()
            : base(typeof(ChoSleepConverter))
        {
        }
    }

    public class ChoSleepConverter : IChoValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;

            int timeout = 0;
            if (String.Compare("INFINITE", value.ToString(), true) == 0)
                return -1;
            else if (int.TryParse(value.ToString(), out timeout))
                return timeout;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.ToString() : String.Empty;
        }
    }

    /// <summary>
    /// Possible command line arguments
    ///     Sample4 /n:Raj /t:100 /s:INFINITE
    ///     Sample4 /n:Tom /t:INFINITE /s:1000
    /// </summary>
    [ChoCommandLineArgObject(ApplicationName = "Hello world", Copyright = "Copyright 2014 Cinchoo Inc.")]
    public class HelloWorldCmdLineParams : ChoCommandLineArgObject
    {
        [ChoCommandLineArg("n", IsRequired = true, Description = "Name of the person.", Order = 1)]
        public string Name;

        [ChoCommandLineArg("s", ShortName = "<int | INFINITE>", Description = "Sleep period.", DefaultValue = "-1")]
        [ChoSleepConverter]
        public int Sleep
        {
            get;
            set;
        }

        [ChoCommandLineArg("t", ShortName = "<int | INFINITE>", Description = "Timeout period.", DefaultValue = "~System.Int32.MinValue~", FallbackValue = "0")]
        [ChoTypeConverter(typeof(ChoTimeoutConverter))]
        public int Timeout
        {
            get;
            set;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            HelloWorldCmdLineParams cmd = new HelloWorldCmdLineParams();
            Console.WriteLine(cmd.ToString());
        }
    }
}
