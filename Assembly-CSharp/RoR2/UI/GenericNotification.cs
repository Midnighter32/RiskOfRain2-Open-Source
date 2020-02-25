using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BF RID: 1471
	public class GenericNotification : MonoBehaviour
	{
		// Token: 0x060022E1 RID: 8929 RVA: 0x00097DD2 File Offset: 0x00095FD2
		private void Start()
		{
			this.age = 0f;
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x00097DE0 File Offset: 0x00095FE0
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

		// Token: 0x060022E3 RID: 8931 RVA: 0x00097E5C File Offset: 0x0009605C
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

		// Token: 0x060022E4 RID: 8932 RVA: 0x00097ECC File Offset: 0x000960CC
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

		// Token: 0x0400208C RID: 8332
		public LanguageTextMeshController titleText;

		// Token: 0x0400208D RID: 8333
		public TextMeshProUGUI titleTMP;

		// Token: 0x0400208E RID: 8334
		public LanguageTextMeshController descriptionText;

		// Token: 0x0400208F RID: 8335
		public RawImage iconImage;

		// Token: 0x04002090 RID: 8336
		public CanvasRenderer[] fadeRenderers;

		// Token: 0x04002091 RID: 8337
		public float duration = 1.5f;

		// Token: 0x04002092 RID: 8338
		public float fadeTime = 0.5f;

		// Token: 0x04002093 RID: 8339
		private float age;
	}
}
