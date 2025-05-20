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

internal sealed class SadismCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Sadism", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Sadism.png")).Sprite
        });
    }
    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 2 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 4,
                    targetPlayer = false
                }
            ],
            Upgrade.B => [
                new AStatus()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 3,
                    targetPlayer = false
                },
                new ASpawn()
                {
                    thing = new OilDrum()
                }
            ],
            _ => [
                new AStatus()
                {
                    status = IntimidationManager.IntimidationStatus.Status,
                    statusAmount = 3,
                    targetPlayer = false
                }
            ],

        };
}
