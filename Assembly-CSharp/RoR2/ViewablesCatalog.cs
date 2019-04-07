using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004E7 RID: 1255
	public static class ViewablesCatalog
	{
		// Token: 0x06001C70 RID: 7280 RVA: 0x00084CB4 File Offset: 0x00082EB4
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

		// Token: 0x06001C71 RID: 7281 RVA: 0x00084D44 File Offset: 0x00082F44
		public static ViewablesCatalog.Node FindNode(string fullName)
		{
			ViewablesCatalog.Node result;
			if (ViewablesCatalog.fullNameToNodeMap.TryGetValue(fullName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x00084D64 File Offset: 0x00082F64
		[ConCommand(commandName = "viewables_list", flags = ConVarFlags.None, helpText = "Displays the full names of all viewables.")]
		private static void CCViewablesList(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from node in ViewablesCatalog.rootNode.Descendants()
			select node.fullName).ToArray<string>()));
		}

		// Token: 0x04001E81 RID: 7809
		private static readonly ViewablesCatalog.Node rootNode = new ViewablesCatalog.Node("", true, null);

		// Token: 0x04001E82 RID: 7810
		private static readonly Dictionary<string, ViewablesCatalog.Node> fullNameToNodeMap = new Dictionary<string, ViewablesCatalog.Node>();

		// Token: 0x020004E8 RID: 1256
		public class Node
		{
			// Token: 0x1700028D RID: 653
			// (get) Token: 0x06001C74 RID: 7284 RVA: 0x00084DD0 File Offset: 0x00082FD0
			// (set) Token: 0x06001C75 RID: 7285 RVA: 0x00084DD8 File Offset: 0x00082FD8
			public ViewablesCatalog.Node parent { get; private set; }

			// Token: 0x1700028E RID: 654
			// (get) Token: 0x06001C76 RID: 7286 RVA: 0x00084DE1 File Offset: 0x00082FE1
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

			// Token: 0x06001C77 RID: 7287 RVA: 0x00084DF8 File Offset: 0x00082FF8
			public Node(string name, bool isFolder, ViewablesCatalog.Node parent = null)
			{
				this.name = name;
				this.isFolder = isFolder;
				this.shouldShowUnviewed = new Func<UserProfile, bool>(this.DefaultShouldShowUnviewedTest);
				this.children = this._children.AsReadOnly();
				this.SetParent(parent);
			}

			// Token: 0x06001C78 RID: 7288 RVA: 0x00084E58 File Offset: 0x00083058
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

			// Token: 0x06001C79 RID: 7289 RVA: 0x00084EAC File Offset: 0x000830AC
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

			// Token: 0x06001C7A RID: 7290 RVA: 0x00084EFC File Offset: 0x000830FC
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

			// Token: 0x06001C7B RID: 7291 RVA: 0x00084F70 File Offset: 0x00083170
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

			// Token: 0x04001E83 RID: 7811
			public readonly string name;

			// Token: 0x04001E84 RID: 7812
			public readonly bool isFolder;

			// Token: 0x04001E86 RID: 7814
			private readonly List<ViewablesCatalog.Node> _children = new List<ViewablesCatalog.Node>();

			// Token: 0x04001E87 RID: 7815
			public ReadOnlyCollection<ViewablesCatalog.Node> children;

			// Token: 0x04001E88 RID: 7816
			private string _fullName;

			// Token: 0x04001E89 RID: 7817
			private bool fullNameDirty = true;

			// Token: 0x04001E8A RID: 7818
			public Func<UserProfile, bool> shouldShowUnviewed;
		}
	}
}
