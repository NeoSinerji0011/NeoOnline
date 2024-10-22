using Neosinerji.BABOnlineTP.Business.KesintiTransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KesintiTransferService : IKesintiTransferService
    {
        public string _message;
        public List<Kesintiler> getKesintiler(string path, int tvmKodu, int ay, int yil)
        {
            List<Kesintiler> Kesintiler = new List<Kesintiler>();

            KesintiExcelReader reader = new KesintiExcelReader(path, tvmKodu, ay, yil);
            if (reader == null)
            {
                _message = "reader factory error";
                return null;
            }

            Kesintiler = reader.getKesintiler();
            _message = reader.getMessage();
            return Kesintiler;
        }
        public string getMessage()
        {
            return _message;
        }
    }
}
