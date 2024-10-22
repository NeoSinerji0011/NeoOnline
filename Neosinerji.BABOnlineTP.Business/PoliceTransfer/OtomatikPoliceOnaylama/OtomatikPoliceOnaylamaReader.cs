using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceOnaylama
{
    public class OtomatikPoliceOnaylamaReader : IOtomatikPoliceOnaylamaReader
    {
        HSSFWorkbook wb;
        ITVMService _TVMService;
        IPoliceTransferService _PoliceTransferService;
        IKomisyonService _KomisyonService;
        ISigortaSirketleriService _SigortaSirketleriService;
        private string message = string.Empty;
        private string excelFileName;
        private string[] columnNames =  {
                                            "SatisKanaliKodu",                  //0
                                            "SirketKodu",              //1  
                                            "PoliceNumarasi",          //2      
                                                                                               
                                        };
        public OtomatikPoliceOnaylamaReader()
        { }

        public OtomatikPoliceOnaylamaReader(string fileName)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _PoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _KomisyonService = DependencyResolver.Current.GetService<IKomisyonService>();
            _SigortaSirketleriService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();

            this.excelFileName = fileName;
        }

        public List<PoliceOnaySonucModel> PoliceleriOnayla()
        {
            List<PoliceOnaySonucModel> sonucListModel = new List<PoliceOnaySonucModel>();
            PoliceOnaySonucModel sonucModel = new PoliceOnaySonucModel();
            try
            {
                List<Police> policeler = new List<Police>();
                // get excel file...
                FileStream excelFile = null;

                List<OtoPolOnayModel> ListModel = new List<OtoPolOnayModel>();
                OtoPolOnayModel model = new OtoPolOnayModel();

                try
                {
                    excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
                }
                catch (IOException ioe)
                {
                    message = ioe.ToString();
                    //return null;
                }
                wb = new HSSFWorkbook(excelFile);
                ISheet sheet = wb.GetSheet("Sayfa1");

                // check sheet correct... 
                int startRow = checkSheetCorrect(sheet);
                if (startRow == -1) // error
                {
                    message = "Sheet format error ....";
                    //  return null;
                }
                // sheet correct. Start to get rows...
                for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    IRow row = sheet.GetRow(indx);

                    // null rowlar icin
                    if (row == null) continue;

                    // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                    if (row.FirstCellNum == 0 && row.GetCell(0).CellType != CellType.Numeric) break;

                    // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                    if (row.FirstCellNum == 0)
                    {
                        List<ICell> cels = row.Cells;
                        model = new OtoPolOnayModel();
                        foreach (ICell cell in cels)
                        {
                            if (cell.ColumnIndex == 0)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    model.AltAcenteKodu = Convert.ToInt32(cell.NumericCellValue);
                                }
                                else if (cell.CellType == CellType.String)
                                {
                                    model.AltAcenteKodu = Convert.ToInt32(cell.StringCellValue);
                                }
                            }
                            if (cell.ColumnIndex == 1)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    model.SigortaSirketKodu = cell.NumericCellValue.ToString();
                                }
                                else if (cell.CellType == CellType.String)
                                {
                                    model.SigortaSirketKodu = cell.StringCellValue;
                                }
                            }
                            if (cell.ColumnIndex == 2)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    model.PoliceNumarasi = cell.NumericCellValue.ToString();
                                }
                                else if (cell.CellType == CellType.String)
                                {
                                    model.PoliceNumarasi = cell.StringCellValue;
                                }
                            }
                        }
                    }
                    ListModel.Add(model);
                }
                bool guncellendiMi = true;
                if (ListModel != null)
                {
                    if (ListModel.Count > 0)
                    {
                        List<TVMOzetModel> yetkiliTVMler = _TVMService.GetTVMListeKullaniciYetki(0);
                        List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
                        foreach (var item in ListModel)
                        {
                            var getPoliceDetay = _PoliceTransferService.getOtoOnayPoliceler(item.PoliceNumarasi, item.SigortaSirketKodu);
                            if (getPoliceDetay.policeler.Count > 0)
                            {
                                foreach (var itemPol in getPoliceDetay.policeler)
                                {
                                    var taliAcenteKomisyonOrani = _KomisyonService.TaliAcenteKomisyonOrani(item.AltAcenteKodu.Value, itemPol.BransKodu.Value, item.SigortaSirketKodu, itemPol.TanzimTarihi.Value, item.PoliceNumarasi);
                                    if (taliAcenteKomisyonOrani != null && taliAcenteKomisyonOrani >= 0)
                                    {
                                        var taliKomisyonTutari = itemPol.Komisyon * taliAcenteKomisyonOrani / 100;
                                        var result = _KomisyonService.TeklifGenelKomisyonGuncelle(itemPol.PoliceId, item.AltAcenteKodu, 0, taliAcenteKomisyonOrani, taliKomisyonTutari.Value, out guncellendiMi);
                                        if (guncellendiMi)
                                        {
                                            sonucModel = new PoliceOnaySonucModel()
                                            {
                                                GuncellemeBasarili = true,
                                                TaliAcenteKodu = item.AltAcenteKodu.Value,

                                                BilgiMesaji = item.AltAcenteKodu.ToString() + " kodlu tali acenteye komisyon ataması başarılı bir şekilde yapılmıştır.",
                                                PoliceNumarasi = item.PoliceNumarasi,
                                                YenilemeNumarasi = itemPol.YenilemeNo,
                                                EkNumarasi = itemPol.EkNo,
                                                SigortaSirketKodu = item.SigortaSirketKodu,
                                                GenelHataMesaji = ""
                                            };

                                            if (item.AltAcenteKodu.HasValue)
                                            {
                                                var tvmDetay = yetkiliTVMler.Where(s => s.Kodu == item.AltAcenteKodu.Value).FirstOrDefault();
                                                if (tvmDetay != null)
                                                {
                                                    sonucModel.TaliAcenteUnvani = tvmDetay.Unvani;
                                                }
                                            }
                                            var sirketDetay = sigortaSirketleri.Where(s => s.SirketKodu == item.SigortaSirketKodu).FirstOrDefault();
                                            if (sirketDetay != null)
                                            {
                                                sonucModel.SigortaSirketUnvani = sirketDetay.SirketAdi;
                                            }
                                            sonucListModel.Add(sonucModel);
                                        }
                                        else
                                        {
                                            sonucModel = new PoliceOnaySonucModel()
                                            {
                                                GuncellemeBasarili = false,
                                                TaliAcenteKodu = item.AltAcenteKodu.Value,
                                                BilgiMesaji = item.AltAcenteKodu.ToString() + " kodlu tali acentenin komisyon ataması yapılırken bir hata oluştu.",
                                                PoliceNumarasi = item.PoliceNumarasi,
                                                YenilemeNumarasi = itemPol.YenilemeNo,
                                                EkNumarasi = itemPol.EkNo,
                                                SigortaSirketKodu = item.SigortaSirketKodu,
                                                GenelHataMesaji = ""
                                            };
                                            if (item.AltAcenteKodu.HasValue)
                                            {
                                                var tvmDetay = yetkiliTVMler.Where(s => s.Kodu == item.AltAcenteKodu.Value).FirstOrDefault();
                                                if (tvmDetay != null)
                                                {
                                                    sonucModel.TaliAcenteUnvani = tvmDetay.Unvani;
                                                }
                                            }
                                            var sirketDetay = sigortaSirketleri.Where(s => s.SirketKodu == item.SigortaSirketKodu).FirstOrDefault();
                                            if (sirketDetay != null)
                                            {
                                                sonucModel.SigortaSirketUnvani = sirketDetay.SirketAdi;
                                            }
                                            sonucListModel.Add(sonucModel);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sonucModel = new PoliceOnaySonucModel()
                                {
                                    GuncellemeBasarili = false,
                                    TaliAcenteKodu = item.AltAcenteKodu.Value,
                                    BilgiMesaji = getPoliceDetay.BilgiMesaji,
                                    PoliceNumarasi = item.PoliceNumarasi,
                                    SigortaSirketKodu = item.SigortaSirketKodu,
                                    GenelHataMesaji = ""
                                };
                                if (item.AltAcenteKodu.HasValue)
                                {
                                    var tvmDetay = yetkiliTVMler.Where(s => s.Kodu == item.AltAcenteKodu.Value).FirstOrDefault();
                                    if (tvmDetay != null)
                                    {
                                        sonucModel.TaliAcenteUnvani = tvmDetay.Unvani;
                                    }
                                }
                                var sirketDetay = sigortaSirketleri.Where(s => s.SirketKodu == item.SigortaSirketKodu).FirstOrDefault();
                                if (sirketDetay != null)
                                {
                                    sonucModel.SigortaSirketUnvani = sirketDetay.SirketAdi;
                                }
                                sonucListModel.Add(sonucModel);
                            }

                        }
                    }
                }
                 return sonucListModel;
            }
            catch (Exception ex)
            {
                sonucModel = new PoliceOnaySonucModel()
                {
                    GuncellemeBasarili = false,
                    GenelHataMesaji = "Otomatik Poliçe Onaylama işlemi başarısız oldu.Hata Detayı : " + ex.Message.ToString(),
                };
                sonucListModel.Add(sonucModel);

                return sonucListModel;
            }
        }

        private int checkSheetCorrect(ISheet sht)
        {
            for (int indx = 0; indx <= sht.LastRowNum; indx++)
            {
                IRow row = sht.GetRow(indx);
                if (row == null) continue;
                if (row.FirstCellNum != 0) continue;
                //check column sequenc is correct !!!
                if (row.GetCell(0).StringCellValue == columnNames[0])
                {
                    if (checkColumnSequence(row) == true)
                    {
                        return indx + 1;
                    }
                }
            }

            return -1;
        }

        private bool checkColumnSequence(IRow row)
        {
            bool tf = false;

            for (int indx = 0; indx < columnNames.Length; indx++)
            {
                string colName = row.GetCell(indx).StringCellValue;
                if (colName.Equals(columnNames[indx]))
                    tf = true;
                else
                {
                    tf = false;
                    break;
                }

            }
            return tf;
        }

        public class OtoPolOnayModel
        {
            public int? AltAcenteKodu { get; set; }
            public string SigortaSirketKodu { get; set; }
            public string PoliceNumarasi { get; set; }
        }


    }
}
