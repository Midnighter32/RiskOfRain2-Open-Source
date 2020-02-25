using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
[RequireComponent(typeof(AudioSource))]
public class DestroyOnSoundEnd : MonoBehaviour
{
	// Token: 0x060000D0 RID: 208 RVA: 0x0000599D File Offset: 0x00003B9D
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000059AB File Offset: 0x00003BAB
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040000DD RID: 221
	private AudioSource audioSource;
}
