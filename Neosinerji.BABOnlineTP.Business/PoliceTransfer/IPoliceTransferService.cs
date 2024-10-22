using Neosinerji.BABOnlineTP.Business.dogapolicetransfer;
using Neosinerji.BABOnlineTP.Database.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IPoliceTransferService
    {
        List<Police> getPoliceler(string sigortaSirketiKodu, string path, int tvmKodu);
        GencListeler gencPoliceler(string path);
        List<Police> getAxaPoliceler(string sigortaSirketiKodu, XmlNode policeXml, int tvmKodu);
        List<Police> getDogaPoliceler(string sigortaSirketiKodu, GeriyePoliceTransferCevap policeListesi, int tvmKodu);

        string getMessage();
        bool getTahsilatMi();
        void setMessage(string mesaj);

        List<Police> getAutoPoliceler(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "", string partajNo = "");
        List<AutoPoliceTransferProcedureModel> AutoPoliceTransferGetir(DateTime TanzimBaslangicTarih, DateTime TanzimBitisTarih, string SigortaSirketleriListe, int tvmKodu);
        AutoPoliceTransfer AutoPoliceTransferGetir(DateTime TanzimBaslangicTarih, string SigortaSirketKodu, int tvmKodu);

        string getAutoTahsilatPoliceKapatma(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi);
        PoliceModel getOtoOnayPoliceler(string policeNumarasi, string sigortaSirketNumarasi);

        PoliceTransferReaderKullanicilari GetPoliceReaderKullanicilari(string readerKullanciKodu);

    }
    public class PoliceModel
    {
        public List<PoliceGenel> policeler { get; set; }
        public string BilgiMesaji { get; set; }
    }
}
