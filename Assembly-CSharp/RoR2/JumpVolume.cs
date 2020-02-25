using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026C RID: 620
	public class JumpVolume : MonoBehaviour
	{
		// Token: 0x06000DC7 RID: 3527 RVA: 0x0003DE58 File Offset: 0x0003C058
		public void OnTriggerStay(Collider other)
		{
			CharacterMotor component = other.GetComponent<CharacterMotor>();
			if (component && component.hasEffectiveAuthority)
			{
				if (!component.disableAirControlUntilCollision)
				{
					Util.PlaySound(this.jumpSoundString, base.gameObject);
				}
				component.velocity = this.jumpVelocity;
				component.disableAirControlUntilCollision = true;
				component.Motor.ForceUnground();
			}
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x0003DEB4 File Offset: 0x0003C0B4
		private void OnDrawGizmos()
		{
			int num = 20;
			float d = this.time / (float)num;
			Vector3 vector = base.transform.position;
			Vector3 position = base.transform.position;
			Vector3 a = this.jumpVelocity;
			Gizmos.color = Color.yellow;
			for (int i = 0; i <= num; i++)
			{
				Vector3 vector2 = vector + a * d;
				a += Physics.gravity * d;
				Gizmos.DrawLine(vector2, vector);
				vector = vector2;
			}
		}

		// Token: 0x04000DCB RID: 3531
		public Transform targetElevationTransform;

		// Token: 0x04000DCC RID: 3532
		public Vector3 jumpVelocity;

		// Token: 0x04000DCD RID: 3533
		public float time;

		// Token: 0x04000DCE RID: 3534
		public string jumpSoundString;
	}
}
