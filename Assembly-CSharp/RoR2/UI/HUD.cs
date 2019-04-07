using System;
using System.Collections.Generic;
using Assets.RoR2.Scripts.GameBehaviors.UI;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E6 RID: 1510
	[RequireComponent(typeof(MPEventSystemProvider))]
	[RequireComponent(typeof(Canvas))]
	public class HUD : MonoBehaviour
	{
		// Token: 0x060021D0 RID: 8656 RVA: 0x0009FEA8 File Offset: 0x0009E0A8
		private static void OnUICameraPreRender(UICamera uiCamera)
		{
			CameraRigController cameraRigController = uiCamera.cameraRigController;
			if (cameraRigController)
			{
				LocalUser localUser = cameraRigController.viewer ? cameraRigController.viewer.localUser : null;
				if (localUser != null)
				{
					HUD.lockInstancesList = true;
					for (int i = 0; i < HUD.instancesList.Count; i++)
					{
						HUD hud = HUD.instancesList[i];
						if (hud.localUserViewer == localUser)
						{
							hud.canvas.worldCamera = uiCamera.camera;
						}
						else
						{
							GameObject gameObject = hud.gameObject;
							HUD.instancesToReenableList.Add(gameObject);
							gameObject.SetActive(false);
						}
					}
					HUD.lockInstancesList = false;
				}
			}
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x0009FF4C File Offset: 0x0009E14C
		private static void OnUICameraPostRender(UICamera uiCamera)
		{
			HUD.lockInstancesList = true;
			for (int i = 0; i < HUD.instancesToReenableList.Count; i++)
			{
				HUD.instancesToReenableList[i].SetActive(true);
			}
			HUD.instancesToReenableList.Clear();
			HUD.lockInstancesList = false;
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x0009FF95 File Offset: 0x0009E195
		public void OnEnable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Add(this);
			}
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x0009FFA9 File Offset: 0x0009E1A9
		public void OnDisable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Remove(this);
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060021D4 RID: 8660 RVA: 0x0009FFBE File Offset: 0x0009E1BE
		public GameObject targetBodyObject
		{
			get
			{
				if (!this.targetMaster)
				{
					return null;
				}
				return this.targetMaster.GetBodyObject();
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060021D5 RID: 8661 RVA: 0x0009FFDA File Offset: 0x0009E1DA
		// (set) Token: 0x060021D6 RID: 8662 RVA: 0x0009FFE2 File Offset: 0x0009E1E2
		public CharacterMaster targetMaster { get; set; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060021D7 RID: 8663 RVA: 0x0009FFEB File Offset: 0x0009E1EB
		// (set) Token: 0x060021D8 RID: 8664 RVA: 0x0009FFF3 File Offset: 0x0009E1F3
		public LocalUser localUserViewer
		{
			get
			{
				return this._localUserViewer;
			}
			set
			{
				if (this._localUserViewer != value)
				{
					this._localUserViewer = value;
					this.eventSystemProvider.eventSystem = this._localUserViewer.eventSystem;
				}
			}
		}

		// Token: 0x060021D9 RID: 8665 RVA: 0x000A001B File Offset: 0x0009E21B
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
			this.canvas = base.GetComponent<Canvas>();
			if (this.scoreboardPanel)
			{
				this.scoreboardPanel.SetActive(false);
			}
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x000A004E File Offset: 0x0009E24E
		private void Start()
		{
			this.mainContainer.SetActive(HUD.HUDEnableConVar.instance.boolValue);
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x000A0068 File Offset: 0x0009E268
		public void Update()
		{
			NetworkUser networkUser;
			if (!this.targetMaster)
			{
				networkUser = null;
			}
			else
			{
				PlayerCharacterMasterController component = this.targetMaster.GetComponent<PlayerCharacterMasterController>();
				networkUser = ((component != null) ? component.networkUser : null);
			}
			NetworkUser networkUser2 = networkUser;
			PlayerCharacterMasterController playerCharacterMasterController = this.targetMaster ? this.targetMaster.GetComponent<PlayerCharacterMasterController>() : null;
			Inventory inventory = this.targetMaster ? this.targetMaster.inventory : null;
			CharacterBody characterBody = this.targetBodyObject ? this.targetBodyObject.GetComponent<CharacterBody>() : null;
			if (this.healthBar && this.targetBodyObject)
			{
				this.healthBar.source = this.targetBodyObject.GetComponent<HealthComponent>();
			}
			if (this.expBar)
			{
				this.expBar.source = this.targetMaster;
			}
			if (this.levelText)
			{
				this.levelText.source = this.targetMaster;
			}
			if (this.moneyText)
			{
				this.moneyText.targetValue = (int)(this.targetMaster ? this.targetMaster.money : 0u);
			}
			if (this.lunarCoinContainer)
			{
				bool flag = this.localUserViewer != null && this.localUserViewer.userProfile.totalCollectedCoins > 0u;
				uint targetValue = networkUser2 ? networkUser2.lunarCoins : 0u;
				this.lunarCoinContainer.SetActive(flag);
				if (flag && this.lunarCoinText)
				{
					this.lunarCoinText.targetValue = (int)targetValue;
				}
			}
			if (this.itemInventoryDisplay)
			{
				this.itemInventoryDisplay.SetSubscribedInventory(inventory);
			}
			if (this.targetBodyObject)
			{
				SkillLocator component2 = this.targetBodyObject.GetComponent<SkillLocator>();
				if (component2)
				{
					if (this.skillIcons.Length != 0 && this.skillIcons[0])
					{
						this.skillIcons[0].targetSkillSlot = SkillSlot.Primary;
						this.skillIcons[0].targetSkill = component2.primary;
						this.skillIcons[0].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 1 && this.skillIcons[1])
					{
						this.skillIcons[1].targetSkillSlot = SkillSlot.Secondary;
						this.skillIcons[1].targetSkill = component2.secondary;
						this.skillIcons[1].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 2 && this.skillIcons[2])
					{
						this.skillIcons[2].targetSkillSlot = SkillSlot.Utility;
						this.skillIcons[2].targetSkill = component2.utility;
						this.skillIcons[2].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 3 && this.skillIcons[3])
					{
						this.skillIcons[3].targetSkillSlot = SkillSlot.Special;
						this.skillIcons[3].targetSkill = component2.special;
						this.skillIcons[3].playerCharacterMasterController = playerCharacterMasterController;
					}
				}
			}
			foreach (EquipmentIcon equipmentIcon in this.equipmentIcons)
			{
				equipmentIcon.targetInventory = inventory;
				equipmentIcon.targetEquipmentSlot = (this.targetBodyObject ? this.targetBodyObject.GetComponent<EquipmentSlot>() : null);
				equipmentIcon.playerCharacterMasterController = (this.targetMaster ? this.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
			}
			if (this.buffDisplay)
			{
				this.buffDisplay.source = characterBody;
			}
			if (this.allyCardManager)
			{
				this.allyCardManager.sourceGameObject = this.targetBodyObject;
			}
			if (this.scoreboardPanel)
			{
				bool active = this.localUserViewer != null && this.localUserViewer.inputPlayer != null && this.localUserViewer.inputPlayer.GetButton("info");
				this.scoreboardPanel.SetActive(active);
			}
			if (this.speedometer)
			{
				this.speedometer.targetTransform = (this.targetBodyObject ? this.targetBodyObject.transform : null);
			}
			if (this.objectivePanelController)
			{
				this.objectivePanelController.SetCurrentMaster(this.targetMaster);
			}
			if (this.combatHealthBarViewer)
			{
				this.combatHealthBarViewer.crosshairTarget = (this.cameraRigController.lastCrosshairHurtBox ? this.cameraRigController.lastCrosshairHurtBox.healthComponent : null);
				this.combatHealthBarViewer.viewerBody = characterBody;
				this.combatHealthBarViewer.viewerBodyObject = this.targetBodyObject;
				this.combatHealthBarViewer.viewerTeamIndex = TeamComponent.GetObjectTeam(this.targetBodyObject);
			}
		}

		// Token: 0x040024C5 RID: 9413
		private static List<HUD> instancesList = new List<HUD>();

		// Token: 0x040024C6 RID: 9414
		private static bool lockInstancesList = false;

		// Token: 0x040024C7 RID: 9415
		private static List<GameObject> instancesToReenableList = new List<GameObject>();

		// Token: 0x040024C9 RID: 9417
		private LocalUser _localUserViewer;

		// Token: 0x040024CA RID: 9418
		[Tooltip("Immediate child of this object which contains all other UI.")]
		public GameObject mainContainer;

		// Token: 0x040024CB RID: 9419
		[NonSerialized]
		public CameraRigController cameraRigController;

		// Token: 0x040024CC RID: 9420
		public HealthBar healthBar;

		// Token: 0x040024CD RID: 9421
		public ExpBar expBar;

		// Token: 0x040024CE RID: 9422
		public LevelText levelText;

		// Token: 0x040024CF RID: 9423
		public MoneyText moneyText;

		// Token: 0x040024D0 RID: 9424
		public GameObject lunarCoinContainer;

		// Token: 0x040024D1 RID: 9425
		public MoneyText lunarCoinText;

		// Token: 0x040024D2 RID: 9426
		public SkillIcon[] skillIcons;

		// Token: 0x040024D3 RID: 9427
		public EquipmentIcon[] equipmentIcons;

		// Token: 0x040024D4 RID: 9428
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x040024D5 RID: 9429
		public BuffDisplay buffDisplay;

		// Token: 0x040024D6 RID: 9430
		public AllyCardManager allyCardManager;

		// Token: 0x040024D7 RID: 9431
		public ContextManager contextManager;

		// Token: 0x040024D8 RID: 9432
		public GameObject scoreboardPanel;

		// Token: 0x040024D9 RID: 9433
		public GameObject mainUIPanel;

		// Token: 0x040024DA RID: 9434
		public GameObject cinematicPanel;

		// Token: 0x040024DB RID: 9435
		public HUDSpeedometer speedometer;

		// Token: 0x040024DC RID: 9436
		public ObjectivePanelController objectivePanelController;

		// Token: 0x040024DD RID: 9437
		public CombatHealthBarViewer combatHealthBarViewer;

		// Token: 0x040024DE RID: 9438
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x040024DF RID: 9439
		private Canvas canvas;

		// Token: 0x020005E7 RID: 1511
		private class HUDEnableConVar : BaseConVar
		{
			// Token: 0x060021DD RID: 8669 RVA: 0x00037E38 File Offset: 0x00036038
			private HUDEnableConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060021DE RID: 8670 RVA: 0x000A050C File Offset: 0x0009E70C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (this.boolValue != flag)
					{
						this.boolValue = flag;
						foreach (HUD hud in HUD.instancesList)
						{
							hud.mainContainer.SetActive(this.boolValue);
						}
					}
				}
			}

			// Token: 0x060021DF RID: 8671 RVA: 0x000A0588 File Offset: 0x0009E788
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040024E0 RID: 9440
			public static HUD.HUDEnableConVar instance = new HUD.HUDEnableConVar("hud_enable", ConVarFlags.None, "1", "Enable/disable the HUD.");

			// Token: 0x040024E1 RID: 9441
			public bool boolValue;
		}
	}
}
