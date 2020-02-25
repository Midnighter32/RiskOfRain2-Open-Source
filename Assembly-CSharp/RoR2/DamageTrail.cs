using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001D0 RID: 464
	public class DamageTrail : MonoBehaviour
	{
		// Token: 0x060009F3 RID: 2547 RVA: 0x0002B694 File Offset: 0x00029894
		private void Awake()
		{
			this.pointsList = new List<DamageTrail.TrailPoint>();
			this.transform = base.transform;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x0002B6AD File Offset: 0x000298AD
		private void Start()
		{
			this.localTime = 0f;
			this.AddPoint();
			this.AddPoint();
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x0002B6C8 File Offset: 0x000298C8
		private void FixedUpdate()
		{
			this.localTime += Time.fixedDeltaTime;
			if (this.localTime >= this.nextUpdate)
			{
				this.nextUpdate += this.updateInterval;
				this.UpdateTrail(this.active);
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

		// Token: 0x060009F6 RID: 2550 RVA: 0x0002B8CC File Offset: 0x00029ACC
		private void UpdateTrail(bool addPoint)
		{
			while (this.pointsList.Count > 0 && this.pointsList[0].localEndTime <= this.localTime)
			{
				this.RemovePoint(0);
			}
			if (addPoint)
			{
				this.AddPoint();
			}
			if (NetworkServer.active)
			{
				this.DoDamage();
			}
			if (this.lineRenderer)
			{
				this.UpdateLineRenderer(this.lineRenderer);
			}
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x0002B938 File Offset: 0x00029B38
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

		// Token: 0x060009F8 RID: 2552 RVA: 0x0002BB24 File Offset: 0x00029D24
		private void UpdateLineRenderer(LineRenderer lineRenderer)
		{
			lineRenderer.positionCount = this.pointsList.Count;
			for (int i = 0; i < this.pointsList.Count; i++)
			{
				lineRenderer.SetPosition(i, this.pointsList[i].position);
			}
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x0002BB70 File Offset: 0x00029D70
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

		// Token: 0x060009FA RID: 2554 RVA: 0x0002BBF0 File Offset: 0x00029DF0
		private void RemovePoint(int pointIndex)
		{
			if (this.destroyTrailSegments && this.pointsList[pointIndex].segmentTransform)
			{
				UnityEngine.Object.Destroy(this.pointsList[pointIndex].segmentTransform.gameObject);
			}
			this.pointsList.RemoveAt(pointIndex);
		}

		// Token: 0x04000A27 RID: 2599
		[Tooltip("How often to drop a new point onto the trail and do damage.")]
		public float updateInterval = 0.2f;

		// Token: 0x04000A28 RID: 2600
		[Tooltip("How large the radius of the damage detection should be.")]
		public float radius = 0.5f;

		// Token: 0x04000A29 RID: 2601
		[Tooltip("How long a point on the trail should last.")]
		public float pointLifetime = 3f;

		// Token: 0x04000A2A RID: 2602
		[Tooltip("The line renderer to use for display.")]
		public LineRenderer lineRenderer;

		// Token: 0x04000A2B RID: 2603
		public bool active = true;

		// Token: 0x04000A2C RID: 2604
		[Tooltip("Prefab to use per segment.")]
		public GameObject segmentPrefab;

		// Token: 0x04000A2D RID: 2605
		public bool destroyTrailSegments;

		// Token: 0x04000A2E RID: 2606
		public float damagePerSecond;

		// Token: 0x04000A2F RID: 2607
		public GameObject owner;

		// Token: 0x04000A30 RID: 2608
		private new Transform transform;

		// Token: 0x04000A31 RID: 2609
		private List<DamageTrail.TrailPoint> pointsList;

		// Token: 0x04000A32 RID: 2610
		private float localTime;

		// Token: 0x04000A33 RID: 2611
		private float nextUpdate;

		// Token: 0x020001D1 RID: 465
		private struct TrailPoint
		{
			// Token: 0x04000A34 RID: 2612
			public Vector3 position;

			// Token: 0x04000A35 RID: 2613
			public float localStartTime;

			// Token: 0x04000A36 RID: 2614
			public float localEndTime;

			// Token: 0x04000A37 RID: 2615
			public Transform segmentTransform;
		}
	}
}
