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

internal sealed class MayhemCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Mayhem", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Scorch.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            _ => new() { cost = 2 }
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.A => [
                new AStatus()
                {
                    status = Status.shield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 2
                },
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 2
                }
            ],
            Upgrade.B => [
                new AStatus()
                {
                    status = Status.shield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AAttack()
                {
                    damage = GetDmg(s, 1),
                    status = Status.heat,
                    statusAmount = 2
                },
                new AAttack()
                {
                    damage = GetDmg(s, 1),
                    status = Status.heat,
                    statusAmount = 2
                },
                new AStatus()
                {
                    status = Status.heat,
                    targetPlayer = true,
                    statusAmount = 1
                }
            ],
            _ => [
                new AStatus()
                {
                    status = Status.shield,
                    targetPlayer = true,
                    statusAmount = 3
                },
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 2
                },
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 2
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
