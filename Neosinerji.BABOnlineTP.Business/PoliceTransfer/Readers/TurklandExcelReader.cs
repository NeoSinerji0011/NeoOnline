using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class TurklandExcelReader : IPoliceTransferReader
    {

        private SFSExcelTurkland sfsExcel;
        private bool TahsilatMi;
        public TurklandExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            sfsExcel = new SFSExcelTurkland(path, tvmKodu, SigortaSirketiBirlikKodlari.TURKLANDSIGORTA, SigortaSirketiBransList, branslar);
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
