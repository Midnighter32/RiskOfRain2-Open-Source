using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EE RID: 1518
	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(RectTransform))]
	public class ModelPanel : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IScrollHandler, IEndDragHandler
	{
		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x060023D0 RID: 9168 RVA: 0x0009C67F File Offset: 0x0009A87F
		// (set) Token: 0x060023D1 RID: 9169 RVA: 0x0009C687 File Offset: 0x0009A887
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

		// Token: 0x060023D2 RID: 9170 RVA: 0x0009C6AB File Offset: 0x0009A8AB
		private void DestroyModelInstance()
		{
			UnityEngine.Object.Destroy(this.modelInstance);
			this.modelInstance = null;
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x0009C6C0 File Offset: 0x0009A8C0
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

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x060023D4 RID: 9172 RVA: 0x0009C997 File Offset: 0x0009AB97
		// (set) Token: 0x060023D5 RID: 9173 RVA: 0x0009C99F File Offset: 0x0009AB9F
		public RenderTexture renderTexture { get; private set; }

		// Token: 0x060023D6 RID: 9174 RVA: 0x0009C9A8 File Offset: 0x0009ABA8
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

		// Token: 0x060023D7 RID: 9175 RVA: 0x0009CA04 File Offset: 0x0009AC04
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

		// Token: 0x060023D8 RID: 9176 RVA: 0x0009CC62 File Offset: 0x0009AE62
		public void Start()
		{
			this.BuildRenderTexture();
			this.desiredZoom = 0.5f;
			this.zoom = this.desiredZoom;
			this.zoomVelocity = 0f;
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x0009CC8C File Offset: 0x0009AE8C
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

		// Token: 0x060023DA RID: 9178 RVA: 0x0009CD0C File Offset: 0x0009AF0C
		private void OnDisable()
		{
			this.DestroyModelInstance();
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x0009CD14 File Offset: 0x0009AF14
		private void OnEnable()
		{
			this.BuildModelInstance();
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x0009CD1C File Offset: 0x0009AF1C
		public void Update()
		{
			this.UpdateForModelViewer(Time.unscaledDeltaTime);
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x0009CD2C File Offset: 0x0009AF2C
		public void LateUpdate()
		{
			this.modelCamera.attachedCamera.aspect = (float)this.renderTexture.width / (float)this.renderTexture.height;
			this.cameraRigController.baseFov = this.fov;
			this.modelCamera.renderSettings = this.renderSettings;
			this.modelCamera.RenderItem(this.modelInstance, this.renderTexture);
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x0009CD9B File Offset: 0x0009AF9B
		private void OnRectTransformDimensionsChange()
		{
			this.BuildRenderTexture();
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x0009CDA4 File Offset: 0x0009AFA4
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

		// Token: 0x060023E0 RID: 9184 RVA: 0x0009CE88 File Offset: 0x0009B088
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

		// Token: 0x060023E1 RID: 9185 RVA: 0x0009D06C File Offset: 0x0009B26C
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

		// Token: 0x060023E2 RID: 9186 RVA: 0x0009D0C4 File Offset: 0x0009B2C4
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

		// Token: 0x060023E3 RID: 9187 RVA: 0x0009D17C File Offset: 0x0009B37C
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

		// Token: 0x060023E4 RID: 9188 RVA: 0x0009D1E1 File Offset: 0x0009B3E1
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

		// Token: 0x060023E5 RID: 9189 RVA: 0x0009D21C File Offset: 0x0009B41C
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

		// Token: 0x060023E6 RID: 9190 RVA: 0x0009D2BF File Offset: 0x0009B4BF
		public void OnScroll(PointerEventData eventData)
		{
			this.desiredZoom = Mathf.Clamp01(this.desiredZoom + eventData.scrollDelta.y * -0.05f);
		}

		// Token: 0x040021C2 RID: 8642
		private GameObject _modelPrefab;

		// Token: 0x040021C3 RID: 8643
		public RenderSettingsState renderSettings;

		// Token: 0x040021C4 RID: 8644
		public Color camBackgroundColor = Color.clear;

		// Token: 0x040021C5 RID: 8645
		public bool disablePostProcessLayer = true;

		// Token: 0x040021C6 RID: 8646
		private RectTransform rectTransform;

		// Token: 0x040021C7 RID: 8647
		private RawImage rawImage;

		// Token: 0x040021C8 RID: 8648
		private GameObject modelInstance;

		// Token: 0x040021C9 RID: 8649
		private CameraRigController cameraRigController;

		// Token: 0x040021CA RID: 8650
		private ModelCamera modelCamera;

		// Token: 0x040021CB RID: 8651
		public GameObject headlightPrefab;

		// Token: 0x040021CC RID: 8652
		public GameObject[] lightPrefabs;

		// Token: 0x040021CD RID: 8653
		private Light headlight;

		// Token: 0x040021CF RID: 8655
		public float fov = 60f;

		// Token: 0x040021D0 RID: 8656
		private float zoom = 0.5f;

		// Token: 0x040021D1 RID: 8657
		private float desiredZoom = 0.5f;

		// Token: 0x040021D2 RID: 8658
		private float zoomVelocity;

		// Token: 0x040021D3 RID: 8659
		private float minDistance = 0.5f;

		// Token: 0x040021D4 RID: 8660
		private float maxDistance = 10f;

		// Token: 0x040021D5 RID: 8661
		private float orbitPitch;

		// Token: 0x040021D6 RID: 8662
		private float orbitYaw = 180f;

		// Token: 0x040021D7 RID: 8663
		private Vector3 orbitalVelocity = Vector3.zero;

		// Token: 0x040021D8 RID: 8664
		private Vector3 orbitalVelocitySmoothDampVelocity = Vector3.zero;

		// Token: 0x040021D9 RID: 8665
		private Vector2 pan;

		// Token: 0x040021DA RID: 8666
		private Vector2 panVelocity;

		// Token: 0x040021DB RID: 8667
		private Vector2 panVelocitySmoothDampVelocity;

		// Token: 0x040021DC RID: 8668
		private Vector3 pivotPoint = Vector3.zero;

		// Token: 0x040021DD RID: 8669
		private List<Light> lights = new List<Light>();

		// Token: 0x040021DE RID: 8670
		private Vector2 orbitDragPoint;

		// Token: 0x040021DF RID: 8671
		private Vector2 panDragPoint;

		// Token: 0x040021E0 RID: 8672
		private int orbitDragCount;

		// Token: 0x040021E1 RID: 8673
		private int panDragCount;

		// Token: 0x020005EF RID: 1519
		private class CameraFramingCalculator
		{
			// Token: 0x060023E8 RID: 9192 RVA: 0x0009D378 File Offset: 0x0009B578
			private static void GenerateBoneList(Transform rootBone, List<Transform> boneList)
			{
				boneList.AddRange(rootBone.gameObject.GetComponentsInChildren<Transform>());
			}

			// Token: 0x060023E9 RID: 9193 RVA: 0x0009D398 File Offset: 0x0009B598
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

			// Token: 0x060023EA RID: 9194 RVA: 0x0009D410 File Offset: 0x0009B610
			private bool FindBestEyePoint(out Vector3 result, out float approximateEyeRadius)
			{
				approximateEyeRadius = 1f;
				IEnumerable<Transform> source = this.boneList.Where(new Func<Transform, bool>(ModelPanel.CameraFramingCalculator.<>c.<>9.<FindBestEyePoint>g__FirstChoice|10_0));
				if (!source.Any<Transform>())
				{
					source = this.boneList.Where(new Func<Transform, bool>(ModelPanel.CameraFramingCalculator.<>c.<>9.<FindBestEyePoint>g__SecondChoice|10_1));
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				for (int i = 0; i < array.Length; i++)
				{
					float magnitude = (array[i] - result).magnitude;
					if (magnitude > approximateEyeRadius)
					{
						approximateEyeRadius = magnitude;
					}
				}
				return array.Length != 0;
			}

			// Token: 0x060023EB RID: 9195 RVA: 0x0009D4D4 File Offset: 0x0009B6D4
			private bool FindBestHeadPoint(string searchName, out Vector3 result, out float approximateRadius)
			{
				Transform[] array = (from bone in this.boneList
				where string.Equals(bone.name, searchName, StringComparison.OrdinalIgnoreCase)
				select bone).ToArray<Transform>();
				if (array.Length == 0)
				{
					array = (from bone in this.boneList
					where bone.name.ToLower(CultureInfo.InvariantCulture).Contains(searchName)
					select bone).ToArray<Transform>();
				}
				if (array.Length != 0)
				{
					foreach (Transform bone2 in array)
					{
						Bounds bounds;
						if (this.TryCalcBoneBounds(bone2, 0.2f, out bounds, out approximateRadius))
						{
							result = bounds.center;
							return true;
						}
					}
				}
				result = Vector3.zero;
				approximateRadius = 0f;
				return false;
			}

			// Token: 0x060023EC RID: 9196 RVA: 0x0009D57C File Offset: 0x0009B77C
			private static float CalcMagnitudeToFrameSphere(float sphereRadius, float fieldOfView)
			{
				float num = fieldOfView * 0.5f;
				float num2 = 90f;
				return Mathf.Tan((180f - num2 - num) * 0.017453292f) * sphereRadius;
			}

			// Token: 0x060023ED RID: 9197 RVA: 0x0009D5B0 File Offset: 0x0009B7B0
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

			// Token: 0x060023EE RID: 9198 RVA: 0x0009D674 File Offset: 0x0009B874
			private static float GetWeightForBone(ref BoneWeight boneWeight, int boneIndex)
			{
				if (boneWeight.boneIndex0 == boneIndex)
				{
					return boneWeight.weight0;
				}
				if (boneWeight.boneIndex1 == boneIndex)
				{
					return boneWeight.weight1;
				}
				if (boneWeight.boneIndex2 == boneIndex)
				{
					return boneWeight.weight2;
				}
				if (boneWeight.boneIndex3 == boneIndex)
				{
					return boneWeight.weight3;
				}
				return 0f;
			}

			// Token: 0x060023EF RID: 9199 RVA: 0x0009D6C8 File Offset: 0x0009B8C8
			private static int FindBoneIndex(SkinnedMeshRenderer _skinnedMeshRenderer, Transform _bone)
			{
				Transform[] bones = _skinnedMeshRenderer.bones;
				for (int i = 0; i < bones.Length; i++)
				{
					if (bones[i] == _bone)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060023F0 RID: 9200 RVA: 0x0009D6F8 File Offset: 0x0009B8F8
			private bool TryCalcBoneBounds(Transform bone, float weightThreshold, out Bounds bounds, out float approximateRadius)
			{
				SkinnedMeshRenderer[] componentsInChildren = this.modelInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
				SkinnedMeshRenderer skinnedMeshRenderer = null;
				Mesh mesh = null;
				int num = -1;
				List<int> list = new List<int>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					mesh = skinnedMeshRenderer.sharedMesh;
					if (mesh)
					{
						num = ModelPanel.CameraFramingCalculator.FindBoneIndex(skinnedMeshRenderer, bone);
						if (num != -1)
						{
							BoneWeight[] boneWeights = mesh.boneWeights;
							for (int j = 0; j < boneWeights.Length; j++)
							{
								if (ModelPanel.CameraFramingCalculator.GetWeightForBone(ref boneWeights[j], num) > weightThreshold)
								{
									list.Add(j);
								}
							}
							if (list.Count == 0)
							{
								num = -1;
							}
						}
						if (num != -1)
						{
							break;
						}
					}
				}
				if (num == -1)
				{
					bounds = default(Bounds);
					approximateRadius = 0f;
					return false;
				}
				Mesh mesh2 = new Mesh();
				skinnedMeshRenderer.BakeMesh(mesh2);
				Vector3[] vertices = mesh2.vertices;
				UnityEngine.Object.Destroy(mesh2);
				if (mesh2.vertexCount != mesh.vertexCount)
				{
					Debug.LogWarningFormat("Baked mesh vertex count differs from the original mesh vertex count! baked={0} original={1}", new object[]
					{
						mesh2.vertexCount,
						mesh.vertexCount
					});
					vertices = mesh.vertices;
				}
				Vector3[] array = new Vector3[list.Count];
				Transform transform = skinnedMeshRenderer.transform;
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				for (int k = 0; k < list.Count; k++)
				{
					int num2 = list[k];
					Vector3 point = vertices[num2];
					Vector3 vector = position + rotation * point;
					array[k] = vector;
				}
				bounds = new Bounds(HGMath.Average<Vector3[]>(array), Vector3.zero);
				float num3 = 0f;
				for (int l = 0; l < array.Length; l++)
				{
					bounds.Encapsulate(array[l]);
					float num4 = Vector3.Distance(bounds.center, array[l]);
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
				approximateRadius = num3;
				return true;
			}

			// Token: 0x060023F1 RID: 9201 RVA: 0x0009D8E0 File Offset: 0x0009BAE0
			public void GetCharacterThumbnailPosition(float fov)
			{
				Vector3 vector = Vector3.zero;
				float sphereRadius = 1f;
				bool flag = this.FindBestHeadPoint("head", out vector, out sphereRadius);
				if (!flag)
				{
					flag = this.FindBestHeadPoint("chest", out vector, out sphereRadius);
				}
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
					Mathf.Max((vector.y - bounds.min.y) / bounds.size.y - 0.5f - 0.2f, 0f);
					Vector3 center = bounds.center;
					num3 = bounds.size.z / bounds.size.x;
				}
				Vector3 vector3 = -this.root.forward;
				for (int i = 0; i < this.boneList.Count; i++)
				{
					if (this.boneList[i].name.Equals("muzzle", StringComparison.OrdinalIgnoreCase))
					{
						Vector3 vector4 = this.root.position - this.boneList[i].position;
						vector4.y = 0f;
						float magnitude = vector4.magnitude;
						if (magnitude > 0.2f)
						{
							vector4 /= magnitude;
							vector3 = vector4;
							break;
						}
					}
				}
				vector3 = Quaternion.Euler(0f, 57.29578f * Mathf.Atan(num3 - 1f) * 1f, 0f) * vector3;
				Vector3 b = -vector3 * (ModelPanel.CameraFramingCalculator.CalcMagnitudeToFrameSphere(sphereRadius, fov) + num);
				Vector3 b2 = vector + b;
				this.outputPivotPoint = vector;
				this.outputCameraPosition = b2;
				this.outputCameraRotation = Util.QuaternionSafeLookRotation(vector - b2);
			}

			// Token: 0x040021E2 RID: 8674
			private GameObject modelInstance;

			// Token: 0x040021E3 RID: 8675
			private Transform root;

			// Token: 0x040021E4 RID: 8676
			private readonly List<Transform> boneList = new List<Transform>();

			// Token: 0x040021E5 RID: 8677
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x040021E6 RID: 8678
			private HurtBox[] hurtBoxes = Array.Empty<HurtBox>();

			// Token: 0x040021E7 RID: 8679
			public Vector3 outputPivotPoint;

			// Token: 0x040021E8 RID: 8680
			public Vector3 outputCameraPosition;

			// Token: 0x040021E9 RID: 8681
			public Quaternion outputCameraRotation;
		}
	}
}
