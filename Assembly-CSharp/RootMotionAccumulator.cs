using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
[RequireComponent(typeof(Animator))]
public class RootMotionAccumulator : MonoBehaviour
{
	// Token: 0x06000151 RID: 337 RVA: 0x00007E40 File Offset: 0x00006040
	public Vector3 ExtractRootMotion()
	{
		Vector3 result = this.accumulatedRootMotion;
		this.accumulatedRootMotion = Vector3.zero;
		return result;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00007E53 File Offset: 0x00006053
	public Quaternion ExtractRootRotation()
	{
		Quaternion result = this.accumulatedRootRotation;
		this.accumulatedRootRotation = Quaternion.identity;
		return result;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00007E66 File Offset: 0x00006066
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.accumulatedRootRotation = Quaternion.identity;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00007E80 File Offset: 0x00006080
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
