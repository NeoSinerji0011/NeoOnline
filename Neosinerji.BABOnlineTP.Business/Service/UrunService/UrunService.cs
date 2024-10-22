using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UrunService : IUrunService
    {
        IParametreContext _ParametreContext;

        public UrunService(IParametreContext parametreContext)
        {
            _ParametreContext = parametreContext;
        }

        #region IUrunService Members

        public Urun GetUrun(int urunKodu)
        {
            Urun urn = _ParametreContext.UrunRepository.FindById(urunKodu);
            return urn;
        }

        public List<Urun> GetListUrun()
        {
            return _ParametreContext.UrunRepository.All().ToList<Urun>();
        }

        public List<UrunServiceModel> GetList()
        {
            IQueryable<Urun> urunler = _ParametreContext.UrunRepository.All();
            IQueryable<Bran> branslar = _ParametreContext.BranRepository.All();

            var urunModel = urunler.Join(branslar, u => u.BransKodu, b => b.BransKodu,
                                            (u, b) => new { u.UrunKodu, u.UrunAdi, u.BransKodu, u.Durum, b.BransAdi })
                                            .OrderBy(o => o.UrunKodu)
                                            .ToList()
                                            .Select(s => new UrunServiceModel()
                                            {
                                                UrunKodu = s.UrunKodu,
                                                UrunAdi = s.UrunAdi,
                                                BransKodu = s.BransKodu,
                                                Durum = s.Durum,
                                                BransAdi = s.BransAdi
                                            })
                                            .ToList<UrunServiceModel>();

            return urunModel;
        }

        public Urun CreateItem(Urun urun)
        {
            Urun urn = _ParametreContext.UrunRepository.Create(urun);
            _ParametreContext.Commit();
            return urn;
        }

        public bool UpdateItem(Urun urun)
        {
            _ParametreContext.UrunRepository.Update(urun);
            _ParametreContext.Commit();
            return true;
        }


        #endregion

        #region IUrunSoruService Members

        public List<UrunSoruServiceModel> GetUrunSorulari(int urunKodu)
        {
            IQueryable<Soru> sorular = _ParametreContext.SoruRepository.All();
            IQueryable<UrunSoru> urunSoru = _ParametreContext.UrunSoruRepository.Filter(f => f.UrunKodu == urunKodu);

            var urunSorulari = sorular.Join(urunSoru, u => u.SoruKodu, us => us.SoruKodu,
                                           (u, us) => new { u.SoruAdi, u.SoruCevapTipi, u.SoruKodu, us.UrunKodu, us.SiraNo })
                                           .OrderBy(o => o.SiraNo)
                                           .ToList()
                                           .Select(s => new UrunSoruServiceModel
                                                          {
                                                              UrunKodu = s.UrunKodu,
                                                              SoruKodu = s.SoruKodu,
                                                              SiraNo = s.SiraNo,
                                                              SoruAdi = s.SoruAdi,
                                                              SoruCevapTipi = s.SoruCevapTipi
                                                          })
                                           .ToList<UrunSoruServiceModel>();

            return urunSorulari;
        }


        public void AddSoru(int urunKodu, int soruKodu)
        {
            Urun urun = _ParametreContext.UrunRepository.FindById(urunKodu);
            int maxSiraNo = 1;
            if (urun.UrunSorus.Count > 0)
            {
                maxSiraNo = urun.UrunSorus.Max(s => s.SiraNo);
                maxSiraNo++;
            }

            UrunSoru vergi = new UrunSoru();
            vergi.UrunKodu = urunKodu;
            vergi.SoruKodu = soruKodu;
            vergi.SiraNo = maxSiraNo;

            urun.UrunSorus.Add(vergi);

            _ParametreContext.Commit();
        }

        public void DeleteSoru(int urunKodu, int soruKodu)
        {
            _ParametreContext.UrunSoruRepository.Delete(u => u.UrunKodu == urunKodu && u.SoruKodu == soruKodu);
            _ParametreContext.Commit();
        }


        #endregion

        #region IUrunTeminatService Members
        public List<UrunTeminatServiceModel> GetUrunTeminatlari(int urunKodu)
        {
            IQueryable<Teminat> teminatlar = _ParametreContext.TeminatRepository.All();
            IQueryable<UrunTeminat> urunTeminatlar = _ParametreContext.UrunTeminatRepository.Filter(f => f.UrunKodu == urunKodu);

            var teminatList = teminatlar.Join(urunTeminatlar, t => t.TeminatKodu, ut => ut.TeminatKodu,
                                                (t, ut) => new { ut.UrunKodu, t.TeminatKodu, t.TeminatAdi, ut.SiraNo })
                                                .OrderBy(o => o.SiraNo)
                                                .ToList()
                                                .Select(s => new UrunTeminatServiceModel()
                                                            {
                                                                UrunKodu = s.UrunKodu,
                                                                TeminatKodu = s.TeminatKodu,
                                                                TeminatAdi = s.TeminatAdi,
                                                                SiraNo = s.SiraNo
                                                            })
                                                .ToList<UrunTeminatServiceModel>();

            return teminatList;
        }

        public void AddTeminat(int urunKodu, int teminatKodu)
        {
            Urun urun = _ParametreContext.UrunRepository.FindById(urunKodu);
            int maxSiraNo = 1;
            if (urun.UrunTeminats.Count > 0)
            {
                maxSiraNo = urun.UrunTeminats.Max(s => s.SiraNo);
                maxSiraNo++;
            }

            UrunTeminat teminat = new UrunTeminat();
            teminat.UrunKodu = urunKodu;
            teminat.TeminatKodu = teminatKodu;
            teminat.SiraNo = maxSiraNo;

            urun.UrunTeminats.Add(teminat);

            _ParametreContext.Commit();
        }

        public void DeleteTeminat(int urunKodu, int teminatKodu)
        {
            _ParametreContext.UrunTeminatRepository.Delete(u => u.UrunKodu == urunKodu && u.TeminatKodu == teminatKodu);
            _ParametreContext.Commit();
        }
        #endregion

        #region IUrunVergiService

        public List<UrunVergiServiceModel> GetUrunVergileri(int urunKodu)
        {
            IQueryable<Vergi> Vergiler = _ParametreContext.VergiRepository.All();
            IQueryable<UrunVergi> UrunVergi = _ParametreContext.UrunVergiRepository.Filter(f => f.UrunKodu == urunKodu);

            var urunVergileri = Vergiler.Join(UrunVergi, v => v.VergiKodu, uv => uv.VergiKodu,
                                                      (v, uv) => new { v.VergiAdi, v.VergiKodu, uv.UrunKodu, uv.SiraNo })
                                                      .OrderBy(o => o.SiraNo)
                                                      .ToList()
                                                      .Select(s => new UrunVergiServiceModel()
                                                                {
                                                                    UrunKodu = s.UrunKodu,
                                                                    VergiKodu = s.VergiKodu,
                                                                    VergiAdi = s.VergiAdi,
                                                                    SiraNo = s.SiraNo
                                                                })
                                                      .ToList<UrunVergiServiceModel>();
            return urunVergileri;
        }

        public void AddVergi(int urunKodu, int vergiKodu)
        {
            Urun urun = _ParametreContext.UrunRepository.FindById(urunKodu);
            int maxSiraNo = 1;
            if (urun.UrunVergis.Count > 0)
            {
                maxSiraNo = urun.UrunVergis.Max(s => s.SiraNo);
                maxSiraNo++;
            }

            UrunVergi vergi = new UrunVergi();
            vergi.UrunKodu = urunKodu;
            vergi.VergiKodu = vergiKodu;
            vergi.SiraNo = maxSiraNo;

            urun.UrunVergis.Add(vergi);

            _ParametreContext.Commit();
        }

        public void DeleteVergi(int urunKodu, int vergiKodu)
        {
            _ParametreContext.UrunVergiRepository.Delete(u => u.UrunKodu == urunKodu && u.VergiKodu == vergiKodu);
            _ParametreContext.Commit();
        }


        #endregion

    }
}
