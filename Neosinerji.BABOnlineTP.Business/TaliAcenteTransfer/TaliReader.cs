using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{
   public class TaliReader
    {
        HSSFWorkbook wb;
        
        private string message = string.Empty;
        private string excelFileName;
        private int tvmKodu;

        private string[] columnNames =  {
                                            "Ünvan",
                                            "Açık Adres",
                                            "Telefon",
                                            "Faks",
                                            "Email"
                                        };

        public TaliReader(string fileName, int tvmKodu)
        {
            this.excelFileName = fileName;
            this.tvmKodu = tvmKodu;
        }

        public List<TaliAcenteExcel> getTaliler()
        {
            // excelden alinan taliler
            List<TaliAcenteExcel> getTaliler = new List<TaliAcenteExcel>();

            // get excel file...
            FileStream excelFile = null;

            try
            {
                excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            wb = new HSSFWorkbook(excelFile);
            ISheet sheet = wb.GetSheet("Sayfa1");
            
            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }

            // sheet correct. Start to get rows...
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {
                IRow row = sheet.GetRow(indx);

                // null rowlar icin
                if (row == null) continue;

                // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                if (row.FirstCellNum == 0 && row.GetCell(0).StringCellValue == "") break;

                // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                if (row.FirstCellNum == 0)
                {

                    TaliAcenteExcel tali = new TaliAcenteExcel();
                    List<ICell> cels = row.Cells;

                    foreach(ICell cell in cels) 
                    {
                        if (cell.ColumnIndex == 0) tali.Unvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 1) tali.AcikAdres = cell.StringCellValue;
                        if (cell.ColumnIndex == 2)
                        {
                            if (cell.CellType == CellType.Numeric)
                            {
                                tali.Telefon = cell.NumericCellValue.ToString();
                            }
                            else if (cell.CellType == CellType.String)
                            {
                                tali.Telefon = cell.StringCellValue;
                            }
                        }
                        if (cell.ColumnIndex == 3) 
                        {
                            if (cell.CellType == CellType.Numeric)
                            {
                                tali.Faks = cell.NumericCellValue.ToString();
                            }
                            else if (cell.CellType == CellType.String)
                            {
                                tali.Faks = cell.StringCellValue;
                            }
                        }
                        if (cell.ColumnIndex == 4) tali.Email = cell.StringCellValue;
                    }
                    getTaliler.Add(tali);
                }
            }
            return getTaliler;
        }
    }
}
