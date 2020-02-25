using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000623 RID: 1571
	[RequireComponent(typeof(RectTransform))]
	public class ServerBrowserController : MonoBehaviour
	{
		// Token: 0x06002523 RID: 9507 RVA: 0x000A1E8D File Offset: 0x000A008D
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.stripContainer.gameObject.AddComponent<ServerBrowserController.StripLayoutGroup>().serverBrowserController = this;
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x000A1EB6 File Offset: 0x000A00B6
		private void Start()
		{
			this.RequestServers();
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x000A1EBE File Offset: 0x000A00BE
		private void OnEnable()
		{
			ServerBrowserController.instance = SingletonHelper.Assign<ServerBrowserController>(ServerBrowserController.instance, this);
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x000A1ED0 File Offset: 0x000A00D0
		private void OnDisable()
		{
			ServerBrowserController.instance = SingletonHelper.Unassign<ServerBrowserController>(ServerBrowserController.instance, this);
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x000A1EE4 File Offset: 0x000A00E4
		private void AddServer(ServerBrowserController.ServerInfo serverInfo)
		{
			this.currentServers.Add(serverInfo);
			ServerBrowserStripController serverBrowserStripController = serverInfo.stripController = UnityEngine.Object.Instantiate<GameObject>(this.stripPrefab, this.stripContainer).GetComponent<ServerBrowserStripController>();
			serverBrowserStripController.nameLabel.SetText(serverInfo.name);
			serverBrowserStripController.addressLabel.SetText(serverInfo.address.ToString());
			serverBrowserStripController.playerCountLabel.SetText(serverInfo.playerCountString);
			serverBrowserStripController.latencyLabel.SetText(serverInfo.latencyString);
			serverInfo.stripController.gameObject.SetActive(true);
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x000A1F78 File Offset: 0x000A0178
		private void ClearServers()
		{
			for (int i = this.currentServers.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this.currentServers[i].stripController.gameObject);
			}
			this.currentServers.Clear();
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x000A1FC4 File Offset: 0x000A01C4
		public void RequestServers()
		{
			ServerBrowserController.<>c__DisplayClass19_0 CS$<>8__locals1 = new ServerBrowserController.<>c__DisplayClass19_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.currentRequest != null)
			{
				this.currentRequest.Dispose();
				this.currentRequest = null;
			}
			CS$<>8__locals1.request = Client.Instance.ServerList.Internet(null);
			this.currentRequest = CS$<>8__locals1.request;
			CS$<>8__locals1.request.OnUpdate = new Action(CS$<>8__locals1.<RequestServers>g__OnUpdate|2);
			CS$<>8__locals1.request.OnServerResponded = new Action<ServerList.Server>(CS$<>8__locals1.<RequestServers>g__OnServerResponded|1);
			CS$<>8__locals1.request.OnFinished = new Action(CS$<>8__locals1.<RequestServers>g__OnFinished|0);
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x000A205F File Offset: 0x000A025F
		private void OnDestroy()
		{
			if (this.currentRequest != null)
			{
				this.currentRequest.Dispose();
				this.currentRequest = null;
			}
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x000A207B File Offset: 0x000A027B
		[ConCommand(commandName = "open_server_browser", flags = ConVarFlags.None, helpText = "Opens the server browser window.")]
		private static void CCOpenServerBrowser(ConCommandArgs args)
		{
			if (!ServerBrowserController.instance)
			{
				UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ServerBrowserWindow")));
			}
		}

		// Token: 0x040022E1 RID: 8929
		private static ServerBrowserController instance;

		// Token: 0x040022E2 RID: 8930
		private RectTransform rectTransform;

		// Token: 0x040022E3 RID: 8931
		public ScrollRect scrollRect;

		// Token: 0x040022E4 RID: 8932
		public RectTransform stripContainer;

		// Token: 0x040022E5 RID: 8933
		public GameObject stripPrefab;

		// Token: 0x040022E6 RID: 8934
		public RectTransform nameHeader;

		// Token: 0x040022E7 RID: 8935
		public RectTransform addressHeader;

		// Token: 0x040022E8 RID: 8936
		public RectTransform playerCountHeader;

		// Token: 0x040022E9 RID: 8937
		public RectTransform latencyHeader;

		// Token: 0x040022EA RID: 8938
		private readonly List<ServerBrowserController.ServerInfo> currentServers = new List<ServerBrowserController.ServerInfo>();

		// Token: 0x040022EB RID: 8939
		private ServerList.Request currentRequest;

		// Token: 0x02000624 RID: 1572
		private class StripLayoutGroup : MonoBehaviour, ILayoutGroup, ILayoutController
		{
			// Token: 0x0600252E RID: 9518 RVA: 0x000A20B0 File Offset: 0x000A02B0
			private void Awake()
			{
				this.rectTransform = (RectTransform)base.transform;
			}

			// Token: 0x0600252F RID: 9519 RVA: 0x000A20C4 File Offset: 0x000A02C4
			public void SetLayoutHorizontal()
			{
				Rect rect = ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__GetRect|3_1(this.serverBrowserController.nameHeader);
				Rect rect2 = ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__GetRect|3_1(this.serverBrowserController.addressHeader);
				Rect rect3 = ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__GetRect|3_1(this.serverBrowserController.playerCountHeader);
				Rect rect4 = ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__GetRect|3_1(this.serverBrowserController.latencyHeader);
				int i = 0;
				int childCount = this.rectTransform.childCount;
				while (i < childCount)
				{
					ServerBrowserStripController component = this.rectTransform.GetChild(i).GetComponent<ServerBrowserStripController>();
					ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__MatchRect|3_0((RectTransform)component.nameLabel.transform, rect);
					ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__MatchRect|3_0((RectTransform)component.addressLabel.transform, rect2);
					ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__MatchRect|3_0((RectTransform)component.playerCountLabel.transform, rect3);
					ServerBrowserController.StripLayoutGroup.<SetLayoutHorizontal>g__MatchRect|3_0((RectTransform)component.latencyLabel.transform, rect4);
					i++;
				}
			}

			// Token: 0x06002530 RID: 9520 RVA: 0x000A21A0 File Offset: 0x000A03A0
			public void SetLayoutVertical()
			{
				int i = 0;
				int childCount = this.rectTransform.childCount;
				while (i < childCount)
				{
					ServerBrowserController.StripLayoutGroup.<SetLayoutVertical>g__SetTransformLocalY|4_0(this.rectTransform.GetChild(i), (float)i * -32f);
					i++;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this.rectTransform.childCount * 32f);
			}

			// Token: 0x06002532 RID: 9522 RVA: 0x000A21FC File Offset: 0x000A03FC
			[CompilerGenerated]
			internal static void <SetLayoutHorizontal>g__MatchRect|3_0(RectTransform childRectTransform, in Rect rect)
			{
				Vector2 offsetMin = childRectTransform.offsetMin;
				Vector2 offsetMax = childRectTransform.offsetMax;
				Rect rect2 = rect;
				offsetMin.x = rect2.xMin;
				rect2 = rect;
				offsetMax.x = rect2.xMax;
				childRectTransform.offsetMin = offsetMin;
				childRectTransform.offsetMax = offsetMax;
			}

			// Token: 0x06002533 RID: 9523 RVA: 0x000A2250 File Offset: 0x000A0450
			[CompilerGenerated]
			internal static Rect <SetLayoutHorizontal>g__GetRect|3_1(RectTransform rectTransform)
			{
				Rect rect = rectTransform.rect;
				rect.x += rectTransform.anchoredPosition.x;
				return rect;
			}

			// Token: 0x06002534 RID: 9524 RVA: 0x000A2280 File Offset: 0x000A0480
			[CompilerGenerated]
			internal static void <SetLayoutVertical>g__SetTransformLocalY|4_0(Transform transform, float localY)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.y = localY;
				transform.localPosition = localPosition;
			}

			// Token: 0x040022EC RID: 8940
			private RectTransform rectTransform;

			// Token: 0x040022ED RID: 8941
			public ServerBrowserController serverBrowserController;
		}

		// Token: 0x02000625 RID: 1573
		private class ServerInfo
		{
			// Token: 0x06002535 RID: 9525 RVA: 0x000A22A3 File Offset: 0x000A04A3
			public ServerInfo(ServerList.Server server)
			{
				this.server = server;
			}

			// Token: 0x170003D6 RID: 982
			// (get) Token: 0x06002536 RID: 9526 RVA: 0x000A22B2 File Offset: 0x000A04B2
			public string name
			{
				get
				{
					return this.server.Name;
				}
			}

			// Token: 0x170003D7 RID: 983
			// (get) Token: 0x06002537 RID: 9527 RVA: 0x000A22BF File Offset: 0x000A04BF
			public IPAddress address
			{
				get
				{
					return this.server.Address;
				}
			}

			// Token: 0x170003D8 RID: 984
			// (get) Token: 0x06002538 RID: 9528 RVA: 0x000A22CC File Offset: 0x000A04CC
			public string playerCountString
			{
				get
				{
					return TextSerialization.ToStringInvariant(this.server.Players) + "/" + TextSerialization.ToStringInvariant(this.server.MaxPlayers);
				}
			}

			// Token: 0x170003D9 RID: 985
			// (get) Token: 0x06002539 RID: 9529 RVA: 0x000A22F8 File Offset: 0x000A04F8
			public string latencyString
			{
				get
				{
					return TextSerialization.ToStringInvariant(this.server.Ping);
				}
			}

			// Token: 0x040022EE RID: 8942
			private ServerList.Server server;

			// Token: 0x040022EF RID: 8943
			public ServerBrowserStripController stripController;
		}
	}
}
