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

namespace Sierra.cards.uncommon;

internal sealed class DrunkenFuryCard : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.SierraDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "DrunkenFury", "name"]).Localize,
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
                new AAttack()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 2,
                    damage = GetDmg(s, 2),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            Upgrade.B => [
                new AAttack()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 3,
                    damage = GetDmg(s, 1),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new AAttack()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 2,
                    damage = GetDmg(s, 1),
                    targetPlayer = false
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                    new AStatus()
                    {
                        status = IntimidationManager.IntimidationStatus.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
