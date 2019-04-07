using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D4 RID: 468
	public class SpawnWards : BaseState
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x0002DCE0 File Offset: 0x0002BEE0
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.childLocator = this.animator.GetComponent<ChildLocator>();
			this.duration = SpawnWards.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(SpawnWards.attackSoundString, base.gameObject, this.attackSpeedStat);
			base.PlayCrossfade("Gesture", "SpawnWards", "SpawnWards.playbackRate", this.duration, 0.5f);
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0002DD5C File Offset: 0x0002BF5C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (!this.hasFiredOrbs && this.animator.GetFloat("SpawnWards.active") > 0.5f)
			{
				this.hasFiredOrbs = true;
				this.FireOrbs();
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0002DDD0 File Offset: 0x0002BFD0
		private void FireOrbs()
		{
			if (!base.isServer)
			{
				return;
			}
			Transform transform = this.childLocator.FindChild(SpawnWards.muzzleString).transform;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = transform.position;
			bullseyeSearch.searchDirection = transform.forward;
			bullseyeSearch.maxDistanceFilter = SpawnWards.orbRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			EffectManager.instance.SimpleMuzzleFlash(SpawnWards.muzzleflashEffectPrefab, base.gameObject, SpawnWards.muzzleString, true);
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			for (int i = 0; i < list.Count; i++)
			{
				HurtBox target = list[i];
				BeetleWardOrb beetleWardOrb = new BeetleWardOrb();
				beetleWardOrb.origin = transform.position;
				beetleWardOrb.target = target;
				beetleWardOrb.speed = SpawnWards.orbTravelSpeed;
				OrbManager.instance.AddOrb(beetleWardOrb);
			}
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0002DEC8 File Offset: 0x0002C0C8
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

		// Token: 0x04000C57 RID: 3159
		public static string muzzleString;

		// Token: 0x04000C58 RID: 3160
		public static string attackSoundString;

		// Token: 0x04000C59 RID: 3161
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04000C5A RID: 3162
		public static float baseDuration = 0.9f;

		// Token: 0x04000C5B RID: 3163
		public static float orbRange;

		// Token: 0x04000C5C RID: 3164
		public static float orbTravelSpeed;

		// Token: 0x04000C5D RID: 3165
		public static int orbCountMax;

		// Token: 0x04000C5E RID: 3166
		private float stopwatch;

		// Token: 0x04000C5F RID: 3167
		private int orbCount;

		// Token: 0x04000C60 RID: 3168
		private float duration;

		// Token: 0x04000C61 RID: 3169
		private bool hasFiredOrbs;

		// Token: 0x04000C62 RID: 3170
		private Animator animator;

		// Token: 0x04000C63 RID: 3171
		private ChildLocator childLocator;
	}
}
