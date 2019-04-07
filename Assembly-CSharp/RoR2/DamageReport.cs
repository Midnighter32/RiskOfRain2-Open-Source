using System;

namespace RoR2
{
	// Token: 0x02000237 RID: 567
	public class DamageReport
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000ABF RID: 2751 RVA: 0x00035228 File Offset: 0x00033428
		// (set) Token: 0x06000ABE RID: 2750 RVA: 0x000351DC File Offset: 0x000333DC
		public HealthComponent victim
		{
			get
			{
				return this._victim;
			}
			set
			{
				this._victim = value;
				this.victimBody = (value ? value.body : null);
				this.victimMaster = (this.victimBody ? this.victimBody.master : null);
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00035230 File Offset: 0x00033430
		// (set) Token: 0x06000AC1 RID: 2753 RVA: 0x00035238 File Offset: 0x00033438
		public CharacterBody victimBody { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00035241 File Offset: 0x00033441
		// (set) Token: 0x06000AC3 RID: 2755 RVA: 0x00035249 File Offset: 0x00033449
		public CharacterMaster victimMaster { get; private set; }

		// Token: 0x04000E78 RID: 3704
		private HealthComponent _victim;

		// Token: 0x04000E7B RID: 3707
		public DamageInfo damageInfo;
	}
}
