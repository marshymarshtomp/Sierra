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
using Sierra.cards.special;

namespace Sierra.cards.uncommon;

internal sealed class HammerlockCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Hammerlock", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Hammerlock.png")).Sprite
        });
    }
    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Hammerlock", "description", upgrade.ToString()]) },
            Upgrade.B => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Hammerlock", "description", upgrade.ToString()]) },
            _ => new() { cost = 1, description = ModEntry.Instance.Locs.Localize(["card", "Hammerlock", "description", upgrade.ToString()]) },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AAddCard()
                {
                    card = new BonkCard(),
                    amount = 2,
                    destination = CardDestination.Hand
                }
            ],
            Upgrade.B => [
                new AAddCard()
                {
                    card = new BonkCard()
                    {
                        upgrade = Upgrade.B,
                    },
                    destination = CardDestination.Hand
                }
            ],
            _ => [
                new AAddCard()
                {
                    card = new BonkCard(),
                    destination = CardDestination.Hand
                }
            ],

        };
}
