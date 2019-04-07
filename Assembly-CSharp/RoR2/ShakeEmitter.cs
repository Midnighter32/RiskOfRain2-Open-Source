using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D9 RID: 985
	public class ShakeEmitter : MonoBehaviour
	{
		// Token: 0x06001552 RID: 5458 RVA: 0x0006640C File Offset: 0x0006460C
		public void StartShake()
		{
			this.stopwatch = 0f;
			this.halfPeriodVector = UnityEngine.Random.onUnitSphere;
			this.halfPeriodTimer = this.wave.period * 0.5f;
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0006643B File Offset: 0x0006463B
		private void Start()
		{
			if (this.scaleShakeRadiusWithLocalScale)
			{
				this.radius *= base.transform.localScale.x;
			}
			if (this.shakeOnStart)
			{
				this.StartShake();
			}
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x00066470 File Offset: 0x00064670
		private void OnEnable()
		{
			ShakeEmitter.instances.Add(this);
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0006647D File Offset: 0x0006467D
		private void OnDisable()
		{
			ShakeEmitter.instances.Remove(this);
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0006648B File Offset: 0x0006468B
		private void OnValidate()
		{
			if (this.wave.frequency == 0f)
			{
				this.wave.frequency = 1f;
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
			}
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000664B9 File Offset: 0x000646B9
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ShakeEmitter.UpdateAll;
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x000664CC File Offset: 0x000646CC
		public static void UpdateAll()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime == 0f)
			{
				return;
			}
			for (int i = 0; i < ShakeEmitter.instances.Count; i++)
			{
				ShakeEmitter.instances[i].ManualUpdate(deltaTime);
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0006650E File Offset: 0x0006470E
		public float CurrentShakeFade()
		{
			if (!this.amplitudeTimeDecay)
			{
				return 1f;
			}
			return 1f - this.stopwatch / this.duration;
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00066534 File Offset: 0x00064734
		public void ManualUpdate(float deltaTime)
		{
			this.stopwatch += deltaTime;
			if (this.stopwatch < this.duration)
			{
				float d = this.CurrentShakeFade();
				this.halfPeriodTimer -= deltaTime;
				if (this.halfPeriodTimer < 0f)
				{
					this.halfPeriodVector = Vector3.Slerp(UnityEngine.Random.onUnitSphere, -this.halfPeriodVector, 0.5f);
					this.halfPeriodTimer += this.wave.period * 0.5f;
				}
				this.currentOffset = this.halfPeriodVector * this.wave.Evaluate(this.halfPeriodTimer) * d;
				return;
			}
			this.currentOffset = Vector3.zero;
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x000665F4 File Offset: 0x000647F4
		public static void ApplySpacialRumble(LocalUser localUser, Transform cameraTransform)
		{
			Vector3 right = cameraTransform.right;
			Vector3 position = cameraTransform.position;
			float num = 0f;
			float num2 = 0f;
			int i = 0;
			int count = ShakeEmitter.instances.Count;
			while (i < count)
			{
				ShakeEmitter shakeEmitter = ShakeEmitter.instances[i];
				Vector3 position2 = shakeEmitter.transform.position;
				float value = Vector3.Dot(position2 - position, right);
				float sqrMagnitude = (position - position2).sqrMagnitude;
				float num3 = shakeEmitter.radius;
				float num4 = 0f;
				if (sqrMagnitude < num3 * num3)
				{
					float num5 = 1f - Mathf.Sqrt(sqrMagnitude) / num3;
					num4 = shakeEmitter.CurrentShakeFade() * shakeEmitter.wave.amplitude * num5;
				}
				float num6 = Mathf.Clamp01(Util.Remap(value, -1f, 1f, 0f, 1f));
				float num7 = num4;
				num += num7 * (1f - num6);
				num2 += num7 * num6;
				i++;
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000666FC File Offset: 0x000648FC
		public static void ApplyDeepQuickRumble(LocalUser localUser, Vector3 bonusPosition)
		{
			float magnitude = bonusPosition.magnitude;
			float gamepadVibrationScale = localUser.userProfile.gamepadVibrationScale;
			localUser.inputPlayer.SetVibration(0, magnitude * gamepadVibrationScale / 5f);
			localUser.inputPlayer.SetVibration(1, magnitude * gamepadVibrationScale);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00066744 File Offset: 0x00064944
		public static Vector3 ComputeTotalShakeAtPoint(Vector3 position)
		{
			Vector3 vector = Vector3.zero;
			int i = 0;
			int count = ShakeEmitter.instances.Count;
			while (i < count)
			{
				ShakeEmitter shakeEmitter = ShakeEmitter.instances[i];
				float sqrMagnitude = (position - shakeEmitter.transform.position).sqrMagnitude;
				float num = shakeEmitter.radius;
				if (sqrMagnitude < num * num)
				{
					float d = 1f - Mathf.Sqrt(sqrMagnitude) / num;
					vector += shakeEmitter.currentOffset * d;
				}
				i++;
			}
			return vector;
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x000667D0 File Offset: 0x000649D0
		public static ShakeEmitter CreateSimpleShakeEmitter(Vector3 position, Wave wave, float duration, float radius, bool amplitudeTimeDecay)
		{
			if (wave.frequency == 0f)
			{
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
				wave.frequency = 1f;
			}
			GameObject gameObject = new GameObject("ShakeEmitter", new Type[]
			{
				typeof(ShakeEmitter),
				typeof(DestroyOnTimer)
			});
			ShakeEmitter component = gameObject.GetComponent<ShakeEmitter>();
			DestroyOnTimer component2 = gameObject.GetComponent<DestroyOnTimer>();
			gameObject.transform.position = position;
			component.wave = wave;
			component.duration = duration;
			component.radius = radius;
			component.amplitudeTimeDecay = amplitudeTimeDecay;
			component2.duration = duration;
			return component;
		}

		// Token: 0x040018A7 RID: 6311
		private static readonly List<ShakeEmitter> instances = new List<ShakeEmitter>();

		// Token: 0x040018A8 RID: 6312
		[Tooltip("Whether or not to begin shaking as soon as this instance becomes active.")]
		public bool shakeOnStart = true;

		// Token: 0x040018A9 RID: 6313
		[Tooltip("The wave description of this motion.")]
		public Wave wave = new Wave
		{
			amplitude = 1f,
			frequency = 1f,
			cycleOffset = 0f
		};

		// Token: 0x040018AA RID: 6314
		[Tooltip("How long the shake lasts, in seconds.")]
		public float duration = 1f;

		// Token: 0x040018AB RID: 6315
		[Tooltip("How far the wave reaches.")]
		public float radius = 10f;

		// Token: 0x040018AC RID: 6316
		[Tooltip("Whether or not the radius should be multiplied with local scale.")]
		public bool scaleShakeRadiusWithLocalScale;

		// Token: 0x040018AD RID: 6317
		[Tooltip("Whether or not the ampitude should decay with time.")]
		public bool amplitudeTimeDecay = true;

		// Token: 0x040018AE RID: 6318
		private float stopwatch = float.PositiveInfinity;

		// Token: 0x040018AF RID: 6319
		private float halfPeriodTimer;

		// Token: 0x040018B0 RID: 6320
		private Vector3 halfPeriodVector;

		// Token: 0x040018B1 RID: 6321
		private Vector3 currentOffset;

		// Token: 0x040018B2 RID: 6322
		private const float deepRumbleFactor = 5f;

		// Token: 0x020003DA RID: 986
		public struct MotorBias
		{
			// Token: 0x040018B3 RID: 6323
			public float deepLeftBias;

			// Token: 0x040018B4 RID: 6324
			public float quickRightBias;
		}
	}
}
