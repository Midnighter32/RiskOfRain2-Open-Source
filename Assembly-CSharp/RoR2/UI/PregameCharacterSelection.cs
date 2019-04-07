using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000623 RID: 1571
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class PregameCharacterSelection : MonoBehaviour
	{
		// Token: 0x0600234A RID: 9034 RVA: 0x000A64B7 File Offset: 0x000A46B7
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x000A64C8 File Offset: 0x000A46C8
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

		// Token: 0x04002646 RID: 9798
		private Image image;

		// Token: 0x04002647 RID: 9799
		public GameObject characterBodyPrefab;

		// Token: 0x04002648 RID: 9800
		public Sprite enabledSprite;

		// Token: 0x04002649 RID: 9801
		public Sprite disabledSprite;
	}
}
