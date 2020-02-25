using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000340 RID: 832
	public class StageSkinVariantController : MonoBehaviour
	{
		// Token: 0x060013E5 RID: 5093 RVA: 0x00055028 File Offset: 0x00053228
		private void Awake()
		{
			if (SceneInfo.instance)
			{
				for (int i = 0; i < this.stageSkinVariants.Length; i++)
				{
					StageSkinVariantController.StageSkinVariant stageSkinVariant = this.stageSkinVariants[i];
					for (int j = 0; j < stageSkinVariant.childObjects.Length; j++)
					{
						stageSkinVariant.childObjects[j].SetActive(false);
					}
				}
				int k = 0;
				while (k < this.stageSkinVariants.Length)
				{
					StageSkinVariantController.StageSkinVariant stageSkinVariant2 = this.stageSkinVariants[k];
					if (SceneInfo.instance.sceneDef.nameToken == stageSkinVariant2.stageNameToken)
					{
						for (int l = 0; l < stageSkinVariant2.childObjects.Length; l++)
						{
							stageSkinVariant2.childObjects[l].SetActive(true);
						}
						if (stageSkinVariant2.replacementRenderInfos.Length != 0)
						{
							this.characterModel.baseRendererInfos = stageSkinVariant2.replacementRenderInfos;
							return;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
		}

		// Token: 0x040012A1 RID: 4769
		public StageSkinVariantController.StageSkinVariant[] stageSkinVariants;

		// Token: 0x040012A2 RID: 4770
		public CharacterModel characterModel;

		// Token: 0x02000341 RID: 833
		[Serializable]
		public struct StageSkinVariant
		{
			// Token: 0x040012A3 RID: 4771
			public string stageNameToken;

			// Token: 0x040012A4 RID: 4772
			public CharacterModel.RendererInfo[] replacementRenderInfos;

			// Token: 0x040012A5 RID: 4773
			public GameObject[] childObjects;
		}
	}
}
