using Suinet.Rpc.Types.MoveTypes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Suinet.Rpc.Types
{
    public class ObjectData
    {
        public RawData Bcs { get; set; }

        public SuiData Content { get; set; }

        public string Digest { get; set; }

        public DisplayFieldsResponse Display { get; set; }

        public string ObjectId { get; set; }

        public Owner Owner { get; set; }

        public string PreviousTransaction { get; set; }

        public BigInteger StorageRebate { get; set; }

        public string Type { get; set; }

        public BigInteger Version { get; set; }
    }

}
