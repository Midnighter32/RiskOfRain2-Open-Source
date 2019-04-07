using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000624 RID: 1572
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class PregameDifficultySelection : MonoBehaviour
	{
		// Token: 0x0600234D RID: 9037 RVA: 0x000A6528 File Offset: 0x000A4728
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x00004507 File Offset: 0x00002707
		private void LateUpdate()
		{
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x000A6536 File Offset: 0x000A4736
		public void SetCharacterSelectControllerDifficulty()
		{
			bool active = NetworkServer.active;
		}

		// Token: 0x0400264A RID: 9802
		private Image image;

		// Token: 0x0400264B RID: 9803
		public DifficultyIndex difficulty;

		// Token: 0x0400264C RID: 9804
		public Sprite enabledSprite;

		// Token: 0x0400264D RID: 9805
		public Sprite disabledSprite;
	}
}
