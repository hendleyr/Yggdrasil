namespace Assets.Planner.Productions
{
    public class ScaleProduction
    {
        public bool[] IsAbsolute { get; set; }
        public float[] Magnitude { get; set; }
        
        public ScaleProduction() {}
        public ScaleProduction(string x, string y, string z)
        {
            IsAbsolute = new[] { !x.Contains("%"), !y.Contains("%"), !z.Contains("%") };
            Magnitude = new[] {
                IsAbsolute[0] 
                    ? float.Parse(x)
                    : float.Parse(x.Replace("%", string.Empty)) / 100f,
                IsAbsolute[1]
                    ? float.Parse(y)
                    : float.Parse(y.Replace("%", string.Empty)) / 100f,
                IsAbsolute[2]
                    ? float.Parse(z)
                    : float.Parse(z.Replace("%", string.Empty)) / 100f
            };
        }
    }
}
