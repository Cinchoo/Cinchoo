namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [Serializable]
    public class ChoConsolePercentageProgressorSettings
    {
        public int ProgressBarStatusMsgSize = 50;
        public string UnitIndicator = "%";
        public int ProgressBarSize = 50;
        public int ProgressBarMarginX = 10;
        public int ProgressBarMarginY = 3;
        public ConsoleColor ProgressBarFontColor = ConsoleColor.White;
        public ConsoleColor ProgressBarCompleteColor = ConsoleColor.Blue;
        public ConsoleColor ProgressBarIncompleteColor = ConsoleColor.DarkGray;
        public ConsoleColor ProgressBarScaleForegroundColor = ConsoleColor.Yellow;
        public ConsoleColor ProgressBarScaleBackgroundColor = Console.BackgroundColor;
        public ConsoleColor ProgressBarStatusMsgForegroundColor = ConsoleColor.Yellow;
        public ConsoleColor ProgressBarStatusMsgBackgroundColor = Console.BackgroundColor;
        public bool DisplayScale = true;
    }

}
