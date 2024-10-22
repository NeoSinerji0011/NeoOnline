using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza
{
    public interface  IKoruFerdiKaza : ITeklif
    {
        string LilyumParatika3DSSonOdemeDurumu(string guidId);
        KoruPoliceResponseModel KoruPolicelestir(ITeklif teklif,int koruTeklifId);
    }
    public class KoruPoliceResponseModel
    {
        public string HataMesaji { get; set; }
        public bool Basarili { get; set; }
    }
}
