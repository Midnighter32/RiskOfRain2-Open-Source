using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Vulture
{
	// Token: 0x0200073B RID: 1851
	public class FlyToLand : BaseSkillState
	{
		// Token: 0x06002AF8 RID: 11000 RVA: 0x000B4F10 File Offset: 0x000B3110
		public override void OnEnter()
		{
			base.OnEnter();
			Vector3 footPosition = this.GetFootPosition();
			if (base.isAuthority)
			{
				bool flag = false;
				NodeGraph groundNodes = SceneInfo.instance.groundNodes;
				if (groundNodes)
				{
					NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNodeWithFlagConditions(base.transform.position, base.characterBody.hullClassification, NodeFlags.None, NodeFlags.None, false);
					flag = (nodeIndex != NodeGraph.NodeIndex.invalid && groundNodes.GetNodePosition(nodeIndex, out this.targetPosition));
				}
				if (!flag)
				{
					this.outer.SetNextState(new Fly
					{
						activatorSkillSlot = this.activatorSkillSlot
					});
					this.duration = 0f;
					this.targetPosition = footPosition;
					return;
				}
			}
			Vector3 vector = this.targetPosition - footPosition;
			float num = this.moveSpeedStat * FlyToLand.speedMultiplier;
			this.duration = vector.magnitude / num;
			if (base.characterMotor)
			{
				base.characterMotor.isFlying = true;
				base.characterMotor.useGravity = false;
			}
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x000B500F File Offset: 0x000B320F
		private Vector3 GetFootPosition()
		{
			if (base.characterBody)
			{
				return base.characterBody.footPosition;
			}
			return base.transform.position;
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x000B5038 File Offset: 0x000B3238
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Vector3 footPosition = this.GetFootPosition();
			base.characterMotor.moveDirection = (this.targetPosition - footPosition).normalized * FlyToLand.speedMultiplier;
			if (base.isAuthority && (base.characterMotor.isGrounded || this.duration <= base.fixedAge))
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x000B50AC File Offset: 0x000B32AC
		public override void OnExit()
		{
			if (base.characterMotor)
			{
				base.characterMotor.isFlying = false;
				base.characterMotor.useGravity = true;
			}
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.SetFloat("Flying", 0f);
			}
			base.OnExit();
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000B5103 File Offset: 0x000B3303
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.targetPosition);
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000B5118 File Offset: 0x000B3318
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.targetPosition = reader.ReadVector3();
		}

		// Token: 0x040026D6 RID: 9942
		private float duration;

		// Token: 0x040026D7 RID: 9943
		private Vector3 targetPosition;

		// Token: 0x040026D8 RID: 9944
		public static float speedMultiplier;
	}
}
