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

namespace Sierra.cards.special;

internal sealed class BoozeCruiseCard : Card, IRegisterable
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
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "BoozeCruise", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/BoozeCruise.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0, temporary = true },
            _ => new() { cost = 1, temporary = true},
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new AStatus()
                {
                    status = Status.evade,
                    targetPlayer = true,
                    statusAmount = 2
                },
                new AStatus()
                {
                    status = OilManager.OilStatus.Status,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ],
            _ => [
                new AStatus()
                {
                    status = Status.evade,
                    targetPlayer = true,
                    statusAmount = 1
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
