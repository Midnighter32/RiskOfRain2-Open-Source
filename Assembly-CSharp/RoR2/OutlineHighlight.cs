using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x020002C5 RID: 709
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(SceneCamera))]
	public class OutlineHighlight : MonoBehaviour
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x0600101B RID: 4123 RVA: 0x00046DE7 File Offset: 0x00044FE7
		// (set) Token: 0x0600101C RID: 4124 RVA: 0x00046DEF File Offset: 0x00044FEF
		public SceneCamera sceneCamera { get; private set; }

		// Token: 0x0600101D RID: 4125 RVA: 0x00046DF8 File Offset: 0x00044FF8
		private void Awake()
		{
			this.sceneCamera = base.GetComponent<SceneCamera>();
			this.CreateBuffers();
			this.CreateMaterials();
			this.m_RTWidth = (int)((float)Screen.width / (float)this.m_resolution);
			this.m_RTHeight = (int)((float)Screen.height / (float)this.m_resolution);
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x00046E47 File Offset: 0x00045047
		private void CreateBuffers()
		{
			this.commandBuffer = new CommandBuffer();
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x00046E54 File Offset: 0x00045054
		private void ClearCommandBuffers()
		{
			this.commandBuffer.Clear();
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x00046E61 File Offset: 0x00045061
		private void CreateMaterials()
		{
			this.highlightMaterial = new Material(Shader.Find("Hopoo Games/Internal/Outline Highlight"));
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x00046E78 File Offset: 0x00045078
		private void RenderHighlights(RenderTexture rt)
		{
			RenderTargetIdentifier renderTarget = new RenderTargetIdentifier(rt);
			this.commandBuffer.SetRenderTarget(renderTarget);
			foreach (Highlight highlight in Highlight.readonlyHighlightList)
			{
				if (highlight.isOn && highlight.targetRenderer)
				{
					this.highlightQueue.Enqueue(new OutlineHighlight.HighlightInfo
					{
						color = highlight.GetColor() * highlight.strength,
						renderer = highlight.targetRenderer
					});
				}
			}
			Action<OutlineHighlight> action = OutlineHighlight.onPreRenderOutlineHighlight;
			if (action != null)
			{
				action(this);
			}
			while (this.highlightQueue.Count > 0)
			{
				OutlineHighlight.HighlightInfo highlightInfo = this.highlightQueue.Dequeue();
				if (highlightInfo.renderer)
				{
					this.highlightMaterial.SetColor("_Color", highlightInfo.color);
					this.commandBuffer.DrawRenderer(highlightInfo.renderer, this.highlightMaterial, 0, 0);
					RenderTexture.active = rt;
					Graphics.ExecuteCommandBuffer(this.commandBuffer);
					RenderTexture.active = null;
					this.ClearCommandBuffers();
				}
			}
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00046FAC File Offset: 0x000451AC
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			RenderTexture renderTexture = RenderTexture.active = RenderTexture.GetTemporary(this.m_RTWidth, this.m_RTHeight, 0, RenderTextureFormat.ARGB32);
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = null;
			this.ClearCommandBuffers();
			this.RenderHighlights(renderTexture);
			this.highlightMaterial.SetTexture("_OutlineMap", renderTexture);
			this.highlightMaterial.SetColor("_Color", Color.white);
			Graphics.Blit(source, destination, this.highlightMaterial, 1);
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		// Token: 0x04000F92 RID: 3986
		public OutlineHighlight.RTResolution m_resolution = OutlineHighlight.RTResolution.Full;

		// Token: 0x04000F93 RID: 3987
		public readonly Queue<OutlineHighlight.HighlightInfo> highlightQueue = new Queue<OutlineHighlight.HighlightInfo>();

		// Token: 0x04000F95 RID: 3989
		private Material highlightMaterial;

		// Token: 0x04000F96 RID: 3990
		private CommandBuffer commandBuffer;

		// Token: 0x04000F97 RID: 3991
		private int m_RTWidth = 512;

		// Token: 0x04000F98 RID: 3992
		private int m_RTHeight = 512;

		// Token: 0x04000F99 RID: 3993
		public static Action<OutlineHighlight> onPreRenderOutlineHighlight;

		// Token: 0x020002C6 RID: 710
		private enum Passes
		{
			// Token: 0x04000F9B RID: 3995
			FillPass,
			// Token: 0x04000F9C RID: 3996
			Blit
		}

		// Token: 0x020002C7 RID: 711
		public enum RTResolution
		{
			// Token: 0x04000F9E RID: 3998
			Quarter = 4,
			// Token: 0x04000F9F RID: 3999
			Half = 2,
			// Token: 0x04000FA0 RID: 4000
			Full = 1
		}

		// Token: 0x020002C8 RID: 712
		public struct HighlightInfo
		{
			// Token: 0x04000FA1 RID: 4001
			public Color color;

			// Token: 0x04000FA2 RID: 4002
			public Renderer renderer;
		}
	}
}
