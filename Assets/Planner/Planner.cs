using System;
using System.Collections.Generic;
using Assets.Planner.Productions;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner
{
    public class Planner : MonoBehaviour
    {
        private static readonly Dictionary<string, IRenderableProduction> ProductionDictionary = new Dictionary<string, IRenderableProduction>();
        public GameObject TerminalArch;
        public GameObject WoodPlank;
        public GameObject TerminalWall;

        private List<GameObject> _terminals;
        private int _ticker;
        private int _cursor;

        // Use this for initialization
        public void Start()
        {
            _terminals = new List<GameObject>();
            DefineProductions();
            InstanceProduction("Bldg");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _terminals[_cursor%_terminals.Count].GetComponentInChildren<MeshRenderer>().enabled = !_terminals[_cursor%_terminals.Count].GetComponentInChildren<MeshRenderer>().enabled;
                ++_cursor;
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                _cursor = Math.Max(0, _cursor - 1);
                _terminals[_cursor % _terminals.Count].GetComponentInChildren<MeshRenderer>().enabled = !_terminals[_cursor % _terminals.Count].GetComponentInChildren<MeshRenderer>().enabled;
            }
        }

        public void ParseProductions(string input)
        {
            //todo: implement a CSS-like grammar that is more user-friendly than "DefineProductions" below.
            throw new NotImplementedException();
        }

        public void DefineProductions()
        {
            DefineTestBldg();
        }

        private void DefineTestBldg()
        {
            #region terminals
            var boxTerm = new TerminalProduction(UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube));
            var wallTerm = new TerminalProduction(TerminalWall).Move(new Vector3(0, -16, 0));
            var archTerm = new TerminalProduction(TerminalArch).Move(new Vector3(0, -16, 0));   // this particukar .obj isn't centered at 0,0,0 so we shift it's coords as needed
            var plankTerm = new TerminalProduction(WoodPlank).Rotate(Quaternion.AngleAxis(180f, Vector3.right));
            #endregion

            #region outers
            var bldg = new RegistrarProduction("Bldg", new CubeProductionScope(new Bounds(new Vector3(0, 16, 0), new Vector3(32, 32, 32)), Quaternion.identity))
                                                           .Rotate(Quaternion.AngleAxis(0, Vector3.up))
                                                           .Scale(new ScaleProduction { IsAbsolute = new[] { false, false, false }, Magnitude = new[] { 1f, 1f, 1f } });
            ProductionDictionary.Add(bldg.Name, bldg);

            var outerWall = new RegistrarProduction("TestWall", bldg.Scope).Terminate(archTerm);
            var floorRepeatX = new Func<RegistrarProduction, RepeatProduction>(parentProd =>
                new RepeatProduction(parentProd)
                    {
                        Name = "",
                        IsOccluder = false,
                        Magnitude = 5f,
                        RemainderMode = RepeatRemainderMode.MergeFirstAndLast,
                        RepetitionAxis = ProductionAxis.X,
                        ReplicandProduction = parent => new RegistrarProduction("", parent.Scope).Terminate(plankTerm)
                    });
            var floorRepeatZ = new Func<RegistrarProduction, RepeatProduction>(parentProd => 
                new RepeatProduction(parentProd)
                {
                    Name = "",
                    IsOccluder = false,
                    Magnitude = 1.5f,
                    RemainderMode = RepeatRemainderMode.MergeFirstAndLast,
                    RepetitionAxis = ProductionAxis.Z,
                    ReplicandProduction = parent => new RegistrarProduction("", parent.Scope).Repeat(floorRepeatX)
                });

            bldg.Select(new SelectProduction(bldg)
            {
                Faces = new[] { ScopeFace.Front, ScopeFace.Back, ScopeFace.Left, ScopeFace.Right },
                FaceBreadth = 5f,
                IsAbsolute = true,
                Name = "TestWalls",
                SelectandProduction = outerWall
            });

            //bldg.Select(parentProd => new SelectProduction(parentProd)
            //{
            //    Faces = new[] { ScopeFace.Bottom },
            //    FaceBreadth = 1f,
            //    IsAbsolute = true,
            //    Name = "TestFloor",
            //    SelectandProduction = new RegistrarProduction("", parentProd.Scope).Repeat(floorRepeatZ)
            //});
            #endregion

            #region inners

            var wallTest = new Func<RegistrarProduction, SwitchProduction>(parentProd => 
                new SwitchProduction(parentProd)
                    {
                        Cases = new List<CaseProduction> {
                            new CaseProduction
                            {
                                Case = parent => parent.Scope.IsOccluded(),
                                Body = parent => new RegistrarProduction("", parent.Scope).Terminate(wallTerm)
                            },
                            new CaseProduction {
                                    Case = delegate(RegistrarProduction parent)
                                                {
                                                    var occlusionProbe = new RegistrarProduction("occlusionProbe", parent.Scope) { IsOccluder = true };
                                                    occlusionProbe.Scale(new ScaleProduction {
                                                        IsAbsolute = new[] { true, false, true }, 
                                                                        Magnitude = new[] {                             // make the walls a bit thicker--but not as long.
                                                                            parent.Scope.Bounds.size.x + .01f, // .51f   //so walls that meet at the corner won't mark each other as occluded;
                                                                            1f,                                         // only walls with adjacent FACES occlude each other
                                                                            parent.Scope.Bounds.size.z - 1.04f //2.99f    // can we generalize/simplify this trick?
                                                                        }
                                                                    });
                                                    ModuleProduction.Occluders.Add(occlusionProbe.Scope);
                                                    return true;
                                                    return !occlusionProbe.Scope.IsOccluded();
                                                },
                                    Body = parent => //RegistrarProduction.Empty
                                    new RegistrarProduction("wallOccluder", parent.Scope) { IsOccluder = false }
                                                    .Scale(new ScaleProduction
                                                    {
                                                        IsAbsolute = new[] { true, false, true },
                                                        Magnitude = new[] {                             // make the walls a bit thicker--but not as long.
                                                                            parent.Scope.Bounds.size.x + .01f, // .51f   //so walls that meet at the corner won't mark each other as occluded;
                                                                            1f,                                         // only walls with adjacent FACES occlude each other
                                                                            parent.Scope.Bounds.size.z - 1.02f //2.99f    // can we generalize/simplify this trick?
                                                                        }
                                                    }).Terminate(boxTerm)
                            }
                        },
                        DefaultProduction = RegistrarProduction.Empty,
                        Name = "wallTest"
                    });

            var roomTermCase = new CaseProduction {
                Case = parent => (UnityEngine.Random.Range(0, 5) > 3.5) || parent.Scope.Bounds.size.x < 8 && parent.Scope.Bounds.size.z < 8,
                Body = parent => new SelectProduction(parent)
                {
                                                      Faces = new[] { ScopeFace.Front, ScopeFace.Left, ScopeFace.Back, ScopeFace.Right },
                                                      FaceBreadth = .5f,
                                                      IsAbsolute = true,
                                                      SelectandProduction = new RegistrarProduction("", parent.Scope).Switch(wallTest)
                                                  }
                                   };
            var roomDivideX = new CaseProduction {
                Case = parent1 => parent1.Scope.Bounds.size.x >= parent1.Scope.Bounds.size.z,
                Body = parent1 => new DivideProduction(parent1)
                {
                    DivisionAxis = ProductionAxis.X,
                    Divisors = new [] {
                                        new DivisorProduction {DividendProduction = parent2 => (RegistrarProduction)parent2.Clone(), Magnitude = .5f, IsAbsolute = false},
                                        new DivisorProduction {DividendProduction = parent2 => (RegistrarProduction)parent2.Clone(), Magnitude = .5f, IsAbsolute = false}
                                    }
                }
            };
            var roomDivideZ = new CaseProduction {
                Case = parent1 => parent1.Scope.Bounds.size.x < parent1.Scope.Bounds.size.z,
                Body = parent1 => new DivideProduction(parent1) {
                    DivisionAxis = ProductionAxis.Z,
                    Divisors = new[] {
                                    new DivisorProduction {DividendProduction = parent2 => (RegistrarProduction)parent2.Clone(), Magnitude = .5f, IsAbsolute = false},
                                    new DivisorProduction {DividendProduction = parent2 => (RegistrarProduction)parent2.Clone(), Magnitude = .5f, IsAbsolute = false}
                                }
                }
            };

            var firstFloorRooms = new RegistrarProduction("FirstFloorRooms", bldg.Scope)
                .Scale(new ScaleProduction { Magnitude = new[] { 1f, 0.1f, 1f }, IsAbsolute = new[] { false, false, false } })
                .Switch(parentProd => new SwitchProduction(parentProd)
                {
                    Cases = new List<CaseProduction>
                    {
                        roomTermCase,
                        roomDivideX,
                        roomDivideZ
                    },
                    DefaultProduction = RegistrarProduction.Empty
                });
            bldg.BoxSplit(firstFloorRooms);
            #endregion
        }

        /// <summary>
        /// ProductionDictionary stores the production rules that we want to serve as a basis for generation.
        /// IE, it contains "house" but not "houseFirstFloor" because we don't want "houseFirstFloor" to be generated without also generating "houseRoof."
        /// </summary>
        /// <param name="productionName"></param>
        public void InstanceProduction(string productionName)
        {
            ModuleProduction.Begin(ProductionDictionary[productionName]);
            ModuleProduction.Expand();
        }
    }
}