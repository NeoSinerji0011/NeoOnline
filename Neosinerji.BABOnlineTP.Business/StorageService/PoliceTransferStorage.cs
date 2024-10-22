using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IPoliceTransferStorage : IStorageService
    {
    }

    public class PoliceTransferStorage : StorageService, IPoliceTransferStorage
    {
        public PoliceTransferStorage()
            : base("otomatik-police-transfer")
        {

        }
    }
}
