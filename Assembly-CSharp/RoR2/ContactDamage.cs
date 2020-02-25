using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001C7 RID: 455
	[RequireComponent(typeof(CharacterBody))]
	public class ContactDamage : MonoBehaviour
	{
		// Token: 0x060009C6 RID: 2502 RVA: 0x0002AAAC File Offset: 0x00028CAC
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0002AABC File Offset: 0x00028CBC
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f)
				{
					this.overlapAttack = new OverlapAttack
					{
						attacker = base.gameObject,
						inflictor = base.gameObject,
						hitBoxGroup = this.hitBoxGroup,
						teamIndex = TeamComponent.GetObjectTeam(base.gameObject)
					};
					this.refreshTimer = this.damageInterval;
				}
				this.overlapAttack.damage = this.characterBody.damage * this.damagePerSecondCoefficient * this.damageInterval;
				this.overlapAttack.pushAwayForce = this.pushForcePerSecond * this.damageInterval;
				this.overlapAttack.damageType = this.damageType;
				this.overlapAttack.Fire(null);
			}
		}

		// Token: 0x040009F3 RID: 2547
		public float damagePerSecondCoefficient = 2f;

		// Token: 0x040009F4 RID: 2548
		public float damageInterval = 0.25f;

		// Token: 0x040009F5 RID: 2549
		public float pushForcePerSecond = 4000f;

		// Token: 0x040009F6 RID: 2550
		public HitBoxGroup hitBoxGroup;

		// Token: 0x040009F7 RID: 2551
		public DamageType damageType;

		// Token: 0x040009F8 RID: 2552
		private OverlapAttack overlapAttack;

		// Token: 0x040009F9 RID: 2553
		private CharacterBody characterBody;

		// Token: 0x040009FA RID: 2554
		private float refreshTimer;
	}
}
