using Construction.Domain.Common;
using Construction.Domain.Equipment;
using Xunit;

namespace Construction.Domain.Tests.Equipment;

public sealed class EquipmentTests
{
    [Fact]
    public void AssignmentReturnAndMaintenance_PreserveValidLifecycle()
    {
        var equipment = Construction.Domain.Equipment.Equipment.Create(
            Guid.NewGuid(),
            "EQ-001",
            "Tower Crane",
            "Example",
            "TC-1",
            "SN-001");

        DateTimeOffset assignedAt =
            new(2026, 1, 1, 8, 0, 0, TimeSpan.Zero);

        equipment.Assign(
            Guid.NewGuid(),
            null,
            assignedAt,
            "Operator assignment");

        Assert.Equal(EquipmentStatus.InUse, equipment.Status);

        Assert.Throws<DomainException>(
            () => equipment.ScheduleMaintenance(
                "Monthly service",
                new DateOnly(2026, 1, 2),
                "Service Co"));

        equipment.Return(assignedAt.AddHours(8));

        EquipmentMaintenanceRecord maintenance =
            equipment.ScheduleMaintenance(
                "Monthly service",
                new DateOnly(2026, 1, 2),
                "Service Co");

        Assert.Equal(EquipmentStatus.Maintenance, equipment.Status);

        equipment.CompleteMaintenance(
            maintenance.Id,
            new DateOnly(2026, 1, 2),
            500m,
            "Completed");

        Assert.Equal(EquipmentStatus.Available, equipment.Status);
        Assert.True(maintenance.IsCompleted);
    }
}
