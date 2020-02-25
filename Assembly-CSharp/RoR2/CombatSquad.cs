using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001B3 RID: 435
	public class CombatSquad : NetworkBehaviour
	{
		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x00028691 File Offset: 0x00026891
		// (set) Token: 0x06000944 RID: 2372 RVA: 0x00028699 File Offset: 0x00026899
		public ReadOnlyCollection<CharacterMaster> readOnlyMembersList { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x000286A2 File Offset: 0x000268A2
		public int memberCount
		{
			get
			{
				return this.membersList.Count;
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x000286B0 File Offset: 0x000268B0
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.onDestroyCallbacksServer = new List<OnDestroyCallback>();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathCallback;
			}
			this.readOnlyMembersList = new ReadOnlyCollection<CharacterMaster>(this.membersList);
			this.awakeTime = Run.FixedTimeStamp.now;
			InstanceTracker.Add<CombatSquad>(this);
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00028704 File Offset: 0x00026904
		private void OnDestroy()
		{
			InstanceTracker.Remove<CombatSquad>(this);
			if (NetworkServer.active)
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathCallback;
			}
			for (int i = this.membersList.Count - 1; i >= 0; i--)
			{
				this.RemoveMemberAt(i);
			}
			this.onDestroyCallbacksServer = null;
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00028758 File Offset: 0x00026958
		[Server]
		public void AddMember(CharacterMaster memberMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CombatSquad::AddMember(RoR2.CharacterMaster)' called on client");
				return;
			}
			if (this.membersList.Count >= 255)
			{
				Debug.LogFormat("Cannot add character {0} to CombatGroup! Limit of {1} members already reached.", new object[]
				{
					memberMaster,
					byte.MaxValue
				});
				return;
			}
			this.membersList.Add(memberMaster);
			base.SetDirtyBit(1U);
			this.onDestroyCallbacksServer.Add(OnDestroyCallback.AddCallback(memberMaster.gameObject, new Action<OnDestroyCallback>(this.OnMemberDestroyedServer)));
			Action<CharacterMaster> action = this.onMemberAddedServer;
			if (action != null)
			{
				action(memberMaster);
			}
			Action<CharacterMaster> action2 = this.onMemberDiscovered;
			if (action2 == null)
			{
				return;
			}
			action2(memberMaster);
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000949 RID: 2377 RVA: 0x00028808 File Offset: 0x00026A08
		// (remove) Token: 0x0600094A RID: 2378 RVA: 0x00028840 File Offset: 0x00026A40
		public event Action onDefeatedServer;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600094B RID: 2379 RVA: 0x00028878 File Offset: 0x00026A78
		// (remove) Token: 0x0600094C RID: 2380 RVA: 0x000288B0 File Offset: 0x00026AB0
		public event Action<CharacterMaster, DamageReport> onMemberDeathServer;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600094D RID: 2381 RVA: 0x000288E8 File Offset: 0x00026AE8
		// (remove) Token: 0x0600094E RID: 2382 RVA: 0x00028920 File Offset: 0x00026B20
		public event Action<CharacterMaster> onMemberAddedServer;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600094F RID: 2383 RVA: 0x00028958 File Offset: 0x00026B58
		// (remove) Token: 0x06000950 RID: 2384 RVA: 0x00028990 File Offset: 0x00026B90
		public event Action<CharacterMaster> onMemberDiscovered;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000951 RID: 2385 RVA: 0x000289C8 File Offset: 0x00026BC8
		// (remove) Token: 0x06000952 RID: 2386 RVA: 0x00028A00 File Offset: 0x00026C00
		public event Action<CharacterMaster> onMemberLost;

		// Token: 0x06000953 RID: 2387 RVA: 0x00028A38 File Offset: 0x00026C38
		[Server]
		private void OnCharacterDeathCallback(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CombatSquad::OnCharacterDeathCallback(RoR2.DamageReport)' called on client");
				return;
			}
			DamageInfo damageInfo = damageReport.damageInfo;
			CharacterBody component = damageReport.victim.gameObject.GetComponent<CharacterBody>();
			if (!component)
			{
				return;
			}
			CharacterMaster master = component.master;
			if (!master)
			{
				return;
			}
			GameObject gameObject = master.gameObject;
			int num = this.membersList.IndexOf(master);
			if (num >= 0)
			{
				Action<CharacterMaster, DamageReport> action = this.onMemberDeathServer;
				if (action != null)
				{
					action(master, damageReport);
				}
				this.RemoveMemberAt(num);
				if (!this.defeatedServer && this.membersList.Count == 0)
				{
					this.defeatedServer = true;
					Action action2 = this.onDefeatedServer;
					if (action2 != null)
					{
						action2();
					}
					UnityEvent unityEvent = this.onDefeatedServerLogicEvent;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
			}
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00028AFC File Offset: 0x00026CFC
		[Server]
		private void RemoveMember(CharacterMaster memberMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CombatSquad::RemoveMember(RoR2.CharacterMaster)' called on client");
				return;
			}
			int num = this.membersList.IndexOf(memberMaster);
			if (num != -1)
			{
				this.RemoveMemberAt(num);
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00028B38 File Offset: 0x00026D38
		private void RemoveMemberAt(int memberIndex)
		{
			CharacterMaster obj = this.membersList[memberIndex];
			this.membersList.RemoveAt(memberIndex);
			if (this.onDestroyCallbacksServer != null)
			{
				this.onDestroyCallbacksServer.RemoveAt(memberIndex);
			}
			base.SetDirtyBit(1U);
			Action<CharacterMaster> action = this.onMemberLost;
			if (action == null)
			{
				return;
			}
			action(obj);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00028B8C File Offset: 0x00026D8C
		[Server]
		public void OnMemberDestroyedServer(OnDestroyCallback onDestroyCallback)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CombatSquad::OnMemberDestroyedServer(RoR2.OnDestroyCallback)' called on client");
				return;
			}
			if (onDestroyCallback)
			{
				GameObject gameObject = onDestroyCallback.gameObject;
				CharacterMaster characterMaster = gameObject ? gameObject.GetComponent<CharacterMaster>() : null;
				if (characterMaster)
				{
					this.membersList.Remove(characterMaster);
				}
			}
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00028BE4 File Offset: 0x00026DE4
		public bool ContainsMember(CharacterMaster master)
		{
			for (int i = 0; i < this.membersList.Count; i++)
			{
				if (this.membersList[i] == master)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00028C1C File Offset: 0x00026E1C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1U;
			}
			bool flag = (num & 1U) > 0U;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write((byte)this.membersList.Count);
				for (int i = 0; i < this.membersList.Count; i++)
				{
					CharacterMaster characterMaster = this.membersList[i];
					GameObject value = characterMaster ? characterMaster.gameObject : null;
					writer.Write(value);
				}
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00028C9C File Offset: 0x00026E9C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) > 0)
			{
				for (int i = 0; i < this.membersList.Count; i++)
				{
					CharacterMaster characterMaster = this.membersList[i];
					if (characterMaster)
					{
						Action<CharacterMaster> action = this.onMemberLost;
						if (action != null)
						{
							action(characterMaster);
						}
					}
				}
				this.membersList.Clear();
				byte b = reader.ReadByte();
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					GameObject gameObject = reader.ReadGameObject();
					CharacterMaster characterMaster2 = gameObject ? gameObject.GetComponent<CharacterMaster>() : null;
					this.membersList.Add(characterMaster2);
					if (characterMaster2)
					{
						Action<CharacterMaster> action2 = this.onMemberDiscovered;
						if (action2 != null)
						{
							action2(characterMaster2);
						}
					}
				}
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x040009AB RID: 2475
		private readonly List<CharacterMaster> membersList = new List<CharacterMaster>();

		// Token: 0x040009AD RID: 2477
		private List<OnDestroyCallback> onDestroyCallbacksServer;

		// Token: 0x040009AE RID: 2478
		private bool defeatedServer;

		// Token: 0x040009AF RID: 2479
		private const uint membersListDirtyBit = 1U;

		// Token: 0x040009B0 RID: 2480
		[NonSerialized]
		public Run.FixedTimeStamp awakeTime;

		// Token: 0x040009B2 RID: 2482
		public UnityEvent onDefeatedServerLogicEvent;
	}
}
