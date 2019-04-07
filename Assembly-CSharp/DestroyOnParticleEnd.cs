using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class DestroyOnParticleEnd : MonoBehaviour
{
	// Token: 0x060000E7 RID: 231 RVA: 0x000059D0 File Offset: 0x00003BD0
	public void Awake()
	{
		this.ps = base.GetComponentInChildren<ParticleSystem>();
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x000059DE File Offset: 0x00003BDE
	public void Update()
	{
		if (this.ps && !this.ps.IsAlive())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040000D6 RID: 214
	private ParticleSystem ps;
}
