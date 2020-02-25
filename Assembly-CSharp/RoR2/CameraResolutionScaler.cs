using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200016C RID: 364
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraResolutionScaler : MonoBehaviour
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0001BBA5 File Offset: 0x00019DA5
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x0001BBAD File Offset: 0x00019DAD
		public Camera camera { get; private set; }

		// Token: 0x060006C6 RID: 1734 RVA: 0x0001BBB6 File Offset: 0x00019DB6
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0001BBC4 File Offset: 0x00019DC4
		private void OnPreRender()
		{
			this.ApplyScalingRenderTexture();
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0001BBCC File Offset: 0x00019DCC
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

		// Token: 0x060006C9 RID: 1737 RVA: 0x0001BC07 File Offset: 0x00019E07
		private static void SetResolutionScale(float newResolutionScale)
		{
			if (CameraResolutionScaler.resolutionScale == newResolutionScale)
			{
				return;
			}
			CameraResolutionScaler.resolutionScale = newResolutionScale;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0001BC18 File Offset: 0x00019E18
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

		// Token: 0x060006CB RID: 1739 RVA: 0x0001BD33 File Offset: 0x00019F33
		private void OnDestroy()
		{
			if (this.scalingRenderTexture)
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
		}

		// Token: 0x0400071A RID: 1818
		private RenderTexture oldRenderTexture;

		// Token: 0x0400071B RID: 1819
		private static float resolutionScale = 1f;

		// Token: 0x0400071C RID: 1820
		private RenderTexture scalingRenderTexture;

		// Token: 0x0200016D RID: 365
		private class ResolutionScaleConVar : BaseConVar
		{
			// Token: 0x060006CE RID: 1742 RVA: 0x0000972B File Offset: 0x0000792B
			private ResolutionScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060006CF RID: 1743 RVA: 0x0001BD60 File Offset: 0x00019F60
			public override void SetString(string newValue)
			{
				float num;
				TextSerialization.TryParseInvariant(newValue, out num);
			}

			// Token: 0x060006D0 RID: 1744 RVA: 0x0001BD76 File Offset: 0x00019F76
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(CameraResolutionScaler.resolutionScale);
			}

			// Token: 0x0400071D RID: 1821
			private static CameraResolutionScaler.ResolutionScaleConVar instance = new CameraResolutionScaler.ResolutionScaleConVar("resolution_scale", ConVarFlags.Archive, null, "Resolution scale. Currently nonfunctional.");
		}
	}
}
