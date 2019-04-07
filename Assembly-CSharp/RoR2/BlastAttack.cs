using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000201 RID: 513
	public class BlastAttack
	{
		// Token: 0x06000A05 RID: 2565 RVA: 0x00031D38 File Offset: 0x0002FF38
		public void Fire()
		{
			Collider[] array = Physics.OverlapSphere(this.position, this.radius, LayerIndex.entityPrecise.mask);
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				Collider collider = array[i];
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent && ((this.canHurtAttacker && healthComponent.gameObject == this.attacker) || healthComponent.GetComponent<TeamComponent>().teamIndex != this.teamIndex))
					{
						BlastAttack.HitPoint hitPoint = default(BlastAttack.HitPoint);
						RaycastHit raycastHit = default(RaycastHit);
						hitPoint.hurtBox = component;
						Vector3 direction = collider.transform.position - this.position;
						if (direction.sqrMagnitude > 0f && collider.Raycast(new Ray(this.position, direction), out raycastHit, this.radius))
						{
							hitPoint.hitPosition = raycastHit.point;
							hitPoint.hitNormal = raycastHit.normal;
						}
						else
						{
							hitPoint.hitPosition = collider.transform.position;
							hitPoint.hitNormal = this.position - hitPoint.hitPosition;
						}
						hitPoint.distanceSqr = (hitPoint.hitPosition - this.position).sqrMagnitude;
						if (!BlastAttack.bestHitPoints.ContainsKey(healthComponent) || BlastAttack.bestHitPoints[healthComponent].distanceSqr > hitPoint.distanceSqr)
						{
							BlastAttack.bestHitPoints[healthComponent] = hitPoint;
						}
					}
				}
			}
			BlastAttack.HitPoint[] array2 = new BlastAttack.HitPoint[BlastAttack.bestHitPoints.Count];
			int num2 = 0;
			foreach (KeyValuePair<HealthComponent, BlastAttack.HitPoint> keyValuePair in BlastAttack.bestHitPoints)
			{
				array2[num2++] = keyValuePair.Value;
			}
			BlastAttack.bestHitPoints.Clear();
			Array.Sort<BlastAttack.HitPoint>(array2, new Comparison<BlastAttack.HitPoint>(BlastAttack.HitPoint.DistanceSort));
			foreach (BlastAttack.HitPoint hitPoint2 in array2)
			{
				float num3 = Mathf.Sqrt(hitPoint2.distanceSqr);
				float num4 = 0f;
				switch (this.falloffModel)
				{
				case BlastAttack.FalloffModel.None:
					num4 = 1f;
					break;
				case BlastAttack.FalloffModel.Linear:
					num4 = 1f - Mathf.Clamp01(num3 / this.radius);
					break;
				case BlastAttack.FalloffModel.SweetSpot:
					num4 = 1f - ((num3 > this.radius / 2f) ? 0.75f : 0f);
					break;
				}
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.attacker = this.attacker;
				damageInfo.inflictor = this.inflictor;
				damageInfo.damage = this.baseDamage * num4;
				damageInfo.crit = this.crit;
				damageInfo.force = this.bonusForce * num4 + this.baseForce * num4 * (hitPoint2.hitPosition - this.position).normalized;
				damageInfo.procChainMask = this.procChainMask;
				damageInfo.procCoefficient = this.procCoefficient;
				damageInfo.damageType = this.damageType;
				damageInfo.damageColorIndex = this.damageColorIndex;
				damageInfo.position = hitPoint2.hitPosition;
				damageInfo.ModifyDamageInfo(hitPoint2.hurtBox.damageModifier);
				hitPoint2.hurtBox.healthComponent.TakeDamage(damageInfo);
				GlobalEventManager.instance.OnHitEnemy(damageInfo, hitPoint2.hurtBox.healthComponent.gameObject);
				GlobalEventManager.instance.OnHitAll(damageInfo, hitPoint2.hurtBox.healthComponent.gameObject);
			}
		}

		// Token: 0x04000D46 RID: 3398
		public GameObject attacker;

		// Token: 0x04000D47 RID: 3399
		public GameObject inflictor;

		// Token: 0x04000D48 RID: 3400
		public TeamIndex teamIndex;

		// Token: 0x04000D49 RID: 3401
		public bool canHurtAttacker;

		// Token: 0x04000D4A RID: 3402
		public Vector3 position;

		// Token: 0x04000D4B RID: 3403
		public float radius;

		// Token: 0x04000D4C RID: 3404
		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

		// Token: 0x04000D4D RID: 3405
		public float baseDamage;

		// Token: 0x04000D4E RID: 3406
		public float baseForce;

		// Token: 0x04000D4F RID: 3407
		public Vector3 bonusForce;

		// Token: 0x04000D50 RID: 3408
		public bool crit;

		// Token: 0x04000D51 RID: 3409
		public DamageType damageType;

		// Token: 0x04000D52 RID: 3410
		public DamageColorIndex damageColorIndex;

		// Token: 0x04000D53 RID: 3411
		public ProcChainMask procChainMask;

		// Token: 0x04000D54 RID: 3412
		public float procCoefficient = 1f;

		// Token: 0x04000D55 RID: 3413
		private static readonly Dictionary<HealthComponent, BlastAttack.HitPoint> bestHitPoints = new Dictionary<HealthComponent, BlastAttack.HitPoint>();

		// Token: 0x02000202 RID: 514
		public enum FalloffModel
		{
			// Token: 0x04000D57 RID: 3415
			None,
			// Token: 0x04000D58 RID: 3416
			Linear,
			// Token: 0x04000D59 RID: 3417
			SweetSpot
		}

		// Token: 0x02000203 RID: 515
		private struct HitPoint
		{
			// Token: 0x06000A08 RID: 2568 RVA: 0x00032152 File Offset: 0x00030352
			public static int DistanceSort(BlastAttack.HitPoint a, BlastAttack.HitPoint b)
			{
				return a.distanceSqr.CompareTo(b.distanceSqr);
			}

			// Token: 0x04000D5A RID: 3418
			public HurtBox hurtBox;

			// Token: 0x04000D5B RID: 3419
			public Vector3 hitPosition;

			// Token: 0x04000D5C RID: 3420
			public Vector3 hitNormal;

			// Token: 0x04000D5D RID: 3421
			public float distanceSqr;
		}
	}
}
