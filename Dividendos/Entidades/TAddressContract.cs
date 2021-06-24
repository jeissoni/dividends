using System;
using System.Collections.Generic;

#nullable disable

namespace Dividendos.Entidades
{
    public partial class TAddressContract
    {
        public TAddressContract()
        {
            TBalanceOfChanges = new HashSet<TBalanceOfChange>();
        }

        public string Account { get; set; }
        public long? Value { get; set; }

        public virtual ICollection<TBalanceOfChange> TBalanceOfChanges { get; set; }
    }
}
