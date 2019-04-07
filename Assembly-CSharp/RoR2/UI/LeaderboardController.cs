using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F3 RID: 1523
	public class LeaderboardController : MonoBehaviour
	{
		// Token: 0x0600222E RID: 8750 RVA: 0x000A19B8 File Offset: 0x0009FBB8
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

		// Token: 0x0600222F RID: 8751 RVA: 0x000A1A28 File Offset: 0x0009FC28
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

		// Token: 0x06002230 RID: 8752 RVA: 0x000A1AB4 File Offset: 0x0009FCB4
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

		// Token: 0x06002231 RID: 8753 RVA: 0x000A1C60 File Offset: 0x0009FE60
		private void GenerateFakeLeaderboardList(int count)
		{
			this.leaderboardInfoList.Clear();
			for (int i = 1; i <= count; i++)
			{
				LeaderboardController.LeaderboardInfo item = default(LeaderboardController.LeaderboardInfo);
				item.userSteamID = 76561197995890564UL;
				item.survivorIndex = (SurvivorIndex)UnityEngine.Random.Range(0, 6);
				item.timeInSeconds = UnityEngine.Random.Range(120f, 600f);
				this.leaderboardInfoList.Add(item);
			}
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x000A1CCC File Offset: 0x0009FECC
		private void SetLeaderboardInfo(LeaderboardController.LeaderboardInfo[] leaderboardInfos)
		{
			this.leaderboardInfoList.Clear();
			foreach (LeaderboardController.LeaderboardInfo item in leaderboardInfos)
			{
				this.leaderboardInfoList.Add(item);
			}
			this.Rebuild();
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06002233 RID: 8755 RVA: 0x000A1D0E File Offset: 0x0009FF0E
		// (set) Token: 0x06002234 RID: 8756 RVA: 0x000A1D16 File Offset: 0x0009FF16
		public int currentPage { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06002235 RID: 8757 RVA: 0x000A1D1F File Offset: 0x0009FF1F
		// (set) Token: 0x06002236 RID: 8758 RVA: 0x000A1D27 File Offset: 0x0009FF27
		public string currentLeaderboardName { get; private set; }

		// Token: 0x06002237 RID: 8759 RVA: 0x000A1D30 File Offset: 0x0009FF30
		public void SetRequestType(string requestTypeName)
		{
			LeaderboardController.RequestType requestType;
			if (Enum.TryParse<LeaderboardController.RequestType>(requestTypeName, false, out requestType))
			{
				this.currentRequestType = requestType;
			}
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x000A1D50 File Offset: 0x0009FF50
		private static LeaderboardController.LeaderboardInfo LeaderboardInfoFromSteamLeaderboardEntry(Leaderboard.Entry entry)
		{
			SurvivorIndex survivorIndex = SurvivorIndex.None;
			int num = (entry.SubScores != null && entry.SubScores.Length >= 1) ? entry.SubScores[1] : 0;
			if (num >= 0 && num < 7)
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

		// Token: 0x06002239 RID: 8761 RVA: 0x000A1DC8 File Offset: 0x0009FFC8
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

		// Token: 0x0600223A RID: 8762 RVA: 0x000A1E54 File Offset: 0x000A0054
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

		// Token: 0x0600223B RID: 8763 RVA: 0x000A1E82 File Offset: 0x000A0082
		private void OrderLeaderboardListByTime(ref List<LeaderboardController.LeaderboardInfo> leaderboardInfoList)
		{
			leaderboardInfoList.Sort(new Comparison<LeaderboardController.LeaderboardInfo>(LeaderboardController.SortByTime));
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x000A1E97 File Offset: 0x000A0097
		private static int SortByTime(LeaderboardController.LeaderboardInfo p1, LeaderboardController.LeaderboardInfo p2)
		{
			return p1.timeInSeconds.CompareTo(p2.timeInSeconds);
		}

		// Token: 0x04002534 RID: 9524
		public GameObject stripPrefab;

		// Token: 0x04002535 RID: 9525
		public RectTransform container;

		// Token: 0x04002536 RID: 9526
		public GameObject noEntryObject;

		// Token: 0x04002537 RID: 9527
		public AnimateImageAlpha animateImageAlpha;

		// Token: 0x04002538 RID: 9528
		private List<LeaderboardStrip> stripList = new List<LeaderboardStrip>();

		// Token: 0x04002539 RID: 9529
		private List<LeaderboardController.LeaderboardInfo> leaderboardInfoList = new List<LeaderboardController.LeaderboardInfo>();

		// Token: 0x0400253A RID: 9530
		public MPButton nextPageButton;

		// Token: 0x0400253B RID: 9531
		public MPButton previousPageButton;

		// Token: 0x0400253C RID: 9532
		public MPButton resetPageButton;

		// Token: 0x0400253D RID: 9533
		public int entriesPerPage = 16;

		// Token: 0x04002540 RID: 9536
		public LeaderboardController.RequestType currentRequestType;

		// Token: 0x04002541 RID: 9537
		private Leaderboard currentLeaderboard;

		// Token: 0x04002542 RID: 9538
		private Action queuedRequest;

		// Token: 0x020005F4 RID: 1524
		private struct LeaderboardInfo
		{
			// Token: 0x04002543 RID: 9539
			public int rank;

			// Token: 0x04002544 RID: 9540
			public ulong userSteamID;

			// Token: 0x04002545 RID: 9541
			public SurvivorIndex survivorIndex;

			// Token: 0x04002546 RID: 9542
			public float timeInSeconds;

			// Token: 0x04002547 RID: 9543
			public bool isMe;
		}

		// Token: 0x020005F5 RID: 1525
		public enum RequestType
		{
			// Token: 0x04002549 RID: 9545
			Global,
			// Token: 0x0400254A RID: 9546
			GlobalAroundUser,
			// Token: 0x0400254B RID: 9547
			Friends
		}
	}
}
