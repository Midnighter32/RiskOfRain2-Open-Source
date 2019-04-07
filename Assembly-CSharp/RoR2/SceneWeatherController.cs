using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D2 RID: 978
	[ExecuteInEditMode]
	public class SceneWeatherController : MonoBehaviour
	{
		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x00065E4C File Offset: 0x0006404C
		public static SceneWeatherController instance
		{
			get
			{
				return SceneWeatherController._instance;
			}
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x00065E53 File Offset: 0x00064053
		private void OnEnable()
		{
			if (!SceneWeatherController._instance)
			{
				SceneWeatherController._instance = this;
			}
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00065E67 File Offset: 0x00064067
		private void OnDisable()
		{
			if (SceneWeatherController._instance == this)
			{
				SceneWeatherController._instance = null;
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00065E7C File Offset: 0x0006407C
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

		// Token: 0x0600153A RID: 5434 RVA: 0x00065F44 File Offset: 0x00064144
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

		// Token: 0x04001884 RID: 6276
		private static SceneWeatherController _instance;

		// Token: 0x04001885 RID: 6277
		public SceneWeatherController.WeatherParams initialWeatherParams;

		// Token: 0x04001886 RID: 6278
		public SceneWeatherController.WeatherParams finalWeatherParams;

		// Token: 0x04001887 RID: 6279
		public Light sun;

		// Token: 0x04001888 RID: 6280
		public Material fogMaterial;

		// Token: 0x04001889 RID: 6281
		public string rtpcWeather;

		// Token: 0x0400188A RID: 6282
		public float rtpcMin;

		// Token: 0x0400188B RID: 6283
		public float rtpcMax = 100f;

		// Token: 0x0400188C RID: 6284
		public AnimationCurve weatherLerpOverChargeTime;

		// Token: 0x0400188D RID: 6285
		[Range(0f, 1f)]
		public float weatherLerp;

		// Token: 0x020003D3 RID: 979
		[Serializable]
		public struct WeatherParams
		{
			// Token: 0x0400188E RID: 6286
			[ColorUsage(true, true, 1f, 5f, 1f, 5f)]
			public Color sunColor;

			// Token: 0x0400188F RID: 6287
			public float sunIntensity;

			// Token: 0x04001890 RID: 6288
			public float fogStart;

			// Token: 0x04001891 RID: 6289
			public float fogScale;

			// Token: 0x04001892 RID: 6290
			public float fogIntensity;
		}
	}
}
