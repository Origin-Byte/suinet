using FluentAssertions;
using System.Collections.Generic;
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

        public static IEnumerable<object[]> GetMnemonicKeypairData => new List<object[]>
        {
            new object[] {
                "film crazy soon outside stand loop subway crumble thrive popular green nuclear struggle pistol arm wife phrase warfare march wheat nephew ask sunny firm",
                "/noDlhYrZghx2Iwlf2hyQzX+X8pCen8nRrWDGC82zNTb5qQlSBB/HwaUvcOiUNyaGvR9QiA3M0ozmhOZ6fmcuQ==",
                "2+akJUgQfx8GpL3DolDcmhr0fUIgNzNKM5oTmen5nLk="
            },
            new object[] {
                "result crisp session latin must fruit genuine question prevent start coconut brave speak student dismiss",
                "7jlISawJNpXqPJ22zuFAB4dKecEY/La4pWhP2kvcaV5oWy1vmHhN12MkmvIckvWIyhvoDECpjFW/fJG3TlrB4g==",
                "aFstb5h4TddjJJryHJL1iMob6AxAqYxVv3yRt05aweI="
            },
        };

        [Theory]
        [MemberData(nameof(GetMnemonicKeypairData))]
        public void TestGetKeypairFromMnemonic(string mnemonic, string expectedPrivateKey, string expectedAddress)
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(mnemonic);
            keyPair.PrivateKeyBase64.Should().Be(expectedPrivateKey);
            keyPair.PublicKeyAsSuiAddress.Should().Be(expectedAddress);
        }

        [Fact]
        public void TestValidateMnemonic()
        {
            var mnemo = Mnemonics.GenerateMnemonic();
            Mnemonics.ValidateMnemonic(mnemo).Should().BeTrue();
        }
    }
}