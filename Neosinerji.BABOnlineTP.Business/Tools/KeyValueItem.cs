using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KeyValueItem<TKey, TValue>
    {
        public KeyValueItem()
        {

        }

        public KeyValueItem(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}
