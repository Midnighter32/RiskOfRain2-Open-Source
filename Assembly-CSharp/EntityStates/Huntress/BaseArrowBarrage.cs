using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200082C RID: 2092
	public class BaseArrowBarrage : BaseState
	{
		// Token: 0x06002F60 RID: 12128 RVA: 0x000CA34F File Offset: 0x000C854F
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BaseArrowBarrage.beginLoopSoundString, base.gameObject);
			this.huntressTracker = base.GetComponent<HuntressTracker>();
			if (this.huntressTracker)
			{
				this.huntressTracker.enabled = false;
			}
		}

		// Token: 0x06002F61 RID: 12129 RVA: 0x000CA390 File Offset: 0x000C8590
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (base.isAuthority && base.inputBank)
			{
				if (base.skillLocator && base.skillLocator.utility.IsReady() && base.inputBank.skill3.justPressed)
				{
					this.outer.SetNextStateToMain();
					return;
				}
				if (base.fixedAge >= this.maxDuration || base.inputBank.skill1.justPressed || base.inputBank.skill4.justPressed)
				{
					this.HandlePrimaryAttack();
				}
			}
		}

		// Token: 0x06002F62 RID: 12130 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void HandlePrimaryAttack()
		{
		}

		// Token: 0x06002F63 RID: 12131 RVA: 0x000CA450 File Offset: 0x000C8650
		public override void OnExit()
		{
			base.PlayAnimation("FullBody, Override", "FireArrowRain");
			Util.PlaySound(BaseArrowBarrage.endLoopSoundString, base.gameObject);
			Util.PlaySound(BaseArrowBarrage.fireSoundString, base.gameObject);
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			if (this.huntressTracker)
			{
				this.huntressTracker.enabled = true;
			}
			base.OnExit();
		}

		// Token: 0x04002CE1 RID: 11489
		[SerializeField]
		public float maxDuration;

		// Token: 0x04002CE2 RID: 11490
		public static string beginLoopSoundString;

		// Token: 0x04002CE3 RID: 11491
		public static string endLoopSoundString;

		// Token: 0x04002CE4 RID: 11492
		public static string fireSoundString;

		// Token: 0x04002CE5 RID: 11493
		private HuntressTracker huntressTracker;
	}
}
