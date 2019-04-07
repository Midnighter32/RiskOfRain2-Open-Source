using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bison
{
	// Token: 0x020001C2 RID: 450
	public class Headbutt : BaseState
	{
		// Token: 0x060008CE RID: 2254 RVA: 0x0002C6C4 File Offset: 0x0002A8C4
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			this.animator = modelTransform.GetComponent<Animator>();
			this.headbuttDuration = Headbutt.baseHeadbuttDuration / this.attackSpeedStat;
			Util.PlaySound(Headbutt.attackSoundString, base.gameObject);
			base.PlayCrossfade("Body", "Headbutt", "Headbutt.playbackRate", this.headbuttDuration, 0.2f);
			base.characterMotor.moveDirection = Vector3.zero;
			base.characterDirection.moveVector = base.characterDirection.forward;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = Headbutt.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = Headbutt.hitEffectPrefab;
			this.attack.forceVector = Vector3.up * Headbutt.upwardForceMagnitude;
			this.attack.pushAwayForce = Headbutt.awayForceMagnitude;
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Headbutt");
			}
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0002C82C File Offset: 0x0002AA2C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator && this.animator.GetFloat("Headbutt.hitBoxActive") > 0.5f)
			{
				if (NetworkServer.active)
				{
					this.attack.Fire(null);
				}
				if (base.isAuthority && !this.hasAttacked)
				{
					this.hasAttacked = true;
				}
			}
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > this.headbuttDuration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000BED RID: 3053
		public static float baseHeadbuttDuration;

		// Token: 0x04000BEE RID: 3054
		public static float damageCoefficient;

		// Token: 0x04000BEF RID: 3055
		public static string attackSoundString;

		// Token: 0x04000BF0 RID: 3056
		public static GameObject hitEffectPrefab;

		// Token: 0x04000BF1 RID: 3057
		public static float upwardForceMagnitude;

		// Token: 0x04000BF2 RID: 3058
		public static float awayForceMagnitude;

		// Token: 0x04000BF3 RID: 3059
		private float headbuttDuration;

		// Token: 0x04000BF4 RID: 3060
		private float stopwatch;

		// Token: 0x04000BF5 RID: 3061
		private OverlapAttack attack;

		// Token: 0x04000BF6 RID: 3062
		private Animator animator;

		// Token: 0x04000BF7 RID: 3063
		private bool hasAttacked;
	}
}
