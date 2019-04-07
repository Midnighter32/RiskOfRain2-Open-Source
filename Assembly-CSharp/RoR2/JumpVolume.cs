using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000346 RID: 838
	public class JumpVolume : MonoBehaviour
	{
		// Token: 0x06001167 RID: 4455 RVA: 0x00056814 File Offset: 0x00054A14
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

		// Token: 0x06001168 RID: 4456 RVA: 0x00056870 File Offset: 0x00054A70
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

		// Token: 0x0400156F RID: 5487
		public Transform targetElevationTransform;

		// Token: 0x04001570 RID: 5488
		public Vector3 jumpVelocity;

		// Token: 0x04001571 RID: 5489
		public float time;

		// Token: 0x04001572 RID: 5490
		public string jumpSoundString;
	}
}
