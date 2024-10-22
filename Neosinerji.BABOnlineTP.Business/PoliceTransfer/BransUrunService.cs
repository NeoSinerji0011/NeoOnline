using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class BransUrunService : IBransUrunService
    {
        IParametreContext _ParametreContext;

        [InjectionConstructor]
        public BransUrunService(IParametreContext parametreContext)
        {
            _ParametreContext = parametreContext;

        }
        public List<BransUrun> getSigortaSirketiBransList(string sigortaSirketiKodu)
        {
            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();

            SigortaSirketiBransList = _ParametreContext.BransUrunRepository.Filter(s => s.SigortaSirketBirlikKodu == sigortaSirketiKodu).ToList<BransUrun>();
            if (SigortaSirketiBransList.Count > 0)
                return SigortaSirketiBransList;
            else return null;
        }
        public List<Bran> getBranslar()
        {
            List<Bran> branslar = new List<Bran>();
            branslar = _ParametreContext.BranRepository.All().ToList<Bran>();

            return branslar;
        }
    }
}
