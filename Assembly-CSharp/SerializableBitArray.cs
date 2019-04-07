using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x0200006F RID: 111
[Serializable]
public class SerializableBitArray
{
	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060001AA RID: 426 RVA: 0x00009339 File Offset: 0x00007539
	public int byteCount
	{
		get
		{
			return this.bytes.Length;
		}
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00009343 File Offset: 0x00007543
	public SerializableBitArray(int length)
	{
		this.bytes = new byte[length + 7 >> 3];
		this.length = length;
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00009364 File Offset: 0x00007564
	public SerializableBitArray(SerializableBitArray src)
	{
		if (src.bytes != null)
		{
			this.bytes = new byte[src.bytes.Length];
			src.bytes.CopyTo(this.bytes, 0);
		}
		this.length = src.length;
	}

	// Token: 0x060001AD RID: 429 RVA: 0x000093B0 File Offset: 0x000075B0
	public byte[] GetBytes()
	{
		byte[] array = new byte[this.bytes.Length];
		this.GetBytes(array);
		return array;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x000093D3 File Offset: 0x000075D3
	public void GetBytes([NotNull] byte[] dest)
	{
		Buffer.BlockCopy(this.bytes, 0, dest, 0, this.bytes.Length);
	}

	// Token: 0x1700001B RID: 27
	public bool this[int index]
	{
		get
		{
			int num = index >> 3;
			int num2 = index & 7;
			return ((int)this.bytes[num] & 1 << num2) != 0;
		}
		set
		{
			int num = index >> 3;
			int num2 = index & 7;
			int num3 = (int)this.bytes[num];
			this.bytes[num] = (byte)(value ? (num3 | 1 << num2) : (num3 & ~(byte)(1 << num2)));
		}
	}

	// Token: 0x040001ED RID: 493
	[SerializeField]
	protected readonly byte[] bytes;

	// Token: 0x040001EE RID: 494
	[SerializeField]
	public readonly int length;

	// Token: 0x040001EF RID: 495
	private const int bitMask = 7;
}
