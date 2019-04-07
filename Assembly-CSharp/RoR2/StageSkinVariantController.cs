using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EB RID: 1003
	public class StageSkinVariantController : MonoBehaviour
	{
		// Token: 0x060015EF RID: 5615 RVA: 0x000690FC File Offset: 0x000672FC
		private void Awake()
		{
			if (SceneInfo.instance)
			{
				for (int i = 0; i < this.stageSkinVariants.Length; i++)
				{
					StageSkinVariantController.StageSkinVariant stageSkinVariant = this.stageSkinVariants[i];
					if (SceneInfo.instance.sceneDef.nameToken == stageSkinVariant.stageNameToken)
					{
						for (int j = 0; j < stageSkinVariant.childObjects.Length; j++)
						{
							stageSkinVariant.childObjects[j].SetActive(true);
						}
						if (stageSkinVariant.replacementRenderInfos.Length != 0)
						{
							this.characterModel.rendererInfos = stageSkinVariant.replacementRenderInfos;
						}
					}
					else
					{
						for (int k = 0; k < stageSkinVariant.childObjects.Length; k++)
						{
							stageSkinVariant.childObjects[k].SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x04001946 RID: 6470
		public StageSkinVariantController.StageSkinVariant[] stageSkinVariants;

		// Token: 0x04001947 RID: 6471
		public CharacterModel characterModel;

		// Token: 0x020003EC RID: 1004
		[Serializable]
		public struct StageSkinVariant
		{
			// Token: 0x04001948 RID: 6472
			public string stageNameToken;

			// Token: 0x04001949 RID: 6473
			public CharacterModel.RendererInfo[] replacementRenderInfos;

			// Token: 0x0400194A RID: 6474
			public GameObject[] childObjects;
		}
	}
}
