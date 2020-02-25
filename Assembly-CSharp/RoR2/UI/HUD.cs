using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005CB RID: 1483
	[RequireComponent(typeof(MPEventSystemProvider))]
	[RequireComponent(typeof(Canvas))]
	public class HUD : MonoBehaviour
	{
		// Token: 0x0600230F RID: 8975 RVA: 0x000992A8 File Offset: 0x000974A8
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

		// Token: 0x06002310 RID: 8976 RVA: 0x0009934C File Offset: 0x0009754C
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

		// Token: 0x06002311 RID: 8977 RVA: 0x00099395 File Offset: 0x00097595
		public void OnEnable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Add(this);
			}
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x000993A9 File Offset: 0x000975A9
		public void OnDisable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Remove(this);
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06002313 RID: 8979 RVA: 0x000993BE File Offset: 0x000975BE
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

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06002314 RID: 8980 RVA: 0x000993DA File Offset: 0x000975DA
		// (set) Token: 0x06002315 RID: 8981 RVA: 0x000993E2 File Offset: 0x000975E2
		public CharacterMaster targetMaster { get; set; }

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06002316 RID: 8982 RVA: 0x000993EB File Offset: 0x000975EB
		// (set) Token: 0x06002317 RID: 8983 RVA: 0x000993F3 File Offset: 0x000975F3
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

		// Token: 0x06002318 RID: 8984 RVA: 0x0009941B File Offset: 0x0009761B
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
			this.canvas = base.GetComponent<Canvas>();
			if (this.scoreboardPanel)
			{
				this.scoreboardPanel.SetActive(false);
			}
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x0009944E File Offset: 0x0009764E
		private void Start()
		{
			this.mainContainer.SetActive(HUD.HUDEnableConVar.instance.boolValue);
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x00099468 File Offset: 0x00097668
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
				this.moneyText.targetValue = (int)(this.targetMaster ? this.targetMaster.money : 0U);
			}
			if (this.lunarCoinContainer)
			{
				bool flag = this.localUserViewer != null && this.localUserViewer.userProfile.totalCollectedCoins > 0U;
				uint targetValue = networkUser2 ? networkUser2.lunarCoins : 0U;
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
			if (this.targetBodyObject != this.previousTargetBodyObject)
			{
				this.previousTargetBodyObject = this.targetBodyObject;
				Action<HUD> action = HUD.onHudTargetChangedGlobal;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x0600231B RID: 8987 RVA: 0x00099938 File Offset: 0x00097B38
		// (remove) Token: 0x0600231C RID: 8988 RVA: 0x0009996C File Offset: 0x00097B6C
		public static event Action<HUD> onHudTargetChangedGlobal;

		// Token: 0x040020E8 RID: 8424
		private static List<HUD> instancesList = new List<HUD>();

		// Token: 0x040020E9 RID: 8425
		private static bool lockInstancesList = false;

		// Token: 0x040020EA RID: 8426
		private static List<GameObject> instancesToReenableList = new List<GameObject>();

		// Token: 0x040020EB RID: 8427
		public static readonly ReadOnlyCollection<HUD> readOnlyInstanceList = HUD.instancesList.AsReadOnly();

		// Token: 0x040020ED RID: 8429
		private LocalUser _localUserViewer;

		// Token: 0x040020EE RID: 8430
		[Tooltip("Immediate child of this object which contains all other UI.")]
		public GameObject mainContainer;

		// Token: 0x040020EF RID: 8431
		[NonSerialized]
		public CameraRigController cameraRigController;

		// Token: 0x040020F0 RID: 8432
		public HealthBar healthBar;

		// Token: 0x040020F1 RID: 8433
		public ExpBar expBar;

		// Token: 0x040020F2 RID: 8434
		public LevelText levelText;

		// Token: 0x040020F3 RID: 8435
		public MoneyText moneyText;

		// Token: 0x040020F4 RID: 8436
		public GameObject lunarCoinContainer;

		// Token: 0x040020F5 RID: 8437
		public MoneyText lunarCoinText;

		// Token: 0x040020F6 RID: 8438
		public SkillIcon[] skillIcons;

		// Token: 0x040020F7 RID: 8439
		public EquipmentIcon[] equipmentIcons;

		// Token: 0x040020F8 RID: 8440
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x040020F9 RID: 8441
		public BuffDisplay buffDisplay;

		// Token: 0x040020FA RID: 8442
		public AllyCardManager allyCardManager;

		// Token: 0x040020FB RID: 8443
		public ContextManager contextManager;

		// Token: 0x040020FC RID: 8444
		public GameObject scoreboardPanel;

		// Token: 0x040020FD RID: 8445
		public GameObject mainUIPanel;

		// Token: 0x040020FE RID: 8446
		public GameObject cinematicPanel;

		// Token: 0x040020FF RID: 8447
		public HUDSpeedometer speedometer;

		// Token: 0x04002100 RID: 8448
		public ObjectivePanelController objectivePanelController;

		// Token: 0x04002101 RID: 8449
		public CombatHealthBarViewer combatHealthBarViewer;

		// Token: 0x04002102 RID: 8450
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x04002103 RID: 8451
		private Canvas canvas;

		// Token: 0x04002104 RID: 8452
		private GameObject previousTargetBodyObject;

		// Token: 0x020005CC RID: 1484
		private class HUDEnableConVar : BaseConVar
		{
			// Token: 0x0600231E RID: 8990 RVA: 0x0000972B File Offset: 0x0000792B
			private HUDEnableConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600231F RID: 8991 RVA: 0x000999A0 File Offset: 0x00097BA0
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

			// Token: 0x06002320 RID: 8992 RVA: 0x00099A1C File Offset: 0x00097C1C
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04002106 RID: 8454
			public static HUD.HUDEnableConVar instance = new HUD.HUDEnableConVar("hud_enable", ConVarFlags.None, "1", "Enable/disable the HUD.");

			// Token: 0x04002107 RID: 8455
			public bool boolValue;
		}
	}
}
