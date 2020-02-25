using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x0200071A RID: 1818
	public class SpawnTeleporterState : BaseState
	{
		// Token: 0x06002A5E RID: 10846 RVA: 0x000B23D8 File Offset: 0x000B05D8
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			if (this.modelAnimator)
			{
				GameObject gameObject = this.modelAnimator.gameObject;
				this.characterModel = gameObject.GetComponent<CharacterModel>();
				this.characterModel.invisibilityCount++;
			}
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x000B2448 File Offset: 0x000B0648
		public override void OnExit()
		{
			base.OnExit();
			if (!this.hasTeleported)
			{
				this.characterModel.invisibilityCount--;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			if (NetworkServer.active)
			{
				base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, 3f);
			}
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x000B24A8 File Offset: 0x000B06A8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnTeleporterState.initialDelay && !this.hasTeleported)
			{
				this.hasTeleported = true;
				this.characterModel.invisibilityCount--;
				this.duration = SpawnTeleporterState.initialDelay;
				TeleportOutController.AddTPOutEffect(this.characterModel, 1f, 0f, this.duration);
				GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(base.gameObject);
				if (teleportEffectPrefab)
				{
					EffectManager.SimpleEffect(teleportEffectPrefab, base.transform.position, Quaternion.identity, false);
				}
				Util.PlaySound(SpawnTeleporterState.soundString, base.gameObject);
			}
			if (base.fixedAge >= this.duration && this.hasTeleported && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400262B RID: 9771
		private float duration = 4f;

		// Token: 0x0400262C RID: 9772
		public static string soundString;

		// Token: 0x0400262D RID: 9773
		public static float initialDelay;

		// Token: 0x0400262E RID: 9774
		private bool hasTeleported;

		// Token: 0x0400262F RID: 9775
		private Animator modelAnimator;

		// Token: 0x04002630 RID: 9776
		private PrintController printController;

		// Token: 0x04002631 RID: 9777
		private CharacterModel characterModel;
	}
}
