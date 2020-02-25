using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000581 RID: 1409
	[RequireComponent(typeof(RectTransform))]
	public class AllyCardManager : MonoBehaviour, ILayoutGroup, ILayoutController
	{
		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06002191 RID: 8593 RVA: 0x000912FE File Offset: 0x0008F4FE
		// (set) Token: 0x06002192 RID: 8594 RVA: 0x0009130B File Offset: 0x0008F50B
		public GameObject sourceGameObject
		{
			get
			{
				return this.currentSource.gameObject;
			}
			set
			{
				if (this.currentSource.gameObject == value)
				{
					return;
				}
				this.currentSource = new AllyCardManager.SourceInfo(value);
				this.OnSourceChanged();
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x00091330 File Offset: 0x0008F530
		private void OnCardCreated(int index, AllyCardController element)
		{
			Vector2 vector = element.rectTransform.anchorMin;
			vector.x = 0f;
			vector.y = 1f;
			element.rectTransform.anchorMin = vector;
			vector = element.rectTransform.anchorMax;
			vector.x = 1f;
			vector.y = 1f;
			element.rectTransform.anchorMax = vector;
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x0009139D File Offset: 0x0008F59D
		private void OnSourceChanged()
		{
			this.needsRefresh = true;
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x000913A8 File Offset: 0x0008F5A8
		private TeamIndex FindTargetTeam()
		{
			TeamIndex result = TeamIndex.None;
			TeamComponent teamComponent = this.currentSource.teamComponent;
			if (teamComponent)
			{
				result = teamComponent.teamIndex;
			}
			return result;
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x000913D3 File Offset: 0x0008F5D3
		private void SetCharacterData(AllyCardManager.CharacterDataSet newCharacterData)
		{
			if (newCharacterData.Equals(this.currentCharacterData))
			{
				return;
			}
			this.currentCharacterData.CopyFrom(newCharacterData);
			this.BuildFromCharacterData(this.currentCharacterData);
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x000913FC File Offset: 0x0008F5FC
		private void PopulateCharacterDataSet(AllyCardManager.CharacterDataSet characterDataSet)
		{
			TeamIndex teamIndex = this.FindTargetTeam();
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CharacterMaster characterMaster = readOnlyInstancesList[i];
				if (characterMaster.teamIndex == teamIndex)
				{
					CharacterBody body = characterMaster.GetBody();
					if ((!body || !body.teamComponent || !body.teamComponent.hideAllyCardDisplay) && (!characterMaster.playerCharacterMasterController || characterMaster.playerCharacterMasterController.networkUser) && this.currentSource.master != characterMaster)
					{
						AllyCardManager.CharacterData characterData = new AllyCardManager.CharacterData(characterMaster);
						characterDataSet.Add(ref characterData);
					}
				}
			}
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x000914A8 File Offset: 0x0008F6A8
		private void BuildFromCharacterData(AllyCardManager.CharacterDataSet characterDataSet)
		{
			if (characterDataSet.count < this.displayElementCount)
			{
				Array.Clear(this.displayElements, characterDataSet.count, this.displayElementCount - characterDataSet.count);
			}
			this.displayElementCount = characterDataSet.count;
			HGArrayUtilities.EnsureCapacity<AllyCardManager.DisplayElement>(ref this.displayElements, this.displayElementCount);
			int i = 0;
			int count = characterDataSet.count;
			while (i < count)
			{
				ref AllyCardManager.CharacterData ptr = ref characterDataSet[i];
				this.displayElements[i] = new AllyCardManager.DisplayElement
				{
					master = ptr.master,
					priority = -1
				};
				i++;
			}
			int num = 0;
			int j = 0;
			int count2 = characterDataSet.count;
			while (j < count2)
			{
				if (characterDataSet[j].isPlayer)
				{
					this.displayElements[j].priority = num;
					num += 2;
				}
				j++;
			}
			int k = 0;
			int count3 = characterDataSet.count;
			while (k < count3)
			{
				if (!characterDataSet[k].isMinion)
				{
					ref AllyCardManager.DisplayElement ptr2 = ref this.displayElements[k];
					if (ptr2.priority == -1)
					{
						ptr2.priority = num;
						num += 2;
					}
				}
				k++;
			}
			int l = 0;
			int count4 = characterDataSet.count;
			while (l < count4)
			{
				ref AllyCardManager.CharacterData ptr3 = ref characterDataSet[l];
				if (ptr3.isMinion)
				{
					ref AllyCardManager.DisplayElement ptr4 = ref this.displayElements[l];
					if (ptr4.priority == -1)
					{
						int num2 = this.<BuildFromCharacterData>g__FindIndexForMaster|22_0(ptr3.leaderMaster);
						if (num2 != -1)
						{
							ptr4.priority = this.displayElements[num2].priority + 1;
							ptr4.shouldIndent = true;
						}
					}
				}
				l++;
			}
			int m = 0;
			int count5 = characterDataSet.count;
			while (m < count5)
			{
				ref AllyCardManager.DisplayElement ptr5 = ref this.displayElements[m];
				if (ptr5.priority == -1)
				{
					ptr5.priority = num;
					num += 2;
				}
				m++;
			}
			AllyCardManager.DisplayElement[] array = this.displayElements;
			int index = 0;
			IComparer<AllyCardManager.DisplayElement> instance = AllyCardManager.DisplayElementComparer.instance;
			Array.Sort<AllyCardManager.DisplayElement>(array, index, this.displayElementCount, instance);
			this.cardAllocator.AllocateElements(this.displayElementCount);
			for (int n = 0; n < this.displayElementCount; n++)
			{
				ref AllyCardManager.DisplayElement ptr6 = ref this.displayElements[n];
				AllyCardController allyCardController = this.cardAllocator.elements[n];
				allyCardController.sourceMaster = ptr6.master;
				allyCardController.shouldIndent = ptr6.shouldIndent;
			}
			HGArrayUtilities.Clear<AllyCardManager.DisplayElement>(this.displayElements, ref this.displayElementCount);
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00091718 File Offset: 0x0008F918
		private void Awake()
		{
			this.cardAllocator = new UIElementAllocator<AllyCardController>((RectTransform)base.transform, Resources.Load<GameObject>("Prefabs/UI/AllyCard"));
			this.rectTransform = (RectTransform)base.transform;
			UIElementAllocator<AllyCardController> uielementAllocator = this.cardAllocator;
			uielementAllocator.onCreateElement = (UIElementAllocator<AllyCardController>.ElementOperationDelegate)Delegate.Combine(uielementAllocator.onCreateElement, new UIElementAllocator<AllyCardController>.ElementOperationDelegate(this.OnCardCreated));
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x00091780 File Offset: 0x0008F980
		private void FixedUpdate()
		{
			AllyCardManager.CharacterDataSet characterDataSet = AllyCardManager.CharacterDataSetPool.Request();
			this.PopulateCharacterDataSet(characterDataSet);
			this.SetCharacterData(characterDataSet);
			AllyCardManager.CharacterDataSetPool.Return(ref characterDataSet);
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000917A8 File Offset: 0x0008F9A8
		private void OnEnable()
		{
			this.needsRefresh = true;
			this.currentCharacterData = AllyCardManager.CharacterDataSetPool.Request();
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x000917BC File Offset: 0x0008F9BC
		private void OnDisable()
		{
			AllyCardManager.CharacterDataSetPool.Return(ref this.currentCharacterData);
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x000917CC File Offset: 0x0008F9CC
		public void SetLayoutHorizontal()
		{
			ReadOnlyCollection<AllyCardController> elements = this.cardAllocator.elements;
			int i = 0;
			int count = elements.Count;
			while (i < count)
			{
				AllyCardController allyCardController = elements[i];
				RectTransform rectTransform = allyCardController.rectTransform;
				Vector2 vector = rectTransform.offsetMin;
				vector.x = (allyCardController.shouldIndent ? this.indentWidth : 0f);
				rectTransform.offsetMin = vector;
				vector = rectTransform.offsetMax;
				vector.x = 0f;
				rectTransform.offsetMax = vector;
				i++;
			}
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x00091850 File Offset: 0x0008FA50
		public void SetLayoutVertical()
		{
			ReadOnlyCollection<AllyCardController> elements = this.cardAllocator.elements;
			float b = this.rectTransform.rect.height / (float)elements.Count;
			float num = 0f;
			int i = 0;
			int count = elements.Count;
			while (i < count)
			{
				AllyCardController allyCardController = elements[i];
				RectTransform rectTransform = allyCardController.rectTransform;
				float preferredHeight = allyCardController.layoutElement.preferredHeight;
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.y = num;
				rectTransform.anchoredPosition = anchoredPosition;
				num -= Mathf.Min(preferredHeight, b);
				i++;
			}
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x00091910 File Offset: 0x0008FB10
		[CompilerGenerated]
		private int <BuildFromCharacterData>g__FindIndexForMaster|22_0(CharacterMaster master)
		{
			for (int i = 0; i < this.displayElementCount; i++)
			{
				if (master == this.displayElements[i].master)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04001EF8 RID: 7928
		public float indentWidth = 16f;

		// Token: 0x04001EF9 RID: 7929
		private AllyCardManager.SourceInfo currentSource;

		// Token: 0x04001EFA RID: 7930
		private UIElementAllocator<AllyCardController> cardAllocator;

		// Token: 0x04001EFB RID: 7931
		private RectTransform rectTransform;

		// Token: 0x04001EFC RID: 7932
		private bool needsRefresh;

		// Token: 0x04001EFD RID: 7933
		private AllyCardManager.DisplayElement[] displayElements = Array.Empty<AllyCardManager.DisplayElement>();

		// Token: 0x04001EFE RID: 7934
		private int displayElementCount;

		// Token: 0x04001EFF RID: 7935
		private AllyCardManager.CharacterDataSet currentCharacterData;

		// Token: 0x02000582 RID: 1410
		private struct SourceInfo
		{
			// Token: 0x060021A1 RID: 8609 RVA: 0x00091948 File Offset: 0x0008FB48
			public SourceInfo(GameObject gameObject)
			{
				bool flag = gameObject;
				this.gameObject = gameObject;
				this.teamComponent = (flag ? gameObject.GetComponent<TeamComponent>() : null);
				CharacterBody characterBody = flag ? gameObject.GetComponent<CharacterBody>() : null;
				this.master = (characterBody ? characterBody.master : null);
			}

			// Token: 0x04001F00 RID: 7936
			public readonly GameObject gameObject;

			// Token: 0x04001F01 RID: 7937
			public readonly TeamComponent teamComponent;

			// Token: 0x04001F02 RID: 7938
			public readonly CharacterMaster master;
		}

		// Token: 0x02000583 RID: 1411
		private struct DisplayElement
		{
			// Token: 0x04001F03 RID: 7939
			public CharacterMaster master;

			// Token: 0x04001F04 RID: 7940
			public bool shouldIndent;

			// Token: 0x04001F05 RID: 7941
			public int priority;
		}

		// Token: 0x02000584 RID: 1412
		private class DisplayElementComparer : IComparer<AllyCardManager.DisplayElement>
		{
			// Token: 0x060021A2 RID: 8610 RVA: 0x0009199C File Offset: 0x0008FB9C
			public int Compare(AllyCardManager.DisplayElement a, AllyCardManager.DisplayElement b)
			{
				int num = a.priority - b.priority;
				if (num == 0)
				{
					num = (int)(a.master.netId.Value - b.master.netId.Value);
				}
				return num;
			}

			// Token: 0x060021A3 RID: 8611 RVA: 0x00004320 File Offset: 0x00002520
			private DisplayElementComparer()
			{
			}

			// Token: 0x04001F06 RID: 7942
			public static AllyCardManager.DisplayElementComparer instance = new AllyCardManager.DisplayElementComparer();
		}

		// Token: 0x02000585 RID: 1413
		private struct CharacterData
		{
			// Token: 0x060021A5 RID: 8613 RVA: 0x000919F0 File Offset: 0x0008FBF0
			public CharacterData(CharacterMaster master)
			{
				this.master = master;
				this.leaderMaster = master.minionOwnership.ownerMaster;
				this.isMinion = this.leaderMaster;
				this.isPlayer = master.playerCharacterMasterController;
				this.masterInstanceId = master.gameObject.GetInstanceID();
				this.leaderMasterInstanceId = (this.isMinion ? this.leaderMaster.gameObject.GetInstanceID() : 0);
			}

			// Token: 0x060021A6 RID: 8614 RVA: 0x00091A69 File Offset: 0x0008FC69
			public bool Equals(in AllyCardManager.CharacterData other)
			{
				return this.masterInstanceId == other.masterInstanceId && this.leaderMasterInstanceId == other.leaderMasterInstanceId && this.isMinion == other.isMinion && this.isPlayer == other.isPlayer;
			}

			// Token: 0x04001F07 RID: 7943
			public readonly CharacterMaster master;

			// Token: 0x04001F08 RID: 7944
			public readonly CharacterMaster leaderMaster;

			// Token: 0x04001F09 RID: 7945
			private readonly int masterInstanceId;

			// Token: 0x04001F0A RID: 7946
			private readonly int leaderMasterInstanceId;

			// Token: 0x04001F0B RID: 7947
			public readonly bool isMinion;

			// Token: 0x04001F0C RID: 7948
			public readonly bool isPlayer;
		}

		// Token: 0x02000586 RID: 1414
		private class CharacterDataSet
		{
			// Token: 0x17000392 RID: 914
			// (get) Token: 0x060021A7 RID: 8615 RVA: 0x00091AA5 File Offset: 0x0008FCA5
			public int count
			{
				get
				{
					return this._count;
				}
			}

			// Token: 0x060021A8 RID: 8616 RVA: 0x00091AB0 File Offset: 0x0008FCB0
			public bool Equals(AllyCardManager.CharacterDataSet other)
			{
				if (other == null)
				{
					return false;
				}
				if (this._count != other._count)
				{
					return false;
				}
				for (int i = 0; i < this._count; i++)
				{
					if (!this.buffer[i].Equals(other.buffer[i]))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x060021A9 RID: 8617 RVA: 0x00091B05 File Offset: 0x0008FD05
			public void Clear()
			{
				Array.Clear(this.buffer, 0, this._count);
				this._count = 0;
			}

			// Token: 0x060021AA RID: 8618 RVA: 0x00091B20 File Offset: 0x0008FD20
			public void Add(ref AllyCardManager.CharacterData element)
			{
				HGArrayUtilities.ArrayAppend<AllyCardManager.CharacterData>(ref this.buffer, ref this._count, ref element);
			}

			// Token: 0x17000393 RID: 915
			public AllyCardManager.CharacterData this[int i]
			{
				get
				{
					return ref this.buffer[i];
				}
			}

			// Token: 0x060021AC RID: 8620 RVA: 0x00091B44 File Offset: 0x0008FD44
			public void CopyFrom(AllyCardManager.CharacterDataSet src)
			{
				int num = this._count - src._count;
				if (num > 0)
				{
					Array.Clear(this.buffer, src._count, num);
				}
				HGArrayUtilities.EnsureCapacity<AllyCardManager.CharacterData>(ref this.buffer, src.buffer.Length);
				this._count = src.count;
				Array.Copy(src.buffer, this.buffer, this._count);
			}

			// Token: 0x04001F0D RID: 7949
			private AllyCardManager.CharacterData[] buffer = new AllyCardManager.CharacterData[128];

			// Token: 0x04001F0E RID: 7950
			private int _count;
		}

		// Token: 0x02000587 RID: 1415
		private static class CharacterDataSetPool
		{
			// Token: 0x060021AE RID: 8622 RVA: 0x00091BC3 File Offset: 0x0008FDC3
			public static AllyCardManager.CharacterDataSet Request()
			{
				if (AllyCardManager.CharacterDataSetPool.pool.Count == 0)
				{
					return new AllyCardManager.CharacterDataSet();
				}
				return AllyCardManager.CharacterDataSetPool.pool.Pop();
			}

			// Token: 0x060021AF RID: 8623 RVA: 0x00091BE1 File Offset: 0x0008FDE1
			public static void Return(ref AllyCardManager.CharacterDataSet characterDataSet)
			{
				characterDataSet.Clear();
				AllyCardManager.CharacterDataSetPool.pool.Push(characterDataSet);
			}

			// Token: 0x04001F0F RID: 7951
			private static readonly Stack<AllyCardManager.CharacterDataSet> pool = new Stack<AllyCardManager.CharacterDataSet>();
		}
	}
}
