using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Neosinerji.BABOnlineTP.Business.Pdf
{
    public class PDFParser
    {
        private PDFHelper _pdf;
        private string _content;
        private int _currentPosition;
        private PDFTag _currentTag;
        private string _activeChunk;
        private string _activeProperty;
        private string _activePropertyValue;

        int rotare = 0;

        public PDFParser(string content, PDFHelper pdfHelper)
        {
            _content = content;
            _pdf = pdfHelper;
            _currentPosition = 0;
            _currentTag = null;
            _activeChunk = String.Empty;
            _activeProperty = String.Empty;
        }

        public void Parse()
        {
            this.ClearComments();

            while (MoveNext())
            {
                switch (_currentTag.Tag)
                {
                    case "p": PrintParagraph(); break;
                    case "img": PrintImage(); break;
                    case "table": PrintTable(); break;
                    case "pagebreak": PrintPagebreak(); break;
                    case "rotate": Rotate(); break;
                }
            }
        }

        private bool MoveNext()
        {
            if (_currentPosition < _content.Length)
            {
                int startTagBegin = _content.IndexOf('<', _currentPosition);

                if (startTagBegin == -1)
                    return false;

                int startTagEnd = _content.IndexOf('>', startTagBegin);
                if (startTagEnd == -1)
                    return false;

                string tag = _content.Substring(startTagBegin + 1, (startTagEnd - startTagBegin) - 1);
                string endTag = String.Format("</{0}>", tag);

                int endTagStart = _content.IndexOf(endTag, startTagEnd);
                if (endTagStart == -1)
                    return false;

                int endTagEnd = endTagStart + endTag.Length;

                if (_currentTag == null)
                    _currentTag = new PDFTag();

                _currentTag.Tag = tag;
                _currentTag.StartIndex = startTagEnd + 1;
                _currentTag.EndIndex = endTagStart - 1;
                _currentTag.CurrentIndex = _currentTag.StartIndex;

                _currentPosition = endTagEnd;

                return true;
            }

            return false;
        }

        public string GetTemplate(string name)
        {
            string template = String.Empty;
            string templateName = String.Format("<template={0}>", name);

            int templateStart = _content.IndexOf(templateName);
            if (templateStart > -1)
            {
                string templateEndName = String.Format("</template={0}>", name);
                int templateEnd = _content.IndexOf(templateEndName, templateStart);

                if (templateEnd > -1)
                {
                    int length = templateEnd - (templateStart + templateName.Length + 1);
                    template = _content.Substring(templateStart + templateName.Length + 1, length);
                }
            }

            return template;
        }

        public void AppendToPlaceHolder(string name, string template)
        {
            string placeHolderName = String.Format("<placeHolder={0}>", name);

            int placeHolderStart = _content.IndexOf(placeHolderName);

            if (placeHolderStart > -1)
            {
                _content = _content.Insert(placeHolderStart - 1, template);
            }
        }

        public void ReplacePlaceHolder(string name, string template)
        {
            string placeHolderName = String.Format("<placeHolder={0}></placeHolder>", name);

            _content = _content.Replace(placeHolderName, template);
        }

        public void SetColumns(string name, int[] columns)
        {
            string placeHolderName = String.Format("<placeHolder={0}>", name);
            int placeHolderStart = _content.IndexOf(placeHolderName);

            if (placeHolderStart > -1)
            {
                StringBuilder sb = new StringBuilder();

                foreach (int width in columns)
                {
                    sb.AppendFormat("<column={0}>", width);
                }

                ReplacePlaceHolder(name, sb.ToString());
            }
        }

        public void SetColumnValues(string name, string template)
        {
            int index = 0;
            int columnIndex = template.IndexOf("<td>", 0);

            while (columnIndex > -1)
            {
                int endColumnIndex = template.IndexOf("</td>", columnIndex);

                if (endColumnIndex > -1)
                {
                    endColumnIndex += 5;

                    string td = template.Substring(columnIndex, endColumnIndex - columnIndex);
                    string placeHolder = String.Format("{0}{1}", name, index);
                    AppendToPlaceHolder(placeHolder, td);
                }

                columnIndex = template.IndexOf("<td>", endColumnIndex);
                index++;
            }
        }

        public void SetVariable(string name, string value)
        {
            _content = _content.Replace(name, value);
        }

        private void PrintParagraph()
        {
            int alignment = Element.ALIGN_JUSTIFIED;
            int spacingBefore = 0;
            int spacingAfter = 0;
            _pdf.BeginParagraph();

            while (MoveInTag())
            {
                if (!String.IsNullOrEmpty(_activeProperty) && _activeChunk == "#")
                {
                    switch (_activeProperty)
                    {
                        case "spacingBefore": spacingBefore = Convert.ToInt32(_activePropertyValue); break;
                        case "spacingAfter": spacingAfter = Convert.ToInt32(_activePropertyValue); break;
                        case "justified": alignment = Element.ALIGN_JUSTIFIED; break;
                        case "left": alignment = Element.ALIGN_LEFT; break;
                        case "center": alignment = Element.ALIGN_CENTER; break;
                        case "right": alignment = Element.ALIGN_RIGHT; break;
                        case "color":
                            {
                                Color color = GetColorFromText(_activePropertyValue);
                                _pdf.SetFontColor(color);
                            }
                            break;
                        case "backgroundColor":
                            {
                                if (_activePropertyValue == "clear")
                                {
                                    _pdf.SetBackgroundColor(null);
                                }
                                else
                                {
                                    Color color = GetColorFromText(_activePropertyValue);
                                    _pdf.SetBackgroundColor(color);
                                }
                            }
                            break;
                        case "strong": _pdf.SetFontType(Font.BOLD); break;
                        case "normal": _pdf.SetFontType(Font.NORMAL); break;
                        case "italic": _pdf.SetFontType(Font.ITALIC); break;
                        case "underline": _pdf.SetFontType(Font.UNDERLINE); break;
                        case "size":
                            {
                                if (!String.IsNullOrEmpty(_activePropertyValue))
                                {
                                    int fontSize = Convert.ToInt32(_activePropertyValue);
                                    _pdf.SetFontSize(fontSize);
                                }
                            }
                            break;
                        case "space":
                            {
                                if (!String.IsNullOrEmpty(_activePropertyValue))
                                {
                                    int count = Convert.ToInt32(_activePropertyValue);

                                    _pdf.SetFont();
                                    for (int i = 0; i < count; i++)
                                    {
                                        _pdf.AddToParagraph(" ");
                                    }
                                }
                                else
                                {
                                    _pdf.AddToParagraph(" ");
                                }
                            }
                            break;
                    }
                }
                else
                {
                    _pdf.SetFont();
                    _pdf.AddToParagraph(_activeChunk);
                }
            }

            _pdf.EndParagraph(alignment, spacingBefore, spacingAfter);
        }

        private void PrintImage()
        {
            string path = String.Empty;
            int width = 0;
            int height = 0;
            int alignment = Element.ALIGN_LEFT;

            while (MoveInTag())
            {
                if (!String.IsNullOrEmpty(_activeProperty) && _activeChunk == "#")
                {
                    switch (_activeProperty)
                    {
                        case "path": path = _activePropertyValue; break;
                        case "width": width = Convert.ToInt32(_activePropertyValue); break;
                        case "height": height = Convert.ToInt32(_activePropertyValue); break;
                        case "left": alignment = Element.ALIGN_LEFT; break;
                        case "center": alignment = Element.ALIGN_CENTER; break;
                        case "right": alignment = Element.ALIGN_RIGHT; break;
                    }
                }
            }

            _pdf.AddImage(path, width, height, alignment);
        }

        private void PrintTable()
        {
            int tableWidth = 0;
            List<float> columnWidths = new List<float>();
            bool rowBegin = false;
            bool cellBegin = false;
            bool cellEmpty = false;
            int alignment = Element.ALIGN_LEFT;
            int colspan = 0;
            bool image = false;
            string imagePath = String.Empty;
            int imageWidth = 0;
            int imageHeight = 0;

            while (MoveInTag())
            {
                if (!String.IsNullOrEmpty(_activeProperty) && _activeChunk == "#")
                {
                    switch (_activeProperty)
                    {
                        case "width":
                            {
                                if (image)
                                {
                                    imageWidth = Convert.ToInt32(_activePropertyValue);
                                }
                                else
                                {
                                    tableWidth = Convert.ToInt32(_activePropertyValue);
                                }
                            }
                            break;
                        case "column":
                            {
                                float w = 0;
                                float.TryParse(_activePropertyValue, out w);
                                columnWidths.Add(w);
                            }
                            break;
                        case "border":
                            {
                                Color borderColor = GetColorFromText(_activePropertyValue);
                                if (_activePropertyValue == "clear")
                                    _pdf.SetBorderColor(null);
                                else
                                    _pdf.SetBorderColor(borderColor);
                            }
                            break;
                        //case "tbody": _pdf.BeginTable(tableWidth, 35, columnWidths.ToArray()); break;
                        case "tbody": _pdf.BeginTable(tableWidth, 5, columnWidths.ToArray()); break;
                        case "tr": rowBegin = true; break;
                        case "/tr": rowBegin = false; break;
                        case "height":
                            {
                                float h = 10;
                                float.TryParse(_activePropertyValue, out h);

                                if (rowBegin && !image)
                                {
                                    _pdf.SetRowHeight(h);
                                }
                                else
                                {
                                    imageHeight = (int)h;
                                }
                            }
                            break;
                        case "td":
                            {
                                cellBegin = true;
                                cellEmpty = true;
                                colspan = 0;
                            }
                            break;
                        case "/td":
                            {
                                cellBegin = false;
                                if (cellEmpty)
                                {
                                    _pdf.SetFont();
                                    _pdf.AddCell("", alignment);
                                }
                                cellEmpty = false;
                            }
                            break;
                        case "left": alignment = Element.ALIGN_LEFT; break;
                        case "center": alignment = Element.ALIGN_CENTER; break;
                        case "right": alignment = Element.ALIGN_RIGHT; break;
                        case "color":
                            {
                                Color color = GetColorFromText(_activePropertyValue);
                                _pdf.SetFontColor(color);
                            }
                            break;
                        case "backgroundColor":
                            {
                                if (_activePropertyValue == "clear")
                                {
                                    _pdf.SetBackgroundColor(null);
                                }
                                else
                                {
                                    Color color = GetColorFromText(_activePropertyValue);
                                    _pdf.SetBackgroundColor(color);
                                }
                            }
                            break;
                        case "strong": _pdf.SetFontType(Font.BOLD); break;
                        case "normal": _pdf.SetFontType(Font.NORMAL); break;
                        case "italic": _pdf.SetFontType(Font.ITALIC); break;
                        case "underline": _pdf.SetFontType(Font.UNDERLINE); break;
                        case "size":
                            {
                                if (!String.IsNullOrEmpty(_activePropertyValue))
                                {
                                    int fontSize = Convert.ToInt32(_activePropertyValue);
                                    _pdf.SetFontSize(fontSize);
                                }
                            }
                            break;
                        case "colspan":
                            {
                                colspan = Convert.ToInt32(_activePropertyValue);
                            }
                            break;
                        case "img": image = true; break;
                        case "/img":
                            {
                                Image img = Image.GetInstance(imagePath);
                                _pdf.AddCell(img, imageWidth, imageHeight, alignment, colspan);
                                image = false;
                                imagePath = String.Empty;
                                cellEmpty = false;
                            }
                            break;
                        case "path":
                            {
                                imagePath = _activePropertyValue;
                            }
                            break;
                        case "rotate":
                            {
                                rotare = Convert.ToInt32(_activePropertyValue);
                                _pdf.SetRotate(rotare);
                            }
                            break;
                    }
                }
                else
                {
                    if (cellBegin)
                    {
                        _pdf.SetFont();

                        if (colspan == 0)
                        {
                            _pdf.AddCell(_activeChunk, alignment);
                        }
                        else
                        {
                            _pdf.AddCellWithColSpan(_activeChunk, alignment, colspan);
                        }

                        cellEmpty = false;
                    }
                }
            }

            _pdf.EndTable();
        }

        private void PrintPagebreak()
        {
            _pdf.AddPage();
        }

        private void Rotate()
        {
            _pdf.Rotate();
        }

        private bool MoveInTag()
        {
            _activeProperty = String.Empty;
            _activePropertyValue = String.Empty;
            _activeChunk = String.Empty;

            SkipWhiteSpace();

            if (_currentTag.CurrentIndex > _currentTag.EndIndex)
                return false;

            char ch = _content[_currentTag.CurrentIndex];

            if (ch == '<')
            {
                _currentTag.CurrentIndex++;

                ch = _content[_currentTag.CurrentIndex];

                int propEnd = _content.IndexOf('>', _currentTag.CurrentIndex);
                string prop = _content.Substring(_currentTag.CurrentIndex, propEnd - _currentTag.CurrentIndex);

                if (prop.Contains("="))
                {
                    string[] parts = prop.Split('=');

                    if (parts.Length == 2)
                    {
                        prop = parts[0];
                        _activePropertyValue = parts[1];
                    }
                }

                _activeProperty = prop;
                _activeChunk = "#";

                _currentTag.CurrentIndex = propEnd + 1;
                return true;
            }

            while (ch != '<')
            {
                _activeChunk += ch;
                _currentTag.CurrentIndex++;
                ch = _content[_currentTag.CurrentIndex];
            }

            return true;
        }

        private Color GetColorFromText(string colorText)
        {
            Color color = Color.BLACK;
            if (colorText.Contains(","))
            {
                string[] parts = colorText.Split(',');

                if (parts.Length == 3)
                {
                    int r = Convert.ToInt32(parts[0]);
                    int g = Convert.ToInt32(parts[1]);
                    int b = Convert.ToInt32(parts[2]);

                    color = new Color(r, g, b);
                }
            }
            else
            {
                string text = colorText.ToLowerInvariant();
                switch (text)
                {
                    case "black": color = Color.BLACK; break;
                    case "blue": color = Color.BLUE; break;
                    case "cyan": color = Color.CYAN; break;
                    case "darkgray": color = Color.DARK_GRAY; break;
                    case "gray": color = Color.GRAY; break;
                    case "green": color = Color.GREEN; break;
                    case "lightgray": color = Color.LIGHT_GRAY; break;
                    case "magenta": color = Color.MAGENTA; break;
                    case "orange": color = Color.ORANGE; break;
                    case "pink": color = Color.PINK; break;
                    case "red": color = Color.RED; break;
                    case "white": color = Color.WHITE; break;
                    case "yellow": color = Color.YELLOW; break;
                }
            }

            return color;
        }

        private void SkipWhiteSpace()
        {
            bool skip = true;
            while (skip)
            {
                char c = _content[_currentTag.CurrentIndex];
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        _currentTag.CurrentIndex++;
                        break;
                    default: skip = false; break;
                }
            }
        }

        private void ClearComments()
        {
            int commentIndex = _content.IndexOf("<!--");
            while (commentIndex > -1)
            {
                int endIndex = _content.IndexOf("-->", commentIndex);

                _content = _content.Remove(commentIndex, endIndex + 3);

                commentIndex = _content.IndexOf("<!--");
            }
        }
    }
}