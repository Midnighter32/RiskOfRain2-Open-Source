using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000365 RID: 869
	[RequireComponent(typeof(Camera))]
	public class UICamera : MonoBehaviour
	{
		// Token: 0x14000047 RID: 71
		// (add) Token: 0x0600151C RID: 5404 RVA: 0x0005A33C File Offset: 0x0005853C
		// (remove) Token: 0x0600151D RID: 5405 RVA: 0x0005A370 File Offset: 0x00058570
		public static event UICamera.UICameraDelegate onUICameraPreCull;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x0600151E RID: 5406 RVA: 0x0005A3A4 File Offset: 0x000585A4
		// (remove) Token: 0x0600151F RID: 5407 RVA: 0x0005A3D8 File Offset: 0x000585D8
		public static event UICamera.UICameraDelegate onUICameraPreRender;

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06001520 RID: 5408 RVA: 0x0005A40C File Offset: 0x0005860C
		// (remove) Token: 0x06001521 RID: 5409 RVA: 0x0005A440 File Offset: 0x00058640
		public static event UICamera.UICameraDelegate onUICameraPostRender;

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06001522 RID: 5410 RVA: 0x0005A473 File Offset: 0x00058673
		// (set) Token: 0x06001523 RID: 5411 RVA: 0x0005A47B File Offset: 0x0005867B
		public Camera camera { get; private set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06001524 RID: 5412 RVA: 0x0005A484 File Offset: 0x00058684
		// (set) Token: 0x06001525 RID: 5413 RVA: 0x0005A48C File Offset: 0x0005868C
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001526 RID: 5414 RVA: 0x0005A495 File Offset: 0x00058695
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0005A4AF File Offset: 0x000586AF
		private void OnEnable()
		{
			UICamera.instancesList.Add(this);
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0005A4BC File Offset: 0x000586BC
		private void OnDisable()
		{
			UICamera.instancesList.Remove(this);
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0005A4CA File Offset: 0x000586CA
		private void OnPreCull()
		{
			if (UICamera.onUICameraPreCull != null)
			{
				UICamera.onUICameraPreCull(this);
			}
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x0005A4DE File Offset: 0x000586DE
		private void OnPreRender()
		{
			if (UICamera.onUICameraPreRender != null)
			{
				UICamera.onUICameraPreRender(this);
			}
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x0005A4F2 File Offset: 0x000586F2
		private void OnPostRender()
		{
			if (UICamera.onUICameraPostRender != null)
			{
				UICamera.onUICameraPostRender(this);
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x0005A506 File Offset: 0x00058706
		public EventSystem GetAssociatedEventSystem()
		{
			if (this.cameraRigController.viewer && this.cameraRigController.viewer.localUser != null)
			{
				return this.cameraRigController.viewer.localUser.eventSystem;
			}
			return null;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0005A544 File Offset: 0x00058744
		public static UICamera FindViewerUICamera(LocalUser localUserViewer)
		{
			if (localUserViewer != null)
			{
				for (int i = 0; i < UICamera.instancesList.Count; i++)
				{
					if (UICamera.instancesList[i].cameraRigController.viewer.localUser == localUserViewer)
					{
						return UICamera.instancesList[i];
					}
				}
			}
			return null;
		}

		// Token: 0x040013C2 RID: 5058
		private static readonly List<UICamera> instancesList = new List<UICamera>();

		// Token: 0x040013C3 RID: 5059
		public static readonly ReadOnlyCollection<UICamera> readOnlyInstancesList = new ReadOnlyCollection<UICamera>(UICamera.instancesList);

		// Token: 0x02000366 RID: 870
		// (Invoke) Token: 0x06001531 RID: 5425
		public delegate void UICameraDelegate(UICamera sceneCamera);
	}
}
