using System;
using System.Collections.Generic;

namespace MVC_Test.Models;

public partial class Medicine
{
    public string Mid { get; set; } = null!;

    public string Mname { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Sid { get; set; } = null!;

    public string? Details { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Supplier SidNavigation { get; set; } = null!;
}
