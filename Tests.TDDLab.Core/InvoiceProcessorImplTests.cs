using System.Collections.Generic;
using FluentAssertions;
using TDDLab.Core.InvoiceMgmt;
using Tests.TDDLab.Core.Utils;
using Xunit;

namespace Tests.TDDLab.Core
{
    public class InvoiceProcessorImplTests
    {
        readonly Invoice _invoice;
        readonly InvoiceProcessorImpl _sut;

        ProcessingResult Result { get; set; }

        InvoiceProcessorImplTests(Invoice invoice)
        {
            _invoice = invoice;
            _sut = new InvoiceProcessorImpl();
        }

        void Act()
        {
            Result = _sut.Process(_invoice);
        }

        public class InvalidInvoiceTests : InvoiceProcessorImplTests
        {
            public InvalidInvoiceTests() : base(InvalidInvoice.Get())
            {
            }

            [Fact]
            void ShouldReturnFailedResult()
            {
                Act();

                // assert
                Result.ShouldBeFailed();
            }

            [Fact]
            void ShouldLeaveProductsUnchanged()
            {
                // arrange
                var initialProducts = _sut.Products.GetAsReadOnly();

                Act();

                // assert
                initialProducts.Should().BeEquivalentTo(_sut.Products);
            }
        }


        public class ValidInvoiceTests : InvoiceProcessorImplTests
        {
            IList<InvoiceLine> Lines { get; }

            ValidInvoiceTests(IList<InvoiceLine> lines) : base(ValidInvoice.GetWithNonZeroDiscount(lines))
            {
                Lines = (_invoice as ValidInvoice)?.Lines;
            }

            void ActAndValidateItReturnedSuccessfulResult()
            {
                Act();
                Result.ShouldBeSucceeded();
            }

            public class ForOneUniqueLine : ValidInvoiceTests
            {
                public ForOneUniqueLine() : base(InvoiceLines.OneUniqueLine())
                {
                }

                [Fact]
                void ShouldContainSingleProductWithItsMoney()
                {
                    Act();

                    // assert
                    _sut.Products.Should().Contain(Lines[0].ProductName, Lines[0].Money);
                }

                [Fact] void ShouldReturnSuccessfulResult() => ActAndValidateItReturnedSuccessfulResult();
            }

            public class ForTwoLinesWithSameProductNameAndDifferentMoney : ValidInvoiceTests
            {
                public ForTwoLinesWithSameProductNameAndDifferentMoney() : base(InvoiceLines
                    .TwoWithSameProductNameWithDifferentMoney())
                {
                }

                [Fact]
                void ShouldContainOneProduct()
                {
                    Act();

                    // assert
                    _sut.Products.Keys.Should().ContainSingle(k => k == Lines[0].ProductName);
                }

                [Fact]
                void ShouldHaveProductMoneySummedWithDiscountDeductedFromSecondItemMoney()
                {
                    // arrange
                    var correctAmountForProduct = Lines[0].Money + (Lines[1].Money - _invoice.Discount);

                    Act();

                    // assert
                    _sut.Products.Values.Should().ContainSingle(v => v == correctAmountForProduct);
                }
                
                [Fact] void ShouldReturnSuccessfulResult() => ActAndValidateItReturnedSuccessfulResult();

            }

            public class ForThreeLinesWithSameProductNameAndDifferentMoney : ValidInvoiceTests
            {
                public ForThreeLinesWithSameProductNameAndDifferentMoney()
                    : base(InvoiceLines.ThreeWithSameProductNameWithDifferentMoney())
                {
                }

                [Fact]
                void ShouldHaveMoneySummedAndDiscountDeductedFromSecondAndThirdItemMoney()
                {
                    // arrange
                    var secondItemMoneyAfterDiscount = Lines[1].Money - _invoice.Discount;
                    var thirdItemMoneyAfterDiscount = Lines[2].Money - _invoice.Discount;
                    var correctAmountForProduct =
                        Lines[0].Money + secondItemMoneyAfterDiscount + thirdItemMoneyAfterDiscount;

                    Act();

                    // assert
                    _sut.Products.Should().Contain(Lines[0].ProductName, correctAmountForProduct);
                }
            }

            public class TwoUniqueLinesTests : ValidInvoiceTests
            {
                public TwoUniqueLinesTests() : base(InvoiceLines.TwoUnique())
                {
                }

                [Fact]
                void ShouldContainTwoProducts()
                {
                    Act();

                    // assert
                    using var _ = new AssertionScope();
                    _sut.Products.Should().Contain(Lines[0].ProductName, Lines[0].Money);
                    _sut.Products.Should().Contain(Lines[1].ProductName, Lines[1].Money);
                }
                
                [Fact] void ShouldReturnSuccessfulResult() => ActAndValidateItReturnedSuccessfulResult();
            }
        }
    }
}
