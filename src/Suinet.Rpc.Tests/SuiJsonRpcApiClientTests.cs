using FluentAssertions;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math;
using Serilog;
using Serilog.Extensions.Logging;
using Suinet.Rpc.Client;
using Suinet.Rpc.Http;
using Suinet.Rpc.Types;
using Suinet.Rpc.Types.Coins;
using Suinet.Rpc.Types.Nfts;
using Suinet.Rpc.Types.ObjectDataParsers;
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

        private const string TEST_MNEMONIC = "pen flush vintage detect work resource stand pole execute arrow purpose muffin";

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
        }

        [Fact]
        public async Task TestGetTotalTransactionBlocksAsync()
        {
            var rpcResult = await _jsonRpcApiClient.GetTotalTransactionBlocksAsync();

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();

            var ulongResult = (ulong)(rpcResult.Result);
            ulongResult.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TestGetDynamicFieldsAsync()
        {
            var rpcResult = await _jsonRpcApiClient.GetDynamicFieldsAsync("0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016", null, null);

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.Data.Should().HaveCountGreaterThan(0);
            rpcResult.Result.Data.Should().AllSatisfy(Data => Data.ObjectId.Should().NotBeNullOrWhiteSpace());
        }

        [Fact]
        public async Task TestGetDynamicFieldObjectAsync()
        {
            var dynamicFieldsRpcResult = await _jsonRpcApiClient.GetDynamicFieldsAsync("0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016", null, null);

            var dynamicFieldObjectRpcResult = await _jsonRpcApiClient.GetDynamicFieldObjectAsync("0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016", dynamicFieldsRpcResult.Result.Data.First().Name);

            dynamicFieldObjectRpcResult.Should().NotBeNull();
            dynamicFieldObjectRpcResult.IsSuccess.Should().BeTrue();
            dynamicFieldObjectRpcResult.ErrorMessage.Should().BeNullOrEmpty();
            dynamicFieldObjectRpcResult.Result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task TestGetTypedDynamicFieldObjectAsync()
        {
            var dynamicFieldsRpcResult = await _jsonRpcApiClient.GetDynamicFieldsAsync("0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016", null, null);

            var dynamicFieldObjectRpcResult = await _jsonRpcApiClient.GetDynamicFieldObjectAsync<SuiFrenAccessory>("0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016", dynamicFieldsRpcResult.Result.Data.First().Name, new SuiFrenAccessoryParser());

            dynamicFieldObjectRpcResult.Should().NotBeNull();
            dynamicFieldObjectRpcResult.IsSuccess.Should().BeTrue();
            dynamicFieldObjectRpcResult.ErrorMessage.Should().BeNullOrEmpty();
            dynamicFieldObjectRpcResult.Result.Id.Should().NotBeNull();
            dynamicFieldObjectRpcResult.Result.Id.Id.Should().NotBeNull();
            dynamicFieldObjectRpcResult.Result.Name.Should().NotBeNull();
            dynamicFieldObjectRpcResult.Result.Type.Should().NotBeNull();
        }

        [Fact]
        public async Task TestGetTransactionBlockAsync()
        {
            var digest = "GMwsnGNNce8Qdrb6LcgnMZvSPYCv6iL7JmAA32iykc2y";
            var rpcResult = await _jsonRpcApiClient.GetTransactionBlockAsync(digest, TransactionBlockResponseOptions.ShowAll());

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.Transaction.Should().NotBeNull();
            rpcResult.Result.Transaction.Data.Transaction.Should().NotBeNull();

            var txes = rpcResult.Result.Transaction.Data.Transaction as ProgrammableTransactionBlockKind;
            txes.Transactions.Should().HaveCountGreaterThan(0); 
        }

        [Fact]
        public async Task TestGetObjectsOwnedByAddressAsync()
        {
            var address = _signerKeyPair.PublicKeyAsSuiAddress;
            var filter = ObjectDataFilterFactory.CreateMatchAllFilter(ObjectDataFilterFactory.CreateAddressOwnerFilter(address));
            var rpcResult = await _jsonRpcApiClient.GetOwnedObjectsAsync(address,
                new ObjectResponseQuery() { Filter = filter }, null, null);

            rpcResult.Should().NotBeNull();
            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.ErrorMessage.Should().BeNullOrEmpty();
            rpcResult.Result.Data.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task TestGenericGetObjectAsync()
        {
            var address = _signerKeyPair.PublicKeyAsSuiAddress;
            var filter = ObjectDataFilterFactory.CreateMatchAllFilter(ObjectDataFilterFactory.CreateAddressOwnerFilter(address));
            var options = new ObjectDataOptions() { ShowType = true };
            var rpcResult = await _jsonRpcApiClient.GetOwnedObjectsAsync(address, new ObjectResponseQuery()
            { Filter = filter, Options = options }, null, null);
            var objectId = rpcResult.Result.Data.First(o => o.Data.Type.ToString() == SuiConstants.SUI_COIN_TYPE).Data.ObjectId;

            var parser = new SUICoinParser();
            var obj = await _jsonRpcApiClient.GetObjectAsync<SUICoin>(objectId, parser);

            obj.IsSuccess.Should().BeTrue();
            obj.Result.Should().NotBeNull();
            var balanceAsULong = (ulong)obj.Result.Balance;
            balanceAsULong.Should().BeGreaterThan(0);
            obj.Result.Id.Id.Should().NotBeNull();
            ObjectId.IsValid(obj.Result.Id.Id).Should().BeTrue();
        }

        [Fact]
        public async Task TestGetObjectsAsync()
        {
            var address = _signerKeyPair.PublicKeyAsSuiAddress;

            var objects = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync<SUICoin>(address, new SUICoinParser());

            objects.IsSuccess.Should().BeTrue();
            objects.Result.Should().NotBeNull();
            objects.Result.Should().OnlyContain(r => r.Balance > 0);
            objects.Result.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Id.Id));
        }

        [Fact]
        public async Task TestGetObjectAsync()
        {
            var objectId = "0x491070500ffff487f9e67ddb7d0d473b4f4510e3a9acc6798058394fe48c8cf8";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId, new ObjectDataOptions());

            objectResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestGetNftDomainsAsync()
        {
            var objectId = "0x16f89ea5b1b7acd6056fc398bd2e94971443b9ff";
            //var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId);

            //var bagResult = await _jsonRpcApiClient.GetObjectAsync<Bag>(objectId);

            //var bagObjectId = "0x65980b5e1e4c703fae7b8f60affbabf40b09f141";
            //var objectsOwnedByBagResult = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync(bagObjectId);

            //var domainObject = await _jsonRpcApiClient.GetObjectAsync<DisplayDomain>("0xfe2ff2ac8bb0d1876e0a6ed8abe6aa4dd3efb3f0");

            //var domainObj2 = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync< DisplayDomain>(bagObjectId);

            //var attributesDomainObj = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<AttributesDomain>(bagObjectId);

            //var attr = attributesDomainObj.Result.First().Attributes;

            //var urlDomainObj = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<UrlDomain>(bagObjectId);

            //var domains = await _jsonRpcApiClient.GetObjectsOwnedByObjectAsync<DomainBase>(bagObjectId);

            //objectResult.IsSuccess.Should().BeTrue();
            //objectsOwnedByBagResult.IsSuccess.Should().BeTrue();
        }
        

        [Fact]
        public async Task TestGetAndParseSuiFrenAsync()
        {
            var objectId = "0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId, ObjectDataOptions.ShowAll());

            objectResult.IsSuccess.Should().BeTrue();
            objectResult.Result.Should().NotBeNull();

            var frenParser = new CapySuiFrenParser();
            var fren = frenParser.Parse(objectResult.Result.Data);

            fren.Display.Should().NotBeNull();
            fren.Properties.Should().NotBeNull();
            fren.Type.Should().NotBeNullOrWhiteSpace();
            fren.ObjectId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task TestGetTypedSuiFrenAsync()
        {
            var objectId = "0xa55b914a7c7715ac9e204f5582ea341934c5d139ad20898db3d397a4edc5b016";
            var objectResult = await _jsonRpcApiClient.GetObjectAsync(objectId, ObjectDataOptions.ShowAll());

            objectResult.IsSuccess.Should().BeTrue();
            objectResult.Result.Should().NotBeNull();
        }


        [Fact]
        public async Task TestGetTypedSuiFrenNftsOwnedByAddressAsync()
        {
            var address = "0x770f28e2ee8be9c1f5eb5edbe3612f9e13fc5b721b4f6f74b697952b1b0eb836";
            var ownedObjectsResult = await _jsonRpcApiClient.GetObjectsOwnedByAddressAsync<CapySuiFren>(address, new CapySuiFrenParser(), null, null);

            ownedObjectsResult.IsSuccess.Should().BeTrue();
            
        }

        [Fact]
        public async Task TestMoveCallAsync()
        {
            var signer = _signerKeyPair.PublicKeyAsSuiAddress;
            var packageObjectId = "0x116c6862df1e71aa13a88e34b460cfdd46d3fc21bbe64df546faea7251b25dce";
            var module = "counter";
            var function = "increment";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { "0xab86eab42d95c987c879fb53292fa47210e30190524e07e4cf7aa9930446b538" };

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, 100000);

            moveCallResult.Should().NotBeNull();
            moveCallResult.IsSuccess.Should().BeTrue();
            moveCallResult.ErrorMessage.Should().BeNullOrEmpty();
            moveCallResult.Result.TxBytes.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Preparation for the test:
        /// 1. Publish the basics sample from sui/sui_programmability/examples/basics
        /// 2. create a counter object - simplest way is calling the create function on the sui explorer
        /// </summary>
        /// <returns></returns>

        [Fact]
        public async Task TestExecuteMoveCallAsync()
        {
            var signer = _signerKeyPair.PublicKeyAsSuiAddress;
            var packageObjectId = "0x116c6862df1e71aa13a88e34b460cfdd46d3fc21bbe64df546faea7251b25dce";
            var module = "counter";
            var function = "increment";
            var typeArgs = System.Array.Empty<string>();
            var args = new object[] { "0xab86eab42d95c987c879fb53292fa47210e30190524e07e4cf7aa9930446b538" };

            var moveCallResult = await _jsonRpcApiClient.MoveCallAsync(signer, packageObjectId, module, function, typeArgs, args, 10000000);

            var txBytes = moveCallResult.Result.TxBytes;
            var rawSigner = new RawSigner(_signerKeyPair);
            var signature = rawSigner.SignData(Intent.GetMessageWithIntent(txBytes));
          
            var txResponse = await _jsonRpcApiClient.ExecuteTransactionBlockAsync(txBytes, new[] { signature.Value }, TransactionBlockResponseOptions.ShowAll(), ExecuteTransactionRequestType.WaitForLocalExecution);

            txResponse.Should().NotBeNull();
            txResponse.IsSuccess.Should().BeTrue();
            txResponse.ErrorMessage.Should().BeNullOrEmpty();

            var effects = txResponse.Result.Effects;
            effects.Should().NotBeNull();
        }      

        [Fact]
        public async Task TestGetBalanceAsync()
        {
            var balance = await _jsonRpcApiClient.GetBalanceAsync(_signerKeyPair.PublicKeyAsSuiAddress, null);
            balance.Should().NotBeNull();
            balance.Result.Should().NotBeNull();
            balance.IsSuccess.Should().BeTrue();
            var ulongBalance = (ulong)balance.Result.TotalBalance;
            ulongBalance.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TestGetEvents()
        {
            var result = await _jsonRpcApiClient.GetEventsAsync("GMwsnGNNce8Qdrb6LcgnMZvSPYCv6iL7JmAA32iykc2y");
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
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
        public async Task TestSuiGetCheckpoints()
        {
            var result = await _jsonRpcApiClient.SuiGetCheckpointsAsync(null, null, false);
            result.IsSuccess.Should().BeTrue();
            result.Result.Data.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task TextSuiGetCheckpoint()
        {
            var id = "69WiPg3DAQiwdxfncX6wYQ2siKwAe6L9BZthQea3JNMD";
            var result = await _jsonRpcApiClient.GetCheckpointAsync(id);
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result.Digest.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task TestGetLatestCheckpointSequenceNumber()
        {
            var result = await _jsonRpcApiClient.GetLatestCheckpointSequenceNumberAsync();
            result.IsSuccess.Should().BeTrue();
            var sequenceNumber = (ulong)result.Result;
            sequenceNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TestTransferObject()
        {
            var signer = _signerKeyPair.PublicKeyAsSuiAddress;            
            //var gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(_jsonRpcApiClient, signer, 1, 2000)).Single();
            //var objectId = "0x421636c6413c13a0662bf4a67da8bb5872921255";
            //var recipient = "0x145fa4fb60ac3d87919d1db491dd15f56df54d0d";

            //var transferObjectResult = await _jsonRpcApiClient.TransferObjectAsync(signer, objectId, gasObjectId, 2000, recipient);


            //var txBytes = transferObjectResult.Result.TxBytes;
            //var signature = _signerKeyPair.Sign(txBytes);

            //var txResponse = await _jsonRpcApiClient.ExecuteTransactionAsync(txBytes, SuiSignatureScheme.ED25519, signature, _signerKeyPair2.PublicKeyBase64, SuiExecuteTransactionRequestType.WaitForEffectsCert);

            //txResponse.Should().NotBeNull();
            //txResponse.IsSuccess.Should().BeTrue();
            //txResponse.ErrorMessage.Should().BeNullOrEmpty();
            //txResponse.Result.ExecuteTransactionRequestType.Should().Be(SuiExecuteTransactionRequestType.WaitForEffectsCert);

            //var effects = txResponse.Result.Effects;
            //effects.Should().NotBeNull();
            //txResponse.Result!.Certificate.Should().NotBeNull();
        }
    }
}