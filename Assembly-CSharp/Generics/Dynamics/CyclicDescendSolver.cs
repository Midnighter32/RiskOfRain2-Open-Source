using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x02000925 RID: 2341
	public static class CyclicDescendSolver
	{
		// Token: 0x06003495 RID: 13461 RVA: 0x000E54F0 File Offset: 0x000E36F0
		public static bool Process(Core.Chain chain)
		{
			if (chain.joints.Count <= 0)
			{
				return false;
			}
			chain.MapVirtualJoints();
			for (int i = 0; i < chain.iterations; i++)
			{
				for (int j = chain.joints.Count - 1; j >= 0; j--)
				{
					float t = chain.weight * chain.joints[j].weight;
					Vector3 source = chain.GetIKtarget() - chain.joints[j].joint.position;
					Vector3 target = chain.joints[chain.joints.Count - 1].joint.position - chain.joints[j].joint.position;
					Quaternion rotation = chain.joints[j].joint.rotation;
					Quaternion qA = Quaternion.Lerp(Quaternion.identity, GenericMath.RotateFromTo(source, target), t);
					chain.joints[j].rot = Quaternion.Lerp(rotation, GenericMath.ApplyQuaternion(qA, rotation), t);
					chain.joints[j].ApplyVirtualMap(false, true);
					chain.joints[j].ApplyRestrictions();
				}
			}
			return true;
		}
	}
}
