using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Sierra.ExternalAPIs.Kokoro;
using Sierra.features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sierra.artifacts;

internal sealed class SprayCan : Artifact
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
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/SprayCan.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "SprayCan", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "SprayCan", "description"]).Localize
        });
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(Card_GetActionsOverridden_Postfix)))
        );
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            .. StatusMeta.GetTooltips(Status.heat, 1),
            ];
    }
    private static void Card_GetActionsOverridden_Postfix(State s, ref List<CardAction> __result, Card __instance)
    {
        var card = __instance;
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is SprayCan) is not { } artifact) return;
        if (s.route is Combat c)
        {
            if (s.deck.Contains(card) || c.discard.Contains(card) || c.exhausted.Contains(card) || c.hand.Contains(card))
            {
                foreach (var actions in __result)
                {
                    foreach (var unwrappedAction in ModEntry.Instance.KokoroApi.WrappedActions.GetWrappedCardActionsRecursively(actions))
                    {
                        if (unwrappedAction is AAttack attack)
                        {
                            if (attack.status == Status.heat && attack.targetPlayer == false) attack.statusAmount += 1;
                        }
                        if (unwrappedAction is AStatus status)
                        {
                            if (status.status == Status.heat && status.targetPlayer == false) status.statusAmount += 1;
                        }
                    }
                }
            }
        }
    }
}