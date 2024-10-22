using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IDuyuruDokumanStorage : IStorageService
    {
    }

    public class DuyuruDokumanStorage : StorageService, IDuyuruDokumanStorage
    {
        public DuyuruDokumanStorage()
            : base("duyuru-dokuman")
        {

        }
    }

}
