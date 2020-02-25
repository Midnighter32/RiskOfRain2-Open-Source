using System;
using RoR2;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class DestroyOnKill : MonoBehaviour, IOnKilledServerReceiver
{
	// Token: 0x060000CB RID: 203 RVA: 0x00005939 File Offset: 0x00003B39
	public void OnKilledServer(DamageReport damageReport)
	{
		UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab, base.transform.position, base.transform.rotation);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x040000DB RID: 219
	public GameObject effectPrefab;
}
