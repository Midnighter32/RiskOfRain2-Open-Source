using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Fauna
{
	// Token: 0x02000865 RID: 2149
	public class BirdsharkDeathState : BaseState
	{
		// Token: 0x0600307A RID: 12410 RVA: 0x000D0FB8 File Offset: 0x000CF1B8
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.deathSoundString, base.gameObject);
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
			if (this.initialExplosion && NetworkServer.active)
			{
				EffectManager.SimpleImpactEffect(this.initialExplosion, base.transform.position, Vector3.up, true);
			}
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x0600307B RID: 12411 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002ECB RID: 11979
		[SerializeField]
		public GameObject initialExplosion;

		// Token: 0x04002ECC RID: 11980
		[SerializeField]
		public string deathSoundString;
	}
}
