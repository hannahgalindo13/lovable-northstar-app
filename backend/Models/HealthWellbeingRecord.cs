using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class HealthWellbeingRecord
{
    public int HealthRecordId { get; set; }

    public int ResidentId { get; set; }

    public DateOnly RecordDate { get; set; }

    public decimal? GeneralHealthScore { get; set; }

    public decimal? NutritionScore { get; set; }

    public decimal? SleepQualityScore { get; set; }

    public decimal? EnergyLevelScore { get; set; }

    public int? HeightCm { get; set; }

    public decimal? WeightKg { get; set; }

    public decimal? Bmi { get; set; }

    public int MedicalCheckupDone { get; set; }

    public int DentalCheckupDone { get; set; }

    public int PsychologicalCheckupDone { get; set; }

    public string? Notes { get; set; }

    public virtual Resident Resident { get; set; } = null!;
}
