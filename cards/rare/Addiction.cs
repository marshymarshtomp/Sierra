using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sierra.features;
using Sierra.cards.special;
using Sierra;

namespace Sierra.cards.rare;

internal sealed class AddictionCard : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.SierraDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Addiction", "name"]).Localize,
            Art = ModEntry.Instance.EndTrigger3.Sprite
        });
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch
    {
        _ => new HashSet<ICardTraitEntry>() { TurnEndTriggerTraitManager.TurnEndTriggerTrait }
    };

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { exhaust = true, cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Addiction", "description", upgrade.ToString()]) },
            Upgrade.B => new() { exhaust = true, cost = 2, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Addiction", "description", upgrade.ToString()]) },
            _ => new() { exhaust = true, cost = 2, description = ModEntry.Instance.Locs.Localize(["card", "Addiction", "description", upgrade.ToString()]) },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            _ => [
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AAddCard()
                    {
                        card = new BoozeCruiseCard()
                        {
                            discount = -1
                        },
                        destination = CardDestination.Hand
                    }).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
        };
}
