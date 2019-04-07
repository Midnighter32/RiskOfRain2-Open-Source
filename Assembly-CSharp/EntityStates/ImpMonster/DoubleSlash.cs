using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000148 RID: 328
	public class DoubleSlash : BaseState
	{
		// Token: 0x0600064C RID: 1612 RVA: 0x0001D694 File Offset: 0x0001B894
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

		// Token: 0x0600064D RID: 1613 RVA: 0x0001C726 File Offset: 0x0001A926
		public override void OnExit()
		{
			base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			base.OnExit();
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0001D810 File Offset: 0x0001BA10
		private void HandleSlash(string animatorParamName, string muzzleName, string hitBoxGroupName)
		{
			if (this.modelAnimator.GetFloat(animatorParamName) > 0.1f)
			{
				Util.PlaySound(DoubleSlash.slashSoundString, base.gameObject);
				EffectManager.instance.SimpleMuzzleFlash(DoubleSlash.swipeEffectPrefab, base.gameObject, muzzleName, true);
				this.slashCount++;
				if (this.modelTransform)
				{
					this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitBoxGroupName);
				}
				if (base.healthComponent)
				{
					base.healthComponent.TakeDamageForce(base.characterDirection.forward * DoubleSlash.selfForce, true);
				}
				this.attack.ResetIgnoredHealthComponents();
				if (base.characterDirection)
				{
					this.attack.forceVector = base.characterDirection.forward * DoubleSlash.forceMagnitude;
				}
				this.attack.Fire(null);
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0001D920 File Offset: 0x0001BB20
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

		// Token: 0x06000650 RID: 1616 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000777 RID: 1911
		public static float baseDuration = 3.5f;

		// Token: 0x04000778 RID: 1912
		public static float damageCoefficient = 4f;

		// Token: 0x04000779 RID: 1913
		public static float procCoefficient;

		// Token: 0x0400077A RID: 1914
		public static float selfForce;

		// Token: 0x0400077B RID: 1915
		public static float forceMagnitude = 16f;

		// Token: 0x0400077C RID: 1916
		public static GameObject hitEffectPrefab;

		// Token: 0x0400077D RID: 1917
		public static GameObject swipeEffectPrefab;

		// Token: 0x0400077E RID: 1918
		public static string enterSoundString;

		// Token: 0x0400077F RID: 1919
		public static string slashSoundString;

		// Token: 0x04000780 RID: 1920
		public static float walkSpeedPenaltyCoefficient;

		// Token: 0x04000781 RID: 1921
		private OverlapAttack attack;

		// Token: 0x04000782 RID: 1922
		private Animator modelAnimator;

		// Token: 0x04000783 RID: 1923
		private float duration;

		// Token: 0x04000784 RID: 1924
		private int slashCount;

		// Token: 0x04000785 RID: 1925
		private Transform modelTransform;
	}
}
