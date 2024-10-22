using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{
    public class AcenteEntity
    {
        public Int32 AcenteKodu { get; set; }
        public String AcenteVergiNo { get; set; }
        public String AcenteLevhaNo { get; set; }
        public String Unvan { get; set; }
        public String TelefonNo { get; set; }
        public String PartajNo { get; set; }
        public String AdresIl { get; set; }
        public String AdresIlce { get; set; }
        public String Adres { get; set; }
        //public String SigortaSirketKodu { get; set; }
        //public String SigortaSirketiVergiNumarasi { get; set; }
        //public String SigortaSirketiUnvani { get; set; }

        public AcenteEntity()
        {

        }

        public AcenteEntity(TVMDetay tvmDetay)
 
        //public AcenteEntity(TVMDetay tvmDetay, TUMDetay tumDetay)
        {
            
            //this.AcenteKodu
            //this.AcenteLevhaNo
            if (tvmDetay != null)
            {
                this.AcenteKodu = tvmDetay.Kodu;
                this.AcenteVergiNo = tvmDetay.VergiNumarasi;
                this.Adres = tvmDetay.Adres;
                this.AdresIl = tvmDetay.IlKodu;
                if (tvmDetay.IlceKodu != null)
                {
                    this.AdresIlce = ((Int32)tvmDetay.IlceKodu).ToString();
                }
                this.TelefonNo = tvmDetay.Telefon;
                this.Unvan = tvmDetay.Unvani;
            }
            
            //this.PartajNo

            /*
            if (tumDetay != null)
            {
                this.SigortaSirketiUnvani = tumDetay.Unvani;
                this.SigortaSirketiVergiNumarasi = tumDetay.VergiNumarasi;
                this.SigortaSirketKodu = tumDetay.BirlikKodu;
            }
            */
           
            
        }
    }

    
}
