using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(UnderWaterFog))]
[ExecuteInEditMode]
public class FogControl : MonoBehaviour
{
	// Token: 0x06000007 RID: 7 RVA: 0x0000207C File Offset: 0x0000027C
	private void OnEnable()
	{
		this.init();
	}

	// Token: 0x06000008 RID: 8 RVA: 0x0000207C File Offset: 0x0000027C
	private void Start()
	{
		this.init();
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002084 File Offset: 0x00000284
	private void Update()
	{
		this.Rate += Time.deltaTime / this.FadeSpeed;
		this.Rate = Mathf.Clamp(this.Rate, 0f, this.FadeSpeed);
		if (this.cam.transform.position.y <= this.fog.height)
		{
			if (!this.fog.enabled)
			{
				this.fog.enabled = true;
			}
			this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 1f, this.Rate);
			return;
		}
		this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 0f, this.Rate * 2f);
		if (this.fog.fogColor.a <= 0.01f)
		{
			this.fog.enabled = false;
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x0000218C File Offset: 0x0000038C
	private void init()
	{
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
		}
		if (this.fog == null)
		{
			this.fog = base.GetComponent<UnderWaterFog>();
		}
		if (this.cam.transform.position.y >= this.fog.height)
		{
			this.fog.fogColor.a = 0f;
		}
	}

	// Token: 0x04000001 RID: 1
	public float FadeSpeed = 10f;

	// Token: 0x04000002 RID: 2
	private float Rate = 1f;

	// Token: 0x04000003 RID: 3
	private UnderWaterFog fog;

	// Token: 0x04000004 RID: 4
	private Camera cam;
}
