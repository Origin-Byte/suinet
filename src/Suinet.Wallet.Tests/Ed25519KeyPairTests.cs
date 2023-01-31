using FluentAssertions;
using System;
using Xunit;

namespace Suinet.Wallet.Tests
{
    public class Ed25519KeyPairTests
    {
        private const string MNEMONIC = "that august urban math slender industry area mountain worry day ski hold";

        [Fact]
        public void TestSign()
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(MNEMONIC);
            var signatureBase64 = keyPair.Sign("VHJhbnNhY3Rpb25EYXRhOjoAAIo14k0fJfmLsmvK1Zb9taXCM55N8jboEPUEbP13tb7OUq9Ks8DtRysBAAAAAAAAACCbaETKxfCPlL1s0f0qKcSFjbsg6Z2zNmVY/7piWAJgjg7CxBXs/bXlUUbk0yZ1wds49LJhjFiDA4hL4QGu32z9qGljR7z7EcQBAAAAAAAAACB7Ji3eM85S9kCa9LwG6RM91AYKBoUh5UqeFEdIBHmxQgEAAAAAAAAA0AcAAAAAAAA=");

            var expectedSignatureBase64 = "7bl70bJVWnfMJEJ4yaDcNesWMjuEmFvIoRe/nA3rUxO8vK0LGManpf3e0jWFP7uw43CJsWgvUzXYSgpvpWlFBA==";
            var expectedBytes = Convert.FromBase64String(expectedSignatureBase64);
            var signatureBytes = Convert.FromBase64String(signatureBase64);
            signatureBytes.Should().BeEquivalentTo(expectedBytes);
            signatureBase64.Should().Be(expectedSignatureBase64);
            keyPair.PublicKeyBase64.Should().Be("2+akJUgQfx8GlL3DolDcmhr0fUIgNzNKM5oTmen5nLk=");

        }

        [Fact]
        public void TestToSuiAddress()
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(MNEMONIC);
            keyPair.PublicKeyAsSuiAddress.Should().Be("0xe604761d791c1b28eff7d5d2558e1968ec1bed63");
        }
    }
}