using System.Collections.Generic;

namespace Suinet.Rpc.Types.MoveTypes
{
    public abstract class MoveValue
    {
    }

    public class IntegerMoveValue : MoveValue
    {
        public uint Value { get; set; }
    }

    public class BooleanMoveValue : MoveValue
    {
        public bool Value { get; set; }
    }

    public class StringMoveValue : MoveValue
    {
        public string Value { get; set; }
    }

    public class SuiAddressMoveValue : MoveValue
    {
        public string Value { get; set; }
    }

    public class ObjectIDMoveValue : MoveValue
    {
        public ObjectId Id { get; set; }
    }

    public class MoveStructMoveValue : MoveValue
    {
        public MoveStruct Value { get; set; }
    }

    public class ArrayMoveValue : MoveValue
    {
        public List<MoveValue> Value { get; set; }
    }
}
