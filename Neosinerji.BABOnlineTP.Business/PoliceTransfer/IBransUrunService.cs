using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IBransUrunService
    {
        List<BransUrun> getSigortaSirketiBransList(string sigortaSirketiKodu);
        List<Bran> getBranslar();
    }
}
