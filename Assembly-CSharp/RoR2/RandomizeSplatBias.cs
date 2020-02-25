using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E1 RID: 737
	public class RandomizeSplatBias : MonoBehaviour
	{
		// Token: 0x060010E6 RID: 4326 RVA: 0x0004A23D File Offset: 0x0004843D
		private void Start()
		{
			this.materialsList = new List<Material>();
			this.rendererList = new List<Renderer>();
			this.printShader = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
			this.Setup();
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0004A26C File Offset: 0x0004846C
		private void Setup()
		{
			this.hasSetupOnce = true;
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel)
			{
				for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
				{
					CharacterModel.RendererInfo rendererInfo = this.characterModel.baseRendererInfos[i];
					Material material = UnityEngine.Object.Instantiate<Material>(rendererInfo.defaultMaterial);
					if (material.shader == this.printShader)
					{
						this.materialsList.Add(material);
						this.rendererList.Add(rendererInfo.renderer);
						rendererInfo.defaultMaterial = material;
						this.characterModel.baseRendererInfos[i] = rendererInfo;
					}
					Renderer renderer = this.rendererList[i];
					this._propBlock = new MaterialPropertyBlock();
					renderer.GetPropertyBlock(this._propBlock);
					this._propBlock.SetFloat("_RedChannelBias", UnityEngine.Random.Range(this.minRedBias, this.maxRedBias));
					this._propBlock.SetFloat("_BlueChannelBias", UnityEngine.Random.Range(this.minBlueBias, this.maxBlueBias));
					this._propBlock.SetFloat("_GreenChannelBias", UnityEngine.Random.Range(this.minGreenBias, this.maxGreenBias));
					renderer.SetPropertyBlock(this._propBlock);
				}
				return;
			}
			Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
			Material material2 = UnityEngine.Object.Instantiate<Material>(componentInChildren.material);
			this.materialsList.Add(material2);
			componentInChildren.material = material2;
			this._propBlock = new MaterialPropertyBlock();
			componentInChildren.GetPropertyBlock(this._propBlock);
			this._propBlock.SetFloat("_RedChannelBias", UnityEngine.Random.Range(this.minRedBias, this.maxRedBias));
			this._propBlock.SetFloat("_BlueChannelBias", UnityEngine.Random.Range(this.minBlueBias, this.maxBlueBias));
			this._propBlock.SetFloat("_GreenChannelBias", UnityEngine.Random.Range(this.minGreenBias, this.maxGreenBias));
			componentInChildren.SetPropertyBlock(this._propBlock);
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0004A460 File Offset: 0x00048660
		private void OnDestroy()
		{
			if (this.materialsList != null)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					UnityEngine.Object.Destroy(this.materialsList[i]);
				}
			}
		}

		// Token: 0x0400103D RID: 4157
		public float minRedBias;

		// Token: 0x0400103E RID: 4158
		public float maxRedBias;

		// Token: 0x0400103F RID: 4159
		public float minGreenBias;

		// Token: 0x04001040 RID: 4160
		public float maxGreenBias;

		// Token: 0x04001041 RID: 4161
		public float minBlueBias;

		// Token: 0x04001042 RID: 4162
		public float maxBlueBias;

		// Token: 0x04001043 RID: 4163
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04001044 RID: 4164
		private CharacterModel characterModel;

		// Token: 0x04001045 RID: 4165
		private List<Material> materialsList;

		// Token: 0x04001046 RID: 4166
		private List<Renderer> rendererList;

		// Token: 0x04001047 RID: 4167
		private Shader printShader;

		// Token: 0x04001048 RID: 4168
		private bool hasSetupOnce;
	}
}
