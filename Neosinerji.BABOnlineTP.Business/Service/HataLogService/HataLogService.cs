using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business {
	public class HataLogService : IHataLogService {
		private const string LogTableName = "NeoOnlineLog";
		//IKullaniciService _KullaniciService;

		ITVMContext _db;
		public HataLogService(ITVMContext tvmContext) {
			_db = tvmContext;
		}

		public List<YetkiHataServisModel> GetHataList(string KullaniciAdi, DateTime Tarih, string Saat, string logType) {
			List<YetkiHataServisModel> list = new List<YetkiHataServisModel>();

			int kullaniciKodu = 0;
			TVMKullanicilar kullanici = _db.TVMKullanicilarRepository.Filter(f => f.Adi == KullaniciAdi).FirstOrDefault();
			if (kullanici != null)
				kullaniciKodu = kullanici.KullaniciKodu;

			CultureInfo turkey = new CultureInfo("tr-TR");

			string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            //connectionString = "DefaultEndpointsProtocol=https;AccountName=neoonlinestrg;AccountKey=wrM/Fs9YFcU5FQlVw7KiMtuY2nY6XsjvQ7SCB29qhK6i3jEcbH1r1NuP0zCm2r8IXun1kc4gAiYYqib4RW23ww==;EndpointSuffix=core.windows.net";

            CloudStorageAccount storageAccount;
			if (CloudStorageAccount.TryParse(connectionString, out storageAccount)) {
				CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
				CloudTable cloudTable = tableClient.GetTableReference(LogTableName);
				cloudTable.CreateIfNotExists();

				string tableEndpoint = storageAccount.TableEndpoint.AbsoluteUri;

				string partition = String.Format("NeoOnlineLog_{0:yyyy_MM_dd}", Tarih);
				string date = String.Format("{0} {1}", Tarih.ToString("yyyy-MM-dd"), Saat);

				DateTime utcDate = Convert.ToDateTime(date).ToUniversalTime();


				string queryPartitionKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition);
				string queryDate = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.GreaterThan, utcDate);

				string filterString = TableQuery.CombineFilters(queryPartitionKey, TableOperators.And, queryDate);

				if (0 < kullaniciKodu) {
					string queryUserId = TableQuery.GenerateFilterConditionForInt("KullaniciKodu", QueryComparisons.Equal, kullaniciKodu);
					filterString = TableQuery.CombineFilters(filterString, TableOperators.And, queryUserId);
				}

				if (!string.IsNullOrWhiteSpace(logType)) {
					string queryLogType = TableQuery.GenerateFilterCondition("LogType", QueryComparisons.Equal, logType);
					filterString = TableQuery.CombineFilters(filterString, TableOperators.And, queryLogType);
				}


				TableQuery<TableLogEntity> query = new TableQuery<TableLogEntity>().Where(filterString);
				query.TakeCount = 200;

				var logEntities = cloudTable.ExecuteQuery(query);
				var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

				foreach (var item in logEntities) {
					YetkiHataServisModel mdl = new YetkiHataServisModel();

					mdl.clientIp = item.ClientIP;
					mdl.hostName = item.Host;
					mdl.kullanici = item.KullaniciKodu;
					DateTime dt = Convert.ToDateTime(item.Date);
					mdl.logDate = TimeZoneInfo.ConvertTime(dt, turkeyTimeZone);
					mdl.logType = item.LogType;
					mdl.message = item.Message;
					mdl.url = item.Url;

					list.Add(mdl);
				}

			}
			
			return list;
		}
	}
}
