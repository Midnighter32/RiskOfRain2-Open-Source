using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002DE RID: 734
	public class RagdollController : MonoBehaviour
	{
		// Token: 0x060010D9 RID: 4313 RVA: 0x00049C68 File Offset: 0x00047E68
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			foreach (Transform transform in this.bones)
			{
				Collider component = transform.GetComponent<Collider>();
				Rigidbody component2 = transform.GetComponent<Rigidbody>();
				if (!component)
				{
					Debug.LogWarningFormat("Bone {0} is missing a collider!", new object[]
					{
						transform
					});
				}
				else
				{
					component.enabled = false;
					component2.interpolation = RigidbodyInterpolation.None;
					component2.isKinematic = true;
				}
			}
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00049CE0 File Offset: 0x00047EE0
		public void BeginRagdoll(Vector3 force)
		{
			if (this.animator)
			{
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

		// Token: 0x04001026 RID: 4134
		public Transform[] bones;

		// Token: 0x04001027 RID: 4135
		public MonoBehaviour[] componentsToDisableOnRagdoll;

		// Token: 0x04001028 RID: 4136
		private Animator animator;
	}
}
