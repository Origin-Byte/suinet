using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Suinet.NftProtocol.Domains;
using Suinet.Rpc.Client;
using Suinet.Rpc.Http;
using Suinet.Rpc.Types;
using Suinet.Rpc.Types.MoveTypes;
using Suinet.Wallet;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Suinet.Rpc.Tests
{
    public class SuiJsonRpcApiClientTests
    {
        private readonly ILogger<SuiJsonRpcApiClientTests> _logger;
        private readonly IJsonRpcApiClient _jsonRpcApiClient;
        private readonly ITestOutputHelper _output;
        private readonly IKeyPair _signerKeyPair;
        private readonly IKeyPair _signerKeyPair2;

        private const string TEST_MNEMONIC = "that august urban math slender industry area mountain worry day ski hold";

        private const string TEST_MNEMONIC2 = "bus indicate leave science minor clip embrace faculty wink industry addict track soup burger scissors another enrich muscle loop fever vacuum buyer paddle roof";

        public SuiJsonRpcApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            var serilogOutputLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
            .CreateLogger()
            .ForContext<SuiJsonRpcApiClientTests>();
            var serilogLoggerFactory = new SerilogLoggerFactory(serilogOutputLogger);
            _logger = serilogLoggerFactory.CreateLogger<SuiJsonRpcApiClientTests>();

            var rpcClient = new RpcClient(SuiConstants.TESTNET_FULLNODE, null, _logger);
            _jsonRpcApiClient = new SuiJsonRpcApiClient(rpcClient);
            _signerKeyPair = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC);
            _signerKeyPair2 = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC2);
        }

        [Fact]
        public async Task TestGetTotalTransactionNumberAsync()
        {
            var rpcResult = await _jsonRpcApiClient.GetTotalTransactionBlocksAsync();

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TestGetObjectsOwnedByAddressAsync()
        {
            var address = _signerKeyPair2.PublicKeyAsSuiAddress;
            var rpcResult = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync(address);

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task TestGenericGetObjectAsync()
        {
            var address = _signerKeyPair2.PublicKeyAsSuiAddress;
            var rpcResult = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync(address);
            var objectId = rpcResult.Result.First(o => o.Type.ToString() == SuiConstants.SUI_COIN_TYPE).ObjectId;
            var obj1 = await _jsonRpcApiClient.GetObjectAsync(objectId);

            var obj = await _jsonRpcApiClient.GetObjectAsync<SUICoin>(objectId);

            obj.IsSuccess.Should().BeTrue();
            obj.Result.Should().NotBeNull();
            obj.Result.Balance.Should().BeGreaterThan(0);
            obj.Result.Id.Id.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task TestGetObjectsAsync()
        {
            var address = _signerKeyPair.PublicKeyAsSuiAddress;

            var objects = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync<SUICoin>(address);

            objects.IsSuccess.Should().BeTrue();
            objects.Result.Should().NotBeNull();
            objects.Result.Should().OnlyContain(r => r.Balance > 0);
            objects.Result.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Id.Id));
        }

        [Fact]
        public async Task TestGetObjectAsync()
        {
            var objectId = "0x16f89ea5b1b7acd6056fc398bd2e94971443b9ff";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId);

            objectResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestGetNftDomainsAsync()
        {
            var objectId = "0x16f89ea5b1b7acd6056fc398bd2e94971443b9ff";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId);

            var bagResult = await _jsonRpcApiClient.GetObjectAsync<Bag>(objectId);

            var bagObjectId = "0x65980b5e1e4c703fae7b8f60affbabf40b09f141";
            var objectsOwnedByBagResult = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync(bagObjectId);

            var domainObject = await _jsonRpcApiClient.GetObjectAsync<DisplayDomain>("0xfe2ff2ac8bb0d1876e0a6ed8abe6aa4dd3efb3f0");

            var domainObj2 = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync< DisplayDomain>(bagObjectId);

            var attributesDomainObj = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<AttributesDomain>(bagObjectId);

            var attr = attributesDomainObj.Result.First().Attributes;

            var urlDomainObj = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<UrlDomain>(bagObjectId);

            var domains = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<DomainBase>(bagObjectId);

            objectResult.IsSuccess.Should().BeTrue();
            objectsOwnedByBagResult.IsSuccess.Should().BeTrue();
        }

        

        [Fact]
        public async Task TestGetCapyAsync()
        {
            var objectId = "0x09013dfa4fc275654fe3530b5cbe9274b4381fc0";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId);
            var ownedObjectsResult = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync(objectId);

            objectResult.IsSuccess.Should().BeTrue();
            ownedObjectsResult.IsSuccess.Should().BeTrue();

            foreach (var dynamicObjectField in ownedObjectsResult.Result)
            {
                var dynamicObjectFieldResult = await _jsonRpcApiClient.GetObjectAsync(dynamicObjectField.ObjectId);
                var dynamicObjectFieldId = dynamicObjectFieldResult.Result.Object.Data.Fields["value"] as string;

                var dynamicObjectResult = await _jsonRpcApiClient.GetObjectAsync(dynamicObjectFieldId);
                dynamicObjectResult.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public async Task TestGetCapyNftsOwnedByAddressAsync()
        {
            var address = "0xa106c6d490ff692411bc6fd2ca59b5804adcac04";
            var ownedObjectsResult = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync<CapyNft>(address);

            ownedObjectsResult.IsSuccess.Should().BeTrue();
            
        }

        [Fact]
        public async Task TestGenericGetObjectsAsync2()
        {
            var address = _signerKeyPair.PublicKeyAsSuiAddress;

            var objects = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync<DevNetNft>(address);

            objects.IsSuccess.Should().BeTrue();
            objects.Result.Should().NotBeEmpty();
            objects.Result.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Name));
            objects.Result.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Url));
        }

        [Fact]
        public async Task TestMoveCallAsync()
        {
            var signer = _signerKeyPair.PublicKeyAsSuiAddress;
            var packageObjectId = "0x2";
            var module = "devnet_nft";
            var function = "mint";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { "test nft name", "test movecall nft desc", "ipfs://bafkreibngqhl3gaa7daob4i2vccziay2jjlp435cf66vhono7nrvww53ty" };
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();

            var rpcResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, gasObjectId, 2000);

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.TxBytes.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task TestExecuteMoveCallAsync_WithU64Param()
        {
            var signer = _signerKeyPair2.PublicKeyAsSuiAddress;
            var packageObjectId = "0xa69f4325d4da26db7b5299fa149242118debe175";
            var module = "playerstate_module";
            var function = "create_playerstate_for_sender";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, gasObjectId, 2000);


            var txBytes = moveCallResult.Result.TxBytes;
            var signature = _signerKeyPair2.Sign(moveCallResult.Result.TxBytes);

            var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForEffectsCert);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
        }
        [Fact]
        public async Task TestExecuteTransactionAsync_WaitForLocalExecution()
        {
            var signer = _signerKeyPair2.PublicKeyAsSuiAddress;
            var packageObjectId = "0x2";
            var module = "devnet_nft";
            var function = "mint";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { "test nft name", "test executetx nft desc", "ipfs://bafkreibngqhl3gaa7daob4i2vccziay2jjlp435cf66vhono7nrvww53ty" };
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, gasObjectId, 2000);


            var txBytes = moveCallResult.Result.TxBytes;
            var signature = _signerKeyPair2.Sign(moveCallResult.Result.TxBytes);

            var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForLocalExecution);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
            txResponse.Result.ConfirmedLocalExecution.Should().BeTrue();
        }

        [Fact]
        public async Task TestExecuteTransactionAsync_WaitForEffectsCert()
        {
            var signer = _signerKeyPair2.PublicKeyAsSuiAddress;
            var packageObjectId = "0x2";
            var module = "devnet_nft";
            var function = "mint";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { "test nft name", "test executetx nft desc", "ipfs://bafkreibngqhl3gaa7daob4i2vccziay2jjlp435cf66vhono7nrvww53ty" };
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, gasObjectId, 2000);

            var txBytes = moveCallResult.Result.TxBytes;
            var signature = _signerKeyPair2.Sign(moveCallResult.Result.TxBytes);

            var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForEffectsCert);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();
            txResponse.Result.ExecuteTransactionRequestType.Should().Be(SuiExecuteTransactionRequestType.WaitForEffectsCert);

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
            txResponse.Result!.Certificate.Should().NotBeNull();
            effects.Should().NotBeNull();
            effects.Effects.Status.Status.Should().Be(SuiExecutionStatus.Success);
            effects.Effects.Created.Should().HaveCount(1);
        }

        [Fact]
        public async Task TestCreateSharedCounter()
        {
            var signer = _signerKeyPair2.PublicKeyAsSuiAddress;
            var packageObjectId = "0xa1382c7886e88709a9dc4db2b35ac56f629806af";
            var module = "counter";
            var function = "create";
            var typeArgs = System.Array.Empty<string>();
            var args = System.Array.Empty<object>();
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, gasObjectId, 2000);

            var txBytes = moveCallResult.Result.TxBytes;
            var signature = _signerKeyPair2.Sign(moveCallResult.Result.TxBytes);

            var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForEffectsCert);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();
            txResponse.Result.ExecuteTransactionRequestType.Should().Be(SuiExecuteTransactionRequestType.WaitForEffectsCert);

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
            txResponse.Result!.Certificate.Should().NotBeNull();
            effects.Should().NotBeNull();
            effects.Effects.Status.Status.Should().Be(SuiExecutionStatus.Success);
            effects.Effects.Created.Should().HaveCount(1);
        }

        [Fact]
        public async Task TestHelperGetBalanceAsync()
        {
            var balance = await SuiHelper.GetBalanceAsync(_jsonRpcApiClient, _signerKeyPair.PublicKeyAsSuiAddress);
            balance.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TestGetAllEvents()
        {
            var query = new SuiAllEventQuery();
            var objects = await _jsonRpcApiClient.QueryEventsAsync(query, null, 10);
            objects.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestQueryEventsBySender()
        {
            var query = new SuiSenderEventQuery()
            {
                Sender = "0x1a4f2b04e99311b0ff8228cf12735402f6618d7be0f0b320364339baf03e49df"
            };
            var objects = await _jsonRpcApiClient.QueryEventsAsync(query, null, 10);
            objects.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestQueryMoveModuleEvents()
        {
            var query = new SuiMoveModuleEventQuery()
            {
                MoveModule = new SuiMoveModuleEventQuery.SuiMoveModule()
                {
                    Module = "access_policy",
                    Package = "0x7f37d6f86facc20063f3e19b95ac84d973ac2cfd64406c561c26921a57b578b2"
                }
            };

            var rpcResult = await _jsonRpcApiClient.QueryEventsAsync(query, null, 10, true);

            rpcResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestGetMoveEvents()
        {
            var query = new SuiMoveEventEventQuery()
            {
                MoveEvent = "0x2::devnet_nft::MintNFTEvent"
            };
            var objects = await _jsonRpcApiClient.QueryEventsAsync(query, null, 10);
            objects.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestGetMoveEventsWithCursor()
        {
            var query = new SuiMoveEventEventQuery()
            {
                MoveEvent = "0x2::devnet_nft::MintNFTEvent"
            };
            var result = await _jsonRpcApiClient.QueryEventsAsync(query, null, 10);
            result.IsSuccess.Should().BeTrue();


            var result2 = await _jsonRpcApiClient.QueryEventsAsync(query, result.Result.NextCursor, 10);
            result2.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestTransferObject()
        {
            var signer = _signerKeyPair2.PublicKeyAsSuiAddress;            
            var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();
            var objectId = "0x421636c6413c13a0662bf4a67da8bb5872921255";
            var recipient = "0x145fa4fb60ac3d87919d1db491dd15f56df54d0d";

            var transferObjectResult = await _jsonRpcApiClient.TransferObjectAsync(signer, objectId, gasObjectId, 2000, recipient);


            var txBytes = transferObjectResult.Result.TxBytes;
            var signature = _signerKeyPair2.Sign(txBytes);

            var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForEffectsCert);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();
            txResponse.Result.ExecuteTransactionRequestType.Should().Be(SuiExecuteTransactionRequestType.WaitForEffectsCert);

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
            txResponse.Result!.Certificate.Should().NotBeNull();
        }

        [Fact]
        public async Task TestGetDynamicFields()
        {
            var id = "0xa7a27550febd72c9bc0928a013534be76ab8a1c9";
            var result = await _jsonRpcApiClient.GetDynamicFieldsAsync(id);
            result.IsSuccess.Should().BeTrue();
        }
    }
}