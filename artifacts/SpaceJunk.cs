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

internal sealed class SpaceJunk : Artifact
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
                pools = [ArtifactPool.Common],
                unremovable = false
            },
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/SpaceJunk.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "SpaceJunk", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "SpaceJunk", "description"]).Localize
        });
    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new GlossaryTooltip($"midrow.{ModEntry.Instance.Package.Manifest.UniqueName}::OilDrum")
            {
                Icon = ModEntry.Instance.oilDrumIcon.Sprite,
                TitleColor = Colors.midrow,
                Title = ModEntry.Instance.Locs.Localize(["midrow", "OilDrum", "name"]),
                Description = ModEntry.Instance.Locs.Localize(["midrow", "OilDrum", "description"])
            },
            .. StatusMeta.GetTooltips(OilManager.OilStatus.Status, 1)
            ];
    }
    public override void OnCombatStart(State state, Combat combat)
    {
        List<int> validSpaces = new List<int>();
        for (int i = state.ship.x - 1; i < state.ship.x + state.ship.parts.Count() + 1; i++)
        {
            if (!combat.stuff.ContainsKey(i))
            {
                validSpaces.Add(i);
            }
        }
        List<int> finalSpaces = validSpaces.Shuffle(state.rngActions).Take(2).ToList();
        foreach (int x in finalSpaces)
        {
            combat.stuff.Add(x, new OilDrum
            {
                x = x,
                xLerped = x
            });
        }
        if (finalSpaces.Count > 0)
        {
            Pulse();
        }
    }
}


