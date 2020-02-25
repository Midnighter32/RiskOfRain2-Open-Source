using System;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class WeightedSelection<T>
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000207 RID: 519 RVA: 0x0000A1E8 File Offset: 0x000083E8
	// (set) Token: 0x06000208 RID: 520 RVA: 0x0000A1F0 File Offset: 0x000083F0
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

	// Token: 0x06000209 RID: 521 RVA: 0x0000A1F9 File Offset: 0x000083F9
	public WeightedSelection(int capacity = 8)
	{
		this.choices = new WeightedSelection<T>.ChoiceInfo[capacity];
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x0600020A RID: 522 RVA: 0x0000A20D File Offset: 0x0000840D
	// (set) Token: 0x0600020B RID: 523 RVA: 0x0000A217 File Offset: 0x00008417
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

	// Token: 0x0600020C RID: 524 RVA: 0x0000A254 File Offset: 0x00008454
	public void AddChoice(T value, float weight)
	{
		this.AddChoice(new WeightedSelection<T>.ChoiceInfo
		{
			value = value,
			weight = weight
		});
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000A280 File Offset: 0x00008480
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

	// Token: 0x0600020E RID: 526 RVA: 0x0000A2DC File Offset: 0x000084DC
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

	// Token: 0x0600020F RID: 527 RVA: 0x0000A358 File Offset: 0x00008558
	public void ModifyChoiceWeight(int choiceIndex, float newWeight)
	{
		this.choices[choiceIndex].weight = newWeight;
		this.RecalculateTotalWeight();
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000A374 File Offset: 0x00008574
	public void Clear()
	{
		for (int i = 0; i < this.Count; i++)
		{
			this.choices[i] = default(WeightedSelection<T>.ChoiceInfo);
		}
		this.Count = 0;
		this.totalWeight = 0f;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000A3B8 File Offset: 0x000085B8
	private void RecalculateTotalWeight()
	{
		this.totalWeight = 0f;
		for (int i = 0; i < this.Count; i++)
		{
			this.totalWeight += this.choices[i].weight;
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000A400 File Offset: 0x00008600
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

	// Token: 0x06000213 RID: 531 RVA: 0x0000A484 File Offset: 0x00008684
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

	// Token: 0x06000214 RID: 532 RVA: 0x0000A4E6 File Offset: 0x000086E6
	public WeightedSelection<T>.ChoiceInfo GetChoice(int i)
	{
		return this.choices[i];
	}

	// Token: 0x04000201 RID: 513
	[HideInInspector]
	[SerializeField]
	public WeightedSelection<T>.ChoiceInfo[] choices;

	// Token: 0x04000202 RID: 514
	[SerializeField]
	[HideInInspector]
	private int _count;

	// Token: 0x04000203 RID: 515
	[SerializeField]
	[HideInInspector]
	private float totalWeight;

	// Token: 0x04000204 RID: 516
	private const int minCapacity = 8;

	// Token: 0x0200007C RID: 124
	[Serializable]
	public struct ChoiceInfo
	{
		// Token: 0x04000205 RID: 517
		public T value;

		// Token: 0x04000206 RID: 518
		public float weight;
	}
}
