using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando
{
	// Token: 0x020008AF RID: 2223
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x060031D5 RID: 12757 RVA: 0x000D6A8C File Offset: 0x000D4C8C
		public override void OnEnter()
		{
			base.OnEnter();
			Vector3 vector = Vector3.up * 3f;
			if (base.characterMotor)
			{
				vector += base.characterMotor.velocity;
				base.characterMotor.enabled = false;
			}
			if (base.cachedModelTransform)
			{
				RagdollController component = base.cachedModelTransform.GetComponent<RagdollController>();
				if (component)
				{
					component.BeginRagdoll(vector);
				}
			}
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x060031D7 RID: 12759 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldAutoDestroy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x000D6B02 File Offset: 0x000D4D02
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 4f)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04003061 RID: 12385
		private Vector3 previousPosition;

		// Token: 0x04003062 RID: 12386
		private float upSpeedVelocity;

		// Token: 0x04003063 RID: 12387
		private float upSpeed;

		// Token: 0x04003064 RID: 12388
		private Animator modelAnimator;
	}
}
