using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Suinet.Wallet.Tests
{
    public class MnemonicsTests
    {
        // test mnemonic from sui sdk tests for reference
        // https://github.com/MystenLabs/sui/blob/main/sdk/typescript/test/unit/cryptography/ed25519-keypair.test.ts

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
                "0xa2d14fad60c56049ecf75246a481934691214ce413e6a8ae2fe6834c173a6133"
            },
            new object[] {
                "require decline left thought grid priority false tiny gasp angle royal system attack beef setup reward aunt skill wasp tray vital bounce inflict level",
                "0x1ada6e6f3f3e4055096f606c746690f1108fcc2ca479055cc434a3e1d3f758aa"
            },
            new object[] {
                "organ crash swim stick traffic remember army arctic mesh slice swear summer police vast chaos cradle squirrel hood useless evidence pet hub soap lake",
                "0xe69e896ca10f5a77732769803cc2b5707f0ab9d4407afb5e4b4464b89769af14"
            },
        };

        [Theory]
        [MemberData(nameof(GetMnemonicKeypairData))]
        public void TestGetKeypairFromMnemonic(string mnemonic, string expectedAddress)
        {
            var keyPair = Mnemonics.GetKeypairFromMnemonic(mnemonic);
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