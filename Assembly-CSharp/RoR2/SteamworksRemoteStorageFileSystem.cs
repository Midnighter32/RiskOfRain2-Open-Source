using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using UnityEngine;
using Zio;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020004A4 RID: 1188
	public class SteamworksRemoteStorageFileSystem : FileSystem
	{
		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x0007C779 File Offset: 0x0007A979
		private static Client steamworksClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001AAD RID: 6829 RVA: 0x0007E0BA File Offset: 0x0007C2BA
		private static RemoteStorage remoteStorage
		{
			get
			{
				return SteamworksRemoteStorageFileSystem.steamworksClient.RemoteStorage;
			}
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x0007E0C8 File Offset: 0x0007C2C8
		public SteamworksRemoteStorageFileSystem()
		{
			this.pathToNodeMap[UPath.Root] = this.rootNode;
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x00004507 File Offset: 0x00002707
		protected override void CreateDirectoryImpl(UPath path)
		{
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x0000AE8B File Offset: 0x0000908B
		protected override bool DirectoryExistsImpl(UPath path)
		{
			return true;
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x0007E11E File Offset: 0x0007C31E
		protected override void MoveDirectoryImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override void DeleteDirectoryImpl(UPath path, bool isRecursive)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x0007E11E File Offset: 0x0007C31E
		protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override void ReplaceFileImpl(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0007E134 File Offset: 0x0007C334
		protected override long GetFileLengthImpl(UPath path)
		{
			int num = 0;
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				this.UpdateDirectories();
				SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
				num = ((fileNode != null) ? fileNode.GetLength() : 0);
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
			return (long)num;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x0007E17C File Offset: 0x0007C37C
		protected override bool FileExistsImpl(UPath path)
		{
			this.UpdateDirectories();
			return this.GetFileNode(path) != null;
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x0007E11E File Offset: 0x0007C31E
		protected override void MoveFileImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x0007E190 File Offset: 0x0007C390
		protected override void DeleteFileImpl(UPath path)
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				this.treeIsDirty = true;
				SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
				if (fileNode != null)
				{
					fileNode.Delete();
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x0007E1D4 File Offset: 0x0007C3D4
		protected override Stream OpenFileImpl(UPath path, FileMode mode, FileAccess access, FileShare share)
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			if (!path.IsAbsolute)
			{
				throw new ArgumentException(string.Format("'{0}' must be absolute. {0} = {1}", "path", path));
			}
			Stream result;
			try
			{
				bool flag = false;
				switch (mode)
				{
				case FileMode.CreateNew:
					flag = true;
					break;
				case FileMode.Create:
					flag = true;
					break;
				case FileMode.Append:
					throw new NotImplementedException();
				}
				flag &= (access == FileAccess.Write);
				if (flag)
				{
					this.treeIsDirty = true;
					result = SteamworksRemoteStorageFileSystem.remoteStorage.CreateFile(path.ToRelative().FullName).OpenWrite();
				}
				else if (access != FileAccess.Read)
				{
					if (access != FileAccess.Write)
					{
						throw new NotImplementedException();
					}
					SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
					result = ((fileNode != null) ? fileNode.OpenWrite() : null);
				}
				else
				{
					SteamworksRemoteStorageFileSystem.FileNode fileNode2 = this.GetFileNode(path);
					result = ((fileNode2 != null) ? fileNode2.OpenRead() : null);
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
			return result;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override FileAttributes GetAttributesImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x00004507 File Offset: 0x00002707
		protected override void SetAttributesImpl(UPath path, FileAttributes attributes)
		{
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override DateTime GetCreationTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x00004507 File Offset: 0x00002707
		protected override void SetCreationTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override DateTime GetLastAccessTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00004507 File Offset: 0x00002707
		protected override void SetLastAccessTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override DateTime GetLastWriteTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x00004507 File Offset: 0x00002707
		protected override void SetLastWriteTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x0007E2C0 File Offset: 0x0007C4C0
		private SteamworksRemoteStorageFileSystem.FileNode AddFileToTree(string path)
		{
			SteamworksRemoteStorageFileSystem.FileNode fileNode = new SteamworksRemoteStorageFileSystem.FileNode(path);
			this.AddNodeToTree(fileNode);
			return fileNode;
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x0007E2E4 File Offset: 0x0007C4E4
		private SteamworksRemoteStorageFileSystem.DirectoryNode AddDirectoryToTree(UPath path)
		{
			SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(path);
			this.AddNodeToTree(directoryNode);
			return directoryNode;
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x0007E300 File Offset: 0x0007C500
		private void AddNodeToTree(SteamworksRemoteStorageFileSystem.Node newNode)
		{
			UPath directory = newNode.path.GetDirectory();
			this.GetDirectoryNode(directory).AddChild(newNode);
			this.pathToNodeMap[newNode.path] = newNode;
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x0007E338 File Offset: 0x0007C538
		[CanBeNull]
		private SteamworksRemoteStorageFileSystem.DirectoryNode GetDirectoryNode(UPath directoryPath)
		{
			SteamworksRemoteStorageFileSystem.Node node;
			if (this.pathToNodeMap.TryGetValue(directoryPath, out node))
			{
				return node as SteamworksRemoteStorageFileSystem.DirectoryNode;
			}
			return this.AddDirectoryToTree(directoryPath);
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x0007E364 File Offset: 0x0007C564
		[CanBeNull]
		private SteamworksRemoteStorageFileSystem.FileNode GetFileNode(UPath filePath)
		{
			SteamworksRemoteStorageFileSystem.Node node;
			if (this.pathToNodeMap.TryGetValue(filePath, out node))
			{
				return node as SteamworksRemoteStorageFileSystem.FileNode;
			}
			return null;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x0007E38C File Offset: 0x0007C58C
		private void UpdateDirectories()
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				if (this.treeIsDirty)
				{
					this.treeIsDirty = false;
					IEnumerable<string> enumerable = from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
					select file.FileName;
					if (!enumerable.SequenceEqual(this.allFilePaths))
					{
						this.allFilePaths = enumerable.ToArray<string>();
						this.pathToNodeMap.Clear();
						this.pathToNodeMap[UPath.Root] = this.rootNode;
						this.rootNode.RemoveAllChildren();
						foreach (string path in this.allFilePaths)
						{
							this.AddFileToTree(path);
						}
					}
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x0007E464 File Offset: 0x0007C664
		private void AssertDirectory(SteamworksRemoteStorageFileSystem.Node node, UPath srcPath)
		{
			if (node is SteamworksRemoteStorageFileSystem.FileNode)
			{
				throw new IOException(string.Format("The source directory `{0}` is a file", srcPath));
			}
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x0007E486 File Offset: 0x0007C686
		protected override IEnumerable<UPath> EnumeratePathsImpl(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
		{
			this.UpdateDirectories();
			SearchPattern search = SearchPattern.Parse(ref path, ref searchPattern);
			List<UPath> foldersToProcess = new List<UPath>();
			foldersToProcess.Add(path);
			SortedSet<UPath> entries = new SortedSet<UPath>(UPath.DefaultComparerIgnoreCase);
			while (foldersToProcess.Count > 0)
			{
				UPath upath = foldersToProcess[0];
				foldersToProcess.RemoveAt(0);
				int num = 0;
				entries.Clear();
				SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
				try
				{
					SteamworksRemoteStorageFileSystem.Node directoryNode = this.GetDirectoryNode(upath);
					if (upath == path)
					{
						this.AssertDirectory(directoryNode, upath);
					}
					else if (!(directoryNode is SteamworksRemoteStorageFileSystem.DirectoryNode))
					{
						continue;
					}
					SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode2 = (SteamworksRemoteStorageFileSystem.DirectoryNode)directoryNode;
					for (int i = 0; i < directoryNode2.childCount; i++)
					{
						SteamworksRemoteStorageFileSystem.Node child = directoryNode2.GetChild(i);
						if (!(child is SteamworksRemoteStorageFileSystem.FileNode) || searchTarget != SearchTarget.Directory)
						{
							bool flag = search.Match(child.path);
							bool flag2 = searchOption == SearchOption.AllDirectories && child is SteamworksRemoteStorageFileSystem.DirectoryNode;
							bool flag3 = (child is SteamworksRemoteStorageFileSystem.FileNode && searchTarget != SearchTarget.Directory && flag) || (child is SteamworksRemoteStorageFileSystem.DirectoryNode && searchTarget != SearchTarget.File && flag);
							UPath item = upath / child.path;
							if (flag2)
							{
								foldersToProcess.Insert(num++, item);
							}
							if (flag3)
							{
								entries.Add(item);
							}
						}
					}
				}
				finally
				{
					SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
				}
				foreach (UPath upath2 in entries)
				{
					yield return upath2;
				}
				SortedSet<UPath>.Enumerator enumerator = default(SortedSet<UPath>.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x0007E4B3 File Offset: 0x0007C6B3
		private static void EnterFileSystemShared()
		{
			Monitor.Enter(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x0007E4BF File Offset: 0x0007C6BF
		private static void ExitFileSystemShared()
		{
			Monitor.Exit(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x0007E12C File Offset: 0x0007C32C
		protected override IFileSystemWatcher WatchImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x0007E4CB File Offset: 0x0007C6CB
		protected override string ConvertPathToInternalImpl(UPath path)
		{
			return path.FullName;
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x0007E4D4 File Offset: 0x0007C6D4
		protected override UPath ConvertPathFromInternalImpl(string innerPath)
		{
			return new UPath(innerPath);
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x0007E4DC File Offset: 0x0007C6DC
		[ConCommand(commandName = "steam_remote_storage_list_files", flags = ConVarFlags.None, helpText = "Lists the files currently being managed by Steamworks remote storage.")]
		private static void CCSteamRemoteStorageListFiles(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
			select string.Format("{0} .. {1}b", file.FileName, file.SizeInBytes)).ToArray<string>()));
		}

		// Token: 0x04001D9F RID: 7583
		private static readonly object globalLock = new object();

		// Token: 0x04001DA0 RID: 7584
		private string[] allFilePaths = Array.Empty<string>();

		// Token: 0x04001DA1 RID: 7585
		private readonly SteamworksRemoteStorageFileSystem.DirectoryNode rootNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(UPath.Root);

		// Token: 0x04001DA2 RID: 7586
		private readonly Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node> pathToNodeMap = new Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node>();

		// Token: 0x04001DA3 RID: 7587
		private bool treeIsDirty = true;

		// Token: 0x020004A5 RID: 1189
		private struct SteamworksRemoteStoragePath : IEquatable<SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath>
		{
			// Token: 0x06001AD1 RID: 6865 RVA: 0x0007E537 File Offset: 0x0007C737
			public SteamworksRemoteStoragePath(string path)
			{
				this.str = path;
			}

			// Token: 0x06001AD2 RID: 6866 RVA: 0x0007E540 File Offset: 0x0007C740
			public static implicit operator SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(string str)
			{
				return new SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(str);
			}

			// Token: 0x06001AD3 RID: 6867 RVA: 0x0007E548 File Offset: 0x0007C748
			public bool Equals(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other)
			{
				return string.Equals(this.str, other.str);
			}

			// Token: 0x06001AD4 RID: 6868 RVA: 0x0007E55C File Offset: 0x0007C75C
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath)
				{
					SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other = (SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06001AD5 RID: 6869 RVA: 0x0007E588 File Offset: 0x0007C788
			public override int GetHashCode()
			{
				if (this.str == null)
				{
					return 0;
				}
				return this.str.GetHashCode();
			}

			// Token: 0x04001DA4 RID: 7588
			public readonly string str;
		}

		// Token: 0x020004A6 RID: 1190
		private class Node
		{
			// Token: 0x06001AD6 RID: 6870 RVA: 0x0007E59F File Offset: 0x0007C79F
			public Node(UPath path)
			{
				this.path = path.ToAbsolute();
			}

			// Token: 0x04001DA5 RID: 7589
			public readonly UPath path;

			// Token: 0x04001DA6 RID: 7590
			public SteamworksRemoteStorageFileSystem.Node parent;
		}

		// Token: 0x020004A7 RID: 1191
		private class FileNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x06001AD7 RID: 6871 RVA: 0x0007E5B3 File Offset: 0x0007C7B3
			public FileNode(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath) : base(steamworksRemoteStoragePath.str)
			{
				this.steamworksRemoteStoragePath = steamworksRemoteStoragePath;
			}

			// Token: 0x17000279 RID: 633
			// (get) Token: 0x06001AD8 RID: 6872 RVA: 0x0007E5CD File Offset: 0x0007C7CD
			private RemoteFile file
			{
				get
				{
					return SteamworksRemoteStorageFileSystem.remoteStorage.OpenFile(this.steamworksRemoteStoragePath.str);
				}
			}

			// Token: 0x06001AD9 RID: 6873 RVA: 0x0007E5E4 File Offset: 0x0007C7E4
			public int GetLength()
			{
				return this.file.SizeInBytes;
			}

			// Token: 0x06001ADA RID: 6874 RVA: 0x0007E5F1 File Offset: 0x0007C7F1
			public Stream OpenWrite()
			{
				return this.file.OpenWrite();
			}

			// Token: 0x06001ADB RID: 6875 RVA: 0x0007E5FE File Offset: 0x0007C7FE
			public Stream OpenRead()
			{
				return this.file.OpenRead();
			}

			// Token: 0x06001ADC RID: 6876 RVA: 0x0007E60B File Offset: 0x0007C80B
			public void Delete()
			{
				this.file.Delete();
			}

			// Token: 0x04001DA7 RID: 7591
			public readonly SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath;
		}

		// Token: 0x020004A8 RID: 1192
		private class DirectoryNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x1700027A RID: 634
			// (get) Token: 0x06001ADD RID: 6877 RVA: 0x0007E619 File Offset: 0x0007C819
			// (set) Token: 0x06001ADE RID: 6878 RVA: 0x0007E621 File Offset: 0x0007C821
			public int childCount { get; private set; }

			// Token: 0x06001ADF RID: 6879 RVA: 0x0007E62A File Offset: 0x0007C82A
			public SteamworksRemoteStorageFileSystem.Node GetChild(int i)
			{
				return this.childNodes[i];
			}

			// Token: 0x06001AE0 RID: 6880 RVA: 0x0007E634 File Offset: 0x0007C834
			public void AddChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int childCount = this.childCount + 1;
				this.childCount = childCount;
				if (this.childCount > this.childNodes.Length)
				{
					Array.Resize<SteamworksRemoteStorageFileSystem.Node>(ref this.childNodes, this.childCount);
				}
				this.childNodes[this.childCount - 1] = node;
				node.parent = this;
			}

			// Token: 0x06001AE1 RID: 6881 RVA: 0x0007E68C File Offset: 0x0007C88C
			public void RemoveChildAt(int i)
			{
				if (this.childCount > 0)
				{
					this.childNodes[i].parent = null;
				}
				int num = this.childCount - 1;
				while (i < num)
				{
					this.childNodes[i] = this.childNodes[i + 1];
					i++;
				}
				if (this.childCount > 0)
				{
					this.childNodes[this.childCount - 1] = null;
				}
				int childCount = this.childCount - 1;
				this.childCount = childCount;
			}

			// Token: 0x06001AE2 RID: 6882 RVA: 0x0007E700 File Offset: 0x0007C900
			public void RemoveChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int num = Array.IndexOf<SteamworksRemoteStorageFileSystem.Node>(this.childNodes, node);
				if (num >= 0)
				{
					this.RemoveChildAt(num);
				}
			}

			// Token: 0x06001AE3 RID: 6883 RVA: 0x0007E728 File Offset: 0x0007C928
			public void RemoveAllChildren()
			{
				for (int i = 0; i < this.childCount; i++)
				{
					this.childNodes[i].parent = null;
					this.childNodes[i] = null;
				}
				this.childCount = 0;
			}

			// Token: 0x06001AE4 RID: 6884 RVA: 0x0007E764 File Offset: 0x0007C964
			public DirectoryNode(UPath path) : base(path)
			{
			}

			// Token: 0x04001DA8 RID: 7592
			private SteamworksRemoteStorageFileSystem.Node[] childNodes = Array.Empty<SteamworksRemoteStorageFileSystem.Node>();
		}
	}
}
