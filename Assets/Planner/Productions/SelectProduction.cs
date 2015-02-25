using System;
using System.Collections.Generic;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    public class SelectProduction : IRenderableProduction
    {
        public SelectProduction(RegistrarProduction parentProduction)
        {
            ParentProduction = parentProduction;
        }

        public string Name { get; set; }
        public bool IsOccluder { get; set; }
        public float FaceBreadth { get; set; }
        public ScopeFace[] Faces { get; set; }
        public bool IsAbsolute { get; set; }
        public RegistrarProduction SelectandProduction { get; set; }
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
                Debug.Log(string.Format("Found a null parent production while trying to expand select: {0}", Name));
            }

            var expansion = new List<IRenderableProduction>();

            var scopes = ParentProduction.Scope.Select(this);
            foreach (var scope in scopes)
            {
                SelectandProduction.Scope = scope;
                expansion.Add(SelectandProduction.Clone());
            }

            return expansion;
        }

        public IRenderableProduction Clone()
        {
            var clone = new SelectProduction(ParentProduction)
                            {
                                Faces = Faces,
                                FaceBreadth = FaceBreadth,
                                IsAbsolute = IsAbsolute,
                                SelectandProduction = (RegistrarProduction)SelectandProduction.Clone(),
                                Name = Name,
                                IsOccluder = IsOccluder
                            };
            return clone;
        }
    }
}