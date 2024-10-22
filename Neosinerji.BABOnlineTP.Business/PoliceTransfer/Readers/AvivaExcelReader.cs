using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{

    class AvivaExcelReader : IPoliceTransferReader
    {
        private SFSExcelAviva sfsExcel;
        private bool TahsilatMi;
        public AvivaExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            sfsExcel = new SFSExcelAviva(path, tvmKodu, SigortaSirketiBirlikKodlari.AVIVASIGORTA, SigortaSirketiBransList, branslar);
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
