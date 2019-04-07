using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
[Serializable]
public struct CubicBezier3
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000066 RID: 102 RVA: 0x0000415A File Offset: 0x0000235A
	public Vector3 p0
	{
		get
		{
			return this.a;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000067 RID: 103 RVA: 0x00004162 File Offset: 0x00002362
	public Vector3 p1
	{
		get
		{
			return this.d;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000068 RID: 104 RVA: 0x0000416A File Offset: 0x0000236A
	public Vector3 v0
	{
		get
		{
			return this.b - this.a;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000069 RID: 105 RVA: 0x0000417D File Offset: 0x0000237D
	public Vector3 v1
	{
		get
		{
			return this.c - this.d;
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00004190 File Offset: 0x00002390
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

	// Token: 0x0600006B RID: 107 RVA: 0x000041D4 File Offset: 0x000023D4
	public Vector3 Evaluate(float t)
	{
		return this.a + 3f * t * (this.b - this.a) + 3f * t * t * (this.c - 2f * this.b + this.a) + t * t * t * (this.d - 3f * this.c + 3f * this.b - this.a);
	}

	// Token: 0x0600006C RID: 108 RVA: 0x0000428C File Offset: 0x0000248C
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

	// Token: 0x04000064 RID: 100
	public Vector3 a;

	// Token: 0x04000065 RID: 101
	public Vector3 b;

	// Token: 0x04000066 RID: 102
	public Vector3 c;

	// Token: 0x04000067 RID: 103
	public Vector3 d;
}
