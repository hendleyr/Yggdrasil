using System;
using System.Collections.Generic;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    /// <summary>
    /// Repeat magnitudes are always absolute. 
    /// Create n production scopes along the repeat axis, where n = parent scope axis length / magnitude
    /// </summary>
    public class RepeatProduction : IRenderableProduction
    {
        public RepeatProduction(RegistrarProduction parentProduction)
        {
            ParentProduction = parentProduction;
        }

        public string Name { get; set; }
        public bool IsOccluder { get; set; }
        public float Magnitude { get; set; }
        public string SnapPlaneKey { get; set; }
        public string[] SnapToPlanes { get; set; }
        public ProductionAxis RepetitionAxis { get; set; }
        public RepeatRemainderMode RemainderMode { get; set; }
        public Func<RegistrarProduction, IRenderableProduction> ReplicandProduction { get; set; }
        public ProductionScope Scope
        {
            get { return ProductionScope.EmptyScope; }
            set { }
        }
        public RegistrarProduction ParentProduction { get; set; }

        public IList<IRenderableProduction> Expand()
        {
            if (ParentProduction == null || ParentProduction.Scope == null)
            {
                Debug.Log(string.Format("Found a null parent production while trying to expand repeat: {0}", Name));
            }

            var expansion = new List<IRenderableProduction>();

            var scopes = ParentProduction.Scope.Repeat(this);
            foreach (var scope in scopes)
            {
                var prod = ReplicandProduction(ParentProduction);
                prod.Scope = scope;

                expansion.Add(prod);
            }

            return expansion;
        }

        public IRenderableProduction Clone()
        {
            var clone = new RepeatProduction(ParentProduction)
                            {
                                RepetitionAxis = RepetitionAxis,
                                Magnitude = Magnitude,
                                RemainderMode = RemainderMode,
                                ReplicandProduction = ReplicandProduction,
                                SnapToPlanes = SnapToPlanes,
                                SnapPlaneKey = SnapPlaneKey,
                                Name = Name,
                                IsOccluder = IsOccluder
                            };
            return clone;
        }
    }
}
