﻿using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sierra.features;
using static Sierra.features.PickDestroyMidrow;

namespace Sierra.cards.special;

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
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["card", "Bonk", "name"]).Localize,
            Art = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/Bonk.png")).Sprite
        });
    }

    public override CardData GetData(State state)
        => upgrade switch
        {
            Upgrade.A => new() { cost = 0, retain = true, temporary = true, exhaust = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]) },
            Upgrade.B => new() { cost = 0, retain = true, temporary = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]) },
            _ => new() { cost = 0, retain = true, temporary = true, exhaust = true, description = ModEntry.Instance.Locs.Localize(["card", "Bonk", "description", upgrade.ToString()]), },
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
            ]
        };
}
