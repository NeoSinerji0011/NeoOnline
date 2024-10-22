using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IEPostaService
    {
        EPostaFormatlari GetEPosta(string formatAdi);
        EPostaFormatlari GetEPosta(int Id);
        List<EPostaFormatlari> GetList();
        EPostaFormatlari CreateItem(EPostaFormatlari eposta);
        bool UpdateItem(EPostaFormatlari eposta);
        void DeleteFormat(int formatId);
    }
}
