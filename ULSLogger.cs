using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using System.Xml;
using System.Xml.XPath;
using System.IO;


namespace PWC.Process.SixSigma
{
    class ULSLogger : SPDiagnosticsServiceBase
    {
        public static string vsDiagnosticAreaName = "PWC SharePoint Logging Service";
        public static string CategoryName = "PWCProject";
        public static uint uintEventID = 700; // Event ID
        private static ULSLogger _Current;
        public static ULSLogger Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new ULSLogger();
                }
                return _Current;
            }
        }
        private ULSLogger()
            : base("PWC Logging Service", SPFarm.Local)
        { }
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
 {
  new SPDiagnosticsArea(vsDiagnosticAreaName, new List<SPDiagnosticsCategory>
   {
    new SPDiagnosticsCategory(CategoryName, TraceSeverity.Medium, EventSeverity.Error)
   })
  };
            return areas;
        }
        public static string LogErrorInULS(string errorMessage)
        {
            string strExecutionResult = "Message Not Logged in ULS. ";
            try
            {
                SPDiagnosticsCategory category = ULSLogger.Current.Areas[vsDiagnosticAreaName].Categories[CategoryName];
                ULSLogger.Current.WriteTrace(uintEventID, category, TraceSeverity.Unexpected, errorMessage);
                strExecutionResult = "Message Logged";
            }
            catch (Exception ex)
            {
                strExecutionResult += ex.Message;
            }
            return strExecutionResult;
        }
        public static string LogErrorInULS(string errorMessage, TraceSeverity tsSeverity)
        {
            string strExecutionResult = "Message Not Logged in ULS. ";
            try
            {
                SPDiagnosticsCategory category = ULSLogger.Current.Areas[vsDiagnosticAreaName].Categories[CategoryName];
                ULSLogger.Current.WriteTrace(uintEventID, category, tsSeverity, errorMessage);
                strExecutionResult = "Message Logged";
            }
            catch (Exception ex)
            {
                strExecutionResult += ex.Message;
            }
            return strExecutionResult;
        }
    }
}
