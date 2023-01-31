using FluentAssertions;
using Xunit;

namespace Suinet.Wallet.Tests
{
    public class MnemonicsTests
    {
        // test mnemonic from sui sdk tests for reference
        // https://github.com/MystenLabs/sui/blob/main/sdk/typescript/test/unit/cryptography/ed25519-keypair.test.ts
        private const string TEST_MNEMONIC_SUI =
  "result crisp session latin must fruit genuine question prevent start coconut brave speak student dismiss";

        [Fact]
        public void TestGenerateMnemonic()
        {
            var mnemo = Mnemonics.GenerateMnemonic();
            mnemo.Split(' ').Should().HaveCount(12);
        }

        [Fact]
        public void TestGetKeypairFromMnemonic2()
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic("that august urban math slender industry area mountain worry day ski hold");

            keyPair.PrivateKeyBase64.Should().Be("/noDlhYrZghx2Iwlf2hyQzX+X8pCen8nRrWDGC82zNTb5qQlSBB/HwaUvcOiUNyaGvR9QiA3M0ozmhOZ6fmcuQ==");
            keyPair.PublicKeyBase64.Should().Be("2+akJUgQfx8GlL3DolDcmhr0fUIgNzNKM5oTmen5nLk=");
        }

        [Fact]
        public void TestGetKeypairFromMnemonic()
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(TEST_MNEMONIC_SUI);

            keyPair.PrivateKeyBase64.Should().Be("7jlISawJNpXqPJ22zuFAB4dKecEY/La4pWhP2kvcaV5oWy1vmHhN12MkmvIckvWIyhvoDECpjFW/fJG3TlrB4g==");
            keyPair.PublicKeyBase64.Should().Be("aFstb5h4TddjJJryHJL1iMob6AxAqYxVv3yRt05aweI=");
            keyPair.PublicKeyAsSuiAddress.Should().Be("0x1a4623343cd42be47d67314fce0ad042f3c82685");
        }

        [Fact]
        public void TestValidateMnemonic()
        {
            var mnemo = Mnemonics.GenerateMnemonic();
            Mnemonics.ValidateMnemonic(mnemo).Should().BeTrue();
        }
    }
}