using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x020004EE RID: 1262
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(SceneCamera))]
	public class OutlineHighlight : MonoBehaviour
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06001C8E RID: 7310 RVA: 0x000852FC File Offset: 0x000834FC
		// (set) Token: 0x06001C8F RID: 7311 RVA: 0x00085304 File Offset: 0x00083504
		public SceneCamera sceneCamera { get; private set; }

		// Token: 0x06001C90 RID: 7312 RVA: 0x00085310 File Offset: 0x00083510
		private void Awake()
		{
			this.sceneCamera = base.GetComponent<SceneCamera>();
			this.CreateBuffers();
			this.CreateMaterials();
			this.m_RTWidth = (int)((float)Screen.width / (float)this.m_resolution);
			this.m_RTHeight = (int)((float)Screen.height / (float)this.m_resolution);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x0008535F File Offset: 0x0008355F
		private void CreateBuffers()
		{
			this.commandBuffer = new CommandBuffer();
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x0008536C File Offset: 0x0008356C
		private void ClearCommandBuffers()
		{
			this.commandBuffer.Clear();
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x00085379 File Offset: 0x00083579
		private void CreateMaterials()
		{
			this.highlightMaterial = new Material(Shader.Find("Custom/OutlineHighlight"));
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x00085390 File Offset: 0x00083590
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

		// Token: 0x06001C95 RID: 7317 RVA: 0x000854C4 File Offset: 0x000836C4
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

		// Token: 0x04001E9F RID: 7839
		public OutlineHighlight.RTResolution m_resolution = OutlineHighlight.RTResolution.Full;

		// Token: 0x04001EA0 RID: 7840
		public readonly Queue<OutlineHighlight.HighlightInfo> highlightQueue = new Queue<OutlineHighlight.HighlightInfo>();

		// Token: 0x04001EA2 RID: 7842
		private Material highlightMaterial;

		// Token: 0x04001EA3 RID: 7843
		private CommandBuffer commandBuffer;

		// Token: 0x04001EA4 RID: 7844
		private int m_RTWidth = 512;

		// Token: 0x04001EA5 RID: 7845
		private int m_RTHeight = 512;

		// Token: 0x04001EA6 RID: 7846
		public static Action<OutlineHighlight> onPreRenderOutlineHighlight;

		// Token: 0x020004EF RID: 1263
		private enum Passes
		{
			// Token: 0x04001EA8 RID: 7848
			FillPass,
			// Token: 0x04001EA9 RID: 7849
			Blit
		}

		// Token: 0x020004F0 RID: 1264
		public enum RTResolution
		{
			// Token: 0x04001EAB RID: 7851
			Quarter = 4,
			// Token: 0x04001EAC RID: 7852
			Half = 2,
			// Token: 0x04001EAD RID: 7853
			Full = 1
		}

		// Token: 0x020004F1 RID: 1265
		public struct HighlightInfo
		{
			// Token: 0x04001EAE RID: 7854
			public Color color;

			// Token: 0x04001EAF RID: 7855
			public Renderer renderer;
		}
	}
}
