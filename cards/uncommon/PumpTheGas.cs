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
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "PumpTheGas", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/PumpTheGas.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 1, flippable = true },
            _ => new() { cost = 1 }
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AVariableHint()
                {
                    status = OilManager.OilStatus.Status,
                },
                new AMove()
                {
                    dir = 2*s.ship.Get(OilManager.OilStatus.Status),
                    xHint = 2,
                    targetPlayer = true
                }
            ],
            Upgrade.B => [
                new AVariableHint()
                {
                    status = OilManager.OilStatus.Status,
                },
                new AMove()
                {
                    dir = 2*s.ship.Get(OilManager.OilStatus.Status),
                    xHint = 2,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ],
            _ => [
                new AStatus()
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AVariableHint()
                {
                    status = OilManager.OilStatus.Status,
                },
                new AMove()
                {
                    dir = 2*s.ship.Get(OilManager.OilStatus.Status),
                    xHint = 2,
                    targetPlayer = true
                }
            ],

        };
}
