using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200006C RID: 108
[Serializable]
public struct RenderSettingsState
{
	// Token: 0x060001A0 RID: 416 RVA: 0x00009008 File Offset: 0x00007208
	public static RenderSettingsState FromCurrent()
	{
		return new RenderSettingsState
		{
			haloStrength = RenderSettings.haloStrength,
			defaultReflectionResolution = RenderSettings.defaultReflectionResolution,
			defaultReflectionMode = RenderSettings.defaultReflectionMode,
			reflectionBounces = RenderSettings.reflectionBounces,
			reflectionIntensity = RenderSettings.reflectionIntensity,
			customReflection = RenderSettings.customReflection,
			ambientProbe = RenderSettings.ambientProbe,
			sun = RenderSettings.sun,
			skybox = RenderSettings.skybox,
			subtractiveShadowColor = RenderSettings.subtractiveShadowColor,
			flareStrength = RenderSettings.flareStrength,
			ambientLight = RenderSettings.ambientLight,
			ambientGroundColor = RenderSettings.ambientGroundColor,
			ambientEquatorColor = RenderSettings.ambientEquatorColor,
			ambientSkyColor = RenderSettings.ambientSkyColor,
			ambientMode = RenderSettings.ambientMode,
			fogDensity = RenderSettings.fogDensity,
			fogColor = RenderSettings.fogColor,
			fogMode = RenderSettings.fogMode,
			fogEndDistance = RenderSettings.fogEndDistance,
			fogStartDistance = RenderSettings.fogStartDistance,
			fog = RenderSettings.fog,
			ambientIntensity = RenderSettings.ambientIntensity,
			flareFadeSpeed = RenderSettings.flareFadeSpeed
		};
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00009140 File Offset: 0x00007340
	public void Apply()
	{
		RenderSettings.haloStrength = this.haloStrength;
		RenderSettings.defaultReflectionResolution = this.defaultReflectionResolution;
		RenderSettings.defaultReflectionMode = this.defaultReflectionMode;
		RenderSettings.reflectionBounces = this.reflectionBounces;
		RenderSettings.reflectionIntensity = this.reflectionIntensity;
		RenderSettings.customReflection = this.customReflection;
		RenderSettings.ambientProbe = this.ambientProbe;
		RenderSettings.sun = this.sun;
		RenderSettings.skybox = this.skybox;
		RenderSettings.subtractiveShadowColor = this.subtractiveShadowColor;
		RenderSettings.flareStrength = this.flareStrength;
		RenderSettings.ambientLight = this.ambientLight;
		RenderSettings.ambientGroundColor = this.ambientGroundColor;
		RenderSettings.ambientEquatorColor = this.ambientEquatorColor;
		RenderSettings.ambientSkyColor = this.ambientSkyColor;
		RenderSettings.ambientMode = this.ambientMode;
		RenderSettings.fogDensity = this.fogDensity;
		RenderSettings.fogColor = this.fogColor;
		RenderSettings.fogMode = this.fogMode;
		RenderSettings.fogEndDistance = this.fogEndDistance;
		RenderSettings.fogStartDistance = this.fogStartDistance;
		RenderSettings.fog = this.fog;
		RenderSettings.ambientIntensity = this.ambientIntensity;
		RenderSettings.flareFadeSpeed = this.flareFadeSpeed;
	}

	// Token: 0x040001D1 RID: 465
	public float haloStrength;

	// Token: 0x040001D2 RID: 466
	public int defaultReflectionResolution;

	// Token: 0x040001D3 RID: 467
	public DefaultReflectionMode defaultReflectionMode;

	// Token: 0x040001D4 RID: 468
	public int reflectionBounces;

	// Token: 0x040001D5 RID: 469
	public float reflectionIntensity;

	// Token: 0x040001D6 RID: 470
	public Cubemap customReflection;

	// Token: 0x040001D7 RID: 471
	public SphericalHarmonicsL2 ambientProbe;

	// Token: 0x040001D8 RID: 472
	public Light sun;

	// Token: 0x040001D9 RID: 473
	public Material skybox;

	// Token: 0x040001DA RID: 474
	public Color subtractiveShadowColor;

	// Token: 0x040001DB RID: 475
	public float flareStrength;

	// Token: 0x040001DC RID: 476
	[ColorUsage(false, true)]
	public Color ambientLight;

	// Token: 0x040001DD RID: 477
	[ColorUsage(false, true)]
	public Color ambientGroundColor;

	// Token: 0x040001DE RID: 478
	[ColorUsage(false, true)]
	public Color ambientEquatorColor;

	// Token: 0x040001DF RID: 479
	[ColorUsage(false, true)]
	public Color ambientSkyColor;

	// Token: 0x040001E0 RID: 480
	public AmbientMode ambientMode;

	// Token: 0x040001E1 RID: 481
	public float fogDensity;

	// Token: 0x040001E2 RID: 482
	[ColorUsage(true, false)]
	public Color fogColor;

	// Token: 0x040001E3 RID: 483
	public FogMode fogMode;

	// Token: 0x040001E4 RID: 484
	public float fogEndDistance;

	// Token: 0x040001E5 RID: 485
	public float fogStartDistance;

	// Token: 0x040001E6 RID: 486
	public bool fog;

	// Token: 0x040001E7 RID: 487
	public float ambientIntensity;

	// Token: 0x040001E8 RID: 488
	public float flareFadeSpeed;
}
