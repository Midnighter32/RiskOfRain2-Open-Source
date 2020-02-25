using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RoboBallBoss
{
	// Token: 0x02000799 RID: 1945
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002C92 RID: 11410 RVA: 0x000BC048 File Offset: 0x000BA248
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

		// Token: 0x06002C93 RID: 11411 RVA: 0x000BC0F4 File Offset: 0x000BA2F4
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

		// Token: 0x06002C94 RID: 11412 RVA: 0x000BC17B File Offset: 0x000BA37B
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000BC1A2 File Offset: 0x000BA3A2
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x040028A8 RID: 10408
		public static GameObject initialEffect;

		// Token: 0x040028A9 RID: 10409
		public static GameObject deathEffect;

		// Token: 0x040028AA RID: 10410
		public static float duration = 2f;

		// Token: 0x040028AB RID: 10411
		private float stopwatch;

		// Token: 0x040028AC RID: 10412
		private Transform modelBaseTransform;

		// Token: 0x040028AD RID: 10413
		private Transform centerTransform;

		// Token: 0x040028AE RID: 10414
		private bool attemptedDeathBehavior;
	}
}
