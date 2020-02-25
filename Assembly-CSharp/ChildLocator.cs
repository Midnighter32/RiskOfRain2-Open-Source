using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
[DisallowMultipleComponent]
public class ChildLocator : MonoBehaviour
{
	// Token: 0x060000C5 RID: 197 RVA: 0x00005860 File Offset: 0x00003A60
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

	// Token: 0x060000C6 RID: 198 RVA: 0x0000589C File Offset: 0x00003A9C
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

	// Token: 0x060000C7 RID: 199 RVA: 0x000058D3 File Offset: 0x00003AD3
	public string FindChildName(int childIndex)
	{
		if ((ulong)childIndex < (ulong)((long)this.transformPairs.Length))
		{
			return this.transformPairs[childIndex].name;
		}
		return null;
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x000058F5 File Offset: 0x00003AF5
	public Transform FindChild(string childName)
	{
		return this.FindChild(this.FindChildIndex(childName));
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00005904 File Offset: 0x00003B04
	public Transform FindChild(int childIndex)
	{
		if ((ulong)childIndex < (ulong)((long)this.transformPairs.Length))
		{
			return this.transformPairs[childIndex].transform;
		}
		return null;
	}

	// Token: 0x040000D8 RID: 216
	[SerializeField]
	private ChildLocator.NameTransformPair[] transformPairs = Array.Empty<ChildLocator.NameTransformPair>();

	// Token: 0x0200002D RID: 45
	[Serializable]
	private struct NameTransformPair
	{
		// Token: 0x040000D9 RID: 217
		public string name;

		// Token: 0x040000DA RID: 218
		public Transform transform;
	}
}
