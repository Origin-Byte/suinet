using System.Collections.Generic;

namespace Suinet.Rpc.Types
{
    public class TransactionBlockResponse
    {
        public List<object> BalanceChanges { get; set; }

        public TransactionBlock Transaction { get; set; }

        public TransactionBlockEffects Effects { get; set; }

        public List<SuiEvent> Events { get; set; }

        public ulong TimestampMs { get; set; }

        public ulong Checkpoint { get; set; }

        public bool? ConfirmedLocalExecution { get; set; }

        public string Digest { get; set; }

        public List<SuiObjectChange> ObjectChanges;

        public List<string> Errors { get; set; }
    }
}
