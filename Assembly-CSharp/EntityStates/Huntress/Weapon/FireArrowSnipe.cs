using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress.Weapon
{
	// Token: 0x02000832 RID: 2098
	public class FireArrowSnipe : GenericBulletBaseState
	{
		// Token: 0x06002F7B RID: 12155 RVA: 0x000CB0DC File Offset: 0x000C92DC
		protected override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
			base.skillLocator ? base.skillLocator.primary : null;
		}

		// Token: 0x06002F7C RID: 12156 RVA: 0x000CB110 File Offset: 0x000C9310
		protected override void FireBullet(Ray aimRay)
		{
			base.FireBullet(aimRay);
			base.characterBody.SetSpreadBloom(0.2f, false);
			base.AddRecoil(-0.6f * FireArrowSnipe.recoilAmplitude, -0.8f * FireArrowSnipe.recoilAmplitude, -0.1f * FireArrowSnipe.recoilAmplitude, 0.1f * FireArrowSnipe.recoilAmplitude);
			base.PlayAnimation("Body", "FireArrowSnipe", "FireArrowSnipe.playbackRate", this.duration);
			base.healthComponent.TakeDamageForce(aimRay.direction * -400f, true, false);
		}

		// Token: 0x04002D10 RID: 11536
		public float charge;

		// Token: 0x04002D11 RID: 11537
		public static float recoilAmplitude;
	}
}
