using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D2 RID: 2258
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06003295 RID: 12949 RVA: 0x000DACC0 File Offset: 0x000D8EC0
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.centerTransform = component.FindChild("Center");
					if (DeathState.initialEffect)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(DeathState.initialEffect, this.centerTransform.position, this.centerTransform.rotation);
						gameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = DeathState.duration;
						gameObject.transform.parent = this.centerTransform;
					}
				}
				this.modelBaseTransform = base.modelLocator.modelBaseTransform;
			}
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x000DAD6C File Offset: 0x000D8F6C
		private void AttemptDeathBehavior()
		{
			if (this.attemptedDeathBehavior)
			{
				return;
			}
			this.attemptedDeathBehavior = true;
			if (DeathState.deathEffect && NetworkServer.active)
			{
				EffectManager.SpawnEffect(DeathState.deathEffect, new EffectData
				{
					origin = this.centerTransform.position
				}, true);
			}
			if (this.modelBaseTransform)
			{
				EntityState.Destroy(this.modelBaseTransform.gameObject);
				this.modelBaseTransform = null;
			}
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x000DADF3 File Offset: 0x000D8FF3
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x000DAE1A File Offset: 0x000D901A
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x040031A2 RID: 12706
		public static GameObject initialEffect;

		// Token: 0x040031A3 RID: 12707
		public static GameObject deathEffect;

		// Token: 0x040031A4 RID: 12708
		public static float duration = 2f;

		// Token: 0x040031A5 RID: 12709
		private float stopwatch;

		// Token: 0x040031A6 RID: 12710
		private Transform modelBaseTransform;

		// Token: 0x040031A7 RID: 12711
		private Transform centerTransform;

		// Token: 0x040031A8 RID: 12712
		private bool attemptedDeathBehavior;
	}
}
