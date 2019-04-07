using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200014E RID: 334
	public class BackflipState : BaseState
	{
		// Token: 0x0600066B RID: 1643 RVA: 0x0001DF68 File Offset: 0x0001C168
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

		// Token: 0x0600066C RID: 1644 RVA: 0x0001E074 File Offset: 0x0001C274
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

		// Token: 0x0600066D RID: 1645 RVA: 0x0001E1D4 File Offset: 0x0001C3D4
		private void FireOrbArrow()
		{
			if (!base.isServer)
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
				EffectManager.instance.SimpleMuzzleFlash(BackflipState.muzzleflashEffectPrefab, base.gameObject, BackflipState.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0001E370 File Offset: 0x0001C570
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

		// Token: 0x040007A2 RID: 1954
		public static float duration = 0.9f;

		// Token: 0x040007A3 RID: 1955
		public static float initialSpeedCoefficient;

		// Token: 0x040007A4 RID: 1956
		public static float finalSpeedCoefficient;

		// Token: 0x040007A5 RID: 1957
		public static string dodgeSoundString;

		// Token: 0x040007A6 RID: 1958
		public static float dodgeFOV;

		// Token: 0x040007A7 RID: 1959
		public static float orbDamageCoefficient;

		// Token: 0x040007A8 RID: 1960
		public static float orbRange;

		// Token: 0x040007A9 RID: 1961
		public static int orbCountMax;

		// Token: 0x040007AA RID: 1962
		public static float orbPrefireDuration;

		// Token: 0x040007AB RID: 1963
		public static float orbFrequency;

		// Token: 0x040007AC RID: 1964
		public static float orbProcCoefficient;

		// Token: 0x040007AD RID: 1965
		public static string muzzleString;

		// Token: 0x040007AE RID: 1966
		public static float smallHopStrength;

		// Token: 0x040007AF RID: 1967
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x040007B0 RID: 1968
		private ChildLocator childLocator;

		// Token: 0x040007B1 RID: 1969
		private float stopwatch;

		// Token: 0x040007B2 RID: 1970
		private float orbStopwatch;

		// Token: 0x040007B3 RID: 1971
		private Vector3 forwardDirection;

		// Token: 0x040007B4 RID: 1972
		private Animator animator;

		// Token: 0x040007B5 RID: 1973
		private int orbCount;
	}
}
