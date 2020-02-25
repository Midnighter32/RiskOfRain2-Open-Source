using System;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class DisableOnStart : MonoBehaviour
{
	// Token: 0x060000D3 RID: 211 RVA: 0x000059C5 File Offset: 0x00003BC5
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
