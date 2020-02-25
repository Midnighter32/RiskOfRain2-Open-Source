using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000732 RID: 1842
	public class EndRain : BaseState
	{
		// Token: 0x06002ACE RID: 10958 RVA: 0x000B4390 File Offset: 0x000B2590
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

		// Token: 0x06002ACF RID: 10959 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002AD1 RID: 10961 RVA: 0x000B4429 File Offset: 0x000B2629
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040026AB RID: 9899
		public static float baseDuration = 3f;

		// Token: 0x040026AC RID: 9900
		public static GameObject effectPrefab;

		// Token: 0x040026AD RID: 9901
		public static GameObject delayPrefab;

		// Token: 0x040026AE RID: 9902
		private float duration;
	}
}
