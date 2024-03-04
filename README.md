# connecting-databases-to-blazor-datagrid-component

This section describes how to connect and retrieve data from a Microsoft SQL Server database using [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient/4.8.6?_src=template) and bind it to the Blazor DataGrid component.

Microsoft SQL Server database can be bound to the Blazor DataGrid in different ways (i.e.) using [DataSource](https://help.syncfusion.com/cr/blazor/Syncfusion.Blazor.Grids.SfGrid-1.html#Syncfusion_Blazor_Grids_SfGrid_1_DataSource) property, [CustomAdaptor](https://blazor.syncfusion.com/documentation/datagrid/custom-binding) feature and Remote data binding using various adaptors (Web API, OData, ODataV4, Url, GraphQL). In this documentation, two approaches will be examined to connect a Microsoft SQL Server database to a Blazor DataGrid component. Both the approaches have capability to handle data and CRUD operations with built-in methods as well as can be customized as per your own.

* **Using UrlAdaptor**

In this approach, data can be bound to the Blazor DataGrid component by calling an existing pre-configured API service connected to the Microsoft SQL Server database. The Blazor DataGrid supports various adaptors to meet this requirement, such as [Web API](https://blazor.syncfusion.com/documentation/data/adaptors#web-api-adaptor), [OData](https://blazor.syncfusion.com/documentation/data/adaptors#odata-adaptor), [ODataV4](https://blazor.syncfusion.com/documentation/data/adaptors#odatav4-adaptor), [Url](https://blazor.syncfusion.com/documentation/data/adaptors#url-adaptor) and [GraphQL](https://blazor.syncfusion.com/documentation/data/adaptors#graphql-service-binding). Among these, the [UrlAdaptor](https://blazor.syncfusion.com/documentation/data/adaptors#url-adaptor) serves as the base adaptor for interacting with remote data services. Hence, this approach is selected to meet the requirement. It is important to note that the UrlAdaptor forwards all requests to an API service as **POST** requests.

* **Using CustomAdaptor**

The [CustomAdaptor](https://blazor.syncfusion.com/documentation/datagrid/custom-binding) allows you to perform manual operation on the data for every action performed in Blazor DataGrid component.