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

internal sealed class DoubleBarrelCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "DoubleBarrel", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/DoubleBarrel.png")).Sprite
        });
    }


    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new () { cost = 2, flippable = true},
            Upgrade.B => new() { cost = 1 },
            _ => new() { cost = 2 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new ASpawn()
                {
                    multiBayVolley = true,
                    thing = new OilDrum()
                },
                new AMove()
                {
                    dir = -1,
                    targetPlayer = true
                },
                new ASpawn()
                {
                    multiBayVolley = true,
                    thing = new OilDrum()
                },
            ],
            _ => [
                new ASpawn()
                {
                    multiBayVolley = true,
                    thing = new OilDrum()
                },
                new AMove()
                {
                    dir = -2,
                    targetPlayer = true
                },
                new ASpawn()
                {
                    multiBayVolley = true,
                    thing = new OilDrum()
                }
            ],

        };
}
