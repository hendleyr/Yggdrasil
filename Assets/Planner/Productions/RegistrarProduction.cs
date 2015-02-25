using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    public class RegistrarProduction : IRenderableProduction
    {
        public static RegistrarProduction Empty = new RegistrarProduction("Empty", new CubeProductionScope(new Bounds(), Quaternion.identity)) {ChildProductions = new IRenderableProduction[0]};

        public RegistrarProduction(string name, ProductionScope scope)
        {
            _finalFlag = false;
            Name = name;
            Scope = new CubeProductionScope(scope);
            ChildProductions = new List<IRenderableProduction>();
        }

        public RegistrarProduction()
        {
            Scope = ProductionScope.EmptyScope;
            ChildProductions = new List<IRenderableProduction>();
        }

        public string Name { get; private set; }
        public bool IsOccluder { get; set; }
        private bool _finalFlag;
        public ProductionScope Scope { get; set; }
        public RegistrarProduction ParentProduction { get; set; }
        private IList<IRenderableProduction> ChildProductions { get; set; }

        public IList<IRenderableProduction> Expand()
        {
            if (Scope == null)
            {
                Debug.Log(string.Format("Found a null scope while trying to expand registrar: {0}", Name));
            }

            if (IsOccluder)
            {
                ModuleProduction.Occluders.Add(Scope);
            }

            return ChildProductions.ToList();
        }

        public IRenderableProduction Clone()
        {
            var clone = new RegistrarProduction(Name, Scope)
                            {
                                Name = Name, 
                                Scope = Scope.Clone(), 
                                IsOccluder = IsOccluder,
                                ParentProduction = ParentProduction,
                                _finalFlag = _finalFlag
                            };

            foreach (var prod in ChildProductions)
            {
                var childClone = prod.Clone();
                childClone.ParentProduction = clone;
                clone.ChildProductions.Add(childClone);
            }

            return clone;
        }

        #region Finalizing Productions

        /// <summary>
        ///  Will set child productions to the default production, or the FIRST conditional production which passes its truth test.
        /// </summary>
        /// <param name="switchFunc"></param>
        public RegistrarProduction Switch(Func<RegistrarProduction, SwitchProduction> switchFunc)
        {
            if (!_finalFlag)
            {
                ChildProductions.Add(switchFunc(this));
                _finalFlag = true;
            }
            else
            {
                Debug.Log(string.Format("Encountered an illegal production definition: Registrar {0} already finalized, may not parse Switch production.", Name));
            }

            return this;
        }

        public RegistrarProduction Divide(DivideProduction divideProduction)
        {
            if (!_finalFlag)
            {
                ChildProductions.Add(divideProduction);
                _finalFlag = true;
            }
            else
            {
                Debug.Log(string.Format("Encountered an illegal production definition: Registrar {0} already finalized, may not parse Divide production.", Name));
            }

            return this;
        }

        public RegistrarProduction Repeat(Func<RegistrarProduction, RepeatProduction> repeatFunc)
        {
            if (!_finalFlag)
            {
                ChildProductions.Add(repeatFunc(this));
                _finalFlag = true;
            }
            else
            {
                Debug.Log(string.Format("Encountered an illegal production definition: Registrar {0} already finalized, may not parse Repeat production.", Name));
            }

            return this;
        }

        //public RegistrarProduction Repeat(RepeatProduction repeater)
        //{
        //    if (!_finalFlag)
        //    {
        //        ChildProductions.Add(repeater);
        //        _finalFlag = true;
        //    }
        //    else
        //    {
        //        Debug.Log(string.Format("Encountered an illegal production definition: Registrar {0} already finalized, may not parse Repeat production.", Name));
        //    }

        //    return this;
        //}

        public RegistrarProduction Terminate(TerminalProduction terminal)
        {
            if (!_finalFlag)
            {
                var term = terminal.Clone();
                term.ParentProduction = this;
                ChildProductions.Add(term);
                _finalFlag = true;
            }
            else
            {
                Debug.Log(string.Format("Encountered an illegal production definition: Registrar {0} already finalized, may not parse Terminate production.", Name));
            }
            
            return this;
        }
        #endregion

        #region Non-Finalizing Productions
        public RegistrarProduction Select(SelectProduction selectProduction)
        {
            ChildProductions.Add(selectProduction);
            return this;
        }
        public RegistrarProduction Select(Func<RegistrarProduction, SelectProduction> selectFunc)
        {
            ChildProductions.Add(selectFunc(this));
            return this;
        }

        public RegistrarProduction BoxSplit(RegistrarProduction box)
        {
            ChildProductions.Add(box);
            return this;
        }

        public RegistrarProduction PrismSplit()
        {
            throw new NotImplementedException();
            //return this;
        }

        public RegistrarProduction CylinderSplit()
        {
            throw new NotImplementedException();
            //return this;
        }

        public RegistrarProduction Move(Vector3 move)
        {
            Scope.Bounds = new Bounds(Scope.Bounds.center + move, Scope.Bounds.size);
            return this;
        }

        public RegistrarProduction Scale(ScaleProduction scale)
        {
            if (scale.Magnitude == null || scale.Magnitude.Length < 3 || scale.IsAbsolute == null || scale.IsAbsolute.Length < 3)
            {
                Debug.Log(string.Format("Encountered an illegal production definition: Scale production is not fully defined."));
                throw new Exception(string.Format("Encountered an illegal production definition: Scale production is not fully defined."));
            }

            var s = Scope.Bounds.size;

            for (var i = 0; i < 3; ++i)
            {
                s[i] = scale.IsAbsolute[i]
                        ? scale.Magnitude[i]       
                        : s[i] * scale.Magnitude[i];
            }
            Scope.Bounds = new Bounds(Scope.Bounds.center, s);

            return this;
        }

        public RegistrarProduction Rotate(Quaternion quaternion)
        {
            Scope.Rotation = Scope.Rotation*quaternion;
            return this;
        }
        #endregion
    }
}
