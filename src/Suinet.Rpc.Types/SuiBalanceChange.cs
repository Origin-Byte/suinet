namespace Suinet.Rpc.Types
{
    public class SuiBalanceChange
    {
        public string Amount { get; set; }

        public string CoinType { get; set; }

        public Owner Owner { get; set; }
    }
}
