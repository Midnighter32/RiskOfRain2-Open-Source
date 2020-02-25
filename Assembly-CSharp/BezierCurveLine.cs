using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class BezierCurveLine : MonoBehaviour
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060000BD RID: 189 RVA: 0x000054DB File Offset: 0x000036DB
	// (set) Token: 0x060000BE RID: 190 RVA: 0x000054E3 File Offset: 0x000036E3
	public LineRenderer lineRenderer { get; private set; }

	// Token: 0x060000BF RID: 191 RVA: 0x000054EC File Offset: 0x000036EC
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.windPhaseShift = UnityEngine.Random.insideUnitSphere * 360f;
		Array.Resize<Vector3>(ref this.vertexList, this.lineRenderer.positionCount + 1);
		this.UpdateBezier(0f);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x0000553D File Offset: 0x0000373D
	public void OnEnable()
	{
		Array.Resize<Vector3>(ref this.vertexList, this.lineRenderer.positionCount + 1);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00005557 File Offset: 0x00003757
	private void LateUpdate()
	{
		this.UpdateBezier(Time.deltaTime);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00005564 File Offset: 0x00003764
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

	// Token: 0x060000C3 RID: 195 RVA: 0x000057CC File Offset: 0x000039CC
	private Vector3 EvaluateBezier(float t)
	{
		Vector3 a = Vector3.Lerp(this.p0, this.p0 + this.finalv0, t);
		Vector3 b = Vector3.Lerp(this.p1, this.p1 + this.finalv1, 1f - t);
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x040000C9 RID: 201
	private Vector3[] vertexList = Array.Empty<Vector3>();

	// Token: 0x040000CA RID: 202
	private Vector3 p0 = Vector3.zero;

	// Token: 0x040000CB RID: 203
	public Vector3 v0 = Vector3.zero;

	// Token: 0x040000CC RID: 204
	public Vector3 p1 = Vector3.zero;

	// Token: 0x040000CD RID: 205
	public Vector3 v1 = Vector3.zero;

	// Token: 0x040000CE RID: 206
	public Transform endTransform;

	// Token: 0x040000CF RID: 207
	public bool animateBezierWind;

	// Token: 0x040000D0 RID: 208
	public Vector3 windMagnitude;

	// Token: 0x040000D1 RID: 209
	public Vector3 windFrequency;

	// Token: 0x040000D2 RID: 210
	private Vector3 windPhaseShift;

	// Token: 0x040000D3 RID: 211
	private Vector3 lastWind;

	// Token: 0x040000D4 RID: 212
	private Vector3 finalv0;

	// Token: 0x040000D5 RID: 213
	private Vector3 finalv1;

	// Token: 0x040000D6 RID: 214
	private float windTime;
}
