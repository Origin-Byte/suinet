using Suinet.Rpc.Types;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Suinet.Rpc.Api
{
    // keeping it here while to deprecated entirely
    public interface ITransactionBuilderApi
    {
        Task<RpcResult<TransactionBlockBytes>> MoveCallAsync(MoveCallTransaction transactionParams);

        Task<RpcResult<TransactionBlockBytes>> MoveCallAsync(string signer, string packageObjectId, string module, string function, IEnumerable<string> typeArguments, IEnumerable<object> arguments, BigInteger gasBudget, string gas = null);
    }
}
