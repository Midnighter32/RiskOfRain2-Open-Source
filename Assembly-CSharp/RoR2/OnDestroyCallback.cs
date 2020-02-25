using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002AD RID: 685
	public class OnDestroyCallback : MonoBehaviour
	{
		// Token: 0x06000F94 RID: 3988 RVA: 0x00044673 File Offset: 0x00042873
		public void OnDestroy()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x00044689 File Offset: 0x00042889
		public static OnDestroyCallback AddCallback(GameObject gameObject, Action<OnDestroyCallback> callback)
		{
			OnDestroyCallback onDestroyCallback = gameObject.AddComponent<OnDestroyCallback>();
			onDestroyCallback.callback = callback;
			return onDestroyCallback;
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00044698 File Offset: 0x00042898
		public static void RemoveCallback(OnDestroyCallback callbackComponent)
		{
			callbackComponent.callback = null;
			UnityEngine.Object.Destroy(callbackComponent);
		}

		// Token: 0x04000F01 RID: 3841
		private Action<OnDestroyCallback> callback;
	}
}
