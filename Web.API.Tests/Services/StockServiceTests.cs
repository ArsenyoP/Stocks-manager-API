using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Web.API.Dtos.Stock;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Services;
using MockQueryable.Moq;
using MockQueryable;
using Web.API.Helpers;
using System.Runtime.CompilerServices;
using Web.API.Mappers;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Net.WebSockets;
using Web.API.Exceptions;
using Web.API.Interfaces.IServices;

namespace Web.API.Tests.Services;

public class StockServiceTests
{
    private readonly Mock<IStockRepository> _mockRepo;
    private readonly Mock<ILogger<StockService>> _loggerMock;
    private readonly StockService _service;
    private readonly Mock<IFinancialService> _financialServiceMock;

    public StockServiceTests()
    {
        _mockRepo = new Mock<IStockRepository>();
        _loggerMock = new Mock<ILogger<StockService>>();
        _financialServiceMock = new Mock<IFinancialService>();
        _service = new StockService(_mockRepo.Object, _loggerMock.Object, _financialServiceMock.Object);
       
    }

    private List<Stock> GetTestStocks()
    {
        return new List<Stock>
        {
            new Stock { ID = 1, Symbol = "APPL", CompanyName = "Apple", MarketCap = 3000000000, LastDiv = 0.24m, Industy = "Technology" },
            new Stock { ID = 2, Symbol = "MSFT", CompanyName = "Microsoft Corporation", MarketCap = 2800000000, LastDiv = 0.75m, Industy = "Technology" },
            new Stock { ID = 3, Symbol = "TSLA", CompanyName = "Tesla Inc.", MarketCap = 800000000, LastDiv = 0.20m, Industy = "Automotive" },
            new Stock { ID = 4, Symbol = "AMZN", CompanyName = "Amazon.com Inc.", MarketCap = 1500000000, LastDiv = 0.01m, Industy = "Retail" },
            new Stock { ID = 5, Symbol = "NVDA", CompanyName = "NVIDIA Corporation", MarketCap = 1200000000, LastDiv = 0.04m, Industy = "Semiconductors" },
            new Stock { ID = 6, Symbol = "NFLX", CompanyName = "Netflix Inc.", MarketCap = 200000000, LastDiv = 0.00m, Industy = "Entertainment" },
            new Stock { ID = 7, Symbol = "META", CompanyName = "Meta Platforms Inc.", MarketCap = 900000000, LastDiv = 0.50m, Industy = "Social Media" },
            new Stock { ID = 8, Symbol = "JPM", CompanyName = "JPMorgan Chase & Co.", MarketCap = 500000000, LastDiv = 1.05m, Industy = "Banking" },
            new Stock { ID = 9, Symbol = "APPL2", CompanyName = "Apple", MarketCap = 3000000000, LastDiv = 0.24m, Industy = "Technology" }
        };
    }


    [Fact]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllStocks()
    {
        var stocks = GetTestStocks();

        var mockQueryable = stocks.BuildMock();

        _mockRepo
            .Setup(x => x.GetAllQuery())
            .Returns(mockQueryable);


        var result = await _service.GetAllAsync(new QueryObject());


        result.Should().NotBeNull();
        result.Should().HaveCount(9);
        result.Should().BeInAscendingOrder(x => x.ID);
        result[0].Symbol.Should().Be("APPL");
    }

    [Fact]
    public async Task GetAllAsync_WhenDatabaseIsEmpty_ReturnsEmptyList()
    {
        var stocksMock = new List<Stock> { };

        var mockQuearyable = stocksMock.BuildMock();

        _mockRepo.Setup(x => x.GetAllQuery())
            .Returns(mockQuearyable);


        var result = await _service.GetAllAsync(new QueryObject());


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(true, "dividends")]
    public async Task GetAllAsync_WhithFiltres_ReturnFiltredStocks(
        bool isDescending, string sortBy)
    {
        var stocks = GetTestStocks();

        var mockQuearyable = stocks.BuildMock();

        var queryObject = new QueryObject
        {
            IsDescending = isDescending,
            CompanyName = "Apple",
            SortBy = sortBy
        };

        _mockRepo.Setup(x => x.GetAllQuery())
            .Returns(mockQuearyable);


        var result = await _service.GetAllAsync(queryObject);


        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(x => x.LastDiv);
        result.All(x => x.CompanyName == "Apple").Should().BeTrue();
    }


    [Fact]
    public async Task GetById_WithExistingId_ReturnsStockWithCertainId()
    {
        int stockId = 10;
        var user = new AppUser { UserName = "testName" };
        var comments = Enumerable.Range(0, 21).Select(i =>
        new Comment
        {
            ID = i + 1,
            Title = $"Title:{i + 1}",
            Content = $"Content: {i + 1}",
            AppUser = user,
            CreatedOn = DateTime.Now.AddDays(i + 1)
        }).ToList();

        var stockForSearching = new Stock
        {
            ID = 10,
            Comments = comments
        };

        var stocks = GetTestStocks();
        stocks.Add(stockForSearching);

        var mockQuery = stocks.BuildMock();
        _mockRepo.Setup(x => x.GetAllQuery())
            .Returns(mockQuery);


        var result = await _service.GetById(stockId);

        result.ID.Should().Be(10);
        result.Comments.Should().HaveCount(20);
        result.Comments[0].Title.Should().Be("Title:21");
        result.Comments[19].Title.Should().Be("Title:2");
    }

    [Fact]
    public async Task GetById_WithIdThatDoesntExists_ThrowsNotFoundException()
    {
        int stockId = 999;
        var stocksMock = GetTestStocks();

        var mockQuery = stocksMock.BuildMock();
        _mockRepo.Setup(x => x.GetAllQuery())
            .Returns(mockQuery);


        Func<Task> act = async () => await _service.GetById(stockId);


        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Can't find stock with ID: 999");

    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedStock()
    {
        var createDto = new CreateStockRequestDto
        {
            Symbol = "GOOGL",
            CompanyName = "Alphabet Inc.",
            Purchase = 150,
            LastDiv = 0.50m,
            Industy = "Technology",
            MarketCap = 1800000000
        };

        _mockRepo
            .Setup(x => x.SymbolExists(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepo
            .Setup(x => x.CreateAsync(It.IsAny<Stock>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stock stock, CancellationToken ct) =>
            {
                stock.ID = 100;
                return stock;
            });


        var result = await _service.Create(createDto, new CancellationToken { });


        result.Should().NotBeNull();
        result.Symbol.Should().Be("GOOGL");
        result.ID.Should().Be(100);

        _mockRepo.Verify(x =>
            x.SymbolExists(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once, "Should check if symbol exists before creating"
            );

        _mockRepo.Verify(x =>
        x.CreateAsync(It.IsAny<Stock>(), It.IsAny<CancellationToken>()),
        Times.Once, "Should call CreateAsync once");
    }

    [Fact]
    public async Task Create_WithExistingSymbol_ReturnsArgumentException()
    {
        var createDto = new CreateStockRequestDto
        {
            Symbol = "APPL",
            CompanyName = "Apple Inc.",
            Purchase = 150,
            LastDiv = 0.24m,
            Industy = "Technology",
            MarketCap = 2000000000
        };

        _mockRepo
            .Setup(x => x.SymbolExists("APPL", 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        Func<Task> act = async () => await _service.Create(createDto, new CancellationToken { });


        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Stock with symbol APPL already exists");

        _mockRepo.Verify(
            x => x.CreateAsync(It.IsAny<Stock>(), It.IsAny<CancellationToken>())
            , Times.Never, "Should not call CreateAsync when symbol already exists");
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsUpdatedStock()
    {
        var stockId = 1;
        var updateDto = new UpdateStockRequestDto
        {
            CompanyName = "Microsoft",
            Symbol = "MCRSF"
        };
        var updatedStock = new Stock { ID = stockId, CompanyName = "Microsoft", Symbol = "MCRSF" };

        _mockRepo.Setup(x => x.SymbolExists(updateDto.Symbol, stockId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepo.Setup(x => x.UpdateAsync(stockId, updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedStock);


        var result = await _service.Update(stockId, updateDto, new CancellationToken { });


        result.Should().NotBeNull();
        result.Symbol.Should().Be("MCRSF");
        result.CompanyName.Should().Be("Microsoft");

        _mockRepo.Verify(
            x => x.UpdateAsync(stockId, updateDto, new CancellationToken { }),
            Times.Once, "Shlould execute UpdateAsync onece");
    }

    [Fact]
    public async Task Update_WithExistingSymbol_ThrowArgumentException()
    {
        var stockId = 1;
        var updateDto = new UpdateStockRequestDto
        {
            Symbol = "APPl"
        };

        _mockRepo.Setup(x => x.SymbolExists("APPL", stockId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        Func<Task> act = async () => await _service.Update(stockId, updateDto, new CancellationToken { });


        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Stock with this symbol already exists");

        _mockRepo.Verify(x => x.UpdateAsync(stockId, updateDto, new CancellationToken { }),
            Times.Never, "Should not call UpdateAsync if stock with this symbol already exists");
    }

    [Fact]
    public async Task Update_WithNonExistingStock_ThrowKeyNotFoundException()
    {
        var stockId = 999;
        var updateDto = new UpdateStockRequestDto { Symbol = "APPL" };

        _mockRepo.Setup(x => x.SymbolExists(updateDto.Symbol, stockId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockRepo.Setup(x => x.UpdateAsync(stockId, It.IsAny<UpdateStockRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stock?)null);


        Func<Task> act = async () => await _service.Update(stockId, updateDto, new CancellationToken { });


        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Stock with ID 999 not found");

        _mockRepo.Verify(x => x.UpdateAsync(stockId, It.IsAny<UpdateStockRequestDto>(), It.IsAny<CancellationToken>()),
            Times.Once, "Should call UpdateAsync once");
    }

    [Fact]
    public async Task Delete_WithExistingStock_ExecuteDeleteMethod()
    {
        var stockId = 1;
        var stock = new Stock { Symbol = "APPl" };

        _mockRepo.Setup(x => x.GetByIdAsync(stockId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stock);


        await _service.Delete(stockId, new CancellationToken { });


        _mockRepo.Verify(x => x.DeleteAsync(stock, It.IsAny<CancellationToken>()),
            Times.Once, "Should call DeleteAsync once");
    }

    [Fact]
    public async Task Delete_WithNonExistingStock_ThrowKeyNotFoundException()
    {
        var stockId = 1;

        _mockRepo.Setup(x => x.GetByIdAsync(stockId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stock?)null);

        Func<Task> act = async () => await _service.Delete(stockId, new CancellationToken { });


        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Can't find stock with ID: 1");

        _mockRepo.Verify(x => x.DeleteAsync(It.IsAny<Stock>(), It.IsAny<CancellationToken>()),
            Times.Never, "Should not call DeleteAsync when Stock doesn not exists");
    }


}
