using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x0200040C RID: 1036
	[RequireComponent(typeof(Camera))]
	public class UICamera : MonoBehaviour
	{
		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06001715 RID: 5909 RVA: 0x0006DF68 File Offset: 0x0006C168
		// (remove) Token: 0x06001716 RID: 5910 RVA: 0x0006DF9C File Offset: 0x0006C19C
		public static event UICamera.UICameraDelegate onUICameraPreCull;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06001717 RID: 5911 RVA: 0x0006DFD0 File Offset: 0x0006C1D0
		// (remove) Token: 0x06001718 RID: 5912 RVA: 0x0006E004 File Offset: 0x0006C204
		public static event UICamera.UICameraDelegate onUICameraPreRender;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06001719 RID: 5913 RVA: 0x0006E038 File Offset: 0x0006C238
		// (remove) Token: 0x0600171A RID: 5914 RVA: 0x0006E06C File Offset: 0x0006C26C
		public static event UICamera.UICameraDelegate onUICameraPostRender;

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600171B RID: 5915 RVA: 0x0006E09F File Offset: 0x0006C29F
		// (set) Token: 0x0600171C RID: 5916 RVA: 0x0006E0A7 File Offset: 0x0006C2A7
		public Camera camera { get; private set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600171D RID: 5917 RVA: 0x0006E0B0 File Offset: 0x0006C2B0
		// (set) Token: 0x0600171E RID: 5918 RVA: 0x0006E0B8 File Offset: 0x0006C2B8
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x0600171F RID: 5919 RVA: 0x0006E0C1 File Offset: 0x0006C2C1
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0006E0DB File Offset: 0x0006C2DB
		private void OnEnable()
		{
			UICamera.instancesList.Add(this);
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0006E0E8 File Offset: 0x0006C2E8
		private void OnDisable()
		{
			UICamera.instancesList.Remove(this);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0006E0F6 File Offset: 0x0006C2F6
		private void OnPreCull()
		{
			if (UICamera.onUICameraPreCull != null)
			{
				UICamera.onUICameraPreCull(this);
			}
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x0006E10A File Offset: 0x0006C30A
		private void OnPreRender()
		{
			if (UICamera.onUICameraPreRender != null)
			{
				UICamera.onUICameraPreRender(this);
			}
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x0006E11E File Offset: 0x0006C31E
		private void OnPostRender()
		{
			if (UICamera.onUICameraPostRender != null)
			{
				UICamera.onUICameraPostRender(this);
			}
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0006E132 File Offset: 0x0006C332
		public EventSystem GetAssociatedEventSystem()
		{
			if (this.cameraRigController.viewer && this.cameraRigController.viewer.localUser != null)
			{
				return this.cameraRigController.viewer.localUser.eventSystem;
			}
			return null;
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x0006E170 File Offset: 0x0006C370
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

		// Token: 0x04001A51 RID: 6737
		private static readonly List<UICamera> instancesList = new List<UICamera>();

		// Token: 0x04001A52 RID: 6738
		public static readonly ReadOnlyCollection<UICamera> readOnlyInstancesList = new ReadOnlyCollection<UICamera>(UICamera.instancesList);

		// Token: 0x0200040D RID: 1037
		// (Invoke) Token: 0x0600172A RID: 5930
		public delegate void UICameraDelegate(UICamera sceneCamera);
	}
}
