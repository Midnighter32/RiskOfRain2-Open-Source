using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B0 RID: 1456
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ArtifactToggleController : MonoBehaviour
	{
		// Token: 0x060020A0 RID: 8352 RVA: 0x0009997C File Offset: 0x00097B7C
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x0009998C File Offset: 0x00097B8C
		private void Start()
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(this.artifactIndex);
			if (artifactDef != null)
			{
				this.image.sprite = artifactDef.smallIconDeselectedSprite;
				this.image.SetNativeSize();
			}
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x000999C4 File Offset: 0x00097BC4
		private void LateUpdate()
		{
			ArtifactCatalog.GetArtifactDef(this.artifactIndex);
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x000999D2 File Offset: 0x00097BD2
		public void ToggleArtifact()
		{
			if (NetworkServer.active)
			{
				PreGameController.instance;
			}
		}

		// Token: 0x0400232E RID: 9006
		public ArtifactIndex artifactIndex;

		// Token: 0x0400232F RID: 9007
		private Image image;
	}
}
