using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B6 RID: 1462
	[RequireComponent(typeof(RectTransform))]
	public class BuffDisplay : MonoBehaviour
	{
		// Token: 0x060020C0 RID: 8384 RVA: 0x00099E29 File Offset: 0x00098029
		private void Awake()
		{
			this.rectTranform = base.GetComponent<RectTransform>();
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x00099E38 File Offset: 0x00098038
		private void AllocateIcons()
		{
			int num = 0;
			if (this.source)
			{
				for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
				{
					if (this.source.HasBuff(buffIndex))
					{
						num++;
					}
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

		// Token: 0x060020C2 RID: 8386 RVA: 0x00099F08 File Offset: 0x00098108
		private void UpdateLayout()
		{
			this.AllocateIcons();
			float width = this.rectTranform.rect.width;
			if (this.source)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
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
				}
			}
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x00099FAA File Offset: 0x000981AA
		private void Update()
		{
			this.UpdateLayout();
		}

		// Token: 0x04002346 RID: 9030
		private RectTransform rectTranform;

		// Token: 0x04002347 RID: 9031
		public CharacterBody source;

		// Token: 0x04002348 RID: 9032
		public GameObject buffIconPrefab;

		// Token: 0x04002349 RID: 9033
		public float iconWidth = 24f;

		// Token: 0x0400234A RID: 9034
		[SerializeField]
		[HideInInspector]
		private List<BuffIcon> buffIcons;
	}
}
