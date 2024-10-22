using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITVMDepartmanlarService
    {
        TVMDepartmanlar GetTVMDepartman(int departmanKodu);
        IQueryable<TVMDepartmanlar> GetList();
        TVMDepartmanlar CreateItem(TVMDepartmanlar departman);
        bool UpdateItem(TVMDepartmanlar departman);
    }
}
