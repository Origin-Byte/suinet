using Suinet.Rpc.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suinet.Rpc.Api
{
    public interface IReadApi
    {
        /// <summary>
        /// Return the list of objects owned by an address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<SuiObjectInfo>>> GetObjectsOwnedByAddressAsync(string address);

        /// <summary>
        /// Return the list of objects owned by an object.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<SuiObjectInfo>>> GetObjectsOwnedByObjectAsync(string objectId);

        /// <summary>
        /// Return the total number of transactions known to the server.
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<ulong>> GetTotalTransactionBlocksAsync();

        /// <summary>
        /// Return list of recent transaction digest.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<(ulong, string)>>> GetRecentTransactionsAsync(ulong count);

        /// <summary>
        /// Return the transaction response object.
        /// </summary>
        /// <param name="digest"></param>
        /// <returns></returns>
        Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest, TransactionBlockResponseOptions options);

        /// <summary>
        /// Returns an ordered list of transaction responses The method will throw an error if the input contains any duplicate or the input size exceeds QUERY_MAX_RESULT_LIMIT
        /// </summary>
        /// <param name="digests"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<RpcResult<TransactionBlockResponse[]>> GetTransactionBlocksAsync(IEnumerable<string> digests, TransactionBlockResponseOptions options);

        /// <summary>
        /// Return the object information for a specified object
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task<RpcResult<SuiObjectResponse>> GetObjectAsync(string objectId, SuiObjectDataOptions options);

        /// <summary>
        /// Return a checkpoint
        /// </summary>
        /// <param name="id">Checkpoint identifier, can use either checkpoint digest, or checkpoint sequence number as input.</param>
        /// <returns></returns>
        Task<RpcResult<SuiCheckpoint>> GetCheckpointAsync(string id);

        /// <summary>
        /// Return paginated list of checkpoints
        /// </summary>
        /// <param name="cursor">An optional paging cursor. If provided, the query will start from the next item after the specified cursor. Default to start from the first item if not specified.</param>
        /// <param name="limit">Maximum item returned per page, default to [QUERY_MAX_RESULT_LIMIT_CHECKPOINTS] if not specified.</param>
        /// <param name="isDescending">query result ordering, default to false (ascending order), oldest record first.</param>
        /// <returns>CheckpointPage</returns>
        Task<RpcResult<SuiPage_for_Checkpoint_and_BigInt_for_uint64>> SuiGetCheckpointsAsync(string cursor, ulong limit, bool isDescending);

        /// <summary>
        /// Return transaction events.
        /// </summary>
        /// <param name="txDigest">The event query criteria.</param>
        /// <returns></returns>
        Task<RpcResult<SuiEvent[]>> GetEventAsync(string txDigest);

        /// <summary>
        /// Return the sequence number of the latest checkpoint that has been executed
        /// </summary>
        /// <param name="txDigest">The event query criteria.</param>
        /// <returns></returns>
        Task<RpcResult<ulong>> GetLatestCheckpointSequenceNumberAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txDigest"></param>
        /// <returns></returns>
        Task<RpcResult<SuiLoadedChildObjectsResponse>> GetLoadedChildObjectsAsync(string txDigest);
    }
}
