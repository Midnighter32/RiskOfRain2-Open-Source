using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000580 RID: 1408
	[RequireComponent(typeof(LayoutElement))]
	[RequireComponent(typeof(RectTransform))]
	public class AllyCardController : MonoBehaviour
	{
		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06002185 RID: 8581 RVA: 0x000911FD File Offset: 0x0008F3FD
		// (set) Token: 0x06002186 RID: 8582 RVA: 0x00091205 File Offset: 0x0008F405
		public bool shouldIndent { get; set; }

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06002187 RID: 8583 RVA: 0x0009120E File Offset: 0x0008F40E
		// (set) Token: 0x06002188 RID: 8584 RVA: 0x00091216 File Offset: 0x0008F416
		public CharacterMaster sourceMaster { get; set; }

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06002189 RID: 8585 RVA: 0x0009121F File Offset: 0x0008F41F
		// (set) Token: 0x0600218A RID: 8586 RVA: 0x00091227 File Offset: 0x0008F427
		public RectTransform rectTransform { get; private set; }

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600218B RID: 8587 RVA: 0x00091230 File Offset: 0x0008F430
		// (set) Token: 0x0600218C RID: 8588 RVA: 0x00091238 File Offset: 0x0008F438
		public LayoutElement layoutElement { get; private set; }

		// Token: 0x0600218D RID: 8589 RVA: 0x00091241 File Offset: 0x0008F441
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.layoutElement = base.GetComponent<LayoutElement>();
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x00091260 File Offset: 0x0008F460
		private void LateUpdate()
		{
			this.UpdateInfo();
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x00091268 File Offset: 0x0008F468
		private void UpdateInfo()
		{
			HealthComponent source = null;
			string text = "";
			Texture texture = null;
			if (this.sourceMaster)
			{
				CharacterBody body = this.sourceMaster.GetBody();
				if (body)
				{
					texture = body.portraitIcon;
					source = body.healthComponent;
					text = Util.GetBestBodyName(body.gameObject);
				}
				else
				{
					text = Util.GetBestMasterName(this.sourceMaster);
				}
			}
			this.healthBar.source = source;
			this.nameLabel.text = text;
			this.portraitIconImage.texture = texture;
			this.portraitIconImage.enabled = (texture != null);
		}

		// Token: 0x04001EF1 RID: 7921
		public HealthBar healthBar;

		// Token: 0x04001EF2 RID: 7922
		public TextMeshProUGUI nameLabel;

		// Token: 0x04001EF3 RID: 7923
		public RawImage portraitIconImage;
	}
}
