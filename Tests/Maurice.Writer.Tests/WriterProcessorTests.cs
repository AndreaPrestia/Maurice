using Maurice.Domain.Entities;
using Maurice.Writer.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Maurice.Writer.Tests
{
    public class WriterProcessorTests
    {
        private readonly IHost _host;
        private readonly Mock<IEventEntityRepository> _mockEventEntityRepository = new();
        private readonly Mock<IEventTypeEntityRepository> _mockEventEntityTypeRepository = new();
        private readonly Mock<IErrorEntityRepository> _mockErrorEntityTypeRepository = new();

        public WriterProcessorTests()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(_mockEventEntityRepository.Object);
                    services.AddSingleton(_mockEventEntityTypeRepository.Object);
                    services.AddSingleton(_mockErrorEntityTypeRepository.Object);
                    services.AddScoped<WriterProcessor>();
                })
                .Build();
        }

        [Fact]
        public async Task WriteAsync_T_Ok()
        {
            // Arrange
            var testClass = new TestClass()
            {
                Name = "Name",
                Description = "Description"
            };

            var eventTypeEntity = new EventTypeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "TestClass",
                Created = new DateTimeOffset(DateTime.UtcNow.AddDays(-10)).ToUnixTimeMilliseconds(),
                Updated = new DateTimeOffset(DateTime.UtcNow.AddDays(-10)).ToUnixTimeMilliseconds(),
                Tags = new List<string>()
                {
                    "Tag1", "Tag2"
                }
            };

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventTypeEntity);

            _mockEventEntityRepository.Setup(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.IsAny<CancellationToken>()));

            var service = _host.Services.GetRequiredService<WriterProcessor>();

            // Act
            var result = await service.WriteAsync(testClass, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            _mockEventEntityRepository.Verify(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WriteAsync_T_Ko_Insert_Fail()
        {
            // Arrange
            var testClass = new TestClass()
            {
                Name = "Name",
                Description = "Description"
            };

            var eventTypeEntity = new EventTypeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "TestClass",
                Created = new DateTimeOffset(DateTime.UtcNow.AddDays(-10)).ToUnixTimeMilliseconds(),
                Updated = new DateTimeOffset(DateTime.UtcNow.AddDays(-10)).ToUnixTimeMilliseconds(),
                Tags = new List<string>()
                {
                    "Tag1", "Tag2"
                }
            };

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventTypeEntity);

            _mockEventEntityRepository.Setup(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("An exception has occurred"));
            _mockErrorEntityTypeRepository.Setup(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<Exception>(t => t.Message == "An exception has occurred"), It.IsAny<CancellationToken>()));

            var service = _host.Services.GetRequiredService<WriterProcessor>();

            // Act
            var result = await service.WriteAsync(testClass, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("An exception has occurred", result.ErrorMessage);
            _mockEventEntityRepository.Verify(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.IsAny<CancellationToken>()), Times.Once);
            _mockErrorEntityTypeRepository.Verify(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<Exception>(t => t.Message == "An exception has occurred"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReadAsync_T_Ko_No_EntityType_Found()
        {
            // Arrange
            var testClass = new TestClass()
            {
                Name = "Name",
                Description = "Description"
            };

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventTypeEntity?)null);
            _mockErrorEntityTypeRepository.Setup(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<InvalidOperationException>(t => t.Message == "EventType 'TestClass' not configured. Cannot proceed with processing."), It.IsAny<CancellationToken>()));

            var service = _host.Services.GetRequiredService<WriterProcessor>();

            // Act
            var result = await service.WriteAsync(testClass, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("EventType 'TestClass' not configured. Cannot proceed with processing.", result.ErrorMessage);
            _mockErrorEntityTypeRepository.Verify(x => x.InsertAsync(It.Is<TestClass>(t => t.Name == testClass.Name && t.Description == testClass.Description), It.Is<InvalidOperationException>(t => t.Message == "EventType 'TestClass' not configured. Cannot proceed with processing."), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReadAsync_T_Ko_No_Value()
        {
            // Arrange
            _mockErrorEntityTypeRepository.Setup(x => x.InsertAsync(It.Is<TestClass?>(t => t == null), It.Is<ArgumentNullException>(t => !string.IsNullOrWhiteSpace(t.Message)), It.IsAny<CancellationToken>()));

            var service = _host.Services.GetRequiredService<WriterProcessor>();

            // Act
            var result = await service.WriteAsync<TestClass>(null, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            _mockErrorEntityTypeRepository.Verify(x => x.InsertAsync(It.Is<TestClass?>(t => t == null), It.Is<ArgumentNullException>(t => !string.IsNullOrWhiteSpace(t.Message)), It.IsAny<CancellationToken>()), Times.Once);
        }

        public class TestClass
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}
