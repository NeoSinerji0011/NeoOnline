using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class RayExcelReader : IPoliceTransferReader
    {
       private SFSExcelRay sfsExcel;
       private bool TahsilatMi;


       public RayExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization

            sfsExcel = new SFSExcelRay(path, tvmKodu, SigortaSirketiBirlikKodlari.RAYSIGORTA, SigortaSirketiBransList, branslar);
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = sfsExcel.getPoliceler();
            
            return policeler;
        
        }


        public string getMessage()
        {
            return sfsExcel.getMessage();
        }
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
