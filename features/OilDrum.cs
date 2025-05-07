using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nickel;
using System.Runtime.InteropServices;

namespace Sierra.features;

public class OilDrum : StuffBase
{
    public override void Render(G g, Vec v)
    {
        DrawWithHilight(g, ModEntry.Instance.oilDrumSprite.Sprite, v + GetOffset(g));
    }
    public override Spr? GetIcon() => new Spr?(ModEntry.Instance.oilDrumIcon.Sprite);
    public override List<Tooltip> GetTooltips()
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
    public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
    {
        c.Queue(new AStatus()
        {
            status = OilManager.OilStatus.Status,
            statusAmount = 1,
            targetPlayer = wasPlayer
        });
        return null;
    }
    public override List<CardAction>? GetActions(State s, Combat c)
    {
        return null;
    }
    public override double GetWiggleAmount()
    {
        return 1.0;
    }

    public override double GetWiggleRate()
    {
        return 1.0;
    }
}
