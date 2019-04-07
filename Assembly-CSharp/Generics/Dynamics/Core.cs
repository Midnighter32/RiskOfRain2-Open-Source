using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006D3 RID: 1747
	public class Core
	{
		// Token: 0x020006D4 RID: 1748
		public enum Solvers
		{
			// Token: 0x04002958 RID: 10584
			CyclicDescend,
			// Token: 0x04002959 RID: 10585
			FastReach
		}

		// Token: 0x020006D5 RID: 1749
		[Serializable]
		public class Joint
		{
			// Token: 0x0600272C RID: 10028 RVA: 0x000B5847 File Offset: 0x000B3A47
			public void MapVirtual()
			{
				this.pos = this.joint.position;
				this.rot = this.joint.rotation;
			}

			// Token: 0x0600272D RID: 10029 RVA: 0x000B586B File Offset: 0x000B3A6B
			public void ApplyVirtualMap(bool applyPos, bool applyRot)
			{
				if (applyPos)
				{
					this.joint.position = this.pos;
				}
				if (applyRot)
				{
					this.joint.rotation = this.rot;
				}
			}

			// Token: 0x0600272E RID: 10030 RVA: 0x000B5898 File Offset: 0x000B3A98
			public Quaternion ApplyRestrictions()
			{
				switch (this.motionFreedom)
				{
				case Core.Joint.MotionLimit.Full:
					return this.joint.localRotation;
				case Core.Joint.MotionLimit.FullRestricted:
					return this.TwistAndSwing();
				case Core.Joint.MotionLimit.SingleDegree:
					return this.SingleDegree();
				default:
					return this.joint.localRotation;
				}
			}

			// Token: 0x0600272F RID: 10031 RVA: 0x000B58E8 File Offset: 0x000B3AE8
			private Quaternion TwistAndSwing()
			{
				object obj = new Func<Quaternion, float, Quaternion>(delegate(Quaternion q, float x)
				{
					if (x == 0f)
					{
						return Quaternion.identity;
					}
					float num2 = GenericMath.QuaternionAngle(Quaternion.identity, q);
					float t = Mathf.Clamp01(x / num2);
					return Quaternion.Slerp(Quaternion.identity, q, t);
				});
				Func<float, float> func = (float x) => x * x;
				Quaternion quaternion = GenericMath.RotateFromTo(GenericMath.TransformVector(this.axis, this.joint.localRotation), this.axis);
				object obj2 = obj;
				Quaternion qB = obj2(quaternion, this.maxAngle);
				Quaternion quaternion2 = GenericMath.ApplyQuaternion(Quaternion.Inverse(quaternion), this.joint.localRotation);
				float num = Mathf.Sqrt(func(quaternion2.w) + func(quaternion2.x) + func(quaternion2.y) + func(quaternion2.z));
				float w = quaternion2.w / num;
				float x2 = quaternion2.x / num;
				float y = quaternion2.y / num;
				float z = quaternion2.z / num;
				Quaternion qA = obj2(new Quaternion(x2, y, z, w), this.maxTwist);
				this.joint.localRotation = GenericMath.ApplyQuaternion(qA, qB);
				return this.joint.localRotation;
			}

			// Token: 0x06002730 RID: 10032 RVA: 0x000B5A1C File Offset: 0x000B3C1C
			private Quaternion SingleDegree()
			{
				Vector3 target = GenericMath.TransformVector(this.axis, this.joint.transform.localRotation);
				float num;
				Vector3 rhs;
				GenericMath.QuaternionToAngleAxis(GenericMath.ApplyQuaternion(GenericMath.RotateFromTo(this.axis, target), this.joint.localRotation), out num, out rhs);
				float x = this.hingLimit.x;
				float y = this.hingLimit.y;
				float num2 = Vector3.Dot(this.axis, rhs);
				num = GenericMath.Clamp(num * num2, x, y);
				this.joint.localRotation = GenericMath.QuaternionFromAngleAxis(num, this.axis);
				return this.joint.localRotation;
			}

			// Token: 0x0400295A RID: 10586
			public Core.Joint.MotionLimit motionFreedom;

			// Token: 0x0400295B RID: 10587
			public Transform joint;

			// Token: 0x0400295C RID: 10588
			[Range(0f, 1f)]
			public float weight = 1f;

			// Token: 0x0400295D RID: 10589
			public float length;

			// Token: 0x0400295E RID: 10590
			public Vector3 pos;

			// Token: 0x0400295F RID: 10591
			public Quaternion rot;

			// Token: 0x04002960 RID: 10592
			public Vector3 localAxis = Vector3.up;

			// Token: 0x04002961 RID: 10593
			public Vector3 axis = Vector3.right;

			// Token: 0x04002962 RID: 10594
			public Vector2 hingLimit = Vector2.one * 180f;

			// Token: 0x04002963 RID: 10595
			public float maxAngle = 180f;

			// Token: 0x04002964 RID: 10596
			public float maxTwist = 180f;

			// Token: 0x020006D6 RID: 1750
			public enum MotionLimit
			{
				// Token: 0x04002966 RID: 10598
				Full,
				// Token: 0x04002967 RID: 10599
				FullRestricted,
				// Token: 0x04002968 RID: 10600
				SingleDegree
			}
		}

		// Token: 0x020006D8 RID: 1752
		[Serializable]
		public class Chain
		{
			// Token: 0x17000360 RID: 864
			// (get) Token: 0x06002736 RID: 10038 RVA: 0x000B5B71 File Offset: 0x000B3D71
			// (set) Token: 0x06002737 RID: 10039 RVA: 0x000B5B79 File Offset: 0x000B3D79
			public bool initiated { get; private set; }

			// Token: 0x17000361 RID: 865
			// (get) Token: 0x06002738 RID: 10040 RVA: 0x000B5B82 File Offset: 0x000B3D82
			// (set) Token: 0x06002739 RID: 10041 RVA: 0x000B5B8A File Offset: 0x000B3D8A
			public float chainLength { get; private set; }

			// Token: 0x0600273A RID: 10042 RVA: 0x000B5B93 File Offset: 0x000B3D93
			public Chain()
			{
				this.iterations = 2;
			}

			// Token: 0x0600273B RID: 10043 RVA: 0x000B5BB0 File Offset: 0x000B3DB0
			public void InitiateJoints()
			{
				this.MapVirtualJoints();
				for (int i = 0; i < this.joints.Count - 1; i++)
				{
					this.joints[i].localAxis = GenericMath.GetLocalAxisToTarget(this.joints[i].joint, this.joints[i + 1].joint.position);
					this.joints[i].length = Vector3.Distance(this.joints[i].joint.position, this.joints[i + 1].joint.position);
					this.chainLength += this.joints[i].length;
				}
				this.joints[this.joints.Count - 1].localAxis = GenericMath.GetLocalAxisToTarget(this.joints[0].joint, this.joints[this.joints.Count - 1].joint.position);
				this.SetIKTarget(this.GetVirtualEE());
				this.initiated = true;
			}

			// Token: 0x0600273C RID: 10044 RVA: 0x000B5CEB File Offset: 0x000B3EEB
			public Transform GetEndEffector()
			{
				if (this.joints.Count <= 0)
				{
					return null;
				}
				return this.joints[this.joints.Count - 1].joint;
			}

			// Token: 0x0600273D RID: 10045 RVA: 0x000B5D1A File Offset: 0x000B3F1A
			public Vector3 GetVirtualEE()
			{
				if (this.joints.Count <= 0)
				{
					return Vector3.zero;
				}
				return this.joints[this.joints.Count - 1].pos;
			}

			// Token: 0x0600273E RID: 10046 RVA: 0x000B5D4D File Offset: 0x000B3F4D
			public Vector3 GetIKtarget()
			{
				if (!this.target)
				{
					return this.IKpos;
				}
				return this.target.position;
			}

			// Token: 0x0600273F RID: 10047 RVA: 0x000B5D6E File Offset: 0x000B3F6E
			public Vector3 SetIKTarget(Vector3 target)
			{
				this.IKpos = (this.target ? this.target.position : target);
				return this.IKpos;
			}

			// Token: 0x06002740 RID: 10048 RVA: 0x000B5D98 File Offset: 0x000B3F98
			public Quaternion SetEERotation(Quaternion target)
			{
				this.joints[this.joints.Count - 1].joint.rotation = target;
				return target;
			}

			// Token: 0x06002741 RID: 10049 RVA: 0x000B5DCC File Offset: 0x000B3FCC
			public void MapVirtualJoints()
			{
				for (int i = 0; i < this.joints.Count; i++)
				{
					this.joints[i].MapVirtual();
				}
			}

			// Token: 0x0400296C RID: 10604
			public Transform target;

			// Token: 0x0400296D RID: 10605
			[Range(0f, 1f)]
			public float weight;

			// Token: 0x0400296E RID: 10606
			public int iterations;

			// Token: 0x0400296F RID: 10607
			public List<Core.Joint> joints = new List<Core.Joint>();

			// Token: 0x04002970 RID: 10608
			private Vector3 IKpos;
		}

		// Token: 0x020006D9 RID: 1753
		[Serializable]
		public class KinematicChain
		{
			// Token: 0x17000362 RID: 866
			// (get) Token: 0x06002742 RID: 10050 RVA: 0x000B5E00 File Offset: 0x000B4000
			// (set) Token: 0x06002743 RID: 10051 RVA: 0x000B5E08 File Offset: 0x000B4008
			public bool initiated { get; private set; }

			// Token: 0x17000363 RID: 867
			// (get) Token: 0x06002744 RID: 10052 RVA: 0x000B5E11 File Offset: 0x000B4011
			// (set) Token: 0x06002745 RID: 10053 RVA: 0x000B5E19 File Offset: 0x000B4019
			public Quaternion[] initLocalRot { get; private set; }

			// Token: 0x17000364 RID: 868
			// (get) Token: 0x06002746 RID: 10054 RVA: 0x000B5E22 File Offset: 0x000B4022
			// (set) Token: 0x06002747 RID: 10055 RVA: 0x000B5E2A File Offset: 0x000B402A
			public Vector3[] prevPos { get; private set; }

			// Token: 0x06002748 RID: 10056 RVA: 0x000B5E34 File Offset: 0x000B4034
			public KinematicChain()
			{
				this.momentOfInteria = 1000f;
				this.stiffness = -0.1f;
			}

			// Token: 0x06002749 RID: 10057 RVA: 0x000B5E94 File Offset: 0x000B4094
			public void InitiateJoints()
			{
				this.initLocalRot = new Quaternion[this.joints.Count];
				this.prevPos = new Vector3[this.joints.Count];
				int i;
				for (i = 0; i < this.joints.Count - 1; i++)
				{
					this.joints[i].MapVirtual();
					this.joints[i].localAxis = GenericMath.GetLocalAxisToTarget(this.joints[i].joint, this.joints[i + 1].joint.position);
					this.joints[i].length = Vector3.Distance(this.joints[i].joint.position, this.joints[i + 1].joint.position);
					this.initLocalRot[i] = this.joints[i].joint.localRotation;
					this.prevPos[i] = this.joints[i].joint.position;
				}
				this.joints[i].MapVirtual();
				this.initLocalRot[i] = this.joints[i].joint.localRotation;
				this.prevPos[i] = this.joints[i].joint.position;
				this.joints[i].localAxis = GenericMath.GetLocalAxisToTarget(this.joints[0].joint, this.joints[i].joint.position);
				this.initiated = true;
			}

			// Token: 0x0600274A RID: 10058 RVA: 0x000B6060 File Offset: 0x000B4260
			public void MapVirtualJoints()
			{
				for (int i = 0; i < this.joints.Count; i++)
				{
					this.joints[i].joint.localRotation = this.initLocalRot[i];
				}
			}

			// Token: 0x04002973 RID: 10611
			[Range(0f, 1f)]
			[Header("Chain")]
			public float weight = 1f;

			// Token: 0x04002974 RID: 10612
			public List<Core.Joint> joints = new List<Core.Joint>();

			// Token: 0x04002975 RID: 10613
			[Header("Interafce")]
			public AnimationCurve solverFallOff;

			// Token: 0x04002976 RID: 10614
			public Vector3 gravity = Vector3.down;

			// Token: 0x04002977 RID: 10615
			public float momentOfInteria = 1000f;

			// Token: 0x04002978 RID: 10616
			public float stiffness = -0.1f;

			// Token: 0x04002979 RID: 10617
			public float torsionDamping;
		}
	}
}
