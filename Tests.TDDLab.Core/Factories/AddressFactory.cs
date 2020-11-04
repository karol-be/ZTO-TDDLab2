using TDDLab.Core.InvoiceMgmt;

namespace Tests.TDDLab.Core.Factories
{
    public static class AddressFactory
    {
        public static Address ValidAddress => new Address("a", "a", "a", "a");
        public static Address InvalidAddress => new Address(default, default, default, default);
    }
}