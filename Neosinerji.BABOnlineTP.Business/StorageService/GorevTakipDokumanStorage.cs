using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{
    
    public interface IGorevTakipDokumanStorage : IStorageService
    {
    }

    public class GorevTakipDokumanStorage : StorageService, IGorevTakipDokumanStorage
    {
        public GorevTakipDokumanStorage()
            : base("gorevtakip-dokuman")
        {

        }
    }
}
