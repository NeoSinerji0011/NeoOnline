using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Tools;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    public class HepIyiExcelReader : IPoliceTransferReader
    {
        private HepIyiExcel excelReader;
        private bool TahsilatMi;
        public HepIyiExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            excelReader = new HepIyiExcel(path, tvmKodu, SigortaSirketiBirlikKodlari.HEPIYISIGORTA, SigortaSirketiBransList, branslar);
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = excelReader.getPoliceler();
            return policeler;
        }

        public string getMessage()
        {
            return excelReader.getMessage();
        }
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
