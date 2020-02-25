using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200058A RID: 1418
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ArtifactToggleController : MonoBehaviour
	{
		// Token: 0x060021C2 RID: 8642 RVA: 0x00092159 File Offset: 0x00090359
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x00092168 File Offset: 0x00090368
		private void Start()
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(this.artifactIndex);
			if (artifactDef != null)
			{
				this.image.sprite = artifactDef.smallIconDeselectedSprite;
				this.image.SetNativeSize();
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x000921A0 File Offset: 0x000903A0
		private void LateUpdate()
		{
			ArtifactCatalog.GetArtifactDef(this.artifactIndex);
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x000921AE File Offset: 0x000903AE
		public void ToggleArtifact()
		{
			if (NetworkServer.active)
			{
				PreGameController.instance;
			}
		}

		// Token: 0x04001F24 RID: 7972
		public ArtifactIndex artifactIndex;

		// Token: 0x04001F25 RID: 7973
		private Image image;
	}
}
