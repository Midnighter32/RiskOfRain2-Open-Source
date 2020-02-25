using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
[Serializable]
public struct CubicBezier3
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600003D RID: 61 RVA: 0x00003CED File Offset: 0x00001EED
	public Vector3 p0
	{
		get
		{
			return this.a;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600003E RID: 62 RVA: 0x00003CF5 File Offset: 0x00001EF5
	public Vector3 p1
	{
		get
		{
			return this.d;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600003F RID: 63 RVA: 0x00003CFD File Offset: 0x00001EFD
	public Vector3 v0
	{
		get
		{
			return this.b - this.a;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000040 RID: 64 RVA: 0x00003D10 File Offset: 0x00001F10
	public Vector3 v1
	{
		get
		{
			return this.c - this.d;
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00003D24 File Offset: 0x00001F24
	public static CubicBezier3 FromVelocities(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1)
	{
		return new CubicBezier3
		{
			a = p0,
			b = p0 + v0,
			c = p1 + v1,
			d = p1
		};
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003D68 File Offset: 0x00001F68
	public Vector3 Evaluate(float t)
	{
		return this.a + 3f * t * (this.b - this.a) + 3f * t * t * (this.c - 2f * this.b + this.a) + t * t * t * (this.d - 3f * this.c + 3f * this.b - this.a);
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003E20 File Offset: 0x00002020
	public float ApproximateLength(int samples)
	{
		float num = 1f / (float)(samples - 1);
		float num2 = 0f;
		for (int i = 1; i < samples; i++)
		{
			Vector3 vector = this.Evaluate((float)(i - 1) * num);
			Vector3 vector2 = this.Evaluate((float)i * num);
			num2 += Vector3.Distance(vector, vector2);
		}
		return num2;
	}

	// Token: 0x0400005F RID: 95
	public Vector3 a;

	// Token: 0x04000060 RID: 96
	public Vector3 b;

	// Token: 0x04000061 RID: 97
	public Vector3 c;

	// Token: 0x04000062 RID: 98
	public Vector3 d;
}
