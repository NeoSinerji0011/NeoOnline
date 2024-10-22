using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EurekoSigorta_Business.Common.EUREKO
{
    public class EurokoPrimOdemeTalimati
    {
        private System.IO.MemoryStream mStream;

        private string ToTurkishCharSet(string text)
        {
            text = text.Replace("İ", "\u0130");
            text = text.Replace("ı", "\u0131");
            text = text.Replace("Ş", "\u015e");
            text = text.Replace("ş", "\u015f");
            text = text.Replace("Ğ", "\u011e");
            text = text.Replace("ğ", "\u011f");
            text = text.Replace("Ö", "\u00d6");
            text = text.Replace("ö", "\u00f6");
            text = text.Replace("ç", "\u00e7");
            text = text.Replace("Ç", "\u00c7");
            text = text.Replace("ü", "\u00fc");
            text = text.Replace("Ü", "\u00dc");
            return text;
        }

        public string PdfDocument(KrediKartiFormModel data)
        {
            mStream = new System.IO.MemoryStream();
            //PDF dosyasi olusturmak için Document türünden degisken tanimliyoruz.
            var pdfDosyasi = new Document();
            PdfWriter writer = PdfWriter.GetInstance(pdfDosyasi, mStream);
            pdfDosyasi.SetPageSize(PageSize.A4);
            //Dosyaya ekleme islemi yapmak için açiyoruz.
            pdfDosyasi.Open();

            string fontPath = HttpContext.Current.Server.MapPath(@"\content\Fonts\calibri.ttf");
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, "iso-8859-9", BaseFont.EMBEDDED);
            Font font = new Font(bf);
            Font baslikfont = new Font(bf);
            baslikfont.Size = 12;
            baslikfont.SetStyle(Font.BOLD);

            PdfPTable kartBilgileri = new PdfPTable(2);

            float[] lw = { 5f, 5f };
            kartBilgileri.SetWidths(lw);
            kartBilgileri.HorizontalAlignment = 0;
            kartBilgileri.SpacingBefore = 5;
            kartBilgileri.SpacingAfter = 5;
            kartBilgileri.WidthPercentage = 100f;
            kartBilgileri.DefaultCell.Border = 0;

            Paragraph pr = new Paragraph(ToTurkishCharSet(""), font);
            PdfPCell cel = new PdfPCell(pr);
            Paragraph baslik = new Paragraph("KREDİ KARTI İLE PRİM ÖDEME TALİMATI", baslikfont);
            baslik.Alignment = Element.ALIGN_CENTER;
            pdfDosyasi.Add(baslik);

            Paragraph prbosluk = new Paragraph(" ");
            pdfDosyasi.Add(prbosluk);
            cel.Colspan = 2;
            cel.Border = 0;
            cel.HorizontalAlignment = 1;
            cel.PaddingTop = 10f;
            cel.PaddingBottom = 10f;

            PdfPTable ustMetin = new PdfPTable(1);
            ustMetin.DefaultCell.Border = 0;
            cel.Border = 0;
            ustMetin.AddCell(new Phrase(ToTurkishCharSet(
                          string.Format("{0} acentesinden {1} TL karşılığında {2} nolu poliçemi teslim aldım.İlgili poliçe peşinatının ve taksitlerinin vadelerinde aşağıdaki bilgisi verilen kartımdan tahsil edilmesini rica ederim.",
                         data.AcenteAdi, data.ToplamTutar, data.PoliceNo)), font));
            ustMetin.HorizontalAlignment = 0;
            ustMetin.SpacingBefore = 5;
            ustMetin.SpacingAfter = 5;
            pdfDosyasi.Add(ustMetin);

            font.Size = 10;
            kartBilgileri.AddCell(cel);

            Paragraph prbosluk1 = new Paragraph(" ");
            pdfDosyasi.Add(prbosluk1);
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("İşlem Tarihi : {0}", data.IslemTarihi))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Kredi Kartı Türü : {0}", data.KrediKartiTuru))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Kredi Kartı No    : {0}", data.KrediKartiNo))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Son Kullanma Tarihi : {0:dd/MM/yyyy}", data.SonKullanmaTarihi))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Ödeme Şekli   : {0}", data.TaksitSayisi))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("CVV : {0}", data.CVV2))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Kimlik No : {0}", data.KimlikNo))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Ürün Adı : {0}", data.UrunAdi))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Acente No : {0}", data.AcenteNo))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet(""), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Poliçe No : {0}", data.PoliceNo))), font));
            kartBilgileri.AddCell(new Phrase(ToTurkishCharSet(""), font));
            pdfDosyasi.Add(kartBilgileri);


            PdfPTable ln3 = new PdfPTable(3);
            float[] lw1 = { 2f, 2f, 2f };

            ln3.SetWidths(lw1);
            ln3.HorizontalAlignment = 0;
            ln3.WidthPercentage = 100f;
            ln3.DefaultCell.Border = 0;
            ln3.SpacingBefore = 5;
            ln3.SpacingAfter = 5;
            Paragraph pr3 = new Paragraph(ToTurkishCharSet("ÖDEME PLANI"), baslikfont);
            PdfPCell cel2 = new PdfPCell(pr3);
            cel2.Colspan = 3;
            cel2.Border = 0;
            cel2.BackgroundColor = GrayColor.LIGHT_GRAY;
            cel2.HorizontalAlignment = 1;
            cel2.PaddingTop = 5f;
            cel2.PaddingBottom = 5f;
            ln3.AddCell(cel2);
            pdfDosyasi.Add(ln3);

            ln3 = new PdfPTable(3);
            cel2 = new PdfPCell();
            lw1 = new float[] { 2f, 2f, 2f };
            cel2.BackgroundColor = GrayColor.WHITE;
            cel2.HorizontalAlignment = Element.ALIGN_CENTER;
            cel2.Border = 0;
            cel2.HorizontalAlignment = 1;
            ln3.SetWidths(lw1);
            ln3.HorizontalAlignment = 0;
            ln3.SpacingBefore = 5;
            ln3.SpacingAfter = 5;
            ln3.WidthPercentage = 100f;
            ln3.DefaultCell.Border = 0;
            PdfPCell taksits = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", "TAKSİTLER"))), baslikfont));
            taksits.HorizontalAlignment = Element.ALIGN_CENTER;
            taksits.Border = 0;
            ln3.AddCell(taksits);

            PdfPCell tarih = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", "TARİHİ"))), baslikfont));
            tarih.HorizontalAlignment = Element.ALIGN_CENTER;
            tarih.Border = 0;
            ln3.AddCell(tarih);

            PdfPCell tutar = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", "TUTARI"))), baslikfont));
            tutar.HorizontalAlignment = Element.ALIGN_CENTER;
            tutar.Border = 0;
            ln3.AddCell(tutar);
            ln3.AddCell(cel2);


            pdfDosyasi.Add(ln3);
          
            PdfPTable lnTakBilgisi;
            if (data.OdemePlani != null)
            {
                foreach (var item in data.OdemePlani)
                {

                    lw1 = new float[] { 2f, 2f, 2f };
                    lnTakBilgisi = new PdfPTable(3);
                    lnTakBilgisi.SetWidths(lw1);
                    lnTakBilgisi.HorizontalAlignment = 1;
                    lnTakBilgisi.WidthPercentage = 100f;
                    lnTakBilgisi.DefaultCell.Border = 0;

                    PdfPCell CellTaksit = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", item.Taksitler))), font));
                    CellTaksit.HorizontalAlignment = Element.ALIGN_CENTER;
                    CellTaksit.Border = 0;
                    lnTakBilgisi.AddCell(CellTaksit);


                    PdfPCell CellTarih = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", item.Tarihi))), font));
                    CellTarih.HorizontalAlignment = Element.ALIGN_CENTER;
                    CellTarih.Border = 0;
                    lnTakBilgisi.AddCell(CellTarih);

                    PdfPCell CellTutar = new PdfPCell(new Phrase(ToTurkishCharSet((string.Format("{0}", item.Tutari))), font));
                    CellTutar.HorizontalAlignment = Element.ALIGN_CENTER;
                    CellTutar.Border = 0;
                    lnTakBilgisi.AddCell(CellTutar);
                   
                    pdfDosyasi.Add(lnTakBilgisi);

                }
            }

            PdfPTable altMetin = new PdfPTable(1);
            altMetin.DefaultCell.Border = 0;
            cel.Border = 0;
            cel.Padding = 7f;
            cel.PaddingBottom = 7f;
            altMetin.AddCell(cel);
            altMetin.AddCell(new Phrase(ToTurkishCharSet(string.Format("Mal hizmeti aldım. Yukarıda bilgisi verilen kredi kartımdan {0} TL tahsil edilmesini onaylıyorum", data.ToplamTutar)), font));
            altMetin.HorizontalAlignment = Element.ALIGN_LEFT;
            altMetin.SpacingBefore = 5;
            altMetin.SpacingAfter = 5;
            pdfDosyasi.Add(altMetin);

            font.Size = 10;

            PdfPTable lnKartSahipBilgileri = new PdfPTable(1);
            float[] lw3 = { 5f };
            lnKartSahipBilgileri.SetWidths(lw3);
            lnKartSahipBilgileri.HorizontalAlignment = 0;
            lnKartSahipBilgileri.SpacingBefore = 5;
            lnKartSahipBilgileri.SpacingAfter = 5;
            lnKartSahipBilgileri.WidthPercentage = 100f;
            lnKartSahipBilgileri.DefaultCell.Border = 0;

            Paragraph pr4 = new Paragraph(ToTurkishCharSet("KART SAHİBİNİN"), baslikfont);
            PdfPCell cel4 = new PdfPCell(pr4);
            cel4.Colspan = 2;
            cel4.Border = 0;
            cel4.BackgroundColor = GrayColor.LIGHT_GRAY;
            cel4.HorizontalAlignment = 1;
            cel4.PaddingTop = 7f;
            cel4.PaddingBottom = 7f;
            lnKartSahipBilgileri.AddCell(cel4);
            lnKartSahipBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Adı Soyadı : {0}", data.KartSahibininAdiSoyadi))), font));
            lnKartSahipBilgileri.AddCell(new Phrase(ToTurkishCharSet("İmza:"), font));
            lnKartSahipBilgileri.AddCell(new Phrase(ToTurkishCharSet((string.Format("Tarih : {0:dd/MM/yyyy}", data.Tarih))), font));
            pdfDosyasi.Add(lnKartSahipBilgileri);
            pdfDosyasi.Close();
            writer.Close();


            byte[] fileData = GetFileBytes();

            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
            string fileName = String.Format("KrediKarti_OdemeFormu_{0}.pdf", System.Guid.NewGuid().ToString("N"));
            string url = storage.UploadFile("KrediKartiOdemeFormu", fileName, fileData);

            return url;
        }

        private string ToTurkishCharSet(string p1, string p2, int p3, int p4, Font font)
        {
            throw new NotImplementedException();
        }

        public byte[] GetFileBytes()
        {
            return this.mStream.ToArray();
        }
    }
}
