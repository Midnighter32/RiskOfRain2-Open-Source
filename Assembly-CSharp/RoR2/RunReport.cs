using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000402 RID: 1026
	public class RunReport
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x060018D9 RID: 6361 RVA: 0x0006B06D File Offset: 0x0006926D
		// (set) Token: 0x060018DA RID: 6362 RVA: 0x0006B08F File Offset: 0x0006928F
		private string gameModeName
		{
			get
			{
				Run gameModePrefabComponent = GameModeCatalog.GetGameModePrefabComponent(this.gameModeIndex);
				return ((gameModePrefabComponent != null) ? gameModePrefabComponent.name : null) ?? "InvalidGameMode";
			}
			set
			{
				this.gameModeIndex = GameModeCatalog.FindGameModeIndex(value);
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x060018DB RID: 6363 RVA: 0x0006B09D File Offset: 0x0006929D
		public int playerInfoCount
		{
			get
			{
				return this.playerInfos.Length;
			}
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0006B0A7 File Offset: 0x000692A7
		[NotNull]
		public RunReport.PlayerInfo GetPlayerInfo(int i)
		{
			return this.playerInfos[i];
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x0006B0B1 File Offset: 0x000692B1
		[CanBeNull]
		public RunReport.PlayerInfo GetPlayerInfoSafe(int i)
		{
			return HGArrayUtilities.GetSafe<RunReport.PlayerInfo>(this.playerInfos, i);
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x0006B0C0 File Offset: 0x000692C0
		public static RunReport Generate([NotNull] Run run, GameResultType resultType)
		{
			RunReport runReport = new RunReport();
			runReport.gameModeIndex = GameModeCatalog.FindGameModeIndex(run.gameObject.name);
			runReport.seed = run.seed;
			runReport.snapshotTime = Run.FixedTimeStamp.now;
			runReport.runStopwatchValue = run.GetRunStopwatch();
			runReport.gameResultType = resultType;
			runReport.ruleBook.Copy(run.ruleBook);
			runReport.playerInfos = new RunReport.PlayerInfo[PlayerCharacterMasterController.instances.Count];
			for (int i = 0; i < runReport.playerInfos.Length; i++)
			{
				runReport.playerInfos[i] = RunReport.PlayerInfo.Generate(PlayerCharacterMasterController.instances[i]);
			}
			runReport.ResolveLocalInformation();
			return runReport;
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x0006B16C File Offset: 0x0006936C
		private void ResolveLocalInformation()
		{
			RunReport.PlayerInfo[] array = this.playerInfos;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveLocalInformation();
			}
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x0006B198 File Offset: 0x00069398
		public void Write(NetworkWriter writer)
		{
			writer.Write((byte)this.gameResultType);
			writer.WritePackedUInt32((uint)this.gameModeIndex);
			writer.Write(this.seed);
			writer.Write(this.snapshotTime);
			writer.Write(this.runStopwatchValue);
			writer.Write(this.ruleBook);
			writer.Write((byte)this.playerInfos.Length);
			for (int i = 0; i < this.playerInfos.Length; i++)
			{
				this.playerInfos[i].Write(writer);
			}
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0006B220 File Offset: 0x00069420
		public void Read(NetworkReader reader)
		{
			this.gameResultType = (GameResultType)reader.ReadByte();
			this.gameModeIndex = (int)reader.ReadPackedUInt32();
			this.seed = reader.ReadUInt64();
			this.snapshotTime = reader.ReadFixedTimeStamp();
			this.runStopwatchValue = reader.ReadSingle();
			reader.ReadRuleBook(this.ruleBook);
			int newSize = (int)reader.ReadByte();
			Array.Resize<RunReport.PlayerInfo>(ref this.playerInfos, newSize);
			for (int i = 0; i < this.playerInfos.Length; i++)
			{
				if (this.playerInfos[i] == null)
				{
					this.playerInfos[i] = new RunReport.PlayerInfo();
				}
				this.playerInfos[i].Read(reader);
			}
			Array.Sort<RunReport.PlayerInfo>(this.playerInfos, delegate(RunReport.PlayerInfo a, RunReport.PlayerInfo b)
			{
				if (a.isLocalPlayer == b.isLocalPlayer)
				{
					if (a.isLocalPlayer)
					{
						return b.localPlayerIndex - a.localPlayerIndex;
					}
					return 0;
				}
				else
				{
					if (!a.isLocalPlayer)
					{
						return 1;
					}
					return -1;
				}
			});
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x0006B2EC File Offset: 0x000694EC
		public static void ToXml(XElement element, RunReport runReport)
		{
			element.RemoveAll();
			element.Add(HGXml.ToXml<string>("version", "2"));
			element.Add(HGXml.ToXml<string>("gameModeName", runReport.gameModeName));
			element.Add(HGXml.ToXml<GameResultType>("gameResultType", runReport.gameResultType));
			element.Add(HGXml.ToXml<ulong>("seed", runReport.seed));
			element.Add(HGXml.ToXml<Run.FixedTimeStamp>("snapshotTime", runReport.snapshotTime));
			element.Add(HGXml.ToXml<float>("runStopwatchValue", runReport.runStopwatchValue));
			element.Add(HGXml.ToXml<RuleBook>("ruleBook", runReport.ruleBook));
			element.Add(HGXml.ToXml<RunReport.PlayerInfo[]>("playerInfos", runReport.playerInfos));
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x0006B3B0 File Offset: 0x000695B0
		public static bool FromXml(XElement element, ref RunReport runReport)
		{
			string text = "NO_VERSION";
			XElement xelement = element.Element("version");
			if (xelement != null)
			{
				xelement.Deserialize(ref text);
			}
			if (text != "2" && !(text == "1"))
			{
				Debug.LogFormat("Could not load RunReport with non-upgradeable version \"{0}\".", new object[]
				{
					text
				});
				runReport = null;
				return false;
			}
			string gameModeName = runReport.gameModeName;
			XElement xelement2 = element.Element("gameModeName");
			if (xelement2 != null)
			{
				xelement2.Deserialize(ref gameModeName);
			}
			runReport.gameModeName = gameModeName;
			XElement xelement3 = element.Element("gameResultType");
			if (xelement3 != null)
			{
				xelement3.Deserialize(ref runReport.gameResultType);
			}
			XElement xelement4 = element.Element("seed");
			if (xelement4 != null)
			{
				xelement4.Deserialize(ref runReport.seed);
			}
			XElement xelement5 = element.Element("snapshotTime");
			if (xelement5 != null)
			{
				xelement5.Deserialize(ref runReport.snapshotTime);
			}
			XElement xelement6 = element.Element("runStopwatchValue");
			if (xelement6 != null)
			{
				xelement6.Deserialize(ref runReport.runStopwatchValue);
			}
			XElement xelement7 = element.Element("ruleBook");
			if (xelement7 != null)
			{
				xelement7.Deserialize(ref runReport.ruleBook);
			}
			XElement xelement8 = element.Element("playerInfos");
			if (xelement8 != null)
			{
				xelement8.Deserialize(ref runReport.playerInfos);
			}
			return true;
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x0006B510 File Offset: 0x00069710
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RunReport.runReportsFolder = Application.dataPath + "/RunReports/";
			HGXml.Register<RunReport>(new HGXml.Serializer<RunReport>(RunReport.ToXml), new HGXml.Deserializer<RunReport>(RunReport.FromXml));
			HGXml.Register<RunReport.PlayerInfo>(new HGXml.Serializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.ToXml), new HGXml.Deserializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.FromXml));
			HGXml.Register<RunReport.PlayerInfo[]>(new HGXml.Serializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayToXml), new HGXml.Deserializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayFromXml));
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x0006B588 File Offset: 0x00069788
		[NotNull]
		private static string FileNameToPath([NotNull] string fileName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}.xml", RunReport.runReportsFolder, fileName);
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0006B5A0 File Offset: 0x000697A0
		[CanBeNull]
		public static RunReport Load([NotNull] string fileName)
		{
			string text = RunReport.FileNameToPath(fileName);
			RunReport result;
			try
			{
				XElement xelement = XDocument.Load(text).Element("RunReport");
				if (xelement == null)
				{
					Debug.LogFormat("Could not load RunReport {0}: {1}", new object[]
					{
						text,
						"File is malformed."
					});
					result = null;
				}
				else
				{
					RunReport runReport = new RunReport();
					RunReport.FromXml(xelement, ref runReport);
					result = runReport;
				}
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Could not load RunReport {0}: {1}", new object[]
				{
					text,
					ex.Message
				});
				result = null;
			}
			return result;
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x0006B638 File Offset: 0x00069838
		public static bool Save([NotNull] RunReport runReport, [NotNull] string fileName)
		{
			string text = RunReport.FileNameToPath(fileName);
			bool result;
			try
			{
				if (!Directory.Exists(RunReport.runReportsFolder))
				{
					Directory.CreateDirectory(RunReport.runReportsFolder);
				}
				XDocument xdocument = new XDocument();
				xdocument.Add(HGXml.ToXml<RunReport>("RunReport", runReport));
				xdocument.Save(text);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Could not save RunReport {0}: {1}", new object[]
				{
					text,
					ex.Message
				});
				result = false;
			}
			return result;
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x0006B6B8 File Offset: 0x000698B8
		public static void TestSerialization(RunReport runReport)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			runReport.Write(networkWriter);
			NetworkReader reader = new NetworkReader(networkWriter.AsArray());
			new RunReport().Read(reader);
		}

		// Token: 0x0400174A RID: 5962
		private const string currentXmlVersion = "2";

		// Token: 0x0400174B RID: 5963
		private int gameModeIndex = -1;

		// Token: 0x0400174C RID: 5964
		public GameResultType gameResultType;

		// Token: 0x0400174D RID: 5965
		public ulong seed;

		// Token: 0x0400174E RID: 5966
		public Run.FixedTimeStamp snapshotTime;

		// Token: 0x0400174F RID: 5967
		public float runStopwatchValue;

		// Token: 0x04001750 RID: 5968
		public RuleBook ruleBook = new RuleBook();

		// Token: 0x04001751 RID: 5969
		private RunReport.PlayerInfo[] playerInfos = Array.Empty<RunReport.PlayerInfo>();

		// Token: 0x04001752 RID: 5970
		private static string runReportsFolder;

		// Token: 0x02000403 RID: 1027
		public class PlayerInfo
		{
			// Token: 0x170002EA RID: 746
			// (get) Token: 0x060018EA RID: 6378 RVA: 0x0006B70E File Offset: 0x0006990E
			[CanBeNull]
			public LocalUser localUser
			{
				get
				{
					if (!this.networkUser)
					{
						return null;
					}
					return this.networkUser.localUser;
				}
			}

			// Token: 0x170002EB RID: 747
			// (get) Token: 0x060018EB RID: 6379 RVA: 0x0006B72A File Offset: 0x0006992A
			public bool isLocalPlayer
			{
				get
				{
					return this.localPlayerIndex >= 0;
				}
			}

			// Token: 0x170002EC RID: 748
			// (get) Token: 0x060018EC RID: 6380 RVA: 0x0006B738 File Offset: 0x00069938
			// (set) Token: 0x060018ED RID: 6381 RVA: 0x0006B75F File Offset: 0x0006995F
			public string bodyName
			{
				get
				{
					GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(this.bodyIndex);
					return ((bodyPrefab != null) ? bodyPrefab.gameObject.name : null) ?? "InvalidBody";
				}
				set
				{
					this.bodyIndex = BodyCatalog.FindBodyIndex(value);
				}
			}

			// Token: 0x170002ED RID: 749
			// (get) Token: 0x060018EE RID: 6382 RVA: 0x0006B76D File Offset: 0x0006996D
			// (set) Token: 0x060018EF RID: 6383 RVA: 0x0006B794 File Offset: 0x00069994
			public string killerBodyName
			{
				get
				{
					GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(this.killerBodyIndex);
					return ((bodyPrefab != null) ? bodyPrefab.gameObject.name : null) ?? "InvalidBody";
				}
				set
				{
					this.killerBodyIndex = BodyCatalog.FindBodyIndex(value);
				}
			}

			// Token: 0x060018F0 RID: 6384 RVA: 0x0006B7A4 File Offset: 0x000699A4
			public void Write(NetworkWriter writer)
			{
				writer.WriteBodyIndex(this.bodyIndex);
				writer.WriteBodyIndex(this.killerBodyIndex);
				writer.Write(this.master ? this.master.gameObject : null);
				this.statSheet.Write(writer);
				writer.WritePackedUInt32((uint)this.itemAcquisitionOrder.Length);
				for (int i = 0; i < this.itemAcquisitionOrder.Length; i++)
				{
					writer.Write(this.itemAcquisitionOrder[i]);
				}
				writer.WriteItemStacks(this.itemStacks);
				writer.WritePackedUInt32((uint)this.equipment.Length);
				for (int j = 0; j < this.equipment.Length; j++)
				{
					writer.Write(this.equipment[j]);
				}
			}

			// Token: 0x060018F1 RID: 6385 RVA: 0x0006B860 File Offset: 0x00069A60
			public void Read(NetworkReader reader)
			{
				this.bodyIndex = reader.ReadBodyIndex();
				this.killerBodyIndex = reader.ReadBodyIndex();
				GameObject gameObject = reader.ReadGameObject();
				this.master = (gameObject ? gameObject.GetComponent<CharacterMaster>() : null);
				this.statSheet.Read(reader);
				int newSize = (int)reader.ReadPackedUInt32();
				Array.Resize<ItemIndex>(ref this.itemAcquisitionOrder, newSize);
				for (int i = 0; i < this.itemAcquisitionOrder.Length; i++)
				{
					ItemIndex itemIndex = reader.ReadItemIndex();
					this.itemAcquisitionOrder[i] = itemIndex;
				}
				reader.ReadItemStacks(this.itemStacks);
				int newSize2 = (int)reader.ReadPackedUInt32();
				Array.Resize<EquipmentIndex>(ref this.equipment, newSize2);
				for (int j = 0; j < this.equipment.Length; j++)
				{
					EquipmentIndex equipmentIndex = reader.ReadEquipmentIndex();
					this.equipment[j] = equipmentIndex;
				}
				this.ResolveLocalInformation();
			}

			// Token: 0x060018F2 RID: 6386 RVA: 0x0006B938 File Offset: 0x00069B38
			public void ResolveLocalInformation()
			{
				this.name = Util.GetBestMasterName(this.master);
				PlayerCharacterMasterController playerCharacterMasterController = null;
				if (this.master)
				{
					playerCharacterMasterController = this.master.GetComponent<PlayerCharacterMasterController>();
				}
				this.networkUser = null;
				if (playerCharacterMasterController)
				{
					this.networkUser = playerCharacterMasterController.networkUser;
				}
				this.localPlayerIndex = -1;
				this.userProfileFileName = string.Empty;
				if (this.networkUser && this.networkUser.localUser != null)
				{
					this.localPlayerIndex = this.networkUser.localUser.id;
					this.userProfileFileName = this.networkUser.localUser.userProfile.fileName;
				}
			}

			// Token: 0x060018F3 RID: 6387 RVA: 0x0006B9EC File Offset: 0x00069BEC
			public static RunReport.PlayerInfo Generate(PlayerCharacterMasterController playerCharacterMasterController)
			{
				CharacterMaster characterMaster = playerCharacterMasterController.master;
				Inventory inventory = characterMaster.inventory;
				PlayerStatsComponent component = playerCharacterMasterController.GetComponent<PlayerStatsComponent>();
				RunReport.PlayerInfo playerInfo = new RunReport.PlayerInfo();
				playerInfo.networkUser = playerCharacterMasterController.networkUser;
				playerInfo.master = characterMaster;
				playerInfo.bodyIndex = BodyCatalog.FindBodyIndex(characterMaster.bodyPrefab);
				playerInfo.killerBodyIndex = characterMaster.GetKillerBodyIndex();
				StatSheet.Copy(component.currentStats, playerInfo.statSheet);
				playerInfo.itemAcquisitionOrder = inventory.itemAcquisitionOrder.ToArray();
				ItemIndex itemIndex = ItemIndex.Syringe;
				ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
				while (itemIndex < itemCount)
				{
					playerInfo.itemStacks[(int)itemIndex] = inventory.GetItemCount(itemIndex);
					itemIndex++;
				}
				playerInfo.equipment = new EquipmentIndex[inventory.GetEquipmentSlotCount()];
				uint num = 0U;
				while ((ulong)num < (ulong)((long)playerInfo.equipment.Length))
				{
					playerInfo.equipment[(int)num] = inventory.GetEquipment(num).equipmentIndex;
					num += 1U;
				}
				return playerInfo;
			}

			// Token: 0x060018F4 RID: 6388 RVA: 0x0006BAC8 File Offset: 0x00069CC8
			public static void ToXml(XElement element, RunReport.PlayerInfo playerInfo)
			{
				element.RemoveAll();
				element.Add(HGXml.ToXml<string>("name", playerInfo.name));
				element.Add(HGXml.ToXml<string>("bodyName", playerInfo.bodyName));
				element.Add(HGXml.ToXml<string>("killerBodyName", playerInfo.killerBodyName));
				element.Add(HGXml.ToXml<StatSheet>("statSheet", playerInfo.statSheet));
				element.Add(HGXml.ToXml<ItemIndex[]>("itemAcquisitionOrder", playerInfo.itemAcquisitionOrder));
				element.Add(HGXml.ToXml<int[]>("itemStacks", playerInfo.itemStacks, RunReport.PlayerInfo.itemStacksRules));
				element.Add(HGXml.ToXml<EquipmentIndex[]>("equipment", playerInfo.equipment, RunReport.PlayerInfo.equipmentRules));
				element.Add(HGXml.ToXml<int>("localPlayerIndex", playerInfo.localPlayerIndex));
				element.Add(HGXml.ToXml<string>("userProfileFileName", playerInfo.userProfileFileName));
			}

			// Token: 0x060018F5 RID: 6389 RVA: 0x0006BBAC File Offset: 0x00069DAC
			public static bool FromXml(XElement element, ref RunReport.PlayerInfo playerInfo)
			{
				playerInfo = new RunReport.PlayerInfo();
				XElement xelement = element.Element("name");
				if (xelement != null)
				{
					xelement.Deserialize(ref playerInfo.name);
				}
				string bodyName = playerInfo.bodyName;
				XElement xelement2 = element.Element("bodyName");
				if (xelement2 != null)
				{
					xelement2.Deserialize(ref bodyName);
				}
				playerInfo.bodyName = bodyName;
				string killerBodyName = playerInfo.killerBodyName;
				XElement xelement3 = element.Element("killerBodyName");
				if (xelement3 != null)
				{
					xelement3.Deserialize(ref killerBodyName);
				}
				playerInfo.killerBodyName = killerBodyName;
				XElement xelement4 = element.Element("statSheet");
				if (xelement4 != null)
				{
					xelement4.Deserialize(ref playerInfo.statSheet);
				}
				XElement xelement5 = element.Element("itemAcquisitionOrder");
				if (xelement5 != null)
				{
					xelement5.Deserialize(ref playerInfo.itemAcquisitionOrder);
				}
				XElement xelement6 = element.Element("itemStacks");
				if (xelement6 != null)
				{
					xelement6.Deserialize(ref playerInfo.itemStacks, RunReport.PlayerInfo.itemStacksRules);
				}
				XElement xelement7 = element.Element("equipment");
				if (xelement7 != null)
				{
					xelement7.Deserialize(ref playerInfo.equipment, RunReport.PlayerInfo.equipmentRules);
				}
				XElement xelement8 = element.Element("localPlayerIndex");
				if (xelement8 != null)
				{
					xelement8.Deserialize(ref playerInfo.localPlayerIndex);
				}
				XElement xelement9 = element.Element("userProfileFileName");
				if (xelement9 != null)
				{
					xelement9.Deserialize(ref playerInfo.userProfileFileName);
				}
				return true;
			}

			// Token: 0x060018F6 RID: 6390 RVA: 0x0006BD14 File Offset: 0x00069F14
			public static void ArrayToXml(XElement element, RunReport.PlayerInfo[] playerInfos)
			{
				element.RemoveAll();
				for (int i = 0; i < playerInfos.Length; i++)
				{
					element.Add(HGXml.ToXml<RunReport.PlayerInfo>("PlayerInfo", playerInfos[i]));
				}
			}

			// Token: 0x060018F7 RID: 6391 RVA: 0x0006BD48 File Offset: 0x00069F48
			public static bool ArrayFromXml(XElement element, ref RunReport.PlayerInfo[] playerInfos)
			{
				playerInfos = (from e in element.Elements()
				where e.Name == "PlayerInfo"
				select e).Select(delegate(XElement e)
				{
					RunReport.PlayerInfo result = null;
					HGXml.FromXml<RunReport.PlayerInfo>(e, ref result);
					return result;
				}).ToArray<RunReport.PlayerInfo>();
				return true;
			}

			// Token: 0x04001753 RID: 5971
			[CanBeNull]
			public NetworkUser networkUser;

			// Token: 0x04001754 RID: 5972
			[CanBeNull]
			public CharacterMaster master;

			// Token: 0x04001755 RID: 5973
			public int localPlayerIndex = -1;

			// Token: 0x04001756 RID: 5974
			public string name = string.Empty;

			// Token: 0x04001757 RID: 5975
			public int bodyIndex = -1;

			// Token: 0x04001758 RID: 5976
			public int killerBodyIndex = -1;

			// Token: 0x04001759 RID: 5977
			public StatSheet statSheet = StatSheet.New();

			// Token: 0x0400175A RID: 5978
			public ItemIndex[] itemAcquisitionOrder = Array.Empty<ItemIndex>();

			// Token: 0x0400175B RID: 5979
			public int[] itemStacks = ItemCatalog.RequestItemStackArray();

			// Token: 0x0400175C RID: 5980
			public EquipmentIndex[] equipment = Array.Empty<EquipmentIndex>();

			// Token: 0x0400175D RID: 5981
			public string userProfileFileName = string.Empty;

			// Token: 0x0400175E RID: 5982
			private static readonly HGXml.SerializationRules<int[]> itemStacksRules = new HGXml.SerializationRules<int[]>
			{
				serializer = delegate(XElement element, int[] value)
				{
					element.RemoveAll();
					element.Add(from itemIndex in ItemCatalog.allItems
					where value[(int)itemIndex] > 0
					select new XElement(itemIndex.ToString(), value[(int)itemIndex]));
				},
				deserializer = delegate(XElement element, ref int[] value)
				{
					Array.Resize<int>(ref value, ItemCatalog.itemCount);
					foreach (XElement xelement in element.Elements())
					{
						ItemIndex itemIndex = ItemCatalog.FindItemIndex(xelement.Name.LocalName);
						if (ItemCatalog.IsIndexValid(itemIndex))
						{
							HGXml.FromXml<int>(element, ref value[(int)itemIndex]);
						}
					}
					return true;
				}
			};

			// Token: 0x0400175F RID: 5983
			private static readonly HGXml.SerializationRules<EquipmentIndex[]> equipmentRules = new HGXml.SerializationRules<EquipmentIndex[]>
			{
				serializer = delegate(XElement element, EquipmentIndex[] value)
				{
					element.Value = string.Join(" ", from equipmentIndex in value
					select equipmentIndex.ToString());
				},
				deserializer = delegate(XElement element, ref EquipmentIndex[] value)
				{
					value = element.Value.Split(new char[]
					{
						' '
					}).Select(delegate(string str)
					{
						EquipmentIndex result;
						if (!Enum.TryParse<EquipmentIndex>(str, false, out result))
						{
							return EquipmentIndex.None;
						}
						return result;
					}).ToArray<EquipmentIndex>();
					return true;
				}
			};
		}
	}
}
