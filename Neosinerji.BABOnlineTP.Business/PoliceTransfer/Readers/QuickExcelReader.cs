using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class QuickExcelReader : IPoliceTransferReader
    {
        private SFSExcelQuick sfsExcel;
        private bool TahsilatMi;

        public QuickExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
            sfsExcel = new SFSExcelQuick(path, tvmKodu, SigortaSirketiBirlikKodlari.QUICKSIGORTA, SigortaSirketiBransList, branslar);
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
