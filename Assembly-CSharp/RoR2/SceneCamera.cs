using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x02000313 RID: 787
	[RequireComponent(typeof(Camera))]
	public class SceneCamera : MonoBehaviour
	{
		// Token: 0x14000039 RID: 57
		// (add) Token: 0x06001279 RID: 4729 RVA: 0x0004F848 File Offset: 0x0004DA48
		// (remove) Token: 0x0600127A RID: 4730 RVA: 0x0004F87C File Offset: 0x0004DA7C
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreCull;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x0600127B RID: 4731 RVA: 0x0004F8B0 File Offset: 0x0004DAB0
		// (remove) Token: 0x0600127C RID: 4732 RVA: 0x0004F8E4 File Offset: 0x0004DAE4
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreRender;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x0600127D RID: 4733 RVA: 0x0004F918 File Offset: 0x0004DB18
		// (remove) Token: 0x0600127E RID: 4734 RVA: 0x0004F94C File Offset: 0x0004DB4C
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPostRender;

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x0600127F RID: 4735 RVA: 0x0004F97F File Offset: 0x0004DB7F
		// (set) Token: 0x06001280 RID: 4736 RVA: 0x0004F987 File Offset: 0x0004DB87
		public Camera camera { get; private set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06001281 RID: 4737 RVA: 0x0004F990 File Offset: 0x0004DB90
		// (set) Token: 0x06001282 RID: 4738 RVA: 0x0004F998 File Offset: 0x0004DB98
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001283 RID: 4739 RVA: 0x0004F9A1 File Offset: 0x0004DBA1
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x0004F9BB File Offset: 0x0004DBBB
		private void OnPreCull()
		{
			if (SceneCamera.onSceneCameraPreCull != null)
			{
				SceneCamera.onSceneCameraPreCull(this);
			}
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0004F9CF File Offset: 0x0004DBCF
		private void OnPreRender()
		{
			if (SceneCamera.onSceneCameraPreRender != null)
			{
				this.camera.opaqueSortMode = this.sortMode;
				SceneCamera.onSceneCameraPreRender(this);
			}
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0004F9F4 File Offset: 0x0004DBF4
		private void OnPostRender()
		{
			if (SceneCamera.onSceneCameraPostRender != null)
			{
				SceneCamera.onSceneCameraPostRender(this);
			}
		}

		// Token: 0x04001171 RID: 4465
		public OpaqueSortMode sortMode = OpaqueSortMode.NoDistanceSort;

		// Token: 0x02000314 RID: 788
		// (Invoke) Token: 0x06001289 RID: 4745
		public delegate void SceneCameraDelegate(SceneCamera sceneCamera);
	}
}
