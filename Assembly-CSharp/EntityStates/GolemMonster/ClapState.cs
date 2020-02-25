using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x0200085D RID: 2141
	public class ClapState : BaseState
	{
		// Token: 0x06003058 RID: 12376 RVA: 0x000D022C File Offset: 0x000CE42C
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

		// Token: 0x06003059 RID: 12377 RVA: 0x000D02FB File Offset: 0x000CE4FB
		public override void OnExit()
		{
			EntityState.Destroy(this.leftHandChargeEffect);
			EntityState.Destroy(this.rightHandChargeEffect);
			base.OnExit();
		}

		// Token: 0x0600305A RID: 12378 RVA: 0x000D031C File Offset: 0x000CE51C
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

		// Token: 0x0600305B RID: 12379 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002E87 RID: 11911
		public static float duration = 3.5f;

		// Token: 0x04002E88 RID: 11912
		public static float damageCoefficient = 4f;

		// Token: 0x04002E89 RID: 11913
		public static float forceMagnitude = 16f;

		// Token: 0x04002E8A RID: 11914
		public static float radius = 3f;

		// Token: 0x04002E8B RID: 11915
		private BlastAttack attack;

		// Token: 0x04002E8C RID: 11916
		public static string attackSoundString;

		// Token: 0x04002E8D RID: 11917
		private Animator modelAnimator;

		// Token: 0x04002E8E RID: 11918
		private Transform modelTransform;

		// Token: 0x04002E8F RID: 11919
		private bool hasAttacked;

		// Token: 0x04002E90 RID: 11920
		private GameObject leftHandChargeEffect;

		// Token: 0x04002E91 RID: 11921
		private GameObject rightHandChargeEffect;
	}
}
