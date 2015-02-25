using System;
using System.Collections.Generic;

namespace Assets.Planner.Productions
{
    public class CaseProduction
    {
        public Func<RegistrarProduction, bool> Case { get; set; }
        public Func<RegistrarProduction, IRenderableProduction> Body { get; set; }

        public IList<IRenderableProduction> Expand(RegistrarProduction parent)
        {
            return Body(parent).Expand();
        }
    }
}
