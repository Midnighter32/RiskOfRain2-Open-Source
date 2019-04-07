using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000379 RID: 889
	public class OnDestroyCallback : MonoBehaviour
	{
		// Token: 0x06001299 RID: 4761 RVA: 0x0005B1C3 File Offset: 0x000593C3
		public void OnDestroy()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0005B1D9 File Offset: 0x000593D9
		public static OnDestroyCallback AddCallback(GameObject gameObject, Action<OnDestroyCallback> callback)
		{
			OnDestroyCallback onDestroyCallback = gameObject.AddComponent<OnDestroyCallback>();
			onDestroyCallback.callback = callback;
			return onDestroyCallback;
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x0005B1E8 File Offset: 0x000593E8
		public static void RemoveCallback(OnDestroyCallback callbackComponent)
		{
			callbackComponent.callback = null;
			UnityEngine.Object.Destroy(callbackComponent);
		}

		// Token: 0x04001646 RID: 5702
		private Action<OnDestroyCallback> callback;
	}
}
