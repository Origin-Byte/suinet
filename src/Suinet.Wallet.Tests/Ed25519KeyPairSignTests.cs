using FluentAssertions;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using Xunit;
using Chaos.NaCl;

namespace Suinet.Wallet.Tests
{
    public class Ed25519KeyPairSignTests
    {
        private const string MNEMONIC = "pen flush vintage detect work resource stand pole execute arrow purpose muffin";

        private const string TX_BYTES =
  "AAACAQDMdYtdFSLGe6VbgpuIsMksv9Ypzpvkq2jiYq0hAjUpOQIAAAAAAAAAIHGwPza+lUm6RuJV1vn9pA4y0PwVT7k/KMMbUViQS5ydACAMVn/9+BYsttUa90vgGZRDuS6CPUumztJN5cbEY3l9RgEBAQEAAAEBAHUFfdk1Tg9l6STLBoSBJbbUuehTDUlLH7p81kpqCKsaBCiJ034Ac84f1oqgmpz79O8L/UeLNDUpOUMa+LadeX93AgAAAAAAAAAgs1e67e789jSlrzOJUXq0bb7Bn/hji+3F5UoMAbze595xCSZCVjU1ItUC9G7KQjygNiBbzZe8t7YLPjRAQyGTzAIAAAAAAAAAIAujHFcrkJJhZfCmxmCHsBWxj5xkviUqB479oupdgMZu07b+hkrjyvCcX50dO30v3PszXFj7+lCNTUTuE4UI3eoCAAAAAAAAACBIv39dyVELUFTkNv72mat5R1uHFkQdViikc1lTMiSVlOD+eESUq3neyciBatafk9dHuhhrS37RaSflqKwFlwzPAgAAAAAAAAAg8gqL3hCkAho8bb0PoqshJdqQFoRP8ZmQMZDFvsGBqa11BX3ZNU4PZekkywaEgSW21LnoUw1JSx+6fNZKagirGgEAAAAAAAAAKgQAAAAAAAAA";
        private const string DIGEST = "VMVv+/L/EG7/yhEbCQ1qiSt30JXV8yIm+4xO6yTkqeM=";

        // Test cases for Ed25519.
        // First element is the mnemonics, second element is the
        // base64 encoded pubkey, derived using DERIVATION_PATH,
        // third element is the hex encoded address, fourth
        // element is the valid signature produced for TX_BYTES.
        public static IEnumerable<object[]> TestCases => new List<object[]>
        {
            new object[] {
                "film crazy soon outside stand loop subway crumble thrive popular green nuclear struggle pistol arm wife phrase warfare march wheat nephew ask sunny firm",
                "ImR/7u82MGC9QgWhZxoV8QoSNnZZGLG19jjYLzPPxGk=",
                "0xa2d14fad60c56049ecf75246a481934691214ce413e6a8ae2fe6834c173a6133",
                "NwIObhuKot7QRWJu4wWCC5ttOgEfN7BrrVq1draImpDZqtKEaWjNNRKKfWr1FL4asxkBlQd8IwpxpKSTzcXMAQ=="
            },
            new object[] {
                "require decline left thought grid priority false tiny gasp angle royal system attack beef setup reward aunt skill wasp tray vital bounce inflict level",
                "vG6hEnkYNIpdmWa/WaLivd1FWBkxG+HfhXkyWgs9uP4=",
                "0x1ada6e6f3f3e4055096f606c746690f1108fcc2ca479055cc434a3e1d3f758aa",
                "8BSMw/VdYSXxbpl5pp8b5ylWLntTWfBG3lSvAHZbsV9uD2/YgsZDbhVba4rIPhGTn3YvDNs3FOX5+EIXMup3Bw=="
            },
            new object[] {
                "organ crash swim stick traffic remember army arctic mesh slice swear summer police vast chaos cradle squirrel hood useless evidence pet hub soap lake",
                "arEzeF7Uu90jP4Sd+Or17c+A9kYviJpCEQAbEt0FHbU=",
                "0xe69e896ca10f5a77732769803cc2b5707f0ab9d4407afb5e4b4464b89769af14",
                "/ihBMku1SsqK+yDxNY47N/tAREZ+gWVTvZrUoCHsGGR9CoH6E7SLKDRYY9RnwBw/Bt3wWcdJ0Wc2Q3ioHIlzDA=="
            },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void TestSign(string mnemonic, string pubKey, string address, string expectedSignatureBase64)
        {
            var txBytes = CryptoBytes.FromBase64String(TX_BYTES);
            var intentMessage = Intent.GetMessageWithIntent(txBytes);

            var digest = HashHelper.ComputeBlake2bHash(intentMessage);
            var digestB64 = CryptoBytes.ToBase64String(digest);
            digestB64.Should().Be(DIGEST);

            var keyPair = Mnemonics.GetKeypairFromMnemonic(mnemonic);

            keyPair.PublicKeyBase64.Should().Be(pubKey);
            keyPair.PublicKeyAsSuiAddress.Should().Be(address);

            var signer = new RawSigner(keyPair);
            var serializedSignature = signer.SignData(intentMessage);
            var signature = SignatureUtils.FromSerializedSignature(serializedSignature);
            var signatureBase64 = CryptoBytes.ToBase64String(signature.Signature);
            signatureBase64.Should().Be(expectedSignatureBase64);
        }

        [Fact]
        public void TestToSuiAddress()
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(MNEMONIC);
            keyPair.PublicKeyAsSuiAddress.Should().Be("0x2d7fe9442e5efeb41fecace0128ddf0ca5b92981c894ec9bf361b54c73aba80c");
        }
    }
}