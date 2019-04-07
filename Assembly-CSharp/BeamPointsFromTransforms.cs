using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
[ExecuteAlways]
public class BeamPointsFromTransforms : MonoBehaviour
{
	// Token: 0x060000D5 RID: 213 RVA: 0x000054F4 File Offset: 0x000036F4
	private void Start()
	{
		this.UpdateBeamPositions();
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x000054F4 File Offset: 0x000036F4
	private void Update()
	{
		this.UpdateBeamPositions();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x000054FC File Offset: 0x000036FC
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

	// Token: 0x040000C1 RID: 193
	[Tooltip("Line Renderer to set the positions of.")]
	public LineRenderer target;

	// Token: 0x040000C2 RID: 194
	[Tooltip("Transforms to use as the points for the line renderer.")]
	[SerializeField]
	private Transform[] pointTransforms;
}
