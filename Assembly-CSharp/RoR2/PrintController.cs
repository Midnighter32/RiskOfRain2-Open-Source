using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D6 RID: 726
	public class PrintController : MonoBehaviour
	{
		// Token: 0x0600108D RID: 4237 RVA: 0x000488A8 File Offset: 0x00046AA8
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			PrintController.printShader = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
			PrintController.sliceHeightShaderPropertyId = Shader.PropertyToID("_SliceHeight");
			PrintController.printBiasShaderPropertyId = Shader.PropertyToID("_PrintBias");
			PrintController.flowHeightPowerShaderPropertyId = Shader.PropertyToID("_FlowHeightPower");
			PrintController.printOnPropertyId = Shader.PropertyToID("_PrintOn");
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x00048900 File Offset: 0x00046B00
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
			this._propBlock = new MaterialPropertyBlock();
			this.SetupPrint();
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x0004891F File Offset: 0x00046B1F
		private void OnDisable()
		{
			this.SetMaterialPrintCutoffEnabled(false);
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x00048928 File Offset: 0x00046B28
		private void OnEnable()
		{
			this.SetMaterialPrintCutoffEnabled(true);
			this.age = 0f;
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x0004893C File Offset: 0x00046B3C
		private void Update()
		{
			this.UpdatePrint(Time.deltaTime);
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x0004894C File Offset: 0x00046B4C
		private void OnDestroy()
		{
			for (int i = this.rendererMaterialPairs.Length - 1; i > 0; i--)
			{
				UnityEngine.Object.Destroy(this.rendererMaterialPairs[i].material);
				this.rendererMaterialPairs[i] = new PrintController.RendererMaterialPair(null, null);
			}
			this.rendererMaterialPairs = Array.Empty<PrintController.RendererMaterialPair>();
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000489A4 File Offset: 0x00046BA4
		private void OnValidate()
		{
			if (this.printCurve == null)
			{
				this.printCurve = new AnimationCurve();
			}
			Keyframe[] keys = this.printCurve.keys;
			for (int i = 1; i < keys.Length; i++)
			{
				Keyframe[] array = keys;
				int num = i - 1;
				ref Keyframe ptr = ref keys[i];
				if (array[num].time >= ptr.time)
				{
					Debug.LogErrorFormat("Animation curve error on object {0}", new object[]
					{
						base.gameObject.name
					});
					break;
				}
			}
			if (this.printTime == 0f)
			{
				Debug.LogErrorFormat("printTime==0f on object {0}", new object[]
				{
					base.gameObject.name
				});
			}
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00048A47 File Offset: 0x00046C47
		public void SetPaused(bool newPaused)
		{
			this.paused = newPaused;
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00048A50 File Offset: 0x00046C50
		private void UpdatePrint(float deltaTime)
		{
			if (this.printCurve == null)
			{
				return;
			}
			if (!this.paused)
			{
				this.age += deltaTime;
			}
			float printThreshold = this.printCurve.Evaluate(this.age / this.printTime);
			this.SetPrintThreshold(printThreshold);
			if (this.age >= this.printTime && this.disableWhenFinished)
			{
				base.enabled = false;
				this.age = 0f;
			}
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x00048AC4 File Offset: 0x00046CC4
		private void SetPrintThreshold(float sample)
		{
			float num = 1f - sample;
			float value = sample * this.maxPrintHeight + num * this.startingPrintHeight;
			float value2 = sample * this.maxPrintBias + num * this.startingPrintBias;
			float value3 = sample * this.maxFlowmapPower + num * this.startingFlowmapPower;
			for (int i = 0; i < this.rendererMaterialPairs.Length; i++)
			{
				PrintController.RendererMaterialPair[] array = this.rendererMaterialPairs;
				int num2 = i;
				array[num2].renderer.GetPropertyBlock(this._propBlock);
				this._propBlock.SetFloat(PrintController.sliceHeightShaderPropertyId, value);
				this._propBlock.SetFloat(PrintController.printBiasShaderPropertyId, value2);
				if (this.animateFlowmapPower)
				{
					this._propBlock.SetFloat(PrintController.flowHeightPowerShaderPropertyId, value3);
				}
				array[num2].renderer.SetPropertyBlock(this._propBlock);
			}
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x00048B90 File Offset: 0x00046D90
		private void SetupPrint()
		{
			if (this.hasSetupOnce)
			{
				return;
			}
			this.hasSetupOnce = true;
			if (this.characterModel)
			{
				CharacterModel.RendererInfo[] baseRendererInfos = this.characterModel.baseRendererInfos;
				int num = 0;
				for (int i = 0; i < baseRendererInfos.Length; i++)
				{
					Material defaultMaterial = baseRendererInfos[i].defaultMaterial;
					if (!(((defaultMaterial != null) ? defaultMaterial.shader : null) != PrintController.printShader))
					{
						num++;
					}
				}
				Array.Resize<PrintController.RendererMaterialPair>(ref this.rendererMaterialPairs, num);
				int j = 0;
				int num2 = 0;
				while (j < baseRendererInfos.Length)
				{
					ref CharacterModel.RendererInfo ptr = ref baseRendererInfos[j];
					Material defaultMaterial2 = ptr.defaultMaterial;
					if (!(((defaultMaterial2 != null) ? defaultMaterial2.shader : null) != PrintController.printShader))
					{
						Material material = UnityEngine.Object.Instantiate<Material>(ptr.defaultMaterial);
						ptr.defaultMaterial = material;
						this.rendererMaterialPairs[num2++] = new PrintController.RendererMaterialPair(ptr.renderer, material);
					}
					j++;
				}
			}
			else
			{
				List<Renderer> gameObjectComponentsInChildren = GetComponentsCache<Renderer>.GetGameObjectComponentsInChildren(base.gameObject, true);
				Array.Resize<PrintController.RendererMaterialPair>(ref this.rendererMaterialPairs, gameObjectComponentsInChildren.Count);
				int k = 0;
				int count = gameObjectComponentsInChildren.Count;
				while (k < count)
				{
					Renderer renderer = gameObjectComponentsInChildren[k];
					Material material2 = renderer.material;
					this.rendererMaterialPairs[k] = new PrintController.RendererMaterialPair(renderer, material2);
					k++;
				}
				GetComponentsCache<Renderer>.ReturnBuffer(gameObjectComponentsInChildren);
			}
			this.SetMaterialPrintCutoffEnabled(false);
			this.age = 0f;
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x00048CFC File Offset: 0x00046EFC
		private void SetMaterialPrintCutoffEnabled(bool shouldEnable)
		{
			if (shouldEnable)
			{
				this.EnableMaterialPrintCutoff();
				return;
			}
			this.DisableMaterialPrintCutoff();
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00048D10 File Offset: 0x00046F10
		private void EnableMaterialPrintCutoff()
		{
			for (int i = 0; i < this.rendererMaterialPairs.Length; i++)
			{
				Material material = this.rendererMaterialPairs[i].material;
				material.EnableKeyword("PRINT_CUTOFF");
				material.SetInt(PrintController.printOnPropertyId, 1);
			}
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x00048D58 File Offset: 0x00046F58
		private void DisableMaterialPrintCutoff()
		{
			for (int i = 0; i < this.rendererMaterialPairs.Length; i++)
			{
				Material material = this.rendererMaterialPairs[i].material;
				material.DisableKeyword("PRINT_CUTOFF");
				material.SetInt(PrintController.printOnPropertyId, 0);
			}
			this.SetPrintThreshold(1f);
		}

		// Token: 0x04000FE3 RID: 4067
		[Header("Print Time and Behaviors")]
		public float printTime;

		// Token: 0x04000FE4 RID: 4068
		public AnimationCurve printCurve;

		// Token: 0x04000FE5 RID: 4069
		public float age;

		// Token: 0x04000FE6 RID: 4070
		public bool disableWhenFinished = true;

		// Token: 0x04000FE7 RID: 4071
		public bool paused;

		// Token: 0x04000FE8 RID: 4072
		[Header("Print Start/End Values")]
		public float startingPrintHeight;

		// Token: 0x04000FE9 RID: 4073
		public float maxPrintHeight;

		// Token: 0x04000FEA RID: 4074
		public float startingPrintBias;

		// Token: 0x04000FEB RID: 4075
		public float maxPrintBias;

		// Token: 0x04000FEC RID: 4076
		public bool animateFlowmapPower;

		// Token: 0x04000FED RID: 4077
		public float startingFlowmapPower;

		// Token: 0x04000FEE RID: 4078
		public float maxFlowmapPower;

		// Token: 0x04000FEF RID: 4079
		private CharacterModel characterModel;

		// Token: 0x04000FF0 RID: 4080
		private MaterialPropertyBlock _propBlock;

		// Token: 0x04000FF1 RID: 4081
		private PrintController.RendererMaterialPair[] rendererMaterialPairs = Array.Empty<PrintController.RendererMaterialPair>();

		// Token: 0x04000FF2 RID: 4082
		private bool hasSetupOnce;

		// Token: 0x04000FF3 RID: 4083
		private static Shader printShader;

		// Token: 0x04000FF4 RID: 4084
		private static int sliceHeightShaderPropertyId;

		// Token: 0x04000FF5 RID: 4085
		private static int printBiasShaderPropertyId;

		// Token: 0x04000FF6 RID: 4086
		private static int flowHeightPowerShaderPropertyId;

		// Token: 0x04000FF7 RID: 4087
		private static int printOnPropertyId;

		// Token: 0x020002D7 RID: 727
		private struct RendererMaterialPair
		{
			// Token: 0x0600109C RID: 4252 RVA: 0x00048DC4 File Offset: 0x00046FC4
			public RendererMaterialPair(Renderer renderer, Material material)
			{
				this.renderer = renderer;
				this.material = material;
			}

			// Token: 0x04000FF8 RID: 4088
			public readonly Renderer renderer;

			// Token: 0x04000FF9 RID: 4089
			public readonly Material material;
		}
	}
}
