using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Safehouse
{
    public int SafehouseId { get; set; }

    public string SafehouseCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Region { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string Country { get; set; } = null!;

    public DateOnly OpenDate { get; set; }

    public string Status { get; set; } = null!;

    public int CapacityGirls { get; set; }

    public int CapacityStaff { get; set; }

    public int CurrentOccupancy { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<DonationAllocation> DonationAllocations { get; set; } = new List<DonationAllocation>();

    public virtual ICollection<IncidentReport> IncidentReports { get; set; } = new List<IncidentReport>();

    public virtual ICollection<PartnerAssignment> PartnerAssignments { get; set; } = new List<PartnerAssignment>();

    public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();

    public virtual ICollection<SafehouseMonthlyMetric> SafehouseMonthlyMetrics { get; set; } = new List<SafehouseMonthlyMetric>();
}
