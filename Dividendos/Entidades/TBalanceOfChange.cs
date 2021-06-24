using System;
using System.Collections.Generic;

#nullable disable

namespace Dividendos.Entidades
{
    public partial class TBalanceOfChange
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long? ValueOld { get; set; }
        public long? ValueNew { get; set; }
        public DateTime? Date { get; set; }
        public long? Block { get; set; }

        public virtual TAddressContract AddressNavigation { get; set; }
    }
}
