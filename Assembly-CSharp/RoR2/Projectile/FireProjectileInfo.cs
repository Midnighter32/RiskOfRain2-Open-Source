using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200051B RID: 1307
	public struct FireProjectileInfo
	{
		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x00085957 File Offset: 0x00083B57
		// (set) Token: 0x06001ED4 RID: 7892 RVA: 0x0008593D File Offset: 0x00083B3D
		public float speedOverride
		{
			get
			{
				if (!this.useSpeedOverride)
				{
					return -1f;
				}
				return this._speedOverride;
			}
			set
			{
				this.useSpeedOverride = (value != -1f);
				this._speedOverride = value;
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001ED7 RID: 7895 RVA: 0x00085987 File Offset: 0x00083B87
		// (set) Token: 0x06001ED6 RID: 7894 RVA: 0x0008596D File Offset: 0x00083B6D
		public float fuseOverride
		{
			get
			{
				if (!this.useFuseOverride)
				{
					return -1f;
				}
				return this._fuseOverride;
			}
			set
			{
				this.useFuseOverride = (value != -1f);
				this._fuseOverride = value;
			}
		}

		// Token: 0x04001C65 RID: 7269
		public GameObject projectilePrefab;

		// Token: 0x04001C66 RID: 7270
		public Vector3 position;

		// Token: 0x04001C67 RID: 7271
		public Quaternion rotation;

		// Token: 0x04001C68 RID: 7272
		public GameObject owner;

		// Token: 0x04001C69 RID: 7273
		public GameObject target;

		// Token: 0x04001C6A RID: 7274
		public bool useSpeedOverride;

		// Token: 0x04001C6B RID: 7275
		private float _speedOverride;

		// Token: 0x04001C6C RID: 7276
		public bool useFuseOverride;

		// Token: 0x04001C6D RID: 7277
		private float _fuseOverride;

		// Token: 0x04001C6E RID: 7278
		public float damage;

		// Token: 0x04001C6F RID: 7279
		public float force;

		// Token: 0x04001C70 RID: 7280
		public bool crit;

		// Token: 0x04001C71 RID: 7281
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001C72 RID: 7282
		public ProcChainMask procChainMask;
	}
}
