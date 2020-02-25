using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038E RID: 910
	public static class GamepadVibrationManager
	{
		// Token: 0x0600162C RID: 5676 RVA: 0x0005F68E File Offset: 0x0005D88E
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += GamepadVibrationManager.Update;
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x0005F6A4 File Offset: 0x0005D8A4
		private static void Update()
		{
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			int count = joysticks.Count;
			if (GamepadVibrationManager.motorValuesBuffer.Length < count)
			{
				Array.Resize<GamepadVibrationManager.MotorValues>(ref GamepadVibrationManager.motorValuesBuffer, count);
			}
			ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
			int count2 = readOnlyInstancesList.Count;
			if (Time.deltaTime != 0f)
			{
				for (int i = 0; i < count2; i++)
				{
					CameraRigController cameraRigController = readOnlyInstancesList[i];
					if (cameraRigController.localUserViewer != null && cameraRigController.localUserViewer.eventSystem && cameraRigController.localUserViewer.eventSystem.currentInputSource == MPEventSystem.InputSource.Gamepad)
					{
						int num = -1;
						Player inputPlayer = cameraRigController.localUserViewer.inputPlayer;
						Controller controller = (inputPlayer != null) ? inputPlayer.controllers.GetLastActiveController<Joystick>() : null;
						if (controller != null)
						{
							for (int j = 0; j < count; j++)
							{
								if (joysticks[j] == controller)
								{
									num = j;
									break;
								}
							}
						}
						if (num != -1)
						{
							UserProfile userProfile = cameraRigController.localUserViewer.userProfile;
							GamepadVibrationManager.MotorValues motorValues = GamepadVibrationManager.CalculateMotorValuesForCameraDisplacement((userProfile != null) ? userProfile.gamepadVibrationScale : 0f, cameraRigController.rawScreenShakeDisplacement);
							GamepadVibrationManager.motorValuesBuffer[num] = motorValues;
						}
					}
				}
			}
			for (int k = 0; k < count; k++)
			{
				Joystick joystick = joysticks[k];
				GamepadVibrationManager.MotorValues motorValues2 = GamepadVibrationManager.motorValuesBuffer[k];
				joystick.SetVibration(0, motorValues2.motor0);
				joystick.SetVibration(1, motorValues2.motor1);
				GamepadVibrationManager.motorValuesBuffer[k] = default(GamepadVibrationManager.MotorValues);
			}
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0005F828 File Offset: 0x0005DA28
		private static GamepadVibrationManager.MotorValues CalculateMotorValuesForCameraDisplacement(float userScale, Vector3 cameraDisplacement)
		{
			float magnitude = cameraDisplacement.magnitude;
			return new GamepadVibrationManager.MotorValues
			{
				deepMotor = magnitude * userScale / 5f,
				quickMotor = magnitude * userScale
			};
		}

		// Token: 0x040014DE RID: 5342
		private static GamepadVibrationManager.MotorValues[] motorValuesBuffer = new GamepadVibrationManager.MotorValues[4];

		// Token: 0x040014DF RID: 5343
		private const float deepRumbleFactor = 5f;

		// Token: 0x0200038F RID: 911
		private struct MotorValues
		{
			// Token: 0x17000296 RID: 662
			// (get) Token: 0x06001630 RID: 5680 RVA: 0x0005F86D File Offset: 0x0005DA6D
			// (set) Token: 0x06001631 RID: 5681 RVA: 0x0005F875 File Offset: 0x0005DA75
			public float deepMotor
			{
				get
				{
					return this.motor0;
				}
				set
				{
					this.motor0 = value;
				}
			}

			// Token: 0x17000297 RID: 663
			// (get) Token: 0x06001632 RID: 5682 RVA: 0x0005F87E File Offset: 0x0005DA7E
			// (set) Token: 0x06001633 RID: 5683 RVA: 0x0005F886 File Offset: 0x0005DA86
			public float quickMotor
			{
				get
				{
					return this.motor1;
				}
				set
				{
					this.motor1 = value;
				}
			}

			// Token: 0x040014E0 RID: 5344
			public float motor0;

			// Token: 0x040014E1 RID: 5345
			public float motor1;
		}
	}
}
