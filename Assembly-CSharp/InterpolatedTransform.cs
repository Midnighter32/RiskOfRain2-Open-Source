using System;
using RoR2;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000041 RID: 65
[RequireComponent(typeof(InterpolatedTransformUpdater))]
public class InterpolatedTransform : MonoBehaviour, ITeleportHandler, IEventSystemHandler
{
	// Token: 0x0600011E RID: 286 RVA: 0x000073B9 File Offset: 0x000055B9
	private void OnEnable()
	{
		this.ForgetPreviousTransforms();
	}

	// Token: 0x0600011F RID: 287 RVA: 0x000073C4 File Offset: 0x000055C4
	public void ForgetPreviousTransforms()
	{
		this.m_lastTransforms = new InterpolatedTransform.TransformData[2];
		InterpolatedTransform.TransformData transformData = new InterpolatedTransform.TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
		this.m_lastTransforms[0] = transformData;
		this.m_lastTransforms[1] = transformData;
		this.m_newTransformIndex = 0;
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00007428 File Offset: 0x00005628
	private void FixedUpdate()
	{
		InterpolatedTransform.TransformData transformData = this.m_lastTransforms[this.m_newTransformIndex];
		base.transform.localPosition = transformData.position;
		base.transform.localRotation = transformData.rotation;
		base.transform.localScale = transformData.scale;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000747C File Offset: 0x0000567C
	public void LateFixedUpdate()
	{
		this.m_newTransformIndex = this.OldTransformIndex();
		this.m_lastTransforms[this.m_newTransformIndex] = new InterpolatedTransform.TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x000074CC File Offset: 0x000056CC
	private void Update()
	{
		InterpolatedTransform.TransformData transformData = this.m_lastTransforms[this.m_newTransformIndex];
		InterpolatedTransform.TransformData transformData2 = this.m_lastTransforms[this.OldTransformIndex()];
		base.transform.localPosition = Vector3.Lerp(transformData2.position, transformData.position, InterpolationController.InterpolationFactor);
		base.transform.localRotation = Quaternion.Slerp(transformData2.rotation, transformData.rotation, InterpolationController.InterpolationFactor);
		base.transform.localScale = Vector3.Lerp(transformData2.scale, transformData.scale, InterpolationController.InterpolationFactor);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00007560 File Offset: 0x00005760
	private int OldTransformIndex()
	{
		if (this.m_newTransformIndex != 0)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x000073B9 File Offset: 0x000055B9
	public void OnTeleport(Vector3 oldPosition, Vector3 newPosition)
	{
		this.ForgetPreviousTransforms();
	}

	// Token: 0x04000132 RID: 306
	private InterpolatedTransform.TransformData[] m_lastTransforms;

	// Token: 0x04000133 RID: 307
	private int m_newTransformIndex;

	// Token: 0x02000042 RID: 66
	private struct TransformData
	{
		// Token: 0x06000126 RID: 294 RVA: 0x0000756D File Offset: 0x0000576D
		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		// Token: 0x04000134 RID: 308
		public Vector3 position;

		// Token: 0x04000135 RID: 309
		public Quaternion rotation;

		// Token: 0x04000136 RID: 310
		public Vector3 scale;
	}
}
