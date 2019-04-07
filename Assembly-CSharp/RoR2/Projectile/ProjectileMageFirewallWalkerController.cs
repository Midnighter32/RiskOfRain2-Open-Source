using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000553 RID: 1363
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileMageFirewallWalkerController : MonoBehaviour
	{
		// Token: 0x06001E57 RID: 7767 RVA: 0x0008F1C8 File Offset: 0x0008D3C8
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lastCenterPosition = base.transform.position;
			this.timer = this.dropInterval / 2f;
			this.moveSign = 1f;
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x0008F21C File Offset: 0x0008D41C
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

		// Token: 0x06001E59 RID: 7769 RVA: 0x0008F2AC File Offset: 0x0008D4AC
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

		// Token: 0x06001E5A RID: 7770 RVA: 0x0008F348 File Offset: 0x0008D548
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

		// Token: 0x040020F6 RID: 8438
		public float dropInterval = 0.15f;

		// Token: 0x040020F7 RID: 8439
		public GameObject firePillarPrefab;

		// Token: 0x040020F8 RID: 8440
		public float pillarAngle = 45f;

		// Token: 0x040020F9 RID: 8441
		public bool curveToCenter = true;

		// Token: 0x040020FA RID: 8442
		private float moveSign;

		// Token: 0x040020FB RID: 8443
		private ProjectileController projectileController;

		// Token: 0x040020FC RID: 8444
		private ProjectileDamage projectileDamage;

		// Token: 0x040020FD RID: 8445
		private Vector3 lastCenterPosition;

		// Token: 0x040020FE RID: 8446
		private float timer;

		// Token: 0x040020FF RID: 8447
		private Vector3 currentPillarVector = Vector3.up;
	}
}
