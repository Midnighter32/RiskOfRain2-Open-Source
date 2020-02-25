using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008EF RID: 2287
	public class SpawnWards : BaseState
	{
		// Token: 0x06003320 RID: 13088 RVA: 0x000DD93C File Offset: 0x000DBB3C
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.childLocator = this.animator.GetComponent<ChildLocator>();
			this.duration = SpawnWards.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(SpawnWards.attackSoundString, base.gameObject, this.attackSpeedStat);
			base.PlayCrossfade("Gesture", "SpawnWards", "SpawnWards.playbackRate", this.duration, 0.5f);
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x000DD9B8 File Offset: 0x000DBBB8
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

		// Token: 0x06003322 RID: 13090 RVA: 0x000DDA2C File Offset: 0x000DBC2C
		private void FireOrbs()
		{
			if (!NetworkServer.active)
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
			EffectManager.SimpleMuzzleFlash(SpawnWards.muzzleflashEffectPrefab, base.gameObject, SpawnWards.muzzleString, true);
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

		// Token: 0x06003323 RID: 13091 RVA: 0x000DDB1C File Offset: 0x000DBD1C
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

		// Token: 0x0400327F RID: 12927
		public static string muzzleString;

		// Token: 0x04003280 RID: 12928
		public static string attackSoundString;

		// Token: 0x04003281 RID: 12929
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04003282 RID: 12930
		public static float baseDuration = 0.9f;

		// Token: 0x04003283 RID: 12931
		public static float orbRange;

		// Token: 0x04003284 RID: 12932
		public static float orbTravelSpeed;

		// Token: 0x04003285 RID: 12933
		public static int orbCountMax;

		// Token: 0x04003286 RID: 12934
		private float stopwatch;

		// Token: 0x04003287 RID: 12935
		private int orbCount;

		// Token: 0x04003288 RID: 12936
		private float duration;

		// Token: 0x04003289 RID: 12937
		private bool hasFiredOrbs;

		// Token: 0x0400328A RID: 12938
		private Animator animator;

		// Token: 0x0400328B RID: 12939
		private ChildLocator childLocator;
	}
}
