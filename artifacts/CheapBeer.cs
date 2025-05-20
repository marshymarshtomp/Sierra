using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Sierra.features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sierra.artifacts;

internal sealed class CheapBeer : Artifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
        helper.Content.Artifacts.RegisterArtifact(type.Name, new()
        {
            ArtifactType = type,
            Meta = new()
            {
                owner = ModEntry.Instance.SierraDeck.Deck,
                pools = [ArtifactPool.Boss],
                unremovable = false
            },
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/CheapBeer.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "CheapBeer", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "CheapBeer", "description"]).Localize
        });
        ModEntry.Instance.Harmony.Patch(
                original: AccessTools.Method(typeof(AStatus), nameof(AStatus.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AStatus_Begin_Prefix))
            );
    }
    public override int ModifyStatusAmount(int baseAmount, Card card, State state, Combat? combat)
    {
        return base.ModifyStatusAmount(baseAmount, card, state, combat);
    }
    private static void AStatus_Begin_Prefix (State s, AStatus __instance)
    {
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is CheapBeer) is { } artifact)
        {
            if (!DB.statuses[__instance.status].isGood && __instance.statusAmount > 0) __instance.statusAmount += 1;
        }
    }
}
