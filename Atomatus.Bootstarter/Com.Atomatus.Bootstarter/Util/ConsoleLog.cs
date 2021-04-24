using System;
using System.Threading;
using SConsole = System.Console;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// Console Log formatted to print in 
    /// <see cref="System.Console"/> and/or 
    /// <see cref="System.Diagnostics.Debug"/>.
    /// </summary>
    public sealed class ConsoleLog : IDisposable
    {
        #region Log type
        /// <summary>
        /// Console log as information format.
        /// </summary>
        /// <returns>console log instance </returns>
        public static ConsoleLog Info() => new ConsoleLog("info: ", ConsoleColor.Green);
        
        /// <summary>
        /// Console log as warning format.
        /// </summary>
        /// <returns>console log instance </returns>
        public static ConsoleLog Warn() => new ConsoleLog("warn: ", ConsoleColor.DarkYellow, ConsoleColor.Yellow);
        
        /// <summary>
        /// Console log as error format.
        /// </summary>
        /// <returns>console log instance </returns>
        public static ConsoleLog Error() => new ConsoleLog("error: ", ConsoleColor.DarkRed, ConsoleColor.Red);

        /// <summary>
        /// Console log as critical error format.
        /// </summary>
        /// <returns>console log instance </returns>
        public static ConsoleLog Critical() => new ConsoleLog("fatal: ", ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.DarkRed);
        #endregion

        #region Fields and events
        private ConsoleColor? fgColor;
        private ConsoleColor? bgColor;
        private ConsoleColor? tColor;
        private string title;
        private string padLeft;
        private string newLine;
        private int initialized;
        private bool writePadLeft;

        private event Action<string> OnWrite;
        private event Action<ConsoleColor> OnBgColor;
        private event Action<ConsoleColor> OnFgColor;
        private event Action OnResetColor;
        #endregion

        private ConsoleLog(string title, ConsoleColor tColor, ConsoleColor? fgColor = null, ConsoleColor? bgColor = null) 
        {
            this.title   = title;
            this.padLeft = new string(' ', title.Length);
            this.tColor  = tColor;
            this.fgColor = fgColor ?? ConsoleColor.White;
            this.bgColor = bgColor ?? ConsoleColor.Black;
            this.newLine = Environment.NewLine;
        }

        #region Print Mode
        /// <summary>
        /// Enable print value to Debug console.
        /// </summary>
        /// <returns></returns>
        public ConsoleLog Debug()
        {
            this.OnWrite += (str) => System.Diagnostics.Debug.Write(str);
            return this;
        }

        /// <summary>
        /// Enable print vlaue to Application console.
        /// </summary>
        /// <returns></returns>
        public ConsoleLog Console()
        {
            this.OnWrite   += (str) => SConsole.Write(str);
            this.OnBgColor += bg => SConsole.BackgroundColor = bg;
            this.OnFgColor += fg => SConsole.ForegroundColor = fg;

            var bgcolor = SConsole.BackgroundColor;
            var fgcolor = SConsole.ForegroundColor;
            this.OnResetColor += () => 
            {
                SConsole.BackgroundColor = bgcolor;
                SConsole.ForegroundColor = fgcolor;
            };

            return this;
        }
        #endregion

        #region Write
        private ConsoleLog WriteLocal(
            string text,
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor,
            bool breakline)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {

                    this.OnBgColor?.Invoke(bgColor ?? this.bgColor ?? throw new ObjectDisposedException(nameof(ConsoleLog)));

                    bool init = Interlocked.CompareExchange(ref initialized, 1, 0) == 0;
                    if (init)
                    {
                        this.OnFgColor?.Invoke(tColor ?? fgColor ?? this.fgColor ?? throw new ObjectDisposedException(nameof(ConsoleLog)));
                        this.OnWrite?.Invoke(title);
                        this.writePadLeft = true;
                    }

                    this.OnFgColor?.Invoke(fgColor ?? this.fgColor ?? throw new ObjectDisposedException(nameof(ConsoleLog)));
                    
                    var lines = text.Split(newLine);

                    for(int i=0, l = lines.Length; i < l; i++)
                    {
                        if(i > 0) this.OnWrite?.Invoke(newLine);

                        if (init)
                        {
                            init = false;
                        }
                        else if (writePadLeft)
                        {
                            this.OnWrite?.Invoke(padLeft);
                        }

                        this.OnWrite?.Invoke(lines[i]);                        
                    }

                    if (writePadLeft = breakline)
                    {
                        this.OnWrite?.Invoke(newLine);
                    }
                }
                finally
                {
                    this.OnResetColor?.Invoke();
                }
            }

            return this;
        }

        /// <summary>
        /// Write value formatted to Console and/or Debug Console.
        /// </summary>
        /// <param name="text">text to print</param>
        /// <param name="fgColor">text foreground color</param>
        /// <param name="bgColor">console background color</param>
        /// <returns>curren console log instance</returns>
        public ConsoleLog Write(string text, ConsoleColor? fgColor = null, ConsoleColor? bgColor = null) 
            => WriteLocal(text, fgColor, bgColor, false);

        /// <summary>
        /// Write value formatted to Console and/or Debug Console and break line.
        /// </summary>
        /// <param name="text">text to print</param>
        /// <param name="fgColor">text foreground color</param>
        /// <param name="bgColor">console background color</param>
        /// <returns>curren console log instance</returns>
        public ConsoleLog WriteLine(string text, ConsoleColor? fgColor = null, ConsoleColor? bgColor = null)
            => WriteLocal(text, fgColor, bgColor, true);
        #endregion

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.OnResetColor?.Invoke();
            }

            this.OnWrite = null;
            this.OnBgColor = null;
            this.OnFgColor = null;
            this.OnResetColor = null;
            this.tColor = null;
            this.bgColor = null;
            this.fgColor = null;
            this.title = null;
            this.newLine = null;
            this.padLeft = null;
        }

        /// <summary>
        /// Release and dispose object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
