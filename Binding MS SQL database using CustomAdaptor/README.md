# Blazor DataGrid with SQL Server and Entity Framework Core

## Project Overview

This repository demonstrates a production-ready pattern for binding **SQL Server** data to **Syncfusion Blazor DataGrid** using **Entity Framework Core (EF Core)**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, DbContext, repository pattern, and a custom adaptor for seamless grid functionality.

## Key Features

- **SQL Server–Entity Framework Core Integration**: Models, DbContext and Entity Framework Core migrations for database operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update ticket records directly from the grid
- **Repository Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **Configurable Connection String**: Database credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net8.0 or compatible | Runtime and build tools |
| SQL Server | 2019 or later | Database server |
| Microsoft.EntityFrameworkCore | 9.0.0 or later | Core framework for database operations |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 or later | Tools for managing database migrations |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 or later | SQL Server provider for Entity Framework Core |
| Syncfusion.Blazor.Grids | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding MS SQL database using CustomAdaptor"
   cd "Blazor Web app/Grid_MSSQL"
   ```

2. **Create the database and table**
   
   Open SQL Server Management Studio (SSMS) or any SQL Server client and run:
   ```sql
   -- Create Database
   IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NetworkSupportDB')
   BEGIN
       CREATE DATABASE NetworkSupportDB;
   END
   GO

   USE NetworkSupportDB;
   GO

   -- Create Tickets Table
   IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tickets')
   BEGIN
       CREATE TABLE dbo.Tickets (
           TicketId INT PRIMARY KEY IDENTITY(1,1),
           PublicTicketId VARCHAR(50) NOT NULL UNIQUE,
           Title VARCHAR(200) NULL,
           Description TEXT NULL,
           Category VARCHAR(100) NULL,
           Department VARCHAR(100) NULL,
           Assignee VARCHAR(100) NULL,
           CreatedBy VARCHAR(100) NULL,
           Status VARCHAR(50) NOT NULL DEFAULT 'Open',
           Priority VARCHAR(50) NOT NULL DEFAULT 'Medium',
           ResponseDue DATETIME2 NULL,
           DueDate DATETIME2 NULL,
           CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
           UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
       );
   END
   GO
   ```

3. **Update the connection string**
   
   Open `appsettings.json` and configure the SQL Server connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=NetworkSupportDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
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
   ```powershell
   dotnet restore; dotnet build
   ```

5. **Run the application**
   ```powershell
   dotnet run
   ```

6. **Open the application**
   
   Navigate to the local URL displayed in the terminal (typically `https://localhost:xxxx`).

## Configuration

### Connection String

The connection string in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| Data Source | SQL Server instance name or IP address | `localhost` |
| Initial Catalog | Database name | `NetworkSupportDB` |
| Integrated Security | Windows Authentication (True) or SQL Authentication (False) | `True` |
| Connect Timeout | Connection timeout in seconds | `30` |
| Encrypt | Enable encryption for the connection | `False` (for local development) |
| Trust Server Certificate | Trust the server certificate | `False` |
| Application Intent | Connection intent (ReadWrite or ReadOnly) | `ReadWrite` |
| Multi Subnet Failover | Used in failover clustering scenarios | `False` |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/Tickets.cs` | Entity model representing the Tickets table |
| `/Data/TicketsDbContext.cs` | Entity Framework Core DbContext for database operations |
| `/Data/TicketRepository.cs` | Repository class providing CRUD methods and public ticket ID generation |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation and ticket management UI |
| `/Program.cs` | Service registration for DbContext, Repository, and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add a Ticket
1. Click the **Add** button in the toolbar
2. Fill in the form fields (Title, Description, Status, Priority, Category, Department, Assignee, etc.)
3. Click **Update** to persist the record to the database
4. The system automatically generates a unique PublicTicketId (e.g., NET-1001)

### Edit a Ticket
1. Select a row in the grid
2. Click the **Edit** button in the toolbar or double-click the row
3. Modify the required fields using the appropriate editors (text, dropdown, date picker)
4. Click **Update** to save changes

### Delete a Ticket
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
- Verify SQL Server is running on the specified host
- Confirm the Data Source, database name, and authentication method are correct
- Ensure the `NetworkSupportDB` database exists

### Missing Tables
- Verify the SQL script was executed successfully in SSMS
- Confirm you're connected to the correct database (`NetworkSupportDB`)
- Check that the `dbo.Tickets` table exists in the database
- Run the database creation script again if needed

### Static Files Not Loading
- Verify Syncfusion stylesheet is referenced in `Components/App.razor`:
  ```html
  <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  ```
- Verify Syncfusion scripts are referenced in `Components/App.razor`:
  ```html
  <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
  ```
- Check browser developer tools for 404 errors on static resources

### Version Conflicts
- Align Entity Framework Core, SQL Server provider, and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `Grid_MSSQL.csproj` file for conflicting version constraints
- Verify all packages are compatible with .NET 10.0

## Full Documentation

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/microsoft-sql-server).