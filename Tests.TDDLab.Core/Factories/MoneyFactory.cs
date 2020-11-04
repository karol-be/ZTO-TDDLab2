using TDDLab.Core.InvoiceMgmt;

 namespace Tests.TDDLab.Core.Factories
{
    public static class MoneyFactory
    {
        public static Money ValidDefaultCurrency() => new Money(0);
        public static Money ValidDefaultCurrency(ulong amount) => new Money(amount);
        public static Money Invalid() => new Money(0, string.Empty);
    }
}