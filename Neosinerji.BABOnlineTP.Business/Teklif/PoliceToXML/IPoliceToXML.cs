using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IPoliceToXML
    {
        void SendPoliceToMuhasebe(ITeklif teklif);
        // string SendPoliceToEKobiApi(string xml);
        XElement XelementParse(ITeklif teklif, TVMDetay tvm);
        string GetTVMPartajNo(int tvmKodu, int tumKodu);
        string GetTUMVergiNumarasi(int tumKodu);
    }
}
