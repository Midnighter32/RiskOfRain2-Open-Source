using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000009 RID: 9
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class NGSS_ContactShadows : MonoBehaviour
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000023 RID: 35 RVA: 0x00002F74 File Offset: 0x00001174
	private Camera mCamera
	{
		get
		{
			if (this._mCamera == null)
			{
				this._mCamera = base.GetComponent<Camera>();
				if (this._mCamera == null)
				{
					this._mCamera = Camera.main;
				}
				if (this._mCamera == null)
				{
					Debug.LogError("NGSS Error: No MainCamera found, please provide one.", this);
				}
				else
				{
					this._mCamera.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
			return this._mCamera;
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000024 RID: 36 RVA: 0x00002FE8 File Offset: 0x000011E8
	private Material mMaterial
	{
		get
		{
			if (this._mMaterial == null)
			{
				if (this.contactShadowsShader == null)
				{
					Shader.Find("Hidden/NGSS_ContactShadows");
				}
				this._mMaterial = new Material(this.contactShadowsShader);
				if (this._mMaterial == null)
				{
					Debug.LogWarning("NGSS Warning: can't find NGSS_ContactShadows shader, make sure it's on your project.", this);
					base.enabled = false;
					return null;
				}
			}
			return this._mMaterial;
		}
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00003058 File Offset: 0x00001258
	private void AddCommandBuffers()
	{
		this.computeShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Compute"
		};
		this.blendShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Mix"
		};
		if (this.mCamera)
		{
			bool flag = true;
			CommandBuffer[] commandBuffers = this.mCamera.GetCommandBuffers((this.mCamera.actualRenderingPath == RenderingPath.Forward) ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == this.computeShadowsCB.name)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.mCamera.AddCommandBuffer((this.mCamera.actualRenderingPath == RenderingPath.Forward) ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, this.computeShadowsCB);
			}
		}
		if (this.mainDirectionalLight)
		{
			bool flag2 = true;
			CommandBuffer[] commandBuffers = this.mainDirectionalLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == this.blendShadowsCB.name)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				this.mainDirectionalLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this.blendShadowsCB);
			}
		}
	}

	// Token: 0x06000026 RID: 38 RVA: 0x0000316C File Offset: 0x0000136C
	private void RemoveCommandBuffers()
	{
		this._mMaterial = null;
		if (this.mCamera)
		{
			this.mCamera.RemoveCommandBuffer((this.mCamera.actualRenderingPath == RenderingPath.Forward) ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, this.computeShadowsCB);
		}
		if (this.mainDirectionalLight)
		{
			this.mainDirectionalLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this.blendShadowsCB);
		}
		this.isInitialized = false;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000031D8 File Offset: 0x000013D8
	private void Init()
	{
		if (this.isInitialized || this.mainDirectionalLight == null)
		{
			return;
		}
		if (this.mCamera.actualRenderingPath == RenderingPath.VertexLit)
		{
			Debug.LogWarning("Vertex Lit Rendering Path is not supported by NGSS Contact Shadows. Please set the Rendering Path in your game camera or Graphics Settings to something else than Vertex Lit.", this);
			base.enabled = false;
			return;
		}
		this.AddCommandBuffers();
		int nameID = Shader.PropertyToID("NGSS_ContactShadowRT");
		int nameID2 = Shader.PropertyToID("NGSS_ContactShadowRT2");
		int nameID3 = Shader.PropertyToID("NGSS_DepthSourceRT");
		this.computeShadowsCB.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
		this.computeShadowsCB.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
		this.computeShadowsCB.GetTemporaryRT(nameID3, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
		this.computeShadowsCB.Blit(nameID, nameID3, this.mMaterial, 0);
		this.computeShadowsCB.Blit(nameID3, nameID, this.mMaterial, 1);
		this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, 1f));
		this.computeShadowsCB.Blit(nameID, nameID2, this.mMaterial, 2);
		this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(1f, 0f));
		this.computeShadowsCB.Blit(nameID2, nameID, this.mMaterial, 2);
		this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, 2f));
		this.computeShadowsCB.Blit(nameID, nameID2, this.mMaterial, 2);
		this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(2f, 0f));
		this.computeShadowsCB.Blit(nameID2, nameID, this.mMaterial, 2);
		this.computeShadowsCB.SetGlobalTexture("NGSS_ContactShadowsTexture", nameID);
		this.blendShadowsCB.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, this.mMaterial, 3);
		this.isInitialized = true;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x000033F8 File Offset: 0x000015F8
	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00003402 File Offset: 0x00001602
	private void OnEnable()
	{
		if (this.IsNotSupported())
		{
			Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
			base.enabled = false;
			return;
		}
		this.Init();
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00003425 File Offset: 0x00001625
	private void OnDisable()
	{
		if (this.isInitialized)
		{
			this.RemoveCommandBuffers();
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00003425 File Offset: 0x00001625
	private void OnApplicationQuit()
	{
		if (this.isInitialized)
		{
			this.RemoveCommandBuffers();
		}
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00003438 File Offset: 0x00001638
	private void OnPreRender()
	{
		this.Init();
		if (!this.isInitialized || this.mainDirectionalLight == null)
		{
			return;
		}
		this.mMaterial.SetVector("LightDir", this.mCamera.transform.InverseTransformDirection(this.mainDirectionalLight.transform.forward));
		this.mMaterial.SetFloat("ShadowsOpacity", 1f - this.mainDirectionalLight.shadowStrength);
		this.mMaterial.SetFloat("ShadowsEdgeTolerance", this.m_shadowsEdgeTolerance * 0.075f);
		this.mMaterial.SetFloat("ShadowsSoftness", this.m_shadowsSoftness * 4f);
		this.mMaterial.SetFloat("ShadowsDistance", this.m_shadowsDistance);
		this.mMaterial.SetFloat("ShadowsFade", this.m_shadowsFade);
		this.mMaterial.SetFloat("ShadowsBias", this.m_shadowsOffset * 0.02f);
		this.mMaterial.SetFloat("RayWidth", this.m_rayWidth);
		this.mMaterial.SetFloat("RaySamples", (float)this.m_raySamples);
		this.mMaterial.SetFloat("RaySamplesScale", this.m_raySamplesScale);
		if (this.m_noiseFilter)
		{
			this.mMaterial.EnableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
			return;
		}
		this.mMaterial.DisableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
	}

	// Token: 0x0400001C RID: 28
	public Light mainDirectionalLight;

	// Token: 0x0400001D RID: 29
	public Shader contactShadowsShader;

	// Token: 0x0400001E RID: 30
	[Tooltip("Poisson Noise. Randomize samples to remove repeated patterns.")]
	[Header("SHADOWS SETTINGS")]
	public bool m_noiseFilter;

	// Token: 0x0400001F RID: 31
	[Tooltip("Tweak this value to remove soft-shadows leaking around edges.")]
	[Range(0.01f, 1f)]
	public float m_shadowsEdgeTolerance = 0.25f;

	// Token: 0x04000020 RID: 32
	[Range(0.01f, 1f)]
	[Tooltip("Overall softness of the shadows.")]
	public float m_shadowsSoftness = 0.25f;

	// Token: 0x04000021 RID: 33
	[Tooltip("Overall distance of the shadows.")]
	[Range(1f, 4f)]
	public float m_shadowsDistance = 1f;

	// Token: 0x04000022 RID: 34
	[Tooltip("The distance where shadows start to fade.")]
	[Range(0.1f, 4f)]
	public float m_shadowsFade = 1f;

	// Token: 0x04000023 RID: 35
	[Range(0f, 2f)]
	[Tooltip("Tweak this value if your objects display backface shadows.")]
	public float m_shadowsOffset = 0.325f;

	// Token: 0x04000024 RID: 36
	[Range(16f, 128f)]
	[Header("RAY SETTINGS")]
	[Tooltip("Number of samplers between each step. The higher values produces less gaps between shadows. Keep this value as low as you can!")]
	public int m_raySamples = 64;

	// Token: 0x04000025 RID: 37
	[Range(0f, 1f)]
	[Tooltip("Samplers scale over distance. Lower this value if you want to speed things up by doing less sampling on far away objects.")]
	public float m_raySamplesScale = 1f;

	// Token: 0x04000026 RID: 38
	[Range(0f, 1f)]
	[Tooltip("The higher the value, the ticker the shadows will look.")]
	public float m_rayWidth = 0.1f;

	// Token: 0x04000027 RID: 39
	private CommandBuffer blendShadowsCB;

	// Token: 0x04000028 RID: 40
	private CommandBuffer computeShadowsCB;

	// Token: 0x04000029 RID: 41
	private bool isInitialized;

	// Token: 0x0400002A RID: 42
	private Camera _mCamera;

	// Token: 0x0400002B RID: 43
	private Material _mMaterial;
}
