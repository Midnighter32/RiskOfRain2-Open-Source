using System;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
	// Token: 0x0200090A RID: 2314
	[AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class TranslucentImageSource : MonoBehaviour
	{
		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x060033A6 RID: 13222 RVA: 0x000E03A2 File Offset: 0x000DE5A2
		// (set) Token: 0x060033A7 RID: 13223 RVA: 0x000E03AA File Offset: 0x000DE5AA
		public RenderTexture BlurredScreen { get; private set; }

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x060033A8 RID: 13224 RVA: 0x000E03B4 File Offset: 0x000DE5B4
		private Camera Cam
		{
			get
			{
				if (!this.camera)
				{
					return this.camera = base.GetComponent<Camera>();
				}
				return this.camera;
			}
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x060033A9 RID: 13225 RVA: 0x000E03E4 File Offset: 0x000DE5E4
		// (set) Token: 0x060033AA RID: 13226 RVA: 0x000E0419 File Offset: 0x000DE619
		public float Strength
		{
			get
			{
				return this.strength = this.Size * Mathf.Pow(2f, (float)(this.Iteration + this.Downsample));
			}
			set
			{
				this.strength = Mathf.Max(0f, value);
				this.SetAdvancedFieldFromSimple();
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x060033AB RID: 13227 RVA: 0x000E0432 File Offset: 0x000DE632
		// (set) Token: 0x060033AC RID: 13228 RVA: 0x000E043A File Offset: 0x000DE63A
		public float Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = Mathf.Max(0f, value);
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x060033AD RID: 13229 RVA: 0x000E044D File Offset: 0x000DE64D
		// (set) Token: 0x060033AE RID: 13230 RVA: 0x000E0455 File Offset: 0x000DE655
		public int Iteration
		{
			get
			{
				return this.iteration;
			}
			set
			{
				this.iteration = Mathf.Max(0, value);
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x060033AF RID: 13231 RVA: 0x000E0464 File Offset: 0x000DE664
		// (set) Token: 0x060033B0 RID: 13232 RVA: 0x000E046C File Offset: 0x000DE66C
		public int Downsample
		{
			get
			{
				return this.downsample;
			}
			set
			{
				this.downsample = Mathf.Max(0, value);
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x060033B1 RID: 13233 RVA: 0x000E047B File Offset: 0x000DE67B
		// (set) Token: 0x060033B2 RID: 13234 RVA: 0x000E0484 File Offset: 0x000DE684
		public Rect BlurRegion
		{
			get
			{
				return this.blurRegion;
			}
			set
			{
				Vector2 vector = new Vector2(1f / (float)this.Cam.pixelWidth, 1f / (float)this.Cam.pixelHeight);
				value.x = Mathf.Clamp(value.x, 0f, 1f - vector.x);
				value.y = Mathf.Clamp(value.y, 0f, 1f - vector.y);
				value.width = Mathf.Clamp(value.width, vector.x, 1f - value.x);
				value.height = Mathf.Clamp(value.height, vector.y, 1f - value.y);
				this.blurRegion = value;
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x060033B3 RID: 13235 RVA: 0x000E0557 File Offset: 0x000DE757
		// (set) Token: 0x060033B4 RID: 13236 RVA: 0x000E055F File Offset: 0x000DE75F
		public int MaxDepth
		{
			get
			{
				return this.maxDepth;
			}
			set
			{
				this.maxDepth = Mathf.Max(1, value);
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x060033B5 RID: 13237 RVA: 0x000E056E File Offset: 0x000DE76E
		private float ScreenSize
		{
			get
			{
				return (float)Mathf.Min(this.Cam.pixelWidth, this.Cam.pixelHeight) / 1080f;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x060033B6 RID: 13238 RVA: 0x000E0592 File Offset: 0x000DE792
		private float MinUpdateCycle
		{
			get
			{
				if (this.maxUpdateRate <= 0f)
				{
					return float.PositiveInfinity;
				}
				return 1f / this.maxUpdateRate;
			}
		}

		// Token: 0x060033B7 RID: 13239 RVA: 0x000E05B4 File Offset: 0x000DE7B4
		protected virtual void SetAdvancedFieldFromSimple()
		{
			this.Size = this.strength / Mathf.Pow(2f, (float)(this.Iteration + this.Downsample));
			if (this.Size < 1f)
			{
				if (this.Downsample > 0)
				{
					int num = this.Downsample;
					this.Downsample = num - 1;
					this.Size *= 2f;
				}
				else if (this.Iteration > 0)
				{
					int num = this.Iteration;
					this.Iteration = num - 1;
					this.Size *= 2f;
				}
			}
			while (this.Size > 8f)
			{
				this.Size /= 2f;
				int num = this.Iteration;
				this.Iteration = num + 1;
			}
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x000E0680 File Offset: 0x000DE880
		protected virtual void Start()
		{
			this.camera = this.Cam;
			this.shader = Shader.Find("Hidden/EfficientBlur");
			if (!this.shader.isSupported || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default))
			{
				base.enabled = false;
				return;
			}
			this.material = new Material(this.shader);
			this.previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));
			TranslucentImageSource._sizePropId = Shader.PropertyToID("size");
			TranslucentImageSource._cropRegionPropId = Shader.PropertyToID("_CropRegion");
			this.CreateNewBlurredScreen();
			this.lastDownsample = this.Downsample;
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x000E071C File Offset: 0x000DE91C
		protected virtual void CreateNewBlurredScreen()
		{
			this.BlurredScreen = new RenderTexture(Mathf.RoundToInt((float)this.Cam.pixelWidth * this.BlurRegion.width) >> this.Downsample, Mathf.RoundToInt((float)this.Cam.pixelHeight * this.BlurRegion.height) >> this.Downsample, 0)
			{
				filterMode = FilterMode.Bilinear
			};
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x000E0790 File Offset: 0x000DE990
		protected virtual void ProgressiveResampling(int level, ref RenderTexture target)
		{
			level = Mathf.Min(level + this.Downsample, this.MaxDepth);
			int width = this.BlurredScreen.width >> level;
			int height = this.BlurredScreen.height >> level;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, this.BlurredScreen.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(target, temporary, this.material, 0);
			RenderTexture.ReleaseTemporary(target);
			target = temporary;
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x000E0808 File Offset: 0x000DEA08
		protected virtual void ProgressiveBlur(RenderTexture sourceRt)
		{
			if (this.Downsample != this.lastDownsample || !this.BlurRegion.Equals(this.lastBlurRegion))
			{
				this.CreateNewBlurredScreen();
				this.lastDownsample = this.Downsample;
				this.lastBlurRegion = this.BlurRegion;
			}
			if (this.BlurredScreen.IsCreated())
			{
				this.BlurredScreen.DiscardContents();
			}
			this.material.SetFloat(TranslucentImageSource._sizePropId, this.Size * this.ScreenSize);
			int num = (this.iteration > 0) ? 1 : 0;
			int width = this.BlurredScreen.width >> num;
			int height = this.BlurredScreen.height >> num;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, sourceRt.format);
			temporary.filterMode = FilterMode.Bilinear;
			sourceRt.filterMode = FilterMode.Bilinear;
			this.material.SetVector(TranslucentImageSource._cropRegionPropId, new Vector4(this.BlurRegion.xMin, this.BlurRegion.yMin, this.BlurRegion.xMax, this.BlurRegion.yMax));
			Graphics.Blit(sourceRt, temporary, this.material, 1);
			for (int i = 2; i <= this.iteration; i++)
			{
				this.ProgressiveResampling(i, ref temporary);
			}
			for (int j = this.iteration - 1; j >= 1; j--)
			{
				this.ProgressiveResampling(j, ref temporary);
			}
			Graphics.Blit(temporary, this.BlurredScreen, this.material, 0);
			RenderTexture.ReleaseTemporary(temporary);
		}

		// Token: 0x060033BC RID: 13244 RVA: 0x000E098C File Offset: 0x000DEB8C
		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Time.unscaledTime - this.lastUpdate >= this.MinUpdateCycle)
			{
				this.ProgressiveBlur(source);
				this.lastUpdate = Time.unscaledTime;
			}
			if (this.preview)
			{
				this.previewMaterial.SetVector("_CropRegion", new Vector4(this.BlurRegion.xMin, this.BlurRegion.yMin, this.BlurRegion.xMax, this.BlurRegion.yMax));
				Graphics.Blit(this.BlurredScreen, destination, this.previewMaterial);
				return;
			}
			Graphics.Blit(source, destination);
		}

		// Token: 0x0400332E RID: 13102
		public float maxUpdateRate = float.PositiveInfinity;

		// Token: 0x0400332F RID: 13103
		[Tooltip("Preview the effect on entire screen")]
		public bool preview;

		// Token: 0x04003330 RID: 13104
		[SerializeField]
		private float size = 5f;

		// Token: 0x04003331 RID: 13105
		[SerializeField]
		private int iteration = 4;

		// Token: 0x04003332 RID: 13106
		[SerializeField]
		private Rect blurRegion = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04003333 RID: 13107
		private Rect lastBlurRegion = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04003334 RID: 13108
		[SerializeField]
		private int maxDepth = 4;

		// Token: 0x04003335 RID: 13109
		[SerializeField]
		private int downsample;

		// Token: 0x04003336 RID: 13110
		[SerializeField]
		private int lastDownsample;

		// Token: 0x04003337 RID: 13111
		[SerializeField]
		private float strength;

		// Token: 0x04003338 RID: 13112
		private float lastUpdate;

		// Token: 0x04003339 RID: 13113
		private Camera camera;

		// Token: 0x0400333A RID: 13114
		private Shader shader;

		// Token: 0x0400333B RID: 13115
		private Material material;

		// Token: 0x0400333C RID: 13116
		private Material previewMaterial;

		// Token: 0x0400333E RID: 13118
		private static int _sizePropId;

		// Token: 0x0400333F RID: 13119
		private static int _cropRegionPropId;
	}
}
