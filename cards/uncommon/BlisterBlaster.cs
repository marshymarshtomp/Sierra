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

internal sealed class BlisterBlasterCard : Card, IRegisterable, IHasCustomCardTraits
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "BlisterBlaster", "name"]).Localize,
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
            _ => new() { cost = 2 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AAttack()
                {
                    piercing = true,
                    damage = GetDmg(s, 6),
                    status = Status.heat,
                    statusAmount = 3
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = Status.heat,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            Upgrade.B => [
                new AAttack()
                {
                    piercing = true,
                    damage = GetDmg(s, 5),
                    status = Status.heat,
                    statusAmount = 5
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = Status.heat,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],
            _ => [
                new AAttack()
                {
                    piercing = true,
                    damage = GetDmg(s, 4),
                    status = Status.heat,
                    statusAmount = 3
                },
                new ADummyAction(),
                ModEntry.Instance.KokoroApi.OnTurnEnd.MakeAction(
                        new AStatus()
                        {
                            status = Status.heat,
                            statusAmount = 1,
                            targetPlayer = true
                        }
                ).SetShowOnTurnEndIcon(false).SetShowOnTurnEndTooltip(false).AsCardAction
            ],

        };
}
