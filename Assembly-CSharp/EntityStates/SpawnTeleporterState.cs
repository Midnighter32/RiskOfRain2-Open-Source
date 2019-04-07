using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000C0 RID: 192
	public class SpawnTeleporterState : BaseState
	{
		// Token: 0x060003C0 RID: 960 RVA: 0x0000F63C File Offset: 0x0000D83C
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

		// Token: 0x060003C1 RID: 961 RVA: 0x0000F6AC File Offset: 0x0000D8AC
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
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0000F6E8 File Offset: 0x0000D8E8
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
					UnityEngine.Object.Instantiate<GameObject>(teleportEffectPrefab, base.gameObject.transform.position, Quaternion.identity);
				}
				Util.PlaySound(SpawnTeleporterState.soundString, base.gameObject);
			}
			if (base.fixedAge >= this.duration && this.hasTeleported && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000371 RID: 881
		private float duration = 4f;

		// Token: 0x04000372 RID: 882
		public static string soundString;

		// Token: 0x04000373 RID: 883
		public static float initialDelay;

		// Token: 0x04000374 RID: 884
		private bool hasTeleported;

		// Token: 0x04000375 RID: 885
		private Animator modelAnimator;

		// Token: 0x04000376 RID: 886
		private PrintController printController;

		// Token: 0x04000377 RID: 887
		private CharacterModel characterModel;
	}
}
