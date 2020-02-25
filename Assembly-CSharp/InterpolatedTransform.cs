using System;
using RoR2;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200003D RID: 61
[RequireComponent(typeof(InterpolatedTransformUpdater))]
public class InterpolatedTransform : MonoBehaviour, ITeleportHandler, IEventSystemHandler
{
	// Token: 0x06000102 RID: 258 RVA: 0x00007311 File Offset: 0x00005511
	private void OnEnable()
	{
		this.ForgetPreviousTransforms();
	}

	// Token: 0x06000103 RID: 259 RVA: 0x0000731C File Offset: 0x0000551C
	public void ForgetPreviousTransforms()
	{
		this.m_lastTransforms = new InterpolatedTransform.TransformData[2];
		InterpolatedTransform.TransformData transformData = new InterpolatedTransform.TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
		this.m_lastTransforms[0] = transformData;
		this.m_lastTransforms[1] = transformData;
		this.m_newTransformIndex = 0;
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00007380 File Offset: 0x00005580
	private void FixedUpdate()
	{
		InterpolatedTransform.TransformData transformData = this.m_lastTransforms[this.m_newTransformIndex];
		base.transform.localPosition = transformData.position;
		base.transform.localRotation = transformData.rotation;
		base.transform.localScale = transformData.scale;
	}

	// Token: 0x06000105 RID: 261 RVA: 0x000073D4 File Offset: 0x000055D4
	public void LateFixedUpdate()
	{
		this.m_newTransformIndex = this.OldTransformIndex();
		this.m_lastTransforms[this.m_newTransformIndex] = new InterpolatedTransform.TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00007424 File Offset: 0x00005624
	private void Update()
	{
		InterpolatedTransform.TransformData transformData = this.m_lastTransforms[this.m_newTransformIndex];
		InterpolatedTransform.TransformData transformData2 = this.m_lastTransforms[this.OldTransformIndex()];
		base.transform.localPosition = Vector3.Lerp(transformData2.position, transformData.position, InterpolationController.InterpolationFactor);
		base.transform.localRotation = Quaternion.Slerp(transformData2.rotation, transformData.rotation, InterpolationController.InterpolationFactor);
		base.transform.localScale = Vector3.Lerp(transformData2.scale, transformData.scale, InterpolationController.InterpolationFactor);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x000074B8 File Offset: 0x000056B8
	private int OldTransformIndex()
	{
		if (this.m_newTransformIndex != 0)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00007311 File Offset: 0x00005511
	public void OnTeleport(Vector3 oldPosition, Vector3 newPosition)
	{
		this.ForgetPreviousTransforms();
	}

	// Token: 0x04000137 RID: 311
	private InterpolatedTransform.TransformData[] m_lastTransforms;

	// Token: 0x04000138 RID: 312
	private int m_newTransformIndex;

	// Token: 0x0200003E RID: 62
	private struct TransformData
	{
		// Token: 0x0600010A RID: 266 RVA: 0x000074C5 File Offset: 0x000056C5
		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		// Token: 0x04000139 RID: 313
		public Vector3 position;

		// Token: 0x0400013A RID: 314
		public Quaternion rotation;

		// Token: 0x0400013B RID: 315
		public Vector3 scale;
	}
}
