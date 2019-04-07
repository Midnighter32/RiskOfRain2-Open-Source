using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D3 RID: 211
	internal class EndRain : BaseState
	{
		// Token: 0x06000425 RID: 1061 RVA: 0x00011344 File Offset: 0x0000F544
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = EndRain.baseDuration / this.attackSpeedStat;
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.moveVector = Vector3.zero;
			}
			base.PlayAnimation("Body", "EndRain", "EndRain.playbackRate", this.duration);
			NodeGraph airNodes = SceneInfo.instance.airNodes;
			NodeGraph.NodeIndex nodeIndex = airNodes.FindClosestNode(base.transform.position, base.characterBody.hullClassification);
			Vector3 position;
			airNodes.GetNodePosition(nodeIndex, out position);
			base.transform.position = position;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x000113DD File Offset: 0x0000F5DD
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003E5 RID: 997
		public static float baseDuration = 3f;

		// Token: 0x040003E6 RID: 998
		public static GameObject effectPrefab;

		// Token: 0x040003E7 RID: 999
		public static GameObject delayPrefab;

		// Token: 0x040003E8 RID: 1000
		private float duration;
	}
}
