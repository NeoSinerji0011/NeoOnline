using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IPoliceListesiPDFStorage : IStorageService
    {
      
    }

    public class PoliceListesiPDFStorage : StorageService, IPoliceListesiPDFStorage
    {
        public PoliceListesiPDFStorage()
            : base("policelistesi-donemselraporpdf") { }
          

    } 
}


