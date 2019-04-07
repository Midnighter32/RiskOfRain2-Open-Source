using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A1 RID: 929
	public class RagdollController : MonoBehaviour
	{
		// Token: 0x060013B2 RID: 5042 RVA: 0x00060290 File Offset: 0x0005E490
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			foreach (Transform transform in this.bones)
			{
				Collider component = transform.GetComponent<Collider>();
				if (!component)
				{
					Debug.LogFormat("Bone {0} is missing a collider!", new object[]
					{
						transform
					});
				}
				else
				{
					component.enabled = false;
				}
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x000602F0 File Offset: 0x0005E4F0
		public void BeginRagdoll(Vector3 force)
		{
			if (this.animator)
			{
				Debug.Log("animator disabled");
				this.animator.enabled = false;
			}
			foreach (Transform transform in this.bones)
			{
				transform.parent = base.transform;
				Rigidbody component = transform.GetComponent<Rigidbody>();
				transform.GetComponent<Collider>().enabled = true;
				component.isKinematic = false;
				component.interpolation = RigidbodyInterpolation.Interpolate;
				component.collisionDetectionMode = CollisionDetectionMode.Continuous;
				component.AddForce(force * UnityEngine.Random.Range(0.9f, 1.2f), ForceMode.VelocityChange);
			}
			MonoBehaviour[] array2 = this.componentsToDisableOnRagdoll;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
		}

		// Token: 0x04001757 RID: 5975
		public Transform[] bones;

		// Token: 0x04001758 RID: 5976
		public MonoBehaviour[] componentsToDisableOnRagdoll;

		// Token: 0x04001759 RID: 5977
		private Animator animator;
	}
}
