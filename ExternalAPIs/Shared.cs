using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra.ExternalAPIs;

internal sealed class MouseDownHandler(Action @delegate) : OnMouseDown
{
    public void OnMouseDown(G g, Box b)
        => @delegate();
}
internal static class StateExt
{
    public static IEnumerable<Card> GetAllCards(this State state)
    {
        IEnumerable<Card> results = state.deck;
        if (state.route is Combat combat)
            results = results.Concat(combat.hand).Concat(combat.discard).Concat(combat.exhausted);
        return results;
    }
}
