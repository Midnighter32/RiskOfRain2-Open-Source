using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006DC RID: 1756
	public static class FastReachSolver
	{
		// Token: 0x0600274F RID: 10063 RVA: 0x000B62EC File Offset: 0x000B44EC
		public static bool Process(Core.Chain chain)
		{
			if (chain.joints.Count <= 0)
			{
				return false;
			}
			if (!chain.initiated)
			{
				chain.InitiateJoints();
			}
			chain.MapVirtualJoints();
			for (int i = 0; i < chain.iterations; i++)
			{
				FastReachSolver.SolveInward(chain);
				FastReachSolver.SolveOutward(chain);
			}
			FastReachSolver.MapSolverOutput(chain);
			return true;
		}

		// Token: 0x06002750 RID: 10064 RVA: 0x000B6344 File Offset: 0x000B4544
		public static void SolveInward(Core.Chain chain)
		{
			int count = chain.joints.Count;
			chain.joints[count - 1].pos = Vector3.Lerp(chain.GetVirtualEE(), chain.GetIKtarget(), chain.weight);
			for (int i = count - 2; i >= 0; i--)
			{
				Vector3 pos = chain.joints[i + 1].pos;
				Vector3 vector = chain.joints[i].pos - pos;
				vector.Normalize();
				vector *= Vector3.Distance(chain.joints[i + 1].joint.position, chain.joints[i].joint.position);
				chain.joints[i].pos = pos + vector;
			}
		}

		// Token: 0x06002751 RID: 10065 RVA: 0x000B6420 File Offset: 0x000B4620
		public static void SolveOutward(Core.Chain chain)
		{
			chain.joints[0].pos = chain.joints[0].joint.position;
			for (int i = 1; i < chain.joints.Count; i++)
			{
				Vector3 pos = chain.joints[i - 1].pos;
				Vector3 vector = chain.joints[i].pos - pos;
				vector.Normalize();
				vector *= Vector3.Distance(chain.joints[i - 1].joint.position, chain.joints[i].joint.position);
				chain.joints[i].pos = pos + vector;
			}
		}

		// Token: 0x06002752 RID: 10066 RVA: 0x000B64F8 File Offset: 0x000B46F8
		public static void MapSolverOutput(Core.Chain chain)
		{
			for (int i = 0; i < chain.joints.Count - 1; i++)
			{
				Vector3 source = chain.joints[i + 1].pos - chain.joints[i].pos;
				Vector3 target = GenericMath.TransformVector(chain.joints[i].localAxis, chain.joints[i].rot);
				Quaternion qA = GenericMath.RotateFromTo(source, target);
				chain.joints[i].rot = GenericMath.ApplyQuaternion(qA, chain.joints[i].rot);
				chain.joints[i].ApplyVirtualMap(true, true);
			}
		}
	}
}
