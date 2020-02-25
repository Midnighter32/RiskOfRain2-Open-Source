using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000851 RID: 2129
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06003024 RID: 12324 RVA: 0x000CE9BC File Offset: 0x000CCBBC
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

		// Token: 0x06003025 RID: 12325 RVA: 0x000CEA6C File Offset: 0x000CCC6C
		private void AttemptDeathBehavior()
		{
			if (this.attemptedDeathBehavior)
			{
				return;
			}
			this.attemptedDeathBehavior = true;
			if (this.deathEffect && NetworkServer.active)
			{
				EffectManager.SpawnEffect(this.deathEffect, new EffectData
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

		// Token: 0x06003026 RID: 12326 RVA: 0x000CEAF5 File Offset: 0x000CCCF5
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= DeathState.duration)
			{
				this.AttemptDeathBehavior();
			}
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x000CEB1C File Offset: 0x000CCD1C
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.AttemptDeathBehavior();
			}
			base.OnExit();
		}

		// Token: 0x04002E17 RID: 11799
		public static GameObject initialEffect;

		// Token: 0x04002E18 RID: 11800
		[SerializeField]
		public GameObject deathEffect;

		// Token: 0x04002E19 RID: 11801
		public static float duration = 2f;

		// Token: 0x04002E1A RID: 11802
		private float stopwatch;

		// Token: 0x04002E1B RID: 11803
		private Transform centerTransform;

		// Token: 0x04002E1C RID: 11804
		private Transform modelBaseTransform;

		// Token: 0x04002E1D RID: 11805
		private bool attemptedDeathBehavior;
	}
}
