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

internal sealed class DominateCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Dominate", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Dominate.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Dominate", "description", upgrade.ToString()]) },
            _ => new() { cost = 0, description = ModEntry.Instance.Locs.Localize(["card", "Dominate", "description", upgrade.ToString()]) }
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new NukeTheFuckingMidrow()
                {
                    playerDidIt = true
                },
                new ADrawCard()
                {
                    count = 1
                }
            ],
            _ => [
                new NukeTheFuckingMidrow()
                {
                    playerDidIt = true
                }
            ],

        };
    public class NukeTheFuckingMidrow : CardAction
    {
        public required bool playerDidIt = true;
        private int x;
        
        public override void Begin(G g, State s, Combat c)
        {
            foreach (var item in c.stuff.Values.ToList())
            {
                x = item.x;
                if (item is not null)
                {
                    c.DestroyDroneAt(s, x, playerDidIt);
                }
            }
        }
    }
}


