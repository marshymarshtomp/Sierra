﻿using Nanoray.PluginManager;
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

internal sealed class BurnBabyBurnCard : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "BurnBabyBurn", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Scorch.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0 },
            _ => new() { cost = 1 },
        };
    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 3
                },
                new AAttack()
                {
                    damage = GetDmg(s, 0),
                    status = Status.heat,
                    statusAmount = 3
                }
            ],
            _ => [
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

        };
}
