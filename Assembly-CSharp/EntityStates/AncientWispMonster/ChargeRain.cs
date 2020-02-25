using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000730 RID: 1840
	public class ChargeRain : BaseState
	{
		// Token: 0x06002AC0 RID: 10944 RVA: 0x000B4154 File Offset: 0x000B2354
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

		// Token: 0x06002AC1 RID: 10945 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002AC2 RID: 10946 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002AC3 RID: 10947 RVA: 0x000B4223 File Offset: 0x000B2423
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new ChannelRain());
				return;
			}
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040026A2 RID: 9890
		public static float baseDuration = 3f;

		// Token: 0x040026A3 RID: 9891
		public static GameObject effectPrefab;

		// Token: 0x040026A4 RID: 9892
		public static GameObject delayPrefab;

		// Token: 0x040026A5 RID: 9893
		private float duration;
	}
}
