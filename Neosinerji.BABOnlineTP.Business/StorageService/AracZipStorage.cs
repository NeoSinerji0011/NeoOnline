using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IAracZipStorage : IStorageService
    {
    }

    public class AracZipStorage : StorageService, IAracZipStorage
    {
        public AracZipStorage()
            :base("arac-zip")
        {

        }
    }
}
