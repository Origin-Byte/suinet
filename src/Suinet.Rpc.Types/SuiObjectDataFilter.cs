using Suinet.Rpc.Types.MoveTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Suinet.Rpc.Types
{
    public abstract class SuiObjectDataFilter
    {
    }

    public class MatchAllFilter : SuiObjectDataFilter
    {
        public List<SuiObjectDataFilter> MatchAll { get; set; }
    }

    public class MatchAnyFilter : SuiObjectDataFilter
    {
        public List<SuiObjectDataFilter> MatchAny { get; set; }
    }

    public class MatchNoneFilter : SuiObjectDataFilter
    {
        public List<SuiObjectDataFilter> MatchNone { get; set; }
    }

    public class PackageFilter : SuiObjectDataFilter
    {
        [Description("Query by type a specified Package.")]
        public ObjectId Package { get; set; }
    }

    public class MoveModuleFilter : SuiObjectDataFilter
    {
        [Description("Query by type a specified Move module.")]
        public MoveModule MoveModule { get; set; }
    }

    public class StructTypeFilter : SuiObjectDataFilter
    {
        [Description("Query by type")]
        public string StructType { get; set; }
    }

    public class AddressOwnerFilter : SuiObjectDataFilter
    {
        public string AddressOwner { get; set; }
    }

    public class ObjectOwnerFilter : SuiObjectDataFilter
    {
        public ObjectId ObjectOwner { get; set; }
    }

    public class ObjectIdFilter : SuiObjectDataFilter
    {
        public ObjectId ObjectId { get; set; }
    }

    public class ObjectIdsFilter : SuiObjectDataFilter
    {
        public List<ObjectId> ObjectIds { get; set; }
    }

    public class VersionFilter : SuiObjectDataFilter
    {
        public BigInteger Version { get; set; }
    }

}
