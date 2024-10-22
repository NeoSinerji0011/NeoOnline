using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITVMKullanicilarService
    {
        TVMKullanicilar GetTVMKullanici(int kullaniciKodu);
        IQueryable<TVMKullanicilar> GetList();
        TVMKullanicilar CreateItem(TVMKullanicilar kullanici);
        List<TVMKullanicilar> GetListTVMKullanicilarByTVMKodu(int tvmKodu);
        bool UpdateItem(TVMKullanicilar kullanici);
    }
}
