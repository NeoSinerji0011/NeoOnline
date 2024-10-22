using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IWEBServiceLogStorage : IStorageService
    {
    }

    public class WEBServiceLogStorage : StorageService, IWEBServiceLogStorage
    {
        public WEBServiceLogStorage()
            : base("service-log")
        {

        }
    }
}
