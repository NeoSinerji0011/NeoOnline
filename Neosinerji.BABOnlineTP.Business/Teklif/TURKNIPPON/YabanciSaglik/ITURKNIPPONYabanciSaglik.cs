using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik
{
    public interface ITURKNIPPONYabanciSaglik :  ITeklif
    {
        List<ListModel> GetMeslekListesi();
        List<ListModel> GetTarifeGrupListesi();
    }

    public class ListModel
    {
        public string kodu { get; set; }
        public string aciklama { get; set; }
    }
}
