using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000103 RID: 259
	[CreateAssetMenu]
	public class DirectorCardCategorySelection : ScriptableObject
	{
		// Token: 0x060004EA RID: 1258 RVA: 0x00013BDC File Offset: 0x00011DDC
		public float SumAllWeightsInCategory(DirectorCardCategorySelection.Category category)
		{
			float num = 0f;
			for (int i = 0; i < category.cards.Length; i++)
			{
				num += (float)category.cards[i].selectionWeight;
			}
			return num;
		}

		// Token: 0x040004B3 RID: 1203
		public DirectorCardCategorySelection.Category[] categories;

		// Token: 0x02000104 RID: 260
		[Serializable]
		public struct Category
		{
			// Token: 0x040004B4 RID: 1204
			[Tooltip("A name to help identify this category")]
			public string name;

			// Token: 0x040004B5 RID: 1205
			public DirectorCard[] cards;

			// Token: 0x040004B6 RID: 1206
			public float selectionWeight;
		}
	}
}
