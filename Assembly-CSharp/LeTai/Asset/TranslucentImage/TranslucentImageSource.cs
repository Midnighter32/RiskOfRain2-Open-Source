using System;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
	// Token: 0x020006BF RID: 1727
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
	public class TranslucentImageSource : MonoBehaviour
	{
		// Token: 0x17000330 RID: 816
		// (get) Token: 0x0600265C RID: 9820 RVA: 0x000B0F62 File Offset: 0x000AF162
		// (set) Token: 0x0600265D RID: 9821 RVA: 0x000B0F6A File Offset: 0x000AF16A
		public RenderTexture BlurredScreen { get; private set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x0600265E RID: 9822 RVA: 0x000B0F74 File Offset: 0x000AF174
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

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x0600265F RID: 9823 RVA: 0x000B0FA4 File Offset: 0x000AF1A4
		// (set) Token: 0x06002660 RID: 9824 RVA: 0x000B0FD9 File Offset: 0x000AF1D9
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

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06002661 RID: 9825 RVA: 0x000B0FF2 File Offset: 0x000AF1F2
		// (set) Token: 0x06002662 RID: 9826 RVA: 0x000B0FFA File Offset: 0x000AF1FA
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

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06002663 RID: 9827 RVA: 0x000B100D File Offset: 0x000AF20D
		// (set) Token: 0x06002664 RID: 9828 RVA: 0x000B1015 File Offset: 0x000AF215
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

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06002665 RID: 9829 RVA: 0x000B1024 File Offset: 0x000AF224
		// (set) Token: 0x06002666 RID: 9830 RVA: 0x000B102C File Offset: 0x000AF22C
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

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06002667 RID: 9831 RVA: 0x000B103B File Offset: 0x000AF23B
		// (set) Token: 0x06002668 RID: 9832 RVA: 0x000B1044 File Offset: 0x000AF244
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

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06002669 RID: 9833 RVA: 0x000B1117 File Offset: 0x000AF317
		// (set) Token: 0x0600266A RID: 9834 RVA: 0x000B111F File Offset: 0x000AF31F
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

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x0600266B RID: 9835 RVA: 0x000B112E File Offset: 0x000AF32E
		private float ScreenSize
		{
			get
			{
				return (float)Mathf.Min(this.Cam.pixelWidth, this.Cam.pixelHeight) / 1080f;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x0600266C RID: 9836 RVA: 0x000B1152 File Offset: 0x000AF352
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

		// Token: 0x0600266D RID: 9837 RVA: 0x000B1174 File Offset: 0x000AF374
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

		// Token: 0x0600266E RID: 9838 RVA: 0x000B1240 File Offset: 0x000AF440
		protected virtual void Start()
		{
			this.camera = this.Cam;
			this.shader = Shader.Find("Hidden/EfficientBlur");
			if (!this.shader.isSupported)
			{
				base.enabled = false;
			}
			this.material = new Material(this.shader);
			this.previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));
			TranslucentImageSource._sizePropId = Shader.PropertyToID("size");
			TranslucentImageSource._cropRegionPropId = Shader.PropertyToID("_CropRegion");
			this.CreateNewBlurredScreen();
			this.lastDownsample = this.Downsample;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000B12D4 File Offset: 0x000AF4D4
		protected virtual void CreateNewBlurredScreen()
		{
			this.BlurredScreen = new RenderTexture(Mathf.RoundToInt((float)this.Cam.pixelWidth * this.BlurRegion.width) >> this.Downsample, Mathf.RoundToInt((float)this.Cam.pixelHeight * this.BlurRegion.height) >> this.Downsample, 0)
			{
				filterMode = FilterMode.Bilinear
			};
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000B1348 File Offset: 0x000AF548
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

		// Token: 0x06002671 RID: 9841 RVA: 0x000B13C0 File Offset: 0x000AF5C0
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

		// Token: 0x06002672 RID: 9842 RVA: 0x000B1544 File Offset: 0x000AF744
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

		// Token: 0x04002895 RID: 10389
		public float maxUpdateRate = float.PositiveInfinity;

		// Token: 0x04002896 RID: 10390
		[Tooltip("Preview the effect on entire screen")]
		public bool preview;

		// Token: 0x04002897 RID: 10391
		[SerializeField]
		private float size = 5f;

		// Token: 0x04002898 RID: 10392
		[SerializeField]
		private int iteration = 4;

		// Token: 0x04002899 RID: 10393
		[SerializeField]
		private Rect blurRegion = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x0400289A RID: 10394
		private Rect lastBlurRegion = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x0400289B RID: 10395
		[SerializeField]
		private int maxDepth = 4;

		// Token: 0x0400289C RID: 10396
		[SerializeField]
		private int downsample;

		// Token: 0x0400289D RID: 10397
		[SerializeField]
		private int lastDownsample;

		// Token: 0x0400289E RID: 10398
		[SerializeField]
		private float strength;

		// Token: 0x0400289F RID: 10399
		private float lastUpdate;

		// Token: 0x040028A0 RID: 10400
		private Camera camera;

		// Token: 0x040028A1 RID: 10401
		private Shader shader;

		// Token: 0x040028A2 RID: 10402
		private Material material;

		// Token: 0x040028A3 RID: 10403
		private Material previewMaterial;

		// Token: 0x040028A5 RID: 10405
		private static int _sizePropId;

		// Token: 0x040028A6 RID: 10406
		private static int _cropRegionPropId;
	}
}
