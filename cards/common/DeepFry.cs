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

internal sealed class DeepFryCard : Card, IRegisterable, IHasCustomCardTraits
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "DeepFry", "name"]).Localize,
            Art = ModEntry.Instance.EndTrigger1.Sprite
        });
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch
    {
        _ => new HashSet<ICardTraitEntry>() { TurnEndTriggerTraitManager.TurnEndTriggerTrait }
    };
    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 1 }
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AAttack()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    damage = GetDmg(s, 1),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 3,
                        targetPlayer = false
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            Upgrade.B => [
                new AAttack()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 2,
                    damage = GetDmg(s, 1),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 2,
                        targetPlayer = false
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new AAttack()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    damage = GetDmg(s, 1),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 2,
                        targetPlayer = false
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
