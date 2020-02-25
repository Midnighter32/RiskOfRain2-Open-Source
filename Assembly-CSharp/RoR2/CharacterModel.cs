using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RoR2.UI;
using RoR2.WwiseUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000191 RID: 401
	public class CharacterModel : MonoBehaviour
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000860 RID: 2144 RVA: 0x00024350 File Offset: 0x00022550
		// (set) Token: 0x06000861 RID: 2145 RVA: 0x00024358 File Offset: 0x00022558
		public VisibilityLevel visibility
		{
			get
			{
				return this._visibility;
			}
			set
			{
				if (this._visibility != value)
				{
					this._visibility = value;
					this.materialsDirty = true;
				}
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x00024371 File Offset: 0x00022571
		// (set) Token: 0x06000863 RID: 2147 RVA: 0x00024379 File Offset: 0x00022579
		public bool isGhost
		{
			get
			{
				return this._isGhost;
			}
			set
			{
				if (this._isGhost != value)
				{
					this._isGhost = value;
					this.materialsDirty = true;
				}
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x00024394 File Offset: 0x00022594
		private void Awake()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			HurtBoxGroup component = base.GetComponent<HurtBoxGroup>();
			this.coreTransform = base.transform;
			if (component)
			{
				HurtBox mainHurtBox = component.mainHurtBox;
				this.coreTransform = (((mainHurtBox != null) ? mainHurtBox.transform : null) ?? this.coreTransform);
				HurtBox[] hurtBoxes = component.hurtBoxes;
				if (hurtBoxes.Length != 0)
				{
					this.hurtBoxInfos = new CharacterModel.HurtBoxInfo[hurtBoxes.Length];
					for (int i = 0; i < hurtBoxes.Length; i++)
					{
						this.hurtBoxInfos[i] = new CharacterModel.HurtBoxInfo(hurtBoxes[i]);
					}
				}
			}
			this.propertyStorage = new MaterialPropertyBlock();
			foreach (CharacterModel.RendererInfo rendererInfo in this.baseRendererInfos)
			{
				if (rendererInfo.renderer is SkinnedMeshRenderer)
				{
					this.mainSkinnedMeshRenderer = (SkinnedMeshRenderer)rendererInfo.renderer;
					break;
				}
			}
			if (this.body && Util.IsPrefab(this.body.gameObject) && !Util.IsPrefab(base.gameObject))
			{
				this.body = null;
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x000244A8 File Offset: 0x000226A8
		private void Start()
		{
			this.visibility = VisibilityLevel.Invisible;
			this.UpdateMaterials();
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x000244B8 File Offset: 0x000226B8
		private void OnEnable()
		{
			InstanceTracker.Add<CharacterModel>(this);
			if (this.body != null)
			{
				this.rtpcEliteEnemy = new RtpcSetter("eliteEnemy", this.body.gameObject);
				this.body.onInventoryChanged += this.OnInventoryChanged;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00024505 File Offset: 0x00022705
		private void OnDisable()
		{
			if (this.body != null)
			{
				this.body.onInventoryChanged -= this.OnInventoryChanged;
			}
			InstanceTracker.Remove<CharacterModel>(this);
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0002452C File Offset: 0x0002272C
		private void OnInventoryChanged()
		{
			if (this.body)
			{
				Inventory inventory = this.body.inventory;
				if (inventory)
				{
					this.UpdateItemDisplay(inventory);
					this.inventoryEquipmentIndex = inventory.GetEquipmentIndex();
					this.SetEquipmentDisplay(this.inventoryEquipmentIndex);
				}
			}
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0002457C File Offset: 0x0002277C
		private void InstanceUpdate()
		{
			if (this.myEliteIndex == EliteIndex.Poison)
			{
				this.lightColorOverride = new Color?(CharacterModel.poisonEliteLightColor);
				this.particleMaterialOverride = CharacterModel.elitePoisonParticleReplacementMaterial;
			}
			else if (this.myEliteIndex == EliteIndex.Haunted)
			{
				this.lightColorOverride = new Color?(CharacterModel.hauntedEliteLightColor);
				this.particleMaterialOverride = CharacterModel.eliteHauntedParticleReplacementMaterial;
			}
			else
			{
				this.lightColorOverride = null;
				this.particleMaterialOverride = null;
			}
			this.UpdateGoldAffix();
			this.UpdatePoisonAffix();
			this.UpdateHauntedAffix();
			this.UpdateLights();
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00024600 File Offset: 0x00022800
		private void UpdateLights()
		{
			CharacterModel.LightInfo[] array = this.baseLightInfos;
			if (array.Length == 0)
			{
				return;
			}
			if (this.lightColorOverride != null)
			{
				Color value = this.lightColorOverride.Value;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].light.color = value;
				}
				return;
			}
			for (int j = 0; j < array.Length; j++)
			{
				ref CharacterModel.LightInfo ptr = ref array[j];
				ptr.light.color = ptr.defaultColor;
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0002467D File Offset: 0x0002287D
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onLateUpdate += CharacterModel.StaticUpdate;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00024690 File Offset: 0x00022890
		private static void StaticUpdate()
		{
			foreach (CharacterModel characterModel in InstanceTracker.GetInstancesList<CharacterModel>())
			{
				characterModel.InstanceUpdate();
			}
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x000246E0 File Offset: 0x000228E0
		private void UpdateGoldAffix()
		{
			if (this.myEliteIndex == EliteIndex.Gold != this.goldAffixEffect)
			{
				if (!this.goldAffixEffect)
				{
					this.goldAffixEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GoldAffixEffect"), base.transform);
					ParticleSystem.ShapeModule shape = this.goldAffixEffect.GetComponent<ParticleSystem>().shape;
					if (this.mainSkinnedMeshRenderer)
					{
						shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
						shape.skinnedMeshRenderer = this.mainSkinnedMeshRenderer;
						return;
					}
				}
				else
				{
					UnityEngine.Object.Destroy(this.goldAffixEffect);
					this.goldAffixEffect = null;
				}
			}
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00024774 File Offset: 0x00022974
		private void UpdatePoisonAffix()
		{
			if ((this.myEliteIndex == EliteIndex.Poison && this.body.healthComponent.alive) != this.poisonAffixEffect)
			{
				if (!this.poisonAffixEffect)
				{
					this.poisonAffixEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PoisonAffixEffect"), base.transform);
					if (this.mainSkinnedMeshRenderer)
					{
						JitterBones[] components = this.poisonAffixEffect.GetComponents<JitterBones>();
						for (int i = 0; i < components.Length; i++)
						{
							components[i].skinnedMeshRenderer = this.mainSkinnedMeshRenderer;
						}
						return;
					}
				}
				else
				{
					UnityEngine.Object.Destroy(this.poisonAffixEffect);
					this.poisonAffixEffect = null;
				}
			}
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0002481C File Offset: 0x00022A1C
		private void UpdateHauntedAffix()
		{
			if ((this.myEliteIndex == EliteIndex.Haunted && this.body.healthComponent.alive) != this.hauntedAffixEffect)
			{
				if (!this.hauntedAffixEffect)
				{
					this.hauntedAffixEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/HauntedAffixEffect"), base.transform);
					if (this.mainSkinnedMeshRenderer)
					{
						JitterBones[] components = this.hauntedAffixEffect.GetComponents<JitterBones>();
						for (int i = 0; i < components.Length; i++)
						{
							components[i].skinnedMeshRenderer = this.mainSkinnedMeshRenderer;
						}
						return;
					}
				}
				else
				{
					UnityEngine.Object.Destroy(this.hauntedAffixEffect);
					this.hauntedAffixEffect = null;
				}
			}
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x000248C4 File Offset: 0x00022AC4
		private void OnValidate()
		{
			if (Application.isPlaying)
			{
				return;
			}
			for (int i = 0; i < this.baseLightInfos.Length; i++)
			{
				ref CharacterModel.LightInfo ptr = ref this.baseLightInfos[i];
				if (ptr.light)
				{
					ptr.defaultColor = ptr.light.color;
				}
			}
			if (!this.itemDisplayRuleSet)
			{
				Debug.LogErrorFormat("CharacterModel \"{0}\" does not have the itemDisplayRuleSet field assigned.", new object[]
				{
					base.gameObject
				});
			}
			if (this.autoPopulateLightInfos)
			{
				CharacterModel.LightInfo[] first = (from light in base.GetComponentsInChildren<Light>()
				select new CharacterModel.LightInfo(light)).ToArray<CharacterModel.LightInfo>();
				if (!first.SequenceEqual(this.baseLightInfos))
				{
					this.baseLightInfos = first;
				}
			}
			if (this.autoPopulateParticleSystemInfos)
			{
				CharacterModel.ParticleSystemInfo[] first2 = (from ps in base.GetComponentsInChildren<ParticleSystem>()
				select new CharacterModel.ParticleSystemInfo(ps)).ToArray<CharacterModel.ParticleSystemInfo>();
				if (!first2.SequenceEqual(this.baseParticleSystemInfos))
				{
					this.baseParticleSystemInfos = first2;
				}
			}
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x000249D8 File Offset: 0x00022BD8
		private static void RefreshObstructorsForCamera(CameraRigController cameraRigController)
		{
			Vector3 position = cameraRigController.transform.position;
			foreach (CharacterModel characterModel in InstanceTracker.GetInstancesList<CharacterModel>())
			{
				characterModel.fade = Mathf.Clamp01(Util.Remap(characterModel.GetNearestHurtBoxDistance(position), cameraRigController.fadeStartDistance, cameraRigController.fadeEndDistance, 0f, 1f));
			}
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00024A5C File Offset: 0x00022C5C
		private float GetNearestHurtBoxDistance(Vector3 cameraPosition)
		{
			float num = float.PositiveInfinity;
			for (int i = 0; i < this.hurtBoxInfos.Length; i++)
			{
				float num2 = Vector3.Distance(this.hurtBoxInfos[i].transform.position, cameraPosition) - this.hurtBoxInfos[i].estimatedRadius;
				if (num2 < num)
				{
					num = Mathf.Min(num2, num);
				}
			}
			return num;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x00024AC0 File Offset: 0x00022CC0
		private void UpdateForCamera(CameraRigController cameraRigController)
		{
			this.visibility = VisibilityLevel.Visible;
			bool flag = false;
			float target = 1f;
			if (this.body)
			{
				if (flag)
				{
					target = 0f;
				}
				if (this.body.HasBuff(BuffIndex.Cloak) || this.body.HasBuff(BuffIndex.AffixHauntedRecipient))
				{
					TeamIndex teamIndex = TeamIndex.Neutral;
					TeamComponent component = this.body.GetComponent<TeamComponent>();
					if (component)
					{
						teamIndex = component.teamIndex;
					}
					this.visibility = ((cameraRigController.targetTeamIndex == teamIndex) ? VisibilityLevel.Revealed : VisibilityLevel.Cloaked);
				}
			}
			this.firstPersonFade = Mathf.MoveTowards(this.firstPersonFade, target, Time.deltaTime / 0.25f);
			this.fade *= this.firstPersonFade;
			if (this.fade <= 0f || this.invisibilityCount > 0)
			{
				this.visibility = VisibilityLevel.Invisible;
			}
			this.UpdateOverlays();
			if (this.materialsDirty)
			{
				this.UpdateMaterials();
				this.materialsDirty = false;
			}
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00024BA8 File Offset: 0x00022DA8
		static CharacterModel()
		{
			SceneCamera.onSceneCameraPreRender += CharacterModel.OnSceneCameraPreRender;
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x00024C8C File Offset: 0x00022E8C
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				CharacterModel.RefreshObstructorsForCamera(sceneCamera.cameraRigController);
			}
			if (sceneCamera.cameraRigController)
			{
				foreach (CharacterModel characterModel in InstanceTracker.GetInstancesList<CharacterModel>())
				{
					characterModel.UpdateForCamera(sceneCamera.cameraRigController);
				}
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x00024D08 File Offset: 0x00022F08
		private void InstantiateDisplayRuleGroup(DisplayRuleGroup displayRuleGroup, ItemIndex itemIndex, EquipmentIndex equipmentIndex)
		{
			if (displayRuleGroup.rules != null)
			{
				for (int i = 0; i < displayRuleGroup.rules.Length; i++)
				{
					ItemDisplayRule itemDisplayRule = displayRuleGroup.rules[i];
					ItemDisplayRuleType ruleType = itemDisplayRule.ruleType;
					if (ruleType != ItemDisplayRuleType.ParentedPrefab)
					{
						if (ruleType == ItemDisplayRuleType.LimbMask)
						{
							CharacterModel.LimbMaskDisplay item = new CharacterModel.LimbMaskDisplay
							{
								itemIndex = itemIndex,
								equipmentIndex = equipmentIndex
							};
							item.Apply(this, itemDisplayRule.limbMask);
							this.limbMaskDisplays.Add(item);
						}
					}
					else if (this.childLocator)
					{
						Transform transform = this.childLocator.FindChild(itemDisplayRule.childName);
						if (transform)
						{
							CharacterModel.ParentedPrefabDisplay item2 = new CharacterModel.ParentedPrefabDisplay
							{
								itemIndex = itemIndex,
								equipmentIndex = equipmentIndex
							};
							item2.Apply(this, itemDisplayRule.followerPrefab, transform, itemDisplayRule.localPos, Quaternion.Euler(itemDisplayRule.localAngles), itemDisplayRule.localScale);
							this.parentedPrefabDisplays.Add(item2);
						}
					}
				}
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00024E14 File Offset: 0x00023014
		private void SetEquipmentDisplay(EquipmentIndex newEquipmentIndex)
		{
			if (newEquipmentIndex == this.currentEquipmentDisplayIndex)
			{
				return;
			}
			for (int i = this.parentedPrefabDisplays.Count - 1; i >= 0; i--)
			{
				if (this.parentedPrefabDisplays[i].equipmentIndex != EquipmentIndex.None)
				{
					this.parentedPrefabDisplays[i].Undo();
					this.parentedPrefabDisplays.RemoveAt(i);
				}
			}
			for (int j = this.limbMaskDisplays.Count - 1; j >= 0; j--)
			{
				if (this.limbMaskDisplays[j].equipmentIndex != EquipmentIndex.None)
				{
					this.limbMaskDisplays[j].Undo(this);
					this.limbMaskDisplays.RemoveAt(j);
				}
			}
			this.currentEquipmentDisplayIndex = newEquipmentIndex;
			if (this.itemDisplayRuleSet)
			{
				DisplayRuleGroup equipmentDisplayRuleGroup = this.itemDisplayRuleSet.GetEquipmentDisplayRuleGroup(newEquipmentIndex);
				this.InstantiateDisplayRuleGroup(equipmentDisplayRuleGroup, ItemIndex.None, newEquipmentIndex);
			}
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x00024EF0 File Offset: 0x000230F0
		private void EnableItemDisplay(ItemIndex itemIndex)
		{
			if (this.enabledItemDisplays.HasItem(itemIndex))
			{
				return;
			}
			this.enabledItemDisplays.AddItem(itemIndex);
			if (this.itemDisplayRuleSet)
			{
				DisplayRuleGroup itemDisplayRuleGroup = this.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex);
				this.InstantiateDisplayRuleGroup(itemDisplayRuleGroup, itemIndex, EquipmentIndex.None);
			}
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00024F3C File Offset: 0x0002313C
		private void DisableItemDisplay(ItemIndex itemIndex)
		{
			if (!this.enabledItemDisplays.HasItem(itemIndex))
			{
				return;
			}
			this.enabledItemDisplays.RemoveItem(itemIndex);
			for (int i = this.parentedPrefabDisplays.Count - 1; i >= 0; i--)
			{
				if (this.parentedPrefabDisplays[i].itemIndex == itemIndex)
				{
					this.parentedPrefabDisplays[i].Undo();
					this.parentedPrefabDisplays.RemoveAt(i);
				}
			}
			for (int j = this.limbMaskDisplays.Count - 1; j >= 0; j--)
			{
				if (this.limbMaskDisplays[j].itemIndex == itemIndex)
				{
					this.limbMaskDisplays[j].Undo(this);
					this.limbMaskDisplays.RemoveAt(j);
				}
			}
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x00025000 File Offset: 0x00023200
		public void UpdateItemDisplay(Inventory inventory)
		{
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				if (inventory.GetItemCount(itemIndex) > 0)
				{
					this.EnableItemDisplay(itemIndex);
				}
				else
				{
					this.DisableItemDisplay(itemIndex);
				}
				itemIndex++;
			}
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x0002503C File Offset: 0x0002323C
		public void HighlightItemDisplay(ItemIndex itemIndex)
		{
			if (!this.enabledItemDisplays.HasItem(itemIndex))
			{
				return;
			}
			string path = "Prefabs/UI/HighlightTier1Item";
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			if (itemDef != null)
			{
				switch (itemDef.tier)
				{
				case ItemTier.Tier1:
					path = "Prefabs/UI/HighlightTier1Item";
					break;
				case ItemTier.Tier2:
					path = "Prefabs/UI/HighlightTier2Item";
					break;
				case ItemTier.Tier3:
					path = "Prefabs/UI/HighlightTier3Item";
					break;
				case ItemTier.Lunar:
					path = "Prefabs/UI/HighlightLunarItem";
					break;
				}
			}
			for (int i = this.parentedPrefabDisplays.Count - 1; i >= 0; i--)
			{
				if (this.parentedPrefabDisplays[i].itemIndex == itemIndex)
				{
					GameObject instance = this.parentedPrefabDisplays[i].instance;
					if (instance)
					{
						Renderer componentInChildren = instance.GetComponentInChildren<Renderer>();
						if (componentInChildren && this.body)
						{
							HighlightRect.CreateHighlight(this.body.gameObject, componentInChildren, Resources.Load<GameObject>(path), -1f, false);
						}
					}
				}
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x00025130 File Offset: 0x00023330
		public List<GameObject> GetEquipmentDisplayObjects(EquipmentIndex equipmentIndex)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = this.parentedPrefabDisplays.Count - 1; i >= 0; i--)
			{
				if (this.parentedPrefabDisplays[i].equipmentIndex == equipmentIndex)
				{
					GameObject instance = this.parentedPrefabDisplays[i].instance;
					list.Add(instance);
				}
			}
			return list;
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x0002518C File Offset: 0x0002338C
		[RuntimeInitializeOnLoadMethod]
		private static void InitMaterials()
		{
			CharacterModel.revealedMaterial = Resources.Load<Material>("Materials/matRevealedEffect");
			CharacterModel.cloakedMaterial = Resources.Load<Material>("Materials/matCloakedEffect");
			CharacterModel.ghostMaterial = Resources.Load<Material>("Materials/matGhostEffect");
			CharacterModel.wolfhatMaterial = Resources.Load<Material>("Materials/matWolfhatOverlay");
			CharacterModel.energyShieldMaterial = Resources.Load<Material>("Materials/matEnergyShield");
			CharacterModel.beetleJuiceMaterial = Resources.Load<Material>("Materials/matBeetleJuice");
			CharacterModel.brittleMaterial = Resources.Load<Material>("Materials/matBrittle");
			CharacterModel.fullCritMaterial = Resources.Load<Material>("Materials/matFullCrit");
			CharacterModel.clayGooMaterial = Resources.Load<Material>("Materials/matClayGooDebuff");
			CharacterModel.slow80Material = Resources.Load<Material>("Materials/matSlow80Debuff");
			CharacterModel.immuneMaterial = Resources.Load<Material>("Materials/matImmune");
			CharacterModel.bellBuffMaterial = Resources.Load<Material>("Materials/matBellBuff");
			CharacterModel.elitePoisonOverlayMaterial = Resources.Load<Material>("Materials/matElitePoisonOverlay");
			CharacterModel.elitePoisonParticleReplacementMaterial = Resources.Load<Material>("Materials/matElitePoisonParticleReplacement");
			CharacterModel.eliteHauntedOverlayMaterial = Resources.Load<Material>("Materials/matEliteHauntedOverlay");
			CharacterModel.eliteHauntedParticleReplacementMaterial = Resources.Load<Material>("Materials/matEliteHauntedParticleReplacement");
			CharacterModel.eliteJustHauntedOverlayMaterial = Resources.Load<Material>("Materials/matEliteJustHauntedOverlay");
			CharacterModel.weakMaterial = Resources.Load<Material>("Materials/matWeakOverlay");
			CharacterModel.pulverizedMaterial = Resources.Load<Material>("Materials/matPulverizedOverlay");
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x000252B8 File Offset: 0x000234B8
		private void UpdateOverlays()
		{
			for (int i = 0; i < this.activeOverlayCount; i++)
			{
				this.currentOverlays[i] = null;
			}
			this.activeOverlayCount = 0;
			if (this.visibility == VisibilityLevel.Invisible)
			{
				return;
			}
			this.myEliteIndex = EliteCatalog.GetEquipmentEliteIndex(this.inventoryEquipmentIndex);
			bool flag = this.body && this.body.HasBuff(BuffIndex.ClayGoo);
			bool flag2 = this.body && this.body.HasBuff(BuffIndex.AffixHauntedRecipient);
			if (this.body)
			{
				this.rtpcEliteEnemy.value = ((this.myEliteIndex != EliteIndex.None) ? 1f : 0f);
				this.rtpcEliteEnemy.FlushIfChanged();
				Inventory inventory = this.body.inventory;
				this.isGhost = (inventory != null && inventory.GetItemCount(ItemIndex.Ghost) > 0);
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.ghostMaterial, this.isGhost);
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.clayGooMaterial, flag);
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.elitePoisonOverlayMaterial, this.myEliteIndex == EliteIndex.Poison || this.body.HasBuff(BuffIndex.HealingDisabled));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.eliteHauntedOverlayMaterial, this.body.HasBuff(BuffIndex.AffixHaunted));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.pulverizedMaterial, this.body.HasBuff(BuffIndex.Pulverized));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.weakMaterial, this.body.HasBuff(BuffIndex.Weak));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.fullCritMaterial, this.body.HasBuff(BuffIndex.FullCrit));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.wolfhatMaterial, this.body.HasBuff(BuffIndex.AttackSpeedOnCrit));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.energyShieldMaterial, this.body.healthComponent && this.body.healthComponent.shield > 0f);
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.beetleJuiceMaterial, this.body.HasBuff(BuffIndex.BeetleJuice));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.immuneMaterial, this.body.HasBuff(BuffIndex.Immune));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.slow80Material, this.body.HasBuff(BuffIndex.Slow80));
				this.<UpdateOverlays>g__AddOverlay|103_0(CharacterModel.brittleMaterial, this.body.inventory && this.body.inventory.GetItemCount(ItemIndex.LunarDagger) > 0);
			}
			if (this.wasPreviouslyClayGooed && !flag)
			{
				TemporaryOverlay temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 0.6f;
				temporaryOverlay.animateShaderAlpha = true;
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = CharacterModel.clayGooMaterial;
				temporaryOverlay.AddToCharacerModel(this);
			}
			if (this.wasPreviouslyHaunted != flag2)
			{
				TemporaryOverlay temporaryOverlay2 = base.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay2.duration = 0.5f;
				temporaryOverlay2.animateShaderAlpha = true;
				temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay2.destroyComponentOnEnd = true;
				temporaryOverlay2.originalMaterial = CharacterModel.eliteJustHauntedOverlayMaterial;
				temporaryOverlay2.AddToCharacerModel(this);
			}
			this.wasPreviouslyClayGooed = flag;
			this.wasPreviouslyHaunted = flag2;
			int num = 0;
			while (num < this.temporaryOverlays.Count && this.activeOverlayCount < CharacterModel.maxOverlays)
			{
				Material[] array = this.currentOverlays;
				int num2 = this.activeOverlayCount;
				this.activeOverlayCount = num2 + 1;
				array[num2] = this.temporaryOverlays[num].materialInstance;
				num++;
			}
			this.wasPreviouslyClayGooed = flag;
			this.wasPreviouslyHaunted = flag2;
			this.materialsDirty = true;
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00025634 File Offset: 0x00023834
		[RuntimeInitializeOnLoadMethod]
		private static void InitSharedMaterialsArrays()
		{
			CharacterModel.sharedMaterialArrays = new Material[CharacterModel.maxMaterials + 1][];
			if (CharacterModel.maxMaterials > 0)
			{
				CharacterModel.sharedMaterialArrays[0] = Array.Empty<Material>();
				for (int i = 1; i < CharacterModel.sharedMaterialArrays.Length; i++)
				{
					CharacterModel.sharedMaterialArrays[i] = new Material[i];
				}
			}
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x00025688 File Offset: 0x00023888
		private void UpdateRendererMaterials(Renderer renderer, Material defaultMaterial, bool ignoreOverlays)
		{
			Material material = null;
			switch (this.visibility)
			{
			case VisibilityLevel.Invisible:
				renderer.sharedMaterial = null;
				return;
			case VisibilityLevel.Cloaked:
				if (!ignoreOverlays)
				{
					ignoreOverlays = true;
					material = CharacterModel.cloakedMaterial;
				}
				break;
			case VisibilityLevel.Revealed:
				if (!ignoreOverlays)
				{
					material = CharacterModel.revealedMaterial;
				}
				break;
			case VisibilityLevel.Visible:
				if (!ignoreOverlays)
				{
					material = (this.isGhost ? CharacterModel.ghostMaterial : defaultMaterial);
				}
				else
				{
					material = (this.particleMaterialOverride ? this.particleMaterialOverride : defaultMaterial);
				}
				break;
			}
			int num = ignoreOverlays ? 0 : this.activeOverlayCount;
			if (material)
			{
				num++;
			}
			Material[] array = CharacterModel.sharedMaterialArrays[num];
			int num2 = 0;
			if (material)
			{
				array[num2++] = material;
			}
			if (!ignoreOverlays)
			{
				for (int i = 0; i < this.activeOverlayCount; i++)
				{
					array[num2++] = this.currentOverlays[i];
				}
			}
			renderer.sharedMaterials = array;
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x00025768 File Offset: 0x00023968
		private void UpdateMaterials()
		{
			if (this.visibility == VisibilityLevel.Invisible)
			{
				for (int i = this.baseRendererInfos.Length - 1; i >= 0; i--)
				{
					CharacterModel.RendererInfo rendererInfo = this.baseRendererInfos[i];
					rendererInfo.renderer.shadowCastingMode = ShadowCastingMode.Off;
					rendererInfo.renderer.enabled = false;
				}
			}
			else
			{
				for (int j = this.baseRendererInfos.Length - 1; j >= 0; j--)
				{
					CharacterModel.RendererInfo rendererInfo2 = this.baseRendererInfos[j];
					rendererInfo2.renderer.shadowCastingMode = rendererInfo2.defaultShadowCastingMode;
					rendererInfo2.renderer.enabled = true;
				}
			}
			Color value = Color.black;
			if (this.body && this.body.healthComponent)
			{
				float num = Mathf.Clamp01(1f - this.body.healthComponent.timeSinceLastHit / CharacterModel.hitFlashDuration);
				float num2 = Mathf.Pow(Mathf.Clamp01(1f - this.body.healthComponent.timeSinceLastHeal / CharacterModel.healFlashDuration), 0.5f);
				if (num2 > num)
				{
					value = CharacterModel.healFlashColor * num2;
				}
				else
				{
					value = ((this.body.healthComponent.shield > 0f) ? CharacterModel.hitFlashShieldColor : CharacterModel.hitFlashBaseColor) * num;
				}
			}
			for (int k = this.baseRendererInfos.Length - 1; k >= 0; k--)
			{
				Renderer renderer = this.baseRendererInfos[k].renderer;
				this.UpdateRendererMaterials(renderer, this.baseRendererInfos[k].defaultMaterial, this.baseRendererInfos[k].ignoreOverlays);
				renderer.GetPropertyBlock(this.propertyStorage);
				this.propertyStorage.SetColor(CommonShaderProperties._FlashColor, value);
				this.propertyStorage.SetFloat(CommonShaderProperties._Fade, this.fade);
				this.propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, (float)(this.myEliteIndex + 1));
				this.propertyStorage.SetFloat(CommonShaderProperties._LimbPrimeMask, this.limbFlagSet.materialMaskValue);
				renderer.SetPropertyBlock(this.propertyStorage);
			}
			for (int l = 0; l < this.parentedPrefabDisplays.Count; l++)
			{
				ItemDisplay itemDisplay = this.parentedPrefabDisplays[l].itemDisplay;
				itemDisplay.SetVisibilityLevel(this.visibility);
				for (int m = 0; m < itemDisplay.rendererInfos.Length; m++)
				{
					Renderer renderer2 = itemDisplay.rendererInfos[m].renderer;
					renderer2.GetPropertyBlock(this.propertyStorage);
					this.propertyStorage.SetColor(CommonShaderProperties._FlashColor, value);
					this.propertyStorage.SetFloat(CommonShaderProperties._Fade, this.fade);
					renderer2.SetPropertyBlock(this.propertyStorage);
				}
			}
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00025B04 File Offset: 0x00023D04
		[CompilerGenerated]
		private void <UpdateOverlays>g__AddOverlay|103_0(Material overlayMaterial, bool condition)
		{
			if (this.activeOverlayCount >= CharacterModel.maxOverlays)
			{
				return;
			}
			if (condition)
			{
				Material[] array = this.currentOverlays;
				int num = this.activeOverlayCount;
				this.activeOverlayCount = num + 1;
				array[num] = overlayMaterial;
			}
		}

		// Token: 0x040008B6 RID: 2230
		public CharacterBody body;

		// Token: 0x040008B7 RID: 2231
		public ItemDisplayRuleSet itemDisplayRuleSet;

		// Token: 0x040008B8 RID: 2232
		public bool autoPopulateLightInfos = true;

		// Token: 0x040008B9 RID: 2233
		public bool autoPopulateParticleSystemInfos = true;

		// Token: 0x040008BA RID: 2234
		public CharacterModel.ParticleSystemInfo[] baseParticleSystemInfos = Array.Empty<CharacterModel.ParticleSystemInfo>();

		// Token: 0x040008BB RID: 2235
		[FormerlySerializedAs("rendererInfos")]
		public CharacterModel.RendererInfo[] baseRendererInfos = Array.Empty<CharacterModel.RendererInfo>();

		// Token: 0x040008BC RID: 2236
		public CharacterModel.LightInfo[] baseLightInfos = Array.Empty<CharacterModel.LightInfo>();

		// Token: 0x040008BD RID: 2237
		private ChildLocator childLocator;

		// Token: 0x040008BE RID: 2238
		private GameObject goldAffixEffect;

		// Token: 0x040008BF RID: 2239
		private CharacterModel.HurtBoxInfo[] hurtBoxInfos = Array.Empty<CharacterModel.HurtBoxInfo>();

		// Token: 0x040008C0 RID: 2240
		private Transform coreTransform;

		// Token: 0x040008C1 RID: 2241
		private static readonly Color hitFlashBaseColor = new Color32(193, 108, 51, byte.MaxValue);

		// Token: 0x040008C2 RID: 2242
		private static readonly Color hitFlashShieldColor = new Color32(132, 159, byte.MaxValue, byte.MaxValue);

		// Token: 0x040008C3 RID: 2243
		private static readonly Color healFlashColor = new Color32(104, 196, 49, byte.MaxValue);

		// Token: 0x040008C4 RID: 2244
		private static readonly float hitFlashDuration = 0.15f;

		// Token: 0x040008C5 RID: 2245
		private static readonly float healFlashDuration = 0.35f;

		// Token: 0x040008C6 RID: 2246
		private VisibilityLevel _visibility = VisibilityLevel.Visible;

		// Token: 0x040008C7 RID: 2247
		private bool _isGhost;

		// Token: 0x040008C8 RID: 2248
		[HideInInspector]
		[NonSerialized]
		public int invisibilityCount;

		// Token: 0x040008C9 RID: 2249
		[NonSerialized]
		public List<TemporaryOverlay> temporaryOverlays = new List<TemporaryOverlay>();

		// Token: 0x040008CA RID: 2250
		private bool materialsDirty = true;

		// Token: 0x040008CB RID: 2251
		private MaterialPropertyBlock propertyStorage;

		// Token: 0x040008CC RID: 2252
		private EquipmentIndex inventoryEquipmentIndex = EquipmentIndex.None;

		// Token: 0x040008CD RID: 2253
		private EliteIndex myEliteIndex = EliteIndex.None;

		// Token: 0x040008CE RID: 2254
		private float fade = 1f;

		// Token: 0x040008CF RID: 2255
		private float firstPersonFade = 1f;

		// Token: 0x040008D0 RID: 2256
		private SkinnedMeshRenderer mainSkinnedMeshRenderer;

		// Token: 0x040008D1 RID: 2257
		private static readonly Color poisonEliteLightColor = new Color32(90, byte.MaxValue, 193, 204);

		// Token: 0x040008D2 RID: 2258
		private static readonly Color hauntedEliteLightColor = new Color32(152, 228, 217, 204);

		// Token: 0x040008D3 RID: 2259
		private Color? lightColorOverride;

		// Token: 0x040008D4 RID: 2260
		private Material particleMaterialOverride;

		// Token: 0x040008D5 RID: 2261
		private GameObject poisonAffixEffect;

		// Token: 0x040008D6 RID: 2262
		private GameObject hauntedAffixEffect;

		// Token: 0x040008D7 RID: 2263
		private float affixHauntedCloakLockoutDuration = 3f;

		// Token: 0x040008D8 RID: 2264
		private EquipmentIndex currentEquipmentDisplayIndex = EquipmentIndex.None;

		// Token: 0x040008D9 RID: 2265
		private ItemMask enabledItemDisplays;

		// Token: 0x040008DA RID: 2266
		private List<CharacterModel.ParentedPrefabDisplay> parentedPrefabDisplays = new List<CharacterModel.ParentedPrefabDisplay>();

		// Token: 0x040008DB RID: 2267
		private List<CharacterModel.LimbMaskDisplay> limbMaskDisplays = new List<CharacterModel.LimbMaskDisplay>();

		// Token: 0x040008DC RID: 2268
		private CharacterModel.LimbFlagSet limbFlagSet = new CharacterModel.LimbFlagSet();

		// Token: 0x040008DD RID: 2269
		public static Material revealedMaterial;

		// Token: 0x040008DE RID: 2270
		public static Material cloakedMaterial;

		// Token: 0x040008DF RID: 2271
		public static Material ghostMaterial;

		// Token: 0x040008E0 RID: 2272
		public static Material bellBuffMaterial;

		// Token: 0x040008E1 RID: 2273
		public static Material wolfhatMaterial;

		// Token: 0x040008E2 RID: 2274
		public static Material energyShieldMaterial;

		// Token: 0x040008E3 RID: 2275
		public static Material fullCritMaterial;

		// Token: 0x040008E4 RID: 2276
		public static Material beetleJuiceMaterial;

		// Token: 0x040008E5 RID: 2277
		public static Material brittleMaterial;

		// Token: 0x040008E6 RID: 2278
		public static Material clayGooMaterial;

		// Token: 0x040008E7 RID: 2279
		public static Material slow80Material;

		// Token: 0x040008E8 RID: 2280
		public static Material immuneMaterial;

		// Token: 0x040008E9 RID: 2281
		public static Material elitePoisonOverlayMaterial;

		// Token: 0x040008EA RID: 2282
		public static Material elitePoisonParticleReplacementMaterial;

		// Token: 0x040008EB RID: 2283
		public static Material eliteHauntedOverlayMaterial;

		// Token: 0x040008EC RID: 2284
		public static Material eliteJustHauntedOverlayMaterial;

		// Token: 0x040008ED RID: 2285
		public static Material eliteHauntedParticleReplacementMaterial;

		// Token: 0x040008EE RID: 2286
		public static Material weakMaterial;

		// Token: 0x040008EF RID: 2287
		public static Material pulverizedMaterial;

		// Token: 0x040008F0 RID: 2288
		private static readonly int maxOverlays = 6;

		// Token: 0x040008F1 RID: 2289
		private Material[] currentOverlays = new Material[CharacterModel.maxOverlays];

		// Token: 0x040008F2 RID: 2290
		private int activeOverlayCount;

		// Token: 0x040008F3 RID: 2291
		private bool wasPreviouslyClayGooed;

		// Token: 0x040008F4 RID: 2292
		private bool wasPreviouslyHaunted;

		// Token: 0x040008F5 RID: 2293
		private RtpcSetter rtpcEliteEnemy;

		// Token: 0x040008F6 RID: 2294
		private static Material[][] sharedMaterialArrays;

		// Token: 0x040008F7 RID: 2295
		private static readonly int maxMaterials = 1 + CharacterModel.maxOverlays;

		// Token: 0x02000192 RID: 402
		[Serializable]
		public struct RendererInfo : IEquatable<CharacterModel.RendererInfo>
		{
			// Token: 0x06000884 RID: 2180 RVA: 0x00025B3C File Offset: 0x00023D3C
			public bool Equals(CharacterModel.RendererInfo other)
			{
				return this.renderer == other.renderer && this.defaultMaterial == other.defaultMaterial && object.Equals(this.defaultShadowCastingMode, other.defaultShadowCastingMode) && object.Equals(this.ignoreOverlays, other.ignoreOverlays);
			}

			// Token: 0x040008F8 RID: 2296
			[PrefabReference]
			public Renderer renderer;

			// Token: 0x040008F9 RID: 2297
			public Material defaultMaterial;

			// Token: 0x040008FA RID: 2298
			public ShadowCastingMode defaultShadowCastingMode;

			// Token: 0x040008FB RID: 2299
			public bool ignoreOverlays;
		}

		// Token: 0x02000193 RID: 403
		[Serializable]
		public struct LightInfo
		{
			// Token: 0x06000885 RID: 2181 RVA: 0x00025B9F File Offset: 0x00023D9F
			public LightInfo(Light light)
			{
				this.light = light;
				this.defaultColor = light.color;
			}

			// Token: 0x040008FC RID: 2300
			public Light light;

			// Token: 0x040008FD RID: 2301
			public Color defaultColor;
		}

		// Token: 0x02000194 RID: 404
		[Serializable]
		public struct ParticleSystemInfo
		{
			// Token: 0x06000886 RID: 2182 RVA: 0x00025BB4 File Offset: 0x00023DB4
			public ParticleSystemInfo(ParticleSystem particleSystem)
			{
				this.particleSystem = particleSystem;
				this.renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
				this.defaultMaterial = this.renderer.sharedMaterial;
			}

			// Token: 0x040008FE RID: 2302
			public ParticleSystem particleSystem;

			// Token: 0x040008FF RID: 2303
			public ParticleSystemRenderer renderer;

			// Token: 0x04000900 RID: 2304
			public Material defaultMaterial;
		}

		// Token: 0x02000195 RID: 405
		private struct HurtBoxInfo
		{
			// Token: 0x06000887 RID: 2183 RVA: 0x00025BDA File Offset: 0x00023DDA
			public HurtBoxInfo(HurtBox hurtBox)
			{
				this.transform = hurtBox.transform;
				this.estimatedRadius = Util.SphereVolumeToRadius(hurtBox.volume);
			}

			// Token: 0x04000901 RID: 2305
			public readonly Transform transform;

			// Token: 0x04000902 RID: 2306
			public readonly float estimatedRadius;
		}

		// Token: 0x02000196 RID: 406
		private struct ParentedPrefabDisplay
		{
			// Token: 0x1700011B RID: 283
			// (get) Token: 0x06000888 RID: 2184 RVA: 0x00025BF9 File Offset: 0x00023DF9
			// (set) Token: 0x06000889 RID: 2185 RVA: 0x00025C01 File Offset: 0x00023E01
			public GameObject instance { get; private set; }

			// Token: 0x1700011C RID: 284
			// (get) Token: 0x0600088A RID: 2186 RVA: 0x00025C0A File Offset: 0x00023E0A
			// (set) Token: 0x0600088B RID: 2187 RVA: 0x00025C12 File Offset: 0x00023E12
			public ItemDisplay itemDisplay { get; private set; }

			// Token: 0x0600088C RID: 2188 RVA: 0x00025C1C File Offset: 0x00023E1C
			public void Apply(CharacterModel characterModel, GameObject prefab, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
			{
				this.instance = UnityEngine.Object.Instantiate<GameObject>(prefab.gameObject, parent);
				this.instance.transform.localPosition = localPosition;
				this.instance.transform.localRotation = localRotation;
				this.instance.transform.localScale = localScale;
				LimbMatcher component = this.instance.GetComponent<LimbMatcher>();
				if (component && characterModel.childLocator)
				{
					component.SetChildLocator(characterModel.childLocator);
				}
				this.itemDisplay = this.instance.GetComponent<ItemDisplay>();
			}

			// Token: 0x0600088D RID: 2189 RVA: 0x00025CAF File Offset: 0x00023EAF
			public void Undo()
			{
				if (this.instance)
				{
					UnityEngine.Object.Destroy(this.instance);
					this.instance = null;
				}
			}

			// Token: 0x04000903 RID: 2307
			public ItemIndex itemIndex;

			// Token: 0x04000904 RID: 2308
			public EquipmentIndex equipmentIndex;
		}

		// Token: 0x02000197 RID: 407
		private struct LimbMaskDisplay
		{
			// Token: 0x0600088E RID: 2190 RVA: 0x00025CD0 File Offset: 0x00023ED0
			public void Apply(CharacterModel characterModel, LimbFlags mask)
			{
				this.maskValue = mask;
				characterModel.limbFlagSet.AddFlags(mask);
			}

			// Token: 0x0600088F RID: 2191 RVA: 0x00025CE5 File Offset: 0x00023EE5
			public void Undo(CharacterModel characterModel)
			{
				characterModel.limbFlagSet.RemoveFlags(this.maskValue);
			}

			// Token: 0x04000907 RID: 2311
			public ItemIndex itemIndex;

			// Token: 0x04000908 RID: 2312
			public EquipmentIndex equipmentIndex;

			// Token: 0x04000909 RID: 2313
			public LimbFlags maskValue;
		}

		// Token: 0x02000198 RID: 408
		[Serializable]
		private class LimbFlagSet
		{
			// Token: 0x1700011D RID: 285
			// (get) Token: 0x06000890 RID: 2192 RVA: 0x00025CF8 File Offset: 0x00023EF8
			// (set) Token: 0x06000891 RID: 2193 RVA: 0x00025D00 File Offset: 0x00023F00
			public float materialMaskValue { get; private set; }

			// Token: 0x06000892 RID: 2194 RVA: 0x00025D09 File Offset: 0x00023F09
			public LimbFlagSet()
			{
				this.materialMaskValue = 1f;
			}

			// Token: 0x06000893 RID: 2195 RVA: 0x00025D28 File Offset: 0x00023F28
			static LimbFlagSet()
			{
				int[] array = new int[]
				{
					2,
					3,
					5,
					11,
					17
				};
				CharacterModel.LimbFlagSet.primeConversionTable = new float[31];
				for (int i = 0; i < CharacterModel.LimbFlagSet.primeConversionTable.Length; i++)
				{
					int num = 1;
					for (int j = 0; j < 5; j++)
					{
						if ((i & 1 << j) != 0)
						{
							num *= array[j];
						}
					}
					CharacterModel.LimbFlagSet.primeConversionTable[i] = (float)num;
				}
			}

			// Token: 0x06000894 RID: 2196 RVA: 0x00025D8C File Offset: 0x00023F8C
			private static float ConvertLimbFlagsToMaterialMask(LimbFlags limbFlags)
			{
				return CharacterModel.LimbFlagSet.primeConversionTable[(int)limbFlags];
			}

			// Token: 0x06000895 RID: 2197 RVA: 0x00025D98 File Offset: 0x00023F98
			public void AddFlags(LimbFlags addedFlags)
			{
				LimbFlags limbFlags = this.flags;
				this.flags |= addedFlags;
				for (int i = 0; i < 5; i++)
				{
					if ((this.flags & (LimbFlags)(1 << i)) != LimbFlags.None)
					{
						byte[] array = this.flagCounts;
						int num = i;
						array[num] += 1;
					}
				}
				if (this.flags != limbFlags)
				{
					this.materialMaskValue = CharacterModel.LimbFlagSet.ConvertLimbFlagsToMaterialMask(this.flags);
				}
			}

			// Token: 0x06000896 RID: 2198 RVA: 0x00025E04 File Offset: 0x00024004
			public void RemoveFlags(LimbFlags removedFlags)
			{
				LimbFlags limbFlags = this.flags;
				for (int i = 0; i < 5; i++)
				{
					if ((removedFlags & (LimbFlags)(1 << i)) != LimbFlags.None)
					{
						byte[] array = this.flagCounts;
						int num = i;
						byte b = array[num] - 1;
						array[num] = b;
						if (b == 0)
						{
							this.flags &= (LimbFlags)(~(LimbFlags)(1 << i));
						}
					}
				}
				if (this.flags != limbFlags)
				{
					this.materialMaskValue = CharacterModel.LimbFlagSet.ConvertLimbFlagsToMaterialMask(this.flags);
				}
			}

			// Token: 0x0400090A RID: 2314
			private readonly byte[] flagCounts = new byte[5];

			// Token: 0x0400090B RID: 2315
			private LimbFlags flags;

			// Token: 0x0400090D RID: 2317
			private static readonly float[] primeConversionTable;
		}
	}
}
