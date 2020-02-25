using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot
{
	// Token: 0x02000748 RID: 1864
	public class BurrowOut : GenericCharacterMain
	{
		// Token: 0x06002B38 RID: 11064 RVA: 0x000B62AC File Offset: 0x000B44AC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowOut.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowOut.burrowOutSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(BurrowOut.burrowPrefab, base.gameObject, "BurrowCenter", false);
			base.characterMotor.velocity = new Vector3(0f, BurrowOut.jumpVelocity, 0f);
			base.characterMotor.Motor.ForceUnground();
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x000B6344 File Offset: 0x000B4544
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x000B634C File Offset: 0x000B454C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002723 RID: 10019
		public static GameObject burrowPrefab;

		// Token: 0x04002724 RID: 10020
		public static float baseDuration;

		// Token: 0x04002725 RID: 10021
		public static string burrowOutSoundString;

		// Token: 0x04002726 RID: 10022
		public static float jumpVelocity;

		// Token: 0x04002727 RID: 10023
		private float stopwatch;

		// Token: 0x04002728 RID: 10024
		private Transform modelTransform;

		// Token: 0x04002729 RID: 10025
		private ChildLocator childLocator;

		// Token: 0x0400272A RID: 10026
		private float duration;
	}
}
