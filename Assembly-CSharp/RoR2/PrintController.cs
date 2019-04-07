using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000399 RID: 921
	public class PrintController : MonoBehaviour
	{
		// Token: 0x0600136F RID: 4975 RVA: 0x0005EDE0 File Offset: 0x0005CFE0
		private void Awake()
		{
			if (!this.hasSetupOnce)
			{
				this.materialsList = new List<Material>();
				this.rendererList = new List<Renderer>();
				this.printShader = Resources.Load<Shader>("Shaders/ToonLitCustom");
				this.SetupPrint();
			}
			this.UpdatePrint(0f);
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x0005EE2C File Offset: 0x0005D02C
		private void OnDisable()
		{
			if (this.hasSetupOnce)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					this.materialsList[i].DisableKeyword("PRINT_CUTOFF");
				}
			}
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x0005EE70 File Offset: 0x0005D070
		private void OnEnable()
		{
			if (this.hasSetupOnce)
			{
				for (int i = 0; i < this.materialsList.Count; i++)
				{
					this.materialsList[i].EnableKeyword("PRINT_CUTOFF");
				}
			}
			this.age = 0f;
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x0005EEBC File Offset: 0x0005D0BC
		private void Update()
		{
			this.UpdatePrint(Time.deltaTime);
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x0005EEC9 File Offset: 0x0005D0C9
		public void SetPaused(bool newPaused)
		{
			this.paused = newPaused;
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x0005EED4 File Offset: 0x0005D0D4
		private void UpdatePrint(float deltaTime)
		{
			if (!this.paused)
			{
				this.age += deltaTime;
			}
			float num = this.printCurve.Evaluate(this.age / this.printTime);
			for (int i = 0; i < this.rendererList.Count; i++)
			{
				Renderer renderer = this.rendererList[i];
				this._propBlock = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(this._propBlock);
				this._propBlock.SetFloat("_SliceHeight", num * this.maxPrintHeight + (1f - num) * this.startingPrintHeight);
				this._propBlock.SetFloat("_PrintBias", num * this.maxPrintBias + (1f - num) * this.startingPrintBias);
				if (this.animateFlowmapPower)
				{
					this._propBlock.SetFloat("_FlowHeightPower", num * this.maxFlowmapPower + (1f - num) * this.startingFlowmapPower);
				}
				renderer.SetPropertyBlock(this._propBlock);
			}
			if (this.age >= this.printTime && this.disableWhenFinished)
			{
				base.enabled = false;
				this.age = 0f;
			}
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x0005F000 File Offset: 0x0005D200
		private void SetupPrint()
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
						material.EnableKeyword("PRINT_CUTOFF");
						this.materialsList.Add(material);
						this.rendererList.Add(rendererInfo.renderer);
						rendererInfo.defaultMaterial = material;
						this.characterModel.rendererInfos[i] = rendererInfo;
					}
				}
			}
			else
			{
				foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
				{
					this.rendererList.Add(renderer);
					this.materialsList.Add(renderer.material);
				}
			}
			this.age = 0f;
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x0005F10C File Offset: 0x0005D30C
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

		// Token: 0x04001713 RID: 5907
		public float printTime;

		// Token: 0x04001714 RID: 5908
		public AnimationCurve printCurve;

		// Token: 0x04001715 RID: 5909
		public float startingPrintHeight;

		// Token: 0x04001716 RID: 5910
		public float maxPrintHeight;

		// Token: 0x04001717 RID: 5911
		public float startingPrintBias;

		// Token: 0x04001718 RID: 5912
		public float maxPrintBias;

		// Token: 0x04001719 RID: 5913
		public bool animateFlowmapPower;

		// Token: 0x0400171A RID: 5914
		public float startingFlowmapPower;

		// Token: 0x0400171B RID: 5915
		public float maxFlowmapPower;

		// Token: 0x0400171C RID: 5916
		public bool disableWhenFinished = true;

		// Token: 0x0400171D RID: 5917
		public bool paused;

		// Token: 0x0400171E RID: 5918
		private MaterialPropertyBlock _propBlock;

		// Token: 0x0400171F RID: 5919
		private CharacterModel characterModel;

		// Token: 0x04001720 RID: 5920
		private List<Material> materialsList;

		// Token: 0x04001721 RID: 5921
		private List<Renderer> rendererList;

		// Token: 0x04001722 RID: 5922
		public float age;

		// Token: 0x04001723 RID: 5923
		private Shader printShader;

		// Token: 0x04001724 RID: 5924
		private bool hasSetupOnce;
	}
}
