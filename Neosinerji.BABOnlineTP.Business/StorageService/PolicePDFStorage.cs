using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IPolicePDFStorage : IStorageService
    {
    }

    public class PolicePDFStorage : StorageService, IPolicePDFStorage
    {
        public PolicePDFStorage()
            :base("police-pdf")
        {

        }
    }
}
