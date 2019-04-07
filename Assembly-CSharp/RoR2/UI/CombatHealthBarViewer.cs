using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C3 RID: 1475
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Canvas))]
	public class CombatHealthBarViewer : MonoBehaviour, ILayoutGroup, ILayoutController
	{
		// Token: 0x0600210C RID: 8460 RVA: 0x0009B4FB File Offset: 0x000996FB
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

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600210D RID: 8461 RVA: 0x0009B51C File Offset: 0x0009971C
		// (set) Token: 0x0600210E RID: 8462 RVA: 0x0009B524 File Offset: 0x00099724
		public HealthComponent crosshairTarget { get; set; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600210F RID: 8463 RVA: 0x0009B52D File Offset: 0x0009972D
		// (set) Token: 0x06002110 RID: 8464 RVA: 0x0009B535 File Offset: 0x00099735
		public GameObject viewerBodyObject { get; set; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x0009B53E File Offset: 0x0009973E
		// (set) Token: 0x06002112 RID: 8466 RVA: 0x0009B546 File Offset: 0x00099746
		public CharacterBody viewerBody { get; set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06002113 RID: 8467 RVA: 0x0009B54F File Offset: 0x0009974F
		// (set) Token: 0x06002114 RID: 8468 RVA: 0x0009B557 File Offset: 0x00099757
		public TeamIndex viewerTeamIndex { get; set; }

		// Token: 0x06002115 RID: 8469 RVA: 0x0009B560 File Offset: 0x00099760
		private void Update()
		{
			if (this.crosshairTarget)
			{
				CombatHealthBarViewer.HealthBarInfo healthBarInfo = this.GetHealthBarInfo(this.crosshairTarget);
				healthBarInfo.endTime = Mathf.Max(healthBarInfo.endTime, Time.time + 1f);
			}
			this.SetDirty();
		}

		// Token: 0x06002116 RID: 8470 RVA: 0x0009B59C File Offset: 0x0009979C
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x0009B5BB File Offset: 0x000997BB
		private void Start()
		{
			this.FindCamera();
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x0009B5C3 File Offset: 0x000997C3
		private void FindCamera()
		{
			this.uiCamera = this.canvas.rootCanvas.worldCamera.GetComponent<UICamera>();
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x0009B5E0 File Offset: 0x000997E0
		private void OnEnable()
		{
			CombatHealthBarViewer.instancesList.Add(this);
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x0009B5F0 File Offset: 0x000997F0
		private void OnDisable()
		{
			CombatHealthBarViewer.instancesList.Remove(this);
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				this.Remove(i);
			}
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x0009B628 File Offset: 0x00099828
		private void Remove(int trackedVictimIndex)
		{
			this.Remove(trackedVictimIndex, this.victimToHealthBarInfo[this.trackedVictims[trackedVictimIndex]]);
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x0009B648 File Offset: 0x00099848
		private void Remove(int trackedVictimIndex, CombatHealthBarViewer.HealthBarInfo healthBarInfo)
		{
			this.trackedVictims.RemoveAt(trackedVictimIndex);
			UnityEngine.Object.Destroy(healthBarInfo.healthBarRootObject);
			this.victimToHealthBarInfo.Remove(healthBarInfo.sourceHealthComponent);
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x0009B673 File Offset: 0x00099873
		private bool VictimIsValid(HealthComponent victim)
		{
			return victim && victim.alive && (this.victimToHealthBarInfo[victim].endTime > Time.time || victim == this.crosshairTarget);
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x0009B6AD File Offset: 0x000998AD
		private void LateUpdate()
		{
			this.CleanUp();
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x0009B6B8 File Offset: 0x000998B8
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

		// Token: 0x06002120 RID: 8480 RVA: 0x0009B708 File Offset: 0x00099908
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

		// Token: 0x06002121 RID: 8481 RVA: 0x0009B7B8 File Offset: 0x000999B8
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

		// Token: 0x06002122 RID: 8482 RVA: 0x0009B80C File Offset: 0x00099A0C
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

		// Token: 0x06002123 RID: 8483 RVA: 0x0009B958 File Offset: 0x00099B58
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

		// Token: 0x06002124 RID: 8484 RVA: 0x0009B978 File Offset: 0x00099B78
		private static void LayoutForCamera(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			for (int i = 0; i < CombatHealthBarViewer.instancesList.Count; i++)
			{
				CombatHealthBarViewer.instancesList[i].UpdateAllHealthbarPositions(sceneCam, camera);
			}
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x0009B9BF File Offset: 0x00099BBF
		public void SetLayoutHorizontal()
		{
			if (this.uiCamera)
			{
				CombatHealthBarViewer.LayoutForCamera(this.uiCamera);
			}
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x00004507 File Offset: 0x00002707
		public void SetLayoutVertical()
		{
		}

		// Token: 0x0400239F RID: 9119
		private static readonly List<CombatHealthBarViewer> instancesList = new List<CombatHealthBarViewer>();

		// Token: 0x040023A0 RID: 9120
		public RectTransform container;

		// Token: 0x040023A1 RID: 9121
		public GameObject healthBarPrefab;

		// Token: 0x040023A2 RID: 9122
		public float healthBarDuration;

		// Token: 0x040023A7 RID: 9127
		private const float hoverHealthBarDuration = 1f;

		// Token: 0x040023A8 RID: 9128
		private RectTransform rectTransform;

		// Token: 0x040023A9 RID: 9129
		private Canvas canvas;

		// Token: 0x040023AA RID: 9130
		private UICamera uiCamera;

		// Token: 0x040023AB RID: 9131
		private List<HealthComponent> trackedVictims = new List<HealthComponent>();

		// Token: 0x040023AC RID: 9132
		private Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo> victimToHealthBarInfo = new Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo>();

		// Token: 0x040023AD RID: 9133
		public float zPosition;

		// Token: 0x040023AE RID: 9134
		private const float overheadOffset = 1f;

		// Token: 0x020005C4 RID: 1476
		private class HealthBarInfo
		{
			// Token: 0x040023AF RID: 9135
			public HealthComponent sourceHealthComponent;

			// Token: 0x040023B0 RID: 9136
			public Transform sourceTransform;

			// Token: 0x040023B1 RID: 9137
			public GameObject healthBarRootObject;

			// Token: 0x040023B2 RID: 9138
			public Transform healthBarRootObjectTransform;

			// Token: 0x040023B3 RID: 9139
			public HealthBar healthBar;

			// Token: 0x040023B4 RID: 9140
			public float verticalOffset;

			// Token: 0x040023B5 RID: 9141
			public float endTime = float.NegativeInfinity;
		}
	}
}
