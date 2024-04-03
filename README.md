# Connecting Databases To Blazor dataGrid Component

This section describes how to connect and retrieve data from a Microsoft SQL Server database using [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient/4.8.6?_src=template) and bind it to the Blazor DataGrid component.

Microsoft SQL Server database can be bound to the Blazor DataGrid in different ways (i.e.) using [DataSource](https://help.syncfusion.com/cr/blazor/Syncfusion.Blazor.Grids.SfGrid-1.html#Syncfusion_Blazor_Grids_SfGrid_1_DataSource) property, [CustomAdaptor](https://blazor.syncfusion.com/documentation/datagrid/custom-binding) feature and Remote data binding using various adaptors (Web API, OData, ODataV4, Url, GraphQL). In this documentation, two approaches will be examined to connect a Microsoft SQL Server database to a Blazor DataGrid component. Both the approaches have capability to handle data and CRUD operations with built-in methods as well as can be customized as per your own.

* **Using UrlAdaptor**

The [UrlAdaptor](https://blazor.syncfusion.com/documentation/data/adaptors#url-adaptor) serves as the base adaptor for facilitating communication between remote data services and a UI component. It enables the remote binding of data to the Blazor SfGrid component by connecting to an existing pre-configured API service linked to the Microsoft SQL Server database. While the Blazor DataGrid supports various adaptors to fulfill this requirement, including [Web API](https://blazor.syncfusion.com/documentation/data/adaptors#web-api-adaptor), [OData](https://blazor.syncfusion.com/documentation/data/adaptors#odata-adaptor), [ODataV4](https://blazor.syncfusion.com/documentation/data/adaptors#odatav4-adaptor), [Url](https://blazor.syncfusion.com/documentation/data/adaptors#url-adaptor), and [GraphQL](https://blazor.syncfusion.com/documentation/data/adaptors#graphql-service-binding), the `UrlAdaptor` is particularly useful for scenarios where a custom API service with unique logic for handling data and CRUD operations is in place. This approach allows for custom handling of data and CRUD operations, and the resultant data returned in the `result` and `count` format for display in the Blazor SfGrid component.

* **Using CustomAdaptor**

The [CustomAdaptor](https://blazor.syncfusion.com/documentation/datagrid/custom-binding) serves as a mediator between the UI component and the database for data binding. While the data source from the database can be directly bound to the `SfGrid` component locally using the [DataSource](https://help.syncfusion.com/cr/blazor/Syncfusion.Blazor.Grids.SfGrid-1.html#Syncfusion_Blazor_Grids_SfGrid_1_DataSource) property, the `CustomAdaptor` approach is preferred as it allows for customization of both data operations and CRUD operations according to specific requirements. In this approach, for every action in the Blazor SfGrid component, a corresponding request with action details is sent to the `CustomAdaptor`. The Blazor DataGrid provides predefined methods such as **PerformSearching**, **PerformFiltering**, **PerformSorting**,  **PerformAggregation**, **PerformSkip**, **PerformTake** and **Group** for executing these data operations. Alternatively, your own custom methods can be employed to execute operations and return the data in the `Result` and `Count` format of the `DataResult` class for display in the Blazor SfGrid component. Additionally, for CRUD operations, predefined methods can be overridden to provide custom functionality. Further details on this can be found in the latter part of the documentation.

Here, we have explained in detail how to bind the listed databases below to the Blazor DataGrid component using CustomAdaptor and UrlAdaptor.

1. Microsoft SQL Server Database
2. MYSQL Database
3. PostgreSQL Database
4. Dapper
5. SQLite
