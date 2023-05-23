namespace Suinet.Rpc.Types
{
    public class TransactionBlockResponseOptions
    {
        public bool ShowBalanceChanges { get; set; } = false;

        public bool ShowEffects { get; set; } = false;

        public bool ShowEvents { get; set; } = false;

        public bool ShowInput { get; set; } = false;

        public bool ShowObjectChanges { get; set; } = false;

        public bool ShowRawInput { get; set; } = false;
    }
}
