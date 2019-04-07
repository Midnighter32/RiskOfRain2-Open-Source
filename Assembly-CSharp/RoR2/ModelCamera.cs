using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035C RID: 860
	[RequireComponent(typeof(Camera))]
	public class ModelCamera : MonoBehaviour
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060011A9 RID: 4521 RVA: 0x00057927 File Offset: 0x00055B27
		// (set) Token: 0x060011AA RID: 4522 RVA: 0x0005792E File Offset: 0x00055B2E
		public static ModelCamera instance { get; private set; }

		// Token: 0x060011AB RID: 4523 RVA: 0x00057936 File Offset: 0x00055B36
		private void OnEnable()
		{
			if (ModelCamera.instance && ModelCamera.instance != this)
			{
				Debug.LogErrorFormat("Only one {0} instance can be active at a time.", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			ModelCamera.instance = this;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00057976 File Offset: 0x00055B76
		private void OnDisable()
		{
			if (ModelCamera.instance == this)
			{
				ModelCamera.instance = null;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060011AD RID: 4525 RVA: 0x0005798B File Offset: 0x00055B8B
		// (set) Token: 0x060011AE RID: 4526 RVA: 0x00057993 File Offset: 0x00055B93
		public Camera attachedCamera { get; private set; }

		// Token: 0x060011AF RID: 4527 RVA: 0x0005799C File Offset: 0x00055B9C
		private void Awake()
		{
			this.attachedCamera = base.GetComponent<Camera>();
			this.attachedCamera.enabled = false;
			this.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			UnityEngine.Object.Destroy(base.GetComponent<AkAudioListener>());
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x000579EC File Offset: 0x00055BEC
		private static void PrepareObjectForRendering(Transform objTransform, List<ModelCamera.ObjectRestoreInfo> objectRestorationList)
		{
			GameObject gameObject = objTransform.gameObject;
			objectRestorationList.Add(new ModelCamera.ObjectRestoreInfo
			{
				obj = gameObject,
				layer = gameObject.layer
			});
			gameObject.layer = LayerIndex.manualRender.intVal;
			int childCount = objTransform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				ModelCamera.PrepareObjectForRendering(objTransform.GetChild(i), objectRestorationList);
			}
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00057A54 File Offset: 0x00055C54
		public void RenderItem(GameObject obj, RenderTexture targetTexture)
		{
			for (int i = 0; i < this.lights.Count; i++)
			{
				this.lights[i].cullingMask = LayerIndex.manualRender.mask;
			}
			RenderSettingsState renderSettingsState = RenderSettingsState.FromCurrent();
			this.renderSettings.Apply();
			List<ModelCamera.ObjectRestoreInfo> list = new List<ModelCamera.ObjectRestoreInfo>();
			if (obj)
			{
				ModelCamera.PrepareObjectForRendering(obj.transform, list);
			}
			this.attachedCamera.targetTexture = targetTexture;
			this.attachedCamera.Render();
			for (int j = 0; j < list.Count; j++)
			{
				list[j].obj.layer = list[j].layer;
			}
			for (int k = 0; k < this.lights.Count; k++)
			{
				this.lights[k].cullingMask = 0;
			}
			renderSettingsState.Apply();
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00057B44 File Offset: 0x00055D44
		public void AddLight(Light light)
		{
			this.lights.Add(light);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00057B52 File Offset: 0x00055D52
		public void RemoveLight(Light light)
		{
			this.lights.Remove(light);
		}

		// Token: 0x040015C6 RID: 5574
		[NonSerialized]
		public RenderSettingsState renderSettings;

		// Token: 0x040015C8 RID: 5576
		public Color ambientLight;

		// Token: 0x040015CA RID: 5578
		private readonly List<Light> lights = new List<Light>();

		// Token: 0x0200035D RID: 861
		private struct ObjectRestoreInfo
		{
			// Token: 0x040015CB RID: 5579
			public GameObject obj;

			// Token: 0x040015CC RID: 5580
			public int layer;
		}
	}
}
