using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
[ExecuteAlways]
public class BeamPointsFromTransforms : MonoBehaviour
{
	// Token: 0x060000B7 RID: 183 RVA: 0x000053D0 File Offset: 0x000035D0
	private void Start()
	{
		this.UpdateBeamPositions();
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000053D0 File Offset: 0x000035D0
	private void Update()
	{
		this.UpdateBeamPositions();
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x000053D8 File Offset: 0x000035D8
	private void UpdateBeamPositions()
	{
		if (this.target)
		{
			int num = this.pointTransforms.Length;
			this.target.positionCount = num;
			for (int i = 0; i < num; i++)
			{
				Transform transform = this.pointTransforms[i];
				if (transform)
				{
					this.target.SetPosition(i, transform.position);
				}
			}
		}
	}

	// Token: 0x040000C2 RID: 194
	[Tooltip("Line Renderer to set the positions of.")]
	public LineRenderer target;

	// Token: 0x040000C3 RID: 195
	[SerializeField]
	[Tooltip("Transforms to use as the points for the line renderer.")]
	private Transform[] pointTransforms;
}
