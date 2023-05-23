using Chaos.NaCl;
using Suinet.Rpc.Api;
using Suinet.Rpc.Client;
using Suinet.Rpc.Http;
using Suinet.Rpc.JsonRpc;
using Suinet.Rpc.Signer;
using Suinet.Rpc.Types;
using Suinet.Rpc.Types.MoveTypes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Suinet.Rpc
{
    public partial class SuiJsonRpcApiClient : IReadApi, IExtendedApi, IWriteApi, IMoveUtils, IGovernanceReadApi, ICoinQueryApi
    {
        private readonly IRpcClient _rpcClient;

        public SuiJsonRpcApiClient(IRpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        private JsonRpcRequest BuildRequest<T>(string method, IEnumerable<object> @params)
        {
            return new JsonRpcRequest(method, @params);
        }

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method)
        {
            var request = BuildRequest<T>(method, null);
            return await _rpcClient.SendAsync<T>(request);
        }

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method, IEnumerable<object> @params)
        {
            var request = BuildRequest<T>(method, @params);
            return await _rpcClient.SendAsync<T>(request);
        }

        //public async Task<RpcResult<SuiExecuteTransactionResponse>> ExecuteTransactionAsync(string txBytes, SuiSignatureScheme sigScheme, string signature, string pubKey, SuiExecuteTransactionRequestType suiExecuteTransactionRequestType)
        //{
        //    // Todo refact this logic from here
        //    var signatureBytes = CryptoBytes.FromBase64String(signature);
        //    var publicKeyBytes = CryptoBytes.FromBase64String(pubKey);
        //    var finalSignatureBytes = new byte[signatureBytes.Length + 1 + publicKeyBytes.Length];

        //    finalSignatureBytes[0] = SignatureSchemeToByte(sigScheme);
        //    Array.Copy(signatureBytes, 0, finalSignatureBytes, 1, signatureBytes.Length);
        //    Array.Copy(publicKeyBytes, 0, finalSignatureBytes, signatureBytes.Length + 1, publicKeyBytes.Length);

        //    var serializedSignature = CryptoBytes.ToBase64String(finalSignatureBytes);

        //    return await SendRpcRequestAsync<SuiExecuteTransactionResponse>("sui_executeTransaction", ArgumentBuilder.BuildArguments(txBytes, serializedSignature, suiExecuteTransactionRequestType));
        //}

        byte SignatureSchemeToByte(SuiSignatureScheme suiSignatureScheme)
        {
            if (suiSignatureScheme == SuiSignatureScheme.ED25519) return 0;

            return 1;
        }

        public async Task<RpcResult<SuiObjectRead>> GetDynamicFieldObjectAsync(string parentObjectId, string fieldName)
        {
            return await SendRpcRequestAsync<SuiObjectRead>("sui_getDynamicFieldObject", ArgumentBuilder.BuildArguments(parentObjectId, fieldName));
        }

        public async Task<RpcResult<IEnumerable<SuiObjectInfo>>> GetObjectsOwnedByObjectAsync(string objectId)
        {
            return await SendRpcRequestAsync<IEnumerable<SuiObjectInfo>>("sui_getObjectsOwnedByObject", ArgumentBuilder.BuildArguments(objectId));
        }

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
        {
            return await SendRpcRequestAsync<BigInteger>("sui_getTotalTransactionBlocks");
        }

        public async Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest)
        {
            return await SendRpcRequestAsync<TransactionBlockResponse>("sui_getTransactionBlock", ArgumentBuilder.BuildArguments(digest));
        }

        public async Task<RpcResult<SuiTransactionBytes>> MoveCallAsync(string signer, string packageObjectId, string module, string function, IEnumerable<string> typeArguments, IEnumerable<object> arguments, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_moveCall", ArgumentBuilder.BuildArguments(signer, packageObjectId, module, function, typeArguments, SuiJsonSanitizer.SanitizeArguments(arguments), gas, gasBudget));
        }

        public async Task<RpcResult<SuiTransactionBytes>> MoveCallAsync(MoveCallTransaction transactionParams)
        {
            return await MoveCallAsync(transactionParams.Signer, transactionParams.PackageObjectId, transactionParams.Module, transactionParams.Function, transactionParams.TypeArguments, transactionParams.Arguments, transactionParams.Gas, transactionParams.GasBudget);
        }

        public async Task<RpcResult<SuiTransactionBytes>> TransferObjectAsync(string signer, string objectId, string gas, ulong gasBudget, string recipient)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_transferObject", ArgumentBuilder.BuildArguments(signer, objectId, gas, gasBudget, recipient));
        }

        public async Task<RpcResult<SuiTransactionBytes>> TransferSuiAsync(string signer, string suiObjectId, ulong gasBudget, string recipient, ulong amount)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_transferSui", ArgumentBuilder.BuildArguments(signer, suiObjectId, gasBudget, recipient, amount));
        }

        public async Task<RpcResult<SuiTransactionBytes>> MergeCoinsAsync(string signer, string primaryCoinId, string coinToMergeId, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_mergeCoins", ArgumentBuilder.BuildArguments(signer, primaryCoinId, coinToMergeId, gas, gasBudget));
        }

        public async Task<RpcResult<SuiTransactionBytes>> SplitCoinAsync(string signer, string coinObjectId, IEnumerable<ulong> splitAmounts, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_splitCoin", ArgumentBuilder.BuildArguments(signer, coinObjectId, splitAmounts, gas, gasBudget));
        }

        public async Task<RpcResult<SuiTransactionBytes>> SplitCoinEqualAsync(string signer, string coinObjectId, ulong splitCount, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_splitCoinEqual", ArgumentBuilder.BuildArguments(signer, coinObjectId, splitCount, gas, gasBudget));
        }

        public async Task<RpcResult<SuiTransactionBytes>> PayAsync(string signer, IEnumerable<string> inputCoins, IEnumerable<string> recipients, IEnumerable<ulong> amounts, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<SuiTransactionBytes>("sui_pay", ArgumentBuilder.BuildArguments(signer, inputCoins, recipients, amounts, gas, gasBudget));
        }

        public async Task<RpcResult<SuiPage_for_Event_and_EventID>> QueryEventsAsync(ISuiEventQuery query, SuiEventId cursor, ulong limit, bool descendingOrder = false)
        {
            return await SendRpcRequestAsync<SuiPage_for_Event_and_EventID>("suix_queryEvents", ArgumentBuilder.BuildArguments(query, cursor, limit, descendingOrder));
        }

        public async Task<RpcResult<SuiPage_for_DynamicFieldInfo_and_ObjectID>> GetDynamicFieldsAsync(string objectId)
        {
            return await SendRpcRequestAsync<SuiPage_for_DynamicFieldInfo_and_ObjectID>("sui_getDynamicFields", ArgumentBuilder.BuildArguments(objectId));
        }

        public Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest, TransactionBlockResponseOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<TransactionBlockResponse[]>> GetTransactionBlocksAsync(IEnumerable<string> digests, TransactionBlockResponseOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiObjectResponse>> GetObjectsAsync(IEnumerable<string> objectIds, ObjectDataOptions options)
        {
            throw new NotImplementedException();
        }

        public async Task<RpcResult<SuiObjectResponse>> GetObjectAsync(string objectId, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<SuiObjectResponse>("sui_getObject", ArgumentBuilder.BuildArguments(objectId, options));
        }

        public Task<RpcResult<SuiCheckpoint>> GetCheckpointAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_Checkpoint_and_BigInt_for_uint64>> SuiGetCheckpointsAsync(string cursor, ulong limit, bool isDescending)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiEvent[]>> GetEventAsync(string txDigest)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<ulong>> GetLatestCheckpointSequenceNumberAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiLoadedChildObjectsResponse>> GetLoadedChildObjectsAsync(string txDigest)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_DynamicFieldInfo_and_ObjectID>> GetDynamicFieldsAsync(string parentObjectId, string cursor, ulong limit)
        {
            throw new NotImplementedException();
        }

        Task<RpcResult<SuiObjectResponse>> IExtendedApi.GetDynamicFieldObjectAsync(string parentObjectId, string fieldName)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<IEnumerable<SuiObjectInfo>>> GetOwnedObjectsAsync(string address, ObjectResponseQuery query, string cursor, ulong? limit)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_TransactionBlockResponse_and_TransactionDigest>> QueryTransactionBlocksAsync(TransactionBlockResponseQuery query, SuiEventId cursor, ulong? limit, bool? descendingOrder = false)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<string>> ResolveNameServiceAddressAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiDevInspectResults>> DevInspectTransactionBlockAsync(string senderAddress, string txBytes, ulong? gasPrice, ulong? epoch)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiDryRunTransactionBlockResponse>> DryRunTransactionBlockAsync(string txBytes)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<TransactionBlockResponse>> ExecuteTransactionBlockAsync(string txBytes, IEnumerable<string> signatures, SuiExecuteTransactionRequestType requestType, TransactionBlockResponseOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<MoveFunctionArgType[]>> GetMoveFunctionArgTypesAsync(string packageId, string module, string function)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiMoveNormalizedFunction>> GetNormalizedMoveFunctionAsync(string packageId, string module, string function)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModuleAsync(string packageId, string module)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackageAsync(string packageId)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStructAsync(string packageId, string module, string structName)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiSystemStateSummary>> GetLatestSuiSystemStateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<ulong>> GetReferenceGasPriceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiDelegatedStake>> GetStakesAsync(string address)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiDelegatedStake[]>> GetStakesByIdsAsync(IEnumerable<string> objectIds)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<ValidatorApys>> GetValidatorsApy()
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_DynamicFieldInfo_and_ObjectID>> GetDynamicFieldsAsync(string parentObjectId, string cursor, ulong? limit)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<BigInteger[]>> GetAllBalancesAsync(string ownerAddress)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_Coin_and_ObjectID>> GetAllCoinsAsync(string ownerAddress, string cursor, ulong limit)
        {
            throw new NotImplementedException();
        }

        public async Task<RpcResult<BigInteger>> GetBalanceAsync(string ownerAddress, string coinType)
        {
            return await SendRpcRequestAsync<BigInteger>("suix_getBalance", ArgumentBuilder.BuildArguments(ownerAddress, coinType));
        }
    }

    public partial class SuiJsonRpcApiClient : IJsonRpcApiClient
    { }
}
