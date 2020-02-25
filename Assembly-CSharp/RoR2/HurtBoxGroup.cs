using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000247 RID: 583
	public class HurtBoxGroup : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x06000CDF RID: 3295 RVA: 0x00039BA3 File Offset: 0x00037DA3
		public void OnDeathStart()
		{
			this.SetHurtboxesActive(false);
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000CE0 RID: 3296 RVA: 0x00039BAC File Offset: 0x00037DAC
		// (set) Token: 0x06000CE1 RID: 3297 RVA: 0x00039BB4 File Offset: 0x00037DB4
		public int hurtBoxesDeactivatorCounter
		{
			get
			{
				return this._hurtBoxesDeactivatorCounter;
			}
			set
			{
				bool flag = this._hurtBoxesDeactivatorCounter <= 0;
				bool flag2 = value <= 0;
				this._hurtBoxesDeactivatorCounter = value;
				if (flag != flag2)
				{
					this.SetHurtboxesActive(flag2);
				}
			}
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x00039BE8 File Offset: 0x00037DE8
		private void SetHurtboxesActive(bool active)
		{
			for (int i = 0; i < this.hurtBoxes.Length; i++)
			{
				this.hurtBoxes[i].gameObject.SetActive(active);
			}
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x00039C1C File Offset: 0x00037E1C
		public void OnValidate()
		{
			int num = 0;
			short num2 = 0;
			while ((int)num2 < this.hurtBoxes.Length)
			{
				this.hurtBoxes[(int)num2].hurtBoxGroup = this;
				this.hurtBoxes[(int)num2].indexInGroup = num2;
				if (this.hurtBoxes[(int)num2].isBullseye)
				{
					num++;
				}
				num2 += 1;
			}
			if (this.bullseyeCount != num)
			{
				this.bullseyeCount = num;
			}
			if (!this.mainHurtBox)
			{
				IEnumerable<HurtBox> source = from v in this.hurtBoxes
				where v.isBullseye
				select v;
				IEnumerable<HurtBox> source2 = from v in source
				where v.transform.parent.name.ToLower(CultureInfo.InvariantCulture) == "chest"
				select v;
				this.mainHurtBox = (source2.FirstOrDefault<HurtBox>() ?? source.FirstOrDefault<HurtBox>());
			}
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x00039CF2 File Offset: 0x00037EF2
		public HurtBoxGroup.VolumeDistribution GetVolumeDistribution()
		{
			return new HurtBoxGroup.VolumeDistribution(this.hurtBoxes);
		}

		// Token: 0x04000CEF RID: 3311
		[Tooltip("The hurtboxes in this group. This really shouldn't be set manually.")]
		public HurtBox[] hurtBoxes;

		// Token: 0x04000CF0 RID: 3312
		[Tooltip("The most important hurtbox in this group, usually a good center-of-mass target like the chest.")]
		public HurtBox mainHurtBox;

		// Token: 0x04000CF1 RID: 3313
		[HideInInspector]
		public int bullseyeCount;

		// Token: 0x04000CF2 RID: 3314
		private int _hurtBoxesDeactivatorCounter;

		// Token: 0x02000248 RID: 584
		public class VolumeDistribution
		{
			// Token: 0x170001A8 RID: 424
			// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x00039CFF File Offset: 0x00037EFF
			// (set) Token: 0x06000CE7 RID: 3303 RVA: 0x00039D07 File Offset: 0x00037F07
			public float totalVolume { get; private set; }

			// Token: 0x06000CE8 RID: 3304 RVA: 0x00039D10 File Offset: 0x00037F10
			public VolumeDistribution(HurtBox[] hurtBoxes)
			{
				this.totalVolume = 0f;
				for (int i = 0; i < hurtBoxes.Length; i++)
				{
					this.totalVolume += hurtBoxes[i].volume;
				}
				this.hurtBoxes = (HurtBox[])hurtBoxes.Clone();
			}

			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x00039D64 File Offset: 0x00037F64
			public Vector3 randomVolumePoint
			{
				get
				{
					float num = UnityEngine.Random.Range(0f, this.totalVolume);
					HurtBox hurtBox = this.hurtBoxes[0];
					float num2 = 0f;
					for (int i = 0; i < this.hurtBoxes.Length; i++)
					{
						num2 += this.hurtBoxes[i].volume;
						if (num2 <= num)
						{
							hurtBox = this.hurtBoxes[i];
							break;
						}
					}
					return hurtBox.randomVolumePoint;
				}
			}

			// Token: 0x04000CF4 RID: 3316
			private HurtBox[] hurtBoxes;
		}
	}
}
