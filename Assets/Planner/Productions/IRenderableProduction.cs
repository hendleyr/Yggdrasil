using System.Collections.Generic;
using Assets.Planner.Scopes;

namespace Assets.Planner.Productions
{
    public interface IRenderableProduction
    {
        string Name { get; }
        bool IsOccluder { get; set; }
        ProductionScope Scope { get; set; }
        RegistrarProduction ParentProduction { get; set; }

        IList<IRenderableProduction> Expand();
        IRenderableProduction Clone();
    }
}
