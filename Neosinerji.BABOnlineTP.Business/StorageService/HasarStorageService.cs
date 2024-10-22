using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    
    public interface IHasarStorageService : IStorageService
    {
    }

    public class HasarStorageService : StorageService, IHasarStorageService
    {
        public HasarStorageService()
            : base("hasar-evrak")
        {

        }
    }
}
