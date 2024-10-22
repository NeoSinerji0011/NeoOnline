using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database
{
    [Serializable]
    public class DuyuruProcedureModel
    {
        public int DuyuruId { get; set; }
        public string Konu { get; set; }
        public DateTime BaslangisTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
    }
}
