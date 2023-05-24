using Newtonsoft.Json;
using System.Numerics;

namespace Suinet.Rpc.Types.MoveTypes
{
    [MoveType("coin::Coin<0x2::sui::SUI>")]
    public class SUICoin 
    {
        [JsonProperty("id")]
        public UID Id { get; set; }

        [JsonProperty("balance")]
        public BigInteger Balance { get; set; }
    }
}
