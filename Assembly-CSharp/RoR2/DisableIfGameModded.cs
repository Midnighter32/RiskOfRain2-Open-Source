using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D7 RID: 727
	public class DisableIfGameModded : MonoBehaviour
	{
		// Token: 0x06000E8A RID: 3722 RVA: 0x00047A2A File Offset: 0x00045C2A
		public void OnEnable()
		{
			if (RoR2Application.isModded)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
