using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000320 RID: 800
	[ExecuteInEditMode]
	public class SceneWeatherController : MonoBehaviour
	{
		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060012CF RID: 4815 RVA: 0x00050C88 File Offset: 0x0004EE88
		public static SceneWeatherController instance
		{
			get
			{
				return SceneWeatherController._instance;
			}
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x00050C8F File Offset: 0x0004EE8F
		private void OnEnable()
		{
			if (!SceneWeatherController._instance)
			{
				SceneWeatherController._instance = this;
			}
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00050CA3 File Offset: 0x0004EEA3
		private void OnDisable()
		{
			if (SceneWeatherController._instance == this)
			{
				SceneWeatherController._instance = null;
			}
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00050CB8 File Offset: 0x0004EEB8
		private SceneWeatherController.WeatherParams GetWeatherParams(float t)
		{
			return new SceneWeatherController.WeatherParams
			{
				sunColor = Color.Lerp(this.initialWeatherParams.sunColor, this.finalWeatherParams.sunColor, t),
				sunIntensity = Mathf.Lerp(this.initialWeatherParams.sunIntensity, this.finalWeatherParams.sunIntensity, t),
				fogStart = Mathf.Lerp(this.initialWeatherParams.fogStart, this.finalWeatherParams.fogStart, t),
				fogScale = Mathf.Lerp(this.initialWeatherParams.fogScale, this.finalWeatherParams.fogScale, t),
				fogIntensity = Mathf.Lerp(this.initialWeatherParams.fogIntensity, this.finalWeatherParams.fogIntensity, t)
			};
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x00050D80 File Offset: 0x0004EF80
		private void Update()
		{
			SceneWeatherController.WeatherParams weatherParams = this.GetWeatherParams(this.weatherLerp);
			if (this.sun)
			{
				this.sun.color = weatherParams.sunColor;
				this.sun.intensity = weatherParams.sunIntensity;
			}
			if (this.fogMaterial)
			{
				this.fogMaterial.SetFloat("_FogPicker", this.weatherLerp);
				this.fogMaterial.SetFloat("_FogStart", weatherParams.fogStart);
				this.fogMaterial.SetFloat("_FogScale", weatherParams.fogScale);
				this.fogMaterial.SetFloat("_FogIntensity", weatherParams.fogIntensity);
			}
			if (true && this.rtpcWeather.Length != 0)
			{
				AkSoundEngine.SetRTPCValue(this.rtpcWeather, Mathf.Lerp(this.rtpcMin, this.rtpcMax, this.weatherLerp), base.gameObject);
			}
		}

		// Token: 0x040011A7 RID: 4519
		private static SceneWeatherController _instance;

		// Token: 0x040011A8 RID: 4520
		public SceneWeatherController.WeatherParams initialWeatherParams;

		// Token: 0x040011A9 RID: 4521
		public SceneWeatherController.WeatherParams finalWeatherParams;

		// Token: 0x040011AA RID: 4522
		public Light sun;

		// Token: 0x040011AB RID: 4523
		public Material fogMaterial;

		// Token: 0x040011AC RID: 4524
		public string rtpcWeather;

		// Token: 0x040011AD RID: 4525
		public float rtpcMin;

		// Token: 0x040011AE RID: 4526
		public float rtpcMax = 100f;

		// Token: 0x040011AF RID: 4527
		public AnimationCurve weatherLerpOverChargeTime;

		// Token: 0x040011B0 RID: 4528
		[Range(0f, 1f)]
		public float weatherLerp;

		// Token: 0x02000321 RID: 801
		[Serializable]
		public struct WeatherParams
		{
			// Token: 0x040011B1 RID: 4529
			[ColorUsage(true, true, 1f, 5f, 1f, 5f)]
			public Color sunColor;

			// Token: 0x040011B2 RID: 4530
			public float sunIntensity;

			// Token: 0x040011B3 RID: 4531
			public float fogStart;

			// Token: 0x040011B4 RID: 4532
			public float fogScale;

			// Token: 0x040011B5 RID: 4533
			public float fogIntensity;
		}
	}
}
