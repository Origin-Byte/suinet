using System.Numerics;

namespace Suinet.Rpc.Types.MoveTypes
{
    public enum SuiCallArgType
    {
        Object,
        Pure
    }

    public enum SuiObjectType
    {
        ImmOrOwnedObject,
        SharedObject
    }

    public abstract class SuiCallArg
    {
        public SuiCallArgType Type { get; set; }
    }

    public class ObjectSuiCallArg : SuiCallArg
    {
        public string Digest { get; set; }
        public string ObjectId { get; set; }
        public SuiObjectType ObjectType { get; set; }
        public BigInteger Version { get; set; }
        public BigInteger InitialSharedVersion { get; set; }
        public bool? Mutable { get; set; }

        public ObjectSuiCallArg()
        {
            Type = SuiCallArgType.Object;
        }
    }

    public class PureSuiCallArg : SuiCallArg
    {
        public object Value { get; set; }
        public string ValueType { get; set; }

        public PureSuiCallArg()
        {
            Type = SuiCallArgType.Pure;
        }
    }
}
