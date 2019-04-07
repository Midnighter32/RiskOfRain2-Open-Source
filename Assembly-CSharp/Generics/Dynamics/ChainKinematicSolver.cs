using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006D2 RID: 1746
	public static class ChainKinematicSolver
	{
		// Token: 0x06002728 RID: 10024 RVA: 0x000B5390 File Offset: 0x000B3590
		public static void Process(Core.KinematicChain chain)
		{
			if (!chain.initiated)
			{
				chain.InitiateJoints();
			}
			chain.MapVirtualJoints();
			ChainKinematicSolver.FollowParent(chain);
			ChainKinematicSolver.MapSolverOutput(chain);
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x000B53B4 File Offset: 0x000B35B4
		private static void FollowParent(Core.KinematicChain chain)
		{
			for (int i = 1; i < chain.joints.Count; i++)
			{
				Vector3 a = chain.joints[i].pos - chain.prevPos[i];
				float d = 6f * Mathf.Pow((float)(chain.joints.Count * i), 2f) - 4f * (float)chain.joints.Count * Mathf.Pow((float)i, 3f) + Mathf.Pow((float)i, 4f);
				Vector3 b = chain.gravity * d / (chain.momentOfInteria * (float)chain.joints.Count * 12f);
				float time = (float)i / ((float)chain.joints.Count - 1f);
				float num = chain.solverFallOff.Evaluate(time) * chain.weight;
				float d2 = (chain.torsionDamping != 0f) ? (Mathf.Sin(num * 1.5707964f) * Mathf.Exp(-chain.torsionDamping * num)) : 1f;
				chain.prevPos[i] = chain.joints[i].pos;
				chain.joints[i].pos += a * d2;
				chain.joints[i].pos += b;
				chain.prevPos[0] = chain.joints[0].pos;
				chain.joints[0].pos = chain.joints[0].joint.position;
				float length = chain.joints[i - 1].length;
				Vector3 a2 = chain.joints[i].joint.position + (chain.joints[i - 1].pos - chain.joints[i - 1].joint.position) - chain.joints[i].pos;
				chain.joints[i].pos += a2 * (1f - chain.weight);
				Vector3 a3 = a2 * (2f - chain.weight);
				float magnitude = a3.magnitude;
				float num2 = length * -chain.stiffness * num;
				Vector3 b2 = a3 * (Mathf.Max(magnitude - num2, 0f) / Mathf.Max(1f, magnitude));
				chain.joints[i].pos += b2;
				Vector3 a4 = chain.joints[i - 1].pos - chain.joints[i].pos;
				float magnitude2 = a4.magnitude;
				chain.joints[i].pos += a4 * ((magnitude2 - length) / Mathf.Max(magnitude2, 1f));
				chain.joints[i].pos = Vector3.Lerp(chain.joints[i].joint.position, chain.joints[i].pos, chain.joints[i].weight);
			}
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x000B5748 File Offset: 0x000B3948
		private static void MapSolverOutput(Core.KinematicChain chain)
		{
			for (int i = 1; i < chain.joints.Count; i++)
			{
				Vector3 target = GenericMath.TransformVector(chain.joints[i - 1].localAxis, chain.joints[i - 1].rot);
				Quaternion qA = GenericMath.RotateFromTo(chain.joints[i].pos - chain.joints[i - 1].pos, target);
				chain.joints[i - 1].rot = GenericMath.ApplyQuaternion(qA, chain.joints[i - 1].rot);
				chain.joints[i - 1].ApplyVirtualMap(false, true);
				chain.joints[i].ApplyVirtualMap(true, false);
				chain.joints[i - 1].ApplyRestrictions();
				chain.joints[i].ApplyRestrictions();
			}
		}
	}
}
