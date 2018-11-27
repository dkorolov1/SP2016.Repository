using Microsoft.SharePoint.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SP2016.Repository.Logging
{
    public class Logger : ILog
    {
        private readonly LoggingService loggingService;

        public Logger()
        {
            if (SPFarm.Local != null)
                loggingService = new LoggingService();
            ApplicationName = "SDC";
        }

        public Logger(string applicationName)
        {
            if (SPFarm.Local != null)
                loggingService = new LoggingService() { AreaName = applicationName };
            ApplicationName = applicationName;
        }

        public string ApplicationName { get; set; }

        public void Error(string message, Exception ex, string category)
        {
            Error(message, ex, null, category);
        }

        public void Error(string message, Exception ex, object[] data, string category)
        {
            List<object> fullData = new List<object>();
            AddExceptionInfo(fullData, ex);
            if (ex != null && ex.InnerException != null)
            {
                AddExceptionInfo(fullData, ex.InnerException);
            }
            if (data != null)
                fullData.AddRange(data);

            if (string.IsNullOrEmpty(category))
                category = ApplicationName;

            string fullMessage = GetFullMessage(message, category, fullData.ToArray());

            loggingService?.Error(fullMessage, category, fullData);
#if DEBUG
            Debug.WriteLine(fullMessage);
#endif
            if (OnLogErrorMessage != null)
                OnLogErrorMessage(fullMessage, category);
        }

        private static void AddExceptionInfo(List<object> fullData, Exception ex)
        {
            fullData.Add(ex);
            if (ex != null)
            {
                foreach (DictionaryEntry dataItem in ex.Data)
                {
                    string msg = $"{dataItem.Key}: {dataItem.Value}, ";
                    fullData.Add(msg);
                }
            }
        }

        private string GetFullMessage(string message, string category, object[] data)
        {
            if (string.IsNullOrEmpty(category))
            {
                category = ApplicationName;
            }

            var values = data.Select(e => e?.ToString() ?? string.Empty);
            return $"{ApplicationName}.{category} Сообщение: {message} Дополнительная информация: {string.Join(" ", values)}";
        }


        public void Info(string message, object[] data, string category)
        {
            if (data == null)
                data = new object[0];

            string fullMessage = GetFullMessage(message, category, data);

            WriteInfo(fullMessage, category, data);
        }

        public void Info(string message, string category)
        {
            Info(message, null, category);
        }

        public void InfoFormat(string format, string category, params object[] data)
        {
            if (data == null)
                data = new object[0];

            string message = string.Format(format, data);
            object[] newData = new object[0];
            string fullMessage = GetFullMessage(message, category, newData);

            WriteInfo(fullMessage, category, newData);
        }

        private void WriteInfo(string fullMessage, string category, params object[] data)
        {
            if (string.IsNullOrEmpty(category))
                category = ApplicationName;

            loggingService?.Info(fullMessage, category, data);
#if DEBUG
            Debug.WriteLine(fullMessage);
#endif
            if (OnLogInfoMessage != null)
                OnLogInfoMessage(fullMessage, category);
        }

        public event Action<string, string> OnLogInfoMessage;

        public event Action<string, string> OnLogErrorMessage;

        private class LoggingService : SPDiagnosticsServiceBase
        {
            public HashSet<string> categories { get; } = new HashSet<string> { "General" };

            public string AreaName { get; set; } = "SDC";

            public LoggingService() : base("SDC Logging Service", SPFarm.Local) { }

            protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
            {
                var diagnosticsCategoryList = categories.Select(c => new SPDiagnosticsCategory(c, TraceSeverity.Monitorable, EventSeverity.None));

                return new List<SPDiagnosticsArea> { new SPDiagnosticsArea(AreaName, 0U, 0U, false, diagnosticsCategoryList) };
            }

            public void Error(string message, string category, params object[] data)
            {
                if (!categories.Contains(category))
                    GetType().BaseType.GetField("m_areas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);

                categories.Add(category);

                if (string.IsNullOrWhiteSpace(category))
                    category = "General";

                SPDiagnosticsCategory diagnosticsCategory = Areas[AreaName].Categories[category];
                diagnosticsCategory.EventSeverity = EventSeverity.Error;
                WriteTrace(0, diagnosticsCategory, TraceSeverity.Unexpected, message, data);
            }

            public void Info(string message, string category, params object[] data)
            {
                if (!categories.Contains(category))
                    GetType().BaseType.GetField("m_areas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);

                categories.Add(category);

                if (string.IsNullOrWhiteSpace(category))
                    category = "General";

                SPDiagnosticsCategory diagnosticsCategory = Areas[AreaName].Categories[category];
                diagnosticsCategory.EventSeverity = EventSeverity.Information;
                WriteTrace(0, diagnosticsCategory, TraceSeverity.Monitorable, message, data);
            }
        }
    }
}
