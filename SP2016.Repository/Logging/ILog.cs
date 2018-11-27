using System;

namespace SP2016.Repository.Logging
{
    public interface ILog
    {
        string ApplicationName { get; set; }
        void Error(string message, Exception ex, string category = "");
        void Error(string message, Exception ex, object[] data, string category = "");
        void Info(string message, object[] data, string category = "");
        void Info(string message, string category = "");
        void InfoFormat(string format, string category, params object[] data);
    }
}
