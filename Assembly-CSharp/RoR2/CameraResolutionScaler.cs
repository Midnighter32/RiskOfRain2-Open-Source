using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000274 RID: 628
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class CameraResolutionScaler : MonoBehaviour
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x00039D0D File Offset: 0x00037F0D
		// (set) Token: 0x06000BD0 RID: 3024 RVA: 0x00039D15 File Offset: 0x00037F15
		public Camera camera { get; private set; }

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00039D1E File Offset: 0x00037F1E
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00039D2C File Offset: 0x00037F2C
		private void OnPreRender()
		{
			this.ApplyScalingRenderTexture();
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x00039D34 File Offset: 0x00037F34
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.scalingRenderTexture)
			{
				Graphics.Blit(source, destination);
				return;
			}
			this.camera.targetTexture = this.oldRenderTexture;
			Graphics.Blit(source, this.oldRenderTexture);
			this.oldRenderTexture = null;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x00039D6F File Offset: 0x00037F6F
		private static void SetResolutionScale(float newResolutionScale)
		{
			if (CameraResolutionScaler.resolutionScale == newResolutionScale)
			{
				return;
			}
			CameraResolutionScaler.resolutionScale = newResolutionScale;
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x00039D80 File Offset: 0x00037F80
		private void ApplyScalingRenderTexture()
		{
			this.oldRenderTexture = this.camera.targetTexture;
			bool flag = CameraResolutionScaler.resolutionScale != 1f;
			this.camera.targetTexture = null;
			Rect pixelRect = this.camera.pixelRect;
			int num = Mathf.FloorToInt(pixelRect.width * CameraResolutionScaler.resolutionScale);
			int num2 = Mathf.FloorToInt(pixelRect.height * CameraResolutionScaler.resolutionScale);
			if (this.scalingRenderTexture && (this.scalingRenderTexture.width != num || this.scalingRenderTexture.height != num2))
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
			if (flag != this.scalingRenderTexture)
			{
				if (flag)
				{
					this.scalingRenderTexture = new RenderTexture(num, num2, 24);
					this.scalingRenderTexture.autoGenerateMips = false;
					this.scalingRenderTexture.filterMode = (((double)CameraResolutionScaler.resolutionScale > 1.0) ? FilterMode.Bilinear : FilterMode.Point);
				}
				else
				{
					UnityEngine.Object.Destroy(this.scalingRenderTexture);
					this.scalingRenderTexture = null;
				}
			}
			if (flag)
			{
				this.camera.targetTexture = this.scalingRenderTexture;
			}
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00039E9B File Offset: 0x0003809B
		private void OnDestroy()
		{
			if (this.scalingRenderTexture)
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
		}

		// Token: 0x04000FC8 RID: 4040
		private RenderTexture oldRenderTexture;

		// Token: 0x04000FC9 RID: 4041
		private static float resolutionScale = 1f;

		// Token: 0x04000FCA RID: 4042
		private RenderTexture scalingRenderTexture;

		// Token: 0x02000275 RID: 629
		private class ResolutionScaleConVar : BaseConVar
		{
			// Token: 0x06000BD9 RID: 3033 RVA: 0x00037E38 File Offset: 0x00036038
			private ResolutionScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000BDA RID: 3034 RVA: 0x00039EC8 File Offset: 0x000380C8
			public override void SetString(string newValue)
			{
				float num;
				TextSerialization.TryParseInvariant(newValue, out num);
			}

			// Token: 0x06000BDB RID: 3035 RVA: 0x00039EDE File Offset: 0x000380DE
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(CameraResolutionScaler.resolutionScale);
			}

			// Token: 0x04000FCB RID: 4043
			private static CameraResolutionScaler.ResolutionScaleConVar instance = new CameraResolutionScaler.ResolutionScaleConVar("resolution_scale", ConVarFlags.Archive, null, "Resolution scale. Currently nonfunctional.");
		}
	}
}
