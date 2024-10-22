using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITVMBolgeleriService
    {
        TVMBolgeleri GetTVMBolge(int bolgeKodu);
        IQueryable<TVMBolgeleri> GetList();
        IQueryable<TVMBolgeleri> GetListByTVM(int tvmKodu);
        TVMBolgeleri CreateItem(TVMBolgeleri bolge);
        bool UpdateItem(TVMBolgeleri bolge);
    }
}
