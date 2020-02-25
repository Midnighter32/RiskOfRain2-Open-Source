using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
[RequireComponent(typeof(Animator))]
public class RootMotionAccumulator : MonoBehaviour
{
	// Token: 0x06000136 RID: 310 RVA: 0x00007D54 File Offset: 0x00005F54
	public Vector3 ExtractRootMotion()
	{
		Vector3 result = this.accumulatedRootMotion;
		this.accumulatedRootMotion = Vector3.zero;
		return result;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007D67 File Offset: 0x00005F67
	public Quaternion ExtractRootRotation()
	{
		Quaternion result = this.accumulatedRootRotation;
		this.accumulatedRootRotation = Quaternion.identity;
		return result;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007D7A File Offset: 0x00005F7A
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.accumulatedRootRotation = Quaternion.identity;
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00007D94 File Offset: 0x00005F94
	private void OnAnimatorMove()
	{
		this.accumulatedRootMotion += this.animator.deltaPosition;
		if (this.accumulateRotation)
		{
			this.accumulatedRootRotation *= this.animator.deltaRotation;
		}
	}

	// Token: 0x04000167 RID: 359
	private Animator animator;

	// Token: 0x04000168 RID: 360
	[NonSerialized]
	public Vector3 accumulatedRootMotion;

	// Token: 0x04000169 RID: 361
	public Quaternion accumulatedRootRotation;

	// Token: 0x0400016A RID: 362
	public bool accumulateRotation;
}
