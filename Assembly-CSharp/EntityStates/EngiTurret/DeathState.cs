using System;
using RoR2;
using UnityEngine;

namespace EntityStates.EngiTurret
{
	// Token: 0x0200017F RID: 383
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x0600075C RID: 1884 RVA: 0x00024048 File Offset: 0x00022248
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(DeathState.deathSoundString, base.gameObject);
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.PlayInFixedTime("Death", layerIndex);
				modelAnimator.Update(0f);
				this.deathDuration = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
				UnityEngine.Object.Instantiate<GameObject>(DeathState.initialExplosion, base.transform.position, base.transform.rotation, base.transform);
			}
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x000240DC File Offset: 0x000222DC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > this.deathDuration)
			{
				if (DeathState.deathExplosion)
				{
					UnityEngine.Object.Instantiate<GameObject>(DeathState.deathExplosion, base.transform.position, Quaternion.identity);
				}
				if (base.modelLocator)
				{
					if (base.modelLocator.modelBaseTransform)
					{
						EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
					}
					if (base.modelLocator.modelTransform)
					{
						EntityState.Destroy(base.modelLocator.modelTransform.gameObject);
					}
				}
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000956 RID: 2390
		public static GameObject initialExplosion;

		// Token: 0x04000957 RID: 2391
		public static GameObject deathExplosion;

		// Token: 0x04000958 RID: 2392
		public static string deathSoundString;

		// Token: 0x04000959 RID: 2393
		private float deathDuration;
	}
}
