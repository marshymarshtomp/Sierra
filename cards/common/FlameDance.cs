using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sierra.features;
using Sierra;

namespace Sierra.cards.common;

internal sealed class FlameDanceCard : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.SierraDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "FlameDance", "name"]).Localize,
            Art = ModEntry.Instance.EndTrigger2.Sprite
        });
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch
    {
        _ => new HashSet<ICardTraitEntry>() { TurnEndTriggerTraitManager.TurnEndTriggerTrait }
    };

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0 },
            _ => new() { cost = 1 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new AStatus()
                {
                    status = Status.tempShield,
                    statusAmount = 4,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = OilManager.OilStatus.Status,
                            statusAmount = 2,
                            targetPlayer = true
                        }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new AStatus()
                {
                    status = Status.tempShield,
                    statusAmount = 3,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = OilManager.OilStatus.Status,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
