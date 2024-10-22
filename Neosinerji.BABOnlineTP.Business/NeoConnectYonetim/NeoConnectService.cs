using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class NeoConnectService : INeoConnectService
    {

        IKomisyonContext _KomisyonContext;
        public NeoConnectService(IKomisyonContext komisyonContext)
        {
            _KomisyonContext = komisyonContext;
        }

        public List<NeoConnectTvmSirketYetkileri> getNeoConnectSirketYetkileri(int tvmKodu)
        {
            List<NeoConnectTvmSirketYetkileri> sirketYetkileri = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.All().Where(s => s.TvmKodu == tvmKodu).ToList();
            return sirketYetkileri;
        }
    }
}
