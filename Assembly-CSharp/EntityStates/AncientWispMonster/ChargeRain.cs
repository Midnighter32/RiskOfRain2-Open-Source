using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D1 RID: 209
	internal class ChargeRain : BaseState
	{
		// Token: 0x06000417 RID: 1047 RVA: 0x00011108 File Offset: 0x0000F308
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeRain.baseDuration / this.attackSpeedStat;
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = Vector3.zero;
			}
			base.PlayAnimation("Body", "ChargeRain", "ChargeRain.playbackRate", this.duration);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 999f, LayerIndex.world.mask))
			{
				NodeGraph groundNodes = SceneInfo.instance.groundNodes;
				NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(raycastHit.point, HullClassification.BeetleQueen);
				Vector3 a;
				groundNodes.GetNodePosition(nodeIndex, out a);
				base.transform.position = a + Vector3.up * 2f;
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x000111D7 File Offset: 0x0000F3D7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new ChannelRain());
				return;
			}
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003DC RID: 988
		public static float baseDuration = 3f;

		// Token: 0x040003DD RID: 989
		public static GameObject effectPrefab;

		// Token: 0x040003DE RID: 990
		public static GameObject delayPrefab;

		// Token: 0x040003DF RID: 991
		private float duration;
	}
}
