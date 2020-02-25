using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Croco
{
	// Token: 0x020008A7 RID: 2215
	public class FireSpit : BaseState
	{
		// Token: 0x060031A8 RID: 12712 RVA: 0x000D5EF0 File Offset: 0x000D40F0
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			this.duration = this.baseDuration / this.attackSpeedStat;
			base.StartAimMode(this.duration + 2f, false);
			base.PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", this.duration);
			Util.PlaySound(this.attackString, base.gameObject);
			base.AddRecoil(-1f * this.recoilAmplitude, -1.5f * this.recoilAmplitude, -0.25f * this.recoilAmplitude, 0.25f * this.recoilAmplitude);
			base.characterBody.AddSpreadBloom(this.bloom);
			string muzzleName = "MouthMuzzle";
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060031A9 RID: 12713 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060031AA RID: 12714 RVA: 0x000D602B File Offset: 0x000D422B
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060031AB RID: 12715 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04003029 RID: 12329
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x0400302A RID: 12330
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x0400302B RID: 12331
		[SerializeField]
		public float baseDuration = 2f;

		// Token: 0x0400302C RID: 12332
		[SerializeField]
		public float damageCoefficient = 1.2f;

		// Token: 0x0400302D RID: 12333
		[SerializeField]
		public float force = 20f;

		// Token: 0x0400302E RID: 12334
		[SerializeField]
		public string attackString;

		// Token: 0x0400302F RID: 12335
		[SerializeField]
		public float recoilAmplitude;

		// Token: 0x04003030 RID: 12336
		[SerializeField]
		public float bloom;

		// Token: 0x04003031 RID: 12337
		private float duration;
	}
}
