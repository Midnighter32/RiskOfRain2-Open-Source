using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x0200091E RID: 2334
	public class Core
	{
		// Token: 0x0200091F RID: 2335
		public enum Solvers
		{
			// Token: 0x040033F1 RID: 13297
			CyclicDescend,
			// Token: 0x040033F2 RID: 13298
			FastReach
		}

		// Token: 0x02000920 RID: 2336
		[Serializable]
		public class Joint
		{
			// Token: 0x06003476 RID: 13430 RVA: 0x000E4C8F File Offset: 0x000E2E8F
			public void MapVirtual()
			{
				this.pos = this.joint.position;
				this.rot = this.joint.rotation;
			}

			// Token: 0x06003477 RID: 13431 RVA: 0x000E4CB3 File Offset: 0x000E2EB3
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

			// Token: 0x06003478 RID: 13432 RVA: 0x000E4CE0 File Offset: 0x000E2EE0
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

			// Token: 0x06003479 RID: 13433 RVA: 0x000E4D30 File Offset: 0x000E2F30
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

			// Token: 0x0600347A RID: 13434 RVA: 0x000E4E64 File Offset: 0x000E3064
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

			// Token: 0x040033F3 RID: 13299
			public Core.Joint.MotionLimit motionFreedom;

			// Token: 0x040033F4 RID: 13300
			public Transform joint;

			// Token: 0x040033F5 RID: 13301
			[Range(0f, 1f)]
			public float weight = 1f;

			// Token: 0x040033F6 RID: 13302
			public float length;

			// Token: 0x040033F7 RID: 13303
			public Vector3 pos;

			// Token: 0x040033F8 RID: 13304
			public Quaternion rot;

			// Token: 0x040033F9 RID: 13305
			public Vector3 localAxis = Vector3.up;

			// Token: 0x040033FA RID: 13306
			public Vector3 axis = Vector3.right;

			// Token: 0x040033FB RID: 13307
			public Vector2 hingLimit = Vector2.one * 180f;

			// Token: 0x040033FC RID: 13308
			public float maxAngle = 180f;

			// Token: 0x040033FD RID: 13309
			public float maxTwist = 180f;

			// Token: 0x02000921 RID: 2337
			public enum MotionLimit
			{
				// Token: 0x040033FF RID: 13311
				Full,
				// Token: 0x04003400 RID: 13312
				FullRestricted,
				// Token: 0x04003401 RID: 13313
				SingleDegree
			}
		}

		// Token: 0x02000923 RID: 2339
		[Serializable]
		public class Chain
		{
			// Token: 0x1700049D RID: 1181
			// (get) Token: 0x06003480 RID: 13440 RVA: 0x000E4FB9 File Offset: 0x000E31B9
			// (set) Token: 0x06003481 RID: 13441 RVA: 0x000E4FC1 File Offset: 0x000E31C1
			public bool initiated { get; private set; }

			// Token: 0x1700049E RID: 1182
			// (get) Token: 0x06003482 RID: 13442 RVA: 0x000E4FCA File Offset: 0x000E31CA
			// (set) Token: 0x06003483 RID: 13443 RVA: 0x000E4FD2 File Offset: 0x000E31D2
			public float chainLength { get; private set; }

			// Token: 0x06003484 RID: 13444 RVA: 0x000E4FDB File Offset: 0x000E31DB
			public Chain()
			{
				this.iterations = 2;
			}

			// Token: 0x06003485 RID: 13445 RVA: 0x000E4FF8 File Offset: 0x000E31F8
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

			// Token: 0x06003486 RID: 13446 RVA: 0x000E5133 File Offset: 0x000E3333
			public Transform GetEndEffector()
			{
				if (this.joints.Count <= 0)
				{
					return null;
				}
				return this.joints[this.joints.Count - 1].joint;
			}

			// Token: 0x06003487 RID: 13447 RVA: 0x000E5162 File Offset: 0x000E3362
			public Vector3 GetVirtualEE()
			{
				if (this.joints.Count <= 0)
				{
					return Vector3.zero;
				}
				return this.joints[this.joints.Count - 1].pos;
			}

			// Token: 0x06003488 RID: 13448 RVA: 0x000E5195 File Offset: 0x000E3395
			public Vector3 GetIKtarget()
			{
				if (!this.target)
				{
					return this.IKpos;
				}
				return this.target.position;
			}

			// Token: 0x06003489 RID: 13449 RVA: 0x000E51B6 File Offset: 0x000E33B6
			public Vector3 SetIKTarget(Vector3 target)
			{
				this.IKpos = (this.target ? this.target.position : target);
				return this.IKpos;
			}

			// Token: 0x0600348A RID: 13450 RVA: 0x000E51E0 File Offset: 0x000E33E0
			public Quaternion SetEERotation(Quaternion target)
			{
				this.joints[this.joints.Count - 1].joint.rotation = target;
				return target;
			}

			// Token: 0x0600348B RID: 13451 RVA: 0x000E5214 File Offset: 0x000E3414
			public void MapVirtualJoints()
			{
				for (int i = 0; i < this.joints.Count; i++)
				{
					this.joints[i].MapVirtual();
				}
			}

			// Token: 0x04003405 RID: 13317
			public Transform target;

			// Token: 0x04003406 RID: 13318
			[Range(0f, 1f)]
			public float weight;

			// Token: 0x04003407 RID: 13319
			public int iterations;

			// Token: 0x04003408 RID: 13320
			public List<Core.Joint> joints = new List<Core.Joint>();

			// Token: 0x04003409 RID: 13321
			private Vector3 IKpos;
		}

		// Token: 0x02000924 RID: 2340
		[Serializable]
		public class KinematicChain
		{
			// Token: 0x1700049F RID: 1183
			// (get) Token: 0x0600348C RID: 13452 RVA: 0x000E5248 File Offset: 0x000E3448
			// (set) Token: 0x0600348D RID: 13453 RVA: 0x000E5250 File Offset: 0x000E3450
			public bool initiated { get; private set; }

			// Token: 0x170004A0 RID: 1184
			// (get) Token: 0x0600348E RID: 13454 RVA: 0x000E5259 File Offset: 0x000E3459
			// (set) Token: 0x0600348F RID: 13455 RVA: 0x000E5261 File Offset: 0x000E3461
			public Quaternion[] initLocalRot { get; private set; }

			// Token: 0x170004A1 RID: 1185
			// (get) Token: 0x06003490 RID: 13456 RVA: 0x000E526A File Offset: 0x000E346A
			// (set) Token: 0x06003491 RID: 13457 RVA: 0x000E5272 File Offset: 0x000E3472
			public Vector3[] prevPos { get; private set; }

			// Token: 0x06003492 RID: 13458 RVA: 0x000E527C File Offset: 0x000E347C
			public KinematicChain()
			{
				this.momentOfInteria = 1000f;
				this.stiffness = -0.1f;
			}

			// Token: 0x06003493 RID: 13459 RVA: 0x000E52DC File Offset: 0x000E34DC
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

			// Token: 0x06003494 RID: 13460 RVA: 0x000E54A8 File Offset: 0x000E36A8
			public void MapVirtualJoints()
			{
				for (int i = 0; i < this.joints.Count; i++)
				{
					this.joints[i].joint.localRotation = this.initLocalRot[i];
				}
			}

			// Token: 0x0400340C RID: 13324
			[Range(0f, 1f)]
			[Header("Chain")]
			public float weight = 1f;

			// Token: 0x0400340D RID: 13325
			public List<Core.Joint> joints = new List<Core.Joint>();

			// Token: 0x0400340E RID: 13326
			[Header("Interafce")]
			public AnimationCurve solverFallOff;

			// Token: 0x0400340F RID: 13327
			public Vector3 gravity = Vector3.down;

			// Token: 0x04003410 RID: 13328
			public float momentOfInteria = 1000f;

			// Token: 0x04003411 RID: 13329
			public float stiffness = -0.1f;

			// Token: 0x04003412 RID: 13330
			public float torsionDamping;
		}
	}
}
