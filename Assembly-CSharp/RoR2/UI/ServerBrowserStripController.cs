using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000627 RID: 1575
	[RequireComponent(typeof(RectTransform))]
	public class ServerBrowserStripController : MonoBehaviour
	{
		// Token: 0x0600253E RID: 9534 RVA: 0x000A236E File Offset: 0x000A056E
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
		}

		// Token: 0x040022F2 RID: 8946
		private RectTransform rectTransform;

		// Token: 0x040022F3 RID: 8947
		public HGTextMeshProUGUI nameLabel;

		// Token: 0x040022F4 RID: 8948
		public HGTextMeshProUGUI addressLabel;

		// Token: 0x040022F5 RID: 8949
		public HGTextMeshProUGUI playerCountLabel;

		// Token: 0x040022F6 RID: 8950
		public HGTextMeshProUGUI latencyLabel;
	}
}
