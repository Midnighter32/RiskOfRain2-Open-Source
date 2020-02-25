using System;
using EntityStates.RoboBallMini.Weapon;
using UnityEngine;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x020007A3 RID: 1955
	public class FireSpinningEyeBeam : FireEyeBeam
	{
		// Token: 0x06002CB9 RID: 11449 RVA: 0x000BCBF4 File Offset: 0x000BADF4
		public override void OnEnter()
		{
			string customName = this.outer.customName;
			this.eyeBeamOriginTransform = base.FindModelChild(customName);
			this.muzzleString = customName;
			base.OnEnter();
		}

		// Token: 0x06002CBA RID: 11450 RVA: 0x000BCC28 File Offset: 0x000BAE28
		public override Ray GetLaserRay()
		{
			Ray result = default(Ray);
			if (this.eyeBeamOriginTransform)
			{
				result.origin = this.eyeBeamOriginTransform.position;
				result.direction = this.eyeBeamOriginTransform.forward;
			}
			return result;
		}

		// Token: 0x06002CBB RID: 11451 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool ShouldFireLaser()
		{
			return true;
		}

		// Token: 0x040028E1 RID: 10465
		private Transform eyeBeamOriginTransform;
	}
}
