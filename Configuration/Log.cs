using ClockScreenSaverGL.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.Config
{
    class Log : IDisposable
    {
        private static Log _instance;
        public const string CAT = "Log";
        static private CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static bool LOG_VERBOSE = c.getParametre("Verbose", false);
        static bool LOG_WARNING = c.getParametre("Warning", false);
        static bool LOG_ERROR = c.getParametre("Error", true);
        TextWriter _tw;

        private Log()
        {
            _tw = new StreamWriter(getLogName(), true);
            
        }

        private string getLogName()
        {
            string res = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString(),
             System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            if (!Directory.Exists(res))
                Directory.CreateDirectory(res);

            return Path.Combine(res, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log");
        }

        public static Log getInstance()
        {
            if (_instance == null)
                return _instance = new Log();

            return _instance;
        }

        public void verbose(string message)
        {
            if (LOG_VERBOSE)
                _tw.WriteLine("V:" + date() + " " + message);
        }

        public void warning(string message)
        {
            if (LOG_WARNING)
                _tw.WriteLine("W:" + date() + " " + message);
        }
        public void error(string message)
        {
            if (LOG_ERROR)
                _tw.WriteLine("E:" + date() + " " + message);
        }

        private string date()
        {
            return DateTime.Now.ToString();
        }

        public void Dispose()
        {
            _tw.Flush();
            _tw.Close();
        }

        internal void flush()
        {
            _tw.Flush();
        }
    }
}
