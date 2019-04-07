using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200013E RID: 318
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000616 RID: 1558 RVA: 0x0001C38C File Offset: 0x0001A58C
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			if (base.modelLocator)
			{
				Transform modelTransform = base.modelLocator.modelTransform;
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
				if (component)
				{
					component.FindChild("DustCenter").gameObject.SetActive(false);
					if (DeathState.initialEffect)
					{
						EffectManager.instance.SimpleMuzzleFlash(DeathState.initialEffect, base.gameObject, "DeathCenter", false);
					}
				}
				if (component2)
				{
					for (int i = 0; i < component2.rendererInfos.Length; i++)
					{
						component2.rendererInfos[i].ignoreOverlays = true;
					}
				}
			}
			base.PlayAnimation("Fullbody Override", "Death");
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0001C470 File Offset: 0x0001A670
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (!this.hasPlayedDeathEffect && this.animator.GetFloat("DeathEffect") > 0.5f)
			{
				this.hasPlayedDeathEffect = true;
				EffectManager.instance.SimpleMuzzleFlash(DeathState.deathEffect, base.gameObject, "DeathCenter", false);
			}
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0001C4E4 File Offset: 0x0001A6E4
		private void AttemptDeathBehavior()
		{
			if (this.attemptedDeathBehavior)
			{
				return;
			}
			this.attemptedDeathBehavior = true;
			if (base.modelLocator.modelBaseTransform)
			{
				EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
			}
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0001C53A File Offset: 0x0001A73A
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x0400071F RID: 1823
		public static GameObject initialEffect;

		// Token: 0x04000720 RID: 1824
		public static GameObject deathEffect;

		// Token: 0x04000721 RID: 1825
		private static float duration = 3.3166666f;

		// Token: 0x04000722 RID: 1826
		private float stopwatch;

		// Token: 0x04000723 RID: 1827
		private Animator animator;

		// Token: 0x04000724 RID: 1828
		private bool hasPlayedDeathEffect;

		// Token: 0x04000725 RID: 1829
		private bool attemptedDeathBehavior;
	}
}
