using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007DA RID: 2010
	public class FireRoller : BaseState
	{
		// Token: 0x06002DBE RID: 11710 RVA: 0x000C2408 File Offset: 0x000C0608
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

		// Token: 0x06002DBF RID: 11711 RVA: 0x000C254C File Offset: 0x000C074C
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

		// Token: 0x06002DC0 RID: 11712 RVA: 0x000C25E1 File Offset: 0x000C07E1
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002DC1 RID: 11713 RVA: 0x000C25EF File Offset: 0x000C07EF
		public override void OnExit()
		{
			base.OnExit();
			if (this.areaIndicatorInstance)
			{
				EntityState.Destroy(this.areaIndicatorInstance);
			}
		}

		// Token: 0x06002DC2 RID: 11714 RVA: 0x000C2610 File Offset: 0x000C0810
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

		// Token: 0x06002DC3 RID: 11715 RVA: 0x000C26E8 File Offset: 0x000C08E8
		private void FireRollerProjectile()
		{
			this.hasFiredRoller = true;
			if (this.muzzleflashEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
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

		// Token: 0x06002DC4 RID: 11716 RVA: 0x000C2860 File Offset: 0x000C0A60
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

		// Token: 0x06002DC5 RID: 11717 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002A89 RID: 10889
		public static GameObject fireProjectilePrefab;

		// Token: 0x04002A8A RID: 10890
		public static GameObject iceProjectilePrefab;

		// Token: 0x04002A8B RID: 10891
		public static GameObject lightningProjectilePrefab;

		// Token: 0x04002A8C RID: 10892
		public static GameObject fireMuzzleflashEffectPrefab;

		// Token: 0x04002A8D RID: 10893
		public static GameObject iceMuzzleflashEffectPrefab;

		// Token: 0x04002A8E RID: 10894
		public static GameObject lightningMuzzleflashEffectPrefab;

		// Token: 0x04002A8F RID: 10895
		public static GameObject fireAreaIndicatorPrefab;

		// Token: 0x04002A90 RID: 10896
		public static GameObject iceAreaIndicatorPrefab;

		// Token: 0x04002A91 RID: 10897
		public static GameObject lightningAreaIndicatorPrefab;

		// Token: 0x04002A92 RID: 10898
		public static string fireAttackSoundString;

		// Token: 0x04002A93 RID: 10899
		public static string iceAttackSoundString;

		// Token: 0x04002A94 RID: 10900
		public static string lightningAttackSoundString;

		// Token: 0x04002A95 RID: 10901
		public static float targetProjectileSpeed;

		// Token: 0x04002A96 RID: 10902
		public static float baseEntryDuration = 2f;

		// Token: 0x04002A97 RID: 10903
		public static float baseDuration = 2f;

		// Token: 0x04002A98 RID: 10904
		public static float baseExitDuration = 2f;

		// Token: 0x04002A99 RID: 10905
		public static float fireDamageCoefficient;

		// Token: 0x04002A9A RID: 10906
		public static float iceDamageCoefficient;

		// Token: 0x04002A9B RID: 10907
		public static float lightningDamageCoefficient;

		// Token: 0x04002A9C RID: 10908
		private float stopwatch;

		// Token: 0x04002A9D RID: 10909
		private float fireDuration;

		// Token: 0x04002A9E RID: 10910
		private float entryDuration;

		// Token: 0x04002A9F RID: 10911
		private float exitDuration;

		// Token: 0x04002AA0 RID: 10912
		private bool hasFiredRoller;

		// Token: 0x04002AA1 RID: 10913
		private bool hasBegunExit;

		// Token: 0x04002AA2 RID: 10914
		private GameObject areaIndicatorInstance;

		// Token: 0x04002AA3 RID: 10915
		private string muzzleString;

		// Token: 0x04002AA4 RID: 10916
		private Transform muzzleTransform;

		// Token: 0x04002AA5 RID: 10917
		private Animator animator;

		// Token: 0x04002AA6 RID: 10918
		private ChildLocator childLocator;

		// Token: 0x04002AA7 RID: 10919
		private GameObject areaIndicatorPrefab;

		// Token: 0x04002AA8 RID: 10920
		private float damageCoefficient = 1.2f;

		// Token: 0x04002AA9 RID: 10921
		private string attackString;

		// Token: 0x04002AAA RID: 10922
		private GameObject projectilePrefab;

		// Token: 0x04002AAB RID: 10923
		private GameObject muzzleflashEffectPrefab;
	}
}
