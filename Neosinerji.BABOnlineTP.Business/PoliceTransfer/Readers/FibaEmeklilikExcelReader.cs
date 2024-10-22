using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class FibaEmeklilikExcelReader : IPoliceTransferReader
    {
        private FibaExcelReader fibaExcel;
        private bool TahsilatMi;

        public FibaEmeklilikExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
            fibaExcel = new FibaExcelReader(path, tvmKodu, SigortaSirketiBirlikKodlari.FIBAEMEKLILIK, SigortaSirketiBransList, branslar);
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = fibaExcel.getPoliceler();

            return policeler;

        }

        public string getMessage()
        {
            return fibaExcel.getMessage();
        }
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }

}

