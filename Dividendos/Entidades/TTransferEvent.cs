using System;
using System.Collections.Generic;

#nullable disable

namespace Dividendos.Entidades
{
    public partial class TTransferEvent
    {
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? FechaLectura { get; set; }
        public long? Valor { get; set; }
        public long? Bloque { get; set; }
    }
}
