using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000612 RID: 1554
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameCharacterSelection : MonoBehaviour
	{
		// Token: 0x060024C9 RID: 9417 RVA: 0x000A0897 File Offset: 0x0009EA97
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x000A08A8 File Offset: 0x0009EAA8
		private void LateUpdate()
		{
			this.image.sprite = this.disabledSprite;
			ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
			for (int i = 0; i < readOnlyLocalPlayersList.Count; i++)
			{
				if (readOnlyLocalPlayersList[i].bodyIndexPreference == BodyCatalog.FindBodyIndex(this.characterBodyPrefab))
				{
					this.image.sprite = this.enabledSprite;
					return;
				}
			}
		}

		// Token: 0x0400228F RID: 8847
		private Image image;

		// Token: 0x04002290 RID: 8848
		public GameObject characterBodyPrefab;

		// Token: 0x04002291 RID: 8849
		public Sprite enabledSprite;

		// Token: 0x04002292 RID: 8850
		public Sprite disabledSprite;
	}
}
