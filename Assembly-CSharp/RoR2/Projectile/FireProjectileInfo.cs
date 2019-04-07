using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000554 RID: 1364
	public struct FireProjectileInfo
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001E5D RID: 7773 RVA: 0x0008F477 File Offset: 0x0008D677
		// (set) Token: 0x06001E5C RID: 7772 RVA: 0x0008F45D File Offset: 0x0008D65D
		public float speedOverride
		{
			get
			{
				return this._speedOverride;
			}
			set
			{
				this.useSpeedOverride = (value != -1f);
				this._speedOverride = value;
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06001E5F RID: 7775 RVA: 0x0008F499 File Offset: 0x0008D699
		// (set) Token: 0x06001E5E RID: 7774 RVA: 0x0008F47F File Offset: 0x0008D67F
		public float fuseOverride
		{
			get
			{
				return this._fuseOverride;
			}
			set
			{
				this.useFuseOverride = (value != -1f);
				this._fuseOverride = value;
			}
		}

		// Token: 0x04002100 RID: 8448
		public GameObject projectilePrefab;

		// Token: 0x04002101 RID: 8449
		public Vector3 position;

		// Token: 0x04002102 RID: 8450
		public Quaternion rotation;

		// Token: 0x04002103 RID: 8451
		public GameObject owner;

		// Token: 0x04002104 RID: 8452
		public GameObject target;

		// Token: 0x04002105 RID: 8453
		public bool useSpeedOverride;

		// Token: 0x04002106 RID: 8454
		private float _speedOverride;

		// Token: 0x04002107 RID: 8455
		public bool useFuseOverride;

		// Token: 0x04002108 RID: 8456
		private float _fuseOverride;

		// Token: 0x04002109 RID: 8457
		public float damage;

		// Token: 0x0400210A RID: 8458
		public float force;

		// Token: 0x0400210B RID: 8459
		public bool crit;

		// Token: 0x0400210C RID: 8460
		public DamageColorIndex damageColorIndex;

		// Token: 0x0400210D RID: 8461
		public ProcChainMask procChainMask;
	}
}
