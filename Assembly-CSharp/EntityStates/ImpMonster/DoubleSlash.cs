using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000824 RID: 2084
	public class DoubleSlash : BaseState
	{
		// Token: 0x06002F39 RID: 12089 RVA: 0x000C9A24 File Offset: 0x000C7C24
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = DoubleSlash.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			base.characterMotor.walkSpeedPenaltyCoefficient = DoubleSlash.walkSpeedPenaltyCoefficient;
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = DoubleSlash.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = DoubleSlash.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			this.attack.procCoefficient = DoubleSlash.procCoefficient;
			this.attack.damageType = DamageType.BleedOnHit;
			Util.PlayScaledSound(DoubleSlash.enterSoundString, base.gameObject, this.attackSpeedStat);
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture, Additive", "DoubleSlash", "DoubleSlash.playbackRate", this.duration);
				base.PlayAnimation("Gesture, Override", "DoubleSlash", "DoubleSlash.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration + 2f);
			}
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x000C8ACA File Offset: 0x000C6CCA
		public override void OnExit()
		{
			base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			base.OnExit();
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x000C9BA0 File Offset: 0x000C7DA0
		private void HandleSlash(string animatorParamName, string muzzleName, string hitBoxGroupName)
		{
			if (this.modelAnimator.GetFloat(animatorParamName) > 0.1f)
			{
				Util.PlaySound(DoubleSlash.slashSoundString, base.gameObject);
				EffectManager.SimpleMuzzleFlash(DoubleSlash.swipeEffectPrefab, base.gameObject, muzzleName, true);
				this.slashCount++;
				if (this.modelTransform)
				{
					this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitBoxGroupName);
				}
				if (base.healthComponent)
				{
					base.healthComponent.TakeDamageForce(base.characterDirection.forward * DoubleSlash.selfForce, true, false);
				}
				this.attack.ResetIgnoredHealthComponents();
				if (base.characterDirection)
				{
					this.attack.forceVector = base.characterDirection.forward * DoubleSlash.forceMagnitude;
				}
				this.attack.Fire(null);
			}
		}

		// Token: 0x06002F3C RID: 12092 RVA: 0x000C9CAC File Offset: 0x000C7EAC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator)
			{
				switch (this.slashCount)
				{
				case 0:
					this.HandleSlash("HandR.hitBoxActive", "SwipeRight", "HandR");
					break;
				case 1:
					this.HandleSlash("HandL.hitBoxActive", "SwipeLeft", "HandL");
					break;
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002CB7 RID: 11447
		public static float baseDuration = 3.5f;

		// Token: 0x04002CB8 RID: 11448
		public static float damageCoefficient = 4f;

		// Token: 0x04002CB9 RID: 11449
		public static float procCoefficient;

		// Token: 0x04002CBA RID: 11450
		public static float selfForce;

		// Token: 0x04002CBB RID: 11451
		public static float forceMagnitude = 16f;

		// Token: 0x04002CBC RID: 11452
		public static GameObject hitEffectPrefab;

		// Token: 0x04002CBD RID: 11453
		public static GameObject swipeEffectPrefab;

		// Token: 0x04002CBE RID: 11454
		public static string enterSoundString;

		// Token: 0x04002CBF RID: 11455
		public static string slashSoundString;

		// Token: 0x04002CC0 RID: 11456
		public static float walkSpeedPenaltyCoefficient;

		// Token: 0x04002CC1 RID: 11457
		private OverlapAttack attack;

		// Token: 0x04002CC2 RID: 11458
		private Animator modelAnimator;

		// Token: 0x04002CC3 RID: 11459
		private float duration;

		// Token: 0x04002CC4 RID: 11460
		private int slashCount;

		// Token: 0x04002CC5 RID: 11461
		private Transform modelTransform;
	}
}
