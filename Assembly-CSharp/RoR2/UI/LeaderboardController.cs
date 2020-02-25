using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D8 RID: 1496
	public class LeaderboardController : MonoBehaviour
	{
		// Token: 0x06002373 RID: 9075 RVA: 0x0009ADC0 File Offset: 0x00098FC0
		private void Update()
		{
			if (this.currentLeaderboard != null && this.currentLeaderboard.IsValid && !this.currentLeaderboard.IsQuerying)
			{
				Action action = this.queuedRequest;
				if (action != null)
				{
					action();
				}
				this.queuedRequest = null;
			}
			if (this.noEntryObject)
			{
				this.noEntryObject.SetActive(this.leaderboardInfoList.Count == 0);
			}
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x0009AE30 File Offset: 0x00099030
		private void SetStripCount(int newCount)
		{
			while (this.stripList.Count > newCount)
			{
				UnityEngine.Object.Destroy(this.stripList[this.stripList.Count - 1].gameObject);
				this.stripList.RemoveAt(this.stripList.Count - 1);
			}
			while (this.stripList.Count < newCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.stripPrefab, this.container);
				this.stripList.Add(gameObject.GetComponent<LeaderboardStrip>());
			}
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x0009AEBC File Offset: 0x000990BC
		private void Rebuild()
		{
			this.SetStripCount(this.leaderboardInfoList.Count);
			for (int i = 0; i < this.leaderboardInfoList.Count; i++)
			{
				LeaderboardController.LeaderboardInfo leaderboardInfo = this.leaderboardInfoList[i];
				int num = Mathf.FloorToInt(leaderboardInfo.timeInSeconds / 60f);
				float num2 = leaderboardInfo.timeInSeconds - (float)(num * 60);
				string text = string.Format("{0:0}:{1:00.00}", num, num2);
				this.stripList[i].rankLabel.text = leaderboardInfo.rank.ToString();
				this.stripList[i].usernameLabel.userSteamId = leaderboardInfo.userSteamID;
				this.stripList[i].timeLabel.text = text;
				this.stripList[i].classIcon.texture = SurvivorCatalog.GetSurvivorPortrait(leaderboardInfo.survivorIndex);
				this.stripList[i].isMeImage.enabled = (leaderboardInfo.userSteamID == Client.Instance.SteamId);
				this.stripList[i].usernameLabel.Refresh();
			}
			if (this.animateImageAlpha)
			{
				UnityEngine.UI.Image[] array = new UnityEngine.UI.Image[this.stripList.Count];
				for (int j = 0; j < this.stripList.Count; j++)
				{
					array[this.stripList.Count - 1 - j] = this.stripList[j].GetComponent<UnityEngine.UI.Image>();
				}
				this.animateImageAlpha.ResetStopwatch();
				this.animateImageAlpha.images = array;
			}
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x0009B068 File Offset: 0x00099268
		private void GenerateFakeLeaderboardList(int count)
		{
			this.leaderboardInfoList.Clear();
			for (int i = 1; i <= count; i++)
			{
				LeaderboardController.LeaderboardInfo item = default(LeaderboardController.LeaderboardInfo);
				item.userSteamID = 76561197995890564UL;
				item.survivorIndex = (SurvivorIndex)UnityEngine.Random.Range(0, SurvivorCatalog.survivorCount - 1);
				item.timeInSeconds = UnityEngine.Random.Range(120f, 600f);
				this.leaderboardInfoList.Add(item);
			}
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x0009B0DC File Offset: 0x000992DC
		private void SetLeaderboardInfo(LeaderboardController.LeaderboardInfo[] leaderboardInfos)
		{
			this.leaderboardInfoList.Clear();
			foreach (LeaderboardController.LeaderboardInfo item in leaderboardInfos)
			{
				this.leaderboardInfoList.Add(item);
			}
			this.Rebuild();
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06002378 RID: 9080 RVA: 0x0009B11E File Offset: 0x0009931E
		// (set) Token: 0x06002379 RID: 9081 RVA: 0x0009B126 File Offset: 0x00099326
		public int currentPage { get; private set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x0600237A RID: 9082 RVA: 0x0009B12F File Offset: 0x0009932F
		// (set) Token: 0x0600237B RID: 9083 RVA: 0x0009B137 File Offset: 0x00099337
		public string currentLeaderboardName { get; private set; }

		// Token: 0x0600237C RID: 9084 RVA: 0x0009B140 File Offset: 0x00099340
		public void SetRequestType(string requestTypeName)
		{
			LeaderboardController.RequestType requestType;
			if (Enum.TryParse<LeaderboardController.RequestType>(requestTypeName, false, out requestType))
			{
				this.currentRequestType = requestType;
			}
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x0009B160 File Offset: 0x00099360
		private static LeaderboardController.LeaderboardInfo LeaderboardInfoFromSteamLeaderboardEntry(Leaderboard.Entry entry)
		{
			SurvivorIndex survivorIndex = SurvivorIndex.None;
			int num = (entry.SubScores != null && entry.SubScores.Length >= 1) ? entry.SubScores[1] : 0;
			if (num >= 0 && num < SurvivorCatalog.survivorCount)
			{
				survivorIndex = (SurvivorIndex)num;
			}
			return new LeaderboardController.LeaderboardInfo
			{
				timeInSeconds = (float)entry.Score * 0.001f,
				survivorIndex = survivorIndex,
				userSteamID = entry.SteamId,
				rank = entry.GlobalRank
			};
		}

		// Token: 0x0600237E RID: 9086 RVA: 0x0009B1DC File Offset: 0x000993DC
		public void SetRequestedInfo(string newLeaderboardName, LeaderboardController.RequestType newRequestType, int newPage)
		{
			bool flag = this.currentLeaderboardName != newLeaderboardName;
			if (flag)
			{
				this.currentLeaderboardName = newLeaderboardName;
				this.currentLeaderboard = Client.Instance.GetLeaderboard(this.currentLeaderboardName, Client.LeaderboardSortMethod.None, Client.LeaderboardDisplayType.None);
				newPage = 0;
			}
			bool flag2 = this.currentRequestType != newRequestType || flag;
			bool flag3 = this.currentPage != newPage || flag;
			if (flag2)
			{
				this.currentRequestType = newRequestType;
			}
			if (flag3)
			{
				this.currentPage = newPage;
			}
			if (flag || flag2 || flag3)
			{
				this.queuedRequest = this.GenerateRequest(this.currentLeaderboard, newRequestType, newPage);
			}
		}

		// Token: 0x0600237F RID: 9087 RVA: 0x0009B268 File Offset: 0x00099468
		private Action GenerateRequest(Leaderboard leaderboard, LeaderboardController.RequestType callRequestType, int page)
		{
			Leaderboard.FetchScoresCallback <>9__1;
			return delegate()
			{
				if (this.currentLeaderboard != leaderboard)
				{
					return;
				}
				int num = page * this.entriesPerPage - this.entriesPerPage / 2;
				Leaderboard leaderboard2 = this.currentLeaderboard;
				Leaderboard.RequestType callRequestType2 = (Leaderboard.RequestType)callRequestType;
				int start = num;
				int end = num + this.entriesPerPage;
				Leaderboard.FetchScoresCallback onSuccess;
				if ((onSuccess = <>9__1) == null)
				{
					onSuccess = (<>9__1 = delegate(Leaderboard.Entry[] entries)
					{
						this.SetLeaderboardInfo(entries.Select(new Func<Leaderboard.Entry, LeaderboardController.LeaderboardInfo>(LeaderboardController.LeaderboardInfoFromSteamLeaderboardEntry)).ToArray<LeaderboardController.LeaderboardInfo>());
					});
				}
				leaderboard2.FetchScores(callRequestType2, start, end, onSuccess, delegate(Result reason)
				{
				});
			};
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x0009B296 File Offset: 0x00099496
		private void OrderLeaderboardListByTime(ref List<LeaderboardController.LeaderboardInfo> leaderboardInfoList)
		{
			leaderboardInfoList.Sort(new Comparison<LeaderboardController.LeaderboardInfo>(LeaderboardController.SortByTime));
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x0009B2AB File Offset: 0x000994AB
		private static int SortByTime(LeaderboardController.LeaderboardInfo p1, LeaderboardController.LeaderboardInfo p2)
		{
			return p1.timeInSeconds.CompareTo(p2.timeInSeconds);
		}

		// Token: 0x04002153 RID: 8531
		public GameObject stripPrefab;

		// Token: 0x04002154 RID: 8532
		public RectTransform container;

		// Token: 0x04002155 RID: 8533
		public GameObject noEntryObject;

		// Token: 0x04002156 RID: 8534
		public AnimateImageAlpha animateImageAlpha;

		// Token: 0x04002157 RID: 8535
		private List<LeaderboardStrip> stripList = new List<LeaderboardStrip>();

		// Token: 0x04002158 RID: 8536
		private List<LeaderboardController.LeaderboardInfo> leaderboardInfoList = new List<LeaderboardController.LeaderboardInfo>();

		// Token: 0x04002159 RID: 8537
		public MPButton nextPageButton;

		// Token: 0x0400215A RID: 8538
		public MPButton previousPageButton;

		// Token: 0x0400215B RID: 8539
		public MPButton resetPageButton;

		// Token: 0x0400215C RID: 8540
		public int entriesPerPage = 16;

		// Token: 0x0400215F RID: 8543
		public LeaderboardController.RequestType currentRequestType;

		// Token: 0x04002160 RID: 8544
		private Leaderboard currentLeaderboard;

		// Token: 0x04002161 RID: 8545
		private Action queuedRequest;

		// Token: 0x020005D9 RID: 1497
		private struct LeaderboardInfo
		{
			// Token: 0x04002162 RID: 8546
			public int rank;

			// Token: 0x04002163 RID: 8547
			public ulong userSteamID;

			// Token: 0x04002164 RID: 8548
			public SurvivorIndex survivorIndex;

			// Token: 0x04002165 RID: 8549
			public float timeInSeconds;

			// Token: 0x04002166 RID: 8550
			public bool isMe;
		}

		// Token: 0x020005DA RID: 1498
		public enum RequestType
		{
			// Token: 0x04002168 RID: 8552
			Global,
			// Token: 0x04002169 RID: 8553
			GlobalAroundUser,
			// Token: 0x0400216A RID: 8554
			Friends
		}
	}
}
