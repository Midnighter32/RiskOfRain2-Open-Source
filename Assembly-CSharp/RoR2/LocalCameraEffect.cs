using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000276 RID: 630
	public class LocalCameraEffect : MonoBehaviour
	{
		// Token: 0x06000E03 RID: 3587 RVA: 0x0003EB09 File Offset: 0x0003CD09
		static LocalCameraEffect()
		{
			UICamera.onUICameraPreCull += LocalCameraEffect.OnUICameraPreCull;
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x0003EB28 File Offset: 0x0003CD28
		private static void OnUICameraPreCull(UICamera uiCamera)
		{
			for (int i = 0; i < LocalCameraEffect.instancesList.Count; i++)
			{
				GameObject target = uiCamera.cameraRigController.target;
				LocalCameraEffect localCameraEffect = LocalCameraEffect.instancesList[i];
				if (localCameraEffect.targetCharacter == target)
				{
					localCameraEffect.effectRoot.SetActive(true);
				}
				else
				{
					localCameraEffect.effectRoot.SetActive(false);
				}
			}
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x0003EB8A File Offset: 0x0003CD8A
		private void Start()
		{
			LocalCameraEffect.instancesList.Add(this);
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x0003EB97 File Offset: 0x0003CD97
		private void OnDestroy()
		{
			LocalCameraEffect.instancesList.Remove(this);
		}

		// Token: 0x04000DF9 RID: 3577
		public GameObject targetCharacter;

		// Token: 0x04000DFA RID: 3578
		public GameObject effectRoot;

		// Token: 0x04000DFB RID: 3579
		private static List<LocalCameraEffect> instancesList = new List<LocalCameraEffect>();
	}
}
