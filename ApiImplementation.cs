using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nickel;
using Sierra.ExternalAPIs.Kokoro;
using Sierra.features;

namespace Sierra;

public sealed class ApiImplementation : ISierraApi
{
    public IDeckEntry SierraDeck => ModEntry.Instance.SierraDeck;
    public IStatusEntry OilStatus => OilManager.OilStatus;
    public IStatusEntry IntimidationStatus => IntimidationManager.IntimidationStatus;
    public ICardTraitEntry TurnEndTriggerTrait => TurnEndTriggerTraitManager.TurnEndTriggerTrait;

}