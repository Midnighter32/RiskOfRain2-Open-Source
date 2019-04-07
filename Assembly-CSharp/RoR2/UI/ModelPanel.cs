using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000600 RID: 1536
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RawImage))]
	public class ModelPanel : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IScrollHandler, IEndDragHandler
	{
		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06002265 RID: 8805 RVA: 0x000A2723 File Offset: 0x000A0923
		// (set) Token: 0x06002266 RID: 8806 RVA: 0x000A272B File Offset: 0x000A092B
		public GameObject modelPrefab
		{
			get
			{
				return this._modelPrefab;
			}
			set
			{
				if (this._modelPrefab == value)
				{
					return;
				}
				this.DestroyModelInstance();
				this._modelPrefab = value;
				this.BuildModelInstance();
			}
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x000A274F File Offset: 0x000A094F
		private void DestroyModelInstance()
		{
			UnityEngine.Object.Destroy(this.modelInstance);
			this.modelInstance = null;
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x000A2764 File Offset: 0x000A0964
		private void BuildModelInstance()
		{
			if (this._modelPrefab && base.enabled && !this.modelInstance)
			{
				this.modelInstance = UnityEngine.Object.Instantiate<GameObject>(this._modelPrefab, Vector3.zero, Quaternion.identity);
				Bounds bounds;
				Util.GuessRenderBoundsMeshOnly(this.modelInstance, out bounds);
				this.pivotPoint = bounds.center;
				this.minDistance = Mathf.Min(new float[]
				{
					bounds.size.x,
					bounds.size.y,
					bounds.size.z
				}) * 1f;
				this.maxDistance = Mathf.Max(new float[]
				{
					bounds.size.x,
					bounds.size.y,
					bounds.size.z
				}) * 2f;
				Renderer[] componentsInChildren = this.modelInstance.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.layer = LayerIndex.noDraw.intVal;
				}
				AimAnimator[] componentsInChildren2 = this.modelInstance.GetComponentsInChildren<AimAnimator>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].inputBank = null;
					componentsInChildren2[j].directionComponent = null;
					componentsInChildren2[j].enabled = false;
				}
				foreach (Animator animator in this.modelInstance.GetComponentsInChildren<Animator>())
				{
					animator.SetBool("isGrounded", true);
					animator.SetFloat("aimPitchCycle", 0.5f);
					animator.SetFloat("aimYawCycle", 0.5f);
					animator.Play("Idle");
					animator.Update(0f);
				}
				IKSimpleChain[] componentsInChildren4 = this.modelInstance.GetComponentsInChildren<IKSimpleChain>();
				for (int l = 0; l < componentsInChildren4.Length; l++)
				{
					componentsInChildren4[l].enabled = false;
				}
				DitherModel[] componentsInChildren5 = this.modelInstance.GetComponentsInChildren<DitherModel>();
				for (int m = 0; m < componentsInChildren5.Length; m++)
				{
					componentsInChildren5[m].enabled = false;
				}
				PrintController[] componentsInChildren6 = this.modelInstance.GetComponentsInChildren<PrintController>();
				for (int m = 0; m < componentsInChildren6.Length; m++)
				{
					componentsInChildren6[m].enabled = false;
				}
				foreach (LightIntensityCurve lightIntensityCurve in this.modelInstance.GetComponentsInChildren<LightIntensityCurve>())
				{
					if (!lightIntensityCurve.loop)
					{
						lightIntensityCurve.enabled = false;
					}
				}
				AkEvent[] componentsInChildren8 = this.modelInstance.GetComponentsInChildren<AkEvent>();
				for (int m = 0; m < componentsInChildren8.Length; m++)
				{
					componentsInChildren8[m].enabled = false;
				}
				this.desiredZoom = 0.5f;
				this.zoom = this.desiredZoom;
				this.zoomVelocity = 0f;
				this.ResetOrbitAndPan();
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06002269 RID: 8809 RVA: 0x000A2A3B File Offset: 0x000A0C3B
		// (set) Token: 0x0600226A RID: 8810 RVA: 0x000A2A43 File Offset: 0x000A0C43
		public RenderTexture renderTexture { get; private set; }

		// Token: 0x0600226B RID: 8811 RVA: 0x000A2A4C File Offset: 0x000A0C4C
		private void ResetOrbitAndPan()
		{
			this.orbitPitch = 0f;
			this.orbitYaw = 0f;
			this.orbitalVelocity = Vector3.zero;
			this.orbitalVelocitySmoothDampVelocity = Vector3.zero;
			this.pan = Vector2.zero;
			this.panVelocity = Vector2.zero;
			this.panVelocitySmoothDampVelocity = Vector2.zero;
		}

		// Token: 0x0600226C RID: 8812 RVA: 0x000A2AA8 File Offset: 0x000A0CA8
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.rawImage = base.GetComponent<RawImage>();
			this.cameraRigController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Main Camera")).GetComponent<CameraRigController>();
			this.cameraRigController.gameObject.name = "ModelCamera";
			this.cameraRigController.uiCam.gameObject.SetActive(false);
			this.cameraRigController.createHud = false;
			this.cameraRigController.fadeStartDistance = float.PositiveInfinity;
			this.cameraRigController.fadeEndDistance = float.PositiveInfinity;
			GameObject gameObject = this.cameraRigController.sceneCam.gameObject;
			this.modelCamera = gameObject.AddComponent<ModelCamera>();
			this.cameraRigController.transform.position = -Vector3.forward * 10f;
			this.cameraRigController.transform.forward = Vector3.forward;
			CameraResolutionScaler component = gameObject.GetComponent<CameraResolutionScaler>();
			if (component)
			{
				component.enabled = false;
			}
			Camera sceneCam = this.cameraRigController.sceneCam;
			sceneCam.backgroundColor = Color.clear;
			sceneCam.clearFlags = CameraClearFlags.Color;
			if (this.disablePostProcessLayer)
			{
				PostProcessLayer component2 = sceneCam.GetComponent<PostProcessLayer>();
				if (component2)
				{
					component2.enabled = false;
				}
			}
			Vector3 eulerAngles = this.cameraRigController.transform.eulerAngles;
			this.orbitPitch = eulerAngles.x;
			this.orbitYaw = eulerAngles.y;
			this.modelCamera.attachedCamera.backgroundColor = this.camBackgroundColor;
			this.modelCamera.attachedCamera.clearFlags = CameraClearFlags.Color;
			this.modelCamera.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			if (this.headlightPrefab)
			{
				this.headlight = UnityEngine.Object.Instantiate<GameObject>(this.headlightPrefab, this.modelCamera.transform).GetComponent<Light>();
				if (this.headlight)
				{
					this.headlight.gameObject.SetActive(true);
					this.modelCamera.AddLight(this.headlight);
				}
			}
			for (int i = 0; i < this.lightPrefabs.Length; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.lightPrefabs[i]);
				Light component3 = gameObject2.GetComponent<Light>();
				gameObject2.SetActive(true);
				this.lights.Add(component3);
				this.modelCamera.AddLight(component3);
			}
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x000A2D06 File Offset: 0x000A0F06
		public void Start()
		{
			this.BuildRenderTexture();
			this.desiredZoom = 0.5f;
			this.zoom = this.desiredZoom;
			this.zoomVelocity = 0f;
		}

		// Token: 0x0600226E RID: 8814 RVA: 0x000A2D30 File Offset: 0x000A0F30
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.renderTexture);
			if (this.cameraRigController)
			{
				UnityEngine.Object.Destroy(this.cameraRigController.gameObject);
			}
			foreach (Light light in this.lights)
			{
				UnityEngine.Object.Destroy(light.gameObject);
			}
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x000A2DB0 File Offset: 0x000A0FB0
		private void OnDisable()
		{
			this.DestroyModelInstance();
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x000A2DB8 File Offset: 0x000A0FB8
		private void OnEnable()
		{
			this.BuildModelInstance();
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x000A2DC0 File Offset: 0x000A0FC0
		public void Update()
		{
			this.UpdateForModelViewer(Time.unscaledDeltaTime);
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x000A2DD0 File Offset: 0x000A0FD0
		public void LateUpdate()
		{
			this.modelCamera.attachedCamera.aspect = (float)this.renderTexture.width / (float)this.renderTexture.height;
			this.cameraRigController.baseFov = this.fov;
			this.modelCamera.renderSettings = this.renderSettings;
			this.modelCamera.RenderItem(this.modelInstance, this.renderTexture);
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x000A2E3F File Offset: 0x000A103F
		private void OnRectTransformDimensionsChange()
		{
			this.BuildRenderTexture();
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x000A2E48 File Offset: 0x000A1048
		private void BuildRenderTexture()
		{
			if (!this.rectTransform)
			{
				return;
			}
			Vector3[] fourCornersArray = new Vector3[4];
			this.rectTransform.GetLocalCorners(fourCornersArray);
			Vector2 size = this.rectTransform.rect.size;
			int num = Mathf.FloorToInt(size.x);
			int num2 = Mathf.FloorToInt(size.y);
			if (this.renderTexture && this.renderTexture.width == num && this.renderTexture.height == num2)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.renderTexture);
			this.renderTexture = null;
			if (num > 0 && num2 > 0)
			{
				this.renderTexture = new RenderTexture(new RenderTextureDescriptor(num, num2, RenderTextureFormat.ARGB32)
				{
					sRGB = true
				});
				this.renderTexture.useMipMap = false;
				this.renderTexture.filterMode = FilterMode.Bilinear;
			}
			this.rawImage.texture = this.renderTexture;
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x000A2F2C File Offset: 0x000A112C
		private void UpdateForModelViewer(float deltaTime)
		{
			this.zoom = Mathf.SmoothDamp(this.zoom, this.desiredZoom, ref this.zoomVelocity, 0.1f);
			this.orbitPitch = Mathf.Clamp(this.orbitPitch + this.orbitalVelocity.x * deltaTime, -89f, 89f);
			this.orbitYaw += this.orbitalVelocity.y * deltaTime;
			this.orbitalVelocity = Vector3.SmoothDamp(this.orbitalVelocity, Vector3.zero, ref this.orbitalVelocitySmoothDampVelocity, 0.25f, 2880f, deltaTime);
			if (this.orbitDragCount > 0)
			{
				this.orbitalVelocity = Vector3.zero;
				this.orbitalVelocitySmoothDampVelocity = Vector3.zero;
			}
			this.pan += this.panVelocity * deltaTime;
			this.panVelocity = Vector2.SmoothDamp(this.panVelocity, Vector2.zero, ref this.panVelocitySmoothDampVelocity, 0.25f, 100f, deltaTime);
			if (this.panDragCount > 0)
			{
				this.panVelocity = Vector2.zero;
				this.panVelocitySmoothDampVelocity = Vector2.zero;
			}
			Quaternion rotation = Quaternion.Euler(this.orbitPitch, this.orbitYaw, 0f);
			this.cameraRigController.transform.forward = rotation * Vector3.forward;
			Vector3 forward = this.cameraRigController.transform.forward;
			Vector3 position = this.pivotPoint + forward * -Mathf.LerpUnclamped(this.minDistance, this.maxDistance, this.zoom) + this.cameraRigController.transform.up * this.pan.y + this.cameraRigController.transform.right * this.pan.x;
			this.cameraRigController.transform.position = position;
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000A3110 File Offset: 0x000A1310
		public void SetAnglesForCharacterThumbnailForSeconds(float time, bool setZoom = false)
		{
			this.SetAnglesForCharacterThumbnail(setZoom);
			float t = time;
			Action func = null;
			func = delegate()
			{
				t -= Time.deltaTime;
				if (this)
				{
					this.SetAnglesForCharacterThumbnail(setZoom);
				}
				if (t <= 0f)
				{
					RoR2Application.onUpdate -= func;
				}
			};
			RoR2Application.onUpdate += func;
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000A3168 File Offset: 0x000A1368
		public void SetAnglesForCharacterThumbnail(bool setZoom = false)
		{
			if (!this.modelInstance)
			{
				return;
			}
			ModelPanel.CameraFramingCalculator cameraFramingCalculator = new ModelPanel.CameraFramingCalculator(this.modelInstance);
			cameraFramingCalculator.GetCharacterThumbnailPosition(this.fov);
			this.pivotPoint = cameraFramingCalculator.outputPivotPoint;
			this.ResetOrbitAndPan();
			Vector3 eulerAngles = cameraFramingCalculator.outputCameraRotation.eulerAngles;
			this.orbitPitch = eulerAngles.x;
			this.orbitYaw = eulerAngles.y;
			if (setZoom)
			{
				this.zoom = Util.Remap(Vector3.Distance(cameraFramingCalculator.outputCameraPosition, cameraFramingCalculator.outputPivotPoint), this.minDistance, this.maxDistance, 0f, 1f);
				this.desiredZoom = this.zoom;
			}
			this.zoomVelocity = 0f;
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x000A3220 File Offset: 0x000A1420
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				this.orbitDragCount++;
				if (this.orbitDragCount == 1)
				{
					this.orbitDragPoint = eventData.pressPosition;
					return;
				}
			}
			else if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.panDragCount++;
				if (this.panDragCount == 1)
				{
					this.panDragPoint = eventData.pressPosition;
				}
			}
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000A3285 File Offset: 0x000A1485
		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				this.orbitDragCount--;
			}
			else if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.panDragCount--;
			}
			this.OnDrag(eventData);
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000A32C0 File Offset: 0x000A14C0
		public void OnDrag(PointerEventData eventData)
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Vector2 vector = eventData.position - this.orbitDragPoint;
				this.orbitDragPoint = eventData.position;
				float num = 0.5f / unscaledDeltaTime;
				this.orbitalVelocity = new Vector3(-vector.y * num * 0.5f, vector.x * num, 0f);
				return;
			}
			Vector2 a = eventData.position - this.panDragPoint;
			this.panDragPoint = eventData.position;
			float d = -0.01f;
			this.panVelocity = a * d / unscaledDeltaTime;
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x000A3363 File Offset: 0x000A1563
		public void OnScroll(PointerEventData eventData)
		{
			this.desiredZoom = Mathf.Clamp01(this.desiredZoom + eventData.scrollDelta.y * -0.05f);
		}

		// Token: 0x0400257C RID: 9596
		private GameObject _modelPrefab;

		// Token: 0x0400257D RID: 9597
		public RenderSettingsState renderSettings;

		// Token: 0x0400257E RID: 9598
		public Color camBackgroundColor = Color.clear;

		// Token: 0x0400257F RID: 9599
		public bool disablePostProcessLayer = true;

		// Token: 0x04002580 RID: 9600
		private RectTransform rectTransform;

		// Token: 0x04002581 RID: 9601
		private RawImage rawImage;

		// Token: 0x04002582 RID: 9602
		private GameObject modelInstance;

		// Token: 0x04002583 RID: 9603
		private CameraRigController cameraRigController;

		// Token: 0x04002584 RID: 9604
		private ModelCamera modelCamera;

		// Token: 0x04002585 RID: 9605
		public GameObject headlightPrefab;

		// Token: 0x04002586 RID: 9606
		public GameObject[] lightPrefabs;

		// Token: 0x04002587 RID: 9607
		private Light headlight;

		// Token: 0x04002589 RID: 9609
		public float fov = 60f;

		// Token: 0x0400258A RID: 9610
		private float zoom = 0.5f;

		// Token: 0x0400258B RID: 9611
		private float desiredZoom = 0.5f;

		// Token: 0x0400258C RID: 9612
		private float zoomVelocity;

		// Token: 0x0400258D RID: 9613
		private float minDistance = 0.5f;

		// Token: 0x0400258E RID: 9614
		private float maxDistance = 10f;

		// Token: 0x0400258F RID: 9615
		private float orbitPitch;

		// Token: 0x04002590 RID: 9616
		private float orbitYaw = 180f;

		// Token: 0x04002591 RID: 9617
		private Vector3 orbitalVelocity = Vector3.zero;

		// Token: 0x04002592 RID: 9618
		private Vector3 orbitalVelocitySmoothDampVelocity = Vector3.zero;

		// Token: 0x04002593 RID: 9619
		private Vector2 pan;

		// Token: 0x04002594 RID: 9620
		private Vector2 panVelocity;

		// Token: 0x04002595 RID: 9621
		private Vector2 panVelocitySmoothDampVelocity;

		// Token: 0x04002596 RID: 9622
		private Vector3 pivotPoint = Vector3.zero;

		// Token: 0x04002597 RID: 9623
		private List<Light> lights = new List<Light>();

		// Token: 0x04002598 RID: 9624
		private Vector2 orbitDragPoint;

		// Token: 0x04002599 RID: 9625
		private Vector2 panDragPoint;

		// Token: 0x0400259A RID: 9626
		private int orbitDragCount;

		// Token: 0x0400259B RID: 9627
		private int panDragCount;

		// Token: 0x02000601 RID: 1537
		private class CameraFramingCalculator
		{
			// Token: 0x0600227D RID: 8829 RVA: 0x000A341C File Offset: 0x000A161C
			private static void GenerateBoneList(Transform rootBone, List<Transform> boneList)
			{
				boneList.Add(rootBone);
				for (int i = 0; i < boneList.Count; i++)
				{
					Transform transform = boneList[i];
					int j = 0;
					int childCount = transform.childCount;
					while (j < childCount)
					{
						boneList.Add(transform.GetChild(j));
						j++;
					}
				}
			}

			// Token: 0x0600227E RID: 8830 RVA: 0x000A346C File Offset: 0x000A166C
			public CameraFramingCalculator(GameObject modelInstance)
			{
				this.modelInstance = modelInstance;
				this.root = modelInstance.transform;
				ModelPanel.CameraFramingCalculator.GenerateBoneList(this.root, this.boneList);
				this.hurtBoxGroup = modelInstance.GetComponent<HurtBoxGroup>();
				if (this.hurtBoxGroup)
				{
					this.hurtBoxes = this.hurtBoxGroup.hurtBoxes;
				}
			}

			// Token: 0x0600227F RID: 8831 RVA: 0x000A34E4 File Offset: 0x000A16E4
			private bool FindBestEyePoint(out Vector3 result, out float approximateEyeRadius)
			{
				approximateEyeRadius = 1f;
				IEnumerable<Transform> source = from bone in this.boneList
				where bone.name.Equals("eye", StringComparison.OrdinalIgnoreCase) || bone.name.Equals("eyeball.1", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower().Contains("eye")
					select bone;
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				return array.Length != 0;
			}

			// Token: 0x06002280 RID: 8832 RVA: 0x000A3594 File Offset: 0x000A1794
			private bool FindBestHeadPoint(out Vector3 result, out float approximateRadius)
			{
				IEnumerable<Transform> source = from bone in this.boneList
				where string.Equals(bone.name, "head", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower().Contains("head")
					select bone;
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				approximateRadius = 1f;
				IEnumerable<float> source2 = from hurtBox in (from bone in source
				select bone.GetComponentInChildren<HurtBox>() into hurtBox
				where hurtBox
				select hurtBox).Distinct<HurtBox>()
				select Util.SphereVolumeToRadius(hurtBox.volume);
				if (source2.Any<float>())
				{
					approximateRadius = source2.Max();
				}
				Transform transform = this.boneList.Find((Transform bone) => string.Equals(bone.name, "chest", StringComparison.OrdinalIgnoreCase));
				if (transform)
				{
					approximateRadius = Mathf.Max(Vector3.Distance(transform.position, result), approximateRadius);
					result = (result + transform.position) / 2f;
				}
				return array.Length != 0;
			}

			// Token: 0x06002281 RID: 8833 RVA: 0x000A3734 File Offset: 0x000A1934
			private static float CalcMagnitudeToFrameSphere(float sphereRadius, float fieldOfView)
			{
				float num = fieldOfView * 0.5f;
				float num2 = 90f;
				return Mathf.Tan((180f - num2 - num) * 0.017453292f) * sphereRadius;
			}

			// Token: 0x06002282 RID: 8834 RVA: 0x000A3768 File Offset: 0x000A1968
			private bool FindBestCenterOfMass(out Vector3 result, out float approximateRadius)
			{
				from bone in this.boneList
				select bone.GetComponent<HurtBox>() into hurtBox
				where hurtBox
				select hurtBox;
				if (this.hurtBoxGroup && this.hurtBoxGroup.mainHurtBox)
				{
					result = this.hurtBoxGroup.mainHurtBox.transform.position;
					approximateRadius = Util.SphereVolumeToRadius(this.hurtBoxGroup.mainHurtBox.volume);
					return true;
				}
				result = Vector3.zero;
				approximateRadius = 1f;
				return false;
			}

			// Token: 0x06002283 RID: 8835 RVA: 0x000A382C File Offset: 0x000A1A2C
			public void GetCharacterThumbnailPosition(float fov)
			{
				Vector3 vector = Vector3.zero;
				float sphereRadius = 1f;
				bool flag = this.FindBestHeadPoint(out vector, out sphereRadius);
				bool flag2 = false;
				float num = 1f;
				float num2 = 1f;
				Vector3 vector2;
				bool flag3 = this.FindBestEyePoint(out vector2, out num2);
				if (!flag)
				{
					sphereRadius = num2;
				}
				if (flag3)
				{
					vector = vector2;
				}
				if (!flag && !flag3)
				{
					flag2 = this.FindBestCenterOfMass(out vector, out sphereRadius);
				}
				float num3 = 1f;
				Bounds bounds;
				if (Util.GuessRenderBoundsMeshOnly(this.modelInstance, out bounds))
				{
					if (flag2)
					{
						sphereRadius = Util.SphereVolumeToRadius(bounds.size.x * bounds.size.y * bounds.size.z);
					}
					num3 = bounds.size.z / bounds.size.x;
					float num4 = Mathf.Max((vector.y - bounds.min.y) / bounds.size.y - 0.5f - 0.2f, 0f);
					vector.y -= num4 * 0.5f * bounds.size.y;
				}
				Vector3 vector3 = -this.root.forward;
				vector3 = Quaternion.Euler(0f, 57.29578f * Mathf.Atan(num3 - 1f) * 1f, 0f) * vector3;
				Vector3 b = -vector3 * (ModelPanel.CameraFramingCalculator.CalcMagnitudeToFrameSphere(sphereRadius, fov) + num);
				Vector3 b2 = vector + b;
				this.outputPivotPoint = vector;
				this.outputCameraPosition = b2;
				this.outputCameraRotation = Util.QuaternionSafeLookRotation(vector - b2);
			}

			// Token: 0x0400259C RID: 9628
			private GameObject modelInstance;

			// Token: 0x0400259D RID: 9629
			private Transform root;

			// Token: 0x0400259E RID: 9630
			private readonly List<Transform> boneList = new List<Transform>();

			// Token: 0x0400259F RID: 9631
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x040025A0 RID: 9632
			private HurtBox[] hurtBoxes = Array.Empty<HurtBox>();

			// Token: 0x040025A1 RID: 9633
			public Vector3 outputPivotPoint;

			// Token: 0x040025A2 RID: 9634
			public Vector3 outputCameraPosition;

			// Token: 0x040025A3 RID: 9635
			public Quaternion outputCameraRotation;
		}
	}
}
