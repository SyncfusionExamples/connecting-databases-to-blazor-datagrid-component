# Blazor DataGrid with MariaDB and Entity Framework Core

## Project Overview

This repository demonstrates a production-ready pattern for binding **MariaDB Server** data to **Syncfusion Blazor DataGrid** using **Entity Framework Core (EF Core)**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, DbContext, repository pattern, and a custom adaptor for seamless grid functionality.

## Key Features

- **MariaDB–Entity Framework Core Integration**: Models, DbContext, and Entity Framework Core migrations for database operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Repository Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **Configurable Connection String**: Database credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| MariaDB Server | 10.5 or later | Database server |
| Microsoft.EntityFrameworkCore | 9.0.0 or later | Core framework for database operations |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 or later | Tools for managing database migrations |
| Pomelo.EntityFrameworkCore.MySql | 9.0.0 or later | MariaDB provider for Entity Framework Core |
| Syncfusion.Blazor.Grids | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding MariaDB using CustomAdaptor"
   cd "Blazor Web App/Grid_MariaDB"
   ```

2. **Create the database and table**
   
   Open MariaDB client or any MariaDB management tool and run:
   ```sql
   CREATE DATABASE IF NOT EXISTS subscriptiondb;
   USE subscriptiondb;

   CREATE TABLE IF NOT EXISTS subscriptions (
       Id INT AUTO_INCREMENT PRIMARY KEY,
       PublicID VARCHAR(50) NOT NULL,
       CustomerId INT NULL,
       SubscriptionID INT NULL,
       InvoiceNumber VARCHAR(100),
       Description VARCHAR(500),
       Amount DECIMAL(15, 2),
       CurrencyCode VARCHAR(10) DEFAULT 'INR',
       SubscriptionType VARCHAR(50),
       PaymentGateway VARCHAR(100),
       SubscriptionStartDate DATETIME NULL,
       SubscriptionEndDate DATETIME NULL,
       Status VARCHAR(50)
   );
   ```

3. **Update the connection string**
   
   Open `appsettings.json` and configure the MariaDB connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=subscriptiondb;Uid=root;Pwd=<password>;SslMode=None;AllowUserVariables=true;"
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

## Configuration

### Connection String

The connection string in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| Server | MariaDB server address | `localhost` |
| Port | MariaDB port number | `3306` |
| Database | Database name | `subscriptiondb` |
| Uid | MariaDB username | `root` |
| Pwd | MariaDB password | `<secure-password>` |
| SslMode | SSL encryption mode | `None` (for local development) |
| AllowUserVariables | Allow user variables | `true` (for MariaDB compatibility) |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/Subscription.cs` | Entity model representing the subscriptions table |
| `/Data/SubscriptionDbContext.cs` | Entity Framework Core DbContext for database operations |
| `/Data/SubscriptionRepository.cs` | Repository class providing CRUD methods |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add a Subscription
1. Click the **Add** button in the toolbar
2. Fill in the form fields (PublicID, CustomerId, Amount, SubscriptionType, etc.)
3. Click **Save** to persist the record to the database

### Edit a Subscription
1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes

### Delete a Subscription
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
- Verify MariaDB Server is running on the specified host and port
- Confirm the database name, username, and password are correct
- Ensure the `subscriptiondb` database exists

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

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/mariadb-server).
