using System.Collections.Generic;
using System.Linq;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    public class SwitchProduction : IRenderableProduction
    {
        public SwitchProduction(RegistrarProduction parentProduction)
        {
            ParentProduction = parentProduction;
        }

        public string Name { get; set; }
        public bool IsOccluder { get; set; }
        public IRenderableProduction DefaultProduction { get; set; }
        public IList<CaseProduction> Cases { get; set; }
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
                Debug.Log(string.Format("Found a null parent production while trying to expand switch: {0}", Name));
            }

            var expansion = new List<IRenderableProduction>();

            var switchResult = Cases.FirstOrDefault(prod => prod.Case(ParentProduction));
            var resProd = switchResult != null
                           ? switchResult.Body(ParentProduction)
                           : DefaultProduction;

            expansion.Add(resProd);

            return expansion;
        }

        public IRenderableProduction Clone()
        {
            var clone = new SwitchProduction(ParentProduction)
                            {
                                DefaultProduction = DefaultProduction.Clone(),
                                Cases = Cases,
                                Name = Name,
                                IsOccluder = IsOccluder
                            };
            return clone;
        }
    }
}
