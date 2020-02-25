using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200032A RID: 810
	public class ShakeEmitter : MonoBehaviour
	{
		// Token: 0x06001316 RID: 4886 RVA: 0x00051BFC File Offset: 0x0004FDFC
		public void StartShake()
		{
			this.stopwatch = 0f;
			this.halfPeriodVector = UnityEngine.Random.onUnitSphere;
			this.halfPeriodTimer = this.wave.period * 0.5f;
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x00051C2B File Offset: 0x0004FE2B
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

		// Token: 0x06001318 RID: 4888 RVA: 0x00051C60 File Offset: 0x0004FE60
		private void OnEnable()
		{
			ShakeEmitter.instances.Add(this);
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x00051C6D File Offset: 0x0004FE6D
		private void OnDisable()
		{
			ShakeEmitter.instances.Remove(this);
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x00051C7B File Offset: 0x0004FE7B
		private void OnValidate()
		{
			if (this.wave.frequency == 0f)
			{
				this.wave.frequency = 1f;
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
			}
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x00051CA9 File Offset: 0x0004FEA9
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ShakeEmitter.UpdateAll;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x00051CBC File Offset: 0x0004FEBC
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

		// Token: 0x0600131D RID: 4893 RVA: 0x00051CFE File Offset: 0x0004FEFE
		public float CurrentShakeFade()
		{
			if (!this.amplitudeTimeDecay)
			{
				return 1f;
			}
			return 1f - this.stopwatch / this.duration;
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x00051D24 File Offset: 0x0004FF24
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

		// Token: 0x0600131F RID: 4895 RVA: 0x00051DE4 File Offset: 0x0004FFE4
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

		// Token: 0x06001320 RID: 4896 RVA: 0x00051EEC File Offset: 0x000500EC
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

		// Token: 0x06001321 RID: 4897 RVA: 0x00051F78 File Offset: 0x00050178
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

		// Token: 0x040011E3 RID: 4579
		private static readonly List<ShakeEmitter> instances = new List<ShakeEmitter>();

		// Token: 0x040011E4 RID: 4580
		[Tooltip("Whether or not to begin shaking as soon as this instance becomes active.")]
		public bool shakeOnStart = true;

		// Token: 0x040011E5 RID: 4581
		[Tooltip("The wave description of this motion.")]
		public Wave wave = new Wave
		{
			amplitude = 1f,
			frequency = 1f,
			cycleOffset = 0f
		};

		// Token: 0x040011E6 RID: 4582
		[Tooltip("How long the shake lasts, in seconds.")]
		public float duration = 1f;

		// Token: 0x040011E7 RID: 4583
		[Tooltip("How far the wave reaches.")]
		public float radius = 10f;

		// Token: 0x040011E8 RID: 4584
		[Tooltip("Whether or not the radius should be multiplied with local scale.")]
		public bool scaleShakeRadiusWithLocalScale;

		// Token: 0x040011E9 RID: 4585
		[Tooltip("Whether or not the ampitude should decay with time.")]
		public bool amplitudeTimeDecay = true;

		// Token: 0x040011EA RID: 4586
		private float stopwatch = float.PositiveInfinity;

		// Token: 0x040011EB RID: 4587
		private float halfPeriodTimer;

		// Token: 0x040011EC RID: 4588
		private Vector3 halfPeriodVector;

		// Token: 0x040011ED RID: 4589
		private Vector3 currentOffset;

		// Token: 0x040011EE RID: 4590
		private const float deepRumbleFactor = 5f;

		// Token: 0x0200032B RID: 811
		public struct MotorBias
		{
			// Token: 0x040011EF RID: 4591
			public float deepLeftBias;

			// Token: 0x040011F0 RID: 4592
			public float quickRightBias;
		}
	}
}
