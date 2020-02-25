using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000590 RID: 1424
	[RequireComponent(typeof(RectTransform))]
	public class BuffDisplay : MonoBehaviour
	{
		// Token: 0x060021E2 RID: 8674 RVA: 0x00092605 File Offset: 0x00090805
		private void Awake()
		{
			this.rectTranform = base.GetComponent<RectTransform>();
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x00092614 File Offset: 0x00090814
		private void AllocateIcons()
		{
			int num = 0;
			if (this.source)
			{
				BuffIndex buffIndex = BuffIndex.Slow50;
				BuffIndex buffCount = (BuffIndex)BuffCatalog.buffCount;
				while (buffIndex < buffCount)
				{
					if (this.source.HasBuff(buffIndex))
					{
						num++;
					}
					buffIndex++;
				}
			}
			if (num != this.buffIcons.Count)
			{
				while (this.buffIcons.Count > num)
				{
					UnityEngine.Object.Destroy(this.buffIcons[this.buffIcons.Count - 1].gameObject);
					this.buffIcons.RemoveAt(this.buffIcons.Count - 1);
				}
				while (this.buffIcons.Count < num)
				{
					BuffIcon component = UnityEngine.Object.Instantiate<GameObject>(this.buffIconPrefab, this.rectTranform).GetComponent<BuffIcon>();
					this.buffIcons.Add(component);
				}
				this.UpdateLayout();
			}
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x000926E8 File Offset: 0x000908E8
		private void UpdateLayout()
		{
			this.AllocateIcons();
			float width = this.rectTranform.rect.width;
			if (this.source)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				BuffIndex buffIndex = BuffIndex.Slow50;
				BuffIndex buffCount = (BuffIndex)BuffCatalog.buffCount;
				while (buffIndex < buffCount)
				{
					if (this.source.HasBuff(buffIndex))
					{
						BuffIcon buffIcon = this.buffIcons[num];
						buffIcon.buffIndex = buffIndex;
						buffIcon.rectTransform.anchoredPosition = zero;
						buffIcon.buffCount = this.source.GetBuffCount(buffIndex);
						zero.x += this.iconWidth;
						buffIcon.UpdateIcon();
						num++;
					}
					buffIndex++;
				}
			}
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x00092791 File Offset: 0x00090991
		private void Update()
		{
			this.UpdateLayout();
		}

		// Token: 0x04001F3C RID: 7996
		private RectTransform rectTranform;

		// Token: 0x04001F3D RID: 7997
		public CharacterBody source;

		// Token: 0x04001F3E RID: 7998
		public GameObject buffIconPrefab;

		// Token: 0x04001F3F RID: 7999
		public float iconWidth = 24f;

		// Token: 0x04001F40 RID: 8000
		[HideInInspector]
		[SerializeField]
		private List<BuffIcon> buffIcons;
	}
}
