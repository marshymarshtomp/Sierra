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
using static Sierra.features.PickDestroyMidrow;

namespace Sierra.cards.uncommon;

internal sealed class FuseCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Fuse", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Fuse.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 1, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Fuse", "description", upgrade.ToString()]) },
            Upgrade.B => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Fuse", "description", upgrade.ToString()]) },
            _ => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Fuse", "description", upgrade.ToString()]) },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            _ => [
                new PickMidrowToDestroyAction()
                {
                    currCard = this,
                    currUpg = upgrade
                }
            ],

        };
}
