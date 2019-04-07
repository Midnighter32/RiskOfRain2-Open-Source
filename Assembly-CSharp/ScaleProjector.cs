using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class ScaleProjector : MonoBehaviour
{
	// Token: 0x06000163 RID: 355 RVA: 0x0000813B File Offset: 0x0000633B
	private void Start()
	{
		this.projector = base.GetComponent<Projector>();
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00008149 File Offset: 0x00006349
	private void Update()
	{
		if (this.projector)
		{
			this.projector.orthographicSize = base.transform.lossyScale.x;
		}
	}

	// Token: 0x04000177 RID: 375
	private Projector projector;
}
