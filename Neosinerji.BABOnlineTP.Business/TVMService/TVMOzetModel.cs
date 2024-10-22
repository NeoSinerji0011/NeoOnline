using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMOzetModel
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
    }
     public class ParaBirimleriModel
    {
        public string Id { get; set; }
        public string Birimi { get; set; }
        public string Adi { get; set; }
        
    }
   

    public class TVMLocationModelList
    {
        public List<TVMLocationModel> List { get; set; }
    }

    public class TVMLocationModel
    {
        public int Kod { get; set; }
        public string Unvan { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Lat { get; set; }
        public string Lgn { get; set; }
    }

    public class TVMUrunYetkileriOzelModel
    {
        public int TumKodu { get; set; }
        public string TumUnvani { get; set; }
        public string IMGUrl { get; set; }
    }




}
