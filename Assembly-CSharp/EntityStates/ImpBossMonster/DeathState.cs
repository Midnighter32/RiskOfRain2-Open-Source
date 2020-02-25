using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200081A RID: 2074
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002F03 RID: 12035 RVA: 0x000C872C File Offset: 0x000C692C
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
						EffectManager.SimpleMuzzleFlash(DeathState.initialEffect, base.gameObject, "DeathCenter", false);
					}
				}
				if (component2)
				{
					for (int i = 0; i < component2.baseRendererInfos.Length; i++)
					{
						component2.baseRendererInfos[i].ignoreOverlays = true;
					}
				}
			}
			base.PlayAnimation("Fullbody Override", "Death");
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x000C880C File Offset: 0x000C6A0C
		public override void FixedUpdate()
		{
			if (this.animator)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (!this.hasPlayedDeathEffect && this.animator.GetFloat("DeathEffect") > 0.5f)
				{
					this.hasPlayedDeathEffect = true;
					EffectManager.SimpleMuzzleFlash(DeathState.deathEffect, base.gameObject, "DeathCenter", false);
				}
				if (this.stopwatch >= DeathState.duration)
				{
					this.AttemptDeathBehavior();
				}
			}
		}

		// Token: 0x06002F05 RID: 12037 RVA: 0x000C8888 File Offset: 0x000C6A88
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

		// Token: 0x06002F06 RID: 12038 RVA: 0x000C88DE File Offset: 0x000C6ADE
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x04002C5F RID: 11359
		public static GameObject initialEffect;

		// Token: 0x04002C60 RID: 11360
		public static GameObject deathEffect;

		// Token: 0x04002C61 RID: 11361
		private static float duration = 3.3166666f;

		// Token: 0x04002C62 RID: 11362
		private float stopwatch;

		// Token: 0x04002C63 RID: 11363
		private Animator animator;

		// Token: 0x04002C64 RID: 11364
		private bool hasPlayedDeathEffect;

		// Token: 0x04002C65 RID: 11365
		private bool attemptedDeathBehavior;
	}
}
