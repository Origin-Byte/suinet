using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Suinet.Faucet.Tests
{
    public class FaucetTest
    {
        [Fact]
        public async Task TestAirdropGas()
        {
            var faucet = new FaucetClient();
            var isSuccess = await faucet.AirdropGasAsync("0xa106c6d490ff692411bc6fd2ca59b5804adcac04");

            isSuccess.Should().BeTrue();
        }
    }
}