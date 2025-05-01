using Sierra.cards.rare;
using Sierra.cards.uncommon;
using Sierra.ExternalAPIs;
using Sierra;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Sierra.features
{
    public sealed class PickDestroyMidrow
    {
        //thanks shockah
        
        private static readonly UK MidrowExecutionUK = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();
        private static readonly UK CancelExecutionUK = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();
        public sealed class PickMidrowToDestroyAction : CardAction
        {
            public required Upgrade currUpg;
            public required Card currCard;
            public override Route? BeginWithRoute(G g, State s, Combat c)
                => new ActionRoute()
                {
                    currUpg2 = currUpg,
                    currCard2 = currCard,
                };
        }
        public sealed class ActionRoute : Route
        {
            public required Upgrade currUpg2;
            public required Card currCard2;
            public override bool GetShowOverworldPanels() => true;
            public override bool CanBePeeked() => false;
            public override void Render(G g)
            {
                base.Render(g);
                if (g.state.route is not Combat combat)
                {
                    g.CloseRoute(this);
                    return;
                }



                Draw.Rect(0, 0, MG.inst.PIX_W, MG.inst.PIX_H, Colors.black.fadeAlpha(0.5));

                var centerX = g.state.ship.x + g.state.ship.parts.Count / 2.0;
                foreach (var (worldX, @object) in combat.stuff)
                {
                    if (Math.Abs(worldX - centerX) > 10) continue;
                    if (g.boxes.FirstOrDefault(b => b.key is { } key && key.k == StableUK.midrow && key.v == worldX) is not { } realBox) continue;
                    if (currCard2 is FuseCard)
                    {
                        var truth = false;
                        for(var i = 0; i < g.state.ship.parts.Count; i++)
                        {
                            if (currUpg2 is Upgrade.B && @object.x == g.state.ship.x + i) { truth = true; break; };
                            if (g.state.ship.parts[i].type == PType.missiles && @object.x == g.state.ship.x + i) { truth = true; break; }
                        }
                        if (!truth) { continue; }
                    }


                    var box = g.Push(new global::UIKey(MidrowExecutionUK, worldX), realBox.rect, onMouseDown: new MouseDownHandler(() => OnMidrowSelected(g, @object)));
                    @object.Render(g, box.rect.xy);
                    if (box.rect.x is > 60 and < 464.0 && box.IsHover())
                    {
                        if (!Input.gamepadIsActiveInput)
                        {
                            MouseUtil.DrawGamepadCursor(box);
                        }
                        g.tooltips.Add(box.rect.xy + new Vec(16.0, 24.0), @object.GetTooltips());
                        @object.hilight = 2;
                    }
                    g.Pop();
                }

                var cardName = "";
                switch(currCard2)
                {
                    case SingleOutCard:
                        cardName = "SingleOut";
                        break;
                    case BonkCard:
                        cardName = "Bonk";
                        break;
                    case FuseCard:
                        cardName = "Fuse";
                        break;
                    default:
                        cardName = "ERROR";
                        break;
                }

                SharedArt.ButtonText(
                        g,
                        new Vec(MG.inst.PIX_W - 69 /*nice*/, MG.inst.PIX_H - 31),
                        CancelExecutionUK,
                        ModEntry.Instance.Locs.Localize(["card", cardName, "ui", "cancel"]),
                        onMouseDown: new MouseDownHandler(() => g.CloseRoute(this))
                );
            }
            private void OnMidrowSelected(G g, StuffBase @object)
            {
                if (g.state.route is not Combat combat)
                {
                    g.CloseRoute(this);
                    return;
                }

                combat.DestroyDroneAt(g.state, @object.x, true);
                switch (currCard2)
                {
                    case SingleOutCard:
                        switch (currUpg2)
                        {
                            case Upgrade.A:
                                Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                                combat.Queue(new AStatus() { status = Status.tempShield, statusAmount = 3, targetPlayer = true });
                                break;
                            default:
                                Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                                combat.Queue(new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true });
                                break;
                        }
                        break;
                    case BonkCard:
                        Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                        break;
                    case FuseCard:
                        Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                        combat.Queue(new AStatus() { status = Status.shield, statusAmount = currUpg2 == Upgrade.A ? 3 : 2, targetPlayer = true });
                        break;
                    default:
                        break;
                }
               
                g.CloseRoute(this);
            }
        }
        private static void Combat_IsVisible_Postfix(Combat __instance, ref bool __result)
        {
            if (__instance.routeOverride is ActionRoute)
                __result = true;
        }
    }
}
