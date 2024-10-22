using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{   
    public interface IAtananIsLogStorage : IStorageService
    {
    }

    public class AtananIsLogStorage : StorageService, IAtananIsLogStorage
    {
        public AtananIsLogStorage()
            : base("atananis-log")
        {

        }
    }
}
