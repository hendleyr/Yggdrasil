using System;
using Assets.Planner.Productions;

namespace Assets.Planner.Scopes
{
    public class CylinderProductionScope : ProductionScope
    {
        public CylinderProductionScope(ProductionScope parent)
            : base(parent)
        {
        }

        public override ProductionScope Clone()
        {
            throw new NotImplementedException();
        }

        public override ProductionScope[] Select(SelectProduction selector)
        {
            throw new NotImplementedException();
        }

        public override ProductionScope[] Divide(DivideProduction divider)
        {
            throw new NotImplementedException();
        }

        public override ProductionScope[] Repeat(RepeatProduction repeater)
        {
            throw new NotImplementedException();
        }
        
        public override bool IsOccluded()
        {
            throw new NotImplementedException();
        }
    }
}
