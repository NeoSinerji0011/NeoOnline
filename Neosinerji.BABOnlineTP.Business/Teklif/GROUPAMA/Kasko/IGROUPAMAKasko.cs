using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.GROUPAMA
{
    public interface IGROUPAMAKasko : ITeklif
    {
        List<GroupamaResponse> KazaDestekTeminatLimitleri();
        List<GroupamaResponse> YHIMSBasamakLimitleri();
    }
  
    public class GroupamaResponse
    {
        public string kodu { get; set; }
        public string aciklama { get; set; }
    }
}
