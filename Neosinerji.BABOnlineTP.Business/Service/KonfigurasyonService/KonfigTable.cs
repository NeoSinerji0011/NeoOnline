using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KonfigTable
    {
        Konfigurasyon[] _Konfig;

        public KonfigTable(Konfigurasyon[] config)
        {
            _Konfig = config;
        }

        public string this[string index]
        {
            get 
            {
                if (_Konfig == null)
                    return String.Empty;

                Konfigurasyon konfig = _Konfig.FirstOrDefault(w => w.Kod == index);
                if (konfig != null)
                    return konfig.Deger;

                return String.Empty;
            }
        }

        public int Count
        {
            get
            {
                return this._Konfig.Length;
            }
        }
    }
}
