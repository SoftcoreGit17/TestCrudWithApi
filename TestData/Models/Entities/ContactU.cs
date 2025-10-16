using System;
using System.Collections.Generic;

namespace TestData.Models.Entities;

public partial class ContactU
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Message { get; set; }
}
