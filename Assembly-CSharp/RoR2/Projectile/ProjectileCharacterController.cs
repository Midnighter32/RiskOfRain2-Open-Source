using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004FD RID: 1277
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileCharacterController : MonoBehaviour
	{
		// Token: 0x06001E48 RID: 7752 RVA: 0x00082B50 File Offset: 0x00080D50
		private void Awake()
		{
			this.downVector = Vector3.down * 3f;
			this.projectileController = base.GetComponent<ProjectileController>();
			this.characterController = base.GetComponent<CharacterController>();
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x00082B80 File Offset: 0x00080D80
		private void FixedUpdate()
		{
			if (NetworkServer.active || this.projectileController.isPrediction)
			{
				this.characterController.Move((base.transform.forward + this.downVector) * (this.velocity * Time.fixedDeltaTime));
			}
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer > this.lifetime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04001B92 RID: 7058
		private Vector3 downVector;

		// Token: 0x04001B93 RID: 7059
		public float velocity;

		// Token: 0x04001B94 RID: 7060
		public float lifetime = 5f;

		// Token: 0x04001B95 RID: 7061
		private float timer;

		// Token: 0x04001B96 RID: 7062
		private ProjectileController projectileController;

		// Token: 0x04001B97 RID: 7063
		private CharacterController characterController;
	}
}
