using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;

namespace Sierra.features;

internal sealed class IntimidationManager : IRegisterable
{
    private static bool fromAAttack = false;
    private static StuffBase? theStuff;
    private static int stuffTime = 0;
    internal static IStatusEntry IntimidationStatus { get; private set; } = null!;
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        IntimidationStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Intimidation", new()
        {
            Definition = new()
            {
                icon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/intimidation.png")).Sprite,
                color = new("fc3c3c"),
                isGood = false
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["status", "Intimidation", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["status", "Intimidation", "description"]).Localize
        });
        ModEntry.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Combat_DestroyDroneAt_Prefix))
            );
       /*ModEntry.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Combat_DestroyDroneAt_Prefix))
            );
            }
            private static void ASpawn_Begin_Prefix(G g, State s, Combat c, ASpawn __instance)
            {
                theStuff = __instance.thing;
                fromASpawn = true;
            }
            private static void ASpawn_Begin_Finalizer(G g, State s, Combat c, ASpawn __instance) 
            {
                theStuff = null;
                fromASpawn = false;
            }
            private static void Combat_DestroyDroneAt_Prefix(State s, int x, bool playerDidIt)
            {
                if (s.route is Combat c)
                {
                    var shipToTarget = playerDidIt ? c.otherShip : s.ship;
                    if (shipToTarget.Get(IntimidationStatus.Status) <= 0) return;
                    if (fromASpawn)
                    {
                        if (theStuff is not null && c.stuff.GetValueOrDefault(x) != theStuff) return;
                    }
                    c.Queue(new AHurt()
                    {
                        targetPlayer = !playerDidIt,
                        hurtAmount = shipToTarget.Get(IntimidationStatus.Status),
                        hurtShieldsFirst = true
                    });
                    shipToTarget.Set(IntimidationStatus.Status, shipToTarget.Get(IntimidationStatus.Status) - 1);
                }
            */

    }
    private static void Combat_DestroyDroneAt_Prefix(State s, int x, bool playerDidIt)
    {
        if (s.route is Combat c)
        {
            var currStuff = c.stuff.GetValueOrDefault(x);
            var shipToTarget = playerDidIt ? c.otherShip : s.ship;
            if (currStuff is null) return;
            if (shipToTarget.Get(IntimidationStatus.Status) <= 0) return;
            if (currStuff.age == 0) return;
            c.Queue(new AHurt()
            {
                targetPlayer = !playerDidIt,
                hurtAmount = shipToTarget.Get(IntimidationStatus.Status),
                hurtShieldsFirst = true
            });
            shipToTarget.Set(IntimidationStatus.Status, shipToTarget.Get(IntimidationStatus.Status) - 1);
        }
    }
}
