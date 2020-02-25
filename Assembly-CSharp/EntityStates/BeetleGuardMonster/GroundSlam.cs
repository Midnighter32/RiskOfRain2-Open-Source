using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020008F4 RID: 2292
	public class GroundSlam : BaseState
	{
		// Token: 0x0600333C RID: 13116 RVA: 0x000DE470 File Offset: 0x000DC670
		private void EnableIndicator(Transform indicator)
		{
			if (indicator)
			{
				indicator.gameObject.SetActive(true);
				ObjectScaleCurve component = indicator.gameObject.GetComponent<ObjectScaleCurve>();
				if (component)
				{
					component.time = 0f;
				}
			}
		}

		// Token: 0x0600333D RID: 13117 RVA: 0x000DE4B0 File Offset: 0x000DC6B0
		private void DisableIndicator(Transform indicator)
		{
			if (indicator)
			{
				indicator.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600333E RID: 13118 RVA: 0x000DE4C8 File Offset: 0x000DC6C8
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			Util.PlaySound(GroundSlam.initialAttackSoundString, base.gameObject);
			base.characterDirection;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = GroundSlam.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = GroundSlam.hitEffectPrefab;
			this.attack.forceVector = Vector3.up * GroundSlam.forceMagnitude;
			if (this.modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "GroundSlam");
			}
			this.duration = GroundSlam.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "GroundSlam", "GroundSlam.playbackRate", this.duration, 0.2f);
			if (this.modelTransform)
			{
				this.modelChildLocator = this.modelTransform.GetComponent<ChildLocator>();
				if (this.modelChildLocator)
				{
					GameObject original = GroundSlam.chargeEffectPrefab;
					Transform transform = this.modelChildLocator.FindChild("HandL");
					Transform transform2 = this.modelChildLocator.FindChild("HandR");
					if (transform)
					{
						this.leftHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform);
					}
					if (transform2)
					{
						this.rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform2);
					}
					this.groundSlamIndicatorInstance = this.modelChildLocator.FindChild("GroundSlamIndicator");
					this.EnableIndicator(this.groundSlamIndicatorInstance);
				}
			}
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x000DE6B4 File Offset: 0x000DC8B4
		public override void OnExit()
		{
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator(this.groundSlamIndicatorInstance);
			base.characterDirection;
			base.OnExit();
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x000DE6EC File Offset: 0x000DC8EC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("GroundSlam.hitBoxActive") > 0.5f && !this.hasAttacked)
			{
				if (NetworkServer.active)
				{
					this.attack.Fire(null);
				}
				if (base.isAuthority && this.modelTransform)
				{
					this.DisableIndicator(this.groundSlamIndicatorInstance);
					EffectManager.SimpleMuzzleFlash(GroundSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
				}
				this.hasAttacked = true;
				EntityState.Destroy(this.leftHandChargeEffect);
				EntityState.Destroy(this.rightHandChargeEffect);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003341 RID: 13121 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040032B9 RID: 12985
		public static float baseDuration = 3.5f;

		// Token: 0x040032BA RID: 12986
		public static float damageCoefficient = 4f;

		// Token: 0x040032BB RID: 12987
		public static float forceMagnitude = 16f;

		// Token: 0x040032BC RID: 12988
		private OverlapAttack attack;

		// Token: 0x040032BD RID: 12989
		public static string initialAttackSoundString;

		// Token: 0x040032BE RID: 12990
		public static GameObject chargeEffectPrefab;

		// Token: 0x040032BF RID: 12991
		public static GameObject slamEffectPrefab;

		// Token: 0x040032C0 RID: 12992
		public static GameObject hitEffectPrefab;

		// Token: 0x040032C1 RID: 12993
		private Animator modelAnimator;

		// Token: 0x040032C2 RID: 12994
		private Transform modelTransform;

		// Token: 0x040032C3 RID: 12995
		private bool hasAttacked;

		// Token: 0x040032C4 RID: 12996
		private float duration;

		// Token: 0x040032C5 RID: 12997
		private GameObject leftHandChargeEffect;

		// Token: 0x040032C6 RID: 12998
		private GameObject rightHandChargeEffect;

		// Token: 0x040032C7 RID: 12999
		private ChildLocator modelChildLocator;

		// Token: 0x040032C8 RID: 13000
		private Transform groundSlamIndicatorInstance;
	}
}
