using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000288 RID: 648
	[RequireComponent(typeof(Camera))]
	public class ModelCamera : MonoBehaviour
	{
		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000E67 RID: 3687 RVA: 0x000401AF File Offset: 0x0003E3AF
		// (set) Token: 0x06000E68 RID: 3688 RVA: 0x000401B6 File Offset: 0x0003E3B6
		public static ModelCamera instance { get; private set; }

		// Token: 0x06000E69 RID: 3689 RVA: 0x000401BE File Offset: 0x0003E3BE
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

		// Token: 0x06000E6A RID: 3690 RVA: 0x000401FE File Offset: 0x0003E3FE
		private void OnDisable()
		{
			if (ModelCamera.instance == this)
			{
				ModelCamera.instance = null;
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000E6B RID: 3691 RVA: 0x00040213 File Offset: 0x0003E413
		// (set) Token: 0x06000E6C RID: 3692 RVA: 0x0004021B File Offset: 0x0003E41B
		public Camera attachedCamera { get; private set; }

		// Token: 0x06000E6D RID: 3693 RVA: 0x00040224 File Offset: 0x0003E424
		private void Awake()
		{
			this.attachedCamera = base.GetComponent<Camera>();
			this.attachedCamera.enabled = false;
			this.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			UnityEngine.Object.Destroy(base.GetComponent<AkAudioListener>());
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00040274 File Offset: 0x0003E474
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

		// Token: 0x06000E6F RID: 3695 RVA: 0x000402DC File Offset: 0x0003E4DC
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

		// Token: 0x06000E70 RID: 3696 RVA: 0x000403CC File Offset: 0x0003E5CC
		public void AddLight(Light light)
		{
			this.lights.Add(light);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x000403DA File Offset: 0x0003E5DA
		public void RemoveLight(Light light)
		{
			this.lights.Remove(light);
		}

		// Token: 0x04000E4E RID: 3662
		[NonSerialized]
		public RenderSettingsState renderSettings;

		// Token: 0x04000E50 RID: 3664
		public Color ambientLight;

		// Token: 0x04000E52 RID: 3666
		private readonly List<Light> lights = new List<Light>();

		// Token: 0x02000289 RID: 649
		private struct ObjectRestoreInfo
		{
			// Token: 0x04000E53 RID: 3667
			public GameObject obj;

			// Token: 0x04000E54 RID: 3668
			public int layer;
		}
	}
}
