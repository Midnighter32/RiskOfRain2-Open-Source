using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleGuardMonster
{
	// Token: 0x020001D9 RID: 473
	public class GroundSlam : BaseState
	{
		// Token: 0x0600093C RID: 2364 RVA: 0x0002E804 File Offset: 0x0002CA04
		private void EnableIndicator(string childLocatorName, ChildLocator childLocator = null)
		{
			if (!childLocator)
			{
				childLocator = base.GetModelTransform().GetComponent<ChildLocator>();
			}
			Transform transform = childLocator.FindChild(childLocatorName);
			if (transform)
			{
				transform.gameObject.SetActive(true);
				ObjectScaleCurve component = transform.gameObject.GetComponent<ObjectScaleCurve>();
				if (component)
				{
					component.time = 0f;
				}
			}
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0002E864 File Offset: 0x0002CA64
		private void DisableIndicator(string childLocatorName, ChildLocator childLocator = null)
		{
			if (!childLocator)
			{
				childLocator = base.GetModelTransform().GetComponent<ChildLocator>();
			}
			Transform transform = childLocator.FindChild(childLocatorName);
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0002E8A4 File Offset: 0x0002CAA4
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
					this.EnableIndicator("GroundSlamIndicator", this.modelChildLocator);
				}
			}
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0002EA7F File Offset: 0x0002CC7F
		public override void OnExit()
		{
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
			base.characterDirection;
			base.OnExit();
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0002EABC File Offset: 0x0002CCBC
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
					this.DisableIndicator("GroundSlamIndicator", this.modelChildLocator);
					EffectManager.instance.SimpleMuzzleFlash(GroundSlam.slamEffectPrefab, base.gameObject, "SlamZone", true);
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

		// Token: 0x06000941 RID: 2369 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C91 RID: 3217
		public static float baseDuration = 3.5f;

		// Token: 0x04000C92 RID: 3218
		public static float damageCoefficient = 4f;

		// Token: 0x04000C93 RID: 3219
		public static float forceMagnitude = 16f;

		// Token: 0x04000C94 RID: 3220
		private OverlapAttack attack;

		// Token: 0x04000C95 RID: 3221
		public static string initialAttackSoundString;

		// Token: 0x04000C96 RID: 3222
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000C97 RID: 3223
		public static GameObject slamEffectPrefab;

		// Token: 0x04000C98 RID: 3224
		public static GameObject hitEffectPrefab;

		// Token: 0x04000C99 RID: 3225
		private Animator modelAnimator;

		// Token: 0x04000C9A RID: 3226
		private Transform modelTransform;

		// Token: 0x04000C9B RID: 3227
		private bool hasAttacked;

		// Token: 0x04000C9C RID: 3228
		private float duration;

		// Token: 0x04000C9D RID: 3229
		private GameObject leftHandChargeEffect;

		// Token: 0x04000C9E RID: 3230
		private GameObject rightHandChargeEffect;

		// Token: 0x04000C9F RID: 3231
		private ChildLocator modelChildLocator;
	}
}
