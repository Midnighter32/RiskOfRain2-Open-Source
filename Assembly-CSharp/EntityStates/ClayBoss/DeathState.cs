using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001B7 RID: 439
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000896 RID: 2198 RVA: 0x0002B0A0 File Offset: 0x000292A0
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

		// Token: 0x06000897 RID: 2199 RVA: 0x0002B14C File Offset: 0x0002934C
		private void AttemptDeathBehavior()
		{
			if (this.attemptedDeathBehavior)
			{
				return;
			}
			this.attemptedDeathBehavior = true;
			if (DeathState.deathEffect && NetworkServer.active)
			{
				EffectManager.instance.SpawnEffect(DeathState.deathEffect, new EffectData
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

		// Token: 0x06000898 RID: 2200 RVA: 0x0002B1D8 File Offset: 0x000293D8
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0002B1FF File Offset: 0x000293FF
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x04000B7A RID: 2938
		public static GameObject initialEffect;

		// Token: 0x04000B7B RID: 2939
		public static GameObject deathEffect;

		// Token: 0x04000B7C RID: 2940
		public static float duration = 2f;

		// Token: 0x04000B7D RID: 2941
		private float stopwatch;

		// Token: 0x04000B7E RID: 2942
		private Transform modelBaseTransform;

		// Token: 0x04000B7F RID: 2943
		private Transform centerTransform;

		// Token: 0x04000B80 RID: 2944
		private bool attemptedDeathBehavior;
	}
}
