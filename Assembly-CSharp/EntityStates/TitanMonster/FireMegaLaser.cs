using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000173 RID: 371
	internal class FireMegaLaser : BaseState
	{
		// Token: 0x06000722 RID: 1826 RVA: 0x00022754 File Offset: 0x00020954
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterBody.SetAimTimer(FireMegaLaser.maximumDuration);
			Util.PlaySound(FireMegaLaser.playAttackSoundString, base.gameObject);
			Util.PlaySound(FireMegaLaser.playLoopSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "FireLaserLoop", 0.25f);
			this.enemyFinder = new BullseyeSearch();
			this.enemyFinder.maxDistanceFilter = FireMegaLaser.maxDistance;
			this.enemyFinder.maxAngleFilter = FireMegaLaser.lockOnAngle;
			this.enemyFinder.searchOrigin = this.aimRay.origin;
			this.enemyFinder.searchDirection = this.aimRay.direction;
			this.enemyFinder.filterByLoS = false;
			this.enemyFinder.sortMode = BullseyeSearch.SortMode.Angle;
			this.enemyFinder.teamMaskFilter = TeamMask.allButNeutral;
			if (base.teamComponent)
			{
				this.enemyFinder.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
			}
			this.aimRay = base.GetAimRay();
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild("MuzzleLaser");
					if (this.muzzleTransform && this.laserPrefab)
					{
						this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(this.laserPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
						this.laserEffect.transform.parent = this.muzzleTransform;
						this.laserChildLocator = this.laserEffect.GetComponent<ChildLocator>();
						this.laserEffectEnd = this.laserChildLocator.FindChild("LaserEnd");
					}
				}
			}
			this.UpdateLockOn();
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x0002292C File Offset: 0x00020B2C
		public override void OnExit()
		{
			if (this.laserEffect)
			{
				EntityState.Destroy(this.laserEffect);
			}
			base.characterBody.SetAimTimer(2f);
			Util.PlaySound(FireMegaLaser.stopLoopSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "FireLaserEnd", 0.25f);
			base.OnExit();
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00022990 File Offset: 0x00020B90
		private void UpdateLockOn()
		{
			if (base.isAuthority)
			{
				this.enemyFinder.searchOrigin = this.aimRay.origin;
				this.enemyFinder.searchDirection = this.aimRay.direction;
				this.enemyFinder.RefreshCandidates();
				HurtBox exists = this.enemyFinder.GetResults().FirstOrDefault<HurtBox>();
				this.lockedOnHurtBox = exists;
				this.foundAnyTarget = exists;
			}
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00022A00 File Offset: 0x00020C00
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireStopwatch += Time.fixedDeltaTime;
			this.stopwatch += Time.fixedDeltaTime;
			this.lockOnStopwatch += Time.fixedDeltaTime;
			this.aimRay = base.GetAimRay();
			if (base.isAuthority && !this.lockedOnHurtBox && this.foundAnyTarget)
			{
				this.outer.SetNextState(new FireMegaLaser
				{
					stopwatch = this.stopwatch
				});
				return;
			}
			Vector3 vector = this.aimRay.origin;
			if (this.muzzleTransform)
			{
				vector = this.muzzleTransform.position;
			}
			Vector3 vector2;
			RaycastHit raycastHit;
			if (this.lockedOnHurtBox)
			{
				vector2 = this.lockedOnHurtBox.transform.position;
			}
			else if (Util.CharacterRaycast(base.gameObject, this.aimRay, out raycastHit, FireMegaLaser.maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
			{
				vector2 = raycastHit.point;
			}
			else
			{
				vector2 = this.aimRay.GetPoint(FireMegaLaser.maxDistance);
			}
			Ray ray = new Ray(vector, vector2 - vector);
			bool flag = false;
			if (this.laserEffect && this.laserChildLocator)
			{
				RaycastHit raycastHit2;
				if (Util.CharacterRaycast(base.gameObject, ray, out raycastHit2, (vector2 - vector).magnitude, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					vector2 = raycastHit2.point;
					RaycastHit raycastHit3;
					if (Util.CharacterRaycast(base.gameObject, new Ray(vector2 - ray.direction * 0.1f, -ray.direction), out raycastHit3, raycastHit2.distance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
					{
						vector2 = ray.GetPoint(0.1f);
						flag = true;
					}
				}
				this.laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(vector2 - vector);
				this.laserEffectEnd.transform.position = vector2;
			}
			if (this.fireStopwatch > 1f / FireMegaLaser.fireFrequency)
			{
				string targetMuzzle = "MuzzleLaser";
				if (!flag)
				{
					this.FireBullet(this.modelTransform, ray, targetMuzzle, (vector2 - ray.origin).magnitude + 0.1f);
				}
				this.fireStopwatch -= 1f / FireMegaLaser.fireFrequency;
			}
			if (base.isAuthority && (((!base.inputBank || !base.inputBank.skill4.down) && this.stopwatch > FireMegaLaser.minimumDuration) || this.stopwatch > FireMegaLaser.maximumDuration))
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00022D14 File Offset: 0x00020F14
		private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle, float maxDistance)
		{
			if (this.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireMegaLaser.minSpread,
					maxSpread = FireMegaLaser.maxSpread,
					bulletCount = 1u,
					damage = FireMegaLaser.damageCoefficient * this.damageStat / FireMegaLaser.fireFrequency,
					force = FireMegaLaser.force,
					muzzleName = targetMuzzle,
					hitEffectPrefab = this.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					HitEffectNormal = false,
					radius = 0f,
					maxDistance = maxDistance
				}.Fire();
			}
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00022E16 File Offset: 0x00021016
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(HurtBoxReference.FromHurtBox(this.lockedOnHurtBox));
			writer.Write(this.stopwatch);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00022E40 File Offset: 0x00021040
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = reader.ReadHurtBoxReference();
			this.stopwatch = reader.ReadSingle();
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.lockedOnHurtBox = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x040008E1 RID: 2273
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x040008E2 RID: 2274
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x040008E3 RID: 2275
		[SerializeField]
		public GameObject laserPrefab;

		// Token: 0x040008E4 RID: 2276
		public static string playAttackSoundString;

		// Token: 0x040008E5 RID: 2277
		public static string playLoopSoundString;

		// Token: 0x040008E6 RID: 2278
		public static string stopLoopSoundString;

		// Token: 0x040008E7 RID: 2279
		public static float damageCoefficient;

		// Token: 0x040008E8 RID: 2280
		public static float force;

		// Token: 0x040008E9 RID: 2281
		public static float minSpread;

		// Token: 0x040008EA RID: 2282
		public static float maxSpread;

		// Token: 0x040008EB RID: 2283
		public static int bulletCount;

		// Token: 0x040008EC RID: 2284
		public static float fireFrequency;

		// Token: 0x040008ED RID: 2285
		public static float maxDistance;

		// Token: 0x040008EE RID: 2286
		public static float minimumDuration;

		// Token: 0x040008EF RID: 2287
		public static float maximumDuration;

		// Token: 0x040008F0 RID: 2288
		public static float lockOnAngle;

		// Token: 0x040008F1 RID: 2289
		private HurtBox lockedOnHurtBox;

		// Token: 0x040008F2 RID: 2290
		private float fireStopwatch;

		// Token: 0x040008F3 RID: 2291
		private float stopwatch;

		// Token: 0x040008F4 RID: 2292
		private Ray aimRay;

		// Token: 0x040008F5 RID: 2293
		private Transform modelTransform;

		// Token: 0x040008F6 RID: 2294
		private GameObject laserEffect;

		// Token: 0x040008F7 RID: 2295
		private ChildLocator laserChildLocator;

		// Token: 0x040008F8 RID: 2296
		private Transform laserEffectEnd;

		// Token: 0x040008F9 RID: 2297
		public int bulletCountCurrent = 1;

		// Token: 0x040008FA RID: 2298
		protected Transform muzzleTransform;

		// Token: 0x040008FB RID: 2299
		private float lockOnTestFrequency = 4f;

		// Token: 0x040008FC RID: 2300
		private float lockOnStopwatch;

		// Token: 0x040008FD RID: 2301
		private BullseyeSearch enemyFinder;

		// Token: 0x040008FE RID: 2302
		private bool foundAnyTarget;
	}
}
