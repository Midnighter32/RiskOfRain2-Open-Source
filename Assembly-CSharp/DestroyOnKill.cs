using System;
using RoR2;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class DestroyOnKill : MonoBehaviour
{
	// Token: 0x060000E5 RID: 229 RVA: 0x000059A1 File Offset: 0x00003BA1
	private void OnKilled(DamageInfo damageInfo)
	{
		UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab, base.transform.position, base.transform.rotation);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040000D5 RID: 213
	public GameObject effectPrefab;
}
