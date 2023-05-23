using Newtonsoft.Json;
using Suinet.Rpc.Types.MoveTypes;
using System.Collections.Generic;

namespace Suinet.Rpc.Types
{
    public class SuiData
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("fields")]
        public MoveStruct Fields { get; set; }

        [JsonProperty("hasPublicTransfer")]
        public bool HasPublicTransfer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("disassembled")]
        public Dictionary<string, object> Disassembled { get; set; }          
    }
}
