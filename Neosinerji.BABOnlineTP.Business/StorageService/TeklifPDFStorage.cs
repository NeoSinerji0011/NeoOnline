using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITeklifPDFStorage : IStorageService
    {
    }

    public class TeklifPDFStorage : StorageService, ITeklifPDFStorage
    {
        public TeklifPDFStorage()
            :base("teklif-pdf")
        {

        }
    }
}
