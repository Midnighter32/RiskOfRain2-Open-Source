using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class ScaleProjector : MonoBehaviour
{
	// Token: 0x06000148 RID: 328 RVA: 0x0000804F File Offset: 0x0000624F
	private void Start()
	{
		this.projector = base.GetComponent<Projector>();
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000805D File Offset: 0x0000625D
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
