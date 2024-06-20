using System;
using System.Collections.Generic;

namespace MVC_Test.Models;

public partial class Customer
{
    public string Cid { get; set; } = null!;

    public string Cname { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public int Age { get; set; }

    public string Province { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DetailedAddress { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
