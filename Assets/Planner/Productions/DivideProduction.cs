using System.Collections.Generic;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    public class DivideProduction: IRenderableProduction
    {
        public DivideProduction(RegistrarProduction parentProduction)
        {
            ParentProduction = parentProduction;
        }

        public string Name { get; set; }
        public bool IsOccluder { get; set; }
        public string[] SnapToPlanes { get; set; }
        public ProductionAxis DivisionAxis { set; get; }
        public DivisorProduction[] Divisors { get; set; }
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
                Debug.Log(string.Format("Found a null parent production while trying to expand divide: {0}", Name));
            }

            var expansion = new List<IRenderableProduction>();

            var scopes = ParentProduction.Scope.Divide(this);
            for (var i = 0; i < Divisors.Length; i++)
            {
                var dividendProd = Divisors[i].DividendProduction(ParentProduction);
                dividendProd.Scope = scopes[i];

                expansion.Add(dividendProd);
            }

            return expansion;
        }

        public IRenderableProduction Clone()
        {
            var clone = new DivideProduction(ParentProduction)
                            {
                                DivisionAxis = DivisionAxis,
                                SnapToPlanes = SnapToPlanes,
                                Name = Name,
                                IsOccluder = IsOccluder,
                                Divisors = Divisors
                            };
            return clone;
        }
    }
}
