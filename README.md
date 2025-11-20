# ⚡ CoinPulse: Professional Crypto Market Dashboard

**CoinPulse** is a high-performance desktop application built with **WPF (.NET 8)**. The project demonstrates modern software development practices, including **Clean Architecture**, reactive UI, network resilience, and Unit Testing coverage.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4) ![WPF](https://img.shields.io/badge/UI-WPF-blue) ![Status](https://img.shields.io/badge/Status-Completed-success)

---

##  Overview & Demo

The application provides real-time cryptocurrency market data, allows for investment portfolio management, and supports multi-currency functionality within a modern dark-themed interface.

### Live Demonstration

![CoinPulse_Git_1](https://github.com/user-attachments/assets/3b88f6df-eab7-4340-9377-302e4f9c01b1)

---

##  Core Features & Functionality

* **Real-time Data & Auto-Refresh:** Fetches top 50 coins from CoinGecko API, supporting scheduled updates every 60 seconds.
* **Multi-Currency Support:** Allows switching between USD, EUR, BTC, ETH, and other currencies with dynamic price recalculation.
* **Portfolio Tracker (PnL):** Users can input their coin holdings; the application calculates the **Total Portfolio Value** and the **24-hour Profit & Loss (PnL)** percentage, calculated via a weighted average, which is displayed in the header.
* **Offline Resilience (Caching):** Data is cached locally using SQLite, enabling the application to display the last known prices instantly upon startup or during network outages.
* **Visualization:**
    * **Sparklines:** Mini charts showing 7-day price history for every coin in the main list.
    * **Pie Chart:** A dedicated modal window visualizing asset allocation in the user's portfolio.

### Portfolio Distribution

The portfolio panel (accessible by clicking the "Portfolio" badge) provides a clear visualization of asset distribution using a Pie Chart.

<img width="977" height="696" alt="Screenshot 2025-11-20 165930" src="https://github.com/user-attachments/assets/c1b915c1-cd0b-4b57-8cf8-5ee0b8299aa9" />

### UI/UX Details

The application features a dark theme built on a custom WPF resource dictionary, including soft entrance animations and custom controls for a polished desktop experience.

<img width="853" height="632" alt="Screenshot 2025-11-20 165959" src="https://github.com/user-attachments/assets/d6a1545d-b371-4f0a-9f82-83aed4be3113" />

##  Technology Stack

| Component | Technology / Library | Purpose |
| :--- | :--- | :--- |
| **Platform** | .NET 8, WPF | Base framework for desktop development. |
| **Architecture** | Clean Architecture | Separation of concerns into layers (Core, Services, UI). |
| **MVVM** | CommunityToolkit.Mvvm | Simplifies Model-View-ViewModel pattern implementation. |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection | Dependency injection and object lifecycle management. |
| **Database** | Entity Framework Core (SQLite) | ORM for local data caching and portfolio storage. |
| **Network** | HttpClient + **Polly** | Handling HTTP requests with retry policies. |
| **Charts** | LiveCharts2 (SkiaSharp) | High-performance GPU-accelerated chart rendering. |
| **Testing** | xUnit, Moq | Unit testing for business logic and ViewModels. |

---

##  Solution Architecture

The project follows **Clean Architecture** principles to ensure loose coupling and testability:

```text
CoinPulse.sln
├── 📂 CoinPulse.Core       
│   │   # Contains pure POCO classes (Entities) with no dependencies.
│   └── Coin.cs
│
├── 📂 CoinPulse.Services  
│   │   # Implementation of data access, API, and database logic.
│   ├── CoinGeckoService.cs (HttpClient + Polly)
│   └── AppDbContext.cs (EF Core SQLite)
│
├── 📂 CoinPulse.UI         
│   │   # Display logic and user interaction.
│   ├── ViewModels (MainViewModel, CoinViewModel)
│   ├── Services (NavigationService, DispatcherService)
│   └── Views (MainWindow, PortfolioWindow)
│
└── 📂 CoinPulse.Tests     
        # Isolated unit tests.

```

##  Technical Deep Dive

### 1. MVVM Pattern & Testability
The project strictly follows the MVVM pattern, completely decoupling the UI from business logic.

* **Navigation Abstraction (`INavigationService`):** The ViewModel never creates windows (`new Window()`) directly. This is delegated to a service, allowing ViewModel logic to be tested without initializing the UI.
* **Threading Abstraction (`IDispatcherService`):** A major issue in WPF testing is the dependency on `Application.Current.Dispatcher`. We introduced an abstraction that uses `Dispatcher.Invoke` in the real app but executes code synchronously in tests. This eliminates `NullReferenceException` during testing.

### 2. Data Resilience & Caching
The application implements a **"Cache-First Failover"** strategy:

1.  **Load Attempt:** The app requests fresh data via the CoinGecko API.
2.  **Polly Retries:** If the network is unstable, `Polly` automatically retries the request with an exponential backoff.
3.  **Persist (Upsert):** Successful data is saved to the local **SQLite** database. User-specific fields like `Amount` are preserved to avoid overwriting the portfolio.
4.  **Fallback:** If the API is unavailable (no internet or 429 error), the service transparently loads data from the local database, ensuring the app works in **Offline Mode**.

### 3. API Optimization (Debouncing)
The free CoinGecko API has strict rate limits.
To implement the **"Currency Switcher"** feature, a **Debounce** mechanism is used:
* When the currency is changed, a timer (1000 ms) starts.
* If the user quickly switches currencies, the previous request is cancelled via `CancellationTokenSource`.
* The network request is sent only when the user "settles" on a selection, preventing IP bans.

### 4. Portfolio Calculation (Weighted Average PnL)
The application calculates not just the sum, but the weighted average portfolio change over 24 hours.

$$
\text{PnL \%} = \frac{\sum (\text{Coin Value}_i \times \text{Change \%}_i)}{\text{Total Portfolio Value}}
$$

This gives the user an accurate understanding of how their balance has changed, taking into account the "weight" of each coin in the portfolio.

### 5. UI/UX Features
* **Custom Styles:** Implemented `ResourceDictionary` with a Dark Theme color palette. Styles for `ScrollBar`, `ComboBox`, and `ListViewItem` are overridden.
* **Animations:** Used `EventTrigger` and `DoubleAnimation` with `BackEase` easing function for a smooth entry effect of list items upon loading.

---

##  Installation and Setup

1.  **Clone:**
    ```bash
    git clone [https://github.com/your-username/CoinPulse.git](https://github.com/your-username/CoinPulse.git)

    2.  **Build:**    
    dotnet build
    
    3.  **Run:**
    dotnet run --project CoinPulse.UI
---

##  Project Status

- [x] Real-time monitoring (Top 50)
- [x] Local caching (SQLite)
- [x] Portfolio tracker and PnL
- [x] Multi-currency support
- [x] Charts (Sparklines & Pie Chart)
- [x] Search and Filtering
- [x] Unit Tests
