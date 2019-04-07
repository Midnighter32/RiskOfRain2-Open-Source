using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AD RID: 1453
	public class AllyCardController : MonoBehaviour
	{
		// Token: 0x06002095 RID: 8341 RVA: 0x00099645 File Offset: 0x00097845
		private void LateUpdate()
		{
			this.UpdateInfo();
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x00099650 File Offset: 0x00097850
		private void UpdateInfo()
		{
			HealthComponent source = null;
			string text = "";
			Texture texture = null;
			if (this.sourceGameObject)
			{
				source = this.sourceGameObject.GetComponent<HealthComponent>();
				text = Util.GetBestBodyName(this.sourceGameObject);
				CharacterBody component = this.sourceGameObject.GetComponent<CharacterBody>();
				if (component)
				{
					texture = component.portraitIcon;
				}
			}
			this.healthBar.source = source;
			this.nameLabel.text = text;
			this.portraitIconImage.texture = texture;
		}

		// Token: 0x0400231E RID: 8990
		public HealthBar healthBar;

		// Token: 0x0400231F RID: 8991
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002320 RID: 8992
		public RawImage portraitIconImage;

		// Token: 0x04002321 RID: 8993
		[HideInInspector]
		public GameObject sourceGameObject;
	}
}
