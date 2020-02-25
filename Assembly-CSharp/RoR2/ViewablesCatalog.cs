using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000482 RID: 1154
	public static class ViewablesCatalog
	{
		// Token: 0x06001C33 RID: 7219 RVA: 0x00078880 File Offset: 0x00076A80
		public static void AddNodeToRoot(ViewablesCatalog.Node node)
		{
			node.SetParent(ViewablesCatalog.rootNode);
			foreach (ViewablesCatalog.Node node2 in node.Descendants())
			{
				if (ViewablesCatalog.fullNameToNodeMap.ContainsKey(node2.fullName))
				{
					Debug.LogFormat("Tried to add duplicate node {0}", new object[]
					{
						node2.fullName
					});
				}
				else
				{
					ViewablesCatalog.fullNameToNodeMap.Add(node2.fullName, node2);
				}
			}
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x00078910 File Offset: 0x00076B10
		public static ViewablesCatalog.Node FindNode(string fullName)
		{
			ViewablesCatalog.Node result;
			if (ViewablesCatalog.fullNameToNodeMap.TryGetValue(fullName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x00078930 File Offset: 0x00076B30
		[ConCommand(commandName = "viewables_list", flags = ConVarFlags.None, helpText = "Displays the full names of all viewables.")]
		private static void CCViewablesList(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from node in ViewablesCatalog.rootNode.Descendants()
			select node.fullName).ToArray<string>()));
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00078980 File Offset: 0x00076B80
		[ConCommand(commandName = "viewables_list_unviewed", flags = ConVarFlags.None, helpText = "Displays the full names of all unviewed viewables.")]
		private static void CCViewablesListUnviewed(ConCommandArgs args)
		{
			UserProfile userProfile = args.GetSenderLocalUser().userProfile;
			Debug.Log(string.Join("\n", (from node in ViewablesCatalog.rootNode.Descendants()
			where node.shouldShowUnviewed(userProfile)
			select node.fullName).ToArray<string>()));
		}

		// Token: 0x0400191D RID: 6429
		private static readonly ViewablesCatalog.Node rootNode = new ViewablesCatalog.Node("", true, null);

		// Token: 0x0400191E RID: 6430
		private static readonly Dictionary<string, ViewablesCatalog.Node> fullNameToNodeMap = new Dictionary<string, ViewablesCatalog.Node>();

		// Token: 0x02000483 RID: 1155
		public class Node
		{
			// Token: 0x1700031A RID: 794
			// (get) Token: 0x06001C38 RID: 7224 RVA: 0x00078A15 File Offset: 0x00076C15
			// (set) Token: 0x06001C39 RID: 7225 RVA: 0x00078A1D File Offset: 0x00076C1D
			public ViewablesCatalog.Node parent { get; private set; }

			// Token: 0x1700031B RID: 795
			// (get) Token: 0x06001C3A RID: 7226 RVA: 0x00078A26 File Offset: 0x00076C26
			public string fullName
			{
				get
				{
					if (this.fullNameDirty)
					{
						this.GenerateFullName();
					}
					return this._fullName;
				}
			}

			// Token: 0x06001C3B RID: 7227 RVA: 0x00078A3C File Offset: 0x00076C3C
			public Node(string name, bool isFolder, ViewablesCatalog.Node parent = null)
			{
				this.name = name;
				this.isFolder = isFolder;
				this.shouldShowUnviewed = new Func<UserProfile, bool>(this.DefaultShouldShowUnviewedTest);
				this.children = this._children.AsReadOnly();
				this.SetParent(parent);
			}

			// Token: 0x06001C3C RID: 7228 RVA: 0x00078A9C File Offset: 0x00076C9C
			public void SetParent(ViewablesCatalog.Node newParent)
			{
				if (this.parent == newParent)
				{
					return;
				}
				ViewablesCatalog.Node parent = this.parent;
				if (parent != null)
				{
					parent._children.Remove(this);
				}
				this.parent = newParent;
				ViewablesCatalog.Node parent2 = this.parent;
				if (parent2 != null)
				{
					parent2._children.Add(this);
				}
				this.fullNameDirty = true;
			}

			// Token: 0x06001C3D RID: 7229 RVA: 0x00078AF0 File Offset: 0x00076CF0
			private void GenerateFullName()
			{
				string text = this.name;
				if (this.parent != null)
				{
					text = this.parent.fullName + text;
				}
				if (this.isFolder)
				{
					text += "/";
				}
				this._fullName = text;
				this.fullNameDirty = false;
			}

			// Token: 0x06001C3E RID: 7230 RVA: 0x00078B40 File Offset: 0x00076D40
			public bool DefaultShouldShowUnviewedTest(UserProfile userProfile)
			{
				if (!this.isFolder && userProfile.HasViewedViewable(this.fullName))
				{
					return false;
				}
				using (IEnumerator<ViewablesCatalog.Node> enumerator = this.children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.shouldShowUnviewed(userProfile))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x06001C3F RID: 7231 RVA: 0x00078BB4 File Offset: 0x00076DB4
			public IEnumerable<ViewablesCatalog.Node> Descendants()
			{
				yield return this;
				foreach (ViewablesCatalog.Node node in this._children)
				{
					foreach (ViewablesCatalog.Node node2 in node.Descendants())
					{
						yield return node2;
					}
					IEnumerator<ViewablesCatalog.Node> enumerator2 = null;
				}
				List<ViewablesCatalog.Node>.Enumerator enumerator = default(List<ViewablesCatalog.Node>.Enumerator);
				yield break;
				yield break;
			}

			// Token: 0x0400191F RID: 6431
			public readonly string name;

			// Token: 0x04001920 RID: 6432
			public readonly bool isFolder;

			// Token: 0x04001922 RID: 6434
			private readonly List<ViewablesCatalog.Node> _children = new List<ViewablesCatalog.Node>();

			// Token: 0x04001923 RID: 6435
			public ReadOnlyCollection<ViewablesCatalog.Node> children;

			// Token: 0x04001924 RID: 6436
			private string _fullName;

			// Token: 0x04001925 RID: 6437
			private bool fullNameDirty = true;

			// Token: 0x04001926 RID: 6438
			public Func<UserProfile, bool> shouldShowUnviewed;
		}
	}
}
