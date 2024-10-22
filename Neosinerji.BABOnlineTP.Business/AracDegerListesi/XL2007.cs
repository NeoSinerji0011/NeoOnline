using System;
using System.Data;
using System.Text;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Business
{
    public class XL2007 : IDisposable
    {
        const string documentRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
        const string worksheetSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        const string sharedStringsRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";
        const string sharedStringSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        string FileName;
        string Sheet;
        int SheetNumber;

        Package xlPackage = null;
        PackagePart DocumentPart = null;
        XmlNode SheetNode = null;
        XmlDocument SheetDoc = null;
        XmlDocument StringDoc = null;
        XmlNamespaceManager nsManager = null;
        Uri documentUri = null;
        NameTable nt = null;
        XmlNodeList StringNodes = null;
        XmlNodeList RowNodes = null;

        public XL2007(string file, string sheet)
        {
            FileName = file;
            Sheet = sheet;
        }

        public XL2007(string file, int sheetnumber)
        {
            FileName = file;
            SheetNumber = sheetnumber;
        }

        public bool Open()
        {
            try
            {
                xlPackage = Package.Open(FileName, FileMode.Open, FileAccess.Read);

                //  Get the main document part (workbook.xml).
                foreach (System.IO.Packaging.PackageRelationship relationship in xlPackage.GetRelationshipsByType(documentRelationshipType))
                {
                    //  There should only be one document part in the package. 
                    documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);
                    DocumentPart = xlPackage.GetPart(documentUri);

                    //  There should only be one instance, 
                    //  but get out no matter what.
                    break;
                }

                if (DocumentPart == null)
                    return false;

                XmlDocument doc = new XmlDocument();
                doc.Load(DocumentPart.GetStream());

                //  Create a namespace manager, so you can search.
                //  Add a prefix (d) for the default namespace.
                nt = new NameTable();
                nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("d", worksheetSchema);
                nsManager.AddNamespace("s", sharedStringSchema);

                string searchString = String.Empty;

                if (String.IsNullOrEmpty(Sheet) == false)
                    searchString = string.Format("//d:sheet[@name='{0}']", Sheet);
                else if (SheetNumber > 0)
                    searchString = string.Format("//d:sheet[@sheetId='{0}']", SheetNumber);

                SheetNode = doc.SelectSingleNode(searchString, nsManager);

                if (SheetNode == null)
                    return false;

                XmlAttribute relationAttribute = SheetNode.Attributes["r:id"];

                if (relationAttribute == null)
                    return false;

                string relId = relationAttribute.Value;

                //  First, get the relation between the 
                // document and the sheet.
                PackageRelationship sheetRelation = DocumentPart.GetRelationship(relId);
                Uri sheetUri = PackUriHelper.ResolvePartUri(documentUri, sheetRelation.TargetUri);
                PackagePart sheetPart = xlPackage.GetPart(sheetUri);

                //  Load the contents of the workbook.
                SheetDoc = new XmlDocument(nt);
                SheetDoc.Load(sheetPart.GetStream());

                return true;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        public List<string> GetRowContents(int row, int colCount)
        {
            List<string> contents = new List<string>();

            string address = String.Empty;

            if (colCount == -1)
                colCount = GetRowColumnCount(row);

            XmlNode node = GetRowFirstCell(row);

            if (node == null)
                return null;

            for (int i = 1; i <= colCount; i++)
            {
                contents.Add(ReadCellNodeValue(node));
                if (node.NextSibling == null)
                    break;
                node = node.NextSibling;
            }

            return contents;
        }

        public int GetRowCount(int startRow)
        {
            XmlNode dimensionNode = SheetDoc.SelectSingleNode("//d:dimension", nsManager);

            if (dimensionNode != null)
            {
                string dimref = dimensionNode.Attributes["ref"].Value;

                string[] dims = dimref.Split(new char[] { ':' });

                if (dims.Length == 2)
                {
                    string val = dims[1];
                    string result = String.Empty;
                    for (int i = 0; i < val.Length; i++)
                    {
                        if (char.IsDigit(val[i]))
                            result += val[i];
                    }

                    int resultInt = 0;
                    if (int.TryParse(result, out resultInt))
                        return resultInt - startRow;

                    return 0;
                }
            }

            return 0;
        }

        public string ReadCellNodeValue(XmlNode cellNode)
        {
            //  Retrieve the value. The value may be stored within 
            //  this element. If the "t" attribute contains "s", then
            //  the cell contains a shared string, and you must look 
            //  up the value individually.
            XmlAttribute typeAttr = cellNode.Attributes["t"];
            string cellType = string.Empty;
            if (typeAttr != null)
                cellType = typeAttr.Value;

            string cellValue = String.Empty;
            XmlNode valueNode = cellNode.SelectSingleNode("d:v", nsManager);
            if (valueNode != null)
                cellValue = valueNode.InnerText;

            if (cellType == "s")
            {
                //  Go retrieve the actual string from the associated string file.
                if (StringDoc == null)
                {
                    foreach (System.IO.Packaging.PackageRelationship stringRelationship in DocumentPart.GetRelationshipsByType(sharedStringsRelationshipType))
                    {
                        //  There should only be one shared string reference, 
                        // so you exit this loop immediately.
                        Uri sharedStringsUri = PackUriHelper.ResolvePartUri(
                          documentUri, stringRelationship.TargetUri);
                        PackagePart stringPart = xlPackage.GetPart(sharedStringsUri);
                        if (stringPart != null)
                        {

                            StringDoc = new XmlDocument(nt);
                            StringDoc.Load(stringPart.GetStream());

                            //XmlNode countNode = StringDoc.SelectSingleNode("//s:sst", nsManager);
                            //string strCount = countNode.Attributes["count"].ToString();
                            //int intCount = Convert.ToInt32(strCount);

                            StringNodes = StringDoc.SelectNodes("//s:sst/s:si", nsManager);

                            //  Add the string schema to the namespace manager:
                            nsManager.AddNamespace("s", sharedStringSchema);
                        }
                    }
                }

                if (StringNodes != null)
                {
                    int requestedString = Convert.ToInt32(cellValue);
                    return StringNodes[requestedString].InnerText;
                }
            }

            return cellValue;
        }

        public string ReadCell(string address)
        {
            XmlNode cellNode = SheetDoc.SelectSingleNode(string.Format("//d:sheetData/d:row/d:c[@r='{0}']", address), nsManager);

            if (cellNode == null)
                return String.Empty;

            return ReadCellNodeValue(cellNode);
        }

        public int GetRowColumnCount(int row)
        {
            if (RowNodes == null)
            {
                RowNodes = SheetDoc.SelectNodes("//d:sheetData/d:row", nsManager);
            }
            XmlNode rowNode = RowNodes[row];

            if (rowNode == null)
                return 0;
            else
            {
                string spans = rowNode.Attributes["spans"].Value;

                string[] values = spans.Split(new char[] { ':' });

                if (values.Length == 2)
                    return Convert.ToInt32(values[1]);
            }

            return 0;
        }

        private XmlNode GetCellNode(string address)
        {
            XmlNode cellNode = SheetDoc.SelectSingleNode(string.Format("//d:sheetData/d:row/d:c[@r='{0}']", address), nsManager);

            if (cellNode == null)
                return null;

            return cellNode;
        }

        private XmlNode GetRowFirstCell(int row)
        {
            if (RowNodes == null)
            {
                RowNodes = SheetDoc.SelectNodes("//d:sheetData/d:row", nsManager);
            }
            XmlNode cellNode = RowNodes[row];

            if (cellNode == null)
                return null;

            if (cellNode.ChildNodes == null)
                return null;

            if (cellNode.ChildNodes.Count == 0)
                return null;

            return cellNode.ChildNodes[0];
        }

        public string GetCellAddress(int Col, int Row)
        {
            if (Col > 25)
                return String.Format("{0}{1}{2}", "A", Convert.ToChar(39 + Col), Row + 1);
            return String.Format("{0}{1}", Convert.ToChar(65 + Col), Row + 1);
        }

        public bool Fill(DataTable dt)
        {
            try
            {
                int ColCount = dt.Columns.Count;
                int RowCount = 0;
                bool RowIsFull = true;

                do
                {
                    if (RowCount > 0)
                    {
                        DataRow row = dt.NewRow();

                        for (int Col = 0; Col < ColCount; Col++)
                        {
                            string address = GetCellAddress(Col, RowCount);
                            string value = ReadCell(address);

                            if (String.IsNullOrEmpty(value) == true && Col == 0)
                            {
                                RowIsFull = false;
                                break;
                            }

                            row[Col] = value;
                        }

                        if (RowIsFull)
                            dt.Rows.Add(row);
                    }
                    RowCount++;
                } while (RowIsFull);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (xlPackage != null)
            {
                xlPackage.Close();
                xlPackage = null;
                DocumentPart = null;
                SheetNode = null;
                SheetDoc = null;
                nsManager = null;
                documentUri = null;
                nt = null;
            }
        }

        #endregion
    }
}
