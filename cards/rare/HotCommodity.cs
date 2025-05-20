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

internal sealed class HotCommodityCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "HotCommodity", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/HotCommodity.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.B => new() { cost = 1 },
            _ => new() { cost = 2 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new ASpawn()
                {
                    thing = new RepairKit()
                }
            ],
            Upgrade.B => [
                new ASpawn()
                {
                    thing = new RepairKit()
                },                
                new AStatus()
                {
                    status = Status.heat,
                    targetPlayer = true,
                    statusAmount = 2
                }
            ],
            _ => [
                new ASpawn()
                {
                    thing = new RepairKit()
                },
                new AStatus()
                {
                    status = Status.heat,
                    targetPlayer = true,
                    statusAmount = 1
                }
            ],

        };
}
