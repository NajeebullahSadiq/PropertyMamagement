using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class VehiclesWitnessDetail
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public double? IndentityCardNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PropertyDetailsId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? NationalIdCard { get; set; }

    public virtual VehiclesPropertyDetail? PropertyDetails { get; set; }
}
