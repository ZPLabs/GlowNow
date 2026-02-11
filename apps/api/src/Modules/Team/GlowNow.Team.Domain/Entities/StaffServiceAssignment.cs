using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Team.Domain.Entities;

/// <summary>
/// Represents a service assignment to a staff member.
/// </summary>
public sealed class StaffServiceAssignment : Entity<Guid>
{
    private StaffServiceAssignment(
        Guid id,
        Guid staffProfileId,
        Guid serviceId,
        DateTime assignedAtUtc)
        : base(id)
    {
        StaffProfileId = staffProfileId;
        ServiceId = serviceId;
        AssignedAtUtc = assignedAtUtc;
    }

    private StaffServiceAssignment()
    {
    }

    /// <summary>
    /// Gets the staff profile ID.
    /// </summary>
    public Guid StaffProfileId { get; private set; }

    /// <summary>
    /// Gets the service ID.
    /// </summary>
    public Guid ServiceId { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the service was assigned.
    /// </summary>
    public DateTime AssignedAtUtc { get; private set; }

    /// <summary>
    /// Creates a new StaffServiceAssignment instance.
    /// </summary>
    /// <param name="staffProfileId">The staff profile ID.</param>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="assignedAtUtc">The assignment timestamp.</param>
    /// <returns>A new StaffServiceAssignment.</returns>
    internal static StaffServiceAssignment Create(
        Guid staffProfileId,
        Guid serviceId,
        DateTime assignedAtUtc)
    {
        return new StaffServiceAssignment(
            Guid.NewGuid(),
            staffProfileId,
            serviceId,
            assignedAtUtc);
    }
}
