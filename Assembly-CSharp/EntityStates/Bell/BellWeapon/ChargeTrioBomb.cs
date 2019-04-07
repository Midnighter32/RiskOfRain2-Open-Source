using System;
using System.Collections.Generic;
using System.Globalization;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Bell.BellWeapon
{
	// Token: 0x020001C8 RID: 456
	internal class ChargeTrioBomb : BaseState
	{
		// Token: 0x060008EB RID: 2283 RVA: 0x0002CED0 File Offset: 0x0002B0D0
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

		// Token: 0x060008EC RID: 2284 RVA: 0x0002CF46 File Offset: 0x0002B146
		private string FindTargetChildStringFromBombIndex()
		{
			return string.Format(CultureInfo.InvariantCulture, "ProjectilePosition{0}", this.currentBombIndex);
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x0002CF64 File Offset: 0x0002B164
		private Transform FindTargetChildTransformFromBombIndex()
		{
			string childName = this.FindTargetChildStringFromBombIndex();
			return this.childLocator.FindChild(childName);
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0002CF84 File Offset: 0x0002B184
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
						EffectManager.instance.SimpleMuzzleFlash(ChargeTrioBomb.muzzleflashPrefab, base.gameObject, this.FindTargetChildStringFromBombIndex(), false);
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

		// Token: 0x060008EF RID: 2287 RVA: 0x0002D158 File Offset: 0x0002B358
		public override void OnExit()
		{
			base.OnExit();
			foreach (GameObject obj in this.preppedBombInstances)
			{
				EntityState.Destroy(obj);
			}
		}

		// Token: 0x04000C15 RID: 3093
		public static float basePrepDuration;

		// Token: 0x04000C16 RID: 3094
		public static float baseTimeBetweenPreps;

		// Token: 0x04000C17 RID: 3095
		public static GameObject preppedBombPrefab;

		// Token: 0x04000C18 RID: 3096
		public static float baseBarrageDuration;

		// Token: 0x04000C19 RID: 3097
		public static float baseTimeBetweenBarrages;

		// Token: 0x04000C1A RID: 3098
		public static GameObject bombProjectilePrefab;

		// Token: 0x04000C1B RID: 3099
		public static GameObject muzzleflashPrefab;

		// Token: 0x04000C1C RID: 3100
		public static float damageCoefficient;

		// Token: 0x04000C1D RID: 3101
		public static float force;

		// Token: 0x04000C1E RID: 3102
		public static float selfForce;

		// Token: 0x04000C1F RID: 3103
		private float prepDuration;

		// Token: 0x04000C20 RID: 3104
		private float timeBetweenPreps;

		// Token: 0x04000C21 RID: 3105
		private float barrageDuration;

		// Token: 0x04000C22 RID: 3106
		private float timeBetweenBarrages;

		// Token: 0x04000C23 RID: 3107
		private ChildLocator childLocator;

		// Token: 0x04000C24 RID: 3108
		private List<GameObject> preppedBombInstances = new List<GameObject>();

		// Token: 0x04000C25 RID: 3109
		private int currentBombIndex;

		// Token: 0x04000C26 RID: 3110
		private float perProjectileStopwatch;
	}
}
