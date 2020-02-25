using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x0200092A RID: 2346
	public class RigReader
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x060034AF RID: 13487 RVA: 0x000E607F File Offset: 0x000E427F
		// (set) Token: 0x060034B0 RID: 13488 RVA: 0x000E6087 File Offset: 0x000E4287
		public RigReader.RigType rigType { get; private set; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x060034B1 RID: 13489 RVA: 0x000E6090 File Offset: 0x000E4290
		// (set) Token: 0x060034B2 RID: 13490 RVA: 0x000E6098 File Offset: 0x000E4298
		public bool initiated { get; private set; }

		// Token: 0x060034B3 RID: 13491 RVA: 0x000E60A4 File Offset: 0x000E42A4
		public RigReader(Animator rig)
		{
			this.animator = rig;
			this.rigType = ((rig && rig.isHuman) ? RigReader.RigType.Humanoid : RigReader.RigType.Generic);
			if (this.rigType == RigReader.RigType.Generic)
			{
				this.initiated = true;
				return;
			}
			this.ReadHumanoidRig();
			this.initiated = true;
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x060034B4 RID: 13492 RVA: 0x000E60F5 File Offset: 0x000E42F5
		// (set) Token: 0x060034B5 RID: 13493 RVA: 0x000E60FD File Offset: 0x000E42FD
		public Core.Joint h_root { get; private set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x060034B6 RID: 13494 RVA: 0x000E6106 File Offset: 0x000E4306
		// (set) Token: 0x060034B7 RID: 13495 RVA: 0x000E610E File Offset: 0x000E430E
		public Core.Joint h_head { get; private set; }

		// Token: 0x060034B8 RID: 13496 RVA: 0x000E6118 File Offset: 0x000E4318
		private void ReadHumanoidRig()
		{
			this.h_root = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.Hips)
			};
			this.h_head = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.Head)
			};
			this.spine.spine = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.Spine)
			};
			this.spine.chest = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.Chest)
			};
			this.r_upperbody.shoulder = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightShoulder)
			};
			this.r_upperbody.upperArm = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightUpperArm)
			};
			this.r_upperbody.lowerArm = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightLowerArm)
			};
			this.r_upperbody.hand = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightHand)
			};
			this.l_upperbody.shoulder = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftShoulder)
			};
			this.l_upperbody.upperArm = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftUpperArm)
			};
			this.l_upperbody.lowerArm = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftLowerArm)
			};
			this.l_upperbody.hand = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftHand)
			};
			this.r_lowerbody.upperLeg = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightUpperLeg)
			};
			this.r_lowerbody.lowerLeg = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightLowerLeg)
			};
			this.r_lowerbody.foot = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.RightFoot)
			};
			this.l_lowerbody.upperLeg = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg)
			};
			this.l_lowerbody.lowerLeg = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg)
			};
			this.l_lowerbody.foot = new Core.Joint
			{
				joint = this.animator.GetBoneTransform(HumanBodyBones.LeftFoot)
			};
		}

		// Token: 0x060034B9 RID: 13497 RVA: 0x000E6388 File Offset: 0x000E4588
		public Core.Chain RightArmChain()
		{
			if (!this.IsReady())
			{
				return null;
			}
			if (this.rigType != RigReader.RigType.Humanoid)
			{
				return null;
			}
			Core.Chain chain = new Core.Chain();
			chain.joints.Add(this.r_upperbody.shoulder);
			chain.joints.Add(this.r_upperbody.upperArm);
			chain.joints.Add(this.r_upperbody.lowerArm);
			chain.joints.Add(this.r_upperbody.hand);
			chain.InitiateJoints();
			return chain;
		}

		// Token: 0x060034BA RID: 13498 RVA: 0x000E6410 File Offset: 0x000E4610
		public Core.Chain LeftArmChain()
		{
			if (!this.IsReady())
			{
				return null;
			}
			if (this.rigType != RigReader.RigType.Humanoid)
			{
				return null;
			}
			Core.Chain chain = new Core.Chain();
			chain.joints.Add(this.l_upperbody.shoulder);
			chain.joints.Add(this.l_upperbody.upperArm);
			chain.joints.Add(this.l_upperbody.lowerArm);
			chain.joints.Add(this.l_upperbody.hand);
			chain.InitiateJoints();
			return chain;
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x000E6498 File Offset: 0x000E4698
		public Core.Chain RightLegChain()
		{
			if (!this.IsReady())
			{
				return null;
			}
			if (this.rigType != RigReader.RigType.Humanoid)
			{
				return null;
			}
			Core.Chain chain = new Core.Chain();
			chain.joints.Add(this.r_lowerbody.upperLeg);
			chain.joints.Add(this.r_lowerbody.lowerLeg);
			chain.joints.Add(this.r_lowerbody.foot);
			chain.InitiateJoints();
			return chain;
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x000E6508 File Offset: 0x000E4708
		public Core.Chain LeftLegChain()
		{
			if (!this.IsReady())
			{
				return null;
			}
			if (this.rigType != RigReader.RigType.Humanoid)
			{
				return null;
			}
			Core.Chain chain = new Core.Chain();
			chain.joints.Add(this.l_lowerbody.upperLeg);
			chain.joints.Add(this.l_lowerbody.lowerLeg);
			chain.joints.Add(this.l_lowerbody.foot);
			chain.InitiateJoints();
			return chain;
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x000E6577 File Offset: 0x000E4777
		private bool IsReady()
		{
			if (!this.initiated)
			{
				Debug.LogWarning("Please initiate the Rig Reader first by calling the Constructor");
			}
			return this.initiated;
		}

		// Token: 0x0400341F RID: 13343
		public Animator animator;

		// Token: 0x04003422 RID: 13346
		public RigReader.LowerbodyRight r_lowerbody;

		// Token: 0x04003423 RID: 13347
		public RigReader.LowerbodyLeft l_lowerbody;

		// Token: 0x04003424 RID: 13348
		public RigReader.UpperbodyRight r_upperbody;

		// Token: 0x04003425 RID: 13349
		public RigReader.UpperbodyLeft l_upperbody;

		// Token: 0x04003426 RID: 13350
		public RigReader.Spine spine;

		// Token: 0x0200092B RID: 2347
		public enum RigType
		{
			// Token: 0x0400342A RID: 13354
			Generic,
			// Token: 0x0400342B RID: 13355
			Humanoid
		}

		// Token: 0x0200092C RID: 2348
		public struct LowerbodyRight
		{
			// Token: 0x0400342C RID: 13356
			public Core.Joint upperLeg;

			// Token: 0x0400342D RID: 13357
			public Core.Joint lowerLeg;

			// Token: 0x0400342E RID: 13358
			public Core.Joint foot;
		}

		// Token: 0x0200092D RID: 2349
		public struct LowerbodyLeft
		{
			// Token: 0x0400342F RID: 13359
			public Core.Joint upperLeg;

			// Token: 0x04003430 RID: 13360
			public Core.Joint lowerLeg;

			// Token: 0x04003431 RID: 13361
			public Core.Joint foot;
		}

		// Token: 0x0200092E RID: 2350
		public struct UpperbodyRight
		{
			// Token: 0x04003432 RID: 13362
			public Core.Joint shoulder;

			// Token: 0x04003433 RID: 13363
			public Core.Joint upperArm;

			// Token: 0x04003434 RID: 13364
			public Core.Joint lowerArm;

			// Token: 0x04003435 RID: 13365
			public Core.Joint hand;
		}

		// Token: 0x0200092F RID: 2351
		public struct UpperbodyLeft
		{
			// Token: 0x04003436 RID: 13366
			public Core.Joint shoulder;

			// Token: 0x04003437 RID: 13367
			public Core.Joint upperArm;

			// Token: 0x04003438 RID: 13368
			public Core.Joint lowerArm;

			// Token: 0x04003439 RID: 13369
			public Core.Joint hand;
		}

		// Token: 0x02000930 RID: 2352
		public struct Spine
		{
			// Token: 0x0400343A RID: 13370
			public Core.Joint spine;

			// Token: 0x0400343B RID: 13371
			public Core.Joint chest;
		}
	}
}
