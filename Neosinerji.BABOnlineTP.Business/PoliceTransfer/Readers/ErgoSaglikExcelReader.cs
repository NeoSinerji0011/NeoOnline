using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class ErgoSaglikExcelReader
    {

        private ExcelErgoSaglik sfsExcel;
        public ErgoSaglikExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            sfsExcel = new ExcelErgoSaglik(path, tvmKodu, SigortaSirketiBirlikKodlari.ERGOSAGLIK, SigortaSirketiBransList, branslar);
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
    }
}
