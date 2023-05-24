using Suinet.Rpc.Api;
using Suinet.Rpc.Types;
using Suinet.Rpc.Types.MoveTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Suinet.Rpc
{
    /// <summary>
    /// Generic methods handling type conversions
    /// </summary>
    public partial class SuiJsonRpcApiClient : IGenericReadApi
    {
        public async Task<RpcResult<T>> GetObjectAsync<T>(string objectId) where T : class
        {
            var options = new ObjectDataOptions()
            {
                ShowContent = true,
                ShowType = true
            };
            var result = await GetObjectAsync(objectId, options);

            if (result.Result.Data.Content is MoveObjectData moveObjectData)
            {
                var typedObject = moveObjectData.ConvertFieldsTo<T>();

                return new RpcResult<T>
                {
                    IsSuccess = result.IsSuccess,
                    RawRpcRequest = result.RawRpcRequest,
                    RawRpcResponse = result.RawRpcResponse,
                    ErrorMessage = result.ErrorMessage,
                    Result = typedObject
                };
            }

            return new RpcResult<T>
            {
                IsSuccess = false,
                RawRpcRequest = result.RawRpcRequest,
                RawRpcResponse = result.RawRpcResponse,
                ErrorMessage = result.ErrorMessage,
                Result = null
            };
        }

        public async Task<RpcResult<IEnumerable<T>>> GetObjectsOwnedByAddressAsync<T>(string address, string cursor, ulong? limit) where T : class
        {
            var filter = ObjectDataFilterFactory.CreateMatchAllFilter(ObjectDataFilterFactory.CreateAddressOwnerFilter(address));
            var rpcresult = await GetOwnedObjectsAsync(address, new ObjectResponseQuery() { Filter = filter }, cursor, limit);
            var objectsOwnedByAddress = new RpcResult<IEnumerable<T>>
            {
                IsSuccess = rpcresult.IsSuccess,
                RawRpcRequest = rpcresult.RawRpcRequest,
                RawRpcResponse = rpcresult.RawRpcResponse,
                ErrorMessage = rpcresult.ErrorMessage,
            };

            if (rpcresult.IsSuccess)
            {
                objectsOwnedByAddress.Result = await GetObjectsAsync<T>(rpcresult.Result.Data.Select(d => d.Data.ObjectId));
            }

            return objectsOwnedByAddress;
        }

        public async Task<RpcResult<IEnumerable<T>>> GetOwnedObjectsAsync<T>(string address, ObjectResponseQuery query, string cursor, ulong? limit) where T : class
        {
            var rpcresult = await GetOwnedObjectsAsync(address, query, cursor, limit);

            var objectsOwned = new RpcResult<IEnumerable<T>>
            {
                IsSuccess = rpcresult.IsSuccess,
                RawRpcRequest = rpcresult.RawRpcRequest,
                RawRpcResponse = rpcresult.RawRpcResponse,
                ErrorMessage = rpcresult.ErrorMessage,
            };

            if (rpcresult.IsSuccess)
            {
                objectsOwned.Result = await GetObjectsAsync<T>(rpcresult.Result.Data.Select(d => d.Data.ObjectId));
            }

            return objectsOwned;
        }

        public async Task<IEnumerable<T>> GetObjectsAsync<T>(IEnumerable<string> objectIds) where T : class
        {
            var typedObjects = new List<T>(objectIds.Count());
            foreach (var objectId in objectIds)
            {
                var obj = await GetObjectAsync<T>(objectId);

                if (obj.IsSuccess)
                {
                    typedObjects.Add(obj.Result);
                }
            }

            return typedObjects;
        }

        public async Task<RpcResult<T>> GetDynamicFieldObjectAsync<T>(string parentObjectId, string fieldName) where T : class
        {
            var result = await GetDynamicFieldObjectAsync(parentObjectId, fieldName);

            return null;
            //var dynamicFieldResult = result.Result.Object.Data.Fields.ToObject<DynamicField>();

            //return new RpcResult<T>
            //{
            //    IsSuccess = result.IsSuccess,
            //    RawRpcRequest = result.RawRpcRequest,
            //    RawRpcResponse = result.RawRpcResponse,
            //    ErrorMessage = result.ErrorMessage,
            //    Result = result.IsSuccess ? dynamicFieldResult.Value.Fields.ToObject<T>() : null
            //};
        }

    }
}
