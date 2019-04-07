using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
[ExecuteInEditMode]
[RequireComponent(typeof(Animator))]
public class ForcePose : MonoBehaviour
{
	// Token: 0x06000115 RID: 277 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000072AB File Offset: 0x000054AB
	private void Update()
	{
		if (this.clip)
		{
			this.clip.SampleAnimation(base.gameObject, this.cycle * this.clip.length);
		}
	}

	// Token: 0x04000127 RID: 295
	[Tooltip("The animation clip to force.")]
	public AnimationClip clip;

	// Token: 0x04000128 RID: 296
	[Range(0f, 1f)]
	[Tooltip("The moment in the cycle to force.")]
	public float cycle;
}
