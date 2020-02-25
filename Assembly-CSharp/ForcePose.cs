using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class ForcePose : MonoBehaviour
{
	// Token: 0x060000F9 RID: 249 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00007203 File Offset: 0x00005403
	private void Update()
	{
		if (this.clip)
		{
			this.clip.SampleAnimation(base.gameObject, this.cycle * this.clip.length);
		}
	}

	// Token: 0x0400012C RID: 300
	[Tooltip("The animation clip to force.")]
	public AnimationClip clip;

	// Token: 0x0400012D RID: 301
	[Tooltip("The moment in the cycle to force.")]
	[Range(0f, 1f)]
	public float cycle;
}
