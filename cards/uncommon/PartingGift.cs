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

internal sealed class PartingGiftCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "PartingGift", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/PartingGift.png")).Sprite
        });
    }


    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0, exhaust = true},
            _ => new() { cost = 1, exhaust = true },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new AMove()
                {
                    dir = 2,
                    targetPlayer = true
                },
                new ASpawn()
                {
                    thing = new RepairKit(),
                },
                new AMove()
                {
                    dir = 2,
                    targetPlayer = true
                }
            ],
            _ => [
                new ASpawn()
                {
                    thing = new RepairKit(),
                },
                new AMove()
                {
                    dir = 3,
                    targetPlayer = true
                }
            ]

        };
}
