using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sierra.features;

namespace Sierra.cards.common;

internal sealed class PumpTheGasCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "PumpTheGas", "name"]).Localize
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.B => new() { cost = 1, flippable = true, art = flipped ? ModEntry.Instance.PumpTheGas.Sprite : ModEntry.Instance.PumpTheGasFlipped.Sprite },
            _ => new() { cost = 1, art = flipped ? ModEntry.Instance.PumpTheGasFlipped.Sprite : ModEntry.Instance.PumpTheGas.Sprite }
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    targetPlayer = false,
                    statusAmount = 2
                }
            ],
            Upgrade.B => [
                new AMove()
                {
                    targetPlayer = true,
                    dir = -1,
                },
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    targetPlayer = false,
                    statusAmount = 1
                }
            ],
            _ => [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    targetPlayer = false,
                    statusAmount = 1
                }
            ],

        };
}
