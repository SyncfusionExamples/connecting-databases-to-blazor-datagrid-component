# Blazor DataGrid with DynamoDB and AWS SDK

## Project Overview

This repository demonstrates a production-ready pattern for binding **AWS DynamoDB** data to **Syncfusion Blazor DataGrid** using **AWS SDK for .NET**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and search capabilities. The implementation follows industry best practices using models, DynamoDBContext, service layer, and a custom adaptor for seamless grid functionality.

## Key Features

- **DynamoDB–AWS SDK Integration**: Models, DynamoDBContext, and AWS SDK for .NET for database operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and update inventory records directly from the grid
- **Service Layer Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **Local Development Ready**: DynamoDB Local emulator for offline development without AWS account
- **Auto-Generated IDs**: Automatic InventoryID generation in format INV-001, INV-002, etc.

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| Java | 11 or later | Required for DynamoDB Local emulator |
| AWS NoSQL Workbench | Latest | Visual IDE for managing DynamoDB tables |
| DynamoDB Local | Latest | Local emulator for offline development |
| AWSSDK.DynamoDBv2 | 3.7.100 or later | AWS SDK for .NET DynamoDB operations |
| AWSSDK.Extensions.NETCore.Setup | 3.7.2 or later | AWS service registration for dependency injection |
| Syncfusion.Blazor.Grid | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

**Download Links:**
- AWS NoSQL Workbench: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.settingup.html
- DynamoDB Local: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html
- Java: https://www.java.com/en/download/

## Quick Start

### 1. Set Up DynamoDB Local Environment

```bash
# Extract DynamoDB Local to a folder (e.g., C:\DynamoDB Local\)
# Open PowerShell in the extracted folder and run:
java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb

# Keep this terminal open. DynamoDB Local runs on http://localhost:8000
```

### 2. Create the Inventory Table

1. Open **AWS NoSQL Workbench**
2. Create a new connection to `http://localhost:8000`
3. Create table with:
   - **Table Name:** `Inventory`
   - **Partition Key:** `InventoryID` (String)
   - Click **Create**

### 3. Load Sample Data

Using NoSQL Workbench, add sample items (INV-001, INV-002, etc.) to the Inventory table. See `dynamodb-local.md` for sample data structure.

### 4. Restore Packages and Build

```powershell
dotnet restore
dotnet build
```

### 5. Run the Application

```powershell
dotnet run
```

6. Open `https://localhost:xxxx` (port shown in terminal)

## Configuration

### Update appsettings.Development.json

Configure local development to connect to DynamoDB Local:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "AWS": {
    "ServiceURL": "http://localhost:8000",
    "AccessKeyId": "local",
    "SecretAccessKey": "local",
    "Region": "us-east-1",
    "TableName": "Inventory"
  },
  "DetailedErrors": "true"
}
```

**Configuration Details:**

| Key | Value | Purpose |
|-----|-------|---------|
| `AWS:ServiceURL` | `http://localhost:8000` | DynamoDB Local endpoint |
| `AWS:AccessKeyId` | `local` | Dummy credentials (local only) |
| `AWS:SecretAccessKey` | `local` | Dummy credentials (local only) |
| `AWS:Region` | `us-east-1` | AWS region setting |
| `AWS:TableName` | `Inventory` | DynamoDB table name |

### Update appsettings.json

For production/cloud deployment, use standard AWS settings:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "TableName": "Inventory"
  }
}
```

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Models/Inventory.cs` | Entity model representing DynamoDB Inventory items with attributes |
| `/Services/DynamoDBService.cs` | Service class handling CRUD operations and database interactions |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor and inventory management UI |
| `/Program.cs` | Service registration for AWS SDK, DynamoDB, and Syncfusion |
| `/appsettings.Development.json` | Local development configuration (DynamoDB Local) |
| `/appsettings.json` | Production configuration |

## Common Tasks

### Add an Inventory Item

1. Click the **Add** button in the toolbar
2. Fill in the form fields (ProductName, Category, CurrentStock, etc.)
3. Click **Update** to save
4. The system auto-generates unique InventoryID (INV-001, INV-002, etc.)

### Edit an Item

1. Select a row in the grid
2. Click the **Edit** button or double-click the row
3. Modify fields using appropriate editors
4. Click **Update** to save changes

### Delete an Item

1. Select a row in the grid
2. Click the **Delete** button
3. Confirm deletion in the dialog

### Search Records

1. Use the **Search** box in the toolbar
2. Enter keywords to filter across all columns

### Filter Records

1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply

### Sort Records

1. Click a column header to sort ascending
2. Click again to sort descending

### Group Records

1. Drag a column header to the group drop area above the grid
2. Click group headers to expand or collapse

## Troubleshooting

### DynamoDB Local Connection Error
- Verify DynamoDB Local is running on `http://localhost:8000`
- Check that Java 11+ is installed: `java -version`
- Ensure firewall allows local connections

### Table Not Found
- Verify `Inventory` table exists in NoSQL Workbench
- Confirm `AWS:TableName` in `appsettings.Development.json` matches table name
- Recreate table if needed

### Data Not Persisting
- By default, DynamoDB Local data does NOT persist between restarts
- To enable persistence, use `-dbPath` flag when starting:
  ```
  java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb -dbPath ./data
  ```

### Syncfusion Components Not Rendering
- Verify Syncfusion stylesheets are referenced in `Components/App.razor`:
  ```html
  <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
  ```
- Verify namespaces in `Components/_Imports.razor` include Syncfusion using statements

### CRUD Operations Not Working
- Ensure `GridEditSettings` is configured in Home.razor:
  ```html
  <GridEditSettings AllowEditing="true" AllowAdding="true" AllowDeleting="true"></GridEditSettings>
  ```
- Verify toolbar includes CRUD items: `"Add", "Edit", "Delete", "Update", "Cancel"`
- Check browser console for JavaScript errors

## Full Documentation

For detailed setup instructions including:
- DynamoDB Local installation and configuration
- AWS NoSQL Workbench usage
- Sample data loading
- Cloud DynamoDB setup


For Syncfusion DataGrid documentation:
https://blazor.syncfusion.com/documentation/datagrid/getting-started-with-web-app
