using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Sierra.features;

internal sealed class IntimidationManager : IRegisterable
{
    private static bool droneGotFuckingAnnihilated = false;
    private static bool fromASpawn = false;
    private static Ship? theShipAnnihilatingTheMidrow;
    private static Ship? theOtherPeskyShip;
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
        /*        ModEntry.Instance.Harmony!.Patch(
                        original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
                        postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Combat_DestroyDroneAt_Postfix))
                        );*/
        ModEntry.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(ASpawn), nameof(ASpawn.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ASpawn_Begin_Prefix)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ASpawn_Begin_Postfix)),
                finalizer: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ASpawn_Begin_Finalizer))
            );
        ModEntry.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Prefix)),
                finalizer: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Finalizer))
            );
        ModEntry.Instance.Harmony.PatchVirtual(
                original: AccessTools.DeclaredMethod(typeof(StuffBase), nameof(StuffBase.DoDestroyedEffect)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(StuffBase_DoDestroyedEffect_Postfix))
            );
    }
    /*    private static void Combat_DestroyDroneAt_Postfix(State s, int x, bool playerDidIt)
        {
            if (s.route is Combat c)
            {
                var revship = playerDidIt ? c.otherShip : s.ship;
                if (revship.Get(IntimidationStatus.Status)<=0) return;
                c.QueueImmediate(new AHurt()
                {
                    targetPlayer = !playerDidIt,
                    hurtAmount = revship.Get(IntimidationStatus.Status),
                    hurtShieldsFirst = true
                });
                revship.Set(IntimidationStatus.Status, revship.Get(IntimidationStatus.Status) - 1);
            }
        }*/
    private static void ASpawn_Begin_Prefix(G g, State s, Combat c, ref StuffBase? __state, ASpawn __instance)
    {
        droneGotFuckingAnnihilated = false;
        int worldX = __instance.GetWorldX(s, c);
        __state = c.stuff.GetValueOrDefault(worldX);
        theShipAnnihilatingTheMidrow = __instance.fromPlayer ? s.ship : c.otherShip;
        theOtherPeskyShip = __instance.fromPlayer ? c.otherShip : s.ship;
        fromASpawn = true;
    }
    private static void ASpawn_Begin_Postfix(G g, State s, Combat c, ref StuffBase? __state, ASpawn __instance)
    {
        int worldX = __instance.GetWorldX(s, c);
        if (c.stuff.GetValueOrDefault(worldX) != __state && __state != null)
        {
            int intimAmt = theOtherPeskyShip.Get(IntimidationStatus.Status);
            bool targetPlr = theOtherPeskyShip.isPlayerShip ? true : false;
            c.QueueImmediate(
            [
                new AHurt
                {
                    targetPlayer = targetPlr,
                    hurtAmount = intimAmt,
                    hurtShieldsFirst = true
                },
                new AStatus
                {
                    targetPlayer = targetPlr,
                    status = IntimidationStatus.Status,
                    statusAmount = -1
                }
            ]);
        }
    }
    private static void ASpawn_Begin_Finalizer() { theShipAnnihilatingTheMidrow = null; theOtherPeskyShip = null; fromASpawn = false; }
    private static void AAttack_Begin_Prefix(AAttack __instance, State s, Combat c)
    {
        theShipAnnihilatingTheMidrow = __instance.targetPlayer ? c.otherShip : s.ship;
        theOtherPeskyShip = __instance.targetPlayer ? s.ship : c.otherShip;
        fromASpawn = false;
    }

    private static void AAttack_Begin_Finalizer() { theShipAnnihilatingTheMidrow = null; theOtherPeskyShip = null; }
    private static void StuffBase_DoDestroyedEffect_Postfix()
    {
        if (theShipAnnihilatingTheMidrow is null)
            return;
        if (theOtherPeskyShip is null)
            return;
        if (fromASpawn)
            return;
        int intimAmt = theOtherPeskyShip.Get(IntimidationStatus.Status);
        if (intimAmt <= 0)
            return;
        bool targetPlr = theOtherPeskyShip.isPlayerShip ? true : false;
        (MG.inst.g.state?.route as Combat)?.QueueImmediate(
            [
                new AHurt
                {
                    targetPlayer = targetPlr,
                    hurtAmount = intimAmt,
                    hurtShieldsFirst = true
                },
                new AStatus
                {
                    targetPlayer = targetPlr,
                    status = IntimidationStatus.Status,
                    statusAmount = -1
                }
            ]
        );
    }
}
