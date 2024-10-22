﻿using System;
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

    class AigExcelReader : IPoliceTransferReader
    {
        private SFSExcelAig sfsExcel;
        private bool TahsilatMi;
        public AigExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran>  branslar)
        {
            sfsExcel = new SFSExcelAig(path, tvmKodu, SigortaSirketiBirlikKodlari.GULFSIGORTA, SigortaSirketiBransList,branslar);
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
