using System;
using System.Collections.Generic;

namespace TestData.Models.Entities;

public partial class CustomerRe
{
    public int Id { get; set; }

    public string? CustomerName { get; set; }

    public long? CustomerMobileno { get; set; }

    public long? CustomerPincode { get; set; }

    public string? Password { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public bool? Isdelete { get; set; }
}
