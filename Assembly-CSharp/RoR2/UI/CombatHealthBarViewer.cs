using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A1 RID: 1441
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Canvas))]
	public class CombatHealthBarViewer : MonoBehaviour, ILayoutGroup, ILayoutController
	{
		// Token: 0x0600223B RID: 8763 RVA: 0x000942A5 File Offset: 0x000924A5
		static CombatHealthBarViewer()
		{
			GlobalEventManager.onClientDamageNotified += delegate(DamageDealtMessage msg)
			{
				if (!msg.victim || msg.isSilent)
				{
					return;
				}
				HealthComponent component = msg.victim.GetComponent<HealthComponent>();
				if (!component || component.dontShowHealthbar)
				{
					return;
				}
				TeamIndex objectTeam = TeamComponent.GetObjectTeam(component.gameObject);
				foreach (CombatHealthBarViewer combatHealthBarViewer in CombatHealthBarViewer.instancesList)
				{
					if (msg.attacker == combatHealthBarViewer.viewerBodyObject && combatHealthBarViewer.viewerBodyObject)
					{
						combatHealthBarViewer.HandleDamage(component, objectTeam);
					}
				}
			};
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x0600223C RID: 8764 RVA: 0x000942C6 File Offset: 0x000924C6
		// (set) Token: 0x0600223D RID: 8765 RVA: 0x000942CE File Offset: 0x000924CE
		public HealthComponent crosshairTarget { get; set; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x0600223E RID: 8766 RVA: 0x000942D7 File Offset: 0x000924D7
		// (set) Token: 0x0600223F RID: 8767 RVA: 0x000942DF File Offset: 0x000924DF
		public GameObject viewerBodyObject { get; set; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06002240 RID: 8768 RVA: 0x000942E8 File Offset: 0x000924E8
		// (set) Token: 0x06002241 RID: 8769 RVA: 0x000942F0 File Offset: 0x000924F0
		public CharacterBody viewerBody { get; set; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06002242 RID: 8770 RVA: 0x000942F9 File Offset: 0x000924F9
		// (set) Token: 0x06002243 RID: 8771 RVA: 0x00094301 File Offset: 0x00092501
		public TeamIndex viewerTeamIndex { get; set; }

		// Token: 0x06002244 RID: 8772 RVA: 0x0009430A File Offset: 0x0009250A
		private void Update()
		{
			if (this.crosshairTarget)
			{
				CombatHealthBarViewer.HealthBarInfo healthBarInfo = this.GetHealthBarInfo(this.crosshairTarget);
				healthBarInfo.endTime = Mathf.Max(healthBarInfo.endTime, Time.time + 1f);
			}
			this.SetDirty();
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x00094346 File Offset: 0x00092546
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x00094365 File Offset: 0x00092565
		private void Start()
		{
			this.FindCamera();
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x0009436D File Offset: 0x0009256D
		private void FindCamera()
		{
			this.uiCamera = this.canvas.rootCanvas.worldCamera.GetComponent<UICamera>();
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x0009438A File Offset: 0x0009258A
		private void OnEnable()
		{
			CombatHealthBarViewer.instancesList.Add(this);
		}

		// Token: 0x06002249 RID: 8777 RVA: 0x00094398 File Offset: 0x00092598
		private void OnDisable()
		{
			CombatHealthBarViewer.instancesList.Remove(this);
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				this.Remove(i);
			}
		}

		// Token: 0x0600224A RID: 8778 RVA: 0x000943D0 File Offset: 0x000925D0
		private void Remove(int trackedVictimIndex)
		{
			this.Remove(trackedVictimIndex, this.victimToHealthBarInfo[this.trackedVictims[trackedVictimIndex]]);
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x000943F0 File Offset: 0x000925F0
		private void Remove(int trackedVictimIndex, CombatHealthBarViewer.HealthBarInfo healthBarInfo)
		{
			this.trackedVictims.RemoveAt(trackedVictimIndex);
			UnityEngine.Object.Destroy(healthBarInfo.healthBarRootObject);
			this.victimToHealthBarInfo.Remove(healthBarInfo.sourceHealthComponent);
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x0009441B File Offset: 0x0009261B
		private bool VictimIsValid(HealthComponent victim)
		{
			return victim && victim.alive && (this.victimToHealthBarInfo[victim].endTime > Time.time || victim == this.crosshairTarget);
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x00094455 File Offset: 0x00092655
		private void LateUpdate()
		{
			this.CleanUp();
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x00094460 File Offset: 0x00092660
		private void CleanUp()
		{
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				HealthComponent healthComponent = this.trackedVictims[i];
				if (!this.VictimIsValid(healthComponent))
				{
					this.Remove(i, this.victimToHealthBarInfo[healthComponent]);
				}
			}
		}

		// Token: 0x0600224F RID: 8783 RVA: 0x000944B0 File Offset: 0x000926B0
		private void UpdateAllHealthbarPositions(Camera sceneCam, Camera uiCam)
		{
			foreach (CombatHealthBarViewer.HealthBarInfo healthBarInfo in this.victimToHealthBarInfo.Values)
			{
				Vector3 position = healthBarInfo.sourceTransform.position;
				position.y += healthBarInfo.verticalOffset;
				Vector3 vector = sceneCam.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = uiCam.ScreenToWorldPoint(vector);
				healthBarInfo.healthBarRootObjectTransform.position = position2;
			}
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x00094560 File Offset: 0x00092760
		private void HandleDamage(HealthComponent victimHealthComponent, TeamIndex victimTeam)
		{
			if (this.viewerTeamIndex == victimTeam || victimTeam == TeamIndex.None)
			{
				return;
			}
			CharacterBody body = victimHealthComponent.body;
			if (body && body.GetVisibilityLevel(this.viewerBody) < VisibilityLevel.Revealed)
			{
				return;
			}
			this.GetHealthBarInfo(victimHealthComponent).endTime = Time.time + this.healthBarDuration;
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x000945B4 File Offset: 0x000927B4
		private CombatHealthBarViewer.HealthBarInfo GetHealthBarInfo(HealthComponent victimHealthComponent)
		{
			CombatHealthBarViewer.HealthBarInfo healthBarInfo;
			if (!this.victimToHealthBarInfo.TryGetValue(victimHealthComponent, out healthBarInfo))
			{
				healthBarInfo = new CombatHealthBarViewer.HealthBarInfo();
				healthBarInfo.healthBarRootObject = UnityEngine.Object.Instantiate<GameObject>(this.healthBarPrefab, this.container);
				healthBarInfo.healthBarRootObjectTransform = healthBarInfo.healthBarRootObject.transform;
				healthBarInfo.healthBar = healthBarInfo.healthBarRootObject.GetComponent<HealthBar>();
				healthBarInfo.healthBar.source = victimHealthComponent;
				healthBarInfo.healthBar.viewerBody = this.viewerBody;
				healthBarInfo.healthBarRootObject.GetComponentInChildren<BuffDisplay>().source = victimHealthComponent.body;
				healthBarInfo.sourceHealthComponent = victimHealthComponent;
				healthBarInfo.verticalOffset = 0f;
				Collider component = victimHealthComponent.GetComponent<Collider>();
				if (component)
				{
					healthBarInfo.verticalOffset = component.bounds.extents.y;
				}
				healthBarInfo.sourceTransform = (victimHealthComponent.body.coreTransform ?? victimHealthComponent.transform);
				ModelLocator component2 = victimHealthComponent.GetComponent<ModelLocator>();
				if (component2)
				{
					Transform modelTransform = component2.modelTransform;
					if (modelTransform)
					{
						ChildLocator component3 = modelTransform.GetComponent<ChildLocator>();
						if (component3)
						{
							Transform transform = component3.FindChild("HealthBarOrigin");
							if (transform)
							{
								healthBarInfo.sourceTransform = transform;
								healthBarInfo.verticalOffset = 0f;
							}
						}
					}
				}
				this.victimToHealthBarInfo.Add(victimHealthComponent, healthBarInfo);
				this.trackedVictims.Add(victimHealthComponent);
			}
			return healthBarInfo;
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x00094711 File Offset: 0x00092911
		private void SetDirty()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!CanvasUpdateRegistry.IsRebuildingLayout())
			{
				LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			}
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x00094730 File Offset: 0x00092930
		private static void LayoutForCamera(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			for (int i = 0; i < CombatHealthBarViewer.instancesList.Count; i++)
			{
				CombatHealthBarViewer.instancesList[i].UpdateAllHealthbarPositions(sceneCam, camera);
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x00094777 File Offset: 0x00092977
		public void SetLayoutHorizontal()
		{
			if (this.uiCamera)
			{
				CombatHealthBarViewer.LayoutForCamera(this.uiCamera);
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x0000409B File Offset: 0x0000229B
		public void SetLayoutVertical()
		{
		}

		// Token: 0x04001FA6 RID: 8102
		private static readonly List<CombatHealthBarViewer> instancesList = new List<CombatHealthBarViewer>();

		// Token: 0x04001FA7 RID: 8103
		public RectTransform container;

		// Token: 0x04001FA8 RID: 8104
		public GameObject healthBarPrefab;

		// Token: 0x04001FA9 RID: 8105
		public float healthBarDuration;

		// Token: 0x04001FAE RID: 8110
		private const float hoverHealthBarDuration = 1f;

		// Token: 0x04001FAF RID: 8111
		private RectTransform rectTransform;

		// Token: 0x04001FB0 RID: 8112
		private Canvas canvas;

		// Token: 0x04001FB1 RID: 8113
		private UICamera uiCamera;

		// Token: 0x04001FB2 RID: 8114
		private List<HealthComponent> trackedVictims = new List<HealthComponent>();

		// Token: 0x04001FB3 RID: 8115
		private Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo> victimToHealthBarInfo = new Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo>();

		// Token: 0x04001FB4 RID: 8116
		public float zPosition;

		// Token: 0x04001FB5 RID: 8117
		private const float overheadOffset = 1f;

		// Token: 0x020005A2 RID: 1442
		private class HealthBarInfo
		{
			// Token: 0x04001FB6 RID: 8118
			public HealthComponent sourceHealthComponent;

			// Token: 0x04001FB7 RID: 8119
			public Transform sourceTransform;

			// Token: 0x04001FB8 RID: 8120
			public GameObject healthBarRootObject;

			// Token: 0x04001FB9 RID: 8121
			public Transform healthBarRootObjectTransform;

			// Token: 0x04001FBA RID: 8122
			public HealthBar healthBar;

			// Token: 0x04001FBB RID: 8123
			public float verticalOffset;

			// Token: 0x04001FBC RID: 8124
			public float endTime = float.NegativeInfinity;
		}
	}
}
