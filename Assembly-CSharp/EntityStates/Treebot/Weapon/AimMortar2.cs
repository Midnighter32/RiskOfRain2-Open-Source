using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x0200074C RID: 1868
	public class AimMortar2 : AimThrowableBase
	{
		// Token: 0x06002B4B RID: 11083 RVA: 0x000B6778 File Offset: 0x000B4978
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Gesture, Additive", "PrepBomb", 0.1f);
			Util.PlaySound(this.enterSound, base.gameObject);
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x000B67A8 File Offset: 0x000B49A8
		protected override void OnProjectileFiredLocal()
		{
			if (NetworkServer.active && base.healthComponent && this.healthCostFraction >= Mathf.Epsilon)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = base.healthComponent.combinedHealth * this.healthCostFraction;
				damageInfo.position = base.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Default;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = DamageType.NonLethal;
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = default(ProcChainMask);
				base.healthComponent.TakeDamage(damageInfo);
			}
			EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, base.gameObject, AimMortar2.muzzleName, false);
			Util.PlaySound(this.fireSound, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "FireBomb", 0.1f);
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x000B689D File Offset: 0x000B4A9D
		protected override bool KeyIsDown()
		{
			return base.inputBank.skill2.down;
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x000B68AF File Offset: 0x000B4AAF
		protected override void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
		{
			base.ModifyProjectile(ref fireProjectileInfo);
			fireProjectileInfo.position = this.currentTrajectoryInfo.hitPoint;
			fireProjectileInfo.rotation = Quaternion.identity;
			fireProjectileInfo.speedOverride = 0f;
		}

		// Token: 0x0400273A RID: 10042
		[SerializeField]
		public float healthCostFraction;

		// Token: 0x0400273B RID: 10043
		public static string muzzleName;

		// Token: 0x0400273C RID: 10044
		[SerializeField]
		public GameObject muzzleFlashPrefab;

		// Token: 0x0400273D RID: 10045
		[SerializeField]
		public string fireSound;

		// Token: 0x0400273E RID: 10046
		[SerializeField]
		public string enterSound;
	}
}
