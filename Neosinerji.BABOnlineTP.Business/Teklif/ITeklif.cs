using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITeklif
    {
        void AddSigortali(int sigortaliKodu);

        void DekontPDF();

        void AddSoru(int soruKodu, string cevap);
        void AddSoru(int soruKodu, bool cevap);
        void AddSoru(int soruKodu, decimal cevap);
        void AddSoru(int soruKodu, DateTime cevap);

        string ReadSoru(int soruKodu, string defaultValue);
        string ReadSoruNullableString(int soruKodu);
        bool ReadSoru(int soruKodu, bool defaultValue);
        bool? ReadSoruNullableBool(int soruKodu);

        decimal ReadSoru(int soruKodu, decimal defaultValue);
        int? ReadSoruNullableInt(int soruKodu);

        DateTime ReadSoru(int soruKodu, DateTime defaultValue);
        DateTime? ReadSoruNullableDateTime(int soruKodu);

        void AddAracEkSoru(int tumKodu, string soruTipi, string soruKodu, string aciklama, decimal bedel, decimal fiyat);

        void AddVergi(int vergiKodu, decimal tutar);
        void AddTeminat(int teminatKodu, decimal tutar, decimal vergi, decimal netprim, decimal brutprim, int adet);
        void AddOdemePlani(int taksitNo, DateTime vade, decimal tutar, byte odemeTipi);
        void AddOdemePlaniALL(ITeklif teklif);
        void ResetOdemePlani();

        void AddWebServisCevap(int cevapKodu, string cevap);
        void AddWebServisCevap(int cevapKodu, bool cevap);
        void AddWebServisCevap(int cevapKodu, decimal cevap);
        void AddWebServisCevap(int cevapKodu, DateTime cevap);

        string ReadWebServisCevap(int cevapKodu, string defaultValue);
        bool ReadWebServisCevap(int cevapKodu, bool defaultValue);
        decimal ReadWebServisCevap(int cevapKodu, decimal defaultValue);
        DateTime ReadWebServisCevap(int cevapKodu, DateTime defaultValue);

        void AddHata(string hataMesaji);

        void Hesapla(ITeklif teklif);
        void Policelestir(Odeme odeme);
        void PolicePDF();
        Hashtable BilgilendirmeFormu(string formName);

        void BeginLog(string istek, byte istekTipi);
        void BeginLog(object request, Type type, byte istekTipi);
        void EndLog(string cevap, bool basarili);
        void EndLog(object response, bool basarili, Type type);

        bool SaveArac();
        bool SaveAracEkSoru();
        bool SaveRizikoAdresi();
        bool SaveSorular();
        bool SaveWebServisCevaplar();
        bool SaveTeminatlar();
        bool SaveVergiler();
        bool SaveOdemePlani();
        bool SaveLog();

        void Import(ITeklif teklif);

        int TeklifNo { get; set; }
        int UrunKodu { get; }
        int TUMKodu { get; }
        int OdemePlaniAlternatifKodu { get; set; }
        bool Basarili { get; }

        TeklifGenel GenelBilgiler { get; set; }
        TeklifSigortaEttiren SigortaEttiren { get; set; }
        List<TeklifSigortali> Sigortalilar { get; set; }

        TeklifArac Arac { get; set; }
        TeklifRizikoAdresi RizikoAdresi { get; set; }

        List<TeklifAracEkSoru> AracEkSorular { get; set; }
        List<TeklifSoru> Sorular { get; set; }
        List<TeklifTeminat> Teminatlar { get; set; }
        List<TeklifVergi> Vergiler { get; set; }
        List<TeklifOdemePlani> OdemePlani { get; set; }
        List<TeklifWebServisCevap> WebServisCevaplar { get; set; }

        List<WEBServisLog> Log { get; }
        List<string> Hatalar { get; }
        List<string> BilgiMesajlari { get; }
        void SetClientIPAdres(string ipadres);
        //ANADOLUKasko.cs de Client ip yi okumak için kullanılıyor.
        string SetClientIPAdress(string ipadres);
    }
}
