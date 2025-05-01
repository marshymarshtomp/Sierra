using Nanoray.PluginManager;
using Nickel;
using FSPRO;
using daisyowl.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sierra.ExternalAPIs;
using Sierra.features;
using static Sierra.features.PickDestroyMidrow;

namespace Sierra.cards.rare;

internal sealed class SingleOutCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "SingleOut", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/SingleOut.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0, description = ModEntry.Instance.Locs.Localize(["card", "SingleOut", "description", upgrade.ToString()]) },
            Upgrade.B => new() { cost = 1, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "SingleOut", "description", upgrade.ToString()]) },
            _ => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "SingleOut", "description", upgrade.ToString()]) },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            _ => [
                new PickMidrowToDestroyAction() {
                    currUpg = upgrade,
                    currCard = this
                }
            ],
        };
    
}
