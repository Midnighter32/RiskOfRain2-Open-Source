using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000356 RID: 854
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class MatchCamera : MonoBehaviour
	{
		// Token: 0x06001195 RID: 4501 RVA: 0x0005728D File Offset: 0x0005548D
		private void Awake()
		{
			this.destCamera = base.GetComponent<Camera>();
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x0005729B File Offset: 0x0005549B
		private void LateUpdate()
		{
			if (this.srcCamera)
			{
				this.destCamera.rect = this.srcCamera.rect;
				this.destCamera.fieldOfView = this.srcCamera.fieldOfView;
			}
		}

		// Token: 0x040015A0 RID: 5536
		private Camera destCamera;

		// Token: 0x040015A1 RID: 5537
		public Camera srcCamera;
	}
}
