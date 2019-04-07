using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006DF RID: 1759
	public class RigReader
	{
		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06002765 RID: 10085 RVA: 0x000B6C37 File Offset: 0x000B4E37
		// (set) Token: 0x06002766 RID: 10086 RVA: 0x000B6C3F File Offset: 0x000B4E3F
		public RigReader.RigType rigType { get; private set; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06002767 RID: 10087 RVA: 0x000B6C48 File Offset: 0x000B4E48
		// (set) Token: 0x06002768 RID: 10088 RVA: 0x000B6C50 File Offset: 0x000B4E50
		public bool initiated { get; private set; }

		// Token: 0x06002769 RID: 10089 RVA: 0x000B6C5C File Offset: 0x000B4E5C
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

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x0600276A RID: 10090 RVA: 0x000B6CAD File Offset: 0x000B4EAD
		// (set) Token: 0x0600276B RID: 10091 RVA: 0x000B6CB5 File Offset: 0x000B4EB5
		public Core.Joint h_root { get; private set; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600276C RID: 10092 RVA: 0x000B6CBE File Offset: 0x000B4EBE
		// (set) Token: 0x0600276D RID: 10093 RVA: 0x000B6CC6 File Offset: 0x000B4EC6
		public Core.Joint h_head { get; private set; }

		// Token: 0x0600276E RID: 10094 RVA: 0x000B6CD0 File Offset: 0x000B4ED0
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

		// Token: 0x0600276F RID: 10095 RVA: 0x000B6F40 File Offset: 0x000B5140
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

		// Token: 0x06002770 RID: 10096 RVA: 0x000B6FC8 File Offset: 0x000B51C8
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

		// Token: 0x06002771 RID: 10097 RVA: 0x000B7050 File Offset: 0x000B5250
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

		// Token: 0x06002772 RID: 10098 RVA: 0x000B70C0 File Offset: 0x000B52C0
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

		// Token: 0x06002773 RID: 10099 RVA: 0x000B712F File Offset: 0x000B532F
		private bool IsReady()
		{
			if (!this.initiated)
			{
				Debug.LogWarning("Please initiate the Rig Reader first by calling the Constructor");
			}
			return this.initiated;
		}

		// Token: 0x04002986 RID: 10630
		public Animator animator;

		// Token: 0x04002989 RID: 10633
		public RigReader.LowerbodyRight r_lowerbody;

		// Token: 0x0400298A RID: 10634
		public RigReader.LowerbodyLeft l_lowerbody;

		// Token: 0x0400298B RID: 10635
		public RigReader.UpperbodyRight r_upperbody;

		// Token: 0x0400298C RID: 10636
		public RigReader.UpperbodyLeft l_upperbody;

		// Token: 0x0400298D RID: 10637
		public RigReader.Spine spine;

		// Token: 0x020006E0 RID: 1760
		public enum RigType
		{
			// Token: 0x04002991 RID: 10641
			Generic,
			// Token: 0x04002992 RID: 10642
			Humanoid
		}

		// Token: 0x020006E1 RID: 1761
		public struct LowerbodyRight
		{
			// Token: 0x04002993 RID: 10643
			public Core.Joint upperLeg;

			// Token: 0x04002994 RID: 10644
			public Core.Joint lowerLeg;

			// Token: 0x04002995 RID: 10645
			public Core.Joint foot;
		}

		// Token: 0x020006E2 RID: 1762
		public struct LowerbodyLeft
		{
			// Token: 0x04002996 RID: 10646
			public Core.Joint upperLeg;

			// Token: 0x04002997 RID: 10647
			public Core.Joint lowerLeg;

			// Token: 0x04002998 RID: 10648
			public Core.Joint foot;
		}

		// Token: 0x020006E3 RID: 1763
		public struct UpperbodyRight
		{
			// Token: 0x04002999 RID: 10649
			public Core.Joint shoulder;

			// Token: 0x0400299A RID: 10650
			public Core.Joint upperArm;

			// Token: 0x0400299B RID: 10651
			public Core.Joint lowerArm;

			// Token: 0x0400299C RID: 10652
			public Core.Joint hand;
		}

		// Token: 0x020006E4 RID: 1764
		public struct UpperbodyLeft
		{
			// Token: 0x0400299D RID: 10653
			public Core.Joint shoulder;

			// Token: 0x0400299E RID: 10654
			public Core.Joint upperArm;

			// Token: 0x0400299F RID: 10655
			public Core.Joint lowerArm;

			// Token: 0x040029A0 RID: 10656
			public Core.Joint hand;
		}

		// Token: 0x020006E5 RID: 1765
		public struct Spine
		{
			// Token: 0x040029A1 RID: 10657
			public Core.Joint spine;

			// Token: 0x040029A2 RID: 10658
			public Core.Joint chest;
		}
	}
}
