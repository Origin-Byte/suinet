using Newtonsoft.Json;
using Suinet.Rpc.Types.JsonConverters;

namespace Suinet.Rpc.Types
{
    public class TransactionBlockData
    {
        public GasData GasData { get; set; }

        public string MessageVersion { get; set; }

        public string Sender { get; set; }

        [JsonConverter(typeof(TransactionBlockKindJsonConverter))]
        public TransactionBlockKind Transaction { get; set; }

        public TransactionBlockData()
        {
            MessageVersion = "v1"; 
        }
    }
}
