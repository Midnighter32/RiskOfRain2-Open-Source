using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000151 RID: 337
	internal class ChargeArrow : BaseState
	{
		// Token: 0x0600067D RID: 1661 RVA: 0x0001EACC File Offset: 0x0001CCCC
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

		// Token: 0x0600067E RID: 1662 RVA: 0x0001EB7C File Offset: 0x0001CD7C
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

		// Token: 0x0600067F RID: 1663 RVA: 0x0001EBC8 File Offset: 0x0001CDC8
		private void FireOrbArrow()
		{
			if (!base.isServer)
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
				EffectManager.instance.SimpleMuzzleFlash(ChargeArrow.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0001ED58 File Offset: 0x0001CF58
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
					EffectManager.instance.SimpleMuzzleFlash(ChargeArrow.chargeEffectPrefab, base.gameObject, this.muzzleString, false);
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

		// Token: 0x06000681 RID: 1665 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040007CC RID: 1996
		public static float baseTotalDuration;

		// Token: 0x040007CD RID: 1997
		public static float baseMaxChargeTime;

		// Token: 0x040007CE RID: 1998
		public static int maxCharges;

		// Token: 0x040007CF RID: 1999
		public static GameObject chargeEffectPrefab;

		// Token: 0x040007D0 RID: 2000
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x040007D1 RID: 2001
		public static string chargeStockSoundString;

		// Token: 0x040007D2 RID: 2002
		public static string chargeLoopStartSoundString;

		// Token: 0x040007D3 RID: 2003
		public static string chargeLoopStopSoundString;

		// Token: 0x040007D4 RID: 2004
		public static float minBonusBloom;

		// Token: 0x040007D5 RID: 2005
		public static float maxBonusBloom;

		// Token: 0x040007D6 RID: 2006
		public static float minArrowDamageCoefficient;

		// Token: 0x040007D7 RID: 2007
		public static float maxArrowDamageCoefficient;

		// Token: 0x040007D8 RID: 2008
		public static float orbDamageCoefficient;

		// Token: 0x040007D9 RID: 2009
		public static float orbRange;

		// Token: 0x040007DA RID: 2010
		public static float orbFrequency;

		// Token: 0x040007DB RID: 2011
		public static float orbProcCoefficient;

		// Token: 0x040007DC RID: 2012
		private float stopwatch;

		// Token: 0x040007DD RID: 2013
		private GameObject chargeLeftInstance;

		// Token: 0x040007DE RID: 2014
		private GameObject chargeRightInstance;

		// Token: 0x040007DF RID: 2015
		private Animator animator;

		// Token: 0x040007E0 RID: 2016
		private int charge;

		// Token: 0x040007E1 RID: 2017
		private int lastCharge;

		// Token: 0x040007E2 RID: 2018
		private ChildLocator childLocator;

		// Token: 0x040007E3 RID: 2019
		private float totalDuration;

		// Token: 0x040007E4 RID: 2020
		private float maxChargeTime;

		// Token: 0x040007E5 RID: 2021
		private bool cachedSprinting;

		// Token: 0x040007E6 RID: 2022
		private float originalMinYaw;

		// Token: 0x040007E7 RID: 2023
		private float originalMaxYaw;

		// Token: 0x040007E8 RID: 2024
		private string muzzleString;
	}
}
