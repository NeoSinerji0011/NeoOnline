using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Neosinerji.BABOnlineTP.Business
{
    public interface ICariHesapEkstrePDFStorage : IStorageService
    {
    }

    public class CariHesapEkstrePDFStorage : StorageService, ICariHesapEkstrePDFStorage
    {
        public CariHesapEkstrePDFStorage()
            : base("carihesap-ekstrepdf")
        {

        }
    }
}
