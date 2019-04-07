using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class WeightedSelection<T>
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060001CE RID: 462 RVA: 0x00009786 File Offset: 0x00007986
	// (set) Token: 0x060001CF RID: 463 RVA: 0x0000978E File Offset: 0x0000798E
	public int Count
	{
		get
		{
			return this._count;
		}
		private set
		{
			this._count = value;
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00009797 File Offset: 0x00007997
	public WeightedSelection(int capacity = 8)
	{
		this.choices = new WeightedSelection<T>.ChoiceInfo[capacity];
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060001D1 RID: 465 RVA: 0x000097AB File Offset: 0x000079AB
	// (set) Token: 0x060001D2 RID: 466 RVA: 0x000097B5 File Offset: 0x000079B5
	public int Capacity
	{
		get
		{
			return this.choices.Length;
		}
		set
		{
			if (value < 8 || value < this.Count)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			Array sourceArray = this.choices;
			this.choices = new WeightedSelection<T>.ChoiceInfo[value];
			Array.Copy(sourceArray, this.choices, this.Count);
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x000097F4 File Offset: 0x000079F4
	public void AddChoice(T value, float weight)
	{
		this.AddChoice(new WeightedSelection<T>.ChoiceInfo
		{
			value = value,
			weight = weight
		});
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00009820 File Offset: 0x00007A20
	public void AddChoice(WeightedSelection<T>.ChoiceInfo choice)
	{
		if (this.Count == this.Capacity)
		{
			this.Capacity *= 2;
		}
		WeightedSelection<T>.ChoiceInfo[] array = this.choices;
		int count = this.Count;
		this.Count = count + 1;
		array[count] = choice;
		this.totalWeight += choice.weight;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000987C File Offset: 0x00007A7C
	public void RemoveChoice(int choiceIndex)
	{
		if (choiceIndex < 0 || this.Count <= choiceIndex)
		{
			throw new ArgumentOutOfRangeException("choiceIndex");
		}
		int i = choiceIndex;
		int num = this.Count - 1;
		while (i < num)
		{
			this.choices[i] = this.choices[i + 1];
			i++;
		}
		WeightedSelection<T>.ChoiceInfo[] array = this.choices;
		int num2 = this.Count - 1;
		this.Count = num2;
		array[num2] = default(WeightedSelection<T>.ChoiceInfo);
		this.RecalculateTotalWeight();
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x000098F8 File Offset: 0x00007AF8
	public void ModifyChoiceWeight(int choiceIndex, float newWeight)
	{
		this.choices[choiceIndex].weight = newWeight;
		this.RecalculateTotalWeight();
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00009914 File Offset: 0x00007B14
	public void Clear()
	{
		for (int i = 0; i < this.Count; i++)
		{
			this.choices[i] = default(WeightedSelection<T>.ChoiceInfo);
		}
		this.Count = 0;
		this.totalWeight = 0f;
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00009958 File Offset: 0x00007B58
	private void RecalculateTotalWeight()
	{
		this.totalWeight = 0f;
		for (int i = 0; i < this.Count; i++)
		{
			this.totalWeight += this.choices[i].weight;
		}
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x000099A0 File Offset: 0x00007BA0
	public T Evaluate(float normalizedIndex)
	{
		if (this.Count == 0)
		{
			throw new InvalidOperationException("Cannot call Choose() without available choices.");
		}
		float num = normalizedIndex * this.totalWeight;
		float num2 = 0f;
		for (int i = 0; i < this.Count; i++)
		{
			num2 += this.choices[i].weight;
			if (num < num2)
			{
				return this.choices[i].value;
			}
		}
		return this.choices[this.Count - 1].value;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00009A24 File Offset: 0x00007C24
	public int EvaluteToChoiceIndex(float normalizedIndex)
	{
		if (this.Count == 0)
		{
			throw new InvalidOperationException("Cannot call Choose() without available choices.");
		}
		float num = normalizedIndex * this.totalWeight;
		float num2 = 0f;
		for (int i = 0; i < this.Count; i++)
		{
			num2 += this.choices[i].weight;
			if (num < num2)
			{
				return i;
			}
		}
		return this.Count - 1;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00009A86 File Offset: 0x00007C86
	public WeightedSelection<T>.ChoiceInfo GetChoice(int i)
	{
		return this.choices[i];
	}

	// Token: 0x040001F6 RID: 502
	[SerializeField]
	[HideInInspector]
	public WeightedSelection<T>.ChoiceInfo[] choices;

	// Token: 0x040001F7 RID: 503
	[HideInInspector]
	[SerializeField]
	private int _count;

	// Token: 0x040001F8 RID: 504
	[HideInInspector]
	[SerializeField]
	private float totalWeight;

	// Token: 0x040001F9 RID: 505
	private const int minCapacity = 8;

	// Token: 0x02000077 RID: 119
	[Serializable]
	public struct ChoiceInfo
	{
		// Token: 0x040001FA RID: 506
		public T value;

		// Token: 0x040001FB RID: 507
		public float weight;
	}
}
