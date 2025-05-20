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

namespace Sierra.cards.rare;

internal sealed class FOMOCard : Card, IRegisterable, IHasCustomCardTraits
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "FOMO", "name"]).Localize,
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
            _ => new() { cost = 1 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new ASpawn()
                {
                    thing = new RepairKit()
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        targetPlayer = true,
                        statusAmount = 1
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            Upgrade.B => [
                new ASpawn()
                {
                    thing = new RepairKit()
                    {
                        bubbleShield = true
                    }
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        targetPlayer = true,
                        statusAmount = 2
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new ASpawn()
                {
                    thing = new RepairKit()
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        targetPlayer = true,
                        statusAmount = 2
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
