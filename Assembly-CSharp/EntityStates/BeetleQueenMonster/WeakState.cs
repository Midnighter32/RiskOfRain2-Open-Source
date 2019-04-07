using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D6 RID: 470
	public class WeakState : BaseState
	{
		// Token: 0x0600092D RID: 2349 RVA: 0x0002E240 File Offset: 0x0002C440
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

		// Token: 0x0600092E RID: 2350 RVA: 0x0002E310 File Offset: 0x0002C510
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

		// Token: 0x0600092F RID: 2351 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04000C72 RID: 3186
		private float stopwatch;

		// Token: 0x04000C73 RID: 3187
		private float grubStopwatch;

		// Token: 0x04000C74 RID: 3188
		public static float weakDuration;

		// Token: 0x04000C75 RID: 3189
		public static float weakToIdleTransitionDuration;

		// Token: 0x04000C76 RID: 3190
		public static string weakPointChildString;

		// Token: 0x04000C77 RID: 3191
		public static int maxGrubCount;

		// Token: 0x04000C78 RID: 3192
		public static float grubSpawnFrequency;

		// Token: 0x04000C79 RID: 3193
		public static float grubSpawnDelay;

		// Token: 0x04000C7A RID: 3194
		private int grubCount;

		// Token: 0x04000C7B RID: 3195
		private bool beginExitTransition;

		// Token: 0x04000C7C RID: 3196
		private ChildLocator childLocator;
	}
}
