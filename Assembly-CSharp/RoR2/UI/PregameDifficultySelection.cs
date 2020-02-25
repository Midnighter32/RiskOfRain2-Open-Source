using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000613 RID: 1555
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class PregameDifficultySelection : MonoBehaviour
	{
		// Token: 0x060024CC RID: 9420 RVA: 0x000A0908 File Offset: 0x0009EB08
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x0000409B File Offset: 0x0000229B
		private void LateUpdate()
		{
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x000A0916 File Offset: 0x0009EB16
		public void SetCharacterSelectControllerDifficulty()
		{
			bool active = NetworkServer.active;
		}

		// Token: 0x04002293 RID: 8851
		private Image image;

		// Token: 0x04002294 RID: 8852
		public DifficultyIndex difficulty;

		// Token: 0x04002295 RID: 8853
		public Sprite enabledSprite;

		// Token: 0x04002296 RID: 8854
		public Sprite disabledSprite;
	}
}
