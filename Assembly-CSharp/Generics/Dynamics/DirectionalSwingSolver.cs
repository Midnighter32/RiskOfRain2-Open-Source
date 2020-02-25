using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x02000926 RID: 2342
	public static class DirectionalSwingSolver
	{
		// Token: 0x06003496 RID: 13462 RVA: 0x000E5637 File Offset: 0x000E3837
		public static void Process(Core.Chain chain, Vector3 lookAtAxis)
		{
			DirectionalSwingSolver.Process(chain, lookAtAxis, chain.GetEndEffector());
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x000E5648 File Offset: 0x000E3848
		public static void Process(Core.Chain chain, Vector3 lookAtAxis, Transform virtualEndEffector)
		{
			Transform endEffector = virtualEndEffector ?? chain.GetEndEffector();
			for (int i = 0; i < chain.iterations; i++)
			{
				DirectionalSwingSolver.Solve(chain, endEffector, lookAtAxis);
			}
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x000E567C File Offset: 0x000E387C
		private static void Solve(Core.Chain chain, Transform endEffector, Vector3 LookAtAxis)
		{
			for (int i = 0; i < chain.joints.Count; i++)
			{
				Vector3 target = GenericMath.TransformVector(LookAtAxis, endEffector.rotation);
				Quaternion b = GenericMath.RotateFromTo(chain.GetIKtarget() - endEffector.position, target);
				Quaternion qA = Quaternion.Lerp(Quaternion.identity, b, chain.weight * chain.joints[i].weight);
				chain.joints[i].joint.rotation = GenericMath.ApplyQuaternion(qA, chain.joints[i].joint.rotation);
				chain.joints[i].ApplyRestrictions();
			}
		}
	}
}
