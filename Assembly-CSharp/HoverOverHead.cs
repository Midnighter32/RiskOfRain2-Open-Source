using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class HoverOverHead : MonoBehaviour
{
	// Token: 0x0600011B RID: 283 RVA: 0x00007312 File Offset: 0x00005512
	private void Start()
	{
		this.parentTransform = base.transform.parent;
		this.bodyCollider = base.transform.parent.GetComponent<Collider>();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000733C File Offset: 0x0000553C
	private void Update()
	{
		Vector3 a = this.parentTransform.position;
		if (this.bodyCollider)
		{
			a = this.bodyCollider.bounds.center + new Vector3(0f, this.bodyCollider.bounds.extents.y, 0f);
		}
		base.transform.position = a + this.bonusOffset;
	}

	// Token: 0x0400012F RID: 303
	private Transform parentTransform;

	// Token: 0x04000130 RID: 304
	private Collider bodyCollider;

	// Token: 0x04000131 RID: 305
	public Vector3 bonusOffset;
}
