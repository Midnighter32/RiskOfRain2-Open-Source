using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200016B RID: 363
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000709 RID: 1801 RVA: 0x00021BC4 File Offset: 0x0001FDC4
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
						gameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = DeathState.duration + 2f;
						gameObject.transform.parent = this.centerTransform;
					}
				}
				this.modelBaseTransform = base.modelLocator.modelBaseTransform;
			}
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x00021C74 File Offset: 0x0001FE74
		private void AttemptDeathBehavior()
		{
			if (this.attemptedDeathBehavior)
			{
				return;
			}
			this.attemptedDeathBehavior = true;
			if (this.deathEffect && NetworkServer.active)
			{
				EffectManager.instance.SpawnEffect(this.deathEffect, new EffectData
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

		// Token: 0x0600070B RID: 1803 RVA: 0x00021D02 File Offset: 0x0001FF02
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00021D29 File Offset: 0x0001FF29
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x040008AB RID: 2219
		public static GameObject initialEffect;

		// Token: 0x040008AC RID: 2220
		[SerializeField]
		public GameObject deathEffect;

		// Token: 0x040008AD RID: 2221
		public static float duration = 2f;

		// Token: 0x040008AE RID: 2222
		private float stopwatch;

		// Token: 0x040008AF RID: 2223
		private Transform centerTransform;

		// Token: 0x040008B0 RID: 2224
		private Transform modelBaseTransform;

		// Token: 0x040008B1 RID: 2225
		private bool attemptedDeathBehavior;
	}
}
