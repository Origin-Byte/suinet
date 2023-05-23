using System;
using System.Collections.Generic;
using System.Text;

namespace Suinet.Rpc.Types
{
    public class SuiObjectResponse
    {
        public ObjectData Data { get; set; }
        public ObjectResponseError Error { get; set; }
    }

}
