using System;
using System.Collections.Generic;
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
	// Token: 0x02000480 RID: 1152
	public class RunReport
	{
		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060019B7 RID: 6583 RVA: 0x0007AC91 File Offset: 0x00078E91
		// (set) Token: 0x060019B8 RID: 6584 RVA: 0x0007ACB3 File Offset: 0x00078EB3
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

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060019B9 RID: 6585 RVA: 0x0007ACC1 File Offset: 0x00078EC1
		public int playerInfoCount
		{
			get
			{
				return this.playerInfos.Length;
			}
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0007ACCB File Offset: 0x00078ECB
		[NotNull]
		public RunReport.PlayerInfo GetPlayerInfo(int i)
		{
			return this.playerInfos[i];
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x0007ACD5 File Offset: 0x00078ED5
		[CanBeNull]
		public RunReport.PlayerInfo GetPlayerInfoSafe(int i)
		{
			return HGArrayUtilities.GetSafe<RunReport.PlayerInfo>(this.playerInfos, i);
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0007ACE4 File Offset: 0x00078EE4
		public static RunReport Generate([NotNull] Run run, GameResultType resultType)
		{
			RunReport runReport = new RunReport();
			runReport.gameModeIndex = GameModeCatalog.FindGameModeIndex(run.gameObject.name);
			runReport.seed = run.seed;
			runReport.snapshotTime = Run.FixedTimeStamp.now;
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

		// Token: 0x060019BD RID: 6589 RVA: 0x0007AD84 File Offset: 0x00078F84
		private void ResolveLocalInformation()
		{
			RunReport.PlayerInfo[] array = this.playerInfos;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveLocalInformation();
			}
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0007ADB0 File Offset: 0x00078FB0
		public void Write(NetworkWriter writer)
		{
			writer.Write((byte)this.gameResultType);
			writer.WritePackedUInt32((uint)this.gameModeIndex);
			writer.Write(this.seed);
			writer.Write(this.snapshotTime);
			writer.Write(this.ruleBook);
			writer.Write((byte)this.playerInfos.Length);
			for (int i = 0; i < this.playerInfos.Length; i++)
			{
				this.playerInfos[i].Write(writer);
			}
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0007AE2C File Offset: 0x0007902C
		public void Read(NetworkReader reader)
		{
			this.gameResultType = (GameResultType)reader.ReadByte();
			this.gameModeIndex = (int)reader.ReadPackedUInt32();
			this.seed = reader.ReadUInt64();
			this.snapshotTime = reader.ReadFixedTimeStamp();
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

		// Token: 0x060019C0 RID: 6592 RVA: 0x0007AEEC File Offset: 0x000790EC
		public static void ToXml(XElement element, RunReport runReport)
		{
			element.RemoveAll();
			element.Add(HGXml.ToXml<string>("version", "2"));
			element.Add(HGXml.ToXml<string>("gameModeName", runReport.gameModeName));
			element.Add(HGXml.ToXml<GameResultType>("gameResultType", runReport.gameResultType));
			element.Add(HGXml.ToXml<ulong>("seed", runReport.seed));
			element.Add(HGXml.ToXml<Run.FixedTimeStamp>("snapshotTime", runReport.snapshotTime));
			element.Add(HGXml.ToXml<RuleBook>("ruleBook", runReport.ruleBook));
			element.Add(HGXml.ToXml<RunReport.PlayerInfo[]>("playerInfos", runReport.playerInfos));
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x0007AF98 File Offset: 0x00079198
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
			XElement xelement6 = element.Element("ruleBook");
			if (xelement6 != null)
			{
				xelement6.Deserialize(ref runReport.ruleBook);
			}
			XElement xelement7 = element.Element("playerInfos");
			if (xelement7 != null)
			{
				xelement7.Deserialize(ref runReport.playerInfos);
			}
			return true;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x0007B0D4 File Offset: 0x000792D4
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RunReport.runReportsFolder = Application.dataPath + "/RunReports/";
			HGXml.Register<RunReport>(new HGXml.Serializer<RunReport>(RunReport.ToXml), new HGXml.Deserializer<RunReport>(RunReport.FromXml));
			HGXml.Register<RunReport.PlayerInfo>(new HGXml.Serializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.ToXml), new HGXml.Deserializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.FromXml));
			HGXml.Register<RunReport.PlayerInfo[]>(new HGXml.Serializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayToXml), new HGXml.Deserializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayFromXml));
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x0007B14C File Offset: 0x0007934C
		[NotNull]
		private static string FileNameToPath([NotNull] string fileName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}.xml", RunReport.runReportsFolder, fileName);
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x0007B164 File Offset: 0x00079364
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

		// Token: 0x060019C5 RID: 6597 RVA: 0x0007B1FC File Offset: 0x000793FC
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

		// Token: 0x060019C6 RID: 6598 RVA: 0x0007B27C File Offset: 0x0007947C
		public static void TestSerialization(RunReport runReport)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			runReport.Write(networkWriter);
			NetworkReader reader = new NetworkReader(networkWriter.AsArray());
			new RunReport().Read(reader);
		}

		// Token: 0x04001D1F RID: 7455
		private const string currentXmlVersion = "2";

		// Token: 0x04001D20 RID: 7456
		private int gameModeIndex = -1;

		// Token: 0x04001D21 RID: 7457
		public GameResultType gameResultType;

		// Token: 0x04001D22 RID: 7458
		public ulong seed;

		// Token: 0x04001D23 RID: 7459
		public Run.FixedTimeStamp snapshotTime;

		// Token: 0x04001D24 RID: 7460
		public RuleBook ruleBook = new RuleBook();

		// Token: 0x04001D25 RID: 7461
		private RunReport.PlayerInfo[] playerInfos = Array.Empty<RunReport.PlayerInfo>();

		// Token: 0x04001D26 RID: 7462
		private static string runReportsFolder;

		// Token: 0x02000481 RID: 1153
		public class PlayerInfo
		{
			// Token: 0x17000267 RID: 615
			// (get) Token: 0x060019C8 RID: 6600 RVA: 0x0007B2D2 File Offset: 0x000794D2
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

			// Token: 0x17000268 RID: 616
			// (get) Token: 0x060019C9 RID: 6601 RVA: 0x0007B2EE File Offset: 0x000794EE
			public bool isLocalPlayer
			{
				get
				{
					return this.localPlayerIndex >= 0;
				}
			}

			// Token: 0x17000269 RID: 617
			// (get) Token: 0x060019CA RID: 6602 RVA: 0x0007B2FC File Offset: 0x000794FC
			// (set) Token: 0x060019CB RID: 6603 RVA: 0x0007B323 File Offset: 0x00079523
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

			// Token: 0x1700026A RID: 618
			// (get) Token: 0x060019CC RID: 6604 RVA: 0x0007B331 File Offset: 0x00079531
			// (set) Token: 0x060019CD RID: 6605 RVA: 0x0007B358 File Offset: 0x00079558
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

			// Token: 0x060019CE RID: 6606 RVA: 0x0007B368 File Offset: 0x00079568
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

			// Token: 0x060019CF RID: 6607 RVA: 0x0007B424 File Offset: 0x00079624
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

			// Token: 0x060019D0 RID: 6608 RVA: 0x0007B4FC File Offset: 0x000796FC
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

			// Token: 0x060019D1 RID: 6609 RVA: 0x0007B5B0 File Offset: 0x000797B0
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
				for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
				{
					playerInfo.itemStacks[(int)itemIndex] = inventory.GetItemCount(itemIndex);
				}
				playerInfo.equipment = new EquipmentIndex[inventory.GetEquipmentSlotCount()];
				uint num = 0u;
				while ((ulong)num < (ulong)((long)playerInfo.equipment.Length))
				{
					playerInfo.equipment[(int)num] = inventory.GetEquipment(num).equipmentIndex;
					num += 1u;
				}
				return playerInfo;
			}

			// Token: 0x060019D2 RID: 6610 RVA: 0x0007B688 File Offset: 0x00079888
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

			// Token: 0x060019D3 RID: 6611 RVA: 0x0007B76C File Offset: 0x0007996C
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

			// Token: 0x060019D4 RID: 6612 RVA: 0x0007B8D4 File Offset: 0x00079AD4
			public static void ArrayToXml(XElement element, RunReport.PlayerInfo[] playerInfos)
			{
				element.RemoveAll();
				for (int i = 0; i < playerInfos.Length; i++)
				{
					element.Add(HGXml.ToXml<RunReport.PlayerInfo>("PlayerInfo", playerInfos[i]));
				}
			}

			// Token: 0x060019D5 RID: 6613 RVA: 0x0007B908 File Offset: 0x00079B08
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

			// Token: 0x04001D27 RID: 7463
			[CanBeNull]
			public NetworkUser networkUser;

			// Token: 0x04001D28 RID: 7464
			[CanBeNull]
			public CharacterMaster master;

			// Token: 0x04001D29 RID: 7465
			public int localPlayerIndex = -1;

			// Token: 0x04001D2A RID: 7466
			public string name = string.Empty;

			// Token: 0x04001D2B RID: 7467
			public int bodyIndex = -1;

			// Token: 0x04001D2C RID: 7468
			public int killerBodyIndex = -1;

			// Token: 0x04001D2D RID: 7469
			public StatSheet statSheet = StatSheet.New();

			// Token: 0x04001D2E RID: 7470
			public ItemIndex[] itemAcquisitionOrder = Array.Empty<ItemIndex>();

			// Token: 0x04001D2F RID: 7471
			public int[] itemStacks = new int[78];

			// Token: 0x04001D30 RID: 7472
			public EquipmentIndex[] equipment = Array.Empty<EquipmentIndex>();

			// Token: 0x04001D31 RID: 7473
			public string userProfileFileName = string.Empty;

			// Token: 0x04001D32 RID: 7474
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
					Array.Resize<int>(ref value, 78);
					using (IEnumerator<XElement> enumerator = element.Elements().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ItemIndex itemIndex;
							if (Enum.TryParse<ItemIndex>(enumerator.Current.Name.LocalName, true, out itemIndex) && itemIndex >= ItemIndex.Syringe)
							{
								HGXml.FromXml<int>(element, ref value[(int)itemIndex]);
							}
						}
					}
					return true;
				}
			};

			// Token: 0x04001D33 RID: 7475
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
