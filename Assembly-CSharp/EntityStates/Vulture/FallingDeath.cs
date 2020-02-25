using System;
using UnityEngine;

namespace EntityStates.Vulture
{
	// Token: 0x02000737 RID: 1847
	public class FallingDeath : GenericCharacterDeath
	{
		// Token: 0x06002AEC RID: 10988 RVA: 0x000B4C1E File Offset: 0x000B2E1E
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterMotor)
			{
				base.characterMotor.velocity.y = 0f;
			}
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x000B4C48 File Offset: 0x000B2E48
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator)
			{
				this.animator.SetBool("isGrounded", base.characterMotor.isGrounded);
				return;
			}
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				int layerIndex = this.animator.GetLayerIndex("FlyOverride");
				this.animator.SetLayerWeight(layerIndex, 0f);
			}
		}

		// Token: 0x040026CB RID: 9931
		private Animator animator;
	}
}
