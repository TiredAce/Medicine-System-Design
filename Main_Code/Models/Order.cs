using System;
using System.Collections.Generic;

namespace MVC_Test.Models;

public partial class Order
{
    public int Oid { get; set; }

    public string Cid { get; set; } = null!;

    public string Mid { get; set; } = null!;

    public string Sid { get; set; } = null!;

    public string Mname { get; set; } = null!;

    public int Number { get; set; }

    public DateTime? Time { get; set; }

    public decimal Price { get; set; }

    public string Province { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DetailedAddress { get; set; } = null!;

    public string State { get; set; } = null!;

    public virtual Customer CidNavigation { get; set; } = null!;

    public virtual Medicine MidNavigation { get; set; } = null!;

    public virtual Supplier SidNavigation { get; set; } = null!;
}
