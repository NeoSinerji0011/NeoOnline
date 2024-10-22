using Neosinerji.BABOnlineTP.Database.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface IProcedureContext : IUnitOfWork
    {
        List<OzetRaporProcedureModel> OzetRapor_AnaTali(string BransList, string UrunList, int TarihTipi, int OdemeTipi, DateTime BaslamaTarihi, DateTime BitisTarihi,
                                                      int TahIpt, int DovizTL, int TVMKodu, int YetkiKodu, string XML);


        List<MapfreBolgeUretimModel> MapfreBolgeUretimRapor(DateTime BaslangicTarih, DateTime BitisTarih, int? BolgeKodu, bool Acenteler);

        List<PoliceSorguProcedurModel> PoliceSorgu(int tvmKodu, int sorguTipi, string kimlikNo, string plakaKodu, string plakaNo, bool teklifMi, bool policeMi);
        List<TeklifAramaTableModel1> PoliceAra(bool pTeklifMi, bool pMapfreMi, bool pMapfreTeklifKontrol, DateTime? pBaslangicTarihi, DateTime? pBitisTarihi, string pKulProjeKodu, int? pTUMKodu, string pPoliceNo
            , string pTeklifNo, int? pTVMKodu, bool pTVMYetkilimi, int pKulTVMKodu, int? pUrunKodu, int? pHazirlayanKodu, int? pTeklifDurumu, int? pMusteriKodu, DateTime pTDNow
            , int pPageSize, int pPage, out int pTotalRowCount);
    }

    public class ProcedureContext : IProcedureContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public ProcedureContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout = 180;
        }

        #region IUnitOfWork
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
        #endregion

        #region OZET RAPOR

        // ==== Ozet Raporu Ana Tali Getirir ====//
        public List<OzetRaporProcedureModel> OzetRapor_AnaTali(string BransList,
                                                             string UrunList,
                                                             int TarihTipi,
                                                             int OdemeTipi,
                                                             DateTime BaslamaTarihi,
                                                             DateTime BitisTarihi,
                                                             int TahIpt,
                                                             int DovizTL,
                                                             int TVMKodu,
                                                             int YetkiKodu,
                                                             string XML)
        {
            SqlParameter branslist = new SqlParameter("@branslist", SqlDbType.NVarChar, 200);
            branslist.Value = BransList;

            SqlParameter urunlist = new SqlParameter("@urunlist", SqlDbType.NVarChar, 200);
            urunlist.Value = UrunList;

            SqlParameter tarihtipi = new SqlParameter("@tarihtipi", SqlDbType.Int);
            tarihtipi.Value = TarihTipi;

            SqlParameter odemetipi = new SqlParameter("@odemetipi", SqlDbType.Int);
            odemetipi.Value = OdemeTipi;



            SqlParameter baslamatarihi = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslamatarihi.Value = BaslamaTarihi;

            SqlParameter bitistarihi = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitistarihi.Value = BitisTarihi;

            SqlParameter tahipt = new SqlParameter("@tahipt", SqlDbType.Int);
            tahipt.Value = TahIpt;

            SqlParameter doviztl = new SqlParameter("@doviztl", SqlDbType.Int);
            doviztl.Value = DovizTL;

            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = TVMKodu;

            SqlParameter yetkikodu = new SqlParameter("@yetkikodu", SqlDbType.Int);
            yetkikodu.Value = YetkiKodu;

            SqlParameter xml = new SqlParameter("@xml", SqlDbType.Xml);
            xml.Value = xml;


            List<OzetRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<OzetRaporProcedureModel>
                        ("SP_GET_OZETRAPOR @branslist, @urunlist, @tarihtipi, @odemetipi, @baslamatarihi, @bitistarihi, @tahipt, @doviztl,@tvmkodu,@yetkikodu,@xml",
                            branslist, urunlist, tarihtipi, odemetipi, baslamatarihi, bitistarihi, tahipt, doviztl, tvmkodu, yetkikodu, xml)
                        .ToList<OzetRaporProcedureModel>();
            return rapor;
        }


        #endregion

        #region Raporlar
        public List<MapfreBolgeUretimModel> MapfreBolgeUretimRapor(DateTime BaslangicTarih, DateTime BitisTarih, int? BolgeKodu, bool Acenteler)
        {
            StringBuilder sb = new StringBuilder("@BaslangicTarih=@BaslangicTarih, @BitisTarih=@BitisTarih");
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@BaslangicTarih", BaslangicTarih));
            parameters.Add(new SqlParameter("@BitisTarih", BitisTarih));

            if (BolgeKodu.HasValue && BolgeKodu > 0)
            {
                parameters.Add(new SqlParameter("@BolgeKodu", BolgeKodu));
                sb.Append(", @BolgeKodu=@BolgeKodu");
            }
            if (Acenteler)
            {
                parameters.Add(new SqlParameter("@Acenteler", 1));
                sb.Append(", @Acenteler=@Acenteler");
            }

            return _dbContext.Database.SqlQuery<MapfreBolgeUretimModel>("SP_MapfreBolgeUretimRaporu " + sb.ToString(),
                                                                        parameters.ToArray()).ToList<MapfreBolgeUretimModel>();
        }
        #endregion

        #region Mapfre PoliceSorgu

        public List<PoliceSorguProcedurModel> PoliceSorgu(int tvmKodu, int sorguTipi, string kimlikNo, string plakaKodu, string plakaNo, bool teklifMi, bool policeMi)
        {
            var parList = new List<SqlParameter>();
            parList.Add(new SqlParameter("@TVMKodu", tvmKodu));
            parList.Add(new SqlParameter("@KimlikNo", kimlikNo));
            parList.Add(new SqlParameter("@PlakaKodu", plakaKodu));
            parList.Add(new SqlParameter("@PlakaNo", plakaNo));
            parList.Add(new SqlParameter("@TeklifMi", teklifMi));
            parList.Add(new SqlParameter("@PoliceMi", policeMi));
            parList.Add(new SqlParameter("@SorguTipi", sorguTipi));
            return _dbContext.Database.SqlQuery<PoliceSorguProcedurModel>("exec SP_TeklifGeneldenPoliceSorgu_Mapfre @TVMKodu,@KimlikNo,@PlakaKodu,@PlakaNo,@TeklifMi,@PoliceMi,@SorguTipi", parList.ToArray()).ToList<PoliceSorguProcedurModel>();
        }

        #endregion

        public List<TeklifAramaTableModel1> PoliceAra(bool pTeklifMi, bool pMapfreMi, bool pMapfreTeklifKontrol, DateTime? pBaslangicTarihi, DateTime? pBitisTarihi, string pKulProjeKodu, int? pTUMKodu, string pPoliceNo
            , string pTeklifNo, int? pTVMKodu, bool pTVMYetkilimi, int pKulTVMKodu, int? pUrunKodu, int? pHazirlayanKodu, int? pTeklifDurumu, int? pMusteriKodu, DateTime pTDNow
            , int pPageSize, int pPage, out int pTotalRowCount)
        {
            if (!string.IsNullOrEmpty(pTeklifNo))
            {
                pBaslangicTarihi = null;
                pBitisTarihi = null;
            }
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TeklifMi", pTeklifMi));
            parameters.Add(new SqlParameter("@MapfreMi", pMapfreMi));
            parameters.Add(new SqlParameter("@MapfreTeklifKontrol", pMapfreTeklifKontrol));

            if (pBaslangicTarihi != null)
                parameters.Add(new SqlParameter("@BaslangicTarihi", pBaslangicTarihi));
            else
                parameters.Add(new SqlParameter("@BaslangicTarihi", DBNull.Value));

            if (pBitisTarihi != null)
                parameters.Add(new SqlParameter("@BitisTarihi", pBitisTarihi));
            else
                parameters.Add(new SqlParameter("@BitisTarihi", DBNull.Value));

            if (!string.IsNullOrEmpty(pKulProjeKodu))
                parameters.Add(new SqlParameter("@KulProjeKodu", pKulProjeKodu));
            else
                parameters.Add(new SqlParameter("@KulProjeKodu", DBNull.Value));

            if (pTUMKodu != null && pTUMKodu > 0)
                parameters.Add(new SqlParameter("@TUMKodu", pTUMKodu));
            else
                parameters.Add(new SqlParameter("@TUMKodu", DBNull.Value));

            if (!string.IsNullOrEmpty(pPoliceNo))
                parameters.Add(new SqlParameter("@PoliceNo", pPoliceNo));
            else
                parameters.Add(new SqlParameter("@PoliceNo", DBNull.Value));

            if (!string.IsNullOrEmpty(pTeklifNo))
                parameters.Add(new SqlParameter("@TeklifNo", pTeklifNo));
            else
                parameters.Add(new SqlParameter("@TeklifNo", DBNull.Value));

            if (pTVMKodu != null && pTVMKodu > 0)
                parameters.Add(new SqlParameter("@TVMKodu", pTVMKodu));
            else
                parameters.Add(new SqlParameter("@TVMKodu", DBNull.Value));

            parameters.Add(new SqlParameter("@TVMYetkilimi", pTVMYetkilimi));

            if (pKulTVMKodu > 0)
                parameters.Add(new SqlParameter("@KulTVMKodu", pKulTVMKodu));
            else
                parameters.Add(new SqlParameter("@KulTVMKodu", DBNull.Value));

            if (pUrunKodu != null && pUrunKodu > 0)
                parameters.Add(new SqlParameter("@UrunKodu", pUrunKodu));
            else
                parameters.Add(new SqlParameter("@UrunKodu", DBNull.Value));

            if (pHazirlayanKodu != null && pHazirlayanKodu > 0)
                parameters.Add(new SqlParameter("@HazirlayanKodu", pHazirlayanKodu));
            else
                parameters.Add(new SqlParameter("@HazirlayanKodu", DBNull.Value));

            if (pTeklifDurumu != null && pTeklifDurumu > 0)
                parameters.Add(new SqlParameter("@TeklifDurumu", pTeklifDurumu));
            else
                parameters.Add(new SqlParameter("@TeklifDurumu", DBNull.Value));

            if (pMusteriKodu != null && pMusteriKodu > 0)
                parameters.Add(new SqlParameter("@MusteriKodu", pMusteriKodu));
            else
                parameters.Add(new SqlParameter("@MusteriKodu", DBNull.Value));

            parameters.Add(new SqlParameter("@TDNow", pTDNow));
            parameters.Add(new SqlParameter("@PageSize", pPageSize));
            parameters.Add(new SqlParameter("@Page", pPage));
            var list = _dbContext.Database.SqlQuery<TeklifAramaTableModel1>(@"exec SP_PoliceAra @TeklifMi,@MapfreMi,@MapfreTeklifKontrol,@BaslangicTarihi,@BitisTarihi
,@KulProjeKodu,@TUMKodu,@PoliceNo,@TeklifNo,@TVMKodu,@TVMYetkilimi,@KulTVMKodu,@UrunKodu,@HazirlayanKodu,@TeklifDurumu,@MusteriKodu,@TDNow,@PageSize,@Page"
                , parameters.ToArray())
                .ToList<TeklifAramaTableModel1>();
            if (list.Count() > 0)
                pTotalRowCount = list[0].TotalCount;
            else
                pTotalRowCount = 0;
            return list;
        }
    }
}
