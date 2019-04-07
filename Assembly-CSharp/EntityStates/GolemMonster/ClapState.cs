using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x02000177 RID: 375
	public class ClapState : BaseState
	{
		// Token: 0x0600073D RID: 1853 RVA: 0x0002342C File Offset: 0x0002162C
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(ClapState.attackSoundString, base.gameObject, this.attackSpeedStat);
			base.PlayCrossfade("Body", "Clap", "Clap.playbackRate", ClapState.duration, 0.1f);
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					GameObject original = Resources.Load<GameObject>("Prefabs/GolemClapCharge");
					Transform transform = component.FindChild("HandL");
					Transform transform2 = component.FindChild("HandR");
					if (transform)
					{
						this.leftHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform);
					}
					if (transform2)
					{
						this.rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform2);
					}
				}
			}
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x000234FB File Offset: 0x000216FB
		public override void OnExit()
		{
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			base.OnExit();
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0002351C File Offset: 0x0002171C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("Clap.hitBoxActive") > 0.5f && !this.hasAttacked)
			{
				if (base.isAuthority && this.modelTransform)
				{
					ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						Transform transform = component.FindChild("ClapZone");
						if (transform)
						{
							this.attack = new BlastAttack();
							this.attack.attacker = base.gameObject;
							this.attack.inflictor = base.gameObject;
							this.attack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
							this.attack.baseDamage = this.damageStat * ClapState.damageCoefficient;
							this.attack.baseForce = ClapState.forceMagnitude;
							this.attack.position = transform.position;
							this.attack.radius = ClapState.radius;
							this.attack.Fire();
						}
					}
				}
				this.hasAttacked = true;
				EntityState.Destroy(this.leftHandChargeEffect);
				EntityState.Destroy(this.rightHandChargeEffect);
			}
			if (base.fixedAge >= ClapState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400091A RID: 2330
		public static float duration = 3.5f;

		// Token: 0x0400091B RID: 2331
		public static float damageCoefficient = 4f;

		// Token: 0x0400091C RID: 2332
		public static float forceMagnitude = 16f;

		// Token: 0x0400091D RID: 2333
		public static float radius = 3f;

		// Token: 0x0400091E RID: 2334
		private BlastAttack attack;

		// Token: 0x0400091F RID: 2335
		public static string attackSoundString;

		// Token: 0x04000920 RID: 2336
		private Animator modelAnimator;

		// Token: 0x04000921 RID: 2337
		private Transform modelTransform;

		// Token: 0x04000922 RID: 2338
		private bool hasAttacked;

		// Token: 0x04000923 RID: 2339
		private GameObject leftHandChargeEffect;

		// Token: 0x04000924 RID: 2340
		private GameObject rightHandChargeEffect;
	}
}
