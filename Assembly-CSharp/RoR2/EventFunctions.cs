using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E9 RID: 745
	public class EventFunctions : MonoBehaviour
	{
		// Token: 0x06000F0E RID: 3854 RVA: 0x0004A8F2 File Offset: 0x00048AF2
		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0004A8FF File Offset: 0x00048AFF
		public void DestroyGameObject(GameObject obj)
		{
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0004A907 File Offset: 0x00048B07
		public void UnparentTransform(Transform transform)
		{
			if (transform)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0004A918 File Offset: 0x00048B18
		public void ToggleGameObjectActive(GameObject obj)
		{
			obj.SetActive(!obj.activeSelf);
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0004A929 File Offset: 0x00048B29
		public void OpenURL(string url)
		{
			Application.OpenURL(url);
		}
	}
}
