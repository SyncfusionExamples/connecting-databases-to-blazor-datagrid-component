# Blazor DataGrid with MongoDB and MongoDB.Driver

## Project Overview

This repository demonstrates a production-ready pattern for binding **MongoDB** data to **Syncfusion Blazor DataGrid** using **MongoDB.Driver**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, searching, and batch updates. The implementation follows industry best practices using models, service layer, custom adaptor, and NoSQL database operations for seamless grid functionality.

## Key Features

- **MongoDB Integration**: Direct MongoDB database connectivity using MongoDB.Driver for flexible document-based operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, grouping, and inline editing capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update project records directly from the grid
- **Service Layer Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group, batch update)
- **Configurable Connection String**: MongoDB connection details managed via `appsettings.json`


## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net9.0 or compatible | Runtime and build tools |
| MongoDB Server | 5.0 or later | NoSQL database server |
| MongoDB Compass | Latest | MongoDB GUI client (optional but recommended) |
| MongoDB.Driver | 2.23.0 or later | Official .NET driver for MongoDB |
| Syncfusion.Blazor.Grid | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |
| Syncfusion.Blazor.DropDowns | Latest | Dropdown component for editors |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding MongoDB using CustomAdaptor"
   cd "Blazor Web app/Grid_MongoDB"
   ```

2. **Ensure MongoDB Server is running**
   
   - Start MongoDB on your local machine (default: `localhost:27017`)
   - Or update the connection string to point to your MongoDB instance

3. **Create the database and collection (Optional)**
   
   MongoDB will auto-create the database and collection on first insert. To manually create using MongoDB Compass:
   
   - Open MongoDB Compass
   - Connect to `mongodb://localhost:27017`
   - Create a new database named `ProjectManagementDB`
   - Create a new collection named `Projects`

4. **Update the connection string**
   
   Open `appsettings.json` and configure the MongoDB connection:
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb://localhost:27017"
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

5. **Restore packages and build**
   ```powershell
   dotnet restore
   dotnet build
   ```

6. **Run the application**
   ```powershell
   dotnet run
   ```

7. **Open the application**
   
   Navigate to the local URL displayed in the terminal (typically `https://localhost:5001` or `https://localhost:7xxx`).

8. **Insert sample data (Optional)**
   
   Use the grid's **Add** button to insert project records, or import bulk data using MongoDB Compass import feature (see `INSERT_RECORDS_GUIDE.md` for detailed instructions).

## Configuration

### Connection String

The connection string in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| mongodb:// | MongoDB protocol prefix | `mongodb://` |
| Server | MongoDB server address | `localhost` |
| Port | MongoDB port number | `27017` (default) |

**Basic Connection (Local Development):**
```
mongodb://localhost:27017
```

**Advanced Connection String Options (if needed):**

For production environments with authentication:
```
mongodb://username:password@localhost:27017/ProjectManagementDB?authSource=admin
```

| Component | Description |
|-----------|-------------|
| username:password | MongoDB authentication credentials |
| /ProjectManagementDB | Specifies the default database |
| authSource=admin | Specifies the authentication database |
| replicaSet=rs0 | For replica set connections |
| ssl=true | Enable SSL encryption |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Models/Project.cs` | Data model representing project documents in MongoDB |
| `/Services/MongoDbService.cs` | Service class handling all MongoDB CRUD operations |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation |
| `/Components/_Imports.razor` | Global namespaces and imports |
| `/Components/App.razor` | Root Blazor component with Syncfusion theme configuration |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including MongoDB connection string |

## Common Tasks

### Add a Project
1. Click the **Add** button in the toolbar
2. Fill in the form fields (ProjectName, Client, Budget, etc.)
3. Note: ProjectId is auto-generated; StartDate defaults to current date
4. Click **Save** to persist the record to MongoDB

### Edit a Project
1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields using inline editors
4. Click **Update** to save changes to MongoDB

### Delete a Project
1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the confirmation dialog
4. The record is removed from MongoDB

### Search Projects
1. Use the **Search** box in the toolbar
2. Enter keywords to filter projects (searches across all fields)
3. Results update in real-time as you type

### Filter Records
1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply conditions
4. Multiple filters can be combined

### Sort Records
1. Click the column header to sort in ascending order
2. Click again to sort in descending order
3. Click a third time to clear sorting

### Batch Operations
1. Make multiple changes to different rows (add, edit, delete)
2. Click **Update** to save all changes in a single batch operation to MongoDB
3. All operations are processed atomically

## Troubleshooting

### Connection Error: "Unable to connect to MongoDB"
- Verify MongoDB Server is running on the specified host and port
- Check the connection string in `appsettings.json`
- Confirm the server address and port are correct (default: `localhost:27017`)
- Test connection using MongoDB Compass

### Database Not Found
- MongoDB creates databases and collections automatically on first insert
- To manually create: Use MongoDB Compass or MongoDB Shell
- Verify the database name in `MongoDbService.cs` (default: `ProjectManagementDB`)

### No Data Displays in Grid
- Verify the MongoDB connection string is correct
- Check that the `Projects` collection exists in `ProjectManagementDB`
- Insert sample data using the grid's **Add** button or import via MongoDB Compass
- Check browser console for JavaScript errors

### Grid Operations Not Working (Add/Edit/Delete)
- Verify MongoDB user has appropriate permissions (read, write, delete)
- Check that `MongoDbService` is properly registered in `Program.cs`
- Ensure the CustomAdaptor is correctly configured in `Home.razor`
- Review browser developer tools console for error messages

### Bulk Import Fails
- Refer to `INSERT_RECORDS_GUIDE.md` for detailed import instructions
- Ensure JSON file is properly formatted with valid array syntax
- Check for duplicate ProjectId values in imported data
- Verify MongoDB Compass connection before importing

### Static Files Not Loading
- Verify Syncfusion stylesheet and script references are present in `App.razor`
- Check browser developer tools (F12) Network tab for 404 errors
- Ensure NuGet package versions match configuration in `.csproj`

### Version Conflicts
- Align MongoDB.Driver and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `.csproj` file for version constraints
- Clear NuGet cache if needed: `dotnet nuget locals all --clear`

## Full Documentation
Detailed, step-by-step directions are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-database/mongodb-server).