﻿using Chaos.NaCl;
using Suinet.Rpc.Api;
using Suinet.Rpc.Client;
using Suinet.Rpc.Http;
using Suinet.Rpc.JsonRpc;
using Suinet.Rpc.Types;
using Suinet.Rpc.Types.MoveTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Suinet.Rpc
{
    public partial class SuiJsonRpcApiClient : IReadApi, IExtendedApi, IWriteApi, IMoveUtils, IGovernanceReadApi, ICoinQueryApi, ITransactionBuilderApi
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

        public async Task<RpcResult<SuiObjectResponse>> GetDynamicFieldObjectAsync(string parentObjectId, DynamicFieldName fieldName)
        {
            return await SendRpcRequestAsync<SuiObjectResponse>("suix_getDynamicFieldObject", ArgumentBuilder.BuildArguments(parentObjectId, fieldName));
        }

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
        {
            return await SendRpcRequestAsync<BigInteger>("sui_getTotalTransactionBlocks");
        }

        public async Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest)
        {
            return await SendRpcRequestAsync<TransactionBlockResponse>("sui_getTransactionBlock", ArgumentBuilder.BuildArguments(digest));
        }

        public async Task<RpcResult<TransactionBlockBytes>> MoveCallAsync(string signer, string packageObjectId, string module, string function, IEnumerable<string> typeArguments, IEnumerable<object> arguments, BigInteger gasBudget, string gas = null)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("unsafe_moveCall", ArgumentBuilder.BuildArguments(signer, packageObjectId, module, function, typeArguments, SuiJsonSanitizer.SanitizeArguments(arguments), gas, gasBudget.ToString()));
        }

        public async Task<RpcResult<TransactionBlockBytes>> MoveCallAsync(MoveCallTransaction transactionParams)
        {
            return await MoveCallAsync(transactionParams.Signer, transactionParams.PackageObjectId, transactionParams.Module, transactionParams.Function, transactionParams.TypeArguments, transactionParams.Arguments, transactionParams.GasBudget, transactionParams.Gas);
        }

        public async Task<RpcResult<TransactionBlockBytes>> TransferObjectAsync(string signer, string objectId, string gas, ulong gasBudget, string recipient)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_transferObject", ArgumentBuilder.BuildArguments(signer, objectId, gas, gasBudget, recipient));
        }

        public async Task<RpcResult<TransactionBlockBytes>> TransferSuiAsync(string signer, string suiObjectId, ulong gasBudget, string recipient, ulong amount)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_transferSui", ArgumentBuilder.BuildArguments(signer, suiObjectId, gasBudget, recipient, amount));
        }

        public async Task<RpcResult<TransactionBlockBytes>> MergeCoinsAsync(string signer, string primaryCoinId, string coinToMergeId, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_mergeCoins", ArgumentBuilder.BuildArguments(signer, primaryCoinId, coinToMergeId, gas, gasBudget));
        }

        public async Task<RpcResult<TransactionBlockBytes>> SplitCoinAsync(string signer, string coinObjectId, IEnumerable<ulong> splitAmounts, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_splitCoin", ArgumentBuilder.BuildArguments(signer, coinObjectId, splitAmounts, gas, gasBudget));
        }

        public async Task<RpcResult<TransactionBlockBytes>> SplitCoinEqualAsync(string signer, string coinObjectId, ulong splitCount, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_splitCoinEqual", ArgumentBuilder.BuildArguments(signer, coinObjectId, splitCount, gas, gasBudget));
        }

        public async Task<RpcResult<TransactionBlockBytes>> PayAsync(string signer, IEnumerable<string> inputCoins, IEnumerable<string> recipients, IEnumerable<ulong> amounts, string gas, ulong gasBudget)
        {
            return await SendRpcRequestAsync<TransactionBlockBytes>("sui_pay", ArgumentBuilder.BuildArguments(signer, inputCoins, recipients, amounts, gas, gasBudget));
        }

        public async Task<RpcResult<SuiPage_for_Event_and_EventID>> QueryEventsAsync(ISuiEventQuery query, SuiEventId cursor, ulong limit, bool descendingOrder = false)
        {
            return await SendRpcRequestAsync<SuiPage_for_Event_and_EventID>("suix_queryEvents", ArgumentBuilder.BuildArguments(query, cursor, limit, descendingOrder));
        }

        public async Task<RpcResult<Page_for_DynamicFieldInfo_and_ObjectID>> GetDynamicFieldsAsync(string objectId)
        {
            return await SendRpcRequestAsync<Page_for_DynamicFieldInfo_and_ObjectID>("sui_getDynamicFields", ArgumentBuilder.BuildArguments(objectId));
        }

        public async Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest, TransactionBlockResponseOptions options)
        {
            return await SendRpcRequestAsync<TransactionBlockResponse>("sui_getTransactionBlock", ArgumentBuilder.BuildArguments(digest, options));
        }

        public Task<RpcResult<TransactionBlockResponse[]>> GetTransactionBlocksAsync(IEnumerable<string> digests, TransactionBlockResponseOptions options)
        {
            throw new NotImplementedException();
        }

        public async Task<RpcResult<IEnumerable<SuiObjectResponse>>> GetObjectsAsync(IEnumerable<string> objectIds, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<IEnumerable<SuiObjectResponse>>("sui_multiGetObjects", ArgumentBuilder.BuildArguments(objectIds, options));
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

        public async Task<RpcResult<Page_for_SuiObjectResponse_and_ObjectID>> GetOwnedObjectsAsync(string address, ObjectResponseQuery query, string cursor, ulong? limit)
        {
            return await SendRpcRequestAsync<Page_for_SuiObjectResponse_and_ObjectID>("suix_getOwnedObjects", ArgumentBuilder.BuildArguments(address, query, cursor, limit));
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

        public async Task<RpcResult<TransactionBlockResponse>> ExecuteTransactionBlockAsync(string txBytes, IEnumerable<string> signatures, IEnumerable<string> pubKeys, TransactionBlockResponseOptions options, ExecuteTransactionRequestType requestType)
        {
            var serializedSignatures = new List<string>();

            //for(int i = 0; i < signatures.Count(); i++)
            //{
            //    var signature = signatures.ElementAt(i);
            //    var pubKey = pubKeys.ElementAt(i);

            //    // Todo refact this logic from here
            //    var signatureBytes = CryptoBytes.FromBase64String(signature);
            //    var publicKeyBytes = CryptoBytes.FromBase64String(pubKey);
            //    var finalSignatureBytes = new byte[signatureBytes.Length + 1 + publicKeyBytes.Length];

            //    finalSignatureBytes[0] = SignatureSchemeToByte(SuiSignatureScheme.ED25519);
            //    Array.Copy(signatureBytes, 0, finalSignatureBytes, 1, signatureBytes.Length);
            //    Array.Copy(publicKeyBytes, 0, finalSignatureBytes, signatureBytes.Length + 1, publicKeyBytes.Length);
            //    var serializedSignature = CryptoBytes.ToBase64String(finalSignatureBytes);

            //    serializedSignatures.Add(serializedSignature);
            //}

            return await SendRpcRequestAsync<TransactionBlockResponse>("sui_executeTransactionBlock", ArgumentBuilder.BuildArguments(txBytes, serializedSignatures, options, requestType));
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

        public async Task<RpcResult<Page_for_DynamicFieldInfo_and_ObjectID>> GetDynamicFieldsAsync(string parentObjectId, string cursor, ulong? limit)
        {
            return await SendRpcRequestAsync<Page_for_DynamicFieldInfo_and_ObjectID>("suix_getDynamicFields", ArgumentBuilder.BuildArguments(parentObjectId, cursor, limit));
        }

        public Task<RpcResult<Balance[]>> GetAllBalancesAsync(string ownerAddress)
        {
            throw new NotImplementedException();
        }

        public Task<RpcResult<SuiPage_for_Coin_and_ObjectID>> GetAllCoinsAsync(string ownerAddress, string cursor, ulong limit)
        {
            throw new NotImplementedException();
        }

        public async Task<RpcResult<Balance>> GetBalanceAsync(string ownerAddress, string coinType)
        {
            return await SendRpcRequestAsync<Balance>("suix_getBalance", ArgumentBuilder.BuildArguments(ownerAddress, coinType));
        }
    }

    public partial class SuiJsonRpcApiClient : IJsonRpcApiClient
    { }
}
