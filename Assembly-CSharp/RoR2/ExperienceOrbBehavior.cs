using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FC RID: 508
	public class ExperienceOrbBehavior : MonoBehaviour
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x0002FD41 File Offset: 0x0002DF41
		// (set) Token: 0x06000AD2 RID: 2770 RVA: 0x0002FD49 File Offset: 0x0002DF49
		public Transform targetTransform { get; set; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x0002FD52 File Offset: 0x0002DF52
		// (set) Token: 0x06000AD4 RID: 2772 RVA: 0x0002FD5A File Offset: 0x0002DF5A
		public float travelTime { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x0002FD63 File Offset: 0x0002DF63
		// (set) Token: 0x06000AD6 RID: 2774 RVA: 0x0002FD6B File Offset: 0x0002DF6B
		public ulong exp { get; set; }

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0002FD74 File Offset: 0x0002DF74
		private void Awake()
		{
			this.transform = base.transform;
			this.trail = base.GetComponent<TrailRenderer>();
			this.light = base.GetComponent<Light>();
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0002FD9C File Offset: 0x0002DF9C
		private void Start()
		{
			this.localTime = 0f;
			this.consumed = false;
			this.startPos = this.transform.position;
			this.previousPos = this.startPos;
			this.scale = 2f * Mathf.Log(this.exp + 1f, 6f);
			this.initialVelocity = (Vector3.up * 4f + UnityEngine.Random.insideUnitSphere * 1f) * this.scale;
			this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
			this.trail.startWidth = 0.05f * this.scale;
			if (this.light)
			{
				this.light.range = 1f * this.scale;
			}
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0002FE90 File Offset: 0x0002E090
		private void Update()
		{
			this.localTime += Time.deltaTime;
			if (!this.targetTransform)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			float num = Mathf.Clamp01(this.localTime / this.travelTime);
			this.previousPos = this.transform.position;
			this.transform.position = ExperienceOrbBehavior.CalculatePosition(this.startPos, this.initialVelocity, this.targetTransform.position, num);
			if (num >= 1f)
			{
				this.OnHitTarget();
				return;
			}
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0002FF24 File Offset: 0x0002E124
		private static Vector3 CalculatePosition(Vector3 startPos, Vector3 initialVelocity, Vector3 targetPos, float t)
		{
			Vector3 a = startPos + initialVelocity * t;
			float t2 = t * t * t;
			return Vector3.LerpUnclamped(a, targetPos, t2);
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0002FF4D File Offset: 0x0002E14D
		private void OnTriggerStay(Collider other)
		{
			if (other.transform == this.targetTransform)
			{
				this.OnHitTarget();
			}
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0002FF68 File Offset: 0x0002E168
		private void OnHitTarget()
		{
			if (!this.consumed)
			{
				this.consumed = true;
				Util.PlaySound(ExperienceOrbBehavior.expSoundString, this.targetTransform.gameObject);
				UnityEngine.Object.Instantiate<GameObject>(this.hitEffectPrefab, this.transform.position, Util.QuaternionSafeLookRotation(this.previousPos - this.startPos)).transform.localScale *= this.scale;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000B1B RID: 2843
		public GameObject hitEffectPrefab;

		// Token: 0x04000B1C RID: 2844
		private new Transform transform;

		// Token: 0x04000B1D RID: 2845
		private TrailRenderer trail;

		// Token: 0x04000B1E RID: 2846
		private Light light;

		// Token: 0x04000B22 RID: 2850
		private float localTime;

		// Token: 0x04000B23 RID: 2851
		private Vector3 startPos;

		// Token: 0x04000B24 RID: 2852
		private Vector3 previousPos;

		// Token: 0x04000B25 RID: 2853
		private Vector3 initialVelocity;

		// Token: 0x04000B26 RID: 2854
		private float scale;

		// Token: 0x04000B27 RID: 2855
		private bool consumed;

		// Token: 0x04000B28 RID: 2856
		private static readonly string expSoundString = "Play_UI_xp_gain";
	}
}
