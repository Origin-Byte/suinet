using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Suinet.Rpc.Http;
using Suinet.Rpc.Types;
using Suinet.Wallet;
using Suinet.Rpc;
using System.Collections.Generic;
using Suinet.Rpc.Client;
using Suinet.Rpc.Signer;
using Suinet.NftProtocol.Domains;
using Suinet.NftProtocol.Examples;

namespace Suinet.NftProtocol.Tests
{
    /// <summary>
    /// Using SuiMarines example collection from Origin Bytes.
    /// To publish your contract and use the collection follow the readme:
    /// https://github.com/Origin-Byte/nft-protocol
    /// </summary>
    public class NftProtocolClientTests
    {
        private const string PACKAGE_OBJECT_ID = "0xb11eda772add7178d97d98fbcb5dc73ea1afec0bb94705416c43efdbedba6e4b";
        private const string MODULE_NAME = "suitraders";
        private const string TEST_MNEMONIC = "pen flush vintage detect work resource stand pole execute arrow purpose muffin";
        private const string RECIPIENT_ADDRESS = "0x2d7fe9442e5efeb41fecace0128ddf0ca5b92981c894ec9bf361b54c73aba80c";

        private readonly ILogger<NftProtocolClientTests> _logger;
        private readonly IJsonRpcApiClient _jsonRpcApiClient;
        private readonly ITestOutputHelper _output;
        private readonly IKeyPair _signerKeyPair;
        private readonly ISigner _signer;
        private readonly INftProtocolClient _nftProtocolClient;

        private readonly IKeyPair _signerKeyPair2;
        private readonly ISigner _signer2;
        private readonly INftProtocolClient _nftProtocolClient2;

        public NftProtocolClientTests(ITestOutputHelper output)
        {
            _output = output;
            var serilogOutputLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
            .CreateLogger()
            .ForContext<NftProtocolClientTests>();
            var serilogLoggerFactory = new SerilogLoggerFactory(serilogOutputLogger);
            _logger = serilogLoggerFactory.CreateLogger<NftProtocolClientTests>();

            var rpcClient = new RpcClient(SuiConstants.TESTNET_FULLNODE, null, _logger);
            _jsonRpcApiClient = new SuiJsonRpcApiClient(rpcClient);
            _signerKeyPair = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC);
            _nftProtocolClient = new NftProtocolClient(_jsonRpcApiClient, _signerKeyPair);
        }

        [Fact]
        public async Task MintNfts()
        {
            var recipient = RECIPIENT_ADDRESS;

            for (int i = 1; i <= 1; i++)
            {
                var txParams = new MintSuitradersNft()
                {
                    Attributes = new Dictionary<string, object>()
                {
                    { "nft_type", "face" },
                },
                    Description = "You can use this as a face of your character in the game!",
                    Recipient = recipient,
                    ModuleName = MODULE_NAME,
                    Function = "airdrop_nft",
                    Name = $"Face {i}",
                    PackageObjectId = PACKAGE_OBJECT_ID,
                    Signer = _signerKeyPair.PublicKeyAsSuiAddress,
                    Url = $"https://suiunitysdksample.blob.core.windows.net/nfts/face{i}.png"
                };

                var rpcResult = await _nftProtocolClient.MintNftAsync(txParams);
                rpcResult.Should().NotBeNull();
                rpcResult.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public async Task TestGetArtNftAsync()
        {
            var objectId = "0x2dfdb7e05c578547d1c3d97ad77353a6c6327d2f08670c3a57dce7241571b8ca";
            var rpcResult = await _nftProtocolClient.GetArtNftAsync(objectId);

            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.Result.Url.Should().NotBeEmpty();
            rpcResult.Result.Name.Should().NotBeEmpty();
            rpcResult.Result.Description.Should().NotBeEmpty();
           // rpcResult.Result.Attributes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task TestGetArtNftAsync_WithUrlDomain()
        {
            var objectId = "0x08cb7a94f3c8244868fe582ec3fdfad1c03a9d6a";
            var rpcResult = await _nftProtocolClient.GetArtNftAsync(objectId, typeof(UrlDomain));

            rpcResult.IsSuccess.Should().BeTrue();
            rpcResult.Result.Url.Should().NotBeNullOrWhiteSpace();
            rpcResult.Result.Name.Should().BeNull();
            rpcResult.Result.Description.Should().BeNull();
            //rpcResult.Result.Attributes.Should().BeNull();
        }

        [Fact]
        public async Task TestGetArtNftsOwnedByAddressAsync_WithAllDomains()
        {
            var address = "0x7d18b23e9314280fb9cccd7259bf397001650ff9";
            //var address = WALLET_2_ADDRESS;
            var rpcResult = await _nftProtocolClient.GetArtNftsOwnedByAddressAsync(address);

            rpcResult.IsSuccess.Should().BeTrue();

            foreach (var nft in rpcResult.Result)
            {
                nft.Url.Should().NotBeNullOrWhiteSpace();
                nft.Name.Should().NotBeNullOrWhiteSpace();
                nft.Description.Should().NotBeNullOrWhiteSpace();
            }
        }


        [Fact]
        public async Task TestGetArtNftsOwnedByAddressAsync_WithUrlDomains()
        {
            var address = RECIPIENT_ADDRESS;
            var rpcResult = await _nftProtocolClient.GetArtNftsOwnedByAddressAsync(address, typeof(UrlDomain));

            rpcResult.IsSuccess.Should().BeTrue();

            foreach (var nft in rpcResult.Result)
            {
                nft.Url.Should().NotBeNullOrWhiteSpace();
                nft.Name.Should().BeNull();
                nft.Description.Should().BeNull();
            }
        }
    }
}