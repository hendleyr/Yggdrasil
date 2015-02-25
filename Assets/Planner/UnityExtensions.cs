using UnityEngine;

namespace Assets.Planner
{
    public static class UnityExtensions
    {
        public static Quaternion QuaternionFromMatrix(this Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }
    }
}
