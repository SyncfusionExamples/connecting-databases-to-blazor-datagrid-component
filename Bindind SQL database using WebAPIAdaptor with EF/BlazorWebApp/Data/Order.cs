using System;
using System.Collections.Generic;

namespace BlazorApp.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerId { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public double? Freight { get; set; }
}
