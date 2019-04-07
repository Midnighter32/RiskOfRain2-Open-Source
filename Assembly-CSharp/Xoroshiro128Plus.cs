using System;
using System.Runtime.CompilerServices;

// Token: 0x02000078 RID: 120
public class Xoroshiro128Plus
{
	// Token: 0x060001DC RID: 476 RVA: 0x00009A94 File Offset: 0x00007C94
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong RotateLeft(ulong x, int k)
	{
		return x << k | x >> 64 - k;
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00009AA6 File Offset: 0x00007CA6
	public Xoroshiro128Plus(ulong seed)
	{
		Xoroshiro128Plus.initializer.x = seed;
		this.state0 = Xoroshiro128Plus.initializer.Next();
		this.state1 = Xoroshiro128Plus.initializer.Next();
	}

	// Token: 0x060001DE RID: 478 RVA: 0x00009ADC File Offset: 0x00007CDC
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

	// Token: 0x060001DF RID: 479 RVA: 0x00009B24 File Offset: 0x00007D24
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

	// Token: 0x060001E0 RID: 480 RVA: 0x00009B90 File Offset: 0x00007D90
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

	// Token: 0x060001E1 RID: 481 RVA: 0x00009BFA File Offset: 0x00007DFA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ToDouble01Fast(ulong x)
	{
		return BitConverter.Int64BitsToDouble((long)(4607182418800017408UL | x >> 12));
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00009C0F File Offset: 0x00007E0F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ToDouble01(ulong x)
	{
		return (x >> 11) * 1.1102230246251565E-16;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00009C21 File Offset: 0x00007E21
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float ToFloat01(uint x)
	{
		return (x >> 8) * 5.9604645E-08f;
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001E4 RID: 484 RVA: 0x00009C2E File Offset: 0x00007E2E
	public bool nextBool
	{
		get
		{
			return this.nextLong < 0L;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001E5 RID: 485 RVA: 0x00009C3A File Offset: 0x00007E3A
	public uint nextUint
	{
		get
		{
			return (uint)this.Next();
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x00009C43 File Offset: 0x00007E43
	public int nextInt
	{
		get
		{
			return (int)this.Next();
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001E7 RID: 487 RVA: 0x00009C4C File Offset: 0x00007E4C
	public ulong nextUlong
	{
		get
		{
			return this.Next();
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001E8 RID: 488 RVA: 0x00009C4C File Offset: 0x00007E4C
	public long nextLong
	{
		get
		{
			return (long)this.Next();
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001E9 RID: 489 RVA: 0x00009C54 File Offset: 0x00007E54
	public double nextNormalizedDouble
	{
		get
		{
			return Xoroshiro128Plus.ToDouble01Fast(this.Next());
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060001EA RID: 490 RVA: 0x00009C61 File Offset: 0x00007E61
	public float nextNormalizedFloat
	{
		get
		{
			return Xoroshiro128Plus.ToFloat01(this.nextUint);
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00009C6E File Offset: 0x00007E6E
	public float RangeFloat(float minInclusive, float maxInclusive)
	{
		return minInclusive + (maxInclusive - minInclusive) * this.nextNormalizedFloat;
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00009C7C File Offset: 0x00007E7C
	public int RangeInt(int minInclusive, int maxExclusive)
	{
		return minInclusive + (int)this.RangeUInt32Uniform((uint)(maxExclusive - minInclusive));
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00009C89 File Offset: 0x00007E89
	public long RangeLong(long minInclusive, long maxExclusive)
	{
		return minInclusive + (long)this.RangeUInt64Uniform((ulong)(maxExclusive - minInclusive));
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00009C98 File Offset: 0x00007E98
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

	// Token: 0x060001EF RID: 495 RVA: 0x00009CD0 File Offset: 0x00007ED0
	private uint RangeUInt32Uniform(uint maxExclusive)
	{
		if (maxExclusive == 0u)
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

	// Token: 0x060001F0 RID: 496 RVA: 0x00009D08 File Offset: 0x00007F08
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

	// Token: 0x060001F1 RID: 497 RVA: 0x00009D4C File Offset: 0x00007F4C
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

	// Token: 0x060001F2 RID: 498 RVA: 0x00009D6C File Offset: 0x00007F6C
	private static int CalcRequiredBits(uint v)
	{
		int num = 0;
		while (v != 0u)
		{
			v >>= 1;
			num++;
		}
		return num;
	}

	// Token: 0x040001FC RID: 508
	private ulong state0;

	// Token: 0x040001FD RID: 509
	private ulong state1;

	// Token: 0x040001FE RID: 510
	private static readonly SplitMix64 initializer = new SplitMix64();

	// Token: 0x040001FF RID: 511
	private const ulong JUMP0 = 16109378705422636197UL;

	// Token: 0x04000200 RID: 512
	private const ulong JUMP1 = 1659688472399708668UL;

	// Token: 0x04000201 RID: 513
	private static readonly ulong[] JUMP = new ulong[]
	{
		16109378705422636197UL,
		1659688472399708668UL
	};

	// Token: 0x04000202 RID: 514
	private const ulong LONG_JUMP0 = 15179817016004374139UL;

	// Token: 0x04000203 RID: 515
	private const ulong LONG_JUMP1 = 15987667697637423809UL;

	// Token: 0x04000204 RID: 516
	private static readonly ulong[] LONG_JUMP = new ulong[]
	{
		15179817016004374139UL,
		15987667697637423809UL
	};

	// Token: 0x04000205 RID: 517
	private static readonly int[] logTable256 = Xoroshiro128Plus.GenerateLogTable();
}
