using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class BereketExcelReader : IPoliceTransferReader
    {
        private VizyoneksExcelBereket vizyoneksExcel;
        private bool TahsilatMi;
        public BereketExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            vizyoneksExcel = new VizyoneksExcelBereket(path, tvmKodu, SigortaSirketiBirlikKodlari.BEREKET, SigortaSirketiBransList, branslar);
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = vizyoneksExcel.getPoliceler();
            return policeler;
        }

        public string getMessage()
        {
            return vizyoneksExcel.getMessage();
        }
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
