using System.Collections.Generic;
using System.Numerics;

namespace Suinet.Rpc.Types
{
    public class RawData
    {
        public MoveObjectData MoveObject { get; set; }

        public PackageData Package { get; set; }
    }

    public class MoveObjectData
    {
        public string BcsBytes { get; set; }

        public string DataType { get; set; }

        public bool HasPublicTransfer { get; set; }

        public string Type { get; set; }

        public BigInteger Version { get; set; }
    }

    public class PackageData
    {
        public string DataType { get; set; }

        public string Id { get; set; }

        public Dictionary<string, UpgradeInfo> LinkageTable { get; set; }

        public Dictionary<string, string> ModuleMap { get; set; }

        public List<TypeOrigin> TypeOriginTable { get; set; }

        public BigInteger Version { get; set; }
    }

}
