using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200081D RID: 2077
	public class GroundPound : BaseState
	{
		// Token: 0x06002F14 RID: 12052 RVA: 0x000C8E58 File Offset: 0x000C7058
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

		// Token: 0x06002F15 RID: 12053 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002F16 RID: 12054 RVA: 0x000C8FA0 File Offset: 0x000C71A0
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
							EffectManager.SimpleMuzzleFlash(GroundPound.slamEffectPrefab, base.gameObject, "GroundPoundCenter", true);
						}
						EffectManager.SimpleMuzzleFlash(GroundPound.swipeEffectPrefab, base.gameObject, (this.attackCount % 2 == 0) ? "FireVoidspikesL" : "FireVoidspikesR", true);
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

		// Token: 0x06002F17 RID: 12055 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002C7C RID: 11388
		private float stopwatch;

		// Token: 0x04002C7D RID: 11389
		public static float baseDuration = 3.5f;

		// Token: 0x04002C7E RID: 11390
		public static float damageCoefficient = 4f;

		// Token: 0x04002C7F RID: 11391
		public static float forceMagnitude = 16f;

		// Token: 0x04002C80 RID: 11392
		public static float blastAttackRadius;

		// Token: 0x04002C81 RID: 11393
		private BlastAttack attack;

		// Token: 0x04002C82 RID: 11394
		public static string initialAttackSoundString;

		// Token: 0x04002C83 RID: 11395
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002C84 RID: 11396
		public static GameObject slamEffectPrefab;

		// Token: 0x04002C85 RID: 11397
		public static GameObject hitEffectPrefab;

		// Token: 0x04002C86 RID: 11398
		public static GameObject swipeEffectPrefab;

		// Token: 0x04002C87 RID: 11399
		private Animator modelAnimator;

		// Token: 0x04002C88 RID: 11400
		private Transform modelTransform;

		// Token: 0x04002C89 RID: 11401
		private bool hasAttacked;

		// Token: 0x04002C8A RID: 11402
		private float duration;

		// Token: 0x04002C8B RID: 11403
		private GameObject leftHandChargeEffect;

		// Token: 0x04002C8C RID: 11404
		private GameObject rightHandChargeEffect;

		// Token: 0x04002C8D RID: 11405
		private ChildLocator childLocator;

		// Token: 0x04002C8E RID: 11406
		private int attackCount;
	}
}
