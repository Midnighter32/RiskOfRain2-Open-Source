using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000223 RID: 547
	[CreateAssetMenu]
	public class DirectorCardCategorySelection : ScriptableObject
	{
		// Token: 0x06000AA3 RID: 2723 RVA: 0x00034A54 File Offset: 0x00032C54
		public float SumAllWeightsInCategory(DirectorCardCategorySelection.Category category)
		{
			float num = 0f;
			for (int i = 0; i < category.cards.Length; i++)
			{
				num += (float)category.cards[i].selectionWeight;
			}
			return num;
		}

		// Token: 0x04000E0A RID: 3594
		public DirectorCardCategorySelection.Category[] categories;

		// Token: 0x02000224 RID: 548
		[Serializable]
		public struct Category
		{
			// Token: 0x04000E0B RID: 3595
			[Tooltip("A name to help identify this category")]
			public string name;

			// Token: 0x04000E0C RID: 3596
			public DirectorCard[] cards;

			// Token: 0x04000E0D RID: 3597
			public float selectionWeight;
		}
	}
}
