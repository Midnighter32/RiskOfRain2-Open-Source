using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2.PostProcess
{
	// Token: 0x0200052D RID: 1325
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class SobelCommandBuffer : MonoBehaviour
	{
		// Token: 0x06001F57 RID: 8023 RVA: 0x000881D5 File Offset: 0x000863D5
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x000881E3 File Offset: 0x000863E3
		private void OnPreRender()
		{
			this.SetupCommandBuffer();
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x000881EB File Offset: 0x000863EB
		private void OnDisable()
		{
			if (this.camera != null && this.sobelCommandBuffer != null)
			{
				this.camera.RemoveCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
				this.sobelCommandBuffer = null;
			}
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x00088224 File Offset: 0x00086424
		private void SetupCommandBuffer()
		{
			if (!this.camera)
			{
				return;
			}
			if (this.sobelCommandBuffer == null)
			{
				int nameID = Shader.PropertyToID("_SobelTex");
				this.sobelBufferMaterial = new Material(Shader.Find("Hopoo Games/Internal/SobelBuffer"));
				this.sobelCommandBuffer = new CommandBuffer();
				this.sobelCommandBuffer.name = "Sobel Command Buffer";
				this.sobelCommandBuffer.GetTemporaryRT(nameID, -1, -1);
				RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(nameID);
				this.sobelCommandBuffer.SetRenderTarget(renderTargetIdentifier);
				this.sobelCommandBuffer.ClearRenderTarget(true, true, Color.clear);
				this.sobelCommandBuffer.Blit(new RenderTargetIdentifier(BuiltinRenderTextureType.ResolvedDepth), renderTargetIdentifier, this.sobelBufferMaterial);
				this.sobelCommandBuffer.SetGlobalTexture(nameID, renderTargetIdentifier);
				this.sobelCommandBuffer.SetRenderTarget(new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget));
				this.sobelCommandBuffer.ReleaseTemporaryRT(nameID);
				this.camera.AddCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
			}
		}

		// Token: 0x04001D00 RID: 7424
		public CameraEvent cameraEvent = CameraEvent.BeforeLighting;

		// Token: 0x04001D01 RID: 7425
		private RenderTexture sobelRT;

		// Token: 0x04001D02 RID: 7426
		private CommandBuffer sobelCommandBuffer;

		// Token: 0x04001D03 RID: 7427
		private Material sobelBufferMaterial;

		// Token: 0x04001D04 RID: 7428
		private Camera camera;
	}
}
