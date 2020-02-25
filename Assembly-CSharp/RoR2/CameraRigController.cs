using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Rewired;
using RoR2.ConVar;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000170 RID: 368
	public class CameraRigController : MonoBehaviour
	{
		// Token: 0x060006D6 RID: 1750 RVA: 0x0001BE68 File Offset: 0x0001A068
		public CameraState GetDesiredCameraState()
		{
			return this.desiredCameraState;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0001BE70 File Offset: 0x0001A070
		private void StartStateLerp(float lerpDuration)
		{
			this.lerpCameraState = this.currentCameraState;
			if (lerpDuration > 0f)
			{
				this.lerpCameraTime = 0f;
				this.lerpCameraTimeScale = 1f / lerpDuration;
				return;
			}
			this.lerpCameraTime = 1f;
			this.lerpCameraTimeScale = 0f;
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x0001BEC0 File Offset: 0x0001A0C0
		public Vector3 desiredPosition
		{
			get
			{
				return this.desiredCameraState.position;
			}
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0001BECD File Offset: 0x0001A0CD
		public void SetOverrideCam(ICameraStateProvider newOverrideCam, float lerpDuration = 1f)
		{
			if (newOverrideCam == this.overrideCam)
			{
				return;
			}
			if (this.overrideCam != null && newOverrideCam == null)
			{
				this.SetPitchYawFromLookVector(this.currentCameraState.rotation * Vector3.forward);
			}
			this.overrideCam = newOverrideCam;
			this.StartStateLerp(lerpDuration);
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x0001BF0D File Offset: 0x0001A10D
		public bool IsOverrideCam(ICameraStateProvider testOverrideCam)
		{
			return this.overrideCam == testOverrideCam;
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x0001BF18 File Offset: 0x0001A118
		public void SetPitchYawFromLookVector(Vector3 lookVector)
		{
			float x = Mathf.Sqrt(lookVector.x * lookVector.x + lookVector.z * lookVector.z);
			this.pitch = Mathf.Atan2(-lookVector.y, x) * 57.29578f;
			this.yaw = Mathf.Repeat(Mathf.Atan2(lookVector.x, lookVector.z) * 57.29578f, 360f);
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0001BF88 File Offset: 0x0001A188
		private void SetPitchYaw(PitchYawPair pitchYawPair)
		{
			this.pitch = pitchYawPair.pitch;
			this.yaw = pitchYawPair.yaw;
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x0001BFA2 File Offset: 0x0001A1A2
		// (set) Token: 0x060006DE RID: 1758 RVA: 0x0001BFAA File Offset: 0x0001A1AA
		public NetworkUser viewer
		{
			get
			{
				return this._viewer;
			}
			set
			{
				this._viewer = value;
				this.localUserViewer = (this._viewer ? this._viewer.localUser : null);
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x0001BFD4 File Offset: 0x0001A1D4
		// (set) Token: 0x060006E0 RID: 1760 RVA: 0x0001BFDC File Offset: 0x0001A1DC
		public LocalUser localUserViewer
		{
			get
			{
				return this._localUserViewer;
			}
			private set
			{
				if (this._localUserViewer == value)
				{
					return;
				}
				if (this._localUserViewer != null)
				{
					this._localUserViewer.cameraRigController = null;
				}
				this._localUserViewer = value;
				if (this._localUserViewer != null)
				{
					this._localUserViewer.cameraRigController = this;
				}
				if (this.hud)
				{
					this.hud.localUserViewer = this._localUserViewer;
				}
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x0001C040 File Offset: 0x0001A240
		// (set) Token: 0x060006E2 RID: 1762 RVA: 0x0001C048 File Offset: 0x0001A248
		public GameObject firstPersonTarget { get; private set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060006E3 RID: 1763 RVA: 0x0001C051 File Offset: 0x0001A251
		// (set) Token: 0x060006E4 RID: 1764 RVA: 0x0001C059 File Offset: 0x0001A259
		public TeamIndex targetTeamIndex { get; private set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060006E5 RID: 1765 RVA: 0x0001C062 File Offset: 0x0001A262
		// (set) Token: 0x060006E6 RID: 1766 RVA: 0x0001C06A File Offset: 0x0001A26A
		public Vector3 crosshairWorldPosition { get; private set; }

		// Token: 0x060006E7 RID: 1767 RVA: 0x0001C073 File Offset: 0x0001A273
		private static bool CanUserSpectateBody(NetworkUser viewer, CharacterBody body)
		{
			return Util.LookUpBodyNetworkUser(body.gameObject);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0001C088 File Offset: 0x0001A288
		public static GameObject GetNextSpectateGameObject(NetworkUser viewer, GameObject currentGameObject)
		{
			ReadOnlyCollection<CharacterBody> readOnlyCollection = CharacterBody.readOnlyInstancesList;
			if (readOnlyCollection.Count == 0)
			{
				return null;
			}
			CharacterBody characterBody = currentGameObject ? currentGameObject.GetComponent<CharacterBody>() : null;
			int num = characterBody ? readOnlyCollection.IndexOf(characterBody) : 0;
			for (int i = num + 1; i < readOnlyCollection.Count; i++)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[i]))
				{
					return readOnlyCollection[i].gameObject;
				}
			}
			for (int j = 0; j <= num; j++)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[j]))
				{
					return readOnlyCollection[j].gameObject;
				}
			}
			return null;
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0001C128 File Offset: 0x0001A328
		public static GameObject GetPreviousSpectateGameObject(NetworkUser viewer, GameObject currentGameObject)
		{
			ReadOnlyCollection<CharacterBody> readOnlyCollection = CharacterBody.readOnlyInstancesList;
			if (readOnlyCollection.Count == 0)
			{
				return null;
			}
			CharacterBody characterBody = currentGameObject ? currentGameObject.GetComponent<CharacterBody>() : null;
			int num = characterBody ? readOnlyCollection.IndexOf(characterBody) : 0;
			for (int i = num - 1; i >= 0; i--)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[i]))
				{
					return readOnlyCollection[i].gameObject;
				}
			}
			for (int j = readOnlyCollection.Count - 1; j >= num; j--)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[j]))
				{
					return readOnlyCollection[j].gameObject;
				}
			}
			return null;
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0001C1CC File Offset: 0x0001A3CC
		private void Start()
		{
			if (this.createHud)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/HUDSimple"));
				this.hud = gameObject.GetComponent<HUD>();
				this.hud.cameraRigController = this;
				this.hud.GetComponent<Canvas>().worldCamera = this.uiCam;
				this.hud.GetComponent<CrosshairManager>().cameraRigController = this;
				this.hud.localUserViewer = this.localUserViewer;
			}
			this.currentFov = this.baseFov;
			if (this.uiCam)
			{
				this.uiCam.transform.parent = null;
				this.uiCam.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
			this.desiredCameraState = new CameraState
			{
				position = base.transform.position,
				rotation = base.transform.rotation,
				fov = this.currentFov
			};
			if (!DamageNumberManager.instance)
			{
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/DamageNumberManager"));
			}
			this.currentCameraState = this.desiredCameraState;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0001C2F0 File Offset: 0x0001A4F0
		private void Update()
		{
			if (Time.deltaTime == 0f)
			{
				return;
			}
			if (this.target != this.previousTarget)
			{
				this.previousTarget = this.target;
				Action<CameraRigController, GameObject> action = CameraRigController.onCameraTargetChanged;
				if (action != null)
				{
					action(this, this.target);
				}
			}
			this.lerpCameraTime += Time.deltaTime * this.lerpCameraTimeScale;
			this.firstPersonTarget = null;
			float num = this.baseFov;
			this.sceneCam.rect = this.viewport;
			Player player = null;
			UserProfile userProfile = null;
			bool flag = false;
			if (this.viewer && this.viewer.localUser != null)
			{
				player = this.viewer.localUser.inputPlayer;
				userProfile = this.viewer.localUser.userProfile;
				flag = this.viewer.localUser.isUIFocused;
			}
			if (this.cameraMode == CameraRigController.CameraMode.SpectateUser && player != null)
			{
				if (player.GetButtonDown("PrimarySkill"))
				{
					this.target = CameraRigController.GetNextSpectateGameObject(this.viewer, this.target);
				}
				if (player.GetButtonDown("SecondarySkill"))
				{
					this.target = CameraRigController.GetPreviousSpectateGameObject(this.viewer, this.target);
				}
			}
			LocalUser localUserViewer = this.localUserViewer;
			MPEventSystem mpeventSystem = (localUserViewer != null) ? localUserViewer.eventSystem : null;
			float num14;
			float num15;
			if ((!mpeventSystem || !mpeventSystem.isCursorVisible) && player != null && userProfile != null && !flag && (!(UnityEngine.Object)this.overrideCam || this.overrideCam.AllowUserLook(this)))
			{
				float mouseLookSensitivity = userProfile.mouseLookSensitivity;
				float num2 = userProfile.stickLookSensitivity * CameraRigController.aimStickGlobalScale.value * 45f;
				Vector2 vector = new Vector2(player.GetAxisRaw(2), player.GetAxisRaw(3));
				Vector2 vector2 = new Vector2(player.GetAxisRaw(16), player.GetAxisRaw(17));
				CameraRigController.<Update>g__ConditionalNegate|70_0(ref vector.x, userProfile.mouseLookInvertX);
				CameraRigController.<Update>g__ConditionalNegate|70_0(ref vector.y, userProfile.mouseLookInvertY);
				CameraRigController.<Update>g__ConditionalNegate|70_0(ref vector2.x, userProfile.stickLookInvertX);
				CameraRigController.<Update>g__ConditionalNegate|70_0(ref vector2.y, userProfile.stickLookInvertY);
				float magnitude = vector2.magnitude;
				float num3 = magnitude;
				this.aimStickPostSmoothing = Vector2.zero;
				this.aimStickPostDualZone = Vector2.zero;
				this.aimStickPostExponent = Vector2.zero;
				if (CameraRigController.aimStickDualZoneSmoothing.value != 0f)
				{
					float maxDelta = Time.deltaTime / CameraRigController.aimStickDualZoneSmoothing.value;
					num3 = Mathf.Min(Mathf.MoveTowards(this.stickAimPreviousAcceleratedMagnitude, magnitude, maxDelta), magnitude);
					this.stickAimPreviousAcceleratedMagnitude = num3;
					this.aimStickPostSmoothing = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				}
				float num4 = num3;
				float value = CameraRigController.aimStickDualZoneSlope.value;
				float num5;
				if (num4 <= CameraRigController.aimStickDualZoneThreshold.value)
				{
					num5 = 0f;
				}
				else
				{
					num5 = 1f - value;
				}
				num3 = value * num4 + num5;
				this.aimStickPostDualZone = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				num3 = Mathf.Pow(num3, CameraRigController.aimStickExponent.value);
				this.aimStickPostExponent = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				if (magnitude != 0f)
				{
					vector2 *= num3 / magnitude;
				}
				if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic && this.targetBody && !this.targetBody.isSprinting)
				{
					AimAssistTarget exists = null;
					AimAssistTarget exists2 = null;
					float value2 = CameraRigController.aimStickAssistMinSize.value;
					float num6 = value2 * CameraRigController.aimStickAssistMaxSize.value;
					float value3 = CameraRigController.aimStickAssistMaxSlowdownScale.value;
					float value4 = CameraRigController.aimStickAssistMinSlowdownScale.value;
					float num7 = 0f;
					float value5 = 0f;
					float num8 = 0f;
					Vector2 v = Vector2.zero;
					Vector2 zero = Vector2.zero;
					Vector2 normalized = vector2.normalized;
					Vector2 vector3 = new Vector2(0.5f, 0.5f);
					for (int i = 0; i < AimAssistTarget.instancesList.Count; i++)
					{
						AimAssistTarget aimAssistTarget = AimAssistTarget.instancesList[i];
						if (aimAssistTarget.teamComponent.teamIndex != this.targetTeamIndex)
						{
							Vector3 vector4 = this.sceneCam.WorldToViewportPoint(aimAssistTarget.point0.position);
							Vector3 vector5 = this.sceneCam.WorldToViewportPoint(aimAssistTarget.point1.position);
							float num9 = Mathf.Lerp(vector4.z, vector5.z, 0.5f);
							if (num9 > 3f)
							{
								float num10 = 1f / num9;
								Vector2 vector6 = Util.ClosestPointOnLine(vector4, vector5, vector3) - vector3;
								float num11 = Mathf.Clamp01(Util.Remap(vector6.magnitude, value2 * aimAssistTarget.assistScale * num10, num6 * aimAssistTarget.assistScale * num10, 1f, 0f));
								float num12 = Mathf.Clamp01(Vector3.Dot(vector6, vector2.normalized));
								float num13 = num12 * num11;
								if (num7 < num11)
								{
									num7 = num11;
									exists2 = aimAssistTarget;
								}
								if (num13 > num8)
								{
									num7 = num11;
									value5 = num12;
									exists = aimAssistTarget;
									v = vector6;
								}
							}
						}
					}
					Vector2 vector7 = vector2;
					if (exists2)
					{
						float magnitude2 = vector2.magnitude;
						float d = Mathf.Clamp01(Util.Remap(1f - num7, 0f, 1f, value3, value4));
						vector7 *= d;
					}
					if (exists)
					{
						vector7 = Vector3.RotateTowards(vector7, v, Util.Remap(value5, 1f, 0f, CameraRigController.aimStickAssistMaxDelta.value, CameraRigController.aimStickAssistMinDelta.value), 0f);
					}
					vector2 = vector7;
				}
				num14 = vector.x * mouseLookSensitivity * userProfile.mouseLookScaleX + vector2.x * num2 * userProfile.stickLookScaleX * Time.deltaTime;
				num15 = vector.y * mouseLookSensitivity * userProfile.mouseLookScaleY + vector2.y * num2 * userProfile.stickLookScaleY * Time.deltaTime;
			}
			else
			{
				num14 = 0f;
				num15 = 0f;
			}
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(this.target);
			NetworkedViewAngles networkedViewAngles = null;
			if (networkUser)
			{
				networkedViewAngles = networkUser.GetComponent<NetworkedViewAngles>();
			}
			this.targetTeamIndex = TeamIndex.None;
			bool flag2 = false;
			this.targetParams = null;
			if (this.target)
			{
				this.targetBody = this.target.GetComponent<CharacterBody>();
				if (this.targetBody)
				{
					flag2 = this.targetBody.isSprinting;
					if (this.targetBody.currentVehicle)
					{
						this.targetParams = this.targetBody.currentVehicle.GetComponent<CameraTargetParams>();
					}
				}
				if (!this.targetParams)
				{
					this.targetParams = this.target.GetComponent<CameraTargetParams>();
				}
				TeamComponent component = this.target.GetComponent<TeamComponent>();
				if (component)
				{
					this.targetTeamIndex = component.teamIndex;
				}
			}
			Vector3 vector8 = this.desiredCameraState.position;
			if (this.targetParams)
			{
				Vector3 position = this.target.transform.position;
				Vector3 cameraPivotPosition = this.targetParams.cameraPivotPosition;
				if (this.targetParams.dontRaycastToPivot)
				{
					vector8 = cameraPivotPosition;
				}
				else
				{
					Vector3 direction = cameraPivotPosition - position;
					Ray ray = new Ray(position, direction);
					float distance = this.Raycast(ray, direction.magnitude, this.targetParams.cameraParams.wallCushion);
					vector8 = ray.GetPoint(distance);
				}
			}
			if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic || this.cameraMode == CameraRigController.CameraMode.SpectateUser)
			{
				float min = -89.9f;
				float max = 89.9f;
				Vector3 idealLocalCameraPos = new Vector3(0f, 0f, 0f);
				float wallCushion = 0.1f;
				Vector2 vector9 = Vector2.zero;
				if (this.targetParams)
				{
					min = this.targetParams.cameraParams.minPitch;
					max = this.targetParams.cameraParams.maxPitch;
					idealLocalCameraPos = this.targetParams.idealLocalCameraPos;
					wallCushion = this.targetParams.cameraParams.wallCushion;
					vector9 = this.targetParams.recoil;
					if (this.targetParams.aimMode == CameraTargetParams.AimType.FirstPerson)
					{
						this.firstPersonTarget = this.target;
					}
					if (this.targetParams.fovOverride >= 0f)
					{
						num = this.targetParams.fovOverride;
						num14 *= num / this.baseFov;
						num15 *= num / this.baseFov;
					}
					if (this.targetBody && flag2 && CameraRigController.enableSprintSensitivitySlowdown.value)
					{
						num14 *= 0.5f;
						num15 *= 0.5f;
					}
				}
				if (this.sprintingParticleSystem)
				{
					ParticleSystem.MainModule main = this.sprintingParticleSystem.main;
					if (flag2)
					{
						main.loop = true;
						if (!this.sprintingParticleSystem.isPlaying)
						{
							this.sprintingParticleSystem.Play();
						}
					}
					else
					{
						main.loop = false;
					}
				}
				if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic)
				{
					float num16 = this.pitch - num15;
					float num17 = this.yaw + num14;
					num16 += vector9.y;
					num17 += vector9.x;
					this.pitch = Mathf.Clamp(num16, min, max);
					this.yaw = Mathf.Repeat(num17, 360f);
				}
				else if (this.cameraMode == CameraRigController.CameraMode.SpectateUser && this.target)
				{
					if (networkedViewAngles)
					{
						this.SetPitchYaw(networkedViewAngles.viewAngles);
					}
					else
					{
						InputBankTest component2 = this.target.GetComponent<InputBankTest>();
						if (component2)
						{
							this.SetPitchYawFromLookVector(component2.aimDirection);
						}
					}
				}
				this.desiredCameraState.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
				Vector3 direction2 = vector8 + this.desiredCameraState.rotation * idealLocalCameraPos - vector8;
				float num18 = direction2.magnitude;
				float num19 = (1f + this.pitch / -90f) * 0.5f;
				num18 *= Mathf.Sqrt(1f - num19);
				if (num18 < 0.25f)
				{
					num18 = 0.25f;
				}
				float a = this.Raycast(new Ray(vector8, direction2), num18, wallCushion);
				this.currentCameraDistance = Mathf.Min(a, Mathf.SmoothDamp(this.currentCameraDistance, a, ref this.cameraDistanceVelocity, 0.5f));
				this.desiredCameraState.position = vector8 + direction2.normalized * this.currentCameraDistance;
				this.pitch -= vector9.y;
				this.yaw -= vector9.x;
				if (networkedViewAngles && networkedViewAngles.hasEffectiveAuthority)
				{
					networkedViewAngles.viewAngles = new PitchYawPair(this.pitch, this.yaw);
				}
			}
			if (this.targetBody)
			{
				num *= (this.targetBody.isSprinting ? 1.3f : 1f);
			}
			this.desiredCameraState.fov = Mathf.SmoothDamp(this.desiredCameraState.fov, num, ref this.fovVelocity, 0.2f, float.PositiveInfinity, Time.deltaTime);
			if (this.hud)
			{
				CharacterMaster targetMaster = this.targetBody ? this.targetBody.master : null;
				this.hud.targetMaster = targetMaster;
			}
			this.UpdateCrosshair(vector8);
			CameraState cameraState = this.desiredCameraState;
			if (this.overrideCam != null)
			{
				if ((UnityEngine.Object)this.overrideCam)
				{
					this.overrideCam.GetCameraState(this, ref cameraState);
				}
				this.overrideCam = null;
			}
			if (this.lerpCameraTime >= 1f)
			{
				this.currentCameraState = cameraState;
			}
			else
			{
				this.currentCameraState = CameraState.Lerp(ref this.lerpCameraState, ref cameraState, CameraRigController.RemapLerpTime(this.lerpCameraTime));
			}
			this.SetCameraState(this.currentCameraState);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0001CF00 File Offset: 0x0001B100
		private float Raycast(Ray ray, float maxDistance, float wallCushion)
		{
			RaycastHit[] array = Physics.SphereCastAll(ray, wallCushion, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
			float num = maxDistance;
			for (int i = 0; i < array.Length; i++)
			{
				float distance = array[i].distance;
				if (distance < num)
				{
					Collider collider = array[i].collider;
					if (collider && !collider.GetComponent<NonSolidToCamera>())
					{
						num = distance;
					}
				}
			}
			return num;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0001CF78 File Offset: 0x0001B178
		private static float RemapLerpTime(float t)
		{
			float num = 1f;
			float num2 = 0f;
			float num3 = 1f;
			if ((t /= num / 2f) < 1f)
			{
				return num3 / 2f * t * t + num2;
			}
			return -num3 / 2f * ((t -= 1f) * (t - 2f) - 1f) + num2;
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0001CFDB File Offset: 0x0001B1DB
		public bool hasOverride
		{
			get
			{
				return this.overrideCam != null;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x0001CFE6 File Offset: 0x0001B1E6
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x0001CFEE File Offset: 0x0001B1EE
		public Vector3 rawScreenShakeDisplacement { get; private set; }

		// Token: 0x060006F1 RID: 1777 RVA: 0x0001CFF8 File Offset: 0x0001B1F8
		private void SetCameraState(CameraState cameraState)
		{
			this.currentCameraState = cameraState;
			float d = (this.localUserViewer == null) ? 1f : this.localUserViewer.userProfile.screenShakeScale;
			Vector3 position = cameraState.position;
			this.rawScreenShakeDisplacement = ShakeEmitter.ComputeTotalShakeAtPoint(cameraState.position);
			Vector3 vector = this.rawScreenShakeDisplacement * d;
			Vector3 position2 = position + vector;
			if (vector != Vector3.zero)
			{
				Vector3 origin = position;
				Vector3 direction = vector;
				RaycastHit raycastHit;
				if (Physics.SphereCast(origin, this.sceneCam.nearClipPlane, direction, out raycastHit, vector.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					position2 = position + vector.normalized * raycastHit.distance;
				}
			}
			base.transform.SetPositionAndRotation(position2, cameraState.rotation);
			this.currentFov = cameraState.fov;
			if (this.sceneCam)
			{
				this.sceneCam.fieldOfView = this.currentFov;
			}
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0001D0F4 File Offset: 0x0001B2F4
		private void UpdateCrosshair(Vector3 raycastStartPlanePoint)
		{
			this.lastAimAssist = this.aimAssist;
			Vector2 zero = Vector2.zero;
			Ray crosshairRaycastRay = this.GetCrosshairRaycastRay(zero, raycastStartPlanePoint);
			bool flag = false;
			this.lastCrosshairHurtBox = null;
			RaycastHit raycastHit = default(RaycastHit);
			RaycastHit[] array = Physics.RaycastAll(crosshairRaycastRay, this.maxAimRaycastDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
			float num = float.PositiveInfinity;
			int num2 = -1;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit2 = array[i];
				HurtBox hurtBox = raycastHit2.collider.GetComponent<HurtBox>();
				EntityLocator component = raycastHit2.collider.GetComponent<EntityLocator>();
				float distance = raycastHit2.distance;
				if (distance > 3f && num > distance)
				{
					if (hurtBox)
					{
						if (hurtBox.teamIndex == this.targetTeamIndex)
						{
							goto IL_145;
						}
						if (hurtBox.healthComponent && hurtBox.healthComponent.dontShowHealthbar)
						{
							hurtBox = null;
						}
					}
					if (component)
					{
						VehicleSeat vehicleSeat = component.entity ? component.entity.GetComponent<VehicleSeat>() : null;
						if (vehicleSeat && vehicleSeat.currentPassengerBody == this.targetBody)
						{
							goto IL_145;
						}
					}
					num = distance;
					num2 = i;
					this.lastCrosshairHurtBox = hurtBox;
				}
				IL_145:;
			}
			if (num2 != -1)
			{
				flag = true;
				raycastHit = array[num2];
			}
			this.aimAssist.aimAssistHurtbox = null;
			if (flag)
			{
				this.crosshairWorldPosition = raycastHit.point;
				float num3 = 1000f;
				if (raycastHit.distance < num3)
				{
					HurtBox component2 = raycastHit.collider.GetComponent<HurtBox>();
					if (component2)
					{
						HealthComponent healthComponent = component2.healthComponent;
						if (healthComponent)
						{
							TeamComponent component3 = healthComponent.GetComponent<TeamComponent>();
							if (component3 && component3.teamIndex != this.targetTeamIndex && component3.teamIndex != TeamIndex.None)
							{
								CharacterBody body = healthComponent.body;
								HurtBox hurtBox2 = (body != null) ? body.mainHurtBox : null;
								if (hurtBox2)
								{
									this.aimAssist.aimAssistHurtbox = hurtBox2;
									this.aimAssist.worldPosition = raycastHit.point;
									this.aimAssist.localPositionOnHurtbox = hurtBox2.transform.InverseTransformPoint(raycastHit.point);
									return;
								}
							}
						}
					}
				}
			}
			else
			{
				this.crosshairWorldPosition = crosshairRaycastRay.GetPoint(this.maxAimRaycastDistance);
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0001D368 File Offset: 0x0001B568
		public static Ray ModifyAimRayIfApplicable(Ray originalAimRay, GameObject target, out float extraRaycastDistance)
		{
			CameraRigController cameraRigController = null;
			foreach (CameraRigController cameraRigController2 in CameraRigController.readOnlyInstancesList)
			{
				if (cameraRigController2.target == target && cameraRigController2._localUserViewer.cachedBodyObject == target)
				{
					cameraRigController = cameraRigController2;
					break;
				}
			}
			if (cameraRigController)
			{
				Camera camera = cameraRigController.sceneCam;
				extraRaycastDistance = (originalAimRay.origin - camera.transform.position).magnitude;
				return camera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
			}
			extraRaycastDistance = 0f;
			return originalAimRay;
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0001D428 File Offset: 0x0001B628
		private Ray GetCrosshairRaycastRay(Vector2 crosshairOffset, Vector3 raycastStartPlanePoint)
		{
			if (!this.sceneCam)
			{
				return default(Ray);
			}
			float fieldOfView = this.sceneCam.fieldOfView;
			float num = fieldOfView * this.sceneCam.aspect;
			Quaternion quaternion = Quaternion.Euler(crosshairOffset.y * fieldOfView, crosshairOffset.x * num, 0f);
			quaternion = this.desiredCameraState.rotation * quaternion;
			return new Ray(Vector3.ProjectOnPlane(this.desiredCameraState.position - raycastStartPlanePoint, this.desiredCameraState.rotation * Vector3.forward) + raycastStartPlanePoint, quaternion * Vector3.forward);
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0001D4D4 File Offset: 0x0001B6D4
		private void OnEnable()
		{
			CameraRigController.instancesList.Add(this);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0001D4E1 File Offset: 0x0001B6E1
		private void OnDisable()
		{
			CameraRigController.instancesList.Remove(this);
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0001D4F0 File Offset: 0x0001B6F0
		private void OnDestroy()
		{
			if (this.uiCam)
			{
				UnityEngine.Object.Destroy(this.uiCam.gameObject);
			}
			if (this.hud)
			{
				UnityEngine.Object.Destroy(this.hud.gameObject);
			}
			LocalUser localUserViewer = this.localUserViewer;
			Player player = (localUserViewer != null) ? localUserViewer.inputPlayer : null;
			if (player != null)
			{
				player.SetVibration(0, 0f);
				player.SetVibration(1, 0f);
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0001D568 File Offset: 0x0001B768
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SceneCamera.onSceneCameraPreCull += delegate(SceneCamera sceneCam)
			{
				sceneCam.cameraRigController.sprintingParticleSystem.gameObject.layer = LayerIndex.defaultLayer.intVal;
			};
			SceneCamera.onSceneCameraPostRender += delegate(SceneCamera sceneCam)
			{
				sceneCam.cameraRigController.sprintingParticleSystem.gameObject.layer = LayerIndex.noDraw.intVal;
			};
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0001D5C0 File Offset: 0x0001B7C0
		public static bool IsObjectSpectatedByAnyCamera(GameObject gameObject)
		{
			for (int i = 0; i < CameraRigController.instancesList.Count; i++)
			{
				if (CameraRigController.instancesList[i].target == gameObject)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060006FA RID: 1786 RVA: 0x0001D600 File Offset: 0x0001B800
		// (remove) Token: 0x060006FB RID: 1787 RVA: 0x0001D634 File Offset: 0x0001B834
		public static event Action<CameraRigController, GameObject> onCameraTargetChanged;

		// Token: 0x060006FE RID: 1790 RVA: 0x0001D864 File Offset: 0x0001BA64
		[CompilerGenerated]
		internal static void <Update>g__ConditionalNegate|70_0(ref float value, bool condition)
		{
			value = (condition ? (-value) : value);
		}

		// Token: 0x04000721 RID: 1825
		[Tooltip("The main camera for rendering the scene.")]
		public Camera sceneCam;

		// Token: 0x04000722 RID: 1826
		[Tooltip("The UI camera.")]
		public Camera uiCam;

		// Token: 0x04000723 RID: 1827
		[Tooltip("The skybox camera.")]
		public Camera skyboxCam;

		// Token: 0x04000724 RID: 1828
		public ParticleSystem sprintingParticleSystem;

		// Token: 0x04000725 RID: 1829
		public float baseFov = 60f;

		// Token: 0x04000726 RID: 1830
		private float currentFov;

		// Token: 0x04000727 RID: 1831
		private float fovVelocity;

		// Token: 0x04000728 RID: 1832
		public float fadeStartDistance = 1f;

		// Token: 0x04000729 RID: 1833
		public float fadeEndDistance = 4f;

		// Token: 0x0400072A RID: 1834
		public bool disableSpectating;

		// Token: 0x0400072B RID: 1835
		[Tooltip("The maximum distance of the raycast used to determine the aim vector.")]
		public float maxAimRaycastDistance = 1000f;

		// Token: 0x0400072C RID: 1836
		private CameraState desiredCameraState;

		// Token: 0x0400072D RID: 1837
		private CameraState currentCameraState;

		// Token: 0x0400072E RID: 1838
		private CameraState lerpCameraState;

		// Token: 0x0400072F RID: 1839
		private float lerpCameraTime = 1f;

		// Token: 0x04000730 RID: 1840
		private float lerpCameraTimeScale = 1f;

		// Token: 0x04000731 RID: 1841
		private Vector3 cameraStateVelocityPosition;

		// Token: 0x04000732 RID: 1842
		private float cameraStateVelocityAngle;

		// Token: 0x04000733 RID: 1843
		private float cameraStateVelocityFov;

		// Token: 0x04000734 RID: 1844
		private ICameraStateProvider overrideCam;

		// Token: 0x04000735 RID: 1845
		public CameraRigController.CameraMode cameraMode = CameraRigController.CameraMode.PlayerBasic;

		// Token: 0x04000736 RID: 1846
		private NetworkUser _viewer;

		// Token: 0x04000737 RID: 1847
		private LocalUser _localUserViewer;

		// Token: 0x04000738 RID: 1848
		public Rect viewport = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04000739 RID: 1849
		public HUD hud;

		// Token: 0x0400073A RID: 1850
		private GameObject previousTarget;

		// Token: 0x0400073B RID: 1851
		public GameObject target;

		// Token: 0x0400073C RID: 1852
		private CharacterBody targetBody;

		// Token: 0x0400073E RID: 1854
		[Tooltip("Whether or not to create a HUD.")]
		public bool createHud = true;

		// Token: 0x04000740 RID: 1856
		private CameraTargetParams targetParams;

		// Token: 0x04000741 RID: 1857
		private float pitch;

		// Token: 0x04000742 RID: 1858
		private float yaw;

		// Token: 0x04000744 RID: 1860
		private float currentCameraDistance;

		// Token: 0x04000745 RID: 1861
		private float cameraDistanceVelocity;

		// Token: 0x04000746 RID: 1862
		private Vector2 aimStickVelocity;

		// Token: 0x04000747 RID: 1863
		private float stickAimPreviousAcceleratedMagnitude;

		// Token: 0x04000748 RID: 1864
		public Vector2 aimStickPostSmoothing;

		// Token: 0x04000749 RID: 1865
		public Vector2 aimStickPostDualZone;

		// Token: 0x0400074A RID: 1866
		public Vector2 aimStickPostExponent;

		// Token: 0x0400074C RID: 1868
		public CameraRigController.AimAssistInfo lastAimAssist;

		// Token: 0x0400074D RID: 1869
		public CameraRigController.AimAssistInfo aimAssist;

		// Token: 0x0400074E RID: 1870
		public HurtBox lastCrosshairHurtBox;

		// Token: 0x0400074F RID: 1871
		private static List<CameraRigController> instancesList = new List<CameraRigController>();

		// Token: 0x04000750 RID: 1872
		public static readonly ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.instancesList.AsReadOnly();

		// Token: 0x04000752 RID: 1874
		private static FloatConVar aimStickExponent = new FloatConVar("aim_stick_exponent", ConVarFlags.None, "1", "The exponent for stick input used for aiming.");

		// Token: 0x04000753 RID: 1875
		private static FloatConVar aimStickDualZoneThreshold = new FloatConVar("aim_stick_dual_zone_threshold", ConVarFlags.None, "0.90", "The threshold for stick dual zone behavior.");

		// Token: 0x04000754 RID: 1876
		private static FloatConVar aimStickDualZoneSlope = new FloatConVar("aim_stick_dual_zone_slope", ConVarFlags.None, "0.40", "The slope value for stick dual zone behavior.");

		// Token: 0x04000755 RID: 1877
		private static FloatConVar aimStickDualZoneSmoothing = new FloatConVar("aim_stick_smoothing", ConVarFlags.None, "0.05", "The smoothing value for stick aiming.");

		// Token: 0x04000756 RID: 1878
		private static FloatConVar aimStickGlobalScale = new FloatConVar("aim_stick_global_scale", ConVarFlags.Archive, "1.00", "The global sensitivity scale for stick aiming.");

		// Token: 0x04000757 RID: 1879
		private static FloatConVar aimStickAssistMinSlowdownScale = new FloatConVar("aim_stick_assist_min_slowdown_scale", ConVarFlags.None, "1", "The MAX amount the sensitivity scales down when passing over an enemy.");

		// Token: 0x04000758 RID: 1880
		private static FloatConVar aimStickAssistMaxSlowdownScale = new FloatConVar("aim_stick_assist_max_slowdown_scale", ConVarFlags.None, "0.4", "The MAX amount the sensitivity scales down when passing over an enemy.");

		// Token: 0x04000759 RID: 1881
		private static FloatConVar aimStickAssistMinDelta = new FloatConVar("aim_stick_assist_min_delta", ConVarFlags.None, "0", "The MIN amount in radians the aim assist will turn towards");

		// Token: 0x0400075A RID: 1882
		private static FloatConVar aimStickAssistMaxDelta = new FloatConVar("aim_stick_assist_max_delta", ConVarFlags.None, "1.57", "The MAX amount in radians the aim assist will turn towards");

		// Token: 0x0400075B RID: 1883
		private static FloatConVar aimStickAssistMaxInputHelp = new FloatConVar("aim_stick_assist_max_input_help", ConVarFlags.None, "0.2", "The amount, from 0-1, that the aim assist will actually ADD magnitude towards. Helps you keep target while strafing. CURRENTLY UNUSED.");

		// Token: 0x0400075C RID: 1884
		public static FloatConVar aimStickAssistMaxSize = new FloatConVar("aim_stick_assist_max_size", ConVarFlags.None, "3", "The size, as a coefficient, of the aim assist 'white' zone.");

		// Token: 0x0400075D RID: 1885
		public static FloatConVar aimStickAssistMinSize = new FloatConVar("aim_stick_assist_min_size", ConVarFlags.None, "1", "The minimum size, as a percentage of the GUI, of the aim assist 'red' zone.");

		// Token: 0x0400075E RID: 1886
		public static BoolConVar enableSprintSensitivitySlowdown = new BoolConVar("enable_sprint_sensitivity_slowdown", ConVarFlags.Archive, "1", "Enables sensitivity reduction while sprinting.");

		// Token: 0x0400075F RID: 1887
		private float hitmarkerAlpha;

		// Token: 0x04000760 RID: 1888
		private float hitmarkerTimer;

		// Token: 0x02000171 RID: 369
		public enum CameraMode
		{
			// Token: 0x04000762 RID: 1890
			None,
			// Token: 0x04000763 RID: 1891
			PlayerBasic,
			// Token: 0x04000764 RID: 1892
			Fly,
			// Token: 0x04000765 RID: 1893
			SpectateOrbit,
			// Token: 0x04000766 RID: 1894
			SpectateUser
		}

		// Token: 0x02000172 RID: 370
		public struct AimAssistInfo
		{
			// Token: 0x04000767 RID: 1895
			public HurtBox aimAssistHurtbox;

			// Token: 0x04000768 RID: 1896
			public Vector3 localPositionOnHurtbox;

			// Token: 0x04000769 RID: 1897
			public Vector3 worldPosition;
		}
	}
}
