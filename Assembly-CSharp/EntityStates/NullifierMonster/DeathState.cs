using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.NullifierMonster
{
	// Token: 0x020007B0 RID: 1968
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06002D01 RID: 11521 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldAutoDestroy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x000BDF6D File Offset: 0x000BC16D
		protected override void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
			base.PlayCrossfade("Body", "Death", "Death.playbackRate", DeathState.duration, 0.1f);
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x000BDF90 File Offset: 0x000BC190
		public override void OnEnter()
		{
			base.OnEnter();
			this.muzzleTransform = base.FindModelChild(DeathState.muzzleName);
			if (this.muzzleTransform && DeathState.deathPreExplosionVFX)
			{
				this.deathPreExplosionInstance = UnityEngine.Object.Instantiate<GameObject>(DeathState.deathPreExplosionVFX, this.muzzleTransform.position, this.muzzleTransform.rotation);
				this.deathPreExplosionInstance.transform.parent = this.muzzleTransform;
				this.deathPreExplosionInstance.transform.localScale = Vector3.one * DeathState.deathExplosionRadius;
				ScaleParticleSystemDuration component = this.deathPreExplosionInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = DeathState.duration;
				}
			}
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x000BE04C File Offset: 0x000BC24C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= DeathState.duration)
			{
				if (!this.hasFiredVoidPortal)
				{
					this.hasFiredVoidPortal = true;
					this.FireVoidPortal();
					return;
				}
				if (NetworkServer.active && base.fixedAge >= DeathState.duration + 4f)
				{
					EntityState.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x000BE0A8 File Offset: 0x000BC2A8
		private void FireVoidPortal()
		{
			if (base.cachedModelTransform)
			{
				EntityState.Destroy(base.cachedModelTransform.gameObject);
			}
			if (NetworkServer.active)
			{
				Collider[] array = Physics.OverlapSphere(this.muzzleTransform.position, DeathState.deathExplosionRadius, LayerIndex.entityPrecise.mask);
				CharacterBody[] array2 = new CharacterBody[array.Length];
				int count = 0;
				for (int i = 0; i < array.Length; i++)
				{
					CharacterBody characterBody = Util.HurtBoxColliderToBody(array[i]);
					if (characterBody && !(characterBody == base.characterBody) && Array.IndexOf<CharacterBody>(array2, characterBody, 0, count) == -1)
					{
						array2[count++] = characterBody;
					}
				}
				foreach (CharacterBody characterBody2 in array2)
				{
					if (characterBody2 && characterBody2.healthComponent)
					{
						characterBody2.healthComponent.Suicide(base.gameObject, base.gameObject, DamageType.VoidDeath);
					}
				}
				if (DeathState.deathExplosionEffect)
				{
					EffectManager.SpawnEffect(DeathState.deathExplosionEffect, new EffectData
					{
						origin = base.characterBody.corePosition,
						scale = DeathState.deathExplosionRadius
					}, true);
				}
			}
		}

		// Token: 0x04002938 RID: 10552
		public static GameObject deathPreExplosionVFX;

		// Token: 0x04002939 RID: 10553
		public static GameObject deathExplosionEffect;

		// Token: 0x0400293A RID: 10554
		public static float duration;

		// Token: 0x0400293B RID: 10555
		public static float deathExplosionRadius;

		// Token: 0x0400293C RID: 10556
		public static string muzzleName;

		// Token: 0x0400293D RID: 10557
		private GameObject deathPreExplosionInstance;

		// Token: 0x0400293E RID: 10558
		private Transform muzzleTransform;

		// Token: 0x0400293F RID: 10559
		private bool hasFiredVoidPortal;
	}
}
