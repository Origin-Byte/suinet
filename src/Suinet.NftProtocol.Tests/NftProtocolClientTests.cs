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
using Suinet.NftProtocol.TransactionBuilders;
using System.Collections.Generic;
using Suinet.Rpc.Client;
using Suinet.Rpc.Signer;
using Suinet.NftProtocol.Domains;

namespace Suinet.NftProtocol.Tests
{
    /// <summary>
    /// Using SuiMarines example collection from Origin Bytes.
    /// To publish your contract and use the collection follow the readme:
    /// https://github.com/Origin-Byte/nft-protocol
    /// </summary>
    public class NftProtocolClientTests
    {
        private const string WALLET_2_ADDRESS = "0xa106c6d490ff692411bc6fd2ca59b5804adcac04";

        private const string PACKAGE_OBJECT_ID = "0x3f2f2e76de27bc62d22a5a005718f0be7f07bd3a";
        private const string MINT_CAP_ID = "0xc0160ae637f0dd6aeb4ba47e135122ae38d4d84f";
        private const string MODULE_NAME = "deadbytes";

        private const string TEST_MNEMONIC = "bus indicate leave science minor clip embrace faculty wink industry addict track soup burger scissors another enrich muscle loop fever vacuum buyer paddle roof";
        private const string TEST_MNEMONIC_2 = "that august urban math slender industry area mountain worry day ski hold";
  
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

            var rpcClient = new RpcClient(SuiConstants.DEVNET_ENDPOINT, null, _logger);
            _jsonRpcApiClient = new SuiJsonRpcApiClient(rpcClient);
            _signerKeyPair = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC);
            _signer = new Signer(_jsonRpcApiClient, _signerKeyPair);
            _nftProtocolClient = new NftProtocolClient(_jsonRpcApiClient, _signer);

            _signerKeyPair2 = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC_2);
            _signer2 = new Signer(_jsonRpcApiClient, _signerKeyPair2);
            _nftProtocolClient2 = new NftProtocolClient(_jsonRpcApiClient, _signer2);           
        }

        [Fact]
        public async Task DeadBytes_MintNfts()
        {
            var recipient = WALLET_2_ADDRESS;

            for (int i = 1; i <= 4; i++)
            {

                var txParams = new MintNft()
                {
                    Attributes = new Dictionary<string, object>()
                {
                    { "nft_type", "face" },
                },
                    Description = "You can use this as a face of your character in the game!",
                    MintCap = MINT_CAP_ID,
                    Recipient = recipient,
                    ModuleName = MODULE_NAME,
                    Name = $"Face {i}",
                    PackageObjectId = PACKAGE_OBJECT_ID,
                    Signer = _signerKeyPair.PublicKeyAsSuiAddress,
                    Url = $"https://suiunitysdksample.blob.core.windows.net/nfts/face{i}.png"
                };

                var rpcResult = await _nftProtocolClient.MintNftAsync(txParams);
                rpcResult.Should().NotBeNull();
                rpcResult.IsSuccess.Should().BeTrue();
            }

            for (int i = 1; i <= 2; i++)
            {

                var txParams = new MintNft()
                {
                    Attributes = new Dictionary<string, object>()
                {
                    { "nft_type", "sticker" },
                },
                    Description = "You can use this as a sticker of your character in the game!",
                    MintCap = MINT_CAP_ID,
                    Recipient = recipient,
                    ModuleName = MODULE_NAME,
                    Name = $"Sticker {i}",
                    PackageObjectId = PACKAGE_OBJECT_ID,
                    Signer = _signerKeyPair.PublicKeyAsSuiAddress,
                    Url = $"https://suiunitysdksample.blob.core.windows.net/nfts/sticker{i}.png"
                };

                var rpcResult = await _nftProtocolClient.MintNftAsync(txParams);
                rpcResult.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Mintv17Nfts()
        {
            var recipient = WALLET_2_ADDRESS;
            var attributes = new Dictionary<string, object>()
                {
                    { "nft_type", "face" },
                    { "attr2", "attr2value" },
                    { "nft_types", "face,sticker" },
                };
              
            var i = 4;
            var txParams = new MintNft()
            {
                Attributes = attributes,
                Description = "You can use this as a face of your character in the game!",
                MintCap = MINT_CAP_ID,
                Recipient = recipient,
                ModuleName = MODULE_NAME,
                Name = $"Face {i}",
                PackageObjectId = PACKAGE_OBJECT_ID,
                Signer = _signerKeyPair.PublicKeyAsSuiAddress,
                Url = $"https://suiunitysdksample.blob.core.windows.net/nfts/face{i}.png"
            };

            var rpcResult = await _nftProtocolClient.MintNftAsync(txParams);

            rpcResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task TestGetArtNftAsync_WithAllDomains()
        {
            var objectId = "0x08cb7a94f3c8244868fe582ec3fdfad1c03a9d6a";
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
            rpcResult.Result.Attributes.Should().BeNull();
        }

        [Fact]
        public async Task TestGetArtNftsOwnedByAddressAsync_WithAllDomains()
        {
            var address = WALLET_2_ADDRESS;
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
            var address = WALLET_2_ADDRESS;
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