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

internal sealed class InsolationCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Insolation", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Insolation.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 1 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    status = Status.tempShield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    targetPlayer = false
                }
            ],
            Upgrade.B => [
                new AStatus()
                {
                    status = Status.tempShield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 2,
                    targetPlayer = true
                }
            ],
            _ => [
                new AStatus()
                {
                    status = Status.tempShield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ],

        };
}
