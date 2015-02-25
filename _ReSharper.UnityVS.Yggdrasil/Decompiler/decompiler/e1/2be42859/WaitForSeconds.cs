// Type: UnityEngine.WaitForSeconds
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Monte Carlo\Documents\Yggdrasil\Library\UnityAssemblies\UnityEngine.dll

using System.Runtime.InteropServices;

namespace UnityEngine
{
  [StructLayout(LayoutKind.Sequential)]
  public sealed class WaitForSeconds : YieldInstruction
  {
    internal float m_Seconds;

    public WaitForSeconds(float seconds)
    {
      this.m_Seconds = seconds;
    }
  }
}
