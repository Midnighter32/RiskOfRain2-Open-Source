using System;
using System.Collections.Generic;
using EntityStates.Missions.Goldshores;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000603 RID: 1539
	public class ObjectivePanelController : MonoBehaviour
	{
		// Token: 0x06002481 RID: 9345 RVA: 0x0009F3D4 File Offset: 0x0009D5D4
		public void SetCurrentMaster(CharacterMaster newMaster)
		{
			if (newMaster == this.currentMaster)
			{
				return;
			}
			for (int i = this.objectiveTrackers.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this.objectiveTrackers[i].stripObject);
			}
			this.objectiveTrackers.Clear();
			this.currentMaster = newMaster;
			this.RefreshObjectiveTrackers();
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x0009F438 File Offset: 0x0009D638
		private void AddObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.objectiveTrackerPrefab, this.objectiveTrackerContainer);
			gameObject.SetActive(true);
			objectiveTracker.owner = this;
			objectiveTracker.SetStrip(gameObject);
			this.objectiveTrackers.Add(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Add(objectiveTracker.sourceDescriptor, objectiveTracker);
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x0009F48A File Offset: 0x0009D68A
		private void RemoveObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.objectiveTrackers.Remove(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Remove(objectiveTracker.sourceDescriptor);
			objectiveTracker.Retire();
			this.AddExitAnimation(objectiveTracker);
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x0009F4B8 File Offset: 0x0009D6B8
		private void RefreshObjectiveTrackers()
		{
			foreach (ObjectivePanelController.ObjectiveTracker objectiveTracker in this.objectiveTrackers)
			{
				objectiveTracker.isRelevant = false;
			}
			if (this.currentMaster)
			{
				this.GetObjectiveSources(this.currentMaster, this.objectiveSourceDescriptors);
				foreach (ObjectivePanelController.ObjectiveSourceDescriptor objectiveSourceDescriptor in this.objectiveSourceDescriptors)
				{
					ObjectivePanelController.ObjectiveTracker objectiveTracker2;
					if (this.objectiveSourceToTrackerDictionary.TryGetValue(objectiveSourceDescriptor, out objectiveTracker2))
					{
						objectiveTracker2.isRelevant = true;
					}
					else
					{
						ObjectivePanelController.ObjectiveTracker objectiveTracker3 = ObjectivePanelController.ObjectiveTracker.Instantiate(objectiveSourceDescriptor);
						objectiveTracker3.isRelevant = true;
						this.AddObjectiveTracker(objectiveTracker3);
					}
				}
			}
			for (int i = this.objectiveTrackers.Count - 1; i >= 0; i--)
			{
				if (!this.objectiveTrackers[i].isRelevant)
				{
					this.RemoveObjectiveTracker(this.objectiveTrackers[i]);
				}
			}
			foreach (ObjectivePanelController.ObjectiveTracker objectiveTracker4 in this.objectiveTrackers)
			{
				objectiveTracker4.UpdateStrip();
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x0009F618 File Offset: 0x0009D818
		private void GetObjectiveSources(CharacterMaster master, [NotNull] List<ObjectivePanelController.ObjectiveSourceDescriptor> output)
		{
			output.Clear();
			WeeklyRun weeklyRun = Run.instance as WeeklyRun;
			if (weeklyRun && weeklyRun.crystalsRequiredToKill > weeklyRun.crystalsKilled)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = Run.instance,
					master = master,
					objectiveType = typeof(ObjectivePanelController.DestroyTimeCrystals)
				});
			}
			TeleporterInteraction instance = TeleporterInteraction.instance;
			if (instance)
			{
				Type type = null;
				if (instance.isCharging)
				{
					type = typeof(ObjectivePanelController.ChargeTeleporterObjectiveTracker);
				}
				else if (instance.isCharged && !instance.isInFinalSequence)
				{
					type = typeof(ObjectivePanelController.FinishTeleporterObjectiveTracker);
				}
				else if (instance.isIdle)
				{
					type = typeof(ObjectivePanelController.FindTeleporterObjectiveTracker);
				}
				if (type != null)
				{
					output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
					{
						source = instance,
						master = master,
						objectiveType = type
					});
				}
			}
			if (GoldshoresMissionController.instance)
			{
				Type type2 = GoldshoresMissionController.instance.entityStateMachine.state.GetType();
				if ((type2 == typeof(ActivateBeacons) || type2 == typeof(GoldshoresBossfight)) && GoldshoresMissionController.instance.beaconsActive < GoldshoresMissionController.instance.beaconCount)
				{
					output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
					{
						source = GoldshoresMissionController.instance,
						master = master,
						objectiveType = typeof(ObjectivePanelController.ActivateGoldshoreBeaconTracker)
					});
				}
			}
			if (ArenaMissionController.instance && ArenaMissionController.instance.clearedRounds < ArenaMissionController.instance.totalRoundsMax)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = ArenaMissionController.instance,
					master = master,
					objectiveType = typeof(ObjectivePanelController.ClearArena)
				});
			}
			Action<CharacterMaster, List<ObjectivePanelController.ObjectiveSourceDescriptor>> action = ObjectivePanelController.collectObjectiveSources;
			if (action == null)
			{
				return;
			}
			action(master, output);
		}

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x06002486 RID: 9350 RVA: 0x0009F804 File Offset: 0x0009DA04
		// (remove) Token: 0x06002487 RID: 9351 RVA: 0x0009F838 File Offset: 0x0009DA38
		public static event Action<CharacterMaster, List<ObjectivePanelController.ObjectiveSourceDescriptor>> collectObjectiveSources;

		// Token: 0x06002488 RID: 9352 RVA: 0x0009F86B File Offset: 0x0009DA6B
		private void Update()
		{
			this.RefreshObjectiveTrackers();
			this.RunExitAnimations();
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x0009F879 File Offset: 0x0009DA79
		private void AddExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.exitAnimations.Add(new ObjectivePanelController.StripExitAnimation(objectiveTracker));
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x0009F88C File Offset: 0x0009DA8C
		private void RunExitAnimations()
		{
			float deltaTime = Time.deltaTime;
			float num = 7f;
			float num2 = deltaTime / num;
			for (int i = this.exitAnimations.Count - 1; i >= 0; i--)
			{
				float num3 = Mathf.Min(this.exitAnimations[i].t + num2, 1f);
				this.exitAnimations[i].SetT(num3);
				if (num3 >= 1f)
				{
					UnityEngine.Object.Destroy(this.exitAnimations[i].objectiveTracker.stripObject);
					this.exitAnimations.RemoveAt(i);
				}
			}
		}

		// Token: 0x04002246 RID: 8774
		public RectTransform objectiveTrackerContainer;

		// Token: 0x04002247 RID: 8775
		public GameObject objectiveTrackerPrefab;

		// Token: 0x04002248 RID: 8776
		public Sprite checkboxActiveSprite;

		// Token: 0x04002249 RID: 8777
		public Sprite checkboxSuccessSprite;

		// Token: 0x0400224A RID: 8778
		public Sprite checkboxFailSprite;

		// Token: 0x0400224B RID: 8779
		private CharacterMaster currentMaster;

		// Token: 0x0400224C RID: 8780
		private readonly List<ObjectivePanelController.ObjectiveTracker> objectiveTrackers = new List<ObjectivePanelController.ObjectiveTracker>();

		// Token: 0x0400224D RID: 8781
		private Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker> objectiveSourceToTrackerDictionary = new Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker>(EqualityComparer<ObjectivePanelController.ObjectiveSourceDescriptor>.Default);

		// Token: 0x0400224E RID: 8782
		private readonly List<ObjectivePanelController.ObjectiveSourceDescriptor> objectiveSourceDescriptors = new List<ObjectivePanelController.ObjectiveSourceDescriptor>();

		// Token: 0x04002250 RID: 8784
		private readonly List<ObjectivePanelController.StripExitAnimation> exitAnimations = new List<ObjectivePanelController.StripExitAnimation>();

		// Token: 0x02000604 RID: 1540
		public struct ObjectiveSourceDescriptor : IEquatable<ObjectivePanelController.ObjectiveSourceDescriptor>
		{
			// Token: 0x0600248C RID: 9356 RVA: 0x0009F958 File Offset: 0x0009DB58
			public override int GetHashCode()
			{
				return (((this.source != null) ? this.source.GetHashCode() : 0) * 397 ^ ((this.master != null) ? this.master.GetHashCode() : 0)) * 397 ^ ((this.objectiveType != null) ? this.objectiveType.GetHashCode() : 0);
			}

			// Token: 0x0600248D RID: 9357 RVA: 0x0009F9C7 File Offset: 0x0009DBC7
			public static bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor a, ObjectivePanelController.ObjectiveSourceDescriptor b)
			{
				return a.source == b.source && a.master == b.master && a.objectiveType == b.objectiveType;
			}

			// Token: 0x0600248E RID: 9358 RVA: 0x0009F9C7 File Offset: 0x0009DBC7
			public bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor other)
			{
				return this.source == other.source && this.master == other.master && this.objectiveType == other.objectiveType;
			}

			// Token: 0x0600248F RID: 9359 RVA: 0x0009FA02 File Offset: 0x0009DC02
			public override bool Equals(object obj)
			{
				return obj != null && obj is ObjectivePanelController.ObjectiveSourceDescriptor && this.Equals((ObjectivePanelController.ObjectiveSourceDescriptor)obj);
			}

			// Token: 0x04002251 RID: 8785
			public UnityEngine.Object source;

			// Token: 0x04002252 RID: 8786
			public CharacterMaster master;

			// Token: 0x04002253 RID: 8787
			public Type objectiveType;
		}

		// Token: 0x02000605 RID: 1541
		public class ObjectiveTracker
		{
			// Token: 0x170003CD RID: 973
			// (get) Token: 0x06002490 RID: 9360 RVA: 0x0009FA1F File Offset: 0x0009DC1F
			// (set) Token: 0x06002491 RID: 9361 RVA: 0x0009FA27 File Offset: 0x0009DC27
			public GameObject stripObject { get; private set; }

			// Token: 0x06002492 RID: 9362 RVA: 0x0009FA30 File Offset: 0x0009DC30
			public void SetStrip(GameObject stripObject)
			{
				this.stripObject = stripObject;
				this.label = stripObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				this.checkbox = stripObject.transform.Find("Checkbox").GetComponent<Image>();
				this.UpdateStrip();
			}

			// Token: 0x06002493 RID: 9363 RVA: 0x0009FA80 File Offset: 0x0009DC80
			public string GetString()
			{
				if (this.IsDirty())
				{
					this.cachedString = this.GenerateString();
				}
				return this.cachedString;
			}

			// Token: 0x06002494 RID: 9364 RVA: 0x0009FA9C File Offset: 0x0009DC9C
			protected virtual string GenerateString()
			{
				return Language.GetString(this.baseToken);
			}

			// Token: 0x06002495 RID: 9365 RVA: 0x0009FAA9 File Offset: 0x0009DCA9
			protected virtual bool IsDirty()
			{
				return this.cachedString == null;
			}

			// Token: 0x06002496 RID: 9366 RVA: 0x0009FAB4 File Offset: 0x0009DCB4
			public void Retire()
			{
				this.retired = true;
				this.OnRetired();
				this.UpdateStrip();
			}

			// Token: 0x06002497 RID: 9367 RVA: 0x0000409B File Offset: 0x0000229B
			protected virtual void OnRetired()
			{
			}

			// Token: 0x06002498 RID: 9368 RVA: 0x0009FACC File Offset: 0x0009DCCC
			public virtual void UpdateStrip()
			{
				if (this.label)
				{
					this.label.text = this.GetString();
					this.label.color = (this.retired ? Color.gray : Color.white);
					if (this.retired)
					{
						this.label.fontStyle |= FontStyles.Strikethrough;
					}
				}
				if (this.checkbox)
				{
					this.checkbox.sprite = (this.retired ? this.owner.checkboxSuccessSprite : this.owner.checkboxActiveSprite);
					this.checkbox.color = (this.retired ? Color.yellow : Color.white);
				}
			}

			// Token: 0x06002499 RID: 9369 RVA: 0x0009FB8C File Offset: 0x0009DD8C
			public static ObjectivePanelController.ObjectiveTracker Instantiate(ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor)
			{
				if (sourceDescriptor.objectiveType != null && sourceDescriptor.objectiveType.IsSubclassOf(typeof(ObjectivePanelController.ObjectiveTracker)))
				{
					ObjectivePanelController.ObjectiveTracker objectiveTracker = (ObjectivePanelController.ObjectiveTracker)Activator.CreateInstance(sourceDescriptor.objectiveType);
					objectiveTracker.sourceDescriptor = sourceDescriptor;
					return objectiveTracker;
				}
				string format = "Bad objectiveType {0}";
				object[] array = new object[1];
				int num = 0;
				Type objectiveType = sourceDescriptor.objectiveType;
				array[num] = ((objectiveType != null) ? objectiveType.FullName : null);
				Debug.LogFormat(format, array);
				return null;
			}

			// Token: 0x04002254 RID: 8788
			public ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor;

			// Token: 0x04002255 RID: 8789
			public ObjectivePanelController owner;

			// Token: 0x04002256 RID: 8790
			public bool isRelevant;

			// Token: 0x04002258 RID: 8792
			protected Image checkbox;

			// Token: 0x04002259 RID: 8793
			protected TextMeshProUGUI label;

			// Token: 0x0400225A RID: 8794
			protected string cachedString;

			// Token: 0x0400225B RID: 8795
			protected string baseToken = "";

			// Token: 0x0400225C RID: 8796
			protected bool retired;
		}

		// Token: 0x02000606 RID: 1542
		private class FindTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x0600249B RID: 9371 RVA: 0x0009FC0F File Offset: 0x0009DE0F
			public FindTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FIND_TELEPORTER";
			}
		}

		// Token: 0x02000607 RID: 1543
		private class ActivateGoldshoreBeaconTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x170003CE RID: 974
			// (get) Token: 0x0600249C RID: 9372 RVA: 0x0009FC22 File Offset: 0x0009DE22
			private GoldshoresMissionController missionController
			{
				get
				{
					return this.sourceDescriptor.source as GoldshoresMissionController;
				}
			}

			// Token: 0x0600249D RID: 9373 RVA: 0x0009FC34 File Offset: 0x0009DE34
			public ActivateGoldshoreBeaconTracker()
			{
				this.baseToken = "OBJECTIVE_GOLDSHORES_ACTIVATE_BEACONS";
			}

			// Token: 0x0600249E RID: 9374 RVA: 0x0009FC58 File Offset: 0x0009DE58
			private bool UpdateCachedValues()
			{
				int beaconsActive = this.missionController.beaconsActive;
				int beaconCount = this.missionController.beaconCount;
				if (beaconsActive != this.cachedActiveBeaconCount || beaconCount != this.cachedRequiredBeaconCount)
				{
					this.cachedActiveBeaconCount = beaconsActive;
					this.cachedRequiredBeaconCount = beaconCount;
					return true;
				}
				return false;
			}

			// Token: 0x0600249F RID: 9375 RVA: 0x0009FCA0 File Offset: 0x0009DEA0
			protected override string GenerateString()
			{
				this.UpdateCachedValues();
				return string.Format(Language.GetString(this.baseToken), this.cachedActiveBeaconCount, this.cachedRequiredBeaconCount);
			}

			// Token: 0x060024A0 RID: 9376 RVA: 0x0009FCCF File Offset: 0x0009DECF
			protected override bool IsDirty()
			{
				return !(this.sourceDescriptor.source as GoldshoresMissionController) || this.UpdateCachedValues();
			}

			// Token: 0x0400225D RID: 8797
			private int cachedActiveBeaconCount = -1;

			// Token: 0x0400225E RID: 8798
			private int cachedRequiredBeaconCount = -1;
		}

		// Token: 0x02000608 RID: 1544
		private class ClearArena : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060024A1 RID: 9377 RVA: 0x0009FCF0 File Offset: 0x0009DEF0
			public ClearArena()
			{
				this.baseToken = "OBJECTIVE_CLEAR_ARENA";
			}

			// Token: 0x060024A2 RID: 9378 RVA: 0x0009FD04 File Offset: 0x0009DF04
			protected override string GenerateString()
			{
				ArenaMissionController instance = ArenaMissionController.instance;
				return string.Format(Language.GetString(this.baseToken), instance.clearedRounds, instance.totalRoundsMax);
			}

			// Token: 0x060024A3 RID: 9379 RVA: 0x0000B933 File Offset: 0x00009B33
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000609 RID: 1545
		private class DestroyTimeCrystals : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060024A4 RID: 9380 RVA: 0x0009FD3D File Offset: 0x0009DF3D
			public DestroyTimeCrystals()
			{
				this.baseToken = "OBJECTIVE_WEEKLYRUN_DESTROY_CRYSTALS";
			}

			// Token: 0x060024A5 RID: 9381 RVA: 0x0009FD50 File Offset: 0x0009DF50
			protected override string GenerateString()
			{
				WeeklyRun weeklyRun = Run.instance as WeeklyRun;
				return string.Format(Language.GetString(this.baseToken), weeklyRun.crystalsKilled, weeklyRun.crystalsRequiredToKill);
			}

			// Token: 0x060024A6 RID: 9382 RVA: 0x0000B933 File Offset: 0x00009B33
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x0200060A RID: 1546
		private class ChargeTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060024A7 RID: 9383 RVA: 0x0009FD8E File Offset: 0x0009DF8E
			public ChargeTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_CHARGE_TELEPORTER";
			}

			// Token: 0x060024A8 RID: 9384 RVA: 0x0009FDA8 File Offset: 0x0009DFA8
			private bool ShouldBeFlashing()
			{
				bool flag = true;
				if (TeleporterInteraction.instance)
				{
					CharacterMaster master = this.sourceDescriptor.master;
					if (master)
					{
						CharacterBody body = master.GetBody();
						if (body)
						{
							flag = TeleporterInteraction.instance.IsInChargingRange(body);
						}
					}
				}
				return !flag;
			}

			// Token: 0x060024A9 RID: 9385 RVA: 0x0009FDF8 File Offset: 0x0009DFF8
			protected override string GenerateString()
			{
				this.lastPercent = ObjectivePanelController.ChargeTeleporterObjectiveTracker.GetTeleporterPercent();
				string text = string.Format(Language.GetString(this.baseToken), this.lastPercent);
				if (this.ShouldBeFlashing())
				{
					text = string.Format(Language.GetString("OBJECTIVE_CHARGE_TELEPORTER_OOB"), this.lastPercent);
					if ((int)(Time.time * 12f) % 2 == 0)
					{
						text = string.Format("<style=cDeath>{0}</style>", text);
					}
				}
				return text;
			}

			// Token: 0x060024AA RID: 9386 RVA: 0x0009FE6C File Offset: 0x0009E06C
			private static int GetTeleporterPercent()
			{
				if (!TeleporterInteraction.instance)
				{
					return 0;
				}
				return Mathf.CeilToInt(TeleporterInteraction.instance.chargeFraction * 100f);
			}

			// Token: 0x060024AB RID: 9387 RVA: 0x0000B933 File Offset: 0x00009B33
			protected override bool IsDirty()
			{
				return true;
			}

			// Token: 0x0400225F RID: 8799
			private int lastPercent = -1;
		}

		// Token: 0x0200060B RID: 1547
		private class FinishTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060024AC RID: 9388 RVA: 0x0009FE91 File Offset: 0x0009E091
			public FinishTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FINISH_TELEPORTER";
			}
		}

		// Token: 0x0200060C RID: 1548
		private class StripExitAnimation
		{
			// Token: 0x060024AD RID: 9389 RVA: 0x0009FEA4 File Offset: 0x0009E0A4
			public StripExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
			{
				this.objectiveTracker = objectiveTracker;
				this.layoutElement = objectiveTracker.stripObject.GetComponent<LayoutElement>();
				this.canvasGroup = objectiveTracker.stripObject.GetComponent<CanvasGroup>();
				this.originalHeight = this.layoutElement.minHeight;
			}

			// Token: 0x060024AE RID: 9390 RVA: 0x0009FEF4 File Offset: 0x0009E0F4
			public void SetT(float newT)
			{
				this.t = newT;
				float alpha = Mathf.Clamp01(Util.Remap(this.t, 0.5f, 0.75f, 1f, 0f));
				this.canvasGroup.alpha = alpha;
				float num = Mathf.Clamp01(Util.Remap(this.t, 0.75f, 1f, 1f, 0f));
				num *= num;
				this.layoutElement.minHeight = num * this.originalHeight;
				this.layoutElement.preferredHeight = this.layoutElement.minHeight;
				this.layoutElement.flexibleHeight = 0f;
			}

			// Token: 0x04002260 RID: 8800
			public float t;

			// Token: 0x04002261 RID: 8801
			private readonly float originalHeight;

			// Token: 0x04002262 RID: 8802
			public readonly ObjectivePanelController.ObjectiveTracker objectiveTracker;

			// Token: 0x04002263 RID: 8803
			private readonly LayoutElement layoutElement;

			// Token: 0x04002264 RID: 8804
			private readonly CanvasGroup canvasGroup;
		}
	}
}
