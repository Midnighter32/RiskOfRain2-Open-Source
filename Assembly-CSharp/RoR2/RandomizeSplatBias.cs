using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A4 RID: 932
	public class RandomizeSplatBias : MonoBehaviour
	{
		// Token: 0x060013BF RID: 5055 RVA: 0x00060859 File Offset: 0x0005EA59
		private void Start()
		{
			this.materialsList = new List<Material>();
			this.rendererList = new List<Renderer>();
			this.printShader = Resources.Load<Shader>("Shaders/ToonLitCustom");
			this.Setup();
		}

		// Token: 0x060013C0 RID: 5056 RVA: 0x00060888 File Offset: 0x0005EA88
		private void Setup()
		{
			this.hasSetupOnce = true;
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel)
			{
				for (int i = 0; i < this.characterModel.rendererInfos.Length; i++)
				{
					CharacterModel.RendererInfo rendererInfo = this.characterModel.rendererInfos[i];
					Material material = UnityEngine.Object.Instantiate<Material>(rendererInfo.defaultMaterial);
					if (material.shader == this.printShader)
					{
						this.materialsList.Add(material);
						this.rendererList.Add(rendererInfo.renderer);
						rendererInfo.defaultMaterial = material;
						this.characterModel.rendererInfos[i] = rendererInfo;
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

		// Token: 0x060013C1 RID: 5057 RVA: 0x00060A7C File Offset: 0x0005EC7C
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

		// Token: 0x0400176E RID: 5998
		public float minRedBias;

		// Token: 0x0400176F RID: 5999
		public float maxRedBias;

		// Token: 0x04001770 RID: 6000
		public float minGreenBias;

		// Token: 0x04001771 RID: 6001
		public float maxGreenBias;

		// Token: 0x04001772 RID: 6002
		public float minBlueBias;

		// Token: 0x04001773 RID: 6003
		public float maxBlueBias;

		// Token: 0x04001774 RID: 6004
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04001775 RID: 6005
		private CharacterModel characterModel;

		// Token: 0x04001776 RID: 6006
		private List<Material> materialsList;

		// Token: 0x04001777 RID: 6007
		private List<Renderer> rendererList;

		// Token: 0x04001778 RID: 6008
		private Shader printShader;

		// Token: 0x04001779 RID: 6009
		private bool hasSetupOnce;
	}
}
