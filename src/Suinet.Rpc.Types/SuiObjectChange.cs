using System.Collections.Generic;

namespace Suinet.Rpc.Types
{
    public abstract class SuiObjectChange
    {
        public string Digest { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Sender { get; set; }
        public SuiObjectChangeType Type { get; set; }
        public ulong Version { get; set; }
    }

    public class ModulePublished : SuiObjectChange
    {
        public List<string> Modules { get; set; }
        public string PackageId { get; set; }
    }

    public class TransferObject : SuiObjectChange
    {
        public Owner Recipient { get; set; }
    }

    public class ObjectMutated : SuiObjectChange
    {
        public Owner Owner { get; set; }
        public ulong PreviousVersion { get; set; }
    }

    public class DeleteObject : SuiObjectChange
    {
    }

    public class WrappedObject : SuiObjectChange
    {
    }

    public class NewObject : SuiObjectChange
    {
        public Owner Owner { get; set; }
    }
}
