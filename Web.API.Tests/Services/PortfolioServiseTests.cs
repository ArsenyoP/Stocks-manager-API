using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Tests.Services
{
    public class PortfolioServiseTests
    {
        private readonly Mock<IPorrfolioRepository> _portfolioRepoMock;
        private readonly Mock<IStockRepository> _stockRepoMock;
        private readonly Mock<ILogger<PortfolioService>> _loggerMock;
        private readonly PortfolioService _portfolioService;

        public PortfolioServiseTests()
        {
            _portfolioRepoMock = new Mock<IPorrfolioRepository>();
            _stockRepoMock = new Mock<IStockRepository>();
            _loggerMock = new Mock<ILogger<PortfolioService>>();
            _portfolioService = new PortfolioService(_portfolioRepoMock.Object, _stockRepoMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task AddToPortfolio_WithValidData_ReturnsPortfolioModel()
        {
            var symbolUpper = "APPL";
            var stockID = 1;
            var isExists = false;
            var userID = "user_id_1";

            var portfolioModel = new Portfolio
            {
                StockId = stockID,
                AppUserId = userID
            };

            _stockRepoMock.Setup(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockID);

            _portfolioRepoMock.Setup(x => x.CheckIfExistsAsync(userID, stockID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(isExists);

            _portfolioRepoMock.Setup(x => x.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(portfolioModel);


            var result = await _portfolioService.AddToPortfolio(symbolUpper, userID, CancellationToken.None);


            result.AppUserId.Should().Be(userID);
            result.StockId.Should().Be(stockID);

            _stockRepoMock.Verify(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.CheckIfExistsAsync(userID, stockID, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.CreatePortfolioAsync(It.Is<Portfolio>(
                p => p.StockId == stockID && p.AppUserId == userID),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddToPortfolio_StockDoesntExists_ThrowKeyNotFoundException()
        {
            var symbolUpper = "APPL";
            var userID = "user_id";
            var stockID = 0;

            _stockRepoMock.Setup(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockID);


            Func<Task> act = async () => await _portfolioService.AddToPortfolio(symbolUpper, userID, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Can't find stock with symbol: APPL");

            _stockRepoMock.Verify(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.CheckIfExistsAsync(userID, stockID, It.IsAny<CancellationToken>()),
                Times.Never);

            _portfolioRepoMock.Verify(x => x.CreatePortfolioAsync(It.IsAny<Portfolio>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddToPortfolio_StockAlreadyExists_ThrowArgumentException()
        {
            var symbolUpper = "APPL";
            var userID = "user_id";
            var stockID = 1;
            var isExists = true;

            _stockRepoMock.Setup(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockID);

            _portfolioRepoMock.Setup(x => x.CheckIfExistsAsync(userID, stockID, CancellationToken.None))
                .ReturnsAsync(true);


            Func<Task> act = async () => await _portfolioService.AddToPortfolio(symbolUpper, userID, CancellationToken.None);


            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Can't add stock twice");

            _stockRepoMock.Verify(x => x.GetIdBySymbolAsync(symbolUpper, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.CheckIfExistsAsync(userID, stockID, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.CreatePortfolioAsync(It.IsAny<Portfolio>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePortfolioAsync_ExistingPortfolio_ExecuteDeleteAsync()
        {
            var symbolUpper = "APPL";
            var stockId = 1;
            var userId = "user_id";
            var portfolio = new Portfolio
            {
                StockId = stockId,
                AppUserId = userId
            };

            _portfolioRepoMock.Setup(x => x.GetByIdAndSymbol(userId, symbolUpper, It.IsAny<CancellationToken>()))
                .ReturnsAsync(portfolio);


            await _portfolioService.DeletePortfolioAsync(symbolUpper, userId, CancellationToken.None);


            _portfolioRepoMock.Verify(x => x.DeletePortfolioAsync(portfolio, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task DeletePortfolioAsync_NonExistingPortfolio_ThrowsKeyNotFoundException()
        {
            var symbolUpper = "APPL";
            var stockId = 1;
            var userId = "user_id";
            var portfolio = new Portfolio
            {
                StockId = stockId,
                AppUserId = userId
            };

            _portfolioRepoMock.Setup(x => x.GetByIdAndSymbol(userId, symbolUpper, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Portfolio?)null);


            Func<Task> act = async () => await _portfolioService.DeletePortfolioAsync(symbolUpper, userId, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Can't find stock with symbol: APPL");

            _portfolioRepoMock.Verify(x => x.GetByIdAndSymbol(userId, symbolUpper, It.IsAny<CancellationToken>()),
                Times.Once);

            _portfolioRepoMock.Verify(x => x.DeletePortfolioAsync(portfolio, It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
