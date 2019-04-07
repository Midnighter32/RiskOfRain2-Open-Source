using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000119 RID: 281
	internal class FireRoller : BaseState
	{
		// Token: 0x0600055D RID: 1373 RVA: 0x000180D4 File Offset: 0x000162D4
		public override void OnEnter()
		{
			base.OnEnter();
			this.InitElement(MageElement.Ice);
			this.stopwatch = 0f;
			this.entryDuration = FireRoller.baseEntryDuration / this.attackSpeedStat;
			this.fireDuration = FireRoller.baseDuration / this.attackSpeedStat;
			this.exitDuration = FireRoller.baseExitDuration / this.attackSpeedStat;
			Util.PlaySound(this.attackString, base.gameObject);
			base.characterBody.SetAimTimer(this.fireDuration + this.entryDuration + this.exitDuration + 2f);
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				this.childLocator = this.animator.GetComponent<ChildLocator>();
			}
			this.muzzleString = "MuzzleRight";
			if (this.childLocator)
			{
				this.muzzleTransform = this.childLocator.FindChild(this.muzzleString);
			}
			base.PlayAnimation("Gesture Left, Additive", "Empty");
			base.PlayAnimation("Gesture Right, Additive", "Empty");
			base.PlayAnimation("Gesture, Additive", "EnterRoller", "EnterRoller.playbackRate", this.entryDuration);
			if (this.areaIndicatorPrefab)
			{
				this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(this.areaIndicatorPrefab);
			}
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00018218 File Offset: 0x00016418
		private void UpdateAreaIndicator()
		{
			if (this.areaIndicatorInstance)
			{
				float maxDistance = 1000f;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.rotation = Util.QuaternionSafeLookRotation(base.transform.position - this.areaIndicatorInstance.transform.position, raycastHit.normal);
				}
			}
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x000182AD File Offset: 0x000164AD
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x000182BB File Offset: 0x000164BB
		public override void OnExit()
		{
			base.OnExit();
			if (this.areaIndicatorInstance)
			{
				EntityState.Destroy(this.areaIndicatorInstance);
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x000182DC File Offset: 0x000164DC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.entryDuration && !this.hasFiredRoller)
			{
				base.PlayAnimation("Gesture, Additive", "FireRoller", "FireRoller.playbackRate", this.fireDuration);
				this.FireRollerProjectile();
				EntityState.Destroy(this.areaIndicatorInstance);
			}
			if (this.stopwatch >= this.entryDuration + this.fireDuration && !this.hasBegunExit)
			{
				this.hasBegunExit = true;
				base.PlayAnimation("Gesture, Additive", "ExitRoller", "ExitRoller.playbackRate", this.exitDuration);
			}
			if (this.stopwatch >= this.entryDuration + this.fireDuration + this.exitDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x000183B4 File Offset: 0x000165B4
		private void FireRollerProjectile()
		{
			this.hasFiredRoller = true;
			if (this.muzzleflashEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority && this.projectilePrefab != null)
			{
				float maxDistance = 1000f;
				Ray aimRay = base.GetAimRay();
				Vector3 forward = aimRay.direction;
				Vector3 vector = aimRay.origin;
				float magnitude = FireRoller.targetProjectileSpeed;
				if (this.muzzleTransform)
				{
					vector = this.muzzleTransform.position;
					RaycastHit raycastHit;
					if (Physics.Raycast(aimRay, out raycastHit, maxDistance, LayerIndex.world.mask))
					{
						float num = magnitude;
						Vector3 vector2 = raycastHit.point - vector;
						Vector2 vector3 = new Vector2(vector2.x, vector2.z);
						float magnitude2 = vector3.magnitude;
						float y = Trajectory.CalculateInitialYSpeed(magnitude2 / num, vector2.y);
						Vector3 a = new Vector3(vector3.x / magnitude2 * num, y, vector3.y / magnitude2 * num);
						magnitude = a.magnitude;
						forward = a / magnitude;
					}
				}
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, vector, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * this.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, magnitude);
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00018530 File Offset: 0x00016730
		private void InitElement(MageElement defaultElement)
		{
			MageCalibrationController component = base.GetComponent<MageCalibrationController>();
			if (component)
			{
				MageElement activeCalibrationElement = component.GetActiveCalibrationElement();
				if (activeCalibrationElement != MageElement.None)
				{
					defaultElement = activeCalibrationElement;
				}
			}
			switch (defaultElement)
			{
			case MageElement.Fire:
				this.damageCoefficient = FireRoller.fireDamageCoefficient;
				this.attackString = FireRoller.fireAttackSoundString;
				this.projectilePrefab = FireRoller.fireProjectilePrefab;
				this.muzzleflashEffectPrefab = FireRoller.fireMuzzleflashEffectPrefab;
				this.areaIndicatorPrefab = FireRoller.fireAreaIndicatorPrefab;
				return;
			case MageElement.Ice:
				this.damageCoefficient = FireRoller.iceDamageCoefficient;
				this.attackString = FireRoller.iceAttackSoundString;
				this.projectilePrefab = FireRoller.iceProjectilePrefab;
				this.muzzleflashEffectPrefab = FireRoller.iceMuzzleflashEffectPrefab;
				this.areaIndicatorPrefab = FireRoller.iceAreaIndicatorPrefab;
				return;
			case MageElement.Lightning:
				this.damageCoefficient = FireRoller.lightningDamageCoefficient;
				this.attackString = FireRoller.lightningAttackSoundString;
				this.projectilePrefab = FireRoller.lightningProjectilePrefab;
				this.muzzleflashEffectPrefab = FireRoller.lightningMuzzleflashEffectPrefab;
				this.areaIndicatorPrefab = FireRoller.lightningAreaIndicatorPrefab;
				return;
			default:
				return;
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040005CC RID: 1484
		public static GameObject fireProjectilePrefab;

		// Token: 0x040005CD RID: 1485
		public static GameObject iceProjectilePrefab;

		// Token: 0x040005CE RID: 1486
		public static GameObject lightningProjectilePrefab;

		// Token: 0x040005CF RID: 1487
		public static GameObject fireMuzzleflashEffectPrefab;

		// Token: 0x040005D0 RID: 1488
		public static GameObject iceMuzzleflashEffectPrefab;

		// Token: 0x040005D1 RID: 1489
		public static GameObject lightningMuzzleflashEffectPrefab;

		// Token: 0x040005D2 RID: 1490
		public static GameObject fireAreaIndicatorPrefab;

		// Token: 0x040005D3 RID: 1491
		public static GameObject iceAreaIndicatorPrefab;

		// Token: 0x040005D4 RID: 1492
		public static GameObject lightningAreaIndicatorPrefab;

		// Token: 0x040005D5 RID: 1493
		public static string fireAttackSoundString;

		// Token: 0x040005D6 RID: 1494
		public static string iceAttackSoundString;

		// Token: 0x040005D7 RID: 1495
		public static string lightningAttackSoundString;

		// Token: 0x040005D8 RID: 1496
		public static float targetProjectileSpeed;

		// Token: 0x040005D9 RID: 1497
		public static float baseEntryDuration = 2f;

		// Token: 0x040005DA RID: 1498
		public static float baseDuration = 2f;

		// Token: 0x040005DB RID: 1499
		public static float baseExitDuration = 2f;

		// Token: 0x040005DC RID: 1500
		public static float fireDamageCoefficient;

		// Token: 0x040005DD RID: 1501
		public static float iceDamageCoefficient;

		// Token: 0x040005DE RID: 1502
		public static float lightningDamageCoefficient;

		// Token: 0x040005DF RID: 1503
		private float stopwatch;

		// Token: 0x040005E0 RID: 1504
		private float fireDuration;

		// Token: 0x040005E1 RID: 1505
		private float entryDuration;

		// Token: 0x040005E2 RID: 1506
		private float exitDuration;

		// Token: 0x040005E3 RID: 1507
		private bool hasFiredRoller;

		// Token: 0x040005E4 RID: 1508
		private bool hasBegunExit;

		// Token: 0x040005E5 RID: 1509
		private GameObject areaIndicatorInstance;

		// Token: 0x040005E6 RID: 1510
		private string muzzleString;

		// Token: 0x040005E7 RID: 1511
		private Transform muzzleTransform;

		// Token: 0x040005E8 RID: 1512
		private Animator animator;

		// Token: 0x040005E9 RID: 1513
		private ChildLocator childLocator;

		// Token: 0x040005EA RID: 1514
		private GameObject areaIndicatorPrefab;

		// Token: 0x040005EB RID: 1515
		private float damageCoefficient = 1.2f;

		// Token: 0x040005EC RID: 1516
		private string attackString;

		// Token: 0x040005ED RID: 1517
		private GameObject projectilePrefab;

		// Token: 0x040005EE RID: 1518
		private GameObject muzzleflashEffectPrefab;
	}
}
