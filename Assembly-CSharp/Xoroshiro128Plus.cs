using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x0200007D RID: 125
public class Xoroshiro128Plus
{
	// Token: 0x06000215 RID: 533 RVA: 0x0000A4F4 File Offset: 0x000086F4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong RotateLeft(ulong x, int k)
	{
		return x << k | x >> 64 - k;
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000A506 File Offset: 0x00008706
	public Xoroshiro128Plus(ulong seed)
	{
		Xoroshiro128Plus.initializer.x = seed;
		this.state0 = Xoroshiro128Plus.initializer.Next();
		this.state1 = Xoroshiro128Plus.initializer.Next();
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000A53C File Offset: 0x0000873C
	public ulong Next()
	{
		ulong num = this.state0;
		ulong num2 = this.state1;
		ulong result = num + num2;
		num2 ^= num;
		this.state0 = (Xoroshiro128Plus.RotateLeft(num, 24) ^ num2 ^ num2 << 16);
		this.state1 = Xoroshiro128Plus.RotateLeft(num2, 37);
		return result;
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000A584 File Offset: 0x00008784
	public void Jump()
	{
		ulong num = 0UL;
		ulong num2 = 0UL;
		for (int i = 0; i < Xoroshiro128Plus.JUMP.Length; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				if ((Xoroshiro128Plus.JUMP[i] & 1UL) << j != 0UL)
				{
					num ^= this.state0;
					num2 ^= this.state1;
				}
				this.Next();
			}
		}
		this.state0 = num;
		this.state1 = num2;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000A5F0 File Offset: 0x000087F0
	public void LongJump()
	{
		ulong num = 0UL;
		ulong num2 = 0UL;
		for (int i = 0; i < Xoroshiro128Plus.LONG_JUMP.Length; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				if ((Xoroshiro128Plus.LONG_JUMP[i] & 1UL) << j != 0UL)
				{
					num ^= this.state0;
					num2 ^= this.state1;
				}
				this.Next();
			}
		}
		this.state0 = num;
		this.state1 = num2;
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000A65A File Offset: 0x0000885A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ToDouble01Fast(ulong x)
	{
		return BitConverter.Int64BitsToDouble((long)(4607182418800017408UL | x >> 12));
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000A66F File Offset: 0x0000886F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ToDouble01(ulong x)
	{
		return (x >> 11) * 1.1102230246251565E-16;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000A681 File Offset: 0x00008881
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float ToFloat01(uint x)
	{
		return (x >> 8) * 5.9604645E-08f;
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600021D RID: 541 RVA: 0x0000A68E File Offset: 0x0000888E
	public bool nextBool
	{
		get
		{
			return this.nextLong < 0L;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600021E RID: 542 RVA: 0x0000A69A File Offset: 0x0000889A
	public uint nextUint
	{
		get
		{
			return (uint)this.Next();
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600021F RID: 543 RVA: 0x0000A6A3 File Offset: 0x000088A3
	public int nextInt
	{
		get
		{
			return (int)this.Next();
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000220 RID: 544 RVA: 0x0000A6AC File Offset: 0x000088AC
	public ulong nextUlong
	{
		get
		{
			return this.Next();
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000221 RID: 545 RVA: 0x0000A6AC File Offset: 0x000088AC
	public long nextLong
	{
		get
		{
			return (long)this.Next();
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000222 RID: 546 RVA: 0x0000A6B4 File Offset: 0x000088B4
	public double nextNormalizedDouble
	{
		get
		{
			return Xoroshiro128Plus.ToDouble01Fast(this.Next());
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000223 RID: 547 RVA: 0x0000A6C1 File Offset: 0x000088C1
	public float nextNormalizedFloat
	{
		get
		{
			return Xoroshiro128Plus.ToFloat01(this.nextUint);
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000A6CE File Offset: 0x000088CE
	public float RangeFloat(float minInclusive, float maxInclusive)
	{
		return minInclusive + (maxInclusive - minInclusive) * this.nextNormalizedFloat;
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000A6DC File Offset: 0x000088DC
	public int RangeInt(int minInclusive, int maxExclusive)
	{
		return minInclusive + (int)this.RangeUInt32Uniform((uint)(maxExclusive - minInclusive));
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000A6E9 File Offset: 0x000088E9
	public long RangeLong(long minInclusive, long maxExclusive)
	{
		return minInclusive + (long)this.RangeUInt64Uniform((ulong)(maxExclusive - minInclusive));
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000A6F8 File Offset: 0x000088F8
	private ulong RangeUInt64Uniform(ulong maxExclusive)
	{
		if (maxExclusive == 0UL)
		{
			throw new ArgumentOutOfRangeException("Range cannot have size of zero.");
		}
		int num = Xoroshiro128Plus.CalcRequiredBits(maxExclusive);
		int num2 = 64 - num;
		ulong num3;
		do
		{
			num3 = this.nextUlong >> num2;
		}
		while (num3 >= maxExclusive);
		return num3;
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000A730 File Offset: 0x00008930
	private uint RangeUInt32Uniform(uint maxExclusive)
	{
		if (maxExclusive == 0U)
		{
			throw new ArgumentOutOfRangeException("Range cannot have size of zero.");
		}
		int num = Xoroshiro128Plus.CalcRequiredBits(maxExclusive);
		int num2 = 32 - num;
		uint num3;
		do
		{
			num3 = this.nextUint >> num2;
		}
		while (num3 >= maxExclusive);
		return num3;
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000A768 File Offset: 0x00008968
	private static int[] GenerateLogTable()
	{
		int[] array = new int[256];
		array[0] = (array[1] = 0);
		for (int i = 2; i < 256; i++)
		{
			array[i] = 1 + array[i / 2];
		}
		array[0] = -1;
		return array;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000A7AC File Offset: 0x000089AC
	private static int CalcRequiredBits(ulong v)
	{
		int num = 0;
		while (v != 0UL)
		{
			v >>= 1;
			num++;
		}
		return num;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000A7CC File Offset: 0x000089CC
	private static int CalcRequiredBits(uint v)
	{
		int num = 0;
		while (v != 0U)
		{
			v >>= 1;
			num++;
		}
		return num;
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000A7EA File Offset: 0x000089EA
	public ref T NextElementUniform<T>(T[] array)
	{
		return ref array[this.RangeInt(0, array.Length)];
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000A7FC File Offset: 0x000089FC
	public T NextElementUniform<T>(List<T> list)
	{
		return list[this.RangeInt(0, list.Count)];
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000A811 File Offset: 0x00008A11
	public T NextElementUniform<T>(IList<T> list)
	{
		return list[this.RangeInt(0, list.Count)];
	}

	// Token: 0x04000207 RID: 519
	private ulong state0;

	// Token: 0x04000208 RID: 520
	private ulong state1;

	// Token: 0x04000209 RID: 521
	private static readonly SplitMix64 initializer = new SplitMix64();

	// Token: 0x0400020A RID: 522
	private const ulong JUMP0 = 16109378705422636197UL;

	// Token: 0x0400020B RID: 523
	private const ulong JUMP1 = 1659688472399708668UL;

	// Token: 0x0400020C RID: 524
	private static readonly ulong[] JUMP = new ulong[]
	{
		16109378705422636197UL,
		1659688472399708668UL
	};

	// Token: 0x0400020D RID: 525
	private const ulong LONG_JUMP0 = 15179817016004374139UL;

	// Token: 0x0400020E RID: 526
	private const ulong LONG_JUMP1 = 15987667697637423809UL;

	// Token: 0x0400020F RID: 527
	private static readonly ulong[] LONG_JUMP = new ulong[]
	{
		15179817016004374139UL,
		15987667697637423809UL
	};

	// Token: 0x04000210 RID: 528
	private static readonly int[] logTable256 = Xoroshiro128Plus.GenerateLogTable();
}
