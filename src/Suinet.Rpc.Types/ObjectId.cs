namespace Suinet.Rpc.Types
{
    public class ObjectId
    {
        private readonly string value;

        public ObjectId(string value)
        {
            this.value = value;
        }

        public static implicit operator ObjectId(string value)
        {
            return new ObjectId(value);
        }

        public static implicit operator string(ObjectId objectId)
        {
            return objectId.value;
        }
    }
}
