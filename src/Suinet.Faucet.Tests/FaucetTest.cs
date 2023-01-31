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
            var isSuccess = await faucet.AirdropGasAsync("0x7438811a9626623e093312d1b552f29567841f7e");

            isSuccess.Should().BeTrue();
        }
    }
}