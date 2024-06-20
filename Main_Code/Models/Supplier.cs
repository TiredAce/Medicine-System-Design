using System;
using System.Collections.Generic;

namespace MVC_Test.Models;

public partial class Supplier
{
    public string Sid { get; set; } = null!;

    public string Sname { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DetailedAddress { get; set; } = null!;

    public string ContactPerson { get; set; } = null!;

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
