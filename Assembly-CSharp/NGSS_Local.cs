using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200000C RID: 12
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Local : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000035 RID: 53 RVA: 0x00003B00 File Offset: 0x00001D00
	private Light LocalLight
	{
		get
		{
			if (this._LocalLight == null)
			{
				this._LocalLight = base.GetComponent<Light>();
			}
			return this._LocalLight;
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003B22 File Offset: 0x00001D22
	private void OnDisable()
	{
		this.isInitialized = false;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00003B2B File Offset: 0x00001D2B
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

	// Token: 0x06000038 RID: 56 RVA: 0x00003B4E File Offset: 0x00001D4E
	private void Init()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.SetProperties(true);
		this.isInitialized = true;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x000033F8 File Offset: 0x000015F8
	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00003B67 File Offset: 0x00001D67
	private void Update()
	{
		if (this.NGSS_DISABLE_ON_PLAY && Application.isPlaying)
		{
			base.enabled = false;
			return;
		}
		this.SetProperties(this.NGSS_MANAGE_GLOBAL_SETTINGS);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003B8C File Offset: 0x00001D8C
	private void SetProperties(bool setLocalAndGlobalProperties)
	{
		this.LocalLight.shadowStrength = this.NGSS_SHADOWS_SOFTNESS;
		if (this.NGSS_SHADOWS_RESOLUTION == NGSS_Local.ShadowMapResolution.UseQualitySettings)
		{
			this.LocalLight.shadowResolution = LightShadowResolution.FromQualitySettings;
		}
		else
		{
			this.LocalLight.shadowCustomResolution = (int)this.NGSS_SHADOWS_RESOLUTION;
		}
		this.LocalLight.shadows = (this.NGSS_SHADOWS_DITHERING ? LightShadows.Soft : LightShadows.Hard);
		if (!setLocalAndGlobalProperties)
		{
			return;
		}
		this.NGSS_SAMPLING_TEST = Mathf.Clamp(this.NGSS_SAMPLING_TEST, 4, this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalFloat("NGSS_PCSS_FILTER_LOCAL", this.NGSS_PCSS_ENABLED ? 1f : 0f);
		Shader.SetGlobalFloat("NGSS_TEST_SAMPLERS", (!this.NGSS_PCSS_ENABLED && this.NGSS_SAMPLING_TEST > this.NGSS_SAMPLING_FILTER / 2) ? 0f : ((float)this.NGSS_SAMPLING_TEST));
		Shader.SetGlobalFloat("NGSS_FILTER_SAMPLERS", (float)this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalFloat("NGSS_BANDING_TO_NOISE_RATIO", this.NGSS_NOISE_SCALE);
		Shader.SetGlobalFloat("NGSS_GLOBAL_OPACITY", 1f - this.NGSS_SHADOWS_OPACITY);
	}

	// Token: 0x0400004C RID: 76
	[Tooltip("Check this option to disable this component from receiving updates calls at runtime or when you hit play in Editor.\nUseful when you have lot of lights in your scene and you don't want that many update calls.")]
	public bool NGSS_DISABLE_ON_PLAY;

	// Token: 0x0400004D RID: 77
	[Tooltip("If enabled, this component will manage GLOBAL SETTINGS for all Local shadows.\nEnable this option only in one of your scene local lights to avoid multiple lights fighting for global tweaks.\nLOCAL SETTINGS are not affected by this option.")]
	public bool NGSS_MANAGE_GLOBAL_SETTINGS;

	// Token: 0x0400004E RID: 78
	[Tooltip("PCSS Requires inline sampling and SM3.5.\nProvides Area Light soft-shadows.\nDisable it if you are looking for PCF filtering (uniform soft-shadows) which runs with SM3.0.")]
	[Header("GLOBAL SETTINGS")]
	public bool NGSS_PCSS_ENABLED = true;

	// Token: 0x0400004F RID: 79
	[Tooltip("Used to test blocker search and early bail out algorithms. Keep it as low as possible, might lead to noise artifacts if too low.\nRecommended values: Mobile = 8, Consoles & VR = 16, Desktop = 24")]
	[Range(4f, 32f)]
	public int NGSS_SAMPLING_TEST = 16;

	// Token: 0x04000050 RID: 80
	[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 12, Consoles & VR = 24, Desktop Med = 32, Desktop High = 48, Desktop Ultra = 64")]
	[Range(4f, 64f)]
	public int NGSS_SAMPLING_FILTER = 32;

	// Token: 0x04000051 RID: 81
	[Tooltip("If zero = no noise.\nIf one = 100% noise.\nUseful when fighting banding.")]
	[Range(0f, 1f)]
	public float NGSS_NOISE_SCALE = 1f;

	// Token: 0x04000052 RID: 82
	[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 12, Consoles & VR = 24, Desktop Med = 32, Desktop High = 48, Desktop Ultra = 64")]
	[Range(0f, 1f)]
	public float NGSS_SHADOWS_OPACITY = 1f;

	// Token: 0x04000053 RID: 83
	[Header("LOCAL SETTINGS")]
	[Range(0f, 1f)]
	public float NGSS_SHADOWS_SOFTNESS = 1f;

	// Token: 0x04000054 RID: 84
	[Tooltip("Improve noise randomnes by aligning samplers in a screen space grid. If disabled, noise will be randomly distributed.\nRecommended when using low sampling count (less than 16 spp)")]
	public bool NGSS_SHADOWS_DITHERING = true;

	// Token: 0x04000055 RID: 85
	[Tooltip("Shadows resolution.\nUseQualitySettings = From Quality Settings, SuperLow = 512, Low = 1024, Med = 2048, High = 4096, Ultra = 8192.")]
	public NGSS_Local.ShadowMapResolution NGSS_SHADOWS_RESOLUTION = NGSS_Local.ShadowMapResolution.UseQualitySettings;

	// Token: 0x04000056 RID: 86
	private bool isInitialized;

	// Token: 0x04000057 RID: 87
	private Light _LocalLight;

	// Token: 0x0200000D RID: 13
	public enum ShadowMapResolution
	{
		// Token: 0x04000059 RID: 89
		UseQualitySettings = 256,
		// Token: 0x0400005A RID: 90
		VeryLow = 512,
		// Token: 0x0400005B RID: 91
		Low = 1024,
		// Token: 0x0400005C RID: 92
		Med = 2048,
		// Token: 0x0400005D RID: 93
		High = 4096,
		// Token: 0x0400005E RID: 94
		Ultra = 8192
	}
}
