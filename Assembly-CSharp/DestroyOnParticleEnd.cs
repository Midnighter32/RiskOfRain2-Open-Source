using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class DestroyOnParticleEnd : MonoBehaviour
{
	// Token: 0x060000CD RID: 205 RVA: 0x00005968 File Offset: 0x00003B68
	public void Awake()
	{
		this.ps = base.GetComponentInChildren<ParticleSystem>();
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00005976 File Offset: 0x00003B76
	public void Update()
	{
		if (this.ps && !this.ps.IsAlive())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040000DC RID: 220
	private ParticleSystem ps;
}
