using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000540 RID: 1344
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileCharacterControllerTrailOnGround : MonoBehaviour
	{
		// Token: 0x06001E01 RID: 7681 RVA: 0x0008D63D File Offset: 0x0008B83D
		private void Awake()
		{
			this.characterController = base.GetComponent<CharacterController>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x0008D664 File Offset: 0x0008B864
		private void FixedUpdate()
		{
			if (this.characterController.isGrounded)
			{
				if (!this.currentTrailObject)
				{
					this.currentTrailObject = UnityEngine.Object.Instantiate<GameObject>(this.trailPrefab, base.transform.position, base.transform.rotation);
					DamageTrail component = this.currentTrailObject.GetComponent<DamageTrail>();
					component.damagePerSecond = this.projectileDamage.damage * this.damageToTrailDpsFactor;
					component.owner = this.projectileController.owner;
				}
				this.currentTrailObject.transform.position = base.transform.position;
				return;
			}
			this.DiscontinueTrail();
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x0008D70A File Offset: 0x0008B90A
		private void OnDestroy()
		{
			this.DiscontinueTrail();
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x0008D712 File Offset: 0x0008B912
		private void DiscontinueTrail()
		{
			if (this.currentTrailObject)
			{
				this.currentTrailObject.AddComponent<DestroyOnTimer>().duration = 1f;
				this.currentTrailObject = null;
			}
		}

		// Token: 0x0400206C RID: 8300
		public GameObject trailPrefab;

		// Token: 0x0400206D RID: 8301
		public float damageToTrailDpsFactor = 1f;

		// Token: 0x0400206E RID: 8302
		private CharacterController characterController;

		// Token: 0x0400206F RID: 8303
		private ProjectileController projectileController;

		// Token: 0x04002070 RID: 8304
		private ProjectileDamage projectileDamage;

		// Token: 0x04002071 RID: 8305
		private GameObject currentTrailObject;
	}
}
