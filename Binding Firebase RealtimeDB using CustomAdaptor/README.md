# Blazor DataGrid with Firebase Realtime Database

## Project Overview

This repository demonstrates a production-ready pattern for binding **Firebase Realtime Database** data to **Syncfusion Blazor DataGrid**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and real-time data synchronization. The implementation follows industry best practices using models, Firebase REST API integration, and a custom adaptor for seamless grid functionality.

## Key Features

- **Firebase Realtime Database Integration**: Uses Firebase REST API for real-time data operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Firebase Service Layer**: Clean separation of concerns with dependency injection support
- **Real-time Data Binding**: Automatic book ID generation and timestamp tracking
- **Configurable Firebase Connection**: Database URL and secret managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net9.0 or net10.0 | Runtime and build tools |
| Firebase Project | Active account | Cloud Realtime Database service |
| Google Account | Required | For Firebase Console access |
| System.Net.Http | Included | For Firebase REST API communication |
| Syncfusion.Blazor.Grids | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

### 1. Create a Firebase Project

1. Navigate to [Firebase Console](https://console.firebase.google.com)
2. Click **Create a new project** or select an existing one
3. Enter a project name (e.g., "BookLibraryApp")
4. Accept Firebase terms and click **Continue**
5. Optionally enable Google Analytics, then click **Create project**
6. Wait for the project to initialize

### 2. Set Up Firebase Realtime Database

1. In the Firebase Console, navigate to **Realtime Database** (under Build section)
2. Click **Create Database**
3. Select your location (e.g., United States)
4. Choose **Start in test mode** (for development; configure security rules for production)
5. Click **Enable**
6. Your database URL will be displayed (e.g., `https://your-project.firebaseio.com`)

### 3. Get Your Database Credentials

**Database URL:**
- Copy the URL from your Realtime Database settings
- It appears in the format: `https://project-id.firebaseio.com`

**Database Secret (for development):**
1. In Firebase Console, go to **Project Settings** (gear icon)
2. Navigate to **Service Accounts** tab
3. Click **Database Secrets**
4. Copy the secret key (keep this secure and never commit to version control)

### 4. Clone the Repository

```bash
git clone <repository-url>
cd "Binding Firebase RealtimeDB using CustomAdaptor"
cd "Blazor Web app/Grid_Firebase_Realtime"
```

### 5. Configure Firebase Connection

Open `appsettings.json` and add your Firebase configuration:

```json
{
  "Firebase": {
    "RealtimeDatabaseURL": "https://your-project.firebaseio.com",
    "DatabaseSecret": "your-database-secret-key-here"
  },
  "AllowedHosts": "*"
}
```

**Security Note**: For production environments:
- Use User Secrets instead of `appsettings.json`
- Store the database secret in environment variables
- Use Azure Key Vault or AWS Secrets Manager
- Never commit `appsettings.Development.json` to version control

### 6. Initialize Firebase with Sample Data (Optional)

You can manually add sample books to your Firebase database:

1. In Firebase Console, go to **Realtime Database**
2. Click **+** next to "Books" or create the path manually
3. Add sample data in JSON format:

```json
{
  "Books": {
    "BOOK001": {
      "bookId": "BOOK001",
      "title": "The Great Gatsby",
      "author": "F. Scott Fitzgerald",
      "isbn": "978-0743273565",
      "category": "Fiction",
      "publishDate": "1925-04-10T00:00:00Z",
      "language": "English",
      "totalCopies": 5,
      "availableCopies": 3,
      "location": "Shelf A1",
      "borrowedBy": null,
      "status": "Available",
      "lastUpdated": "2026-02-19T10:30:00Z"
    },
    "BOOK002": {
      "bookId": "BOOK002",
      "title": "To Kill a Mockingbird",
      "author": "Harper Lee",
      "isbn": "978-0061120084",
      "category": "Fiction",
      "publishDate": "1960-07-11T00:00:00Z",
      "language": "English",
      "totalCopies": 4,
      "availableCopies": 2,
      "location": "Shelf B2",
      "borrowedBy": "John Doe",
      "status": "Borrowed",
      "lastUpdated": "2026-02-19T09:15:00Z"
    }
  }
}
```

### 7. Restore Packages and Build

```bash
dotnet restore
dotnet build
```

### 8. Run the Application

```bash
dotnet run
```

### 9. Open the Application

Navigate to the local URL displayed in the terminal (typically `https://localhost:7xxx`).

## Configuration

### Firebase Connection Settings

The configuration in `appsettings.json` contains the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| RealtimeDatabaseURL | Firebase Realtime Database URL | `https://project-id.firebaseio.com` |
| DatabaseSecret | Firebase Database Secret for authentication | `your-secret-key-abc123xyz` |

**Retrieving Configuration Values:**

1. **RealtimeDatabaseURL**: Found in Firebase Console → Realtime Database → URL bar at the top
2. **DatabaseSecret**: Found in Firebase Console → Project Settings → Service Accounts → Database Secrets

### appsettings.json Structure

```json
{
  "Firebase": {
    "RealtimeDatabaseURL": "https://your-project.firebaseio.com",
    "DatabaseSecret": "your-database-secret"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Security Rules for Production

Configure Firebase Security Rules in Firebase Console → Realtime Database → Rules:

```json
{
  "rules": {
    "Books": {
      ".read": "auth != null",
      ".write": "auth != null",
      "$bookId": {
        ".validate": "newData.hasChildren(['bookId', 'title', 'author', 'isbn', 'category', 'status'])"
      }
    }
  }
}
```

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Models/Book.cs` | Entity model representing a book record in Firebase |
| `/Services/FirebaseService.cs` | Service class providing CRUD methods for Firebase operations |
| `/Components/Pages/Home.razor` | DataGrid page with book management functionality |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including Firebase credentials |
| `/appsettings.Development.json` | Development-specific configuration (local testing) |

## Common Tasks

### Add a Book

1. Click the **Add** button in the toolbar
2. Fill in the form fields:
   - **Title**: Book title (e.g., "The Hobbit")
   - **Author**: Author name (e.g., "J.R.R. Tolkien")
   - **ISBN**: International Standard Book Number
   - **Category**: Genre or category (e.g., "Fantasy", "Science Fiction")
   - **Publish Date**: Publication date
   - **Language**: Language of the book
   - **Total Copies**: Total inventory count
   - **Available Copies**: Current available quantity
   - **Location**: Shelf or storage location
   - **Status**: Book status (Available, Borrowed, Reserved, etc.)
3. Click **Save** to persist the record to Firebase Realtime Database
4. The system will auto-generate a unique BookId (BOOK001, BOOK002, etc.)

### Edit a Book

1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes to Firebase
5. The `LastUpdated` timestamp is automatically set to the current UTC time

### Delete a Book

1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the dialog
4. The record is removed from Firebase Realtime Database

### Search Records

1. Use the **Search** box in the toolbar
2. Enter keywords to filter records (searches across all columns)
3. Results are filtered in real-time as you type

### Filter Records

1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply
4. Grid updates with filtered results

### Sort Records

1. Click the column header to sort ascending
2. Click again to sort descending
3. Click a third time to remove sorting

### Group Records

1. Drag a column header to the group drop area above the grid
2. Click the group header to expand or collapse groups
3. Useful for grouping books by Category, Status, or Language

## Troubleshooting

### Firebase Connection Error

**Symptoms**: "Firebase Realtime Database URL not found in configuration"

**Solution**:
- Verify `appsettings.json` contains the correct `Firebase:RealtimeDatabaseURL`
- Ensure the URL is in the format: `https://project-id.firebaseio.com`
- Check for typos or extra whitespace in the configuration
- Restart the application after updating configuration

### Authentication Failed (401 Unauthorized)

**Symptoms**: HTTP 401 errors when reading/writing data

**Solution**:
- Verify the `Firebase:DatabaseSecret` is correct and hasn't expired
- Check Firebase Console → Project Settings → Service Accounts → Database Secrets
- Regenerate the secret if necessary
- Update `appsettings.json` with the new secret
- For test mode, ensure you're using the database secret, not API key

### Database Rules Block Access (403 Forbidden)

**Symptoms**: HTTP 403 errors even with correct credentials

**Solution**:
- In Firebase Console, go to Realtime Database → Rules
- Temporarily change rules to test mode:
  ```json
  {
    "rules": {
      ".read": true,
      ".write": true
    }
  }
  ```
- Test the application
- Once working, implement proper security rules for production
- Never leave test mode rules in production

### Books Node Not Found

**Symptoms**: Application starts but no books are displayed

**Solution**:
- Verify the "Books" node exists in Firebase Realtime Database
- In Firebase Console, check if any data exists
- Manually add sample data using the Firebase Console interface
- Or add a book through the application UI, which will create the node

### Null or Empty Response from Firebase

**Symptoms**: "The input is not a valid Base-64 string" or JSON deserialization errors

**Solution**:
- Check that the database secret is correct and valid
- Verify the Firebase database URL is correctly formatted
- Ensure the Books node structure matches the expected JSON format
- Check the browser console for detailed error messages
- Verify that data in Firebase is properly formatted JSON

### Static Files Not Loading

**Symptoms**: CSS/JS files return 404 errors

**Solution**:
- Verify Syncfusion stylesheet and script references are present in `App.razor`
- Check that NuGet packages are properly restored: `dotnet restore`
- Clear browser cache (Ctrl+F5 or Cmd+Shift+R)
- Verify `wwwroot` folder contains required Syncfusion resources
- Check browser developer tools for specific 404 paths

### Version Conflicts

**Symptoms**: Build errors or runtime exceptions related to dependencies

**Solution**:
- Ensure all packages have compatible versions
- Run `dotnet clean` followed by `dotnet restore`
- Update packages to latest stable versions: `dotnet package update`
- Check the `.csproj` file for conflicting version constraints
- Verify .NET SDK version matches the project target framework

### Performance Issues with Large Datasets

**Symptoms**: Application becomes slow with many books (1000+)

**Solution**:
- Implement pagination (DataGrid has built-in paging support)
- Use Firebase filtering to retrieve only necessary records
- Implement virtual scrolling in the DataGrid
- Consider implementing search-before-load pattern
- Monitor network requests in browser developer tools
- Check Firebase database read/write limits

## Security Best Practices

1. **Never Commit Secrets**: Add `appsettings.Development.json` to `.gitignore`
2. **Use User Secrets for Development**:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Firebase:DatabaseSecret" "your-secret-key"
   ```
3. **Use Environment Variables for Production**: Set `Firebase__DatabaseSecret` in production environment
4. **Implement Firebase Security Rules**: Restrict database access based on authentication
5. **Rotate Secrets Regularly**: Change database secrets periodically
6. **Use HTTPS Only**: Never use HTTP in production
7. **Limit Firebase API Keys**: Implement proper API key restrictions in Firebase Console

## Full Documentation

- [Syncfusion Blazor DataGrid Documentation](https://blazor.syncfusion.com/documentation/datagrid/overview/)
- [Firebase Realtime Database Documentation](https://firebase.google.com/docs/database)
- [Firebase REST API Reference](https://firebase.google.com/docs/database/rest/start)
- [Entity Relationships in Firebase](https://firebase.google.com/docs/database/usage/best-practices)