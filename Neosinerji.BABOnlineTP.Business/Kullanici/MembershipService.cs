using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MembershipService : IMembershipService
    {
        private readonly ITVMContext _TVMContext;

        public MembershipService(ITVMContext tvmContext)
        {
            _TVMContext = tvmContext;
        }

        #region IMembershipService Members
        /// <summary>
        /// Kullanıcı adı ve şifresinin onaylanması
        /// 
        /// Exceptions :
        ///     AccountLockedException : Kullanıcı hesabı kilitli ise
        /// </summary>
        /// <param name="tckn">Kullanıcı TCKN no</param>
        /// <param name="sifre">Şifre</param>
        /// <returns>
        ///     true : kullanıcı bilgileri doğru
        ///     false : kullanıcı bilgileri yanlış
        /// </returns>
        public bool ValidateUser(string email, string sifre)
        {
            bool validated = false;
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.Email == email).FirstOrDefault();

            if (kullanici != null)
            {
                if (kullanici.Durum == KullaniciDurumTipleri.Pasif || kullanici.Durum == KullaniciDurumTipleri.Dondurulmus)
                {
                    throw new AccountPasiveException();
                }

                if (kullanici.SifreDurumKodu == KullaniciSifreDurumTipleri.Kilitli)
                {
                    throw new AccountLockedException();
                }

                string hashedPassword = Encryption.HashPassword(sifre);

                if (hashedPassword == kullanici.Sifre)
                {
                    validated = true;
                }
               //var aa= Sifreleme.Decrypt(kullanici.Sifre,null);

                //Kullanıcı onaylanmadığında, eğer son 30 dakika içinde TVM hatalı giriş sayısından 
                //fazla yanlış şifre girildi is kullanıcı hesabı kilitlenir.
                if (!validated)
                {
                    TVMDetay tvmDetay = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);

                    if (kullanici.HataliSifreGirisTarihi.HasValue)
                    {
                        if (kullanici.HataliSifreGirisTarihi.Value > TurkeyDateTime.Now.AddMinutes(-30))
                        {
                            if (kullanici.HataliSifreGirisSayisi.HasValue && kullanici.HataliSifreGirisSayisi.Value >= tvmDetay.SifreKontralSayisi)
                            {
                                kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.Kilitli;
                                kullanici.HataliSifreGirisTarihi = null;
                                kullanici.HataliSifreGirisSayisi = 0;
                            }
                            else
                            {
                                kullanici.HataliSifreGirisSayisi++;
                            }
                        }
                        else
                        {
                            kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                            kullanici.HataliSifreGirisSayisi = 1;
                            kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                        }
                    }
                    else
                    {
                        kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                        kullanici.HataliSifreGirisSayisi = 1;
                        kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                    }
                }
                else
                {
                    kullanici.HataliSifreGirisTarihi = null;
                    kullanici.HataliSifreGirisSayisi = 0;
                    kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.SorunYok;
                }
            }

            _TVMContext.Commit();

            return validated;
        }

        public bool ValidateUserAEGON(string personelKodu, string sifre)
        {
            bool validated = false;
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == personelKodu).FirstOrDefault();

            if (kullanici != null)
            {
                if (kullanici.Durum == KullaniciDurumTipleri.Pasif || kullanici.Durum == KullaniciDurumTipleri.Dondurulmus)
                {
                    throw new AccountPasiveException();
                }

                if (kullanici.SifreDurumKodu == KullaniciSifreDurumTipleri.Kilitli)
                {
                    throw new AccountLockedException();
                }

                string hashedPassword = Encryption.HashPassword(sifre);

                if (hashedPassword == kullanici.Sifre)
                {
                    validated = true;
                }

                //Kullanıcı onaylanmadığında, eğer son 30 dakika içinde TVM hatalı giriş sayısından 
                //fazla yanlış şifre girildi is kullanıcı hesabı kilitlenir.
                if (!validated)
                {
                    TVMDetay tvmDetay = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);

                    if (kullanici.HataliSifreGirisTarihi.HasValue)
                    {
                        if (kullanici.HataliSifreGirisTarihi.Value > TurkeyDateTime.Now.AddMinutes(-30))
                        {
                            if (kullanici.HataliSifreGirisSayisi.HasValue && kullanici.HataliSifreGirisSayisi.Value >= tvmDetay.SifreKontralSayisi)
                            {
                                kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.Kilitli;
                                kullanici.HataliSifreGirisTarihi = null;
                                kullanici.HataliSifreGirisSayisi = 0;
                            }
                            else
                            {
                                kullanici.HataliSifreGirisSayisi++;
                            }
                        }
                        else
                        {
                            kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                            kullanici.HataliSifreGirisSayisi = 1;
                            kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                        }
                    }
                    else
                    {
                        kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                        kullanici.HataliSifreGirisSayisi = 1;
                        kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                    }
                }
                else
                {
                    kullanici.HataliSifreGirisTarihi = null;
                    kullanici.HataliSifreGirisSayisi = 0;
                    kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.SorunYok;
                }
            }

            _TVMContext.Commit();

            return validated;
        }

        public bool ValidateUserAEGON_SSO(string USER_NAME, string SESSION_TOKEN, string CHECKSUM)
        {
            bool result = false;

            try
            {
                #region Validate

                TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == USER_NAME).FirstOrDefault();

                if (kullanici != null)
                {
                    if (kullanici.Durum != KullaniciDurumTipleri.Aktif)
                    {
                        throw new AccountPasiveException();
                    }

                    if (kullanici.SifreDurumKodu == KullaniciSifreDurumTipleri.Kilitli)
                    {
                        throw new AccountLockedException();
                    }

                    #region CHECK CHECKSUM

                    IKonfigurasyonService _konfig = DependencyResolver.Current.GetService<IKonfigurasyonService>();
                    KonfigTable konfig = _konfig.GetKonfig(Konfig.BundleAEGON_SSO_LOGIN);

                    if (konfig != null)
                    {
                        string secret = konfig[Konfig.AEGON_SSO_LOGIN_Secret];
                        string input = String.Format("{0}{1}{2}{3}", SESSION_TOKEN, USER_NAME, GetClientIP(), secret);
                        string md5 = MD5Helper.GetMd5Hash(input);

                        // Comparer
                        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                        if (0 == comparer.Compare(md5, CHECKSUM))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception)
            { }

            return result;
        }

        public bool ValidateUserMAPFRE(string personelKodu, string sifre)
        {
            bool validated = false;
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == personelKodu).FirstOrDefault();

            if (kullanici != null)
            {
                if (kullanici.Durum == KullaniciDurumTipleri.Pasif || kullanici.Durum == KullaniciDurumTipleri.Dondurulmus)
                {
                    throw new AccountPasiveException();
                }

                if (kullanici.SifreDurumKodu == KullaniciSifreDurumTipleri.Kilitli)
                {
                    throw new AccountLockedException();
                }

                string hashedPassword = Encryption.HashPassword(sifre);

                if (hashedPassword == kullanici.Sifre)
                {
                    validated = true;
                }

                //Kullanıcı onaylanmadığında, eğer son 30 dakika içinde TVM hatalı giriş sayısından 
                //fazla yanlış şifre girildi is kullanıcı hesabı kilitlenir.
                if (!validated)
                {
                    TVMDetay tvmDetay = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);

                    if (kullanici.HataliSifreGirisTarihi.HasValue)
                    {
                        if (kullanici.HataliSifreGirisTarihi.Value > TurkeyDateTime.Now.AddMinutes(-30))
                        {
                            if (kullanici.HataliSifreGirisSayisi.HasValue && kullanici.HataliSifreGirisSayisi.Value >= tvmDetay.SifreKontralSayisi)
                            {
                                kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.Kilitli;
                                kullanici.HataliSifreGirisTarihi = null;
                                kullanici.HataliSifreGirisSayisi = 0;
                            }
                            else
                            {
                                kullanici.HataliSifreGirisSayisi++;
                            }
                        }
                        else
                        {
                            kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                            kullanici.HataliSifreGirisSayisi = 1;
                            kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                        }
                    }
                    else
                    {
                        kullanici.HataliSifreGirisTarihi = TurkeyDateTime.Now;
                        kullanici.HataliSifreGirisSayisi = 1;
                        kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.YanlisGiris;
                    }
                }
                else
                {
                    kullanici.HataliSifreGirisTarihi = null;
                    kullanici.HataliSifreGirisSayisi = 0;
                    kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.SorunYok;
                }
            }

            _TVMContext.Commit();

            return validated;
        }

        public void LockUser(int kullaniciKodu)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Find(f => f.KullaniciKodu == kullaniciKodu);
            if (kullanici != null)
            {
                kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.Kilitli;
                _TVMContext.Commit();
            }
        }

        public void UnlockUser(int kullaniciKodu)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Find(f => f.KullaniciKodu == kullaniciKodu);
            if (kullanici != null)
            {
                kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.SorunYok;
                _TVMContext.Commit();
            }
        }

        private static string GetClientIP()
        {
            if (System.Web.HttpContext.Current != null)
            {
                string ip = System.Web.HttpContext.Current.Request.UserHostAddress;

                if (String.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (String.IsNullOrEmpty(ip))
                        ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }

            return String.Empty;
        }

        #endregion
    }

    public class AccountPasiveException : ApplicationException
    {

    }

    public class AccountLockedException : ApplicationException
    {

    }

    public class MapfreValidationException : ApplicationException
    {
        public MapfreValidationException(string message)
            : base(message)
        {

        }
    }

    public class MapfrePartajException : ApplicationException
    {

    }
}
