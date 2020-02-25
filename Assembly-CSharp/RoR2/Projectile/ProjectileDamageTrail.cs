using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000500 RID: 1280
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileDamageTrail : MonoBehaviour
	{
		// Token: 0x06001E67 RID: 7783 RVA: 0x00083166 File Offset: 0x00081366
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x00083180 File Offset: 0x00081380
		private void FixedUpdate()
		{
			if (!this.currentTrailObject)
			{
				this.currentTrailObject = UnityEngine.Object.Instantiate<GameObject>(this.trailPrefab, base.transform.position, base.transform.rotation);
				DamageTrail component = this.currentTrailObject.GetComponent<DamageTrail>();
				component.damagePerSecond = this.projectileDamage.damage * this.damageToTrailDpsFactor;
				component.owner = this.projectileController.owner;
				return;
			}
			this.currentTrailObject.transform.position = base.transform.position;
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x00083210 File Offset: 0x00081410
		private void OnDestroy()
		{
			this.DiscontinueTrail();
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x00083218 File Offset: 0x00081418
		private void DiscontinueTrail()
		{
			if (this.currentTrailObject)
			{
				this.currentTrailObject.AddComponent<DestroyOnTimer>().duration = this.trailLifetimeAfterExpiration;
				this.currentTrailObject.GetComponent<DamageTrail>().active = false;
				this.currentTrailObject = null;
			}
		}

		// Token: 0x04001BAE RID: 7086
		public GameObject trailPrefab;

		// Token: 0x04001BAF RID: 7087
		public float damageToTrailDpsFactor = 1f;

		// Token: 0x04001BB0 RID: 7088
		public float trailLifetimeAfterExpiration = 1f;

		// Token: 0x04001BB1 RID: 7089
		private ProjectileController projectileController;

		// Token: 0x04001BB2 RID: 7090
		private ProjectileDamage projectileDamage;

		// Token: 0x04001BB3 RID: 7091
		private GameObject currentTrailObject;
	}
}
