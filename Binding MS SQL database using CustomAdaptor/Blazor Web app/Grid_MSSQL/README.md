# Network Support Ticket Table

This sample demonstrates integrating **Syncfusion Blazor DataGrid** with a **SQL Server backend** using **SfDataManager** and a **Custom Adaptor**.

The grid supports server-driven operations (searching, filtering, sorting, paging, grouping) and CRUD (add / edit / delete) executed by the adaptor against SQL Server.

## Architecture

**Blazor (Server)**

- **UI**: Syncfusion Blazor `SfGrid` defined in `Components/Pages/Home.razor`.
- **Data access**: `SfDataManager` with a `CustomAdaptor` (in the same `Home.razor`) that routes all grid actions to `Data/TicketData.cs`.
- **Theme**: Syncfusion `tailwind3.css` linked in `Components/App.razor`.

**Data Layer**

- `TicketData.cs` connects to SQL Server and performs CRUD operations.
- All operations are executed via SQL statements using `SqlConnection` and `SqlCommand`.
- User supplies their own connection string.

**High-level Flow**

1. Grid triggers data actions (initial load, search, filter, sort, paging, grouping).
2. `SfDataManager` forwards the `DataManagerRequest` to `CustomAdaptor` in `Home.razor`.
3. `CustomAdaptor`:
   - Loads data via `TicketData.GetTicketsData()`
   - Applies search/filter/sort/group in memory using Syncfusion `DataOperations` and `DataUtil`
   - Applies skip/take for paging if enabled
   - Calls `TicketData.Add/Update/Remove` for CRUD operations
4. Grid renders rows and updates UI state.

## Project Structure

**Key Files:**

- `Components/App.razor`
  - Includes Syncfusion styles: `_content/Syncfusion.Blazor/styles/tailwind3.css`
  
- `Components/Pages/Home.razor`
  - Defines the `SfGrid`, columns, toolbar, edit settings, virtualization, and `CustomAdaptor`
  - Implements `ReadAsync`, `InsertAsync`, `UpdateAsync`, `RemoveAsync`, and `BatchUpdateAsync`
  
- `Data/TicketData.cs`
  - Implements `GetTicketsData`, `AddTicketAsync`, `UpdateTicketAsync`, `RemoveTicketAsync`
  - Connection string placeholder: `"Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;Integrated Security=True;Encrypt=False"`
  
- `Data/Tickets.cs`
  - Ticket model with properties: `TicketId`, `PublicTicketId`, `Title`, `Description`, `Category`, `Department`, `Assignee`, `CreatedBy`, `Status`, `Priority`, `ResponseDue`, `DueDate`, `CreatedAt`, `UpdatedAt`
  
- `Program.cs`
  - Registers Syncfusion Blazor services and maps the `App` component
  
- `Grid_MSSQL.csproj`
  - References `Syncfusion.Blazor` and `Microsoft.Data.SqlClient`

## Prerequisites

- .NET SDK installed (net10.0 or compatible version)
- SQL Server instance with a `Tickets` table and data
- A valid Syncfusion setup (packages already referenced in `Grid_MSSQL.csproj`)

## Configuration

### 1) Update the SQL Connection

- Open `Data/TicketData.cs`
- Locate the `connectionString` variable
- Replace the placeholder with your actual SQL Server connection string

**Example:**
```csharp
string connectionString = "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;Integrated Security=True;Encrypt=False";
```

### 2) Theme

The sample uses Syncfusion `tailwind3.css` (see `Components/App.razor`). You can switch to any Syncfusion theme (e.g., `material.css`, `bootstrap.css`) by updating the link in `App.razor`.

## Grid Features

**Enabled in Components/Pages/Home.razor:**

- **Sorting**: `AllowSorting="true"` — Click column headers to sort
- **Filtering**: `AllowFiltering="true"` with Excel filter (`GridFilterSettings Type="FilterType.Excel"`)
- **Search**: Toolbar includes `"Search"` button for global search
- **CRUD Operations**:
  - Toolbar includes `"Add"`, `"Edit"`, `"Delete"`, `"Update"`, `"Cancel"`
  - Routed through `CustomAdaptor` methods:
    - `InsertAsync` → `TicketData.AddTicketAsync`
    - `UpdateAsync` → `TicketData.UpdateTicketAsync`
    - `RemoveAsync` → `TicketData.RemoveTicketAsync`
    - `BatchUpdateAsync` → For batch mode scenarios
- **Virtualization**: `EnableVirtualization="true"` — Efficient rendering of large datasets
- **Custom Templates**: Status, Priority, Category columns render with custom HTML and CSS styling
- **Date Formatting**: Date columns use `Format="MMM d, yyyy, h:mm tt"`

**Also Supported (Enable as Needed):**

- **Paging**: Add `AllowPaging="true"` and configure `PageSettings` (Skip/Take is already handled in `CustomAdaptor`)
- **Grouping**: Add `AllowGrouping="true"` (Grouping logic is already implemented in `CustomAdaptor`)

## How It Works

### CustomAdaptor Server Operations

**ReadAsync(DataManagerRequest):**
- Loads data from `TicketData.GetTicketsData()`
- Applies operations in sequence:
  1. Searching via `DataOperations.PerformSearching`
  2. Filtering via `DataOperations.PerformFiltering`
  3. Sorting via `DataOperations.PerformSorting`
  4. Grouping via `DataUtil.Group` (if Group is present in request)
  5. Paging via `DataOperations.PerformSkip` and `PerformTake` (if enabled)
- Returns `DataResult` with `Result`, `Count`, and `Aggregates` (when requested)

**InsertAsync / UpdateAsync / RemoveAsync / BatchUpdateAsync:**
- Call respective methods in `TicketData` to modify SQL Server data
- Changes are persisted immediately to the database

### What the `TicketData` CRUD methods do

- `AddTicketAsync(Tickets value)`
  - Generates a new public ticket id with `GeneratePublicTicketIdAsync()` by scanning existing IDs and incrementing the max number.
  - Assigns the generated value to `value.PublicTicketId`.
  - Builds an `INSERT` SQL statement with the ticket fields and executes it using `SqlConnection` + `SqlCommand`.
  - Persists the new ticket in the `Tickets` table.

- `UpdateTicketAsync(Tickets value)`
  - Builds an `UPDATE` SQL statement setting all mutable columns from the incoming `value` where `TicketId` matches.
  - Executes the command to persist edits to the `Tickets` table.

- `RemoveTicketAsync(int? key)`
  - Builds a `DELETE` SQL statement to remove the row where `TicketId = key`.
  - Executes the command to remove the record from the `Tickets` table.

### How the SQL query executes (`GetTicketsData`)

The `GetTicketsData` method loads rows from SQL Server and maps them to the `Tickets` model:

1. Define the query string:
   - `string queryString = "SELECT * FROM dbo.Tickets ORDER BY TicketId";`
     - Selects all columns from `dbo.Tickets`, ordered by `TicketId`.
2. Create and open a connection:
   - `SqlConnection sqlConnection = new(connectionString);`
   - `sqlConnection.Open();`
     - Uses the configured connection string to establish a database session.
3. Prepare the command and data adapter:
   - `SqlCommand sqlCommand = new(queryString, sqlConnection);`
   - `SqlDataAdapter sqlDataAdapter = new(sqlCommand);`
     - The adapter will execute the command and fill a `DataTable`.
4. Fill a `DataTable` with results:
   - `DataTable dataTable = new DataTable();`
   - `sqlDataAdapter.Fill(dataTable);`
     - Executes the select query and loads all rows into memory.
5. Close the connection:
   - `sqlConnection.Close();`
     - Releases the database connection promptly after data retrieval.
6. Project rows to the `Tickets` model list:
   - LINQ over `dataTable.Rows` maps each `DataRow` to a `Tickets` instance, converting types and handling `DBNull` as nullable `DateTime?`.
7. Return the list for consumption by the grid and adaptor.

## Run the Sample

### 1) Update Connection String

Open `Data/TicketData.cs` and set your SQL Server connection string:

```csharp
string connectionString = "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;Integrated Security=True;Encrypt=False";
```

### 2) Restore and Run

From the `Grid_MSSQL` folder, execute:

```powershell
dotnet restore; dotnet run
```

### 3) Open the Application

- The console will print the application URL (typically `https://localhost:7xxx`)
- Open that URL in your browser

## Verify It Works

- **Sorting**: Click column headers to sort by any column
- **Filtering**: Use the Excel filter button on column headers
- **Search**: Enter text in the toolbar search box to filter records
- **CRUD**: 
  - Click `Add` to create a new ticket
  - Click `Edit` to modify an existing ticket
  - Click `Delete` to remove a ticket
  - Changes persist to SQL Server immediately
- **Virtualization**: Scroll through the grid for efficient rendering
- **Optional Features**:
  - Enable `AllowPaging="true"` in `SfGrid` to activate paging
  - Enable `AllowGrouping="true"` in `SfGrid` to activate grouping by any column

## References

**Files:**

- `Components/App.razor` — Application layout and Syncfusion theme
- `Components/Pages/Home.razor` — Grid UI and CustomAdaptor implementation
- `Data/TicketData.cs` — SQL Server data access
- `Data/Tickets.cs` — Ticket model
- `Program.cs` — Service registration
- `Grid_MSSQL.csproj` — Project dependencies

## Notes

- Users are responsible for providing and managing their own SQL Server database, schema, and data.
- This sample demonstrates the recommended pattern for integrating Syncfusion Blazor DataGrid with a SQL Server backend via `SfDataManager` and `CustomAdaptor`.
- The `CustomAdaptor` executes all data operations on the server, ensuring efficient handling of large datasets.
- For production use, consider parameterizing SQL queries to prevent SQL injection vulnerabilities.
