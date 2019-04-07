using System;
using System.Collections.Generic;
using EntityStates.Missions.Goldshores;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000614 RID: 1556
	public class ObjectivePanelController : MonoBehaviour
	{
		// Token: 0x06002311 RID: 8977 RVA: 0x000A5264 File Offset: 0x000A3464
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

		// Token: 0x06002312 RID: 8978 RVA: 0x000A52C8 File Offset: 0x000A34C8
		private void AddObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.objectiveTrackerPrefab, this.objectiveTrackerContainer);
			gameObject.SetActive(true);
			objectiveTracker.owner = this;
			objectiveTracker.SetStrip(gameObject);
			this.objectiveTrackers.Add(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Add(objectiveTracker.sourceDescriptor, objectiveTracker);
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000A531A File Offset: 0x000A351A
		private void RemoveObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.objectiveTrackers.Remove(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Remove(objectiveTracker.sourceDescriptor);
			objectiveTracker.Retire();
			this.AddExitAnimation(objectiveTracker);
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x000A5348 File Offset: 0x000A3548
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

		// Token: 0x06002315 RID: 8981 RVA: 0x000A54A8 File Offset: 0x000A36A8
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
			if (BossGroup.instance && BossGroup.instance.readOnlyMembersList.Count != 0)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = BossGroup.instance,
					master = master,
					objectiveType = typeof(ObjectivePanelController.DefeatBossObjectiveTracker)
				});
			}
			if (GoldshoresMissionController.instance)
			{
				Type type2 = GoldshoresMissionController.instance.entityStateMachine.state.GetType();
				if (type2 == typeof(ActivateBeacons) || type2 == typeof(GoldshoresBossfight))
				{
					output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
					{
						source = GoldshoresMissionController.instance,
						master = master,
						objectiveType = typeof(ObjectivePanelController.ActivateGoldshoreBeaconTracker)
					});
				}
			}
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x000A5662 File Offset: 0x000A3862
		private void Update()
		{
			this.RefreshObjectiveTrackers();
			this.RunExitAnimations();
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x000A5670 File Offset: 0x000A3870
		private void AddExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.exitAnimations.Add(new ObjectivePanelController.StripExitAnimation(objectiveTracker));
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x000A5684 File Offset: 0x000A3884
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

		// Token: 0x04002602 RID: 9730
		public RectTransform objectiveTrackerContainer;

		// Token: 0x04002603 RID: 9731
		public GameObject objectiveTrackerPrefab;

		// Token: 0x04002604 RID: 9732
		public Sprite checkboxActiveSprite;

		// Token: 0x04002605 RID: 9733
		public Sprite checkboxSuccessSprite;

		// Token: 0x04002606 RID: 9734
		public Sprite checkboxFailSprite;

		// Token: 0x04002607 RID: 9735
		private CharacterMaster currentMaster;

		// Token: 0x04002608 RID: 9736
		private readonly List<ObjectivePanelController.ObjectiveTracker> objectiveTrackers = new List<ObjectivePanelController.ObjectiveTracker>();

		// Token: 0x04002609 RID: 9737
		private Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker> objectiveSourceToTrackerDictionary = new Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker>(EqualityComparer<ObjectivePanelController.ObjectiveSourceDescriptor>.Default);

		// Token: 0x0400260A RID: 9738
		private readonly List<ObjectivePanelController.ObjectiveSourceDescriptor> objectiveSourceDescriptors = new List<ObjectivePanelController.ObjectiveSourceDescriptor>();

		// Token: 0x0400260B RID: 9739
		private readonly List<ObjectivePanelController.StripExitAnimation> exitAnimations = new List<ObjectivePanelController.StripExitAnimation>();

		// Token: 0x02000615 RID: 1557
		public struct ObjectiveSourceDescriptor : IEquatable<ObjectivePanelController.ObjectiveSourceDescriptor>
		{
			// Token: 0x0600231A RID: 8986 RVA: 0x000A5750 File Offset: 0x000A3950
			public override int GetHashCode()
			{
				return (((this.source != null) ? this.source.GetHashCode() : 0) * 397 ^ ((this.master != null) ? this.master.GetHashCode() : 0)) * 397 ^ ((this.objectiveType != null) ? this.objectiveType.GetHashCode() : 0);
			}

			// Token: 0x0600231B RID: 8987 RVA: 0x000A57BF File Offset: 0x000A39BF
			public static bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor a, ObjectivePanelController.ObjectiveSourceDescriptor b)
			{
				return a.source == b.source && a.master == b.master && a.objectiveType == b.objectiveType;
			}

			// Token: 0x0600231C RID: 8988 RVA: 0x000A57BF File Offset: 0x000A39BF
			public bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor other)
			{
				return this.source == other.source && this.master == other.master && this.objectiveType == other.objectiveType;
			}

			// Token: 0x0600231D RID: 8989 RVA: 0x000A57FA File Offset: 0x000A39FA
			public override bool Equals(object obj)
			{
				return obj != null && obj is ObjectivePanelController.ObjectiveSourceDescriptor && this.Equals((ObjectivePanelController.ObjectiveSourceDescriptor)obj);
			}

			// Token: 0x0400260C RID: 9740
			public UnityEngine.Object source;

			// Token: 0x0400260D RID: 9741
			public CharacterMaster master;

			// Token: 0x0400260E RID: 9742
			public Type objectiveType;
		}

		// Token: 0x02000616 RID: 1558
		private class ObjectiveTracker
		{
			// Token: 0x17000317 RID: 791
			// (get) Token: 0x0600231E RID: 8990 RVA: 0x000A5817 File Offset: 0x000A3A17
			// (set) Token: 0x0600231F RID: 8991 RVA: 0x000A581F File Offset: 0x000A3A1F
			public GameObject stripObject { get; private set; }

			// Token: 0x06002320 RID: 8992 RVA: 0x000A5828 File Offset: 0x000A3A28
			public void SetStrip(GameObject stripObject)
			{
				this.stripObject = stripObject;
				this.label = stripObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				this.checkbox = stripObject.transform.Find("Checkbox").GetComponent<Image>();
				this.UpdateStrip();
			}

			// Token: 0x06002321 RID: 8993 RVA: 0x000A5878 File Offset: 0x000A3A78
			public string GetString()
			{
				if (this.IsDirty())
				{
					this.cachedString = this.GenerateString();
				}
				return this.cachedString;
			}

			// Token: 0x06002322 RID: 8994 RVA: 0x000A5894 File Offset: 0x000A3A94
			protected virtual string GenerateString()
			{
				return Language.GetString(this.baseToken);
			}

			// Token: 0x06002323 RID: 8995 RVA: 0x000A58A1 File Offset: 0x000A3AA1
			protected virtual bool IsDirty()
			{
				return this.cachedString == null;
			}

			// Token: 0x06002324 RID: 8996 RVA: 0x000A58AC File Offset: 0x000A3AAC
			public void Retire()
			{
				this.retired = true;
				this.OnRetired();
				this.UpdateStrip();
			}

			// Token: 0x06002325 RID: 8997 RVA: 0x00004507 File Offset: 0x00002707
			protected virtual void OnRetired()
			{
			}

			// Token: 0x06002326 RID: 8998 RVA: 0x000A58C4 File Offset: 0x000A3AC4
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

			// Token: 0x06002327 RID: 8999 RVA: 0x000A5984 File Offset: 0x000A3B84
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

			// Token: 0x0400260F RID: 9743
			public ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor;

			// Token: 0x04002610 RID: 9744
			public ObjectivePanelController owner;

			// Token: 0x04002611 RID: 9745
			public bool isRelevant;

			// Token: 0x04002613 RID: 9747
			protected Image checkbox;

			// Token: 0x04002614 RID: 9748
			protected TextMeshProUGUI label;

			// Token: 0x04002615 RID: 9749
			protected string cachedString;

			// Token: 0x04002616 RID: 9750
			protected string baseToken = "";

			// Token: 0x04002617 RID: 9751
			protected bool retired;
		}

		// Token: 0x02000617 RID: 1559
		private class FindTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002329 RID: 9001 RVA: 0x000A5A07 File Offset: 0x000A3C07
			public FindTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FIND_TELEPORTER";
			}
		}

		// Token: 0x02000618 RID: 1560
		private class ActivateGoldshoreBeaconTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x0600232A RID: 9002 RVA: 0x000A5A1A File Offset: 0x000A3C1A
			public ActivateGoldshoreBeaconTracker()
			{
				this.baseToken = "OBJECTIVE_GOLDSHORES_ACTIVATE_BEACONS";
			}

			// Token: 0x0600232B RID: 9003 RVA: 0x000A5A2D File Offset: 0x000A3C2D
			protected override string GenerateString()
			{
				return string.Format(Language.GetString(this.baseToken), GoldshoresMissionController.instance.beaconsActive, GoldshoresMissionController.instance.beaconsToSpawnOnMap);
			}

			// Token: 0x0600232C RID: 9004 RVA: 0x0000AE8B File Offset: 0x0000908B
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000619 RID: 1561
		private class DestroyTimeCrystals : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x0600232D RID: 9005 RVA: 0x000A5A5D File Offset: 0x000A3C5D
			public DestroyTimeCrystals()
			{
				this.baseToken = "OBJECTIVE_WEEKLYRUN_DESTROY_CRYSTALS";
			}

			// Token: 0x0600232E RID: 9006 RVA: 0x000A5A70 File Offset: 0x000A3C70
			protected override string GenerateString()
			{
				WeeklyRun weeklyRun = Run.instance as WeeklyRun;
				return string.Format(Language.GetString(this.baseToken), weeklyRun.crystalsKilled, weeklyRun.crystalsRequiredToKill);
			}

			// Token: 0x0600232F RID: 9007 RVA: 0x0000AE8B File Offset: 0x0000908B
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x0200061A RID: 1562
		private class ChargeTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002330 RID: 9008 RVA: 0x000A5AAE File Offset: 0x000A3CAE
			public ChargeTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_CHARGE_TELEPORTER";
			}

			// Token: 0x06002331 RID: 9009 RVA: 0x000A5AC8 File Offset: 0x000A3CC8
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

			// Token: 0x06002332 RID: 9010 RVA: 0x000A5B18 File Offset: 0x000A3D18
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

			// Token: 0x06002333 RID: 9011 RVA: 0x000A5B8C File Offset: 0x000A3D8C
			private static int GetTeleporterPercent()
			{
				if (!TeleporterInteraction.instance)
				{
					return 0;
				}
				return Mathf.CeilToInt(TeleporterInteraction.instance.chargeFraction * 100f);
			}

			// Token: 0x06002334 RID: 9012 RVA: 0x0000AE8B File Offset: 0x0000908B
			protected override bool IsDirty()
			{
				return true;
			}

			// Token: 0x04002618 RID: 9752
			private int lastPercent = -1;
		}

		// Token: 0x0200061B RID: 1563
		private class FinishTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002335 RID: 9013 RVA: 0x000A5BB1 File Offset: 0x000A3DB1
			public FinishTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FINISH_TELEPORTER";
			}
		}

		// Token: 0x0200061C RID: 1564
		private class DefeatBossObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002336 RID: 9014 RVA: 0x000A5BC4 File Offset: 0x000A3DC4
			public DefeatBossObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_DEFEAT_BOSS";
			}
		}

		// Token: 0x0200061D RID: 1565
		private class StripExitAnimation
		{
			// Token: 0x06002337 RID: 9015 RVA: 0x000A5BD8 File Offset: 0x000A3DD8
			public StripExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
			{
				this.objectiveTracker = objectiveTracker;
				this.layoutElement = objectiveTracker.stripObject.GetComponent<LayoutElement>();
				this.canvasGroup = objectiveTracker.stripObject.GetComponent<CanvasGroup>();
				this.originalHeight = this.layoutElement.minHeight;
			}

			// Token: 0x06002338 RID: 9016 RVA: 0x000A5C28 File Offset: 0x000A3E28
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

			// Token: 0x04002619 RID: 9753
			public float t;

			// Token: 0x0400261A RID: 9754
			private readonly float originalHeight;

			// Token: 0x0400261B RID: 9755
			public readonly ObjectivePanelController.ObjectiveTracker objectiveTracker;

			// Token: 0x0400261C RID: 9756
			private readonly LayoutElement layoutElement;

			// Token: 0x0400261D RID: 9757
			private readonly CanvasGroup canvasGroup;
		}
	}
}
