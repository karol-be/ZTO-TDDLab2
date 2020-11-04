using System;
using System.Collections.Generic;
using TDDLab.Core.InvoiceMgmt;
using Tests.TDDLab.Core.Factories;

namespace Tests.TDDLab.Core.Utils
{

    public static class ValidInvoiceLineFactory
    {
        const string DefaultProductName = "punkty ECTS";

        public static InvoiceLine Get(string productName, Money money) => new InvoiceLine(productName, money);

        public static InvoiceLine GetWithZeroMoney(string productName = DefaultProductName) =>
            new InvoiceLine(productName, Money.ZERO);

        public static InvoiceLine GetWithDefaultName(Money money) => new InvoiceLine(DefaultProductName, money);
    }


    public class InvoiceLines
    {
        public static IList<InvoiceLine> OneUniqueLine() => new[]
        {
            ValidInvoiceLineFactory.GetWithZeroMoney()
        };

        public static IList<InvoiceLine> TwoWithSameProductNameWithDifferentMoney() => new[]
        {
            ValidInvoiceLineFactory.GetWithDefaultName(MoneyFactory.ValidDefaultCurrency(20)),
            ValidInvoiceLineFactory.GetWithDefaultName(MoneyFactory.ValidDefaultCurrency(10))
        };
        
        public static IList<InvoiceLine> ThreeWithSameProductNameWithDifferentMoney() => new[]
        {
            ValidInvoiceLineFactory.GetWithDefaultName(MoneyFactory.ValidDefaultCurrency(20)),
            ValidInvoiceLineFactory.GetWithDefaultName(MoneyFactory.ValidDefaultCurrency(10)),
            ValidInvoiceLineFactory.GetWithDefaultName(MoneyFactory.ValidDefaultCurrency(30))
        };

        public static IList<InvoiceLine> TwoUnique() => new[]
        {
            ValidInvoiceLineFactory.Get("Testy Data Driven", MoneyFactory.ValidDefaultCurrency(20_000)),
            ValidInvoiceLineFactory.Get("Testy Behaviour Driven", MoneyFactory.ValidDefaultCurrency(40_000))
        };
    }


    public class InvalidInvoice : Invoice
    {
        public static Invoice Get() => new InvalidInvoice();
    }

    public class ValidInvoice : Invoice
    {
        const string ValidNumber = "123";
        
        private ValidInvoice(IList<InvoiceLine> lines, Money discount) 
            : base(ValidNumber, RecipientsFactory.ValidRecipient, AddressFactory.ValidAddress, lines, discount)
        {
            Lines = lines;
            Discount = discount;
        }
        
        public new IList<InvoiceLine> Lines { get; private set; }
        public new Money Discount { get; private set; }
        
        public static Invoice Get() => new ValidInvoice(Array.Empty<InvoiceLine>(), Money.ZERO);
        public static Invoice Get(IList<InvoiceLine> lines) => new ValidInvoice ( lines, Money.ZERO );
        public static Invoice GetWithNonZeroDiscount(IList<InvoiceLine> lines) => new ValidInvoice(lines, MoneyFactory.ValidDefaultCurrency(30));
    }
}
