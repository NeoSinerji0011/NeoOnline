using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business
{
    public class DataTableParameters<T>
    {
        private const string SEARCH_KEY = "sSearch";
        private const string SORT_KEY = "iSortCol_0";
        private const string SORT_DIRECTION_KEY = "sSortDir_0";
        private const string DISPLAY_START = "iDisplayStart";
        private const string DISPLAY_LENGTH = "iDisplayLength";
        private const string ECHO = "sEcho";
        private const string ASCENDING_SORT = "asc";
        private const string UPDATE_COLUMN_FORMAT =
@"<div class='btn-group'> 
    <a href='{0}{1}' class='btn btn-mini' rel='tooltip' data-placement='bottom' data-original-title='Detay'><i class='icon-info-sign'></i></a>
    <a href='{2}{1}' id='btn-yetkili-guncelle' class='btn btn-mini' rel='tooltip' data-placement='bottom' data-original-title='Güncelle'><i class='icon-edit'></i></a>
    <a href='{3}{1}' id='btn-gorev-ata' class='btn btn-mini' rel='tooltip' data-placement='bottom' data-original-title='Görev Ata'><i class='icon-arrow-right'></i></a>
</div>";

        private HttpRequestBase _HttpRequest;

        public DataTableParameters(HttpRequestBase httpRequest, Expression<Func<T, object>>[] selectColumns)
        {
            _HttpRequest = httpRequest;
            if (_HttpRequest.Params[ECHO] != null)
            {
                this.Echo = int.Parse(_HttpRequest.Params[ECHO]);
            }

            this.RowIdColumn = null;
            this.LinkColumn1 = null;
            this.LinkColumn1Url = String.Empty;
            this.UpdateUrl = String.Empty;

            this.SelectColumns = selectColumns;
            if (_HttpRequest.Params[SORT_KEY] != null)
            {
                int sortColumnIndex = int.Parse(_HttpRequest.Params[SORT_KEY]);

                this.OrderByProperty = this.SelectColumns[sortColumnIndex];

                string sortDirection = _HttpRequest.Params[SORT_DIRECTION_KEY];
                if (String.IsNullOrEmpty(sortDirection))
                    this.IsAscendingOrder = true;
                else
                    this.IsAscendingOrder = sortDirection.ToLower() == "asc";
            }
            else
            {
                this.OrderByProperty = this.SelectColumns[0];
                this.IsAscendingOrder = true;
            }
            int displayStart = 0;
            if (_HttpRequest.Params[DISPLAY_START] != null)
            {
                displayStart = int.Parse(_HttpRequest.Params[DISPLAY_START]);
            }
            if (_HttpRequest.Params[DISPLAY_LENGTH] != null)
            {
                this.PageSize = int.Parse(_HttpRequest.Params[DISPLAY_LENGTH]);
            }

            if (displayStart == 0)
                this.Page = 1;
            else if (this.PageSize <= displayStart)
                this.Page = (int)Math.Ceiling((decimal)(displayStart + 1) / (decimal)this.PageSize);
            else
                this.Page = 1;
            if (_HttpRequest.Params[SEARCH_KEY] != null)
            {
                this.SearchKeyword = _HttpRequest.Params[SEARCH_KEY];
            }

        }

        public DataTableParameters(HttpRequestBase httpRequest,
                           Expression<Func<T, object>>[] selectColumns,
                           Expression<Func<T, object>> rowIdColumn)
            : this(httpRequest, selectColumns)
        {
            this.RowIdColumn = rowIdColumn;
        }

        public DataTableParameters(HttpRequestBase httpRequest,
                                   Expression<Func<T, object>>[] selectColumns,
                                   Expression<Func<T, object>> rowIdColumn,
                                   Expression<Func<T, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateUrl)
            : this(httpRequest, selectColumns)
        {
            this.RowIdColumn = rowIdColumn;
            this.LinkColumn1 = linkColumn1;
            this.LinkColumn1Url = linkColumn1Url;
            this.UpdateUrl = updateUrl;
        }
        public DataTableParameters(HttpRequestBase httpRequest,
                                Expression<Func<T, object>>[] selectColumns,
                                Expression<Func<T, object>> rowIdColumn,
                                Expression<Func<T, object>> linkColumn1,
                                string linkColumn1Url,
                                string updateUrl,
                                string gorevUrl)
         : this(httpRequest, selectColumns)
        {
            this.RowIdColumn = rowIdColumn;
            this.LinkColumn1 = linkColumn1;
            this.LinkColumn1Url = linkColumn1Url;
            this.UpdateUrl = updateUrl;
            this.GorevUrl = gorevUrl;
        }
        public DataTableList Prepare(IQueryable<T> query, int totalRowCount)
        {
            DataTableList list = new DataTableList();
            Type type = typeof(T);

            // import property names
            List<string> selectColumnNames = new List<string>();
            foreach (var item in this.SelectColumns)
            {
                string columnName = GetColumnName(item);

                if (!String.IsNullOrEmpty(columnName))
                {
                    selectColumnNames.Add(columnName);
                }
            }

            //Row id column name
            string rowIdColumnName = String.Empty;
            PropertyInfo rowIdProperty = null;
            if (this.RowIdColumn != null)
            {
                rowIdColumnName = GetColumnName(this.RowIdColumn);
                rowIdProperty = type.GetProperty(rowIdColumnName);
            }

            //LinkColumn1 column name
            string linkColumn1Name = String.Empty;
            PropertyInfo linkColumn1Property = null;
            if (this.LinkColumn1 != null)
            {
                linkColumn1Name = GetColumnName(this.LinkColumn1);
                linkColumn1Property = type.GetProperty(linkColumn1Name);
            }

            list.Import(selectColumnNames.ToArray());

            // parse the echo property (must be returned as int to prevent XSS-attack)
            list.sEcho = this.Echo;

            list.iTotalRecords = totalRowCount;
            list.iTotalDisplayRecords = totalRowCount;

            // setup the data
            PropertyInfo[] properties = type.GetProperties().Join(selectColumnNames, o => o.Name, i => i, (o, i) => o).ToArray();

            IList<T> data = query.ToList();
            list.aaData = new List<Dictionary<string, string>>();
            foreach (T item in data)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                string rowIdValue = String.Empty;

                int index = 0;
                string linkColumnKey = String.Empty;
                foreach (var property in properties)
                {
                    string key = index.ToString();
                    string value = (property.GetValue(item, new object[0]) ?? String.Empty).ToString();

                    if (linkColumn1Property != null && property.Name == linkColumn1Property.Name)
                    {
                        linkColumnKey = key;
                    }

                    if (HasFormatter(property.Name))
                    {
                        value = ApplyFormat(item, property.Name);
                    }

                    row.Add(key, value);

                    if (rowIdProperty != null && property.Name == rowIdProperty.Name)
                    {
                        rowIdValue = value;
                    }

                    index++;
                }

                if (!String.IsNullOrEmpty(rowIdColumnName))
                {
                    if (!String.IsNullOrEmpty(linkColumnKey))
                    {
                        string linkText = row[linkColumnKey].ToString();
                        string link = String.Format("<a href='{0}{1}'>{2}</a>", this.LinkColumn1Url, rowIdValue, linkText);
                        row[linkColumnKey] = link;
                    }
                    if (!String.IsNullOrEmpty(this.UpdateUrl))
                    {
                        string updateColumn = String.Format(UPDATE_COLUMN_FORMAT, this.LinkColumn1Url, rowIdValue, this.UpdateUrl,"");
                        row.Add(index.ToString(), updateColumn);
                    }
                    if (!String.IsNullOrEmpty(this.GorevUrl))
                    {
                        string gorevColumn = String.Format(UPDATE_COLUMN_FORMAT, this.LinkColumn1Url, rowIdValue, this.UpdateUrl, this.GorevUrl);
                        row.Add(index.ToString(), gorevColumn);
                    }
                    row.Add("DT_RowId", rowIdValue);
                }

                list.aaData.Add(row);
            }

            return list;
        }

        public DataTableList Prepare(IList<T> data, int totalRowCount)
        {
            DataTableList list = new DataTableList();
            Type type = typeof(T);

            // import property names
            List<string> selectColumnNames = new List<string>();
            foreach (var item in this.SelectColumns)
            {
                string columnName = GetColumnName(item);

                if (!String.IsNullOrEmpty(columnName))
                {
                    selectColumnNames.Add(columnName);
                }
            }

            //Row id column name
            string rowIdColumnName = String.Empty;
            PropertyInfo rowIdProperty = null;
            if (this.RowIdColumn != null)
            {
                rowIdColumnName = GetColumnName(this.RowIdColumn);
                rowIdProperty = type.GetProperty(rowIdColumnName);
            }

            //LinkColumn1 column name
            string linkColumn1Name = String.Empty;
            PropertyInfo linkColumn1Property = null;
            if (this.LinkColumn1 != null)
            {
                linkColumn1Name = GetColumnName(this.LinkColumn1);
                linkColumn1Property = type.GetProperty(linkColumn1Name);
            }

            list.Import(selectColumnNames.ToArray());

            // parse the echo property (must be returned as int to prevent XSS-attack)
            list.sEcho = this.Echo;

            list.iTotalRecords = totalRowCount;
            list.iTotalDisplayRecords = totalRowCount;

            // setup the data
            PropertyInfo[] properties = type.GetProperties().Join(selectColumnNames, o => o.Name, i => i, (o, i) => o).ToArray();

            list.aaData = new List<Dictionary<string, string>>();
            foreach (T item in data)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                string rowIdValue = String.Empty;

                int index = 0;
                string linkColumnKey = String.Empty;
                foreach (var property in properties)
                {
                    string key = index.ToString();
                    string value = (property.GetValue(item, new object[0]) ?? String.Empty).ToString();

                    if (linkColumn1Property != null && property.Name == linkColumn1Property.Name)
                    {
                        linkColumnKey = key;
                    }

                    if (HasFormatter(property.Name))
                    {
                        value = ApplyFormat(item, property.Name);
                    }

                    row.Add(key, value);

                    index++;
                }

                if (!String.IsNullOrEmpty(rowIdColumnName))
                {
                    rowIdValue = (rowIdProperty.GetValue(item, new object[0]) ?? String.Empty).ToString();

                    if (!String.IsNullOrEmpty(linkColumnKey))
                    {
                        string linkText = row[linkColumnKey].ToString();
                        string link = String.Format("<a class='identity' data-id='{1}' href='{0}{1}'>{2}</a>", this.LinkColumn1Url, rowIdValue, linkText);
                        row[linkColumnKey] = link;
                    }
                    string updateColumn = "";
                    if (!String.IsNullOrEmpty(this.UpdateUrl))
                    {                        
                         updateColumn = String.Format(UPDATE_COLUMN_FORMAT, this.LinkColumn1Url, rowIdValue, this.UpdateUrl,"");
                     }
                    if (!String.IsNullOrEmpty(this.GorevUrl))
                    {
                        updateColumn = String.Format(UPDATE_COLUMN_FORMAT, this.LinkColumn1Url, rowIdValue, this.UpdateUrl, this.GorevUrl);                        
                    }
                    if (updateColumn!="")
                    {
                        row.Add(index.ToString(), updateColumn);
                    }
                 
                    row.Add("DT_RowId", rowIdValue);
                }

                list.aaData.Add(row);
            }

            return list;
        }

        public void AddFormatter(Expression<Func<T, object>> column, Func<T, string> function)
        {
            string columnName = GetColumnName(column);
            this.Formatters.Add(columnName, function);
        }

        public bool HasFormatter(string columnName)
        {
            return _Formatters != null && _Formatters.Count > 0 && _Formatters.ContainsKey(columnName);
        }

        public string ApplyFormat(T item, string columnName)
        {
            Func<T, string> func = this.Formatters[columnName];
            return func(item);
        }

        private string GetColumnName(Expression<Func<T, object>> item)
        {
            string result = String.Empty;
            if (item.NodeType == ExpressionType.Lambda)
            {
                if (item.Body is UnaryExpression)
                {
                    UnaryExpression exp = item.Body as UnaryExpression;
                    MemberExpression mem = exp.Operand as MemberExpression;

                    result = mem.Member.Name;
                }
                else if (item.Body is MemberExpression)
                {
                    MemberExpression exp = item.Body as MemberExpression;
                    result = exp.Member.Name;
                }
            }

            return result;
        }

        public int? TryParseParamInt(string paramName)
        {
            int? result = null;
            string value = this._HttpRequest[paramName];

            if (!String.IsNullOrEmpty(value))
            {
                int val = 0;
                if (int.TryParse(value, out val))
                {
                    result = val;
                }
            }

            return result;
        }

        public string[] TryParseParamArray(string paramName)
        {
            string[] result = null;
            string value = this._HttpRequest[paramName];

            if (!String.IsNullOrEmpty(value))
            {
                result = value.Split(',');
            }

            return result;
        }

        public DateTime? TryParseParamDate(string paramName)
        {
            DateTime? result = null;
            var value = this._HttpRequest[paramName];
            DateTime date = new DateTime();
            if (!String.IsNullOrEmpty(value))
            {
                System.Globalization.CultureInfo tr = new System.Globalization.CultureInfo("tr-TR");

                if (DateTime.TryParse(value, tr, System.Globalization.DateTimeStyles.None, out date))
                {
                    result = date;
                }
            }

            return result;
        }

        public short? TryParseParamShort(string paramName)
        {
            short? result = null;
            string value = this._HttpRequest[paramName];

            if (!String.IsNullOrEmpty(value))
            {
                short val = 0;
                if (short.TryParse(value, out val))
                {
                    result = val;
                }
            }

            return result;
        }

        public byte? TryParseParamByte(string paramName)
        {
            byte? result = null;
            string value = this._HttpRequest[paramName];

            if (!String.IsNullOrEmpty(value))
            {
                byte val = 0;
                if (byte.TryParse(value, out val))
                {
                    result = val;
                }
            }

            return result;
        }

        public string TryParseParamString(string paramName)
        {
            string value = this._HttpRequest[paramName];
            return value;
        }

        private Dictionary<string, Func<T, string>> _Formatters;
        public Dictionary<string, Func<T, string>> Formatters
        {
            get
            {
                if (_Formatters == null)
                    _Formatters = new Dictionary<string, Func<T, string>>();

                return _Formatters;
            }
        }

        private int Echo { get; set; }
        public bool IsAscendingOrder { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchKeyword { get; set; }
        public Expression<Func<T, object>> OrderByProperty { get; set; }
        public Expression<Func<T, object>>[] SelectColumns { get; set; }

        public Expression<Func<T, object>> RowIdColumn { get; set; }
        public Expression<Func<T, object>> LinkColumn1 { get; set; }
        public string LinkColumn1Url { get; set; }
        public string UpdateUrl { get; set; }
        public string GorevUrl { get; set; }
    }
}
