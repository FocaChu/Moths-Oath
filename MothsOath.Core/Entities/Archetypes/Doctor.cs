using MothsOath.Core.Common.ArchetypeCores;

namespace MothsOath.Core.Entities.Archetypes;

public class Doctor : Player
{
    public DoctorLab Lab { get; private set; } = new DoctorLab();

    public Doctor()
    {
    }
}
