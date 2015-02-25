using Assets.Planner.Productions;
using UnityEngine;

namespace Assets.Planner.Scopes
{
    public abstract class ProductionScope
    {
        public static ProductionScope EmptyScope = new CubeProductionScope(new Bounds(), Quaternion.identity);

        // distribute volume remainder to replicands if the remainder <= repetition epsilon (1 meter)
        protected static float RepetitionEpsilon = 1;
        public Bounds Bounds { get; set; }
        public Quaternion Rotation { get; set; }

        protected ProductionScope(ProductionScope parent)
        {
            Bounds = parent.Bounds;
            Rotation = parent.Rotation;
        }

        protected ProductionScope(Bounds bounds, Quaternion rotation)
        {
            Bounds = bounds;
            Rotation = rotation;
        }

        public abstract ProductionScope Clone();

        public abstract ProductionScope[] Select(SelectProduction selector);
        public abstract ProductionScope[] Divide(DivideProduction divider);
        public abstract ProductionScope[] Repeat(RepeatProduction repeater);

        public abstract bool IsOccluded();

        public override string ToString()
        {
            return string.Format("Center: {0} {1} {2}; Size: {3} {4} {5}",
                                 Bounds.center.x, Bounds.center.y, Bounds.center.z,
                                 Bounds.size.x, Bounds.size.y, Bounds.size.z);
        }
    }
}