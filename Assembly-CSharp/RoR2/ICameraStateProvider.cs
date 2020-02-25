using System;

namespace RoR2
{
	// Token: 0x0200016E RID: 366
	public interface ICameraStateProvider
	{
		// Token: 0x060006D2 RID: 1746
		void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState);

		// Token: 0x060006D3 RID: 1747
		bool AllowUserLook(CameraRigController cameraRigController);
	}
}
