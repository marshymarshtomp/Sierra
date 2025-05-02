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

internal sealed class BonkCard : Card, IRegisterable
{
    public int permDiscount = 0;
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Bonk", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Bonk.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 1, infinite = true, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]) },
            Upgrade.B => new() { cost = 0, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]) },
            _ => new() { cost = 0, infinite = true, retain = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]),  },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            _ => [
                new PickMidrowToDestroyAction()
                {
                    currCard = this,
                    currUpg = upgrade,
                }
            ],
        };
    public override void OnDraw(State s, Combat c)
    {
        if (this.upgrade == Upgrade.A)
        {
            this.discount -= 1;
        }
    }
    public override void AfterWasPlayed(State state, Combat c)
    {
        if (upgrade == Upgrade.None) { permDiscount+=1; discount = permDiscount; }
    }
    public override void OnExitCombat(State s, Combat c)
    {
        permDiscount = 0;
    }
}
