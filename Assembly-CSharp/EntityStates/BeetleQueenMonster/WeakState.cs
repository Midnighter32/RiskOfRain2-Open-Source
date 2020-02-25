using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008F1 RID: 2289
	public class WeakState : BaseState
	{
		// Token: 0x0600332D RID: 13101 RVA: 0x000DDEAC File Offset: 0x000DC0AC
		public override void OnEnter()
		{
			base.OnEnter();
			this.grubStopwatch -= WeakState.grubSpawnDelay;
			if (base.sfxLocator && base.sfxLocator.barkSound != "")
			{
				Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);
			}
			base.PlayAnimation("Body", "WeakEnter");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					Transform transform = this.childLocator.FindChild(WeakState.weakPointChildString);
					if (transform)
					{
						UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/WeakPointProcEffect"), transform.position, transform.rotation);
					}
				}
			}
		}

		// Token: 0x0600332E RID: 13102 RVA: 0x000DDF7C File Offset: 0x000DC17C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.grubStopwatch += Time.fixedDeltaTime;
			if (this.grubStopwatch >= 1f / WeakState.grubSpawnFrequency && this.grubCount < WeakState.maxGrubCount)
			{
				this.grubCount++;
				this.grubStopwatch -= 1f / WeakState.grubSpawnFrequency;
				if (NetworkServer.active)
				{
					Transform transform = this.childLocator.FindChild("GrubSpawnPoint" + UnityEngine.Random.Range(1, 10));
					if (transform)
					{
						NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GrubPack"), transform.transform.position, UnityEngine.Random.rotation));
						transform.gameObject.SetActive(false);
					}
				}
			}
			if (this.stopwatch >= WeakState.weakDuration - WeakState.weakToIdleTransitionDuration && !this.beginExitTransition)
			{
				this.beginExitTransition = true;
				base.PlayCrossfade("Body", "WeakExit", "WeakExit.playbackRate", WeakState.weakToIdleTransitionDuration, 0.5f);
			}
			if (this.stopwatch >= WeakState.weakDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600332F RID: 13103 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x0400329A RID: 12954
		private float stopwatch;

		// Token: 0x0400329B RID: 12955
		private float grubStopwatch;

		// Token: 0x0400329C RID: 12956
		public static float weakDuration;

		// Token: 0x0400329D RID: 12957
		public static float weakToIdleTransitionDuration;

		// Token: 0x0400329E RID: 12958
		public static string weakPointChildString;

		// Token: 0x0400329F RID: 12959
		public static int maxGrubCount;

		// Token: 0x040032A0 RID: 12960
		public static float grubSpawnFrequency;

		// Token: 0x040032A1 RID: 12961
		public static float grubSpawnDelay;

		// Token: 0x040032A2 RID: 12962
		private int grubCount;

		// Token: 0x040032A3 RID: 12963
		private bool beginExitTransition;

		// Token: 0x040032A4 RID: 12964
		private ChildLocator childLocator;
	}
}
