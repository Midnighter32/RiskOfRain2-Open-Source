using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000622 RID: 1570
	[RequireComponent(typeof(RectTransform))]
	public class PregameArtifactBarController : MonoBehaviour
	{
		// Token: 0x06002348 RID: 9032 RVA: 0x000A6480 File Offset: 0x000A4680
		private void Start()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.artifactTogglePrefab, component).GetComponent<ArtifactToggleController>().artifactIndex = artifactIndex;
			}
		}

		// Token: 0x04002645 RID: 9797
		public GameObject artifactTogglePrefab;
	}
}
