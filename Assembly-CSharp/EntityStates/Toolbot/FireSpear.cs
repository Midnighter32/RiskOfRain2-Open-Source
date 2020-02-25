using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x02000764 RID: 1892
	public class FireSpear : GenericBulletBaseState
	{
		// Token: 0x06002BAF RID: 11183 RVA: 0x000B8800 File Offset: 0x000B6A00
		protected override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.stopperMask = LayerIndex.world.mask;
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
		}

		// Token: 0x06002BB0 RID: 11184 RVA: 0x000B8830 File Offset: 0x000B6A30
		protected override void FireBullet(Ray aimRay)
		{
			base.FireBullet(aimRay);
			base.characterBody.SetSpreadBloom(0.2f, false);
			base.AddRecoil(-0.6f * FireSpear.recoilAmplitude, -0.8f * FireSpear.recoilAmplitude, -0.1f * FireSpear.recoilAmplitude, 0.1f * FireSpear.recoilAmplitude);
			base.PlayAnimation("Gesture, Additive", "FireSpear", "FireSpear.playbackRate", this.duration);
		}

		// Token: 0x040027D2 RID: 10194
		public float charge;

		// Token: 0x040027D3 RID: 10195
		public static float recoilAmplitude;
	}
}
