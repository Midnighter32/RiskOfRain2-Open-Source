using System;
using RoR2;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class WispFlameControl : MonoBehaviour
{
	// Token: 0x06000180 RID: 384 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00008874 File Offset: 0x00006A74
	private void Update()
	{
		this.flame = Mathf.SmoothDamp(this.flame, this.flameTarget, ref this.velocity, 1f);
		this.flameMain.transform.localScale = Vector3.one * this.flame;
		this.flameCore.transform.localScale = Vector3.one * this.flame;
	}

	// Token: 0x06000182 RID: 386 RVA: 0x000088E3 File Offset: 0x00006AE3
	public void SetFlame(float target)
	{
		this.flameTarget = target;
	}

	// Token: 0x06000183 RID: 387 RVA: 0x000088EC File Offset: 0x00006AEC
	public void OnIncomingDamage(DamageInfo damageinfo)
	{
		this.flame = 0.5f;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x000088F9 File Offset: 0x00006AF9
	public void OnKilled(DamageInfo damageinfo)
	{
		this.flameTarget = 0f;
	}

	// Token: 0x040001AB RID: 427
	public float flame;

	// Token: 0x040001AC RID: 428
	public float flameTarget = 1f;

	// Token: 0x040001AD RID: 429
	private float velocity;

	// Token: 0x040001AE RID: 430
	public ParticleSystem flameMain;

	// Token: 0x040001AF RID: 431
	public ParticleSystem flameCore;
}
