using TDDLab.Core.InvoiceMgmt;
using Xunit;

namespace Tests.TDDLab.Core.Utils
{
    public static class ProcessingResultAssertions
    {
        public static void ShouldBeSucceeded(this ProcessingResult result)
        {
            Assert.Equal(result.Result, InvoiceResult.Succeeded);
        }
        
        public static void ShouldBeFailed(this ProcessingResult result)
        {
            Assert.Equal(result.Result, InvoiceResult.Failed);
        }
    }
}