using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x02000141 RID: 321
	public class GroundPound : BaseState
	{
		// Token: 0x06000627 RID: 1575 RVA: 0x0001CABC File Offset: 0x0001ACBC
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(GroundPound.initialAttackSoundString, base.gameObject);
			this.attack = new BlastAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			this.attack.baseDamage = this.damageStat * GroundPound.damageCoefficient;
			this.attack.baseForce = GroundPound.forceMagnitude;
			this.attack.radius = GroundPound.blastAttackRadius;
			this.attack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
			this.duration = GroundPound.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Fullbody Override", "GroundPound", "GroundPound.playbackRate", this.duration, 0.2f);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration + 3f);
			}
			if (this.modelTransform)
			{
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0001CC04 File Offset: 0x0001AE04
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.modelAnimator)
			{
				if (this.modelAnimator.GetFloat("GroundPound.hitBoxActive") > 0.5f)
				{
					if (!this.hasAttacked)
					{
						if (NetworkServer.active)
						{
							this.attack.position = this.childLocator.FindChild("GroundPoundCenter").transform.position;
							this.attack.Fire();
						}
						if (base.isAuthority)
						{
							EffectManager.instance.SimpleMuzzleFlash(GroundPound.slamEffectPrefab, base.gameObject, "GroundPoundCenter", true);
						}
						EffectManager.instance.SimpleMuzzleFlash(GroundPound.swipeEffectPrefab, base.gameObject, (this.attackCount % 2 == 0) ? "FireVoidspikesL" : "FireVoidspikesR", true);
						this.attackCount++;
						this.hasAttacked = true;
					}
				}
				else
				{
					this.hasAttacked = false;
				}
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400073C RID: 1852
		private float stopwatch;

		// Token: 0x0400073D RID: 1853
		public static float baseDuration = 3.5f;

		// Token: 0x0400073E RID: 1854
		public static float damageCoefficient = 4f;

		// Token: 0x0400073F RID: 1855
		public static float forceMagnitude = 16f;

		// Token: 0x04000740 RID: 1856
		public static float blastAttackRadius;

		// Token: 0x04000741 RID: 1857
		private BlastAttack attack;

		// Token: 0x04000742 RID: 1858
		public static string initialAttackSoundString;

		// Token: 0x04000743 RID: 1859
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000744 RID: 1860
		public static GameObject slamEffectPrefab;

		// Token: 0x04000745 RID: 1861
		public static GameObject hitEffectPrefab;

		// Token: 0x04000746 RID: 1862
		public static GameObject swipeEffectPrefab;

		// Token: 0x04000747 RID: 1863
		private Animator modelAnimator;

		// Token: 0x04000748 RID: 1864
		private Transform modelTransform;

		// Token: 0x04000749 RID: 1865
		private bool hasAttacked;

		// Token: 0x0400074A RID: 1866
		private float duration;

		// Token: 0x0400074B RID: 1867
		private GameObject leftHandChargeEffect;

		// Token: 0x0400074C RID: 1868
		private GameObject rightHandChargeEffect;

		// Token: 0x0400074D RID: 1869
		private ChildLocator childLocator;

		// Token: 0x0400074E RID: 1870
		private int attackCount;
	}
}
