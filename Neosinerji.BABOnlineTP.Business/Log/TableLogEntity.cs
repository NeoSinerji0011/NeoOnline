using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Neosinerji.BABOnlineTP.Business {
	public class TableLogEntity : TableEntity
    {
        private const string LogPartitionKey = "NeoOnlineLog";

        public TableLogEntity()
        {
            this.PartitionKey = TableLogEntity.LogPartitionKey + "_" + TurkeyDateTime.Today.ToString("yyyy_MM_dd");
            this.RowKey = String.Format("{0} - {1}", DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss"), System.Guid.NewGuid());
        }

        public string Url { get; set; }
        public string LogType { get; set; }
        public string KullaniciKodu { get; set; }
        public string TCKN { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string Host { get; set; }
        public string ClientIP { get; set; }
    }
}
