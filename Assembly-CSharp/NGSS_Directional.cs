using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200000A RID: 10
[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class NGSS_Directional : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600002E RID: 46 RVA: 0x0000360C File Offset: 0x0000180C
	private Light DirLight
	{
		get
		{
			if (this._DirLight == null)
			{
				this._DirLight = base.GetComponent<Light>();
			}
			return this._DirLight;
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000362E File Offset: 0x0000182E
	private void OnDisable()
	{
		this.isInitialized = false;
		if (this.NGSS_KEEP_ONDISABLE)
		{
			return;
		}
		if (this.isGraphicSet)
		{
			this.isGraphicSet = false;
			GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/Internal-ScreenSpaceShadows"));
			GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseBuiltin);
		}
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00003666 File Offset: 0x00001866
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

	// Token: 0x06000031 RID: 49 RVA: 0x0000368C File Offset: 0x0000188C
	private void Init()
	{
		if (this.isInitialized)
		{
			return;
		}
		if (!this.isGraphicSet)
		{
			GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseCustom);
			GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/NGSS_Directional"));
			this.DirLight.shadows = LightShadows.Soft;
			this.isGraphicSet = true;
		}
		this.isInitialized = true;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x000033F8 File Offset: 0x000015F8
	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000036DC File Offset: 0x000018DC
	private void Update()
	{
		this.NGSS_SAMPLING_TEST = Mathf.Clamp(this.NGSS_SAMPLING_TEST, 4, this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalFloat("NGSS_TEST_SAMPLERS_DIR", (!this.NGSS_PCSS_ENABLED && this.NGSS_SAMPLING_TEST > this.NGSS_SAMPLING_FILTER / 2) ? 0f : ((float)this.NGSS_SAMPLING_TEST));
		Shader.SetGlobalFloat("NGSS_FILTER_SAMPLERS_DIR", (float)this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS", this.NGSS_GLOBAL_SOFTNESS / (QualitySettings.shadowDistance * 0.66f) * ((QualitySettings.shadowCascades == 2) ? 1.5f : ((QualitySettings.shadowCascades == 4) ? 1f : 0.25f)));
		Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS_OPTIMIZED", this.NGSS_GLOBAL_SOFTNESS);
		int num = (int)Mathf.Sqrt((float)this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalInt("NGSS_OPTIMIZED_ITERATIONS", (num % 2 == 0) ? (num + 1) : num);
		Shader.SetGlobalInt("NGSS_OPTIMIZED_SAMPLERS", this.NGSS_SAMPLING_FILTER);
		Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS_DENOISER", this.NGSS_GLOBAL_SOFTNESS);
		int num2 = Mathf.RoundToInt(Mathf.Sqrt((float)this.NGSS_SAMPLING_DENOISER));
		Shader.SetGlobalInt("NGSS_DENOISER_ITERATIONS", (num2 % 2 == 0) ? (num2 + 1) : num2);
		Shader.SetGlobalFloat("NGSS_DENOISER_SIZE", 512f);
		if (this.NGSS_RECEIVER_PLANE_BIAS)
		{
			Shader.EnableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
		}
		else
		{
			Shader.DisableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
		}
		if (this.NGSS_SHADOWS_DITHERING)
		{
			Shader.EnableKeyword("NGSS_NOISE_GRID_DIR");
		}
		else
		{
			Shader.DisableKeyword("NGSS_NOISE_GRID_DIR");
		}
		Shader.SetGlobalFloat("NGSS_BANDING_TO_NOISE_RATIO_DIR", this.NGSS_NOISE_SCALE);
		if (this.NGSS_PCSS_ENABLED)
		{
			float num3 = this.NGSS_PCSS_SOFTNESS_MIN * 0.1f;
			float num4 = this.NGSS_PCSS_SOFTNESS_MAX * 0.25f;
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (num3 > num4) ? num4 : num3);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (num4 < num3) ? num3 : num4);
			Shader.EnableKeyword("NGSS_PCSS_FILTER_DIR");
		}
		else
		{
			Shader.DisableKeyword("NGSS_PCSS_FILTER_DIR");
		}
		if (this.NGSS_SHADOWS_RESOLUTION == NGSS_Directional.ShadowMapResolution.UseQualitySettings)
		{
			this.DirLight.shadowResolution = LightShadowResolution.FromQualitySettings;
		}
		else
		{
			this.DirLight.shadowCustomResolution = (int)this.NGSS_SHADOWS_RESOLUTION;
		}
		this.GLOBAL_CASCADES_COUNT = ((this.GLOBAL_CASCADES_COUNT == 3) ? 4 : this.GLOBAL_CASCADES_COUNT);
		this.GLOBAL_SHADOWS_DISTANCE = Mathf.Clamp(this.GLOBAL_SHADOWS_DISTANCE, 0f, this.GLOBAL_SHADOWS_DISTANCE);
		if (this.GLOBAL_SETTINGS_OVERRIDE)
		{
			QualitySettings.shadowDistance = this.GLOBAL_SHADOWS_DISTANCE;
			QualitySettings.shadowProjection = this.GLOBAL_SHADOWS_PROJECTION;
			if (!this.GLOBAL_CASCADED_SHADOWS)
			{
				return;
			}
			if (this.GLOBAL_CASCADES_COUNT > 1)
			{
				QualitySettings.shadowCascades = this.GLOBAL_CASCADES_COUNT;
				QualitySettings.shadowCascade4Split = new Vector3(this.GLOBAL_CASCADES_SPLIT_VALUE, this.GLOBAL_CASCADES_SPLIT_VALUE * 2f, this.GLOBAL_CASCADES_SPLIT_VALUE * 2f * 2f);
				QualitySettings.shadowCascade2Split = this.GLOBAL_CASCADES_SPLIT_VALUE * 2f;
			}
		}
		if (this.GLOBAL_CASCADES_COUNT > 1)
		{
			Shader.SetGlobalFloat("NGSS_CASCADES_SOFTNESS_NORMALIZATION", this.NGSS_CASCADES_SOFTNESS_NORMALIZATION);
			Shader.SetGlobalFloat("NGSS_CASCADES_COUNT", (float)QualitySettings.shadowCascades);
			Shader.SetGlobalVector("NGSS_CASCADES_SPLITS", (QualitySettings.shadowCascades == 2) ? new Vector4(QualitySettings.shadowCascade2Split, 1f, 1f, 1f) : new Vector4(QualitySettings.shadowCascade4Split.x, QualitySettings.shadowCascade4Split.y, QualitySettings.shadowCascade4Split.z, 1f));
		}
		if (this.NGSS_CASCADES_BLENDING && this.GLOBAL_CASCADES_COUNT > 1)
		{
			Shader.EnableKeyword("NGSS_USE_CASCADE_BLENDING");
			Shader.SetGlobalFloat("NGSS_CASCADE_BLEND_DISTANCE", this.NGSS_CASCADES_BLENDING_VALUE * 0.125f);
			return;
		}
		Shader.DisableKeyword("NGSS_USE_CASCADE_BLENDING");
	}

	// Token: 0x0400002C RID: 44
	[Tooltip("If disabled, NGSS Directional shadows replacement will be removed from Graphics settings when OnDisable is called in this component.")]
	[Header("MAIN SETTINGS")]
	public bool NGSS_KEEP_ONDISABLE = true;

	// Token: 0x0400002D RID: 45
	[Tooltip("Shadows resolution.\nUseQualitySettings = From Quality Settings, SuperLow = 512, Low = 1024, Med = 2048, High = 4096, Ultra = 8192, Mega = 16384.")]
	public NGSS_Directional.ShadowMapResolution NGSS_SHADOWS_RESOLUTION = NGSS_Directional.ShadowMapResolution.UseQualitySettings;

	// Token: 0x0400002E RID: 46
	[Header("SAMPLING")]
	[Tooltip("Used to test blocker search and early bail out algorithms. Keep it as low as possible, might lead to white noise if too low.\nRecommended values: Mobile = 8, Consoles & VR = 16, Desktop = 24")]
	[Range(4f, 32f)]
	public int NGSS_SAMPLING_TEST = 16;

	// Token: 0x0400002F RID: 47
	[Range(1f, 32f)]
	[Tooltip("Number of samplers per pixel used for the Denoiser algorithm.\nRequires NGSS Shadows Libraries to be installed and Cascaded Shadows to be enabled.")]
	public int NGSS_SAMPLING_DENOISER = 16;

	// Token: 0x04000030 RID: 48
	[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 16, Consoles & VR = 32, Desktop Med = 48, Desktop High = 64, Desktop Ultra = 128")]
	[Range(8f, 128f)]
	public int NGSS_SAMPLING_FILTER = 48;

	// Token: 0x04000031 RID: 49
	[Tooltip("Overall softness for all shadows.")]
	[Header("SOFTNESS")]
	[Range(0f, 2f)]
	public float NGSS_GLOBAL_SOFTNESS = 1f;

	// Token: 0x04000032 RID: 50
	[Tooltip("PCSS Requires inline sampling and SM3.5.\nProvides Area Light soft-shadows.\nDisable it if you are looking for PCF filtering (uniform soft-shadows) which runs with SM3.0.")]
	[Header("PCSS")]
	public bool NGSS_PCSS_ENABLED;

	// Token: 0x04000033 RID: 51
	[Range(0f, 2f)]
	[Tooltip("How soft shadows are when close to caster.")]
	public float NGSS_PCSS_SOFTNESS_MIN = 1f;

	// Token: 0x04000034 RID: 52
	[Tooltip("How soft shadows are when far from caster.")]
	[Range(0f, 2f)]
	public float NGSS_PCSS_SOFTNESS_MAX = 1f;

	// Token: 0x04000035 RID: 53
	[Header("NOISE")]
	[Tooltip("Improve noise randomnes by aligning samplers in a screen space grid. If disabled, noise will be randomly distributed.\nRecommended when using low sampling count (less than 16 spp)")]
	public bool NGSS_SHADOWS_DITHERING = true;

	// Token: 0x04000036 RID: 54
	[Range(0f, 1f)]
	[Tooltip("If zero = no noise.\nIf one = 100% noise.\nUseful when fighting banding.")]
	public float NGSS_NOISE_SCALE = 1f;

	// Token: 0x04000037 RID: 55
	[Header("BIAS")]
	[Tooltip("This estimates receiver slope using derivatives and tries to tilt the filtering kernel along it.\nHowever, when doing it in screenspace from the depth texture can leads to shadow artifacts.\nThus it is disabled by default.")]
	public bool NGSS_RECEIVER_PLANE_BIAS;

	// Token: 0x04000038 RID: 56
	[Header("GLOBAL SETTINGS")]
	[Tooltip("Enable it to let NGSS_Directional control global shadows settings through this component.\nDisable it if you want to manage shadows settings through Unity Quality & Graphics Settings panel.")]
	public bool GLOBAL_SETTINGS_OVERRIDE;

	// Token: 0x04000039 RID: 57
	[Tooltip("Shadows projection.\nRecommeded StableFit as it helps stabilizing shadows as camera moves.")]
	public ShadowProjection GLOBAL_SHADOWS_PROJECTION = ShadowProjection.StableFit;

	// Token: 0x0400003A RID: 58
	[Tooltip("Sets the maximum distance at wich shadows are visible from camera.\nThis option affects your shadow distance in Quality Settings.")]
	public float GLOBAL_SHADOWS_DISTANCE = 150f;

	// Token: 0x0400003B RID: 59
	[Tooltip("Must be disabled on very low end hardware.\nIf enabled, Cascaded Shadows will be turned off in Graphics Settings.")]
	public bool GLOBAL_CASCADED_SHADOWS = true;

	// Token: 0x0400003C RID: 60
	[Tooltip("Number of cascades the shadowmap will have. This option affects your cascade counts in Quality Settings.\nYou should entierly disable Cascaded Shadows (Graphics Menu) if you are targeting low-end devices.")]
	[Range(1f, 4f)]
	public int GLOBAL_CASCADES_COUNT = 4;

	// Token: 0x0400003D RID: 61
	[Range(0.01f, 0.25f)]
	[Tooltip("Used for the cascade stitching algorithm.\nCompute cascades splits distribution exponentially in a x*2^n form.\nIf 4 cascades, set this value to 0.1. If 2 cascades, set it to 0.25.\nThis option affects your cascade splits in Quality Settings.")]
	public float GLOBAL_CASCADES_SPLIT_VALUE = 0.1f;

	// Token: 0x0400003E RID: 62
	[Tooltip("Blends cascades at seams intersection.\nAdditional overhead required for this option.")]
	[Header("CASCADES")]
	public bool NGSS_CASCADES_BLENDING = true;

	// Token: 0x0400003F RID: 63
	[Tooltip("Tweak this value to adjust the blending transition between cascades.")]
	[Range(0f, 2f)]
	public float NGSS_CASCADES_BLENDING_VALUE = 1f;

	// Token: 0x04000040 RID: 64
	[Tooltip("If one, softness across cascades will be matched using splits distribution, resulting in realistic soft-ness over distance.\nIf zero the softness distribution will be based on cascade index, resulting in blurrier shadows over distance thus less realistic.")]
	[Range(0f, 1f)]
	public float NGSS_CASCADES_SOFTNESS_NORMALIZATION = 1f;

	// Token: 0x04000041 RID: 65
	private bool isInitialized;

	// Token: 0x04000042 RID: 66
	private bool isGraphicSet;

	// Token: 0x04000043 RID: 67
	private Light _DirLight;

	// Token: 0x0200000B RID: 11
	public enum ShadowMapResolution
	{
		// Token: 0x04000045 RID: 69
		UseQualitySettings = 256,
		// Token: 0x04000046 RID: 70
		VeryLow = 512,
		// Token: 0x04000047 RID: 71
		Low = 1024,
		// Token: 0x04000048 RID: 72
		Med = 2048,
		// Token: 0x04000049 RID: 73
		High = 4096,
		// Token: 0x0400004A RID: 74
		Ultra = 8192,
		// Token: 0x0400004B RID: 75
		Mega = 16384
	}
}
