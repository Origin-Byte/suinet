using Newtonsoft.Json;
using Suinet.Rpc.Types.JsonConverters;
using System.Collections.Generic;
using System.Numerics;

namespace Suinet.Rpc.Types
{
    [JsonConverter(typeof(ObjectChangeConverter))]
    public abstract class ObjectChange
    {
        public string Digest { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }

    public class PublishedObjectChange : ObjectChange
    {
        public List<string> Modules { get; set; }
        public string PackageId { get; set; }
    }

    public class TransferredObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
    }

    public class MutatedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Owner Owner { get; set; }
        public BigInteger PreviousVersion { get; set; }
        public string Sender { get; set; }
    }

    public class DeletedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Sender { get; set; }
    }

    public class WrappedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Sender { get; set; }
    }

    public class CreatedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Owner Owner { get; set; }
        public string Sender { get; set; }
    }
}
