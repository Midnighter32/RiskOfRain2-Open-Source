using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000280 RID: 640
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class MatchCamera : MonoBehaviour
	{
		// Token: 0x06000E30 RID: 3632 RVA: 0x0003F3C7 File Offset: 0x0003D5C7
		private void Awake()
		{
			this.destCamera = base.GetComponent<Camera>();
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x0003F3D5 File Offset: 0x0003D5D5
		private void LateUpdate()
		{
			if (this.srcCamera)
			{
				this.destCamera.rect = this.srcCamera.rect;
				this.destCamera.fieldOfView = this.srcCamera.fieldOfView;
			}
		}

		// Token: 0x04000E1D RID: 3613
		private Camera destCamera;

		// Token: 0x04000E1E RID: 3614
		public Camera srcCamera;
	}
}
