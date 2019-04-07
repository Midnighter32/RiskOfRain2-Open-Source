using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2.PostProcess
{
	// Token: 0x02000563 RID: 1379
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class SobelCommandBuffer : MonoBehaviour
	{
		// Token: 0x06001EC9 RID: 7881 RVA: 0x00091605 File Offset: 0x0008F805
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06001ECA RID: 7882 RVA: 0x00091613 File Offset: 0x0008F813
		private void OnPreRender()
		{
			this.SetupCommandBuffer();
		}

		// Token: 0x06001ECB RID: 7883 RVA: 0x0009161B File Offset: 0x0008F81B
		private void OnDisable()
		{
			if (this.camera != null && this.sobelCommandBuffer != null)
			{
				this.camera.RemoveCommandBuffer(this.cameraEvent, this.sobelCommandBuffer);
				this.sobelCommandBuffer = null;
			}
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x00091654 File Offset: 0x0008F854
		private void SetupCommandBuffer()
		{
			if (!this.camera)
			{
				return;
			}
			if (this.sobelCommandBuffer == null)
			{
				int nameID = Shader.PropertyToID("_SobelTex");
				this.sobelBufferMaterial = new Material(Shader.Find("Hidden/SobelBuffer"));
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

		// Token: 0x04002176 RID: 8566
		public CameraEvent cameraEvent = CameraEvent.BeforeLighting;

		// Token: 0x04002177 RID: 8567
		private RenderTexture sobelRT;

		// Token: 0x04002178 RID: 8568
		private CommandBuffer sobelCommandBuffer;

		// Token: 0x04002179 RID: 8569
		private Material sobelBufferMaterial;

		// Token: 0x0400217A RID: 8570
		private Camera camera;
	}
}
