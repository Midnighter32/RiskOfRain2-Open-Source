using System;
using UnityEngine;

namespace RoR2.Audio
{
	// Token: 0x02000680 RID: 1664
	public static class LoopSoundManager
	{
		// Token: 0x060026FF RID: 9983 RVA: 0x000A9BC0 File Offset: 0x000A7DC0
		public static LoopSoundManager.SoundLoopPtr PlaySoundLoopLocal(GameObject gameObject, LoopSoundDef loopSoundDef)
		{
			gameObject.GetComponent<AkGameObj>();
			LoopSoundManager.SoundLoopHelper soundLoopHelper = gameObject.GetComponent<LoopSoundManager.SoundLoopHelper>();
			if (!soundLoopHelper)
			{
				soundLoopHelper = gameObject.AddComponent<LoopSoundManager.SoundLoopHelper>();
				soundLoopHelper.cachedGameObject = soundLoopHelper.gameObject;
			}
			LoopSoundManager.SoundLoopPtr soundLoopPtr = new LoopSoundManager.SoundLoopPtr(LoopSoundManager.soundLoopHeap.Alloc());
			LoopSoundManager.SoundLoopPtr last = soundLoopHelper.last;
			ref LoopSoundManager.SoundLoopNode @ref = ref soundLoopPtr.GetRef();
			@ref.owner = soundLoopHelper;
			@ref.loopSoundDef = loopSoundDef;
			if (soundLoopHelper.last.isValid)
			{
				@ref.previous = last;
				last.GetRef().next = soundLoopPtr;
			}
			else
			{
				soundLoopHelper.first = soundLoopPtr;
			}
			soundLoopHelper.last = soundLoopPtr;
			@ref.akId = AkSoundEngine.PostEvent(loopSoundDef.startSoundName, gameObject);
			return soundLoopPtr;
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x000A9C6C File Offset: 0x000A7E6C
		public static void StopSoundLoopLocal(LoopSoundManager.SoundLoopPtr ptr)
		{
			if (!ptr.isValid)
			{
				return;
			}
			ref LoopSoundManager.SoundLoopNode @ref = ref ptr.GetRef();
			AkSoundEngine.PostEvent(@ref.loopSoundDef.stopSoundName, @ref.owner.cachedGameObject);
			if (@ref.previous.isValid)
			{
				@ref.previous.GetRef().next = @ref.next;
			}
			else
			{
				@ref.owner.first = @ref.next;
			}
			if (@ref.next.isValid)
			{
				@ref.next.GetRef().previous = @ref.previous;
			}
			else
			{
				@ref.owner.last = @ref.previous;
			}
			LoopSoundManager.soundLoopHeap.Free(ptr.ptr);
		}

		// Token: 0x040024C5 RID: 9413
		private static readonly HGHeap<LoopSoundManager.SoundLoopNode> soundLoopHeap = new HGHeap<LoopSoundManager.SoundLoopNode>(128U);

		// Token: 0x02000681 RID: 1665
		public struct SoundLoopPtr
		{
			// Token: 0x06002702 RID: 9986 RVA: 0x000A9D35 File Offset: 0x000A7F35
			public SoundLoopPtr(HGHeap<LoopSoundManager.SoundLoopNode>.Ptr ptr)
			{
				this.ptr = ptr;
			}

			// Token: 0x170003EE RID: 1006
			// (get) Token: 0x06002703 RID: 9987 RVA: 0x000A9D3E File Offset: 0x000A7F3E
			public bool isValid
			{
				get
				{
					return LoopSoundManager.soundLoopHeap.PtrIsValid(this.ptr);
				}
			}

			// Token: 0x06002704 RID: 9988 RVA: 0x000A9D50 File Offset: 0x000A7F50
			public LoopSoundManager.SoundLoopNode GetValue()
			{
				return LoopSoundManager.soundLoopHeap.GetValue(this.ptr);
			}

			// Token: 0x06002705 RID: 9989 RVA: 0x000A9D62 File Offset: 0x000A7F62
			public void SetValue(in LoopSoundManager.SoundLoopNode value)
			{
				LoopSoundManager.soundLoopHeap.SetValue(this.ptr, value);
			}

			// Token: 0x06002706 RID: 9990 RVA: 0x000A9D75 File Offset: 0x000A7F75
			public ref LoopSoundManager.SoundLoopNode GetRef()
			{
				return LoopSoundManager.soundLoopHeap.GetRef(this.ptr);
			}

			// Token: 0x06002707 RID: 9991 RVA: 0x000A9D87 File Offset: 0x000A7F87
			public void SetRtpc(string rtpcName, float value)
			{
				AkSoundEngine.SetRTPCValueByPlayingID(rtpcName, value, this.GetRef().akId);
			}

			// Token: 0x040024C6 RID: 9414
			public readonly HGHeap<LoopSoundManager.SoundLoopNode>.Ptr ptr;
		}

		// Token: 0x02000682 RID: 1666
		public class SoundLoopHelper : MonoBehaviour
		{
			// Token: 0x170003EF RID: 1007
			// (get) Token: 0x06002708 RID: 9992 RVA: 0x000A9D9C File Offset: 0x000A7F9C
			// (set) Token: 0x06002709 RID: 9993 RVA: 0x000A9DA4 File Offset: 0x000A7FA4
			public LoopSoundManager.SoundLoopPtr first { get; set; }

			// Token: 0x170003F0 RID: 1008
			// (get) Token: 0x0600270A RID: 9994 RVA: 0x000A9DAD File Offset: 0x000A7FAD
			// (set) Token: 0x0600270B RID: 9995 RVA: 0x000A9DB5 File Offset: 0x000A7FB5
			public LoopSoundManager.SoundLoopPtr last { get; set; }

			// Token: 0x170003F1 RID: 1009
			// (get) Token: 0x0600270C RID: 9996 RVA: 0x000A9DBE File Offset: 0x000A7FBE
			// (set) Token: 0x0600270D RID: 9997 RVA: 0x000A9DC6 File Offset: 0x000A7FC6
			public GameObject cachedGameObject { get; set; }

			// Token: 0x0600270E RID: 9998 RVA: 0x000A9DD0 File Offset: 0x000A7FD0
			private void OnDestroy()
			{
				while (this.first.isValid)
				{
					LoopSoundManager.StopSoundLoopLocal(this.first);
				}
			}
		}

		// Token: 0x02000683 RID: 1667
		public struct SoundLoopNode
		{
			// Token: 0x040024CA RID: 9418
			public LoopSoundManager.SoundLoopHelper owner;

			// Token: 0x040024CB RID: 9419
			public LoopSoundDef loopSoundDef;

			// Token: 0x040024CC RID: 9420
			public uint akId;

			// Token: 0x040024CD RID: 9421
			public LoopSoundManager.SoundLoopPtr next;

			// Token: 0x040024CE RID: 9422
			public LoopSoundManager.SoundLoopPtr previous;
		}
	}
}
