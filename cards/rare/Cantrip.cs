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

internal sealed class CantripCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Cantrip", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Cantrip.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 0 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    targetPlayer = false,
                    status = Status.heat,
                    statusAmount = 2
                },
                new ADrawCard()
                {
                    count = 1
                }
            ],
            Upgrade.B => [
                new AStatus()
                {
                    targetPlayer = false,
                    status = Status.heat,
                    statusAmount = 4
                },
                new ADrawCard()
                {
                    count = 1
                },
                new AStatus()
                {
                    targetPlayer = true,
                    status = Status.heat,
                    statusAmount = 1
                }
            ],
            _ => [
                new AStatus()
                {
                    targetPlayer = false,
                    status = Status.heat,
                    statusAmount = 1
                },
                new ADrawCard()
                {
                    count = 1
                }
            ],

        };
}
