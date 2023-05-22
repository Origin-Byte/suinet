using FluentAssertions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Suinet.Faucet.Tests
{
    public class FaucetTest
    {
        [Fact (Skip = "test / dev faucet are not public anymore, TODO")]
        public async Task TestAirdropGas()
        {
            var faucet = new FaucetClient();
            var isSuccess = await faucet.AirdropGasAsync("0x1a4f2b04e99311b0ff8228cf12735402f6618d7be0f0b320364339baf03e49df");

            isSuccess.Should().BeTrue();
        }
    }
}