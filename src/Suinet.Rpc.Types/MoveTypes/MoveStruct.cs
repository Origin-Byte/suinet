using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Suinet.Rpc.Types.MoveTypes
{
    public abstract class MoveStruct
    {
        public T ToObject<T>()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception e)
            {
                var a = 5;
            }
            return default(T);
        }
    }

    public class ArrayMoveStruct : MoveStruct
    {
        public List<MoveValue> Value { get; set; }
    }

    public class ObjectMoveStruct : MoveStruct
    {
        public Dictionary<string, MoveValue> Fields { get; set; }
        public string Type { get; set; }
    }

    public class AdditionalPropertiesMoveStruct : MoveStruct
    {
        public Dictionary<string, MoveValue> AdditionalProperties { get; set; }
    }
}
