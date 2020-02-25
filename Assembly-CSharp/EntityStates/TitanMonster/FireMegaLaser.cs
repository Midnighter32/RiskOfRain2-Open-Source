using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000859 RID: 2137
	public class FireMegaLaser : BaseState
	{
		// Token: 0x0600303D RID: 12349 RVA: 0x000CF550 File Offset: 0x000CD750
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

		// Token: 0x0600303E RID: 12350 RVA: 0x000CF728 File Offset: 0x000CD928
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

		// Token: 0x0600303F RID: 12351 RVA: 0x000CF78C File Offset: 0x000CD98C
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

		// Token: 0x06003040 RID: 12352 RVA: 0x000CF7FC File Offset: 0x000CD9FC
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

		// Token: 0x06003041 RID: 12353 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x000CFB10 File Offset: 0x000CDD10
		private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle, float maxDistance)
		{
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, targetMuzzle, false);
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
					bulletCount = 1U,
					damage = FireMegaLaser.damageCoefficient * this.damageStat / FireMegaLaser.fireFrequency,
					force = FireMegaLaser.force,
					muzzleName = targetMuzzle,
					hitEffectPrefab = this.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					procCoefficient = FireMegaLaser.procCoefficientPerTick,
					HitEffectNormal = false,
					radius = 0f,
					maxDistance = maxDistance
				}.Fire();
			}
		}

		// Token: 0x06003043 RID: 12355 RVA: 0x000CFC18 File Offset: 0x000CDE18
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(HurtBoxReference.FromHurtBox(this.lockedOnHurtBox));
			writer.Write(this.stopwatch);
		}

		// Token: 0x06003044 RID: 12356 RVA: 0x000CFC40 File Offset: 0x000CDE40
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = reader.ReadHurtBoxReference();
			this.stopwatch = reader.ReadSingle();
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.lockedOnHurtBox = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x04002E4D RID: 11853
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x04002E4E RID: 11854
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x04002E4F RID: 11855
		[SerializeField]
		public GameObject laserPrefab;

		// Token: 0x04002E50 RID: 11856
		public static string playAttackSoundString;

		// Token: 0x04002E51 RID: 11857
		public static string playLoopSoundString;

		// Token: 0x04002E52 RID: 11858
		public static string stopLoopSoundString;

		// Token: 0x04002E53 RID: 11859
		public static float damageCoefficient;

		// Token: 0x04002E54 RID: 11860
		public static float force;

		// Token: 0x04002E55 RID: 11861
		public static float minSpread;

		// Token: 0x04002E56 RID: 11862
		public static float maxSpread;

		// Token: 0x04002E57 RID: 11863
		public static int bulletCount;

		// Token: 0x04002E58 RID: 11864
		public static float fireFrequency;

		// Token: 0x04002E59 RID: 11865
		public static float maxDistance;

		// Token: 0x04002E5A RID: 11866
		public static float minimumDuration;

		// Token: 0x04002E5B RID: 11867
		public static float maximumDuration;

		// Token: 0x04002E5C RID: 11868
		public static float lockOnAngle;

		// Token: 0x04002E5D RID: 11869
		public static float procCoefficientPerTick;

		// Token: 0x04002E5E RID: 11870
		private HurtBox lockedOnHurtBox;

		// Token: 0x04002E5F RID: 11871
		private float fireStopwatch;

		// Token: 0x04002E60 RID: 11872
		private float stopwatch;

		// Token: 0x04002E61 RID: 11873
		private Ray aimRay;

		// Token: 0x04002E62 RID: 11874
		private Transform modelTransform;

		// Token: 0x04002E63 RID: 11875
		private GameObject laserEffect;

		// Token: 0x04002E64 RID: 11876
		private ChildLocator laserChildLocator;

		// Token: 0x04002E65 RID: 11877
		private Transform laserEffectEnd;

		// Token: 0x04002E66 RID: 11878
		public int bulletCountCurrent = 1;

		// Token: 0x04002E67 RID: 11879
		protected Transform muzzleTransform;

		// Token: 0x04002E68 RID: 11880
		private float lockOnTestFrequency = 4f;

		// Token: 0x04002E69 RID: 11881
		private float lockOnStopwatch;

		// Token: 0x04002E6A RID: 11882
		private BullseyeSearch enemyFinder;

		// Token: 0x04002E6B RID: 11883
		private bool foundAnyTarget;
	}
}
