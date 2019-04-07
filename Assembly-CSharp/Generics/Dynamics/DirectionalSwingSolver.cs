using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006DB RID: 1755
	public static class DirectionalSwingSolver
	{
		// Token: 0x0600274C RID: 10060 RVA: 0x000B61EF File Offset: 0x000B43EF
		public static void Process(Core.Chain chain, Vector3 lookAtAxis)
		{
			DirectionalSwingSolver.Process(chain, lookAtAxis, chain.GetEndEffector());
		}

		// Token: 0x0600274D RID: 10061 RVA: 0x000B6200 File Offset: 0x000B4400
		public static void Process(Core.Chain chain, Vector3 lookAtAxis, Transform virtualEndEffector)
		{
			Transform endEffector = virtualEndEffector ?? chain.GetEndEffector();
			for (int i = 0; i < chain.iterations; i++)
			{
				DirectionalSwingSolver.Solve(chain, endEffector, lookAtAxis);
			}
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x000B6234 File Offset: 0x000B4434
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
