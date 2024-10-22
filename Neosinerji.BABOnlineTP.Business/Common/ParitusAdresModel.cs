using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class ParitusAdresModel
    {
        public byte Durum { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public string BeldeKodu { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string DaireNo { get; set; }
        public string BinaNo { get; set; }
        public string PostaKodu { get; set; }
        public string IsMerkezi { get; set; }
        public string Blok { get; set; }
        public string Kat { get; set; }
        public string FullAdres { get; set; }
        public string VerificationScore { get; set; }
        public string Undefined { get; set; }
        public string VerificationType { get; set; }

        public string uavtStreetCode { get; set; }
        public string uavtBuildingCode { get; set; }
        public string uavtAddressCode { get; set; }

        public string Ada { get; set; }
        public string Pafta { get; set; }
        public string Parsel { get; set; }

        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public List<SelectListItem> Beldeler { get; set; }
        public List<SelectListItem> Mahalleler { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public List<string> CokluAdres { get; set; }
    }

    public class ParitusAdresSorgulamaDurum
    {

        public const byte Basarisiz = 0;
        public const byte YanlizIlIlce = 1;
        public const byte TekliAdes = 2;
        public const byte CokluAdres = 3;
    }
}
