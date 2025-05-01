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

internal sealed class BobAndWeaveCard : Card, IRegisterable, IHasCustomCardTraits
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "BobAndWeave", "name"]).Localize,
            Art =ModEntry.Instance.EndTrigger1.Sprite
        });
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch
    {
        _ => new HashSet<ICardTraitEntry>() { TurnEndTriggerTraitManager.TurnEndTriggerTrait }
    };

    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 0 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AMove()
                {
                    dir = 2,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = Status.tempShield,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AMove()
                        {
                            dir = -2,
                            targetPlayer = true
                        }
                    ).AsCardAction,
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = Status.tempShield,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                    ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            Upgrade.B => [
                new AMove()
                {
                    dir = 3,
                    targetPlayer = true
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AMove()
                        {
                            dir = -3,
                            targetPlayer = true
                        }
                    ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new AMove()
                {
                    dir = 2,
                    targetPlayer = true
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AMove()
                        {
                            dir = -2,
                            targetPlayer = true
                        }
                    ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
