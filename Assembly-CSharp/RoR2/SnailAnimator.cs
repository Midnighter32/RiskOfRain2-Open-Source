using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E8 RID: 1000
	public class SnailAnimator : MonoBehaviour
	{
		// Token: 0x060015C8 RID: 5576 RVA: 0x00068763 File Offset: 0x00066963
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.characterModel = base.GetComponentInParent<CharacterModel>();
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x00068780 File Offset: 0x00066980
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

		// Token: 0x04001931 RID: 6449
		public ParticleSystem healEffectSystem;

		// Token: 0x04001932 RID: 6450
		private bool lastOutOfDanger;

		// Token: 0x04001933 RID: 6451
		private Animator animator;

		// Token: 0x04001934 RID: 6452
		private CharacterModel characterModel;
	}
}
