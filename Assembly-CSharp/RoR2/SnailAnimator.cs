using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200033C RID: 828
	public class SnailAnimator : MonoBehaviour
	{
		// Token: 0x060013B6 RID: 5046 RVA: 0x00054563 File Offset: 0x00052763
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.characterModel = base.GetComponentInParent<CharacterModel>();
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x00054580 File Offset: 0x00052780
		private void FixedUpdate()
		{
			if (this.characterModel)
			{
				CharacterBody body = this.characterModel.body;
				if (body)
				{
					bool outOfDanger = body.outOfDanger;
					if (outOfDanger && !this.lastOutOfDanger)
					{
						this.animator.SetBool("spawn", true);
						this.animator.SetBool("hide", false);
						Util.PlaySound("Play_item_proc_slug_emerge", this.characterModel.gameObject);
						this.healEffectSystem.main.loop = true;
						this.healEffectSystem.Play();
					}
					else if (!outOfDanger && this.lastOutOfDanger)
					{
						this.animator.SetBool("hide", true);
						this.animator.SetBool("spawn", false);
						Util.PlaySound("Play_item_proc_slug_hide", this.characterModel.gameObject);
						this.healEffectSystem.main.loop = false;
					}
					this.lastOutOfDanger = outOfDanger;
				}
			}
		}

		// Token: 0x04001287 RID: 4743
		public ParticleSystem healEffectSystem;

		// Token: 0x04001288 RID: 4744
		private bool lastOutOfDanger;

		// Token: 0x04001289 RID: 4745
		private Animator animator;

		// Token: 0x0400128A RID: 4746
		private CharacterModel characterModel;
	}
}
