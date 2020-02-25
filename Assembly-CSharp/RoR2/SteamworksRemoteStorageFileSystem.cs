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
	// Token: 0x02000435 RID: 1077
	public class SteamworksRemoteStorageFileSystem : FileSystem
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x0006E17A File Offset: 0x0006C37A
		private static Client steamworksClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001A13 RID: 6675 RVA: 0x0006FB3E File Offset: 0x0006DD3E
		private static RemoteStorage remoteStorage
		{
			get
			{
				return SteamworksRemoteStorageFileSystem.steamworksClient.RemoteStorage;
			}
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0006FB4C File Offset: 0x0006DD4C
		public SteamworksRemoteStorageFileSystem()
		{
			this.pathToNodeMap[UPath.Root] = this.rootNode;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void CreateDirectoryImpl(UPath path)
		{
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool DirectoryExistsImpl(UPath path)
		{
			return true;
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0006FBA2 File Offset: 0x0006DDA2
		protected override void MoveDirectoryImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override void DeleteDirectoryImpl(UPath path, bool isRecursive)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0006FBA2 File Offset: 0x0006DDA2
		protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override void ReplaceFileImpl(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0006FBB8 File Offset: 0x0006DDB8
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

		// Token: 0x06001A1C RID: 6684 RVA: 0x0006FC00 File Offset: 0x0006DE00
		protected override bool FileExistsImpl(UPath path)
		{
			this.UpdateDirectories();
			return this.GetFileNode(path) != null;
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0006FBA2 File Offset: 0x0006DDA2
		protected override void MoveFileImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x0006FC14 File Offset: 0x0006DE14
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

		// Token: 0x06001A1F RID: 6687 RVA: 0x0006FC58 File Offset: 0x0006DE58
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

		// Token: 0x06001A20 RID: 6688 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override FileAttributes GetAttributesImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void SetAttributesImpl(UPath path, FileAttributes attributes)
		{
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override DateTime GetCreationTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void SetCreationTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override DateTime GetLastAccessTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void SetLastAccessTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override DateTime GetLastWriteTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void SetLastWriteTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x0006FD44 File Offset: 0x0006DF44
		private SteamworksRemoteStorageFileSystem.FileNode AddFileToTree(string path)
		{
			SteamworksRemoteStorageFileSystem.FileNode fileNode = new SteamworksRemoteStorageFileSystem.FileNode(path);
			this.AddNodeToTree(fileNode);
			return fileNode;
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0006FD68 File Offset: 0x0006DF68
		private SteamworksRemoteStorageFileSystem.DirectoryNode AddDirectoryToTree(UPath path)
		{
			SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(path);
			this.AddNodeToTree(directoryNode);
			return directoryNode;
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0006FD84 File Offset: 0x0006DF84
		private void AddNodeToTree(SteamworksRemoteStorageFileSystem.Node newNode)
		{
			UPath directory = newNode.path.GetDirectory();
			this.GetDirectoryNode(directory).AddChild(newNode);
			this.pathToNodeMap[newNode.path] = newNode;
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0006FDBC File Offset: 0x0006DFBC
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

		// Token: 0x06001A2C RID: 6700 RVA: 0x0006FDE8 File Offset: 0x0006DFE8
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

		// Token: 0x06001A2D RID: 6701 RVA: 0x0006FE10 File Offset: 0x0006E010
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

		// Token: 0x06001A2E RID: 6702 RVA: 0x0006FEE8 File Offset: 0x0006E0E8
		private void AssertDirectory(SteamworksRemoteStorageFileSystem.Node node, UPath srcPath)
		{
			if (node is SteamworksRemoteStorageFileSystem.FileNode)
			{
				throw new IOException(string.Format("The source directory `{0}` is a file", srcPath));
			}
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x0006FF0A File Offset: 0x0006E10A
		protected override IEnumerable<UPath> EnumeratePathsImpl(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
		{
			this.UpdateDirectories();
			SearchPattern search = SearchPattern.Parse(ref path, ref searchPattern);
			List<UPath> foldersToProcess = new List<UPath>
			{
				path
			};
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

		// Token: 0x06001A30 RID: 6704 RVA: 0x0006FF37 File Offset: 0x0006E137
		private static void EnterFileSystemShared()
		{
			Monitor.Enter(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x0006FF43 File Offset: 0x0006E143
		private static void ExitFileSystemShared()
		{
			Monitor.Exit(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		protected override IFileSystemWatcher WatchImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0006FF4F File Offset: 0x0006E14F
		protected override string ConvertPathToInternalImpl(UPath path)
		{
			return path.FullName;
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0006FF58 File Offset: 0x0006E158
		protected override UPath ConvertPathFromInternalImpl(string innerPath)
		{
			return new UPath(innerPath);
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0006FF60 File Offset: 0x0006E160
		[ConCommand(commandName = "steam_remote_storage_list_files", flags = ConVarFlags.None, helpText = "Lists the files currently being managed by Steamworks remote storage.")]
		private static void CCSteamRemoteStorageListFiles(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
			select string.Format("{0} .. {1}b", file.FileName, file.SizeInBytes)).ToArray<string>()));
		}

		// Token: 0x04001805 RID: 6149
		private static readonly object globalLock = new object();

		// Token: 0x04001806 RID: 6150
		private string[] allFilePaths = Array.Empty<string>();

		// Token: 0x04001807 RID: 6151
		private readonly SteamworksRemoteStorageFileSystem.DirectoryNode rootNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(UPath.Root);

		// Token: 0x04001808 RID: 6152
		private readonly Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node> pathToNodeMap = new Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node>();

		// Token: 0x04001809 RID: 6153
		private bool treeIsDirty = true;

		// Token: 0x02000436 RID: 1078
		private struct SteamworksRemoteStoragePath : IEquatable<SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath>
		{
			// Token: 0x06001A37 RID: 6711 RVA: 0x0006FFBB File Offset: 0x0006E1BB
			public SteamworksRemoteStoragePath(string path)
			{
				this.str = path;
			}

			// Token: 0x06001A38 RID: 6712 RVA: 0x0006FFC4 File Offset: 0x0006E1C4
			public static implicit operator SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(string str)
			{
				return new SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(str);
			}

			// Token: 0x06001A39 RID: 6713 RVA: 0x0006FFCC File Offset: 0x0006E1CC
			public bool Equals(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other)
			{
				return string.Equals(this.str, other.str);
			}

			// Token: 0x06001A3A RID: 6714 RVA: 0x0006FFE0 File Offset: 0x0006E1E0
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

			// Token: 0x06001A3B RID: 6715 RVA: 0x0007000C File Offset: 0x0006E20C
			public override int GetHashCode()
			{
				if (this.str == null)
				{
					return 0;
				}
				return this.str.GetHashCode();
			}

			// Token: 0x0400180A RID: 6154
			public readonly string str;
		}

		// Token: 0x02000437 RID: 1079
		private class Node
		{
			// Token: 0x06001A3C RID: 6716 RVA: 0x00070023 File Offset: 0x0006E223
			public Node(UPath path)
			{
				this.path = path.ToAbsolute();
			}

			// Token: 0x0400180B RID: 6155
			public readonly UPath path;

			// Token: 0x0400180C RID: 6156
			public SteamworksRemoteStorageFileSystem.Node parent;
		}

		// Token: 0x02000438 RID: 1080
		private class FileNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x06001A3D RID: 6717 RVA: 0x00070037 File Offset: 0x0006E237
			public FileNode(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath) : base(steamworksRemoteStoragePath.str)
			{
				this.steamworksRemoteStoragePath = steamworksRemoteStoragePath;
			}

			// Token: 0x17000300 RID: 768
			// (get) Token: 0x06001A3E RID: 6718 RVA: 0x00070051 File Offset: 0x0006E251
			private RemoteFile file
			{
				get
				{
					return SteamworksRemoteStorageFileSystem.remoteStorage.OpenFile(this.steamworksRemoteStoragePath.str);
				}
			}

			// Token: 0x06001A3F RID: 6719 RVA: 0x00070068 File Offset: 0x0006E268
			public int GetLength()
			{
				return this.file.SizeInBytes;
			}

			// Token: 0x06001A40 RID: 6720 RVA: 0x00070075 File Offset: 0x0006E275
			public Stream OpenWrite()
			{
				return this.file.OpenWrite();
			}

			// Token: 0x06001A41 RID: 6721 RVA: 0x00070082 File Offset: 0x0006E282
			public Stream OpenRead()
			{
				return this.file.OpenRead();
			}

			// Token: 0x06001A42 RID: 6722 RVA: 0x0007008F File Offset: 0x0006E28F
			public void Delete()
			{
				this.file.Delete();
			}

			// Token: 0x0400180D RID: 6157
			public readonly SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath;
		}

		// Token: 0x02000439 RID: 1081
		private class DirectoryNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x17000301 RID: 769
			// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0007009D File Offset: 0x0006E29D
			// (set) Token: 0x06001A44 RID: 6724 RVA: 0x000700A5 File Offset: 0x0006E2A5
			public int childCount { get; private set; }

			// Token: 0x06001A45 RID: 6725 RVA: 0x000700AE File Offset: 0x0006E2AE
			public SteamworksRemoteStorageFileSystem.Node GetChild(int i)
			{
				return this.childNodes[i];
			}

			// Token: 0x06001A46 RID: 6726 RVA: 0x000700B8 File Offset: 0x0006E2B8
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

			// Token: 0x06001A47 RID: 6727 RVA: 0x00070110 File Offset: 0x0006E310
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

			// Token: 0x06001A48 RID: 6728 RVA: 0x00070184 File Offset: 0x0006E384
			public void RemoveChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int num = Array.IndexOf<SteamworksRemoteStorageFileSystem.Node>(this.childNodes, node);
				if (num >= 0)
				{
					this.RemoveChildAt(num);
				}
			}

			// Token: 0x06001A49 RID: 6729 RVA: 0x000701AC File Offset: 0x0006E3AC
			public void RemoveAllChildren()
			{
				for (int i = 0; i < this.childCount; i++)
				{
					this.childNodes[i].parent = null;
					this.childNodes[i] = null;
				}
				this.childCount = 0;
			}

			// Token: 0x06001A4A RID: 6730 RVA: 0x000701E8 File Offset: 0x0006E3E8
			public DirectoryNode(UPath path) : base(path)
			{
			}

			// Token: 0x0400180E RID: 6158
			private SteamworksRemoteStorageFileSystem.Node[] childNodes = Array.Empty<SteamworksRemoteStorageFileSystem.Node>();
		}
	}
}
