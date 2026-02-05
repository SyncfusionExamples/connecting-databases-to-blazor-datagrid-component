# Blazor DataGrid with SQL Server and Dapper

## Project Overview

This repository demonstrates a production-ready pattern for binding **SQL Server** data to **Syncfusion Blazor DataGrid** using **Dapper ORM**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, repository pattern, Dapper micro-ORM, and a custom adaptor for seamless grid functionality.

## Key Features

- **SQL Server–Dapper Integration**: Lightweight and high-performance data access using Dapper with SQL Server
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Repository Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **Configurable Connection String**: Database credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net8.0 or compatible | Runtime and build tools |
| SQL Server | 2019 or later | Database server |
| Dapper | 2.0.0 or later | Lightweight ORM for data access |
| Microsoft.Data.SqlClient | Latest | SQL Server data provider |
| Syncfusion.Blazor.Grids | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding Dapper using CustomAdaptor"
   cd "Blazor Web App/Grid_Dapper"
   ```

2. **Create the database and table**
   
   Open SQL Server Management Studio or SQL Server Express and run:
   ```sql
   CREATE DATABASE IF NOT EXISTS HotelBookingDB;
   USE HotelBookingDB;

   CREATE TABLE IF NOT EXISTS [dbo].[Rooms] (
       [Id] INT IDENTITY(1,1) PRIMARY KEY,
       [ReservationId] VARCHAR(50) NOT NULL,
       [GuestName] VARCHAR(100) NOT NULL,
       [GuestEmail] VARCHAR(100),
       [CheckInDate] DATETIME NOT NULL,
       [CheckOutDate] DATETIME NOT NULL,
       [RoomType] VARCHAR(50) NOT NULL,
       [RoomNumber] VARCHAR(10),
       [AmountPerDay] DECIMAL(18, 2),
       [NoOfDays] INT,
       [TotalAmount] DECIMAL(18, 2),
       [PaymentStatus] VARCHAR(50),
       [ReservationStatus] VARCHAR(50),
       [CreatedAt] DATETIME DEFAULT GETDATE()
   );
   ```

3. **Update the connection string**
   
   Open `appsettings.json` and configure the SQL Server connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=localhost;Initial Catalog=HotelBookingDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
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

## Configuration

### Connection String

The connection string in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| Data Source | SQL Server instance address | `localhost` or `.\SQLEXPRESS` |
| Initial Catalog | Database name | `HotelBookingDB` |
| Integrated Security | Windows Authentication | `True` (for local development) |
| Connect Timeout | Connection timeout in seconds | `30` |
| Encrypt | SSL encryption | `False` (for local development) |
| TrustServerCertificate | Certificate validation | `False` |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

For SQL Server Authentication (username/password):
```
Data Source=your-server;Initial Catalog=HotelBookingDB;User ID=sa;Password=<secure-password>;
```

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/Reservation.cs` | Entity model representing the Rooms table |
| `/Data/ReservationRepository.cs` | Repository class providing CRUD methods using Dapper |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including connection string |

## Common Tasks

### Add a Reservation
1. Click the **Add** button in the toolbar
2. Fill in the form fields (Guest Name, Email, Check-In Date, Room Type, etc.)
3. Click **Save** to persist the record to the database

### Edit a Reservation
1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes

### Delete a Reservation
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
- Verify SQL Server is running and accessible on the specified host
- Confirm the database name and authentication method are correct
- For Windows Authentication, ensure user account has access to SQL Server
- For SQL Server Authentication, verify the username and password
- Ensure the `HotelBookingDB` database exists

### Missing Tables
- Verify the SQL script was executed successfully in SQL Server Management Studio
- Run the database creation script again
- Confirm the table name is `[dbo].[Rooms]` with correct schema

### Static Files Not Loading
- Verify Syncfusion stylesheet and script references are present in `App.razor`
- Check browser developer tools for 404 errors on static resources

### Dapper Mapping Issues
- Ensure column names in the SQL query match the `Reservation` model property names
- Use column aliases if database column names differ from model properties:
  ```csharp
  SELECT Id as Id, ReservationId as ReservationId, ... FROM [dbo].[Rooms]
  ```

### Version Conflicts
- Align Dapper, System.Data.SqlClient, and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `.csproj` file for conflicting version constraints

## Full Documentation

Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/dapper).