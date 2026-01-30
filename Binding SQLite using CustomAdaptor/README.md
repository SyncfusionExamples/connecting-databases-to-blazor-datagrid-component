# Blazor DataGrid with SQLite and Entity Framework Core

## Project Overview

This repository demonstrates a production-ready pattern for binding **SQLite Server** data to **Syncfusion Blazor DataGrid** using **Entity Framework Core (EF Core)**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, DbContext, repository pattern, and a custom adaptor for seamless grid functionality.

## Key features

- **SQLite–Entity Framework Core Integration**: Models, DbContext, and Entity Framework Core migrations for database operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Repository Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **Configurable Connection String**: Database credentials managed via `appsettings.json`

## Prerequisites

| Software/Package | Version | Purpose |
|-----------------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net9.0 or compatible | Runtime and build tools |
| SQLite | 3.0 or later | Embedded Database engine |
| Syncfusion.Blazor | {{site.blazorversion}} | DataGrid and UI components |
| Microsoft.EntityFrameworkCore | 9.0.0 or later | Core framework for database operations |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 or later | Tools for managing database migrations |
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.0 or later | SQLite provider for Entity Framework Core |


## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding SQLite using CustomAdaptor"
   cd "Blazor Web app/Grid_SQLite"
   ```

2. **Create the database and table**
   
   Open SQLiteStudio and create a database named `asset` and run the following query:

   ```sql
   CREATE TABLE IF NOT EXISTS asset (
      Id              INTEGER PRIMARY KEY AUTOINCREMENT,
      AssetID         TEXT NOT NULL UNIQUE,
      AssetName       TEXT NOT NULL,
      AssetType       TEXT NOT NULL,
      Model           TEXT,
      SerialNumber    TEXT NOT NULL,
      InvoiceID       TEXT,
      AssignedTo      TEXT,
      Department      TEXT,
      PurchaseDate    DATE,
      PurchaseCost    REAL,
      WarrantyExpiry  DATE,
      Condition       TEXT CHECK(Condition IN ('New', 'Good', 'Fair', 'Poor')) DEFAULT 'New',
      LastMaintenance DATE,
      Status          TEXT CHECK(Status IN ('Active', 'In Repair', 'Retired', 'Available')) DEFAULT 'Available'
   );
   ```

3. **Update the connection string**
   
   Open `appsettings.json` and configure the SQLite connection:
   ```json
   {
   "ConnectionStrings": {
      "DefaultConnection": "Data Source=C:\\Users\\AmrishDharmaraj\\OneDrive - Syncfusion\\Desktop\\db\\asset.db"
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

4. **Restore packages and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open the application**
   
   Navigate to the local URL displayed in the terminal (typically `https://localhost:7xxx`).


## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/Asset.cs` | Entity model representing the asset table |
| `/Data/AssetDbContext.cs` | Entity Framework Core DbContext for database operations |
| `/Data/AssetRepository.cs` | Repository class providing CRUD methods |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add a Asset
1. Click the **Add** button in the toolbar
2. Fill in the form fields (AssetName, AssetType, Model, etc.)
3. Click **Save** to persist the record to the database

### Edit a Asset
1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes

### Delete a Asset
1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the dialog

### Search Records
1. Use the **Search** box in the toolbar
2. Enter keywords to filter records (searches across all columns)

### Filter Records
1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply

### Sort Records
1. Click the column header to sort ascending
2. Click again to sort descending

### Group Records
1. Drag a column header to the group drop area above the grid
2. Click the group header to expand or collapse groups

## Troubleshooting

### Connection Error
- Confirm the path is correct for your environment.
- Ensure the `asset` database exists

### Missing Tables
- Verify the SQL script was executed successfully
- Check that migrations were applied (if using EF migrations)
- Run the database creation script again

### Static Files Not Loading
- Verify Syncfusion stylesheet and script references are present in `App.razor`
- Check browser developer tools for 404 errors on static resources

### Version Conflicts
- Align Entity Framework Core, Pomelo, and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `.csproj` file for conflicting version constraints

## Full Documentation

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/sqlite-server).