using Maurice.Domain.Entities;
using Maurice.Reader.Abstractions.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Maurice.Reader.Tests
{
    public class ReaderProcessorTests
    {
        private readonly IHost _host;

        private readonly Mock<IEventEntityRepository> _mockEventEntityRepository = new();
        private readonly Mock<IEventTypeEntityRepository> _mockEventEntityTypeRepository = new();

        public ReaderProcessorTests()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(_mockEventEntityRepository.Object);
                    services.AddSingleton(_mockEventEntityTypeRepository.Object);
                    services.AddScoped<ReaderProcessor>();
                })
                .Build();
        }

        [Fact]
        public async Task ReadAsync_T_Ok()
        {
            // Arrange
            var start = new DateTimeOffset(DateTime.UtcNow.AddDays(-3)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

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

            var eventEntities = new List<EventEntity>()
            {
                new()
                {
                    Body = @"{""name"": ""Name 1"", ""description"": ""Description 1""}",
                    EventTypeId = eventTypeEntity.Id,
                    Id = Guid.NewGuid(),
                    Timestamp = new DateTimeOffset(DateTime.UtcNow.AddDays(-2)).ToUnixTimeMilliseconds()
                },
                new()
                {
                    Body = @"{""name"": ""Name 2"", ""description"": ""Description 2""}",
                    EventTypeId = eventTypeEntity.Id,
                    Id = Guid.NewGuid(),
                    Timestamp = new DateTimeOffset(DateTime.UtcNow.AddDays(-1)).ToUnixTimeMilliseconds()
                }
            };

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventTypeEntity);

            _mockEventEntityRepository.Setup(x => x.ReadAsync(It.Is<long>(t => t == start), It.Is<long>(t => t == end), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.Is<bool>(t => t), It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntities);

            var service = _host.Services.GetRequiredService<ReaderProcessor>();

            // Act
            var result = await service.ReadAsync<TestClass>(start, end, true, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Collection(result.Items, e =>
            {
                Assert.NotNull(e);
                Assert.Equal("Name 1", e.Name);
                Assert.Equal("Description 1", e.Description);
            }, e =>
            {
                Assert.NotNull(e);
                Assert.Equal("Name 2", e.Name);
                Assert.Equal("Description 2", e.Description);
            });
        }

        [Fact]
        public async Task ReadAsync_T_Ok_Empty()
        {
            // Arrange
            var start = new DateTimeOffset(DateTime.UtcNow.AddDays(-3)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

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

            _mockEventEntityRepository.Setup(x => x.ReadAsync(It.Is<long>(t => t == start), It.Is<long>(t => t == end), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.Is<bool>(t => t), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventEntity>());

            var service = _host.Services.GetRequiredService<ReaderProcessor>();

            // Act
            var result = await service.ReadAsync<TestClass>(start, end, true, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task ReadAsync_T_Ok_Null_Empty()
        {
            // Arrange
            var start = new DateTimeOffset(DateTime.UtcNow.AddDays(-3)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

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

            _mockEventEntityRepository.Setup(x => x.ReadAsync(It.Is<long>(t => t == start), It.Is<long>(t => t == end), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.Is<bool>(t => t), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IList<EventEntity>?)null);

            var service = _host.Services.GetRequiredService<ReaderProcessor>();

            // Act
            var result = await service.ReadAsync<TestClass>(start, end, true, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task ReadAsync_T_Ko_No_EntityType_Found()
        {
            // Arrange
            var start = new DateTimeOffset(DateTime.UtcNow.AddDays(-3)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventTypeEntity?)null);

            var service = _host.Services.GetRequiredService<ReaderProcessor>();

            // Act
            var result = await service.ReadAsync<TestClass>(start, end, true, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("EventType 'TestClass' not configured. Cannot proceed with processing.", result.ErrorMessage);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task ReadAsync_T_Ko_Failed_Deserialization()
        {
            // Arrange
            var start = new DateTimeOffset(DateTime.UtcNow.AddDays(-3)).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

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

            var eventEntities = new List<EventEntity>()
            {
                new()
                {
                    Body = @"{""name1"": ""Name 1"", ""description1"" ""Description 1""}",
                    EventTypeId = eventTypeEntity.Id,
                    Id = Guid.NewGuid(),
                    Timestamp = new DateTimeOffset(DateTime.UtcNow.AddDays(-2)).ToUnixTimeMilliseconds()
                },
                new()
                {
                    Body = @"{""name"": ""Name 2"", ""description"": ""Description 2""}",
                    EventTypeId = eventTypeEntity.Id,
                    Id = Guid.NewGuid(),
                    Timestamp = new DateTimeOffset(DateTime.UtcNow.AddDays(-1)).ToUnixTimeMilliseconds()
                }
            };

            _mockEventEntityTypeRepository.Setup(x => x.GetAsync<TestClass>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventTypeEntity);

            _mockEventEntityRepository.Setup(x => x.ReadAsync(It.Is<long>(t => t == start), It.Is<long>(t => t == end), It.Is<EventTypeEntity>(t => t.Id == eventTypeEntity.Id && t.Name == eventTypeEntity.Name && t.Tags.SequenceEqual(eventTypeEntity.Tags)), It.Is<bool>(t => t), It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntities);

            var service = _host.Services.GetRequiredService<ReaderProcessor>();

            // Act
            var result = await service.ReadAsync<TestClass>(start, end, true, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Empty(result.Items);
        }

        public class TestClass
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}
