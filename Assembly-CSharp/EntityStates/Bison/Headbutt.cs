using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bison
{
	// Token: 0x020008DD RID: 2269
	public class Headbutt : BaseState
	{
		// Token: 0x060032CD RID: 13005 RVA: 0x000DC320 File Offset: 0x000DA520
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

		// Token: 0x060032CE RID: 13006 RVA: 0x000DC488 File Offset: 0x000DA688
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

		// Token: 0x060032CF RID: 13007 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003216 RID: 12822
		public static float baseHeadbuttDuration;

		// Token: 0x04003217 RID: 12823
		public static float damageCoefficient;

		// Token: 0x04003218 RID: 12824
		public static string attackSoundString;

		// Token: 0x04003219 RID: 12825
		public static GameObject hitEffectPrefab;

		// Token: 0x0400321A RID: 12826
		public static float upwardForceMagnitude;

		// Token: 0x0400321B RID: 12827
		public static float awayForceMagnitude;

		// Token: 0x0400321C RID: 12828
		private float headbuttDuration;

		// Token: 0x0400321D RID: 12829
		private float stopwatch;

		// Token: 0x0400321E RID: 12830
		private OverlapAttack attack;

		// Token: 0x0400321F RID: 12831
		private Animator animator;

		// Token: 0x04003220 RID: 12832
		private bool hasAttacked;
	}
}
