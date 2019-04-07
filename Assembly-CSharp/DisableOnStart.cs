using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class DisableOnStart : MonoBehaviour
{
	// Token: 0x060000EF RID: 239 RVA: 0x00005A6E File Offset: 0x00003C6E
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
