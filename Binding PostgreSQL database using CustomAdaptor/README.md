# Blazor DataGrid with PostgreSQL and Entity Framework Core

## Project Overview

This repository demonstrates a production-ready pattern for binding PostgreSQL data to Syncfusion Blazor DataGrid using Entity Framework Core (EF Core). The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, DbContext, repository pattern, and a custom adaptor for seamless grid functionality.

## Key Features

- PostgreSQL–Entity Framework Core Integration: Models, DbContext, and EF Core configuration for database operations
- Syncfusion Blazor DataGrid: Built-in search, filter, sort, paging, and grouping capabilities
- Complete CRUD Operations: Add, edit, delete, and batch update purchase order records directly from the grid
- Repository Pattern: Clean separation of concerns with dependency injection support
- CustomAdaptor: Full control over grid data operations (read, search, filter, sort, page, group)
- Configurable Connection String: Database credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| PostgreSQL | 12 or later | Database server |
| Microsoft.EntityFrameworkCore | Latest | Core framework for database operations |
| Npgsql.EntityFrameworkCore.PostgreSQL | Latest | PostgreSQL provider for Entity Framework Core |
| Syncfusion.Blazor.Grid | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. Clone the repository
   ```powershell
   git clone <repository-url>
   cd "connecting-databases-to-blazor-datagrid-component/Binding PostgreSQL database using CustomAdaptor"
   cd Grid_PostgreSQL
   ```

2. Create the database and table (run in psql or any PostgreSQL client)
   ```sql
   -- Create Database
   CREATE DATABASE PurchaseOrderDB;

   -- Connect to the database (psql)
   \c PurchaseOrderDB;

   -- Create PurchaseOrder Table
   CREATE TABLE public.PurchaseOrder (
       PurchaseOrderId SERIAL PRIMARY KEY,
       PoNumber VARCHAR(30) NOT NULL UNIQUE,
       VendorID VARCHAR(50) NOT NULL,
       ItemName VARCHAR(200) NOT NULL,
       ItemCategory VARCHAR(100),
       Quantity INTEGER NOT NULL,
       UnitPrice NUMERIC(12,2) NOT NULL,
       TotalAmount NUMERIC(14,2),
       Status VARCHAR(30) NOT NULL DEFAULT 'Pending',
       OrderedBy VARCHAR(100) NOT NULL,
       ApprovedBy VARCHAR(100),
       OrderDate DATE NOT NULL,
       ExpectedDeliveryDate DATE,
       CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
       UpdatedAt TIMESTAMP NOT NULL DEFAULT NOW()
   );
   ```

3. Update the connection string

   Open `Grid_PostgreSQL/appsettings.json` and configure the PostgreSQL connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=5432;Database=PurchaseOrderDB;User Id=postgres;Password=your-password"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

4. Restore packages and build
   ```powershell
   dotnet restore; dotnet build
   ```

5. Run the application
   ```powershell
   dotnet run
   ```

6. Open the application

   Navigate to the local URL displayed in the terminal (typically `https://localhost:xxxx`).

## Configuration

### Connection String

The connection string in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| Server | PostgreSQL host or IP address | `localhost` |
| Port | PostgreSQL port | `5432` |
| Database | Database name | `PurchaseOrderDB` |
| User Id | Database user | `postgres` |
| Password | Database user password | `your-password` |

Security note: For production environments, store sensitive credentials using environment variables, user secrets, or a secure vault.

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/PurchaseOrder.cs` | Entity model representing the PurchaseOrder table |
| `/Data/PurchaseOrderDbContext.cs` | EF Core DbContext for PostgreSQL operations |
| `/Data/PurchaseOrderRepository.cs` | Repository class providing CRUD methods and PoNumber generation |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation and purchase order UI |
| `/Program.cs` | Service registration for DbContext, Repository, and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add a Purchase Order
1. Click the Add button in the toolbar.
2. Fill required fields (VendorID, ItemName, Status, OrderedBy, OrderDate, etc.).
3. Click Update to persist the record to the database.
4. The system generates a unique PoNumber (e.g., PO-2026-0001).

### Edit a Purchase Order
1. Select a row in the grid.
2. Click the Edit button in the toolbar or double-click the row.
3. Modify fields using editors (text, dropdown, date picker, numeric inputs).
4. Click Update to save changes.

### Delete a Purchase Order
1. Select a row in the grid.
2. Click the Delete button in the toolbar.
3. Confirm the deletion in the dialog.

### Search Records
1. Use the Search box in the toolbar.
2. Enter keywords to filter records across all columns.

### Filter Records
1. Click the filter icon in any column header.
2. Select filter criteria (equals, contains, greater than, etc.).
3. Click Filter to apply.

### Sort Records
1. Click the column header to sort ascending.
2. Click again to sort descending.

### Group Records
1. Drag a column header to the group area above the grid.
2. Click the group header to expand or collapse groups.

## Troubleshooting

### Connection Error
- Verify PostgreSQL is running and accessible.
- Confirm the Server, Port, Database, User Id, and Password values in `appsettings.json`.
- Ensure the `PurchaseOrderDB` database exists.

### Missing Tables
- Verify the SQL script executed successfully in the PostgreSQL client.
- Confirm the `public.PurchaseOrder` table exists in `PurchaseOrderDB`.
- Re-run the table creation script if needed.

### Static Files Not Loading
- Verify Syncfusion stylesheet is referenced in `Components/App.razor`:
  ```html
  <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  <link href="./grid-column-template.css" rel="stylesheet" />
  ```
- Verify Syncfusion scripts are referenced in `Components/App.razor`:
  ```html
  <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
  ```
- Check browser developer tools for 404 errors on static resources.

### Version Conflicts
- Align Entity Framework Core, Npgsql provider, and Syncfusion package versions (Latest).
- Run `dotnet restore` to update NuGet packages.
- Check `Grid_PostgreSQL.csproj` for conflicting version constraints.
- Ensure packages are compatible with .NET 10.0.

## Full Documentation

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/postgresql-server).