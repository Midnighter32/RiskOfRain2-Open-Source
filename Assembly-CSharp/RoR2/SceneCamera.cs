using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C6 RID: 966
	[RequireComponent(typeof(Camera))]
	public class SceneCamera : MonoBehaviour
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x060014F7 RID: 5367 RVA: 0x00064C1C File Offset: 0x00062E1C
		// (remove) Token: 0x060014F8 RID: 5368 RVA: 0x00064C50 File Offset: 0x00062E50
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreCull;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x060014F9 RID: 5369 RVA: 0x00064C84 File Offset: 0x00062E84
		// (remove) Token: 0x060014FA RID: 5370 RVA: 0x00064CB8 File Offset: 0x00062EB8
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreRender;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x060014FB RID: 5371 RVA: 0x00064CEC File Offset: 0x00062EEC
		// (remove) Token: 0x060014FC RID: 5372 RVA: 0x00064D20 File Offset: 0x00062F20
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPostRender;

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x00064D53 File Offset: 0x00062F53
		// (set) Token: 0x060014FE RID: 5374 RVA: 0x00064D5B File Offset: 0x00062F5B
		public Camera camera { get; private set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x00064D64 File Offset: 0x00062F64
		// (set) Token: 0x06001500 RID: 5376 RVA: 0x00064D6C File Offset: 0x00062F6C
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001501 RID: 5377 RVA: 0x00064D75 File Offset: 0x00062F75
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00064D8F File Offset: 0x00062F8F
		private void OnPreCull()
		{
			if (SceneCamera.onSceneCameraPreCull != null)
			{
				SceneCamera.onSceneCameraPreCull(this);
			}
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x00064DA3 File Offset: 0x00062FA3
		private void OnPreRender()
		{
			if (SceneCamera.onSceneCameraPreRender != null)
			{
				SceneCamera.onSceneCameraPreRender(this);
			}
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00064DB7 File Offset: 0x00062FB7
		private void OnPostRender()
		{
			if (SceneCamera.onSceneCameraPostRender != null)
			{
				SceneCamera.onSceneCameraPostRender(this);
			}
		}

		// Token: 0x020003C7 RID: 967
		// (Invoke) Token: 0x06001507 RID: 5383
		public delegate void SceneCameraDelegate(SceneCamera sceneCamera);
	}
}
