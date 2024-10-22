using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IMusteriDokumanStorage : IStorageService
    {
    }

    public class MusteriDokumanStorage : StorageService, IMusteriDokumanStorage
    {
        public MusteriDokumanStorage()
            :base("musteri-dokuman")
        {

        }
    }
}
