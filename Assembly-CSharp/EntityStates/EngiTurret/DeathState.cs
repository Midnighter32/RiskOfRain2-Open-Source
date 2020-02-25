using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.EngiTurret
{
	// Token: 0x02000867 RID: 2151
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x0600307F RID: 12415 RVA: 0x000D1128 File Offset: 0x000CF328
		protected override void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Body");
				modelAnimator.PlayInFixedTime("Death", layerIndex);
				modelAnimator.Update(0f);
				this.deathDuration = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
				if (this.initialExplosion)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.initialExplosion, base.transform.position, base.transform.rotation, base.transform);
				}
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06003080 RID: 12416 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldAutoDestroy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003081 RID: 12417 RVA: 0x000D11B4 File Offset: 0x000CF3B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > this.deathDuration && NetworkServer.active && this.deathExplosion)
			{
				EffectManager.SpawnEffect(this.deathExplosion, new EffectData
				{
					origin = base.transform.position,
					scale = 2f
				}, true);
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06003082 RID: 12418 RVA: 0x000BB337 File Offset: 0x000B9537
		public override void OnExit()
		{
			base.DestroyModel();
			base.OnExit();
		}

		// Token: 0x04002ED1 RID: 11985
		[SerializeField]
		public GameObject initialExplosion;

		// Token: 0x04002ED2 RID: 11986
		[SerializeField]
		public GameObject deathExplosion;

		// Token: 0x04002ED3 RID: 11987
		private float deathDuration;
	}
}
