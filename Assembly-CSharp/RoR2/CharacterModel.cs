using System;
using System.Collections;
using System.Collections.Generic;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace RoR2
{
	// Token: 0x0200028E RID: 654
	public class CharacterModel : MonoBehaviour
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000D15 RID: 3349 RVA: 0x00040AF0 File Offset: 0x0003ECF0
		// (set) Token: 0x06000D16 RID: 3350 RVA: 0x00040AF8 File Offset: 0x0003ECF8
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

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000D17 RID: 3351 RVA: 0x00040B11 File Offset: 0x0003ED11
		// (set) Token: 0x06000D18 RID: 3352 RVA: 0x00040B19 File Offset: 0x0003ED19
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

		// Token: 0x06000D19 RID: 3353 RVA: 0x00040B34 File Offset: 0x0003ED34
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CharacterModel.revealedMaterial = Resources.Load<Material>("Materials/matRevealedEffect");
			CharacterModel.cloakedMaterial = Resources.Load<Material>("Materials/matCloakedEffect");
			CharacterModel.ghostMaterial = Resources.Load<Material>("Materials/matGhostEffect");
			CharacterModel.repairModeMaterial = Resources.Load<Material>("Materials/matRepairModeOverlay");
			CharacterModel.wolfhatMaterial = Resources.Load<Material>("Materials/matWolfhatOverlay");
			CharacterModel.energyShieldMaterial = Resources.Load<Material>("Materials/matEnergyShield");
			CharacterModel.beetleJuiceMaterial = Resources.Load<Material>("Materials/matBeetleJuice");
			CharacterModel.brittleMaterial = Resources.Load<Material>("Materials/matBrittle");
			CharacterModel.fullCritMaterial = Resources.Load<Material>("Materials/matFullCrit");
			CharacterModel.clayGooMaterial = Resources.Load<Material>("Materials/matClayGooDebuff");
			CharacterModel.slow80Material = Resources.Load<Material>("Materials/matSlow80Debuff");
			CharacterModel.immuneMaterial = Resources.Load<Material>("Materials/matImmune");
			CharacterModel.bellBuffMaterial = Resources.Load<Material>("Materials/matBellBuff");
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x00040C04 File Offset: 0x0003EE04
		private void Awake()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			HurtBoxGroup component = base.GetComponent<HurtBoxGroup>();
			this.coreTransform = base.transform;
			if (component)
			{
				Transform transform;
				if (component == null)
				{
					transform = null;
				}
				else
				{
					HurtBox mainHurtBox = component.mainHurtBox;
					transform = ((mainHurtBox != null) ? mainHurtBox.transform : null);
				}
				this.coreTransform = (transform ?? this.coreTransform);
				HurtBox[] hurtBoxes = component.hurtBoxes;
				if (hurtBoxes.Length != 0)
				{
					this.hurtBoxInfos = new CharacterModel.HurtBoxInfo[hurtBoxes.Length];
					for (int i = 0; i < hurtBoxes.Length; i++)
					{
						this.hurtBoxInfos[i] = new CharacterModel.HurtBoxInfo
						{
							transform = hurtBoxes[i].transform,
							estimatedRadius = Util.SphereVolumeToRadius(hurtBoxes[i].volume)
						};
					}
				}
			}
			this.propertyStorage = new MaterialPropertyBlock();
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x00040CCD File Offset: 0x0003EECD
		private void Start()
		{
			this.visibility = VisibilityLevel.Invisible;
			this.UpdateMaterials();
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x00040CDC File Offset: 0x0003EEDC
		private static void RefreshObstructorsForCamera(CameraRigController cameraRigController)
		{
			Vector3 position = cameraRigController.transform.position;
			for (int i = 0; i < CharacterModel.instancesList.Count; i++)
			{
				CharacterModel characterModel = CharacterModel.instancesList[i];
				characterModel.fade = Mathf.Clamp01(Util.Remap(characterModel.GetNearestHurtBoxDistance(position), cameraRigController.fadeStartDistance, cameraRigController.fadeEndDistance, 0f, 1f));
			}
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x00040D44 File Offset: 0x0003EF44
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

		// Token: 0x06000D1E RID: 3358 RVA: 0x00040DA8 File Offset: 0x0003EFA8
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
				if (this.body.HasBuff(BuffIndex.Cloak))
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

		// Token: 0x06000D1F RID: 3359 RVA: 0x00040E80 File Offset: 0x0003F080
		static CharacterModel()
		{
			SceneCamera.onSceneCameraPreRender += CharacterModel.OnSceneCameraPreRender;
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x00040EF4 File Offset: 0x0003F0F4
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				CharacterModel.RefreshObstructorsForCamera(sceneCamera.cameraRigController);
			}
			if (sceneCamera.cameraRigController)
			{
				for (int i = 0; i < CharacterModel.instancesList.Count; i++)
				{
					CharacterModel.instancesList[i].UpdateForCamera(sceneCamera.cameraRigController);
				}
			}
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x00040F51 File Offset: 0x0003F151
		private void OnEnable()
		{
			CharacterModel.instancesList.Add(this);
			if (this.body != null)
			{
				this.body.onInventoryChanged += this.OnInventoryChanged;
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x00040F7D File Offset: 0x0003F17D
		private void OnDisable()
		{
			CharacterModel.instancesList.Remove(this);
			if (this.body != null)
			{
				this.body.onInventoryChanged -= this.OnInventoryChanged;
			}
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x00040FAC File Offset: 0x0003F1AC
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

		// Token: 0x06000D24 RID: 3364 RVA: 0x00040FFC File Offset: 0x0003F1FC
		private void UpdateMaterials()
		{
			if (this.visibility == VisibilityLevel.Invisible)
			{
				for (int i = this.rendererInfos.Length - 1; i >= 0; i--)
				{
					CharacterModel.RendererInfo rendererInfo = this.rendererInfos[i];
					rendererInfo.renderer.shadowCastingMode = ShadowCastingMode.Off;
					rendererInfo.renderer.enabled = false;
				}
			}
			else
			{
				for (int j = this.rendererInfos.Length - 1; j >= 0; j--)
				{
					CharacterModel.RendererInfo rendererInfo2 = this.rendererInfos[j];
					rendererInfo2.renderer.shadowCastingMode = rendererInfo2.defaultShadowCastingMode;
					rendererInfo2.renderer.enabled = true;
				}
			}
			Color value = Color.black;
			if (this.body && this.body.healthComponent)
			{
				if (this.body.healthComponent.shield > 0f)
				{
					value = CharacterModel.hitFlashShieldColor * Mathf.Clamp01(1f - this.body.healthComponent.timeSinceLastHit / 0.15f);
				}
				else
				{
					value = CharacterModel.hitFlashBaseColor * Mathf.Clamp01(1f - this.body.healthComponent.timeSinceLastHit / 0.15f);
				}
			}
			for (int k = this.rendererInfos.Length - 1; k >= 0; k--)
			{
				Renderer renderer = this.rendererInfos[k].renderer;
				this.UpdateRendererMaterials(renderer, this.rendererInfos[k].defaultMaterial, this.rendererInfos[k].ignoreOverlays);
				renderer.GetPropertyBlock(this.propertyStorage);
				this.propertyStorage.SetColor("_FlashColor", value);
				this.propertyStorage.SetFloat("_Fade", this.fade);
				this.propertyStorage.SetFloat("_EliteIndex", (float)(this.myEliteIndex + 1));
				this.propertyStorage.SetFloat("_LimbPrimeMask", this.limbFlagSet.materialMaskValue);
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
					this.propertyStorage.SetColor("_FlashColor", value);
					this.propertyStorage.SetFloat("_Fade", this.fade);
					renderer2.SetPropertyBlock(this.propertyStorage);
				}
			}
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x000412A8 File Offset: 0x0003F4A8
		private void UpdateOverlays()
		{
			for (int i = 0; i < this.overlaysCount; i++)
			{
				this.currentOverlays[i] = null;
			}
			this.overlaysCount = 0;
			if (this.visibility == VisibilityLevel.Invisible)
			{
				return;
			}
			EliteIndex eliteIndex = EliteCatalog.IsEquipmentElite(this.inventoryEquipmentIndex);
			this.myEliteIndex = (int)eliteIndex;
			if (this.myEliteIndex >= 0)
			{
				if (this.body)
				{
					AkSoundEngine.SetRTPCValue("eliteEnemy", 1f, this.body.gameObject);
				}
			}
			else if (this.body)
			{
				AkSoundEngine.SetRTPCValue("eliteEnemy", 0f, this.body.gameObject);
			}
			if (this.body)
			{
				bool flag = this.body.HasBuff(BuffIndex.ClayGoo);
				Inventory inventory = this.body.inventory;
				this.isGhost = (inventory != null && inventory.GetItemCount(ItemIndex.Ghost) > 0);
				if (this.body.HasBuff(BuffIndex.RepairMode))
				{
					Material[] array = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array[j] = CharacterModel.repairModeMaterial;
				}
				if (this.body.HasBuff(BuffIndex.AttackSpeedOnCrit))
				{
					Material[] array2 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array2[j] = CharacterModel.wolfhatMaterial;
				}
				if (this.body.healthComponent && this.body.healthComponent.shield > 0f)
				{
					Material[] array3 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array3[j] = CharacterModel.energyShieldMaterial;
				}
				if (this.body.HasBuff(BuffIndex.FullCrit))
				{
					Material[] array4 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array4[j] = CharacterModel.fullCritMaterial;
				}
				if (this.body.HasBuff(BuffIndex.BeetleJuice))
				{
					Material[] array5 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array5[j] = CharacterModel.beetleJuiceMaterial;
				}
				if (this.body.HasBuff(BuffIndex.Immune))
				{
					Material[] array6 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array6[j] = CharacterModel.immuneMaterial;
				}
				if (this.body.HasBuff(BuffIndex.Slow80))
				{
					Material[] array7 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array7[j] = CharacterModel.slow80Material;
				}
				if (this.body.HasBuff(BuffIndex.Invincibility))
				{
					Material[] array8 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array8[j] = CharacterModel.bellBuffMaterial;
				}
				if (flag)
				{
					Material[] array9 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array9[j] = CharacterModel.clayGooMaterial;
				}
				if (this.body.inventory && this.body.inventory.GetItemCount(ItemIndex.LunarDagger) > 0)
				{
					Material[] array10 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array10[j] = CharacterModel.brittleMaterial;
				}
				if (this.isGhost)
				{
					Material[] array11 = this.currentOverlays;
					int j = this.overlaysCount;
					this.overlaysCount = j + 1;
					array11[j] = CharacterModel.ghostMaterial;
				}
				if (this.body.equipmentSlot)
				{
					if (this.body.equipmentSlot.equipmentIndex == EquipmentIndex.AffixGold)
					{
						if (!this.goldAffixEffect)
						{
							this.goldAffixEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GoldAffixEffect"), base.transform);
							ParticleSystem component = this.goldAffixEffect.GetComponent<ParticleSystem>();
							SkinnedMeshRenderer skinnedMeshRenderer = null;
							foreach (CharacterModel.RendererInfo rendererInfo in this.rendererInfos)
							{
								if (rendererInfo.renderer is SkinnedMeshRenderer)
								{
									skinnedMeshRenderer = (SkinnedMeshRenderer)rendererInfo.renderer;
									break;
								}
							}
							ParticleSystem.ShapeModule shape = component.shape;
							if (skinnedMeshRenderer)
							{
								shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
								shape.skinnedMeshRenderer = skinnedMeshRenderer;
							}
						}
					}
					else if (this.goldAffixEffect)
					{
						UnityEngine.Object.Destroy(this.goldAffixEffect);
					}
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
				this.wasPreviouslyClayGooed = this.body.HasBuff(BuffIndex.ClayGoo);
			}
			for (int k = 0; k < this.temporaryOverlays.Count; k++)
			{
				Material[] array13 = this.currentOverlays;
				int j = this.overlaysCount;
				this.overlaysCount = j + 1;
				array13[j] = this.temporaryOverlays[k].materialInstance;
			}
			this.materialsDirty = true;
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x00041730 File Offset: 0x0003F930
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
				material = ((this.isGhost && !ignoreOverlays) ? CharacterModel.ghostMaterial : defaultMaterial);
				break;
			}
			int num = ignoreOverlays ? 0 : this.overlaysCount;
			if (material)
			{
				num++;
			}
			Material[] array = new Material[num];
			int num2 = 0;
			if (material)
			{
				array[num2++] = material;
			}
			if (!ignoreOverlays)
			{
				for (int i = 0; i < this.overlaysCount; i++)
				{
					array[num2++] = this.currentOverlays[i];
				}
			}
			renderer.sharedMaterials = array;
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x000417F4 File Offset: 0x0003F9F4
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

		// Token: 0x06000D28 RID: 3368 RVA: 0x00041900 File Offset: 0x0003FB00
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
			DisplayRuleGroup equipmentDisplayRuleGroup = this.itemDisplayRuleSet.GetEquipmentDisplayRuleGroup(newEquipmentIndex);
			this.InstantiateDisplayRuleGroup(equipmentDisplayRuleGroup, ItemIndex.None, newEquipmentIndex);
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x000419D0 File Offset: 0x0003FBD0
		private void EnableItemDisplay(ItemIndex itemIndex)
		{
			if (this.enabledItemDisplays[(int)itemIndex])
			{
				return;
			}
			this.enabledItemDisplays[(int)itemIndex] = true;
			DisplayRuleGroup itemDisplayRuleGroup = this.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex);
			this.InstantiateDisplayRuleGroup(itemDisplayRuleGroup, itemIndex, EquipmentIndex.None);
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00041A10 File Offset: 0x0003FC10
		private void DisableItemDisplay(ItemIndex itemIndex)
		{
			if (!this.enabledItemDisplays[(int)itemIndex])
			{
				return;
			}
			this.enabledItemDisplays[(int)itemIndex] = false;
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

		// Token: 0x06000D2B RID: 3371 RVA: 0x00041AD4 File Offset: 0x0003FCD4
		public void UpdateItemDisplay(Inventory inventory)
		{
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (inventory.GetItemCount(itemIndex) > 0)
				{
					this.EnableItemDisplay(itemIndex);
				}
				else
				{
					this.DisableItemDisplay(itemIndex);
				}
			}
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x00041B08 File Offset: 0x0003FD08
		public void HighlightItemDisplay(ItemIndex itemIndex)
		{
			if (!this.enabledItemDisplays[(int)itemIndex])
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

		// Token: 0x06000D2D RID: 3373 RVA: 0x00041BFC File Offset: 0x0003FDFC
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

		// Token: 0x0400111D RID: 4381
		public CharacterBody body;

		// Token: 0x0400111E RID: 4382
		public ItemDisplayRuleSet itemDisplayRuleSet;

		// Token: 0x0400111F RID: 4383
		public CharacterModel.RendererInfo[] rendererInfos;

		// Token: 0x04001120 RID: 4384
		private ChildLocator childLocator;

		// Token: 0x04001121 RID: 4385
		private GameObject goldAffixEffect;

		// Token: 0x04001122 RID: 4386
		private static readonly CharacterModel.HurtBoxInfo[] emptyHurtBoxInfos = new CharacterModel.HurtBoxInfo[0];

		// Token: 0x04001123 RID: 4387
		private CharacterModel.HurtBoxInfo[] hurtBoxInfos = CharacterModel.emptyHurtBoxInfos;

		// Token: 0x04001124 RID: 4388
		private Transform coreTransform;

		// Token: 0x04001125 RID: 4389
		public static Material revealedMaterial;

		// Token: 0x04001126 RID: 4390
		public static Material cloakedMaterial;

		// Token: 0x04001127 RID: 4391
		public static Material ghostMaterial;

		// Token: 0x04001128 RID: 4392
		public static Material bellBuffMaterial;

		// Token: 0x04001129 RID: 4393
		public static Material repairModeMaterial;

		// Token: 0x0400112A RID: 4394
		public static Material wolfhatMaterial;

		// Token: 0x0400112B RID: 4395
		public static Material energyShieldMaterial;

		// Token: 0x0400112C RID: 4396
		public static Material fullCritMaterial;

		// Token: 0x0400112D RID: 4397
		public static Material beetleJuiceMaterial;

		// Token: 0x0400112E RID: 4398
		public static Material brittleMaterial;

		// Token: 0x0400112F RID: 4399
		public static Material clayGooMaterial;

		// Token: 0x04001130 RID: 4400
		public static Material slow80Material;

		// Token: 0x04001131 RID: 4401
		public static Material immuneMaterial;

		// Token: 0x04001132 RID: 4402
		private static Color hitFlashBaseColor = new Color32(193, 108, 51, byte.MaxValue);

		// Token: 0x04001133 RID: 4403
		private static Color hitFlashShieldColor = new Color32(132, 159, byte.MaxValue, byte.MaxValue);

		// Token: 0x04001134 RID: 4404
		private const float hitFlashDuration = 0.15f;

		// Token: 0x04001135 RID: 4405
		private VisibilityLevel _visibility = VisibilityLevel.Visible;

		// Token: 0x04001136 RID: 4406
		private bool _isGhost;

		// Token: 0x04001137 RID: 4407
		[HideInInspector]
		public int invisibilityCount;

		// Token: 0x04001138 RID: 4408
		[NonSerialized]
		public List<TemporaryOverlay> temporaryOverlays = new List<TemporaryOverlay>();

		// Token: 0x04001139 RID: 4409
		private bool materialsDirty = true;

		// Token: 0x0400113A RID: 4410
		private EquipmentIndex inventoryEquipmentIndex = EquipmentIndex.None;

		// Token: 0x0400113B RID: 4411
		private MaterialPropertyBlock propertyStorage;

		// Token: 0x0400113C RID: 4412
		private int myEliteIndex = -1;

		// Token: 0x0400113D RID: 4413
		private float fade = 1f;

		// Token: 0x0400113E RID: 4414
		private float firstPersonFade = 1f;

		// Token: 0x0400113F RID: 4415
		private CharacterModel.LimbFlagSet limbFlagSet = new CharacterModel.LimbFlagSet();

		// Token: 0x04001140 RID: 4416
		private static List<CharacterModel> instancesList = new List<CharacterModel>();

		// Token: 0x04001141 RID: 4417
		private BitArray enabledItemDisplays = new BitArray(78);

		// Token: 0x04001142 RID: 4418
		private List<CharacterModel.ParentedPrefabDisplay> parentedPrefabDisplays = new List<CharacterModel.ParentedPrefabDisplay>();

		// Token: 0x04001143 RID: 4419
		private List<CharacterModel.LimbMaskDisplay> limbMaskDisplays = new List<CharacterModel.LimbMaskDisplay>();

		// Token: 0x04001144 RID: 4420
		private const int maxOverlays = 8;

		// Token: 0x04001145 RID: 4421
		private Material[] currentOverlays = new Material[8];

		// Token: 0x04001146 RID: 4422
		private int overlaysCount;

		// Token: 0x04001147 RID: 4423
		private bool wasPreviouslyClayGooed;

		// Token: 0x04001148 RID: 4424
		private EquipmentIndex currentEquipmentDisplayIndex = EquipmentIndex.None;

		// Token: 0x0200028F RID: 655
		[Serializable]
		public struct RendererInfo
		{
			// Token: 0x04001149 RID: 4425
			public Renderer renderer;

			// Token: 0x0400114A RID: 4426
			public Material defaultMaterial;

			// Token: 0x0400114B RID: 4427
			public ShadowCastingMode defaultShadowCastingMode;

			// Token: 0x0400114C RID: 4428
			public bool ignoreOverlays;
		}

		// Token: 0x02000290 RID: 656
		private struct HurtBoxInfo
		{
			// Token: 0x0400114D RID: 4429
			public Transform transform;

			// Token: 0x0400114E RID: 4430
			public float estimatedRadius;
		}

		// Token: 0x02000291 RID: 657
		[Serializable]
		private class LimbFlagSet
		{
			// Token: 0x17000114 RID: 276
			// (get) Token: 0x06000D2F RID: 3375 RVA: 0x00041CF4 File Offset: 0x0003FEF4
			// (set) Token: 0x06000D30 RID: 3376 RVA: 0x00041CFC File Offset: 0x0003FEFC
			public float materialMaskValue { get; private set; }

			// Token: 0x06000D31 RID: 3377 RVA: 0x00041D05 File Offset: 0x0003FF05
			public LimbFlagSet()
			{
				this.materialMaskValue = 1f;
			}

			// Token: 0x06000D32 RID: 3378 RVA: 0x00041D24 File Offset: 0x0003FF24
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

			// Token: 0x06000D33 RID: 3379 RVA: 0x00041D88 File Offset: 0x0003FF88
			private static float ConvertLimbFlagsToMaterialMask(LimbFlags limbFlags)
			{
				return CharacterModel.LimbFlagSet.primeConversionTable[(int)limbFlags];
			}

			// Token: 0x06000D34 RID: 3380 RVA: 0x00041D94 File Offset: 0x0003FF94
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

			// Token: 0x06000D35 RID: 3381 RVA: 0x00041E00 File Offset: 0x00040000
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

			// Token: 0x0400114F RID: 4431
			private readonly byte[] flagCounts = new byte[5];

			// Token: 0x04001150 RID: 4432
			private LimbFlags flags;

			// Token: 0x04001152 RID: 4434
			private static readonly float[] primeConversionTable;
		}

		// Token: 0x02000292 RID: 658
		private struct ParentedPrefabDisplay
		{
			// Token: 0x17000115 RID: 277
			// (get) Token: 0x06000D36 RID: 3382 RVA: 0x00041E6F File Offset: 0x0004006F
			// (set) Token: 0x06000D37 RID: 3383 RVA: 0x00041E77 File Offset: 0x00040077
			public GameObject instance { get; private set; }

			// Token: 0x17000116 RID: 278
			// (get) Token: 0x06000D38 RID: 3384 RVA: 0x00041E80 File Offset: 0x00040080
			// (set) Token: 0x06000D39 RID: 3385 RVA: 0x00041E88 File Offset: 0x00040088
			public ItemDisplay itemDisplay { get; private set; }

			// Token: 0x06000D3A RID: 3386 RVA: 0x00041E94 File Offset: 0x00040094
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

			// Token: 0x06000D3B RID: 3387 RVA: 0x00041F27 File Offset: 0x00040127
			public void Undo()
			{
				if (this.instance)
				{
					UnityEngine.Object.Destroy(this.instance);
					this.instance = null;
				}
			}

			// Token: 0x04001153 RID: 4435
			public ItemIndex itemIndex;

			// Token: 0x04001154 RID: 4436
			public EquipmentIndex equipmentIndex;
		}

		// Token: 0x02000293 RID: 659
		private struct LimbMaskDisplay
		{
			// Token: 0x06000D3C RID: 3388 RVA: 0x00041F48 File Offset: 0x00040148
			public void Apply(CharacterModel characterModel, LimbFlags mask)
			{
				this.maskValue = mask;
				characterModel.limbFlagSet.AddFlags(mask);
			}

			// Token: 0x06000D3D RID: 3389 RVA: 0x00041F5D File Offset: 0x0004015D
			public void Undo(CharacterModel characterModel)
			{
				characterModel.limbFlagSet.RemoveFlags(this.maskValue);
			}

			// Token: 0x04001157 RID: 4439
			public ItemIndex itemIndex;

			// Token: 0x04001158 RID: 4440
			public EquipmentIndex equipmentIndex;

			// Token: 0x04001159 RID: 4441
			public LimbFlags maskValue;
		}
	}
}
