using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001F7 RID: 503
	public class EventFunctions : MonoBehaviour
	{
		// Token: 0x06000AB2 RID: 2738 RVA: 0x0002F7B2 File Offset: 0x0002D9B2
		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0002F7BF File Offset: 0x0002D9BF
		public void DestroyGameObject(GameObject obj)
		{
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0002F7C7 File Offset: 0x0002D9C7
		public void UnparentTransform(Transform transform)
		{
			if (transform)
			{
				transform.SetParent(null);
			}
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0002F7D8 File Offset: 0x0002D9D8
		public void ToggleGameObjectActive(GameObject obj)
		{
			obj.SetActive(!obj.activeSelf);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0002F7EC File Offset: 0x0002D9EC
		public void CreateLocalEffect(GameObject effectObj)
		{
			EffectManager.SpawnEffect(effectObj, new EffectData
			{
				origin = base.transform.position
			}, false);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0002F818 File Offset: 0x0002DA18
		public void OpenURL(string url)
		{
			Application.OpenURL(url);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0002F820 File Offset: 0x0002DA20
		public void PlaySound(string soundString)
		{
			Util.PlaySound(soundString, base.gameObject);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0002F82F File Offset: 0x0002DA2F
		public void RunSetFlag(string flagName)
		{
			if (NetworkServer.active)
			{
				Run instance = Run.instance;
				if (instance == null)
				{
					return;
				}
				instance.SetEventFlag(flagName);
			}
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0002F848 File Offset: 0x0002DA48
		public void RunResetFlag(string flagName)
		{
			if (NetworkServer.active)
			{
				Run instance = Run.instance;
				if (instance == null)
				{
					return;
				}
				instance.ResetEventFlag(flagName);
			}
		}
	}
}
