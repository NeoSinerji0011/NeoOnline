using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IKullaniciFotografStorage : IStorageService
    {
        string DownloadToFiles(string url);
    }
    public class KullaniciFotografStorage : StorageService, IKullaniciFotografStorage
    {
        public KullaniciFotografStorage()
            : base("kullanici-fotograf")
        {

        }

        public string DownloadToFiles(string url)
        {
            Uri uri = new Uri(url);
            string fileName = System.IO.Path.GetFileName(uri.LocalPath);

            string serverPath = HttpContext.Current.Server.MapPath("/Files");
            string localFile = String.Format("{0}\\{1}", serverPath, fileName);

            if (!System.IO.File.Exists(localFile))
            {
                int localPathStart = uri.LocalPath.IndexOf(this.ContainerName) + this.ContainerName.Length + 1;
                string path = uri.LocalPath.Substring(localPathStart, uri.LocalPath.Length - localPathStart);

                byte[] contents = this.DownloadFile(path);

                using (System.IO.FileStream sw = System.IO.File.Create(localFile))
                {
                    sw.Write(contents, 0, contents.Length);
                }
            }

            return localFile;
        }
    }
}
