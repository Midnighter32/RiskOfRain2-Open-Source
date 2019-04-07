using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E0 RID: 1504
	public class GenericNotification : MonoBehaviour
	{
		// Token: 0x060021B1 RID: 8625 RVA: 0x0009ED8E File Offset: 0x0009CF8E
		private void Start()
		{
			this.age = 0f;
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x0009ED9C File Offset: 0x0009CF9C
		private void Update()
		{
			this.age += Time.deltaTime;
			float num = Mathf.Clamp01((this.age - (this.duration - this.fadeTime)) / this.fadeTime);
			float alpha = 1f - num;
			for (int i = 0; i < this.fadeRenderers.Length; i++)
			{
				this.fadeRenderers[i].SetAlpha(alpha);
			}
			if (num >= 1f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x0009EE18 File Offset: 0x0009D018
		public void SetItem(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			this.titleText.token = itemDef.nameToken;
			this.descriptionText.token = itemDef.pickupToken;
			if (itemDef.pickupIconPath != null)
			{
				this.iconImage.texture = Resources.Load<Texture>(itemDef.pickupIconPath);
			}
			this.titleTMP.color = ColorCatalog.GetColor(itemDef.colorIndex);
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x0009EE88 File Offset: 0x0009D088
		public void SetEquipment(EquipmentIndex equipmentIndex)
		{
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			this.titleText.token = equipmentDef.nameToken;
			this.descriptionText.token = equipmentDef.pickupToken;
			if (equipmentDef.pickupIconPath != null)
			{
				this.iconImage.texture = Resources.Load<Texture>(equipmentDef.pickupIconPath);
			}
			this.titleTMP.color = ColorCatalog.GetColor(equipmentDef.colorIndex);
		}

		// Token: 0x0400247B RID: 9339
		public LanguageTextMeshController titleText;

		// Token: 0x0400247C RID: 9340
		public TextMeshProUGUI titleTMP;

		// Token: 0x0400247D RID: 9341
		public LanguageTextMeshController descriptionText;

		// Token: 0x0400247E RID: 9342
		public RawImage iconImage;

		// Token: 0x0400247F RID: 9343
		public CanvasRenderer[] fadeRenderers;

		// Token: 0x04002480 RID: 9344
		public float duration = 1.5f;

		// Token: 0x04002481 RID: 9345
		public float fadeTime = 0.5f;

		// Token: 0x04002482 RID: 9346
		private float age;
	}
}
