using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITeklifBase
    {
        List<int> UretimMerkezleri { get; }
        List<int> OdemePlaniKodlari { get; }
        ITeklif Teklif { get; set; }
        List<ITeklif> TUMTeklifler { get; set; }

        IsDurum Hesapla(ITeklif teklif);
        void Policelestir(ITeklif teklif, Odeme odeme);
        void CreatePDF();
        void EPostaGonder(string DigerAdSoyad, string DigerEmail);

        void AddUretimMerkezi(int tumKodu);
        void AddOdemePlani(int odemePlaniAlternatifKodu);
        void AddTeklif(ITeklif teklif);
        void TSSEPostaGonder(string teklifPDF, string DigerAdSoyad, string DigerEmail);
        IsDurum GetIsDurum();
    }
}
