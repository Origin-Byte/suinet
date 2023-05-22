using System.Collections.Generic;

namespace Suinet.Rpc.Types
{
    public class SuiPage_for_Event_and_EventID
    {
        public IEnumerable<SuiEventEnvelope> Data { get; set; }

        public SuiEventId NextCursor { get; set; }
    }
}
