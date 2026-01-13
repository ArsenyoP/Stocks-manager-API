# Stocks Manager API 📈

A robust RESTful API built with **.NET 9.0** for managing stock market data and user portfolios. This project demonstrates modern backend architecture and clean code principles.

## 🚀 Features
- **Full CRUD operations** for Stocks and Comments.
- **Advanced Filtering** & Sorting for stock data.
- **JWT Authentication** for secure user registration and login.
- **Portfolio Management**: Users can add or remove stocks from their personal portfolios.

## 🛠 Tech Stack
- **Framework:** .NET 9.0 (ASP.NET Core Web API)
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Security:** Identity Framework & JWT
- **Patterns:** Repository Pattern, DTOs, Dependency Injection

## 🏗 Architecture
The project follows a decoupled architecture using the **Repository Pattern** to ensure the business logic is separated from data access, making the code easier to test and maintain.

## 🚦 Getting Started
1. Clone the repo: `git clone https://github.com/ArsenyoP/Stocks-manager-API.git`
2. Update `appsettings.json` with your SQL Server connection string.
3. Run `dotnet ef database update` to apply migrations.
4. Hit `F5` or `dotnet run` to launch Swagger UI.
