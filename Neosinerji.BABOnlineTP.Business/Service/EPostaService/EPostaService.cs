using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class EPostaService : IEPostaService
    {
        IParametreContext _UnitOfWork;

        public EPostaService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        #region IEPostaService Members

        public EPostaFormatlari GetEPosta(string formatAdi)
        {
            return _UnitOfWork.EPostaFormatlariRepository.Filter(s => s.FormatAdi == formatAdi).First();
        }
        public EPostaFormatlari GetEPosta(int Id)
        {
            return _UnitOfWork.EPostaFormatlariRepository.FindById(Id);
        }

        public List<EPostaFormatlari> GetList()
        {
            return _UnitOfWork.EPostaFormatlariRepository.All().ToList<EPostaFormatlari>();
        }

        public EPostaFormatlari CreateItem(EPostaFormatlari EPosta)
        {
            EPostaFormatlari epsta = _UnitOfWork.EPostaFormatlariRepository.Create(EPosta);
            _UnitOfWork.Commit();
            return epsta;
        }

        public bool UpdateItem(EPostaFormatlari EPosta)
        {
            _UnitOfWork.EPostaFormatlariRepository.Update(EPosta);
            _UnitOfWork.Commit();
            return true;
        }

        public void DeleteFormat(int formatId)
        {
            _UnitOfWork.EPostaFormatlariRepository.Delete(m => m.FormatId == formatId);
            _UnitOfWork.Commit();
        }
        #endregion

    }
}
