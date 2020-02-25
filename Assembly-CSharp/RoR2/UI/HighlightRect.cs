using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C9 RID: 1481
	[RequireComponent(typeof(Canvas))]
	public class HighlightRect : MonoBehaviour
	{
		// Token: 0x06002303 RID: 8963 RVA: 0x00098B87 File Offset: 0x00096D87
		static HighlightRect()
		{
			RoR2Application.onLateUpdate += HighlightRect.UpdateAll;
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x00098BAF File Offset: 0x00096DAF
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x00098BBD File Offset: 0x00096DBD
		private void OnEnable()
		{
			HighlightRect.instancesList.Add(this);
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x00098BCA File Offset: 0x00096DCA
		private void OnDisable()
		{
			HighlightRect.instancesList.Remove(this);
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x00098BD8 File Offset: 0x00096DD8
		private void Start()
		{
			this.highlightState = HighlightRect.HighlightState.Expanding;
			this.bottomLeftImage = this.bottomLeftRectTransform.GetComponent<Image>();
			this.bottomRightImage = this.bottomRightRectTransform.GetComponent<Image>();
			this.topLeftImage = this.topLeftRectTransform.GetComponent<Image>();
			this.topRightImage = this.topRightRectTransform.GetComponent<Image>();
			this.bottomLeftImage.sprite = this.cornerImage;
			this.bottomRightImage.sprite = this.cornerImage;
			this.topLeftImage.sprite = this.cornerImage;
			this.topRightImage.sprite = this.cornerImage;
			this.bottomLeftImage.color = this.highlightColor;
			this.bottomRightImage.color = this.highlightColor;
			this.topLeftImage.color = this.highlightColor;
			this.topRightImage.color = this.highlightColor;
			if (this.nametagRectTransform)
			{
				this.nametagText = this.nametagRectTransform.GetComponent<TextMeshProUGUI>();
				this.nametagText.color = this.highlightColor;
				this.nametagText.text = this.nametagString;
			}
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x00098CF8 File Offset: 0x00096EF8
		private static void UpdateAll()
		{
			for (int i = HighlightRect.instancesList.Count - 1; i >= 0; i--)
			{
				HighlightRect.instancesList[i].DoUpdate();
			}
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x00098D2C File Offset: 0x00096F2C
		private void DoUpdate()
		{
			if (!this.targetRenderer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			switch (this.highlightState)
			{
			case HighlightRect.HighlightState.Expanding:
				this.time += Time.deltaTime;
				if (this.time >= this.expandTime)
				{
					this.time = this.expandTime;
					this.highlightState = HighlightRect.HighlightState.Holding;
				}
				break;
			case HighlightRect.HighlightState.Holding:
				if (this.destroyOnLifeEnd)
				{
					this.time += Time.deltaTime;
					if (this.time > this.maxLifeTime)
					{
						this.highlightState = HighlightRect.HighlightState.Contracting;
						this.time = this.expandTime;
					}
				}
				break;
			case HighlightRect.HighlightState.Contracting:
				this.time -= Time.deltaTime;
				if (this.time <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				break;
			}
			Rect rect = HighlightRect.GUIRectWithObject(this.sceneCam, this.targetRenderer);
			Vector2 a = new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, 0.5f), Mathf.Lerp(rect.yMin, rect.yMax, 0.5f));
			float t = this.curve.Evaluate(this.time / this.expandTime);
			this.bottomLeftRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMin), t);
			this.bottomRightRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMax, rect.yMin), t);
			this.topLeftRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMax), t);
			this.topRightRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMax, rect.yMax), t);
			if (this.nametagRectTransform)
			{
				this.nametagRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMax), t);
			}
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x00098F38 File Offset: 0x00097138
		public static Rect GUIRectWithObject(Camera cam, Renderer rend)
		{
			Vector3 center = rend.bounds.center;
			Vector3 extents = rend.bounds.extents;
			HighlightRect.extentPoints[0] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z));
			HighlightRect.extentPoints[1] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z));
			HighlightRect.extentPoints[2] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z));
			HighlightRect.extentPoints[3] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z));
			HighlightRect.extentPoints[4] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z));
			HighlightRect.extentPoints[5] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z));
			HighlightRect.extentPoints[6] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z));
			HighlightRect.extentPoints[7] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z));
			Vector2 vector = HighlightRect.extentPoints[0];
			Vector2 vector2 = HighlightRect.extentPoints[0];
			foreach (Vector2 rhs in HighlightRect.extentPoints)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x000991C7 File Offset: 0x000973C7
		public static Vector2 WorldToGUIPoint(Camera cam, Vector3 world)
		{
			return cam.WorldToScreenPoint(world);
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x000991D8 File Offset: 0x000973D8
		public static void CreateHighlight(GameObject viewerBodyObject, Renderer targetRenderer, GameObject highlightPrefab, float overrideDuration = -1f, bool visibleToAll = false)
		{
			ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
			int i = 0;
			int count = readOnlyInstancesList.Count;
			while (i < count)
			{
				CameraRigController cameraRigController = readOnlyInstancesList[i];
				if (!(cameraRigController.target != viewerBodyObject) || visibleToAll)
				{
					HighlightRect component = UnityEngine.Object.Instantiate<GameObject>(highlightPrefab).GetComponent<HighlightRect>();
					component.targetRenderer = targetRenderer;
					component.canvas.worldCamera = cameraRigController.uiCam;
					component.uiCam = cameraRigController.uiCam;
					component.sceneCam = cameraRigController.sceneCam;
					if (overrideDuration > 0f)
					{
						component.maxLifeTime = overrideDuration;
					}
				}
				i++;
			}
		}

		// Token: 0x040020CA RID: 8394
		public AnimationCurve curve;

		// Token: 0x040020CB RID: 8395
		public Color highlightColor;

		// Token: 0x040020CC RID: 8396
		public Sprite cornerImage;

		// Token: 0x040020CD RID: 8397
		public string nametagString;

		// Token: 0x040020CE RID: 8398
		private Image bottomLeftImage;

		// Token: 0x040020CF RID: 8399
		private Image bottomRightImage;

		// Token: 0x040020D0 RID: 8400
		private Image topLeftImage;

		// Token: 0x040020D1 RID: 8401
		private Image topRightImage;

		// Token: 0x040020D2 RID: 8402
		private TextMeshProUGUI nametagText;

		// Token: 0x040020D3 RID: 8403
		public Renderer targetRenderer;

		// Token: 0x040020D4 RID: 8404
		public GameObject cameraTarget;

		// Token: 0x040020D5 RID: 8405
		public RectTransform nametagRectTransform;

		// Token: 0x040020D6 RID: 8406
		public RectTransform bottomLeftRectTransform;

		// Token: 0x040020D7 RID: 8407
		public RectTransform bottomRightRectTransform;

		// Token: 0x040020D8 RID: 8408
		public RectTransform topLeftRectTransform;

		// Token: 0x040020D9 RID: 8409
		public RectTransform topRightRectTransform;

		// Token: 0x040020DA RID: 8410
		public float expandTime = 1f;

		// Token: 0x040020DB RID: 8411
		public float maxLifeTime;

		// Token: 0x040020DC RID: 8412
		public bool destroyOnLifeEnd;

		// Token: 0x040020DD RID: 8413
		private float time;

		// Token: 0x040020DE RID: 8414
		public HighlightRect.HighlightState highlightState;

		// Token: 0x040020DF RID: 8415
		private static List<HighlightRect> instancesList = new List<HighlightRect>();

		// Token: 0x040020E0 RID: 8416
		private Canvas canvas;

		// Token: 0x040020E1 RID: 8417
		private Camera uiCam;

		// Token: 0x040020E2 RID: 8418
		private Camera sceneCam;

		// Token: 0x040020E3 RID: 8419
		private static readonly Vector2[] extentPoints = new Vector2[8];

		// Token: 0x020005CA RID: 1482
		public enum HighlightState
		{
			// Token: 0x040020E5 RID: 8421
			Expanding,
			// Token: 0x040020E6 RID: 8422
			Holding,
			// Token: 0x040020E7 RID: 8423
			Contracting
		}
	}
}
