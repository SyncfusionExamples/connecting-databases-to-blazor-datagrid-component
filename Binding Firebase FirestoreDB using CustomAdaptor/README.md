# Blazor DataGrid with Firebase Cloud Firestore

## Project Overview

This repository demonstrates a production-ready pattern for binding **Firebase Cloud Firestore** data to **Syncfusion Blazor DataGrid**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and real-time data synchronization. The implementation follows industry best practices using models, Firebase Admin SDK for .NET, and a custom adaptor for seamless grid functionality.

## Key Features

- **Firebase Cloud Firestore Integration**: Uses Google Cloud Firestore Admin SDK for secure data operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Firebase Service Layer**: Clean separation of concerns with dependency injection support
- **Real-time Data Binding**: Automatic book ID generation and timestamp tracking
- **Configurable Firebase Connection**: Project ID and service account managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net9.0 or net10.0 | Runtime and build tools |
| Firebase Project | Active account | Cloud Firestore service |
| Google Account | Required | For Firebase Console access |
| Google.Cloud.Firestore | Latest | Firestore Admin SDK for .NET |
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

### 2. Set Up Firebase Cloud Firestore

1. In the Firebase Console, navigate to **Firestore Database** (under Build section)
2. Click **Create Database**
3. Select your location (e.g., United States)
4. Choose **Start in test mode** (for development; configure security rules for production)
5. Click **Enable**
6. Your Firestore database will be initialized and ready for use

### 3. Get Your Project Credentials

**Project ID:**
- Found in Firebase Console → Project Settings (gear icon) → General tab
- Listed as "Project ID" (e.g., `my-project-12345`)

**Service Account Key:**
1. In Firebase Console, go to **Project Settings** (gear icon)
2. Navigate to **Service Accounts** tab
3. Click **Generate New Private Key** button at the bottom
4. A JSON file (`your-project-firebase-adminsdk-xxxxx.json`) will download automatically
5. Keep this file secure and never commit it to version control

### 4. Clone the Repository

```bash
git clone <repository-url>
cd "Binding Firebase FirestoreDB using CustomAdaptor"
cd "Blazor Web app/Grid_Firebase_Firestore"
```

### 5. Configure Firebase Connection

**Step 1:** Place the downloaded service account key in the project directory:
- Copy the JSON file (e.g., `your-project-firebase-adminsdk-xxxxx.json`)
- Rename it to `serviceAccountKey.json`
- Place it in the root directory of the `Grid_Firebase_Firestore` project

**Step 2:** Open `appsettings.json` and add your Firebase configuration:

```json
{
  "Firebase": {
    "ServiceAccountPath": "serviceAccountKey.json",
    "ProjectId": "your-project-id"
  },
  "AllowedHosts": "*"
}
```

Replace `your-project-id` with your actual Firebase project ID.

**Security Note**: For production environments:
- Store the service account key file securely (outside the repository)
- Use environment-based configuration for the key path
- Never commit `serviceAccountKey.json` to version control
- Implement proper file access permissions on the server

### 6. Initialize Firebase with Sample Data (Optional)

You can manually add sample books to your Firestore database:

1. In Firebase Console, go to **Firestore Database**
2. Click **+ Start collection** to create a new collection named "books"
3. For the first document, enter document ID: `BOOK001`
4. Add the following sample data:

```json
{
  "bookId": "BOOK001",
  "title": "The Great Gatsby",
  "author": "F. Scott Fitzgerald",
  "isbn": "978-0743273565",
  "category": "Fiction",
  "publishDate": "1925-04-10",
  "language": "English",
  "totalCopies": 5,
  "availableCopies": 3,
  "location": "Shelf A1",
  "borrowedBy": null,
  "status": "Available",
  "lastUpdated": "2026-02-19T10:30:00Z"
}
```

5. Click **Save**
6. Add additional documents (BOOK002, BOOK003, etc.) with different book data

Alternatively, let the application create the collection automatically by adding a book through the UI.

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
| ServiceAccountPath | Path to the service account JSON key file | `serviceAccountKey.json` |
| ProjectId | Firebase project ID from Firebase Console | `my-project-12345` |

**Retrieving Configuration Values:**

1. **ProjectId**: Found in Firebase Console → Project Settings → General tab
2. **ServiceAccountPath**: Path where you placed the downloaded service account JSON file

### appsettings.json Structure

```json
{
  "Firebase": {
    "ServiceAccountPath": "serviceAccountKey.json",
    "ProjectId": "your-project-id"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Models/Book.cs` | Entity model representing a book record in Firestore |
| `/Services/FirebaseService.cs` | Service class providing CRUD methods for Firestore operations |
| `/Components/Pages/Home.razor` | DataGrid page with book management functionality |
| `/Program.cs` | Service registration and Syncfusion configuration |
| `/appsettings.json` | Application configuration including Firebase credentials |
| `/appsettings.Development.json` | Development-specific configuration (local testing) |
| `serviceAccountKey.json` | Firebase service account credentials (not committed to version control) |

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
3. Click **Save** to persist the record to Firestore
4. The system will auto-generate a unique BookId (BOOK001, BOOK002, etc.)

### Edit a Book

1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes to Firestore
5. The `LastUpdated` timestamp is automatically set to the current UTC time

### Delete a Book

1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the dialog
4. The record is removed from Firestore

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

### Service Account Key Not Found

**Symptoms**: "Service account key not found at: serviceAccountKey.json"

**Solution**:
- Download the service account JSON file from Firebase Console
- Place it in the root directory of the project
- Ensure the filename matches the `ServiceAccountPath` value in `appsettings.json`
- Verify the file path is correct (use relative path from project root)
- Restart the application after adding the file

### Firebase Project ID Not Found

**Symptoms**: "Firebase project ID not found. Please configure 'Firebase:ProjectId' in appsettings.json"

**Solution**:
- Navigate to Firebase Console → Project Settings
- Copy the Project ID from the General tab
- Add it to `appsettings.json` under `Firebase:ProjectId`
- Verify there are no typos or extra whitespace
- Restart the application

### Authentication Failed (403 Permission Denied)

**Symptoms**: HTTP 403 errors when reading/writing data

**Solution**:
- Verify the service account key is valid and not expired
- Check that the key belongs to the correct Firebase project
- Ensure `Firebase:ProjectId` matches the project ID in the service account key
- Regenerate the service account key if necessary: Firebase Console → Project Settings → Service Accounts → Generate New Private Key
- Replace `serviceAccountKey.json` with the new key
- Restart the application

### Database Rules Block Access (403 Forbidden)

**Symptoms**: HTTP 403 errors even with correct credentials

**Solution**:
- In Firebase Console, go to Firestore Database → Rules
- Temporarily change rules to test mode:
  ```javascript
  rules_version = '2';
  service cloud.firestore {
    match /databases/{database}/documents {
      match /{document=**} {
        allow read, write: if true;
      }
    }
  }
  ```
- Test the application
- Once working, implement proper security rules for production
- Never leave test mode rules in production

### Books Collection Not Found

**Symptoms**: Application starts but no books are displayed or "Collection not found" errors

**Solution**:
- Verify the "books" collection exists in Firestore
- In Firebase Console, check if the collection is displayed in the data view
- Manually create the collection if it doesn't exist
- Or add a book through the application UI, which will create the collection
- Ensure the collection name is lowercase "books"

### Null or Empty Response from Firestore

**Symptoms**: JSON deserialization errors or "DateTime parsing" failures

**Solution**:
- Check that the service account has proper access permissions
- Verify the data structure in Firestore matches the Book model
- Ensure DateTime fields are stored in ISO 8601 format (e.g., "2026-02-19T10:30:00Z")
- Check the browser console for detailed error messages
- Verify that all required fields are present in Firestore documents
- Use the `NormalizeToUtc()` method for datetime conversions

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
- Use Firestore queries to retrieve only necessary records
- Implement virtual scrolling in the DataGrid
- Consider implementing search-before-load pattern
- Monitor network requests in browser developer tools
- Check Firestore read/write operations and quotas

## Full Documentation

- [Syncfusion Blazor DataGrid Documentation](https://blazor.syncfusion.com/documentation/datagrid/overview/)
- [Firebase Cloud Firestore Documentation](https://firebase.google.com/docs/firestore)
- [Google Cloud Firestore Admin SDK for .NET](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.Firestore/latest)
- [Firebase Service Account Authentication](https://firebase.google.com/docs/admin/setup)
- [Firestore Security Rules](https://firebase.google.com/docs/firestore/security/start)
