using System;
using System.Collections.Generic;
using System.Globalization;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Bell.BellWeapon
{
	// Token: 0x020008E3 RID: 2275
	public class ChargeTrioBomb : BaseState
	{
		// Token: 0x060032EB RID: 13035 RVA: 0x000DCB48 File Offset: 0x000DAD48
		public override void OnEnter()
		{
			base.OnEnter();
			this.prepDuration = ChargeTrioBomb.basePrepDuration / this.attackSpeedStat;
			this.timeBetweenPreps = ChargeTrioBomb.baseTimeBetweenPreps / this.attackSpeedStat;
			this.barrageDuration = ChargeTrioBomb.baseBarrageDuration / this.attackSpeedStat;
			this.timeBetweenBarrages = ChargeTrioBomb.baseTimeBetweenBarrages / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
			}
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x000DCBBE File Offset: 0x000DADBE
		private string FindTargetChildStringFromBombIndex()
		{
			return string.Format(CultureInfo.InvariantCulture, "ProjectilePosition{0}", this.currentBombIndex);
		}

		// Token: 0x060032ED RID: 13037 RVA: 0x000DCBDC File Offset: 0x000DADDC
		private Transform FindTargetChildTransformFromBombIndex()
		{
			string childName = this.FindTargetChildStringFromBombIndex();
			return this.childLocator.FindChild(childName);
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x000DCBFC File Offset: 0x000DADFC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.perProjectileStopwatch += Time.fixedDeltaTime;
			if (base.fixedAge < this.prepDuration)
			{
				if (this.perProjectileStopwatch > this.timeBetweenPreps && this.currentBombIndex < 3)
				{
					this.currentBombIndex++;
					this.perProjectileStopwatch = 0f;
					Transform transform = this.FindTargetChildTransformFromBombIndex();
					if (transform)
					{
						GameObject item = UnityEngine.Object.Instantiate<GameObject>(ChargeTrioBomb.preppedBombPrefab, transform);
						this.preppedBombInstances.Add(item);
						return;
					}
				}
			}
			else if (base.fixedAge < this.prepDuration + this.barrageDuration)
			{
				if (this.perProjectileStopwatch > this.timeBetweenBarrages && this.currentBombIndex > 0)
				{
					this.perProjectileStopwatch = 0f;
					Ray aimRay = base.GetAimRay();
					Transform transform2 = this.FindTargetChildTransformFromBombIndex();
					if (transform2)
					{
						if (base.isAuthority)
						{
							ProjectileManager.instance.FireProjectile(ChargeTrioBomb.bombProjectilePrefab, transform2.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * ChargeTrioBomb.damageCoefficient, ChargeTrioBomb.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
							Rigidbody component = base.GetComponent<Rigidbody>();
							if (component)
							{
								component.AddForceAtPosition(-ChargeTrioBomb.selfForce * transform2.forward, transform2.position);
							}
						}
						EffectManager.SimpleMuzzleFlash(ChargeTrioBomb.muzzleflashPrefab, base.gameObject, this.FindTargetChildStringFromBombIndex(), false);
					}
					this.currentBombIndex--;
					EntityState.Destroy(this.preppedBombInstances[this.currentBombIndex]);
					return;
				}
			}
			else if (base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060032EF RID: 13039 RVA: 0x000DCDC8 File Offset: 0x000DAFC8
		public override void OnExit()
		{
			base.OnExit();
			foreach (GameObject obj in this.preppedBombInstances)
			{
				EntityState.Destroy(obj);
			}
		}

		// Token: 0x0400323D RID: 12861
		public static float basePrepDuration;

		// Token: 0x0400323E RID: 12862
		public static float baseTimeBetweenPreps;

		// Token: 0x0400323F RID: 12863
		public static GameObject preppedBombPrefab;

		// Token: 0x04003240 RID: 12864
		public static float baseBarrageDuration;

		// Token: 0x04003241 RID: 12865
		public static float baseTimeBetweenBarrages;

		// Token: 0x04003242 RID: 12866
		public static GameObject bombProjectilePrefab;

		// Token: 0x04003243 RID: 12867
		public static GameObject muzzleflashPrefab;

		// Token: 0x04003244 RID: 12868
		public static float damageCoefficient;

		// Token: 0x04003245 RID: 12869
		public static float force;

		// Token: 0x04003246 RID: 12870
		public static float selfForce;

		// Token: 0x04003247 RID: 12871
		private float prepDuration;

		// Token: 0x04003248 RID: 12872
		private float timeBetweenPreps;

		// Token: 0x04003249 RID: 12873
		private float barrageDuration;

		// Token: 0x0400324A RID: 12874
		private float timeBetweenBarrages;

		// Token: 0x0400324B RID: 12875
		private ChildLocator childLocator;

		// Token: 0x0400324C RID: 12876
		private List<GameObject> preppedBombInstances = new List<GameObject>();

		// Token: 0x0400324D RID: 12877
		private int currentBombIndex;

		// Token: 0x0400324E RID: 12878
		private float perProjectileStopwatch;
	}
}
