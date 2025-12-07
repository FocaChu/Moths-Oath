namespace MothsOath.Core.Common.EffectInterfaces.Damage;

public interface IGlobalDamageInteractor : IDamageDealtReactor, IDamageReceivedReactor, IIncomingDamageModifier, IOutgoingDamageModifier
{
}
