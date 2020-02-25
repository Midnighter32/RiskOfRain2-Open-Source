using System;

// Token: 0x02000024 RID: 36
public class DoXInYSecondsTracker
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000097 RID: 151 RVA: 0x00005026 File Offset: 0x00003226
	private float newestTime
	{
		get
		{
			return this.timestamps[0];
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000098 RID: 152 RVA: 0x00005030 File Offset: 0x00003230
	private int requirement
	{
		get
		{
			return this.timestamps.Length;
		}
	}

	// Token: 0x06000099 RID: 153 RVA: 0x0000503A File Offset: 0x0000323A
	public DoXInYSecondsTracker(int requirement, float window)
	{
		if (requirement < 1)
		{
			throw new ArgumentException("Argument must be greater than zero", "requirement");
		}
		this.timestamps = new float[requirement];
		this.Clear();
		this.window = window;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005070 File Offset: 0x00003270
	public void Clear()
	{
		for (int i = 0; i < this.timestamps.Length; i++)
		{
			this.timestamps[i] = float.NegativeInfinity;
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000050A0 File Offset: 0x000032A0
	private int FindInsertionPosition(float t)
	{
		for (int i = 0; i < this.lastValidCount; i++)
		{
			if (this.timestamps[i] < t)
			{
				return i;
			}
		}
		return this.lastValidCount;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000050D4 File Offset: 0x000032D4
	public bool Push(float t)
	{
		float num = t - this.window;
		if (t < this.newestTime)
		{
			this.lastValidCount = this.timestamps.Length;
		}
		int num2 = this.lastValidCount - 1;
		while (num2 >= 0 && num > this.timestamps[num2])
		{
			this.lastValidCount--;
			num2--;
		}
		int num3 = this.FindInsertionPosition(t);
		if (num3 < this.timestamps.Length)
		{
			this.lastValidCount++;
			HGArrayUtilities.ArrayInsertNoResize<float>(this.timestamps, this.lastValidCount, num3, ref t);
		}
		return this.lastValidCount == this.requirement;
	}

	// Token: 0x040000AF RID: 175
	private readonly float[] timestamps;

	// Token: 0x040000B0 RID: 176
	private readonly float window;

	// Token: 0x040000B1 RID: 177
	private int lastValidCount;
}
