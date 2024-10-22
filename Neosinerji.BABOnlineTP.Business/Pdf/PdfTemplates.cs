using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business.Pdf
{
    public class PdfTemplates
    {
        public const string TEMPLATE_PATH = "/Content/templates/";
        public const string TRAFIK_KARSILASTIRMA = "trafik_karsilastirma.ptm";
        public const string TRAFIK_KARSILASTIRMA_YENİ= "trafik_karsilastirmayeni.ptm";
        public const string TRAFIK_KARSILASTIRMA_IHSAN = "trafik_karsilastirma_ihsan.ptm";
        public const string KASKO_KARSILASTIRMA = "kasko_karsilastirma.ptm";
        public const string KASKO_KARSILASTIRMA_IHSAN = "kasko_karsilastirma_ihsan.ptm";
        public const string DEMIRKREDILIHAYAT_POLICE = "kredilihayat_police.ptm";
        public const string DASK_KARSILASTIRMA = "dask_karsilastirma.ptm";
        public const string DASK_POLICE = "dask_police.ptm";
        public const string SEYAHAT_KARSILASTIRMA = "seyahat_karsilastirma.ptm";
        public const string KONUT_KARSILASTIRMA = "konut_karsilastirma.ptm";
        public const string ISYERI_KARSILASTIRMA = "Is_Yeri_Karsilastirma.ptm";

        public const string SEYAHAT_POLICE = "seyahatPolice.ptm";
        public const string TE_SABITPRIMLI = "teTeklif.ptm";
        public const string ODULLU_BIRIKIM = "odullubirikim.ptm";
        public const string PRIM_IADELI = "PrimIadeli.ptm";
        public const string PRIM_IADELI_YEDEK = "PrimIadeli_Yedek.ptm";
        public const string EGITIM = "Egitim.ptm";
        public const string ODEME_GUVENCE = "OdemeGuvence.ptm";
        public const string KORUNAN_GEECEK = "KorunanGelecek.ptm";

        public const string PRIM_IADELI2 = "PrimIadeli2.ptm";
        public const string CARI_HESAP = "carihesap-ekstre.ptm";
        public const string CARI_HESAP1 = "carihesap-ekstre1.ptm";
        public const string GELIR_GIDER = "gelir-gider.ptm";

        public const string SenticoSansDT_Regular = "SenticoSansDT-Regular.otf";
        public const string PoliceListesiDonemselRapor = "policeListesiDonemselUretimRaporu.ptm";

        public static string GetTemplate(string templateName)
        {
            string template = String.Empty;
            string templatePath = HttpContext.Current.Server.MapPath(TEMPLATE_PATH + templateName);

            using (TextReader reader = File.OpenText(templatePath))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }

        public static string GetTemplate(string serverPath, string templateName)
        {
            string template = String.Empty;
            string templatePath = serverPath + templateName;

            using (TextReader reader = File.OpenText(templatePath))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }
    }
}
