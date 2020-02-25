using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000611 RID: 1553
	[RequireComponent(typeof(RectTransform))]
	public class PregameArtifactBarController : MonoBehaviour
	{
		// Token: 0x060024C7 RID: 9415 RVA: 0x000A0860 File Offset: 0x0009EA60
		private void Start()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.artifactTogglePrefab, component).GetComponent<ArtifactToggleController>().artifactIndex = artifactIndex;
			}
		}

		// Token: 0x0400228E RID: 8846
		public GameObject artifactTogglePrefab;
	}
}
