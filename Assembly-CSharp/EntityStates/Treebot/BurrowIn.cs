using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot
{
	// Token: 0x02000747 RID: 1863
	public class BurrowIn : BaseState
	{
		// Token: 0x06002B33 RID: 11059 RVA: 0x000B61B0 File Offset: 0x000B43B0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowIn.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "BurrowIn", "BurrowIn.playbackRate", this.duration, 0.1f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowIn.burrowInSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(BurrowIn.burrowPrefab, base.gameObject, "BurrowCenter", false);
			if (base.characterBody)
			{
				base.characterBody.hideCrosshair = true;
			}
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x000B6254 File Offset: 0x000B4454
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				Burrowed nextState = new Burrowed();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400271C RID: 10012
		public static GameObject burrowPrefab;

		// Token: 0x0400271D RID: 10013
		public static float baseDuration;

		// Token: 0x0400271E RID: 10014
		public static string burrowInSoundString;

		// Token: 0x0400271F RID: 10015
		private float stopwatch;

		// Token: 0x04002720 RID: 10016
		private float duration;

		// Token: 0x04002721 RID: 10017
		private Transform modelTransform;

		// Token: 0x04002722 RID: 10018
		private ChildLocator childLocator;
	}
}
