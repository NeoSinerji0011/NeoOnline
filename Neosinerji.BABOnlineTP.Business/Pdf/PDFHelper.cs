using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Neosinerji.BABOnlineTP.Business.Pdf
{
    public class PDFHelper : IDisposable
    {
        private System.IO.MemoryStream mStream;
        private Document mMainDocument;
        private Document mDocument;
        private PdfWriter mPdfWriter;
        private BaseFont mCalibriBase;
        private Font mFontNormal;
        private Font mFontBold;
        private PdfPTable mTable;
        private float mRowHeight;
        private Color mBorderColor;
        private Color mBackgroundColor;
        private Font mCurrentFont;
        private Phrase mPhrase;
        private int mFontSize;
        private int mFontType;
        private bool mFontChanged;
        private Color mFontColor;
        private int mRotate;

        public PDFHelper(string author, string subject, string title, int fontSize, string fontsPath, string fontName = null)
        {
            mStream = new System.IO.MemoryStream();
            mMainDocument = new Document(PageSize.A4);

            mPdfWriter = PdfWriter.GetInstance(mMainDocument, mStream);
            mPdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
            mPdfWriter.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);

            if (String.IsNullOrEmpty(fontName))
                fontName = "calibri.ttf";

            string fontPath = fontsPath + fontName; // "SenticoSansDT-Regular.otf";// "verdana.ttf"; //"calibri.ttf";
            mCalibriBase = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            mFontNormal = new Font(mCalibriBase, fontSize, Font.NORMAL);
            mFontBold = new Font(mCalibriBase, fontSize, Font.NORMAL);
            mFontSize = fontSize;
            mFontType = Font.NORMAL;
            mFontChanged = true;
            mFontColor = Color.BLACK;

            mMainDocument.Open();
            mDocument = mMainDocument;

            mDocument.AddAuthor(author);
            mDocument.AddSubject(subject);
            mDocument.AddTitle(title);

            SetCurrentFont(Font.NORMAL);
        }

        public void AddPage()
        {
            mDocument.SetPageSize(iTextSharp.text.PageSize.A4);
            mDocument.NewPage();
        }

        public void Rotate()
        {
            mDocument.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            mDocument.NewPage();
        }

        public void SetPageEventHelper(PdfPageEventHelper eventHelper)
        {
            mPdfWriter.PageEvent = eventHelper;
        }

        public void SetActiveDocument(Document document)
        {
            mDocument = document;
        }

        public void SetFontSize(int fontSize)
        {
            mFontSize = fontSize;
            mFontChanged = true;
        }

        public void SetFontType(int fontType)
        {
            mFontType = fontType;
            mFontChanged = true;
        }

        public void SetFontColor(Color color)
        {
            mFontColor = color;
            mFontChanged = true;
        }

        public void SetFont()
        {
            if (mFontChanged)
            {
                mCurrentFont = new Font(mCalibriBase, mFontSize, mFontType, mFontColor);
                mFontChanged = false;
            }
        }

        public void SetCurrentFont(int fontType)
        {
            if (fontType == Font.NORMAL)
                mCurrentFont = mFontNormal;
            else
                mCurrentFont = mFontBold;
        }

        public void SetCurrentFont(Font font)
        {
            mCurrentFont = font;
        }

        public void SetRowHeight(float height)
        {
            mRowHeight = height;
        }

        public void SetBorderColor(Color borderColor)
        {
            mBorderColor = borderColor;
        }

        public void SetBackgroundColor(Color backgroundColor)
        {
            mBackgroundColor = backgroundColor;
        }

        public void AddImage(string path)
        {
            float clientWidth = mDocument.PageSize.Width - mDocument.LeftMargin - mDocument.RightMargin;
            Image img = Image.GetInstance(path);
            img.ScaleToFit(clientWidth, mDocument.PageSize.Height);
            mDocument.Add(img);
        }

        public void AddImage(string path, PdfWriter writer)
        {

            float clientWidth = mDocument.PageSize.Width - mDocument.LeftMargin - mDocument.RightMargin;
            Image img = Image.GetInstance(path);
            img.ScaleToFit(clientWidth, mDocument.PageSize.Height);
            writer.Add(img);
        }

        public void AddImage(string path, int width, int height, int alignment)
        {
            if (!String.IsNullOrEmpty(path))
            {
                Image img = Image.GetInstance(path);
                img.ScaleToFit(width, height);
                img.Alignment = alignment;
                mDocument.Add(img);
            }
        }

        //Custom Add
        public void SetRotate(int rotate)
        {
            mRotate = rotate;
        }

        public void AddParagraph(string text, int alignment, int spacingBefore)
        {
            text = ToTurkishCharSet(text);
            Paragraph p1 = new Paragraph(text, mCurrentFont);
            p1.Alignment = alignment;
            p1.SpacingBefore = spacingBefore;
            p1.SetLeading(mCurrentFont.GetCalculatedLeading(0.2f), 0.9f);
            mDocument.Add(p1);
        }

        public void BeginParagraph()
        {
            mPhrase = new Phrase();
        }

        public void AddToParagraph(string text)
        {
            text = ToTurkishCharSet(text);
            Chunk c = new Chunk(text, mCurrentFont);

            if (mBackgroundColor != null)
            {
                c.SetBackground(mBackgroundColor);
            }

            mPhrase.Add(c);
        }

        public void EndParagraph(int horizontalAlignment, int spacingBefore, int spacingAfter)
        {
            if (mPhrase.Count == 0)
            {
                AddToParagraph(" ");
            }

            Paragraph p1 = new Paragraph(mPhrase);
            p1.Alignment = horizontalAlignment;
            p1.SpacingBefore = spacingBefore;
            p1.SpacingAfter = spacingAfter;
            p1.SetLeading(mCurrentFont.GetCalculatedLeading(0.2f), 0.9f);
            mDocument.Add(p1);
            mPhrase = null;
        }

        public void BeginTable(float totalWidth, int spacingBefore, float[] widths)
        {
            mTable = new PdfPTable(widths.Length);
            mTable.SpacingBefore = spacingBefore;
            mTable.TotalWidth = totalWidth;
            mTable.LockedWidth = true;
            mTable.HorizontalAlignment = 0;
            mTable.SetWidths(widths);
        }

        public void EndTable()
        {
            mDocument.Add(mTable);
        }

        public void EndTable(PdfWriter writer, float ypos)
        {
            mTable.WriteSelectedRows(0, -1, mMainDocument.LeftMargin, ypos, writer.DirectContent);
            mTable.FlushContent();
        }

        public void AddRow(string[] cells, int alignment)
        {
            foreach (string celltxt in cells)
            {
                AddCell(celltxt, alignment);
            }
        }

        public void AddCell(string celltxt, int horizontalAlignment)
        {
            AddCell(celltxt, horizontalAlignment, Element.ALIGN_MIDDLE, 0);
        }

        public void AddCell(Image img, float width, float height, int alignment, int colspan)
        {
            img.ScaleToFit(width, height);
            PdfPCell cell = new PdfPCell(img);
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.PaddingTop = 2;
            cell.PaddingBottom = 2;

            if (mBackgroundColor != null)
                cell.BackgroundColor = mBackgroundColor;

            if (colspan > 0)
                cell.Colspan = colspan;

            if (mBorderColor != null)
            {
                cell.BorderColorTop = mBorderColor;
                cell.BorderColorBottom = mBorderColor;
                cell.BorderColorLeft = mBorderColor;
                cell.BorderColorRight = mBorderColor;
                cell.BorderWidth = 1;
            }
            else
                cell.Border = 0;

            mTable.AddCell(cell);
        }

        public void AddCell(string celltxt, int horizontalAlignment, int verticalAlignment)
        {
            AddCell(celltxt, horizontalAlignment, verticalAlignment, 0);
        }

        public void AddCellWithColSpan(string celltxt, int horizontalAlignment, int colSpan)
        {
            AddCell(celltxt, horizontalAlignment, Element.ALIGN_MIDDLE, colSpan);
        }

        public void AddCellWithColSpan(Image img, int horizontalAlignment, int colSpan)
        {
            PdfPCell cell = new PdfPCell(img, true);
            cell.HorizontalAlignment = horizontalAlignment;
            cell.Colspan = colSpan;

            if (mBackgroundColor != null)
                cell.BackgroundColor = mBackgroundColor;

            if (mBorderColor != null)
            {
                cell.BorderColorTop = mBorderColor;
                cell.BorderColorBottom = mBorderColor;
                cell.BorderColorLeft = mBorderColor;
                cell.BorderColorRight = mBorderColor;
                cell.BorderWidth = 1;
            }
            else
                cell.Border = 0;

            if (mRowHeight > 0)
                cell.FixedHeight = mRowHeight;

            mTable.AddCell(cell);
        }

        public void AddCellLinkWithColSpan(string text, string referance, Font font, int horizontalAlignment, int colSpan)
        {
            Anchor anc = CreateLink(text, referance, font);

            PdfPCell cell = new PdfPCell();
            Phrase p = new Phrase();
            p.Add(anc);
            cell.Phrase = p;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.Colspan = colSpan;

            if (mBackgroundColor != null)
                cell.BackgroundColor = mBackgroundColor;

            if (mBorderColor != null)
            {
                cell.BorderColorTop = mBorderColor;
                cell.BorderColorBottom = mBorderColor;
                cell.BorderColorLeft = mBorderColor;
                cell.BorderColorRight = mBorderColor;
                cell.BorderWidth = 1;
            }
            else
                cell.Border = 0;

            if (mRowHeight > 0)
                cell.FixedHeight = mRowHeight;
            mTable.AddCell(cell);
        }

        public void AddRowImageLinkWithColSpan(List<Image> images, int horizontalAlignment, int columns)
        {
            float[] widths = new float[] { 16f, 16f, 16f };
            PdfPTable table = new PdfPTable(columns);

            List<PdfPCell> cells = new List<PdfPCell>();

            foreach (Image img in images)
            {
                PdfPCell cell = new PdfPCell(img);
                cell.BorderWidth = 0;
                cells.Add(cell);
            }

            PdfPRow row = new PdfPRow(new PdfPCell[] { cells[0], cells[1], cells[2] });
            row.SetWidths(widths);
            table.Rows.Add(row);

            mTable.AddCell(table);
        }

        private void AddCell(string celltxt, int horizontalAlignment, int verticalAlignment, int colSpan)
        {
            string text = ToTurkishCharSet(celltxt);

            PdfPCell cell = new PdfPCell(new Phrase(text, mCurrentFont));
            cell.HorizontalAlignment = horizontalAlignment;
            cell.VerticalAlignment = verticalAlignment;
            if (colSpan > 0)
                cell.Colspan = colSpan;

            if (mBackgroundColor != null)
                cell.BackgroundColor = mBackgroundColor;

            if (mBorderColor != null)
            {
                cell.BorderColorTop = mBorderColor;
                cell.BorderColorBottom = mBorderColor;
                cell.BorderColorLeft = mBorderColor;
                cell.BorderColorRight = mBorderColor;
                cell.BorderWidth = 1;
            }
            else
                cell.Border = 0;

            if (mRowHeight > 0)
                cell.FixedHeight = mRowHeight;

            if (mRotate != 0)
                cell.Rotation = mRotate;

            mTable.AddCell(cell);
        }

        public void Close()
        {
            try
            {
                if (mDocument.IsOpen())
                    mDocument.Close();
            }
            catch (Exception)
            {

            }
        }

        public BaseFont BaseFont
        {
            get
            {
                return mCalibriBase;
            }
        }

        public Document MainDocument
        {
            get
            {
                return mMainDocument;
            }
        }

        public byte[] GetFileBytes()
        {
            return this.mStream.ToArray();
        }

        public void WriteToFile(string fileName)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                byte[] arr = mStream.ToArray();
                fs.Write(arr, 0, arr.Length);
            }
        }

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

        public Anchor CreateLink(string text, string referance, Font font)
        {
            Anchor anchor = new Anchor(text, font);
            anchor.Reference = referance;

            return anchor;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
        }

        #endregion
    }

    public class PDFCustomEventHelper : PdfPageEventHelper
    {
        // ==== Bu method her bir pdf sayfasına footer ekliyor. Poliçelerde Kullanılmaz ==== //
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            //string root = System.Web.HttpContext.Current.Server.MapPath("/");
            //Image img = Image.GetInstance(root + "Content/img/FooterLeft.png");
            //img.ScaleToFit(120, 40);
            //img.Alignment = Image.UNDERLYING;
            //img.SetAbsolutePosition(0, 0);
            //document.Add(img);

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfContentByte cb = writer.DirectContent;
            PdfTemplate template = cb.CreateTemplate(50, 50);
            Rectangle pageSize = document.PageSize;

            String text = writer.PageNumber.ToString();
            float len = bf.GetWidthPoint(text, 10);
            cb.SetRGBColorFill(100, 100, 100);

            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, pageSize.GetRight(300), pageSize.GetBottom(30), 0);

            text = "www.neoonline.com.tr";

            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, pageSize.GetLeft(20), pageSize.GetBottom(30), 0);

            text = "© Neosinerji Bilgi Teknolojileri A.S";

            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, text, pageSize.GetRight(20), pageSize.GetBottom(30), 0);
            cb.EndText();

            //img = Image.GetInstance(root + "Content/img/neosinerji_logo.png");
            //img.ScaleToFit(64, 64);
            //img.Alignment = Image.UNDERLYING;
            //img.SetAbsolutePosition(490, 0);
            //document.Add(img);
        }
    }

    public class PDFCustomEventHelperAEGON : PdfPageEventHelper
    {
        private string surumno;
        public PDFCustomEventHelperAEGON()
        {

        }

        public PDFCustomEventHelperAEGON(string srmno)
        {
            this.surumno = srmno;
        }

        // ==== Bu method her bir pdf sayfasına footer ekliyor. Poliçelerde Kullanılmaz ==== //
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfContentByte cb = writer.DirectContent;
            PdfTemplate template = cb.CreateTemplate(50, 50);
            Rectangle pageSize = document.PageSize;

            String text = writer.PageNumber.ToString();
            float len = bf.GetWidthPoint(text, 10);
            cb.SetRGBColorFill(100, 100, 100);

            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, (pageSize.Width / 2), pageSize.GetBottom(30), 0);

            text = "Sürüm No: " + this.surumno;
            cb.SetFontAndSize(bf, 6);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, pageSize.GetLeft(20), pageSize.GetBottom(30), 90);

            cb.EndText();
        }

        public void SetSurumNo(string srmNo)
        {
            this.surumno = srmNo;
        }
    }
}
