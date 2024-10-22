using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;
using System.Web;
using System.Configuration;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TableStorageLogService : ILogService
    {
        private const string LogTableName = "NeoOnlineLog";
        private static CloudStorageAccount _cloudStorageAccount;
        IAktifKullaniciService _AktifKullanici;

        public TableStorageLogService(IAktifKullaniciService aktifKullanici)
        {
            _AktifKullanici = aktifKullanici;
        }

        private TableLogEntity GetLogEntry()
        {
            TableLogEntity log = new TableLogEntity();

            if (HttpContext.Current != null)
                log.Url = HttpContext.Current.Request.Path;
            else
                log.Url = String.Empty;

            log.KullaniciKodu = _AktifKullanici.KullaniciKodu.ToString();
            log.TCKN = _AktifKullanici.TCKN;
            log.Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            if (HttpContext.Current != null)
                log.Host = HttpContext.Current.Server.MachineName;
            else
                log.Host = String.Empty;

            log.ClientIP = GetClientIP();

            return log;
        }

        private void Store(TableLogEntity entity)
        {
            if (_cloudStorageAccount == null)
            {
                CloudStorageAccount storageAccount;

                // string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=neoonlinestrg;AccountKey=wrM/Fs9YFcU5FQlVw7KiMtuY2nY6XsjvQ7SCB29qhK6i3jEcbH1r1NuP0zCm2r8IXun1kc4gAiYYqib4RW23ww==;EndpointSuffix=core.windows.net";
                //string connectionString = "DefaultEndpointsProtocol=https;AccountName=neoonlineteststrg;AccountKey=GIKzpmVRKERVupBcohtixyZTlhXoW8umnAO+wLpneFaJzEQcDoH5THfb4C7LMh9Khfecu/gv/xfKK105Vt8w1Q==;EndpointSuffix=core.windows.net";
                if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                    _cloudStorageAccount = storageAccount;
                }
            }

            CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = tableClient.GetTableReference(TableStorageLogService.LogTableName);
            cloudTable.CreateIfNotExists();

            TableOperation insertOperation = TableOperation.Insert(entity);
            cloudTable.Execute(insertOperation);
        }

        public void Visit()
        {
            Visit(String.Empty);
        }

        public void Visit(string message)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Visit";
            log.Message = message;

            this.Store(log);
        }

        public void Info(string message)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Info";
            log.Message = message;

            this.Store(log);
        }

        public void Info(string format, params object[] args)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Info";
            log.Message = String.Format(format, args);

            this.Store(log);
        }

        public void Warning(string message)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Warning";
            log.Message = message;

            this.Store(log);
        }

        public void Warning(string format, params object[] args)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Warning";
            log.Message = String.Format(format, args);

            this.Store(log);
        }

        public void Error(string message)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Error";
            log.Message = message;

            this.Store(log);
        }

        public void Error(string format, params object[] args)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Error";
            log.Message = String.Format(format, args);

            this.Store(log);
        }

        public void Error(Exception ex)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Error";
            log.Message = "";
            log.Data = "";
            if (ex != null)
            {
                log.Message = ex.Message;
                log.Data = GetExceptionReport(ex);
            }

            this.Store(log);
        }

        public void Error(string source, Exception ex)
        {
            TableLogEntity log = GetLogEntry();
            log.LogType = "Error";
            log.Message = "";
            log.Data = "";
            if (ex != null)
            {
                log.Message = String.Format("[{0}] : {1}", source, ex.Message);
                log.Data = GetExceptionReport(ex);
            }

            this.Store(log);
        }

        private static string GetClientIP()
        {
            if (HttpContext.Current != null)
            {
                string ip = HttpContext.Current.Request.UserHostAddress;

                if (String.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (String.IsNullOrEmpty(ip))
                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }

            return String.Empty;
        }

        public static string GetExceptionReport(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Exception--------------------------------------------");
            sb.AppendFormat("Name       : {0}", ex.GetType().FullName);
            sb.AppendLine();
            sb.AppendFormat("Message    : {0}", ex.Message);
            sb.AppendLine();
            sb.AppendFormat("Source     : {0}", ex.Source);
            sb.AppendLine();
            sb.AppendFormat("TargetSite : {0}", ex.TargetSite.Name);
            sb.AppendLine();
            sb.AppendFormat("StackTrace : {0}", ex.StackTrace);
            sb.AppendLine();
            sb.AppendLine();

            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException---------------------------------------");
                sb.AppendFormat("Name       : {0}", ex.InnerException.GetType().FullName);
                sb.AppendLine();
                sb.AppendFormat("Message    : {0}", ex.InnerException.Message);
                sb.AppendLine();
                sb.AppendFormat("Source     : {0}", ex.InnerException.Source);
                sb.AppendLine();

                if (ex.InnerException.TargetSite != null)
                    sb.AppendFormat("TargetSite : {0}", ex.InnerException.TargetSite.Name);
                else
                    sb.AppendFormat("TargetSite : {0}", "");

                sb.AppendLine();
                sb.AppendFormat("StackTrace : {0}", ex.InnerException.StackTrace);
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
