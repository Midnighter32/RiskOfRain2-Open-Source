using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
[RequireComponent(typeof(AudioSource))]
public class DestroyOnSoundEnd : MonoBehaviour
{
	// Token: 0x060000EA RID: 234 RVA: 0x00005A05 File Offset: 0x00003C05
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00005A13 File Offset: 0x00003C13
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040000D7 RID: 215
	private AudioSource audioSource;
}
