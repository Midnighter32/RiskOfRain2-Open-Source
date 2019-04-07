using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053F RID: 1343
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileCharacterController : MonoBehaviour
	{
		// Token: 0x06001DFE RID: 7678 RVA: 0x0008D592 File Offset: 0x0008B792
		private void Awake()
		{
			this.downVector = Vector3.down * 3f;
			this.characterController = base.GetComponent<CharacterController>();
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x0008D5B8 File Offset: 0x0008B7B8
		private void FixedUpdate()
		{
			this.characterController.Move((base.transform.forward + this.downVector) * (this.velocity * Time.fixedDeltaTime));
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer > this.lifetime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04002067 RID: 8295
		private Vector3 downVector;

		// Token: 0x04002068 RID: 8296
		public float velocity;

		// Token: 0x04002069 RID: 8297
		public float lifetime = 5f;

		// Token: 0x0400206A RID: 8298
		private float timer;

		// Token: 0x0400206B RID: 8299
		private CharacterController characterController;
	}
}
