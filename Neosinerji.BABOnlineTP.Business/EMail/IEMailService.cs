using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IEMailService
    {
        void SendYeniKullaniciEMail(TVMKullanicilar kullanici, string password, string projeKodu);
        bool SendYeniKullaniciEMail_Update(TVMKullanicilar kullanici, string password, string projeKodu);

        void SendSigortaliYeniKullaniciEMail(TVMKullanicilar kullanici, string password);

        void SendSifreYenileEMail(TVMKullanicilar kullanici, string password);
        bool SendSifreYenileEMail_Update(TVMKullanicilar kullanici, string password, string projeKodu);

        void SendSifreYenileLink(TVMKullanicilar kullanici, string link);
        bool SendSifreYenileLink_Update(TVMKullanicilar kullanici, string link, string projeKodu);

        bool GenelUrunMailGonder(ITeklifBase teklifBase, string digerAdSoyad, string digerEmail);

        void SendTrafikTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);
        void SendKaskoTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);
        void SendDaskTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);
        void SendKrediHayat(IKrediliHayatTeklif teklif, string digerAdSoyad, string digerEmail);
        void SendIkinciElGarantiTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);

        void SendSeyahatSaglikTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);
        void SendKonutTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);

        void SendIsYeriTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail);

        bool SendAegonEMailTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail, bool acenteyeMI);

        void SendMetlifeEmail(int KullaniciGrupKodu, int teklifId, int asama);        

        bool SendEMail(string formatName, Hashtable data, Dictionary<string, string> to);
        bool SendOnlyEmail(EPostaFormatlari format, Hashtable data, Dictionary<string, string> to, string projeKodu);
        bool SendEMailPDF(ITeklif teklif, EPostaFormatlari format, Hashtable data, Dictionary<string, string> to, List<FileAtachmentList> attachments);
        bool SendHaritaIletisimForm(int TVMKodu, string AdSoyad, string Email, string Tel);

        void SendLilyumYeniKullaniciEMail(TVMKullanicilar kullanici, string password);
        void SendLilyumBilgilendirme(string adSoyad, string referansNo, string yapilanOdeme, string OdemeDurumu, string email, string odemeTutari);
    }
}
