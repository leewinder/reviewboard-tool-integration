using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Logging
{
    // Simple logging class
    public abstract class Logging : IDisposable
    {
        // Thread behaviour
        public enum Threading
        {
            SingleThread,
            MultiThread,
        }

        // Type of logs
        public enum Type
        {
            Null,
            File,
        }

        //
        // Creates a logging object
        //
        public static Logging Create(string logName, Type logType, Threading threadBehaviour)
        {
            // Create our object
            if (logType == Type.File)
                return new Implementations.LoggerFile(logName, threadBehaviour);
            else
                return new Implementations.LoggerNull();
        }

        //
        // Logs text out
        //
        public void Log(string text)
        {
            WriteText(text);
        }

        //
        // Logs text out
        //
        public void Log(string text, object arg0)
        {
            string logMessage = string.Format(text, arg0);
            WriteText(logMessage);
        }

        //
        // Logs text out
        //
        public void Log(string text, object arg0, object arg1)
        {
            string logMessage = string.Format(text, arg0, arg1);
            WriteText(logMessage);
        }

        //
        // Logs text out
        //
        public void Log(string text, object arg0, object arg1, object arg2)
        {
            string logMessage = string.Format(text, arg0, arg1, arg2);
            WriteText(logMessage);
        }

        //
        // Logs text out
        //
        public void Log(string text, params object[] args)
        {
            string logMessage = string.Format(text, args);
            WriteText(logMessage);
        }

        //
        // Disposes the object
        //
        public void Dispose()
        {
            Dispose(true);
        }

        // Protected delegates
        protected delegate void WriteTextDelegate(string logMessage);
        protected delegate void OnDisposeDelegate();

        // Properties
        protected WriteTextDelegate WriteText { private get; set; }

        //
        // Dispose method
        //
        protected virtual void Dispose(bool supressFinalise)
        {
            if (supressFinalise == true)
                GC.SuppressFinalize(this);
        }
    }
}
