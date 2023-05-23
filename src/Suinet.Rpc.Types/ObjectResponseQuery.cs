namespace Suinet.Rpc.Types
{
    public class ObjectResponseQuery
    {
        public SuiObjectDataFilter Filter { get; set; } = null;

        public ObjectDataOptions Options { get; set; } = null;
    }
}
