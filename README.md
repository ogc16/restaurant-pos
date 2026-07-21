# RestaurantPOS

A full-stack Restaurant Point of Sale system built with .NET 10, ASP.NET Core Web API, and Blazor Server.

## Features

### Order Management
- Table and seat assignment
- Order status workflow: Pending → Confirmed → Preparing → Ready → Served → Completed
- Split bills across multiple payments
- Order modifications and item voiding

### Menu Integration
- Dynamic menu with categories (Starters, Mains, Desserts, Drinks, Sides, Specials)
- Admin panel for adding/removing items and updating prices
- Daily specials and sold-out item handling

### Payment Processing
- Cash, credit/debit card, mobile money, PayPal support
- Tip entry and receipt generation
- Loyalty point accrual on payments

### Inventory Tracking
- Automatic ingredient deduction per order
- Low-stock and out-of-stock alerts
- Restock logging with supplier management

### User Roles & Authentication
- Role-based access: Admin, Cashier, Waiter
- ASP.NET Identity with JWT authentication
- Staff management panel

### Analytics & Reporting
- Daily, weekly, monthly sales reports
- Top-selling items and revenue tracking
- CSV export

### Customer Experience
- Customer profiles with loyalty points
- Discount and promotion codes
- Repeat-visit preference tracking

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10 Web API |
| Frontend | Blazor Server (Interactive Server Rendering) |
| Database | SQL Server + Entity Framework Core |
| Auth | ASP.NET Identity + JWT Bearer |
| Validation | FluentValidation |
| API Docs | Swagger / Swashbuckle |

## Architecture

Clean Architecture with 4 layers:

```
RestaurantPOS.Core           → Domain entities, interfaces, DTOs, enums
RestaurantPOS.Infrastructure → EF Core DbContext, repositories, Identity
RestaurantPOS.Application    → Business logic, services, validators
RestaurantPOS.Api            → REST controllers, auth config, middleware
RestaurantPOS.Web            → Blazor Server frontend
```

## Project Structure

```
POS/
├── src/
│   ├── RestaurantPOS.Core/
│   │   ├── Entities/        # MenuItem, Order, Table, Payment, Customer, etc.
│   │   ├── Enums/           # OrderStatus, PaymentMethod, TableStatus, etc.
│   │   ├── Interfaces/      # Repository contracts
│   │   ├── DTOs/            # Request/Response models
│   │   └── Common/          # Result<T>, BaseEntity
│   ├── RestaurantPOS.Infrastructure/
│   │   ├── Data/            # AppDbContext, SeedData
│   │   ├── Repositories/    # EF Core implementations
│   │   └── Identity/        # JWT auth service
│   ├── RestaurantPOS.Application/
│   │   ├── Services/        # MenuItemService, OrderService, etc.
│   │   ├── Interfaces/      # Service contracts
│   │   └── Validation/      # FluentValidation validators
│   ├── RestaurantPOS.Api/
│   │   ├── Controllers/     # Auth, Orders, Menu, Payments, Tables, etc.
│   │   └── Program.cs       # Startup configuration
│   └── RestaurantPOS.Web/
│       ├── Components/
│       │   ├── Pages/       # Dashboard, POS, Orders, Tables, etc.
│       │   └── Layout/      # MainLayout, NavMenu
│       └── Services/        # ApiService, AuthService
├── publish/                  # Compiled executables
├── Start-RestaurantPOS.bat   # Launch script
└── Stop-RestaurantPOS.bat    # Stop script
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB, Express, or full instance)

### Database Setup

Update the connection string in `src/RestaurantPOS.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestaurantPOS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Migrations are applied automatically on first API startup. A default admin account is seeded:

| Field | Value |
|-------|-------|
| Email | `admin@restaurant.com` |
| Password | `Admin@123456` |

### Run from Source

```bash
# Terminal 1 - Start API
dotnet run --project src/RestaurantPOS.Api --urls https://localhost:7001

# Terminal 2 - Start Web UI
dotnet run --project src/RestaurantPOS.Web --urls https://localhost:5001
```

### Run Compiled Executables

```cmd
# Double-click or run from command line
Start-RestaurantPOS.bat
```

### Endpoints

| URL | Description |
|-----|------------|
| https://localhost:5001 | Web UI |
| https://localhost:7001 | API |
| https://localhost:7001/swagger | Swagger docs |

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | None | Login and get JWT token |
| POST | `/api/auth/register` | None | Register new user |
| GET | `/api/menuitems` | None | List all menu items |
| POST | `/api/menuitems` | Admin | Create menu item |
| GET | `/api/orders` | Any | List all orders |
| POST | `/api/orders` | Waiter/Admin | Create new order |
| PUT | `/api/orders/{id}/status` | Any | Update order status |
| POST | `/api/orders/split` | Any | Split bill |
| POST | `/api/payments` | Any | Process payment |
| GET | `/api/tables` | Any | List tables |
| GET | `/api/inventory` | Admin | List inventory |
| POST | `/api/inventory/restock` | Admin | Restock item |
| GET | `/api/reports/daily` | Admin/Cashier | Daily sales report |
| GET | `/api/reports/export` | Admin/Cashier | Export CSV |
| GET | `/api/customers` | Any | List customers |
| GET | `/api/promotions` | Any | List promotions |

## Frontend Pages

| Page | Route | Access |
|------|-------|--------|
| Dashboard | `/` | All |
| POS Terminal | `/pos` | All |
| Orders | `/orders` | All |
| Tables | `/tables` | All |
| Menu Management | `/menu` | Admin |
| Inventory | `/inventory` | Admin |
| Reports | `/reports` | Admin, Cashier |
| Customers | `/customers` | Admin |
| Promotions | `/promotions` | Admin |
| Staff | `/staff` | Admin |

## Seed Data

The database is seeded with:

- **8 tables** across Indoor, Outdoor, Bar, and VIP sections
- **16 menu items** across Starters, Mains, Desserts, and Drinks
- **10 inventory items** with suppliers
- **3 roles**: Admin, Cashier, Waiter
- **1 admin user**: admin@restaurant.com

## License

MIT
