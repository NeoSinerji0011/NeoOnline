using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.KesintiTransfer
{
    class KesintiExcelReader
    {
        HSSFWorkbook wb;


        private int tvmKodu;
        private int ay;
        private int yil;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string filePath;
        private string[] columnNames =  {
                                            "Id"
                                            ,"SubeAdi"
                                            ,"NeoŞubeKodu"
                                            ,"SGK"
                                            ,"Maas"
                                            ,"SabitTelefon"	
                                            ,"GSMTelefon"	
                                            ,"Muhasebe"	
                                            ,"StopajSgk"	
                                            ,"StopajVergi"	
                                            ,"Vergi"	
                                            ,"Borç"	
                                            ,"İnternet"
                                            ,"Elektrik"	
                                            ,"Su"	
                                            ,"Isınma"	
                                            ,"BinaAidat"	
                                            ,"Diger"	
                                            ,"DigerAciklama"

                                        };

        public KesintiExcelReader(string path, int tvmKodu, int ay, int yil)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.filePath = path;
            this.tvmKodu = tvmKodu;
            this.ay = ay;
            this.yil = yil;
        }

        public List<Kesintiler> getKesintiler()
        {
            List<Kesintiler> kesintiler = new List<Kesintiler>();
            FileStream excelFile = null;
            try
            {
                excelFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }
            wb = new HSSFWorkbook(excelFile);
            ISheet sheet = wb.GetSheet("Kesintiler");

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
                if (row == null) break;
                Kesintiler kesinti = new Kesintiler();
                int? taliTvmKodu = null;
                List<ICell> cels = row.Cells;

                foreach (ICell cell in cels)
                {
                    if (cell.ColumnIndex == 2)
                    {
                        taliTvmKodu = Util.toInt(cell.NumericCellValue.ToString());
                    }
                    else if (cell.ColumnIndex > 2)
                    {
                       kesinti= this.KesintiListesiOlustur(taliTvmKodu.Value, cell.ColumnIndex, cell.NumericCellValue.ToString());
                        kesintiler.Add(kesinti);
                    }

                }
            }

            return kesintiler;
        }

        private Kesintiler KesintiListesiOlustur(int TaliTvmKodu, int collIdex, string collValue)
        {
            Kesintiler kesinti = new Kesintiler();

            kesinti.TVMKodu = tvmKodu;
            kesinti.TVMKoduTali = TaliTvmKodu;
            kesinti.Donem = yil;
            kesinti.KayitTarihi = TurkeyDateTime.Now;

            if (collIdex == 3)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.SGK;
            }
            else if (collIdex == 4)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.Maas;
            }
            else if (collIdex == 5)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.SabitTelefon;
            }
            else if (collIdex == 6)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.GsmTelefon;
            }
            else if (collIdex == 7)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.Muhasebe;
            }
            else if (collIdex == 8)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.StopajSgk;
            }
            else if (collIdex == 9)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.StopajKira;
            }
            else if (collIdex == 10)
            {
                kesinti.KesintiKodu = SatisKanaliKesintiTurleri.Vergi;
            }

            if (ay == 1)
            {
                kesinti.Borc1 = Util.ToDecimal(collValue);
            }
            else if (ay == 2)
            {
                kesinti.Borc2 = Util.ToDecimal(collValue);
            }
            else if (ay == 3)
            {
                kesinti.Borc3 = Util.ToDecimal(collValue);
            }
            else if (ay == 4)
            {
                kesinti.Borc4 = Util.ToDecimal(collValue);
            }
            else if (ay == 5)
            {
                kesinti.Borc5 = Util.ToDecimal(collValue);
            }
            else if (ay == 6)
            {
                kesinti.Borc6 = Util.ToDecimal(collValue);
            }
            else if (ay == 7)
            {
                kesinti.Borc7 = Util.ToDecimal(collValue);
            }
            else if (ay == 8)
            {
                kesinti.Borc8 = Util.ToDecimal(collValue);
            }
            else if (ay == 9)
            {
                kesinti.Borc9 = Util.ToDecimal(collValue);
            }
            else if (ay == 10)
            {
                kesinti.Borc10 = Util.ToDecimal(collValue);
            }
            else if (ay == 11)
            {
                kesinti.Borc11 = Util.ToDecimal(collValue);
            }
            else if (ay == 12)
            {
                kesinti.Borc12 = Util.ToDecimal(collValue);
            }
            return kesinti;
        }

        public string getMessage()
        {
            return this.message;
        }
    }
}
