using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200051A RID: 1306
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileMageFirewallWalkerController : MonoBehaviour
	{
		// Token: 0x06001ECF RID: 7887 RVA: 0x000856A8 File Offset: 0x000838A8
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lastCenterPosition = base.transform.position;
			this.timer = this.dropInterval / 2f;
			this.moveSign = 1f;
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x000856FC File Offset: 0x000838FC
		private void Start()
		{
			if (this.projectileController.owner)
			{
				Vector3 position = this.projectileController.owner.transform.position;
				Vector3 vector = base.transform.position - position;
				vector.y = 0f;
				if (vector.x != 0f && vector.z != 0f)
				{
					this.moveSign = Mathf.Sign(Vector3.Dot(base.transform.right, vector));
				}
			}
			this.UpdateDirections();
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x0008578C File Offset: 0x0008398C
		private void UpdateDirections()
		{
			if (!this.curveToCenter)
			{
				return;
			}
			Vector3 vector = base.transform.position - this.lastCenterPosition;
			vector.y = 0f;
			if (vector.x != 0f && vector.z != 0f)
			{
				vector.Normalize();
				Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
				base.transform.forward = vector2 * this.moveSign;
				this.currentPillarVector = Quaternion.AngleAxis(this.pillarAngle, vector2) * Vector3.Cross(vector, vector2);
			}
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x00085828 File Offset: 0x00083A28
		private void FixedUpdate()
		{
			if (this.projectileController.owner)
			{
				this.lastCenterPosition = this.projectileController.owner.transform.position;
			}
			this.UpdateDirections();
			if (NetworkServer.active)
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					this.timer = this.dropInterval;
					if (this.firePillarPrefab)
					{
						ProjectileManager.instance.FireProjectile(this.firePillarPrefab, base.transform.position, Util.QuaternionSafeLookRotation(this.currentPillarVector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
					}
				}
			}
		}

		// Token: 0x04001C5B RID: 7259
		public float dropInterval = 0.15f;

		// Token: 0x04001C5C RID: 7260
		public GameObject firePillarPrefab;

		// Token: 0x04001C5D RID: 7261
		public float pillarAngle = 45f;

		// Token: 0x04001C5E RID: 7262
		public bool curveToCenter = true;

		// Token: 0x04001C5F RID: 7263
		private float moveSign;

		// Token: 0x04001C60 RID: 7264
		private ProjectileController projectileController;

		// Token: 0x04001C61 RID: 7265
		private ProjectileDamage projectileDamage;

		// Token: 0x04001C62 RID: 7266
		private Vector3 lastCenterPosition;

		// Token: 0x04001C63 RID: 7267
		private float timer;

		// Token: 0x04001C64 RID: 7268
		private Vector3 currentPillarVector = Vector3.up;
	}
}
