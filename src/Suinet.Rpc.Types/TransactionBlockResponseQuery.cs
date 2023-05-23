using System;
using System.Collections.Generic;
using System.Text;

namespace Suinet.Rpc.Types
{
    public class TransactionBlockResponseQuery
    {
        public TransactionFilter Filter { get; set; }

        public TransactionBlockResponseOptions Options { get; set; }
    }
}
