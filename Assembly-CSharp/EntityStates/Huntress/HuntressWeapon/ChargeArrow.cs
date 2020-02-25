using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000833 RID: 2099
	public class ChargeArrow : BaseState
	{
		// Token: 0x06002F7E RID: 12158 RVA: 0x000CB1A0 File Offset: 0x000C93A0
		public override void OnEnter()
		{
			base.OnEnter();
			this.totalDuration = ChargeArrow.baseTotalDuration / this.attackSpeedStat;
			this.maxChargeTime = ChargeArrow.baseMaxChargeTime / this.attackSpeedStat;
			this.muzzleString = "Muzzle";
			Transform modelTransform = base.GetModelTransform();
			this.childLocator = modelTransform.GetComponent<ChildLocator>();
			this.animator = base.GetModelAnimator();
			this.cachedSprinting = base.characterBody.isSprinting;
			if (!this.cachedSprinting)
			{
				this.animator.SetBool("chargingArrow", true);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.maxChargeTime + 1f);
			}
		}

		// Token: 0x06002F7F RID: 12159 RVA: 0x000CB250 File Offset: 0x000C9450
		public override void OnExit()
		{
			base.OnExit();
			this.animator.SetBool("chargingArrow", false);
			if (!this.cachedSprinting)
			{
				base.PlayAnimation("Gesture, Override", "BufferEmpty");
				base.PlayAnimation("Gesture, Additive", "BufferEmpty");
			}
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x000CB29C File Offset: 0x000C949C
		private void FireOrbArrow()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			ArrowOrb arrowOrb = new ArrowOrb();
			arrowOrb.damageValue = base.characterBody.damage * ChargeArrow.orbDamageCoefficient;
			arrowOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			arrowOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			arrowOrb.attacker = base.gameObject;
			arrowOrb.damageColorIndex = DamageColorIndex.Poison;
			arrowOrb.procChainMask.AddProc(ProcType.HealOnHit);
			arrowOrb.procCoefficient = ChargeArrow.orbProcCoefficient;
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = ChargeArrow.orbRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(arrowOrb.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			HurtBox hurtBox = (list.Count > 0) ? list[UnityEngine.Random.Range(0, list.Count)] : null;
			if (hurtBox)
			{
				Transform transform = this.childLocator.FindChild(this.muzzleString).transform;
				EffectManager.SimpleMuzzleFlash(ChargeArrow.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06002F81 RID: 12161 RVA: 0x000CB424 File Offset: 0x000C9624
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.cachedSprinting != base.characterBody.isSprinting && base.isAuthority)
			{
				Debug.Log("switched states");
				this.outer.SetNextStateToMain();
				return;
			}
			if (!this.cachedSprinting)
			{
				this.lastCharge = this.charge;
				this.stopwatch += Time.fixedDeltaTime;
				this.charge = Mathf.Min((int)(this.stopwatch / this.maxChargeTime * (float)ChargeArrow.maxCharges), ChargeArrow.maxCharges);
				float damageCoefficient = Mathf.Lerp(ChargeArrow.minArrowDamageCoefficient, ChargeArrow.maxArrowDamageCoefficient, (float)this.charge);
				if (this.lastCharge < this.charge && this.charge == ChargeArrow.maxCharges)
				{
					EffectManager.SimpleMuzzleFlash(ChargeArrow.chargeEffectPrefab, base.gameObject, this.muzzleString, false);
				}
				if ((this.stopwatch >= this.totalDuration || !base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
				{
					FireArrow fireArrow = new FireArrow();
					fireArrow.damageCoefficient = damageCoefficient;
					this.outer.SetNextState(fireArrow);
					return;
				}
			}
			else
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch >= 1f / ChargeArrow.orbFrequency / this.attackSpeedStat)
				{
					this.stopwatch -= 1f / ChargeArrow.orbFrequency / this.attackSpeedStat;
					this.FireOrbArrow();
				}
				if ((!base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D12 RID: 11538
		public static float baseTotalDuration;

		// Token: 0x04002D13 RID: 11539
		public static float baseMaxChargeTime;

		// Token: 0x04002D14 RID: 11540
		public static int maxCharges;

		// Token: 0x04002D15 RID: 11541
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002D16 RID: 11542
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002D17 RID: 11543
		public static string chargeStockSoundString;

		// Token: 0x04002D18 RID: 11544
		public static string chargeLoopStartSoundString;

		// Token: 0x04002D19 RID: 11545
		public static string chargeLoopStopSoundString;

		// Token: 0x04002D1A RID: 11546
		public static float minBonusBloom;

		// Token: 0x04002D1B RID: 11547
		public static float maxBonusBloom;

		// Token: 0x04002D1C RID: 11548
		public static float minArrowDamageCoefficient;

		// Token: 0x04002D1D RID: 11549
		public static float maxArrowDamageCoefficient;

		// Token: 0x04002D1E RID: 11550
		public static float orbDamageCoefficient;

		// Token: 0x04002D1F RID: 11551
		public static float orbRange;

		// Token: 0x04002D20 RID: 11552
		public static float orbFrequency;

		// Token: 0x04002D21 RID: 11553
		public static float orbProcCoefficient;

		// Token: 0x04002D22 RID: 11554
		private float stopwatch;

		// Token: 0x04002D23 RID: 11555
		private GameObject chargeLeftInstance;

		// Token: 0x04002D24 RID: 11556
		private GameObject chargeRightInstance;

		// Token: 0x04002D25 RID: 11557
		private Animator animator;

		// Token: 0x04002D26 RID: 11558
		private int charge;

		// Token: 0x04002D27 RID: 11559
		private int lastCharge;

		// Token: 0x04002D28 RID: 11560
		private ChildLocator childLocator;

		// Token: 0x04002D29 RID: 11561
		private float totalDuration;

		// Token: 0x04002D2A RID: 11562
		private float maxChargeTime;

		// Token: 0x04002D2B RID: 11563
		private bool cachedSprinting;

		// Token: 0x04002D2C RID: 11564
		private float originalMinYaw;

		// Token: 0x04002D2D RID: 11565
		private float originalMaxYaw;

		// Token: 0x04002D2E RID: 11566
		private string muzzleString;
	}
}
