using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress
{
	// Token: 0x0200082D RID: 2093
	public class BackflipState : BaseState
	{
		// Token: 0x06002F65 RID: 12133 RVA: 0x000CA4C8 File Offset: 0x000C86C8
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			this.childLocator = modelTransform.GetComponent<ChildLocator>();
			base.characterMotor.velocity.y = Mathf.Max(base.characterMotor.velocity.y, 0f);
			this.animator = base.GetModelAnimator();
			Util.PlaySound(BackflipState.dodgeSoundString, base.gameObject);
			this.orbStopwatch = -BackflipState.orbPrefireDuration;
			if (base.characterMotor && BackflipState.smallHopStrength != 0f)
			{
				base.characterMotor.velocity.y = BackflipState.smallHopStrength;
			}
			if (base.isAuthority && base.inputBank)
			{
				this.forwardDirection = -Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
			}
			base.characterDirection.moveVector = -this.forwardDirection;
			base.PlayAnimation("FullBody, Override", "Backflip", "Backflip.playbackRate", BackflipState.duration);
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x000CA5D4 File Offset: 0x000C87D4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.orbStopwatch += Time.fixedDeltaTime;
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = Mathf.Lerp(BackflipState.dodgeFOV, 60f, this.stopwatch / BackflipState.duration);
			}
			if (base.characterMotor && base.characterDirection)
			{
				Vector3 velocity = base.characterMotor.velocity;
				Vector3 velocity2 = this.forwardDirection * (this.moveSpeedStat * Mathf.Lerp(BackflipState.initialSpeedCoefficient, BackflipState.finalSpeedCoefficient, this.stopwatch / BackflipState.duration));
				base.characterMotor.velocity = velocity2;
				base.characterMotor.velocity.y = velocity.y;
				base.characterMotor.moveDirection = this.forwardDirection;
			}
			if (this.orbStopwatch >= 1f / BackflipState.orbFrequency / this.attackSpeedStat && this.orbCount < BackflipState.orbCountMax)
			{
				this.orbStopwatch -= 1f / BackflipState.orbFrequency / this.attackSpeedStat;
				this.FireOrbArrow();
			}
			if (this.stopwatch >= BackflipState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002F67 RID: 12135 RVA: 0x000CA734 File Offset: 0x000C8934
		private void FireOrbArrow()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.orbCount++;
			ArrowOrb arrowOrb = new ArrowOrb();
			arrowOrb.damageValue = base.characterBody.damage * BackflipState.orbDamageCoefficient;
			arrowOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			arrowOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			arrowOrb.attacker = base.gameObject;
			arrowOrb.damageColorIndex = DamageColorIndex.Poison;
			arrowOrb.procChainMask.AddProc(ProcType.HealOnHit);
			arrowOrb.procCoefficient = BackflipState.orbProcCoefficient;
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = BackflipState.orbRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(arrowOrb.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			HurtBox hurtBox = (list.Count > 0) ? list[UnityEngine.Random.Range(0, list.Count)] : null;
			if (hurtBox)
			{
				Transform transform = this.childLocator.FindChild(BackflipState.muzzleString).transform;
				EffectManager.SimpleMuzzleFlash(BackflipState.muzzleflashEffectPrefab, base.gameObject, BackflipState.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06002F68 RID: 12136 RVA: 0x000CA8C8 File Offset: 0x000C8AC8
		public override void OnExit()
		{
			base.OnExit();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
			int layerIndex = this.animator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				this.animator.SetLayerWeight(layerIndex, 1.5f);
				this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
			}
		}

		// Token: 0x04002CE6 RID: 11494
		public static float duration = 0.9f;

		// Token: 0x04002CE7 RID: 11495
		public static float initialSpeedCoefficient;

		// Token: 0x04002CE8 RID: 11496
		public static float finalSpeedCoefficient;

		// Token: 0x04002CE9 RID: 11497
		public static string dodgeSoundString;

		// Token: 0x04002CEA RID: 11498
		public static float dodgeFOV;

		// Token: 0x04002CEB RID: 11499
		public static float orbDamageCoefficient;

		// Token: 0x04002CEC RID: 11500
		public static float orbRange;

		// Token: 0x04002CED RID: 11501
		public static int orbCountMax;

		// Token: 0x04002CEE RID: 11502
		public static float orbPrefireDuration;

		// Token: 0x04002CEF RID: 11503
		public static float orbFrequency;

		// Token: 0x04002CF0 RID: 11504
		public static float orbProcCoefficient;

		// Token: 0x04002CF1 RID: 11505
		public static string muzzleString;

		// Token: 0x04002CF2 RID: 11506
		public static float smallHopStrength;

		// Token: 0x04002CF3 RID: 11507
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002CF4 RID: 11508
		private ChildLocator childLocator;

		// Token: 0x04002CF5 RID: 11509
		private float stopwatch;

		// Token: 0x04002CF6 RID: 11510
		private float orbStopwatch;

		// Token: 0x04002CF7 RID: 11511
		private Vector3 forwardDirection;

		// Token: 0x04002CF8 RID: 11512
		private Animator animator;

		// Token: 0x04002CF9 RID: 11513
		private int orbCount;
	}
}
