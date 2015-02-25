using System.Collections.Generic;
using Assets.Planner.Scopes;
using UnityEngine;

namespace Assets.Planner.Productions
{
    public class TerminalProduction : IRenderableProduction
    {
        public string Name { get; set; }
        public bool IsOccluder { get; set; }
        public GameObject Terminal { get; set; }
        public ProductionScope Scope { get; set; }
        public RegistrarProduction ParentProduction { get; set; }

        public TerminalProduction(GameObject asset)
        {
            Terminal = asset;
            Scope = new CubeProductionScope(new Bounds(Vector3.zero, Vector3.zero), Quaternion.identity);
        }

        public IList<IRenderableProduction> Expand()
        {
            if (ParentProduction == null || ParentProduction.Scope == null)
            {
                Debug.Log(string.Format("Found a null parent production while trying to expand terminal: {0}", Name));
            }
            var terminal = (GameObject)Object.Instantiate(Terminal, Vector3.zero, Quaternion.identity);
            
            var terminalBounds = terminal.GetComponentInChildren<MeshRenderer>().bounds;
            terminal.GetComponentInChildren<MeshRenderer>().transform.localScale = new Vector3(
                1 / (terminalBounds.size.x / ParentProduction.Scope.Bounds.size.x),
                1 / (terminalBounds.size.y / ParentProduction.Scope.Bounds.size.y),
                1 / (terminalBounds.size.z / ParentProduction.Scope.Bounds.size.z)
            );
            terminal.GetComponentInChildren<MeshRenderer>().transform.position = Scope.Bounds.center;

            terminal.transform.rotation = ParentProduction.Scope.Rotation * Scope.Rotation;
            terminal.transform.position = ParentProduction.Scope.Bounds.center;
            
            return new List<IRenderableProduction>();
        }

        public IRenderableProduction Clone()
        {
            var clone = new TerminalProduction(Terminal)
                            {
                                Name = Name,
                                Scope = Scope.Clone(),
                                IsOccluder = IsOccluder,
                                ParentProduction = ParentProduction
                            };
            return clone;
        }

        public TerminalProduction Rotate(Quaternion quaternion)
        {
            Scope.Rotation = Scope.Rotation * quaternion;
            return this;
        }

        public TerminalProduction Move(Vector3 move)
        {
            Scope.Bounds = new Bounds(Scope.Bounds.center + move, Scope.Bounds.size);
            return this;
        }
    }
}