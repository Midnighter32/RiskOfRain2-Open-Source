using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C1 RID: 705
	public class DamageTrail : MonoBehaviour
	{
		// Token: 0x06000E53 RID: 3667 RVA: 0x0004686D File Offset: 0x00044A6D
		private void Awake()
		{
			this.pointsList = new List<DamageTrail.TrailPoint>();
			this.transform = base.transform;
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x00046886 File Offset: 0x00044A86
		private void Start()
		{
			this.localTime = 0f;
			this.AddPoint();
			this.AddPoint();
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x000468A0 File Offset: 0x00044AA0
		private void FixedUpdate()
		{
			this.localTime += Time.fixedDeltaTime;
			if (this.localTime >= this.nextUpdate)
			{
				this.nextUpdate += this.updateInterval;
				this.UpdateTrail();
			}
			if (this.pointsList.Count > 0)
			{
				DamageTrail.TrailPoint trailPoint = this.pointsList[this.pointsList.Count - 1];
				trailPoint.position = this.transform.position;
				trailPoint.localEndTime = this.localTime + this.pointLifetime;
				this.pointsList[this.pointsList.Count - 1] = trailPoint;
				if (trailPoint.segmentTransform)
				{
					trailPoint.segmentTransform.position = this.transform.position;
				}
				if (this.lineRenderer)
				{
					this.lineRenderer.SetPosition(this.pointsList.Count - 1, trailPoint.position);
				}
			}
			if (this.segmentPrefab)
			{
				Vector3 position = this.transform.position;
				for (int i = this.pointsList.Count - 1; i >= 0; i--)
				{
					Transform segmentTransform = this.pointsList[i].segmentTransform;
					segmentTransform.LookAt(position, Vector3.up);
					Vector3 a = this.pointsList[i].position - position;
					segmentTransform.position = position + a * 0.5f;
					float num = Mathf.Clamp01(Mathf.InverseLerp(this.pointsList[i].localStartTime, this.pointsList[i].localEndTime, this.localTime));
					Vector3 localScale = new Vector3(this.radius * (1f - num), this.radius * (1f - num), a.magnitude);
					segmentTransform.localScale = localScale;
					position = this.pointsList[i].position;
				}
			}
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00046A9C File Offset: 0x00044C9C
		private void UpdateTrail()
		{
			while (this.pointsList.Count > 0 && this.pointsList[0].localEndTime <= this.localTime)
			{
				this.RemovePoint(0);
			}
			this.AddPoint();
			if (NetworkServer.active)
			{
				this.DoDamage();
			}
			if (this.lineRenderer)
			{
				this.UpdateLineRenderer(this.lineRenderer);
			}
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00046B08 File Offset: 0x00044D08
		private void DoDamage()
		{
			if (this.pointsList.Count == 0)
			{
				return;
			}
			float damage = this.damagePerSecond * this.updateInterval;
			Vector3 vector = this.pointsList[this.pointsList.Count - 1].position;
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			TeamIndex teamIndex = TeamIndex.Neutral;
			if (this.owner)
			{
				hashSet.Add(this.owner);
				teamIndex = TeamComponent.GetObjectTeam(this.owner);
			}
			for (int i = this.pointsList.Count - 2; i >= 0; i--)
			{
				Vector3 position = this.pointsList[i].position;
				Vector3 direction = position - vector;
				RaycastHit[] array = Physics.SphereCastAll(new Ray(vector, direction), this.radius, direction.magnitude, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
				for (int j = 0; j < array.Length; j++)
				{
					Collider collider = array[j].collider;
					if (collider.gameObject)
					{
						HurtBox component = collider.GetComponent<HurtBox>();
						if (component)
						{
							HealthComponent healthComponent = component.healthComponent;
							if (healthComponent)
							{
								GameObject gameObject = healthComponent.gameObject;
								if (!hashSet.Contains(gameObject))
								{
									hashSet.Add(gameObject);
									if (TeamComponent.GetObjectTeam(gameObject) != teamIndex)
									{
										healthComponent.TakeDamage(new DamageInfo
										{
											position = array[j].point,
											attacker = this.owner,
											inflictor = base.gameObject,
											crit = false,
											damage = damage,
											damageColorIndex = DamageColorIndex.Item,
											damageType = DamageType.Generic,
											force = Vector3.zero,
											procCoefficient = 0f
										});
									}
								}
							}
						}
					}
				}
				vector = position;
			}
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00046CF4 File Offset: 0x00044EF4
		private void UpdateLineRenderer(LineRenderer lineRenderer)
		{
			lineRenderer.positionCount = this.pointsList.Count;
			for (int i = 0; i < this.pointsList.Count; i++)
			{
				lineRenderer.SetPosition(i, this.pointsList[i].position);
			}
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00046D40 File Offset: 0x00044F40
		private void AddPoint()
		{
			DamageTrail.TrailPoint item = new DamageTrail.TrailPoint
			{
				position = this.transform.position,
				localStartTime = this.localTime,
				localEndTime = this.localTime + this.pointLifetime
			};
			if (this.segmentPrefab)
			{
				item.segmentTransform = UnityEngine.Object.Instantiate<GameObject>(this.segmentPrefab, this.transform).transform;
			}
			this.pointsList.Add(item);
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x00046DC0 File Offset: 0x00044FC0
		private void RemovePoint(int pointIndex)
		{
			if (this.destroyTrailSegments && this.pointsList[pointIndex].segmentTransform)
			{
				UnityEngine.Object.Destroy(this.pointsList[pointIndex].segmentTransform.gameObject);
			}
			this.pointsList.RemoveAt(pointIndex);
		}

		// Token: 0x04001240 RID: 4672
		[Tooltip("How often to drop a new point onto the trail and do damage.")]
		public float updateInterval = 0.2f;

		// Token: 0x04001241 RID: 4673
		[Tooltip("How large the radius of the damage detection should be.")]
		public float radius = 0.5f;

		// Token: 0x04001242 RID: 4674
		[Tooltip("How long a point on the trail should last.")]
		public float pointLifetime = 3f;

		// Token: 0x04001243 RID: 4675
		[Tooltip("The line renderer to use for display.")]
		public LineRenderer lineRenderer;

		// Token: 0x04001244 RID: 4676
		[Tooltip("Prefab to use per segment.")]
		public GameObject segmentPrefab;

		// Token: 0x04001245 RID: 4677
		public bool destroyTrailSegments;

		// Token: 0x04001246 RID: 4678
		public float damagePerSecond;

		// Token: 0x04001247 RID: 4679
		public GameObject owner;

		// Token: 0x04001248 RID: 4680
		private new Transform transform;

		// Token: 0x04001249 RID: 4681
		private List<DamageTrail.TrailPoint> pointsList;

		// Token: 0x0400124A RID: 4682
		private float localTime;

		// Token: 0x0400124B RID: 4683
		private float nextUpdate;

		// Token: 0x020002C2 RID: 706
		private struct TrailPoint
		{
			// Token: 0x0400124C RID: 4684
			public Vector3 position;

			// Token: 0x0400124D RID: 4685
			public float localStartTime;

			// Token: 0x0400124E RID: 4686
			public float localEndTime;

			// Token: 0x0400124F RID: 4687
			public Transform segmentTransform;
		}
	}
}
