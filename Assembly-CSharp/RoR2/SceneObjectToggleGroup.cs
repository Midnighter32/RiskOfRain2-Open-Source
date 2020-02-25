using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200031E RID: 798
	public class SceneObjectToggleGroup : NetworkBehaviour
	{
		// Token: 0x060012C4 RID: 4804 RVA: 0x000508B3 File Offset: 0x0004EAB3
		static SceneObjectToggleGroup()
		{
			GameNetworkManager.onServerSceneChangedGlobal += SceneObjectToggleGroup.OnServerSceneChanged;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x000508D0 File Offset: 0x0004EAD0
		private static void OnServerSceneChanged(string sceneName)
		{
			while (SceneObjectToggleGroup.activationsQueue.Count > 0)
			{
				SceneObjectToggleGroup sceneObjectToggleGroup = SceneObjectToggleGroup.activationsQueue.Dequeue();
				if (sceneObjectToggleGroup)
				{
					sceneObjectToggleGroup.ApplyActivations();
				}
			}
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00050908 File Offset: 0x0004EB08
		private void Awake()
		{
			SceneObjectToggleGroup.activationsQueue.Enqueue(this);
			int num = 0;
			for (int i = 0; i < this.toggleGroups.Length; i++)
			{
				num += this.toggleGroups[i].objects.Length;
			}
			this.allToggleableObjects = new GameObject[num];
			this.activations = new bool[num];
			this.internalToggleGroups = new SceneObjectToggleGroup.ToggleGroupRange[this.toggleGroups.Length];
			int start = 0;
			for (int j = 0; j < this.toggleGroups.Length; j++)
			{
				GameObject[] objects = this.toggleGroups[j].objects;
				SceneObjectToggleGroup.ToggleGroupRange toggleGroupRange = default(SceneObjectToggleGroup.ToggleGroupRange);
				toggleGroupRange.start = start;
				toggleGroupRange.count = objects.Length;
				toggleGroupRange.minEnabled = this.toggleGroups[j].minEnabled;
				toggleGroupRange.maxEnabled = this.toggleGroups[j].maxEnabled;
				this.internalToggleGroups[j] = toggleGroupRange;
				foreach (GameObject gameObject in objects)
				{
					this.allToggleableObjects[start++] = gameObject;
				}
			}
			if (NetworkServer.active)
			{
				this.Generate();
			}
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x00050A36 File Offset: 0x0004EC36
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!NetworkServer.active)
			{
				this.ApplyActivations();
			}
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x00050A4C File Offset: 0x0004EC4C
		[Server]
		private void Generate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SceneObjectToggleGroup::Generate()' called on client");
				return;
			}
			for (int i = 0; i < this.internalToggleGroups.Length; i++)
			{
				SceneObjectToggleGroup.ToggleGroupRange toggleGroupRange = this.internalToggleGroups[i];
				int num = Run.instance.stageRng.RangeInt(toggleGroupRange.minEnabled, toggleGroupRange.maxEnabled + 1);
				List<int> list = SceneObjectToggleGroup.<Generate>g__RangeList|12_0(toggleGroupRange.start, toggleGroupRange.count);
				Util.ShuffleList<int>(list, Run.instance.stageRng);
				for (int j = num - 1; j >= 0; j--)
				{
					this.activations[list[j]] = true;
					list.RemoveAt(j);
				}
				for (int k = 0; k < list.Count; k++)
				{
					this.activations[list[k]] = false;
				}
			}
			base.SetDirtyBit(1U);
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x00050B24 File Offset: 0x0004ED24
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = initialState ? 1U : base.syncVarDirtyBits;
			writer.Write((byte)num);
			if ((num & 1U) != 0U)
			{
				int num2 = 0;
				int num3 = (this.activations.Length - 1 >> 3) + 1;
				for (int i = 0; i < num3; i++)
				{
					byte b = 0;
					int num4 = 0;
					while (num4 < 8 && num2 < this.activations.Length)
					{
						if (this.activations[num2])
						{
							b |= (byte)(1 << num4);
						}
						num4++;
						num2++;
					}
					writer.Write(b);
				}
			}
			return !initialState && num > 0U;
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x00050BB4 File Offset: 0x0004EDB4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				int num = 0;
				int num2 = (this.activations.Length - 1 >> 3) + 1;
				for (int i = 0; i < num2; i++)
				{
					byte b = reader.ReadByte();
					int num3 = 0;
					while (num3 < 8 && num < this.activations.Length)
					{
						this.activations[num] = ((b & (byte)(1 << num3)) > 0);
						num3++;
						num++;
					}
				}
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00050C24 File Offset: 0x0004EE24
		private void ApplyActivations()
		{
			for (int i = 0; i < this.allToggleableObjects.Length; i++)
			{
				this.allToggleableObjects[i].SetActive(this.activations[i]);
			}
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00050C5C File Offset: 0x0004EE5C
		[CompilerGenerated]
		internal static List<int> <Generate>g__RangeList|12_0(int start, int count)
		{
			List<int> list = new List<int>(count);
			int i = start;
			int num = start + count;
			while (i < num)
			{
				list.Add(i);
				i++;
			}
			return list;
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0400119C RID: 4508
		public GameObjectToggleGroup[] toggleGroups;

		// Token: 0x0400119D RID: 4509
		private const byte enabledObjectsDirtyBit = 1;

		// Token: 0x0400119E RID: 4510
		private const byte initialStateMask = 1;

		// Token: 0x0400119F RID: 4511
		private static readonly Queue<SceneObjectToggleGroup> activationsQueue = new Queue<SceneObjectToggleGroup>();

		// Token: 0x040011A0 RID: 4512
		private GameObject[] allToggleableObjects;

		// Token: 0x040011A1 RID: 4513
		private bool[] activations;

		// Token: 0x040011A2 RID: 4514
		private SceneObjectToggleGroup.ToggleGroupRange[] internalToggleGroups;

		// Token: 0x0200031F RID: 799
		private struct ToggleGroupRange
		{
			// Token: 0x040011A3 RID: 4515
			public int start;

			// Token: 0x040011A4 RID: 4516
			public int count;

			// Token: 0x040011A5 RID: 4517
			public int minEnabled;

			// Token: 0x040011A6 RID: 4518
			public int maxEnabled;
		}
	}
}
