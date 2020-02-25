using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class BeginRapidlyActivatingAndDeactivating : MonoBehaviour
{
	// Token: 0x060000BB RID: 187 RVA: 0x00005438 File Offset: 0x00003638
	private void FixedUpdate()
	{
		this.fixedAge += Time.fixedDeltaTime;
		if (this.fixedAge >= this.delayBeforeBeginningBlinking)
		{
			this.blinkAge += Time.fixedDeltaTime;
			if (this.blinkAge >= 1f / this.blinkFrequency)
			{
				this.blinkAge -= 1f / this.blinkFrequency;
				this.blinkingRootObject.SetActive(!this.blinkingRootObject.activeSelf);
			}
		}
	}

	// Token: 0x040000C4 RID: 196
	public float blinkFrequency = 10f;

	// Token: 0x040000C5 RID: 197
	public float delayBeforeBeginningBlinking = 30f;

	// Token: 0x040000C6 RID: 198
	public GameObject blinkingRootObject;

	// Token: 0x040000C7 RID: 199
	private float fixedAge;

	// Token: 0x040000C8 RID: 200
	private float blinkAge;
}
