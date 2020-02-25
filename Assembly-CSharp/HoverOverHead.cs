using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class HoverOverHead : MonoBehaviour
{
	// Token: 0x060000FF RID: 255 RVA: 0x0000726A File Offset: 0x0000546A
	private void Start()
	{
		this.parentTransform = base.transform.parent;
		this.bodyCollider = base.transform.parent.GetComponent<Collider>();
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00007294 File Offset: 0x00005494
	private void Update()
	{
		Vector3 a = this.parentTransform.position;
		if (this.bodyCollider)
		{
			a = this.bodyCollider.bounds.center + new Vector3(0f, this.bodyCollider.bounds.extents.y, 0f);
		}
		base.transform.position = a + this.bonusOffset;
	}

	// Token: 0x04000134 RID: 308
	private Transform parentTransform;

	// Token: 0x04000135 RID: 309
	private Collider bodyCollider;

	// Token: 0x04000136 RID: 310
	public Vector3 bonusOffset;
}
