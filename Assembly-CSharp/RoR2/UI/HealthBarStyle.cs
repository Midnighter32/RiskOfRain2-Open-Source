using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000572 RID: 1394
	[CreateAssetMenu(menuName = "RoR2/UI/HealthBarStyle")]
	public class HealthBarStyle : ScriptableObject
	{
		// Token: 0x04001E95 RID: 7829
		public GameObject barPrefab;

		// Token: 0x04001E96 RID: 7830
		public bool flashOnHealthCritical;

		// Token: 0x04001E97 RID: 7831
		public HealthBarStyle.BarStyle trailingBarStyle;

		// Token: 0x04001E98 RID: 7832
		public HealthBarStyle.BarStyle healthBarStyle;

		// Token: 0x04001E99 RID: 7833
		public HealthBarStyle.BarStyle shieldBarStyle;

		// Token: 0x04001E9A RID: 7834
		public HealthBarStyle.BarStyle curseBarStyle;

		// Token: 0x04001E9B RID: 7835
		public HealthBarStyle.BarStyle barrierBarStyle;

		// Token: 0x04001E9C RID: 7836
		public HealthBarStyle.BarStyle flashBarStyle;

		// Token: 0x04001E9D RID: 7837
		public HealthBarStyle.BarStyle cullBarStyle;

		// Token: 0x02000573 RID: 1395
		[Serializable]
		public struct BarStyle
		{
			// Token: 0x04001E9E RID: 7838
			public bool enabled;

			// Token: 0x04001E9F RID: 7839
			public Color baseColor;

			// Token: 0x04001EA0 RID: 7840
			public Sprite sprite;

			// Token: 0x04001EA1 RID: 7841
			public Image.Type imageType;

			// Token: 0x04001EA2 RID: 7842
			public float sizeDelta;
		}
	}
}
