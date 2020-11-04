using System;
using Moq;
using TDDLab.Core.Infrastructure;
using TDDLab.Core.InvoiceMgmt;
using Tests.TDDLab.Core.Utils;
using Xunit;

namespace Tests.TDDLab.Core
{
    public class WorkerImplTests
    {
        readonly Mock<IConfigurationSettings> _configurationSettingsMock = new Mock<IConfigurationSettings>();

        readonly Mock<IMessagingFacility<Invoice, ProcessingResult>> _messagingFacilityMock =
            new Mock<IMessagingFacility<Invoice, ProcessingResult>>();

        readonly Mock<IExceptionHandler> _exceptionHandlerMock = new Mock<IExceptionHandler>();
        readonly Mock<IInvoiceProcessor> _invoiceProcessorMock = new Mock<IInvoiceProcessor>();

        readonly Message<Invoice> _inputMessage = new Message<Invoice>
        {
            Data = ValidInvoice.Get(),
            Metadata = new Metadata()
        };

        readonly ProcessingResult _processingResult = new ProcessingResult();

        readonly WorkerImpl _sut;

        protected WorkerImplTests()
        {
            _sut = new WorkerImpl(
                _configurationSettingsMock.Object,
                _messagingFacilityMock.Object,
                _exceptionHandlerMock.Object,
                _invoiceProcessorMock.Object);
        }

        void SetupMessageFacilityReadMessageSuccessfully()
        {
            _messagingFacilityMock.Setup(mf => mf.ReadMessage())
                .Returns(_inputMessage);
        }

        void SetupInvoiceProcessorProcessSuccessfully()
        {
            _invoiceProcessorMock.Setup(ip => ip.Process(_inputMessage.Data))
                .Returns(_processingResult);
        }

        public class StartTests : WorkerImplTests
        {
            string InputQueueSetting = "kolejkaDoDziekanatu";
            string OutputQueueSetting = "kolejkaDoAutomatuZKawa";

            public StartTests()
            {
                _configurationSettingsMock.Setup(c => c.GetSettingsByKey("inputQueue"))
                    .Returns(InputQueueSetting);

                _configurationSettingsMock.Setup(c => c.GetSettingsByKey("outputQueue"))
                    .Returns(OutputQueueSetting);

                _sut.Start();
            }

            [Fact]
            void ShouldInitializeInputChannelWithInputQueueSetting()
            {
                // assert
                _messagingFacilityMock.Verify(m => m.InitializeInputChannel(InputQueueSetting), Times.Once);
            }

            [Fact]
            void ShouldInitializeOutputChannelWithOutputQueueSetting()
            {
                // assert
                _messagingFacilityMock.Verify(m => m.InitializeOutputChannel(OutputQueueSetting), Times.Once);
            }
        }

        public class StopTests : WorkerImplTests
        {
            public StopTests()
            {
                _sut.Stop();
            }

            [Fact]
            void ShouldDisposeMessagingFacility()
            {
                _messagingFacilityMock.Verify(mf => mf.Dispose(), Times.Once);
            }
        }

        public class DoJobTests
        {
            public class OnSuccessfulFlow : WorkerImplTests
            {
                public OnSuccessfulFlow()
                {
                    SetupMessageFacilityReadMessageSuccessfully();
                    SetupInvoiceProcessorProcessSuccessfully();

                    _sut.DoJob();
                }

                [Fact]
                void ShouldProcessInputMessage()
                {
                    _invoiceProcessorMock.Verify(ip => ip.Process(_inputMessage.Data), Times.Once);
                }

                [Fact]
                void ShouldWriteMessageBasedOnMetadataAndProcessingResult()
                {
                    _messagingFacilityMock.Verify(mf =>
                        mf.WriteMessage(It.Is<Message<ProcessingResult>>(message =>
                            message.Data.Equals(_processingResult) && message.Metadata == _inputMessage.Metadata)));
                }
            }


            public abstract class ForExceptionOn : WorkerImplTests
            {
                readonly Exception _ex = new Exception();

                protected ForExceptionOn()
                {
                    Setup();
                    _sut.DoJob();
                }

                protected abstract void Setup();

                void VerifyExceptionPassedToHandler()
                {
                    _exceptionHandlerMock.Verify(eh => eh.HandleException(_ex));
                }

                public class ReadingMessage : ForExceptionOn
                {
                    protected override void Setup()
                    {
                        _messagingFacilityMock.Setup(x => x.ReadMessage()).Throws(_ex);
                    }

                    [Fact]
                    void ShouldNotProcess()
                    {
                        _invoiceProcessorMock.Verify(ip => ip.Process(It.IsAny<Invoice>()), Times.Never);
                    }

                    [Fact]
                    void ShouldNotWriteMessage()
                    {
                        _messagingFacilityMock.Verify(mf => mf.WriteMessage(It.IsAny<Message<ProcessingResult>>()),
                            Times.Never);
                    }

                    [Fact]
                    void ShouldNotPassExceptionToHandler()
                    {
                        VerifyExceptionPassedToHandler();
                    }
                }

                public class Processing : ForExceptionOn
                {
                    protected override void Setup()
                    {
                        SetupMessageFacilityReadMessageSuccessfully();

                        _invoiceProcessorMock.Setup(ip => ip.Process(It.IsAny<Invoice>()))
                            .Throws(_ex);
                    }

                    [Fact]
                    void ShouldNotWriteMessage()
                    {
                        _messagingFacilityMock.Verify(mf => mf.WriteMessage(It.IsAny<Message<ProcessingResult>>()),
                            Times.Never);
                    }

                    [Fact]
                    void ShouldNotPassExceptionToHandler()
                    {
                        VerifyExceptionPassedToHandler();
                    }
                }

                public class OnWritingMessage : ForExceptionOn
                {
                    protected override void Setup()
                    {
                        SetupMessageFacilityReadMessageSuccessfully();
                        SetupInvoiceProcessorProcessSuccessfully();

                        _messagingFacilityMock.Setup(mf => mf.WriteMessage(It.IsAny<Message<ProcessingResult>>()))
                            .Throws(_ex);
                    }

                    [Fact]
                    void ShouldNotPassExceptionToHandler()
                    {
                        VerifyExceptionPassedToHandler();
                    }
                }
            }
        }
    }
}