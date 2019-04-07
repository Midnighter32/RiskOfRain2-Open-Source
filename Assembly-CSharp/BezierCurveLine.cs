using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class BezierCurveLine : MonoBehaviour
{
	// Token: 0x060000D9 RID: 217 RVA: 0x0000555C File Offset: 0x0000375C
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.windPhaseShift = UnityEngine.Random.insideUnitSphere * 360f;
		this.vertexList = new Vector3[this.lineRenderer.positionCount + 1];
		this.UpdateBezier(0f);
	}

	// Token: 0x060000DA RID: 218 RVA: 0x000055AD File Offset: 0x000037AD
	public void OnEnable()
	{
		this.vertexList = new Vector3[this.lineRenderer.positionCount + 1];
	}

	// Token: 0x060000DB RID: 219 RVA: 0x000055C7 File Offset: 0x000037C7
	private void LateUpdate()
	{
		this.UpdateBezier(Time.deltaTime);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000055D4 File Offset: 0x000037D4
	public void UpdateBezier(float deltaTime)
	{
		this.windTime += deltaTime;
		this.p0 = base.transform.position;
		if (this.endTransform)
		{
			this.p1 = this.endTransform.position;
		}
		if (this.animateBezierWind)
		{
			this.finalv0 = this.v0 + new Vector3(Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.x) * this.windFrequency.x) * this.windMagnitude.x, Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.y) * this.windFrequency.y) * this.windMagnitude.y, Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.z) * this.windFrequency.z) * this.windMagnitude.z);
			this.finalv1 = this.v1 + new Vector3(Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.x + this.p1.x) * this.windFrequency.x) * this.windMagnitude.x, Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.y + this.p1.z) * this.windFrequency.y) * this.windMagnitude.y, Mathf.Sin(0.017453292f * (this.windTime * 360f + this.windPhaseShift.z + this.p1.y) * this.windFrequency.z) * this.windMagnitude.z);
		}
		else
		{
			this.finalv0 = this.v0;
			this.finalv1 = this.v1;
		}
		for (int i = 0; i < this.vertexList.Length; i++)
		{
			float t = (float)i / (float)(this.vertexList.Length - 2);
			this.vertexList[i] = this.EvaluateBezier(t);
		}
		this.lineRenderer.SetPositions(this.vertexList);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000583C File Offset: 0x00003A3C
	private Vector3 EvaluateBezier(float t)
	{
		Vector3 a = Vector3.Lerp(this.p0, this.p0 + this.finalv0, t);
		Vector3 b = Vector3.Lerp(this.p1, this.p1 + this.finalv1, 1f - t);
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x040000C3 RID: 195
	private Vector3[] vertexList;

	// Token: 0x040000C4 RID: 196
	private Vector3 p0 = Vector3.zero;

	// Token: 0x040000C5 RID: 197
	public Vector3 v0 = Vector3.zero;

	// Token: 0x040000C6 RID: 198
	public Vector3 p1 = Vector3.zero;

	// Token: 0x040000C7 RID: 199
	public Vector3 v1 = Vector3.zero;

	// Token: 0x040000C8 RID: 200
	public Transform endTransform;

	// Token: 0x040000C9 RID: 201
	public bool animateBezierWind;

	// Token: 0x040000CA RID: 202
	public Vector3 windMagnitude;

	// Token: 0x040000CB RID: 203
	public Vector3 windFrequency;

	// Token: 0x040000CC RID: 204
	private Vector3 windPhaseShift;

	// Token: 0x040000CD RID: 205
	private Vector3 lastWind;

	// Token: 0x040000CE RID: 206
	private Vector3 finalv0;

	// Token: 0x040000CF RID: 207
	private Vector3 finalv1;

	// Token: 0x040000D0 RID: 208
	private float windTime;

	// Token: 0x040000D1 RID: 209
	[HideInInspector]
	public LineRenderer lineRenderer;
}
