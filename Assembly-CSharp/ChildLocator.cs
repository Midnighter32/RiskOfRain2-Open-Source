using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
[DisallowMultipleComponent]
public class ChildLocator : MonoBehaviour
{
	// Token: 0x060000DF RID: 223 RVA: 0x000058C8 File Offset: 0x00003AC8
	public int FindChildIndex(string childName)
	{
		for (int i = 0; i < this.transformPairs.Length; i++)
		{
			if (childName == this.transformPairs[i].name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00005904 File Offset: 0x00003B04
	public int FindChildIndex(Transform childTransform)
	{
		for (int i = 0; i < this.transformPairs.Length; i++)
		{
			if (childTransform == this.transformPairs[i].transform)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x0000593B File Offset: 0x00003B3B
	public string FindChildName(int childIndex)
	{
		if ((ulong)childIndex < (ulong)((long)this.transformPairs.Length))
		{
			return this.transformPairs[childIndex].name;
		}
		return null;
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x0000595D File Offset: 0x00003B5D
	public Transform FindChild(string childName)
	{
		return this.FindChild(this.FindChildIndex(childName));
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x0000596C File Offset: 0x00003B6C
	public Transform FindChild(int childIndex)
	{
		if ((ulong)childIndex < (ulong)((long)this.transformPairs.Length))
		{
			return this.transformPairs[childIndex].transform;
		}
		return null;
	}

	// Token: 0x040000D2 RID: 210
	[SerializeField]
	private ChildLocator.NameTransformPair[] transformPairs = Array.Empty<ChildLocator.NameTransformPair>();

	// Token: 0x02000030 RID: 48
	[Serializable]
	private struct NameTransformPair
	{
		// Token: 0x040000D3 RID: 211
		public string name;

		// Token: 0x040000D4 RID: 212
		public Transform transform;
	}
}
