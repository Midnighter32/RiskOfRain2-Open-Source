using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000DD RID: 221
	public class FireSpear : GenericBulletBaseState
	{
		// Token: 0x06000454 RID: 1108 RVA: 0x00011F38 File Offset: 0x00010138
		protected override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.stopperMask = LayerIndex.world.mask;
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00011F68 File Offset: 0x00010168
		protected override void FireBullet(Ray aimRay)
		{
			base.FireBullet(aimRay);
			base.characterBody.SetSpreadBloom(0.2f, false);
			base.AddRecoil(-0.6f * FireSpear.recoilAmplitude, -0.8f * FireSpear.recoilAmplitude, -0.1f * FireSpear.recoilAmplitude, 0.1f * FireSpear.recoilAmplitude);
			base.PlayAnimation("Gesture, Additive", "FireSpear", "FireSpear.playbackRate", this.duration);
		}

		// Token: 0x04000417 RID: 1047
		public float charge;

		// Token: 0x04000418 RID: 1048
		public static float recoilAmplitude;
	}
}
