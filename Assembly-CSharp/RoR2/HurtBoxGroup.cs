using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000326 RID: 806
	public class HurtBoxGroup : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x06001095 RID: 4245 RVA: 0x00052CBC File Offset: 0x00050EBC
		public void OnDeathStart()
		{
			this.SetHurtboxesActive(false);
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x00052CC5 File Offset: 0x00050EC5
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x00052CD0 File Offset: 0x00050ED0
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

		// Token: 0x06001098 RID: 4248 RVA: 0x00052D04 File Offset: 0x00050F04
		private void SetHurtboxesActive(bool active)
		{
			for (int i = 0; i < this.hurtBoxes.Length; i++)
			{
				this.hurtBoxes[i].gameObject.SetActive(active);
			}
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00052D38 File Offset: 0x00050F38
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
				where v.transform.parent.name.ToLower() == "chest"
				select v;
				this.mainHurtBox = (source2.FirstOrDefault<HurtBox>() ?? source.FirstOrDefault<HurtBox>());
			}
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x00052E0E File Offset: 0x0005100E
		public HurtBoxGroup.VolumeDistribution GetVolumeDistribution()
		{
			return new HurtBoxGroup.VolumeDistribution(this.hurtBoxes);
		}

		// Token: 0x040014B1 RID: 5297
		[Tooltip("The hurtboxes in this group. This really shouldn't be set manually.")]
		public HurtBox[] hurtBoxes;

		// Token: 0x040014B2 RID: 5298
		[Tooltip("The most important hurtbox in this group, usually a good center-of-mass target like the chest.")]
		public HurtBox mainHurtBox;

		// Token: 0x040014B3 RID: 5299
		[HideInInspector]
		public int bullseyeCount;

		// Token: 0x040014B4 RID: 5300
		private int _hurtBoxesDeactivatorCounter;

		// Token: 0x02000327 RID: 807
		public class VolumeDistribution
		{
			// Token: 0x1700016E RID: 366
			// (get) Token: 0x0600109C RID: 4252 RVA: 0x00052E1B File Offset: 0x0005101B
			// (set) Token: 0x0600109D RID: 4253 RVA: 0x00052E23 File Offset: 0x00051023
			public float totalVolume { get; private set; }

			// Token: 0x0600109E RID: 4254 RVA: 0x00052E2C File Offset: 0x0005102C
			public VolumeDistribution(HurtBox[] hurtBoxes)
			{
				this.totalVolume = 0f;
				for (int i = 0; i < hurtBoxes.Length; i++)
				{
					this.totalVolume += hurtBoxes[i].volume;
				}
				this.hurtBoxes = (HurtBox[])hurtBoxes.Clone();
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x0600109F RID: 4255 RVA: 0x00052E80 File Offset: 0x00051080
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

			// Token: 0x040014B6 RID: 5302
			private HurtBox[] hurtBoxes;
		}
	}
}
