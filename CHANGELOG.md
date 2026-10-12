# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-10-12

### Added
- Full-stack restaurant point of sale system: ASP.NET Core 10 Web API + Blazor Server.
- **Order Management**: table and seat assignment, status workflow
  (Pending -> Confirmed -> Preparing -> Ready -> Served -> Completed),
  split bills, item voiding, order notes.
- **Menu Integration**: categories (Starters, Mains, Desserts, Drinks, Sides,
  Specials), admin management, daily specials, sold-out handling.
- **Payment Processing**: cash, card, mobile money, PayPal; tip entry,
  receipt generation, loyalty point accrual.
- **Inventory Tracking**: automatic ingredient deduction, low/out-of-stock
  alerts, restock logging with supplier management.
- **Roles & Authentication**: Admin / Cashier / Waiter roles,
  ASP.NET Identity with JWT, staff management.
- **Analytics & Reporting**: daily/weekly/monthly sales reports,
  top-selling items, CSV export.
- **Customer Experience**: loyalty profiles and redemption, discount/
  promotion codes, preference tracking.
- Clean Architecture solution (Core, Infrastructure, Application, Api, Web)
  and 64 automated tests (44 unit + 20 integration).

### Changed
- Migrated persistence from in-memory provider to SQLite + Entity Framework Core.

### Fixed
- Login render-loop and render-batch conflicts during auth transitions.
- Route conflict and query-filter warnings in the API layer.
- Seed data applied via EF Core `HasData` configuration.

[1.0.0]: https://github.com/ogc16/restaurant-pos/releases/tag/v1.0.0