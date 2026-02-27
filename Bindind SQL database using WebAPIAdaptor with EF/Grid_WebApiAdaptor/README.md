# Blazor DataGrid with SQL Server, Entity Framework Core, and URL Adaptor

## Project Overview

This repository demonstrates a production-ready pattern for binding **SQL Server** data to **Syncfusion Blazor DataGrid** using **Entity Framework Core (EF Core)** with **URL Adaptor**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, DbContext, ASP.NET Core API controllers, and a URL Adaptor for seamless grid-to-server communication.

## Key Features

- **SQL Server–Entity Framework Core Integration**: Models, DbContext, and Entity Framework Core for database operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **URL Adaptor Pattern**: Leverages RESTful API endpoints for all grid operations
- **Complete CRUD Operations**: Add, edit, delete, and batch update order records directly from the grid
- **ASP.NET Core API Controllers**: RESTful API endpoints for data management
- **Batch Operations Support**: Efficient handling of multiple add, update, and delete operations
- **Configurable Connection String**: Database credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| SQL Server | 2019 or later | Database server |
| Microsoft.EntityFrameworkCore | 10.0.2 or later | Core framework for database operations |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.2 or later | SQL Server provider for Entity Framework Core |
| Syncfusion.Blazor.Grid | Latest | DataGrid component |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding SQL database using EF and UrlAdaptor"
   cd "Grid_EF_UrlAdaptor"
   ```

2. **Create the database and table**
   
   Open SQL Server Management Studio (SSMS) or any SQL Server client and run:
   ```sql
   -- Create Database
   IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderDB')
   BEGIN
       CREATE DATABASE OrderDB;
   END
   GO

   USE OrderDB;
   GO

   -- Create Orders Table
   IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Order')
   BEGIN
       CREATE TABLE dbo.[Order] (
           OrderID INT PRIMARY KEY IDENTITY(1,1),
           CustomerID VARCHAR(100) NOT NULL,
           EmployeeID INT NOT NULL,
           Freight DECIMAL(10,2) NULL,
           ShipCity VARCHAR(100) NULL
       );
   END
   GO

   -- Insert sample data (optional)
   IF (SELECT COUNT(*) FROM dbo.[Order]) = 0
   BEGIN
       INSERT INTO dbo.[Order] (CustomerID, EmployeeID, Freight, ShipCity)
       VALUES 
           ('ALFKI', 1, 32.38, 'Berlin'),
           ('ANATR', 2, 11.61, 'Mexico D.F.'),
           ('ANTON', 3, 65.83, 'Mexico D.F.'),
           ('AROUT', 4, 42.34, 'Colchester'),
           ('BERGS', 5, 55.15, 'London');
   END
   GO
   ```

3. **Update the connection string**
   
   Open `appsettings.json` and configure the SQL Server connection:
   ```json
   {
     "ConnectionStrings": {
       "ConnectionString": "Data Source=YOUR_SERVER;Initial Catalog=OrderDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30"
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
| Initial Catalog | Database name | `OrderDB` |
| Integrated Security | Windows Authentication (True) or SQL Authentication (False) | `True` |
| Connect Timeout | Connection timeout in seconds | `30` |
| Encrypt | Enable encryption for the connection | `False` (for local development) |
| Trust Server Certificate | Trust the server certificate | `False` |
| Application Intent | Connection intent (ReadWrite or ReadOnly) | `ReadWrite` |
| Multi Subnet Failover | Used in failover clustering scenarios | `False` |
| Command Timeout | Command execution timeout in seconds | `30` |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

## Architecture Overview

### URL Adaptor Pattern

The URL Adaptor is a Syncfusion Blazor component that handles grid operations through HTTP requests to server endpoints. The following diagram illustrates the flow:

```
DataGrid (Client) → URL Adaptor → HTTP Requests → API Controller → DbContext → SQL Server
```

### Request/Response Flow

| Operation | HTTP Method | Endpoint | Purpose |
|-----------|-------------|----------|---------|
| Read/Filter/Sort/Page | POST | `/api/Grid` | Retrieves data with applied filters, sorting, and paging |
| Insert | POST | `/api/Grid/Insert` | Adds a new order record |
| Update | POST | `/api/Grid/Update` | Updates an existing order record |
| Delete | POST | `/api/Grid/Delete` | Deletes an order record |
| Batch Operations | POST | `/api/Grid/BatchUpdate` | Handles multiple add, update, and delete operations in a single request |

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/Order.cs` | Entity model representing the Order table |
| `/Data/OrderDbContext.cs` | Entity Framework Core DbContext for database operations |
| `/Controllers/GridController.cs` | ASP.NET Core API controller with CRUD endpoints |
| `/Components/Pages/Home.razor` | DataGrid page with URL Adaptor configuration |
| `/Program.cs` | Service registration for DbContext, Controllers, and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add an Order
1. Click the **Add** button in the toolbar
2. Fill in the form fields (Customer ID, Employee ID, Freight, Ship City)
3. Click **Update** to persist the record to the database
4. The grid automatically refreshes with the new order

### Edit an Order
1. Select a row in the grid
2. Click the **Edit** button in the toolbar or double-click the row
3. Modify the required fields using the appropriate editors
4. Click **Update** to save changes
5. The database is immediately updated

### Delete an Order
1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion
4. The record is removed from the database and grid

### Search Records
1. Use the **Search** box in the toolbar
2. Enter keywords to filter records across all columns
3. Results are displayed in real-time

### Filter Records
1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply
4. Results are updated on the server and returned to the grid

### Sort Records
1. Click the column header to sort ascending
2. Click again to sort descending
3. Multi-column sorting is supported

### Batch Operations
1. Make multiple changes (add, edit, delete) in the grid
2. Click **Update** to send all changes in a single batch request
3. All operations are processed together on the server

## Key Code Examples

### URL Adaptor Configuration (Home.razor)

```razor
<SfDataManager Url="http://localhost:5175/api/Grid"
               InsertUrl="http://localhost:5175/api/Grid/Insert"
               UpdateUrl="http://localhost:5175/api/Grid/Update"
               RemoveUrl="http://localhost:5175/api/Grid/Delete"
               BatchUrl="http://localhost:5175/api/Grid/BatchUpdate"
               Adaptor="Adaptors.UrlAdaptor">
</SfDataManager>
```

### API Controller Structure (GridController.cs)

The `GridController.cs` file implements the following key methods:

- **Post()**: Handles data retrieval with filtering, sorting, and paging
- **Insert()**: Adds new orders
- **Update()**: Modifies existing orders
- **Delete()**: Removes orders
- **Batch()**: Processes multiple operations atomically

## Troubleshooting

### Connection Error
- Verify SQL Server is running on the specified host
- Confirm the Data Source, database name, and authentication method are correct
- Ensure the `OrderDB` database exists
- Test connection string in SQL Server Management Studio

### API Endpoint Not Found
- Verify the API URLs in the URL Adaptor configuration match the controller routes
- Ensure `app.MapControllers();` is called in `Program.cs`
- Check that the controller is properly registered with dependency injection
- Verify the application is running on the correct port (default: 5175)

### Missing Tables
- Verify the SQL script was executed successfully in SSMS
- Confirm you're connected to the correct database (`OrderDB`)
- Check that the `dbo.Order` table exists in the database
- Run the database creation script again if needed

### Static Files Not Loading
- Verify Syncfusion stylesheet is referenced in `Components/App.razor`:
  ```html
  <link href="_content/Syncfusion.Blazor.Themes/fluent.css" rel="stylesheet" />
  ```
- Verify Syncfusion scripts are referenced in `Components/App.razor`:
  ```html
  <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
  ```
- Check browser developer tools for 404 errors on static resources

### Version Conflicts
- Align Entity Framework Core, SQL Server provider, and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `Grid_EF_UrlAdaptor.csproj` file for conflicting version constraints
- Verify all packages are compatible with .NET 10.0

### Data Not Displaying
- Check browser console for JavaScript errors
- Verify the API endpoints are returning data (use Postman or similar tools)
- Ensure the Order model properties match the database columns
- Check that DbContext is properly registered in `Program.cs`

## Full Documentation

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/entityframework).
