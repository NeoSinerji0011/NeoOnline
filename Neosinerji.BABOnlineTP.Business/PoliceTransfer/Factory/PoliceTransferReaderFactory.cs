using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public static class PoliceTransferReaderFactory
    {
        public static IPoliceTransferReader createReader(string birlikKodu, string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            //if (birlikKodu == "000")
            //{
            //    return new GencExcelReader(path, tvmKodu,branslar);
            //}

            switch (birlikKodu)
            {
                case SigortaSirketiBirlikKodlari.ANADOLUSIGORTA:
                    return new AnadoluXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.AXASIGORTA:
                    return new AxaXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA:
                    return new MapfreXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.MAPFREGENELYASAM:
                    return new MapfreSeyahatSaglikXmlRader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.SOMPOJAPANSIGORTA:
                    return new SompoExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.MAGDEBURGERSIGORTA:
                    return new MagdeExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.QUICKSIGORTA:
                    return new QuickExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.MAPFREDASK:
                    return new MapfreDaskXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.RAYSIGORTA:
                    return new RayExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.HDISIGORTA:
                    return new HdiXMLReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.EUREKOSIGORTA:
                    return new EurekoXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.GUNESSIGORTA:
                    return new GunesXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ETHICA:
                    return new EthicaXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.HALKSIGORTA:
                    return new HalkXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.TURKIYESIGORTA:
                    return new TurkiyeXMLReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.BASAKGROUPAMASIGORTA:
                    return new BasakXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.SBNSIGORTA:
                    return new SBNXMLReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ERGOISVICRESIGORTA:
                    return new ERGOXMLReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ALLIANZSIGORTA:
                    return new AllianzXMLReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ALLIANZHAYATVEEMEKLILIK:
                    return new AllianzHayatveEmeklilikReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.GULFSIGORTA:
                    return new AigExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA:
                    return new NipponExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.AKSIGORTA:
                    return new AkExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ORIENTSIGORTA:
                    return new OrientExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.TURKLANDSIGORTA:
                    return new TurklandExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.GENERALISIGORTA:
                    return new GeneraliXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.DUBAIGROUPSIGORTA:
                    return new DubaiXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ZURICHSIGORTA:
                    return new ZurichXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.BEREKET:
                    return new BereketExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.NEOVASIGORTA:
                    return new NeovaXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.LIBERTYSIGORTA:
                    return new LibertyExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.EGESIGORTA:
                    return new EgeXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.AVIVASIGORTA:
                    return new AvivaExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.KORUSIGORTA:
                    return new KoruXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.ANKARASIGORTA:
                    return new AnkaraExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.SSDOGASİGORTAKOOPERATİF:
                    return new DogaXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.GRISIGORTA:
                    return new GriXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.FIBAEMEKLILIK:
                    return new FibaEmeklilikExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.AVEONGLOBAL:
                    return new AveonGlobalXmlReader(path, tvmKodu, SigortaSirketiBransList, branslar);
                case SigortaSirketiBirlikKodlari.HEPIYISIGORTA:
                    return new HepIyiExcelReader(path, tvmKodu, SigortaSirketiBransList, branslar);  
                case SigortaSirketiBirlikKodlari.ACNTURKSIGORTA:
                    return new AcnTurkExcelReader2(path, tvmKodu, SigortaSirketiBransList, branslar);
                default:
                    return null;
            }

        }

        public static IGencPoliceTransferReader createReaderGenc(string birlikKodu, string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            if (birlikKodu == "000")
            {
                return new GencExcelReader(path, tvmKodu, branslar);
            }


            return null;
        }
    }
}
