using Nanoray.PluginManager;
using Nickel;
using Sierra.cards.special;
using Sierra.cards.uncommon;
using Sierra.features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sierra.artifacts;

internal sealed class MrBreaky : Artifact
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
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/MrBreaky.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "MrBreaky", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "MrBreaky", "description"]).Localize
        });
    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new BonkCard() }
            ];
    }
    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new AAddCard()
        {
            card = new BonkCard()
            {
                temporaryOverride = true,
            },
            destination = CardDestination.Hand
        });
    }
}
