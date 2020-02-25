using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000073 RID: 115
[Serializable]
public class SerializableBitArray
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001D8 RID: 472 RVA: 0x00009C45 File Offset: 0x00007E45
	public int byteCount
	{
		get
		{
			return this.bytes.Length;
		}
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00009C4F File Offset: 0x00007E4F
	public SerializableBitArray(int length)
	{
		this.bytes = new byte[length + 7 >> 3];
		this.length = length;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00009C70 File Offset: 0x00007E70
	public SerializableBitArray(SerializableBitArray src)
	{
		if (src.bytes != null)
		{
			this.bytes = new byte[src.bytes.Length];
			src.bytes.CopyTo(this.bytes, 0);
		}
		this.length = src.length;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00009CBC File Offset: 0x00007EBC
	public byte[] GetBytes()
	{
		byte[] array = new byte[this.bytes.Length];
		this.GetBytes(array);
		return array;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00009CDF File Offset: 0x00007EDF
	public void GetBytes([NotNull] byte[] dest)
	{
		Buffer.BlockCopy(this.bytes, 0, dest, 0, this.bytes.Length);
	}

	// Token: 0x17000020 RID: 32
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

	// Token: 0x040001F4 RID: 500
	[SerializeField]
	protected readonly byte[] bytes;

	// Token: 0x040001F5 RID: 501
	[SerializeField]
	public readonly int length;

	// Token: 0x040001F6 RID: 502
	private const int bitMask = 7;
}
