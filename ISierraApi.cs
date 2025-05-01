using Nickel;
using Sierra.features;

namespace Sierra;

public interface ISierraApi
{
    IDeckEntry SierraDeck { get; }
    IStatusEntry IntimidationStatus { get; }
    IStatusEntry OilStatus { get; }
    ICardTraitEntry TurnEndTriggerTrait { get; }

}
