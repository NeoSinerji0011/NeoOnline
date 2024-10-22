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
    

    class NipponExcelReader : IPoliceTransferReader
    {
       private SFSExcelNippon sfsExcel;
        private bool TahsilatMi;

        public NipponExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
            sfsExcel = new SFSExcelNippon(path, tvmKodu, SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA, SigortaSirketiBransList, branslar);           
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
