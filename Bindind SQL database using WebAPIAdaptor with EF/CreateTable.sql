CREATE TABLE [dbo].[Orders] (
    [OrderID]    INT            IDENTITY (1001, 1) NOT NULL,
    [CustomerID] NVARCHAR (100) NOT NULL,
    [OrderDate]  DATETIME       NULL,
    [Freight]    FLOAT (53)     NULL,
    PRIMARY KEY CLUSTERED ([OrderID] ASC)
);