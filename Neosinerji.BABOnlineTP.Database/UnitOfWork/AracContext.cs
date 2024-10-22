using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface IAracContext : IUnitOfWork
    {
        AracMarkaRepository AracMarkaRepository { get; }
        AracTipRepository AracTipRepository { get; }
        AracModelRepository AracModelRepository { get; }
        AracKullanimSekliRepository AracKullanimSekliRepository { get; }
        AracKullanimTarziRepository AracKullanimTarziRepository { get; }
        AracDegerRepository AracDegerRepository { get; }
        AracTrafikTeminatRepository AracTrafikTeminatRepository { get; }
        bool HdiWebServiceRunProcedure(string xml);
        bool AracDegerleriGonder(List<AracDegerleriExcelModel> model);
    }

    public class AracContext : IAracContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public AracContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
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

        #region IAracContext
        private AracMarkaRepository _aracMarkaRepository;
        public AracMarkaRepository AracMarkaRepository
        {
            get
            {
                if (_aracMarkaRepository == null)
                    _aracMarkaRepository = new AracMarkaRepository(_dbContext);

                return _aracMarkaRepository;
            }
        }

        private AracTipRepository _aracTipRepository;
        public AracTipRepository AracTipRepository
        {
            get
            {
                if (_aracTipRepository == null)
                    _aracTipRepository = new AracTipRepository(_dbContext);

                return _aracTipRepository;
            }
        }

        private AracModelRepository _aracModelRepository;
        public AracModelRepository AracModelRepository
        {
            get
            {
                if (_aracModelRepository == null)
                    _aracModelRepository = new AracModelRepository(_dbContext);

                return _aracModelRepository;
            }
        }

        private AracKullanimSekliRepository _aracKullanimSekliRepository;
        public AracKullanimSekliRepository AracKullanimSekliRepository
        {
            get
            {
                if (_aracKullanimSekliRepository == null)
                    _aracKullanimSekliRepository = new AracKullanimSekliRepository(_dbContext);

                return _aracKullanimSekliRepository;
            }
        }

        private AracKullanimTarziRepository _aracKullanimTarziRepository;
        public AracKullanimTarziRepository AracKullanimTarziRepository
        {
            get
            {
                if (_aracKullanimTarziRepository == null)
                    _aracKullanimTarziRepository = new AracKullanimTarziRepository(_dbContext);

                return _aracKullanimTarziRepository;
            }
        }

        private AracDegerRepository _aracdegerRepository;
        public AracDegerRepository AracDegerRepository
        {
            get
            {
                if (_aracdegerRepository == null)
                    _aracdegerRepository = new AracDegerRepository(_dbContext);

                return _aracdegerRepository;
            }
        }

        private AracTrafikTeminatRepository _aracTrafikTeminatRepository;
        public AracTrafikTeminatRepository AracTrafikTeminatRepository
        {
            get
            {
                if (_aracTrafikTeminatRepository == null)
                    _aracTrafikTeminatRepository = new AracTrafikTeminatRepository(_dbContext);

                return _aracTrafikTeminatRepository;
            }
        }
        #endregion

        #region Procedure
        public bool AracDegerleriGonder(List<AracDegerleriExcelModel> model)
        {
            string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BABOnlineContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // ========== Arac Deger Ara Tablosundaki kayıtları temizliyor======//
                SqlCommand cmd = new SqlCommand("SP_AracDegerleriTemizle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                // ============== Yeni Procedure command e veriliyor =============//
                cmd = new SqlCommand("SP_AracEkleme_ByExcel", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter MarkaKodu = new SqlParameter("@MarkaKodu", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(MarkaKodu);

                SqlParameter tipkodu = new SqlParameter("@tipkodu", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(tipkodu);

                SqlParameter tipadi = new SqlParameter("@tipadi", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add(tipadi);

                SqlParameter markaadi = new SqlParameter("@markaadi", SqlDbType.NVarChar, 50);
                cmd.Parameters.Add(markaadi);

                SqlParameter yil21 = new SqlParameter("@yil2021", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil21);

                SqlParameter yil20 = new SqlParameter("@yil2020", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil20);

                SqlParameter yil19 = new SqlParameter("@yil2019", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil19);

                SqlParameter yil18 = new SqlParameter("@yil2018", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil18);

                SqlParameter yil17 = new SqlParameter("@yil2017", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil17);

                SqlParameter yil16 = new SqlParameter("@yil2016", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil16);

                SqlParameter yil15 = new SqlParameter("@yil2015", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil15);

                SqlParameter yil14 = new SqlParameter("@yil2014", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil14);

                SqlParameter yil13 = new SqlParameter("@yil2013", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil13);

                SqlParameter yil12 = new SqlParameter("@yil2012", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil12);

                SqlParameter yil11 = new SqlParameter("@yil2011", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil11);

                SqlParameter yil10 = new SqlParameter("@yil2010", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil10);

                SqlParameter yil9 = new SqlParameter("@yil2009", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil9);

                SqlParameter yil8 = new SqlParameter("@yil2008", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil8);

                SqlParameter yil7 = new SqlParameter("@yil2007", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add(yil7);


                foreach (var item in model)
                {
                    MarkaKodu.Value = item.MarkaKodu;
                    markaadi.Value = item.markaAdi;
                    tipkodu.Value = item.tipKodu;
                    tipadi.Value = item.tipAdi;
                    yil21.Value = item.yil2023;//en yeni tarihten baslayarak  ekliyoruz (sağ  tarafı)
                    yil20.Value = item.yil2022;
                    yil19.Value = item.yil2021;
                    yil18.Value = item.yil2020;
                    yil17.Value = item.yil2019;
                    yil16.Value = item.yil2018;
                    yil15.Value = item.yil2017;
                    yil14.Value = item.yil2016;
                    yil13.Value = item.yil2015;
                    yil12.Value = item.yil2014;
                    yil11.Value = item.yil2013;
                    yil10.Value = item.yil2012;
                    yil9.Value = item.yil2011;
                    yil8.Value = item.yil2010;
                    yil7.Value = item.yil2009;

                    cmd.ExecuteNonQuery();
                }
                cmd.Dispose();
                cmd = null;

                // =============== Tül liste eklendikten sonra  Dağıtımı yapıcak procedure calıstırılıyor.=======//
                cmd = new SqlCommand("SP_AracListesiDagitim", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public bool HdiWebServiceRunProcedure(string xml)
        {
            string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BABOnlineContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SP_HDIWebServiceAktar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter XmlParameter = new SqlParameter("@XmlDoc", SqlDbType.Xml);
                XmlParameter.Value = xml;

                cmd.Parameters.Add(XmlParameter);
                cmd.CommandTimeout = 600;
                //Procedure Calıştırılıyor..
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return true;
        }
        #endregion
    }
}
