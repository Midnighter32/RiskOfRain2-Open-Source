using System;
using System.Collections.Generic;
using System.Reflection;
using Rewired.Integration.UnityUI;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200060D RID: 1549
	[RequireComponent(typeof(MPInput))]
	public class MPInputModule : RewiredStandaloneInputModule
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060022E6 RID: 8934 RVA: 0x000A48BB File Offset: 0x000A2ABB
		private bool useCursor
		{
			get
			{
				return ((MPEventSystem)base.eventSystem).isCursorVisible;
			}
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000A48CD File Offset: 0x000A2ACD
		protected override void Awake()
		{
			this.m_InputOverride = base.GetComponent<MPInput>();
			this.m_MouseState = (RewiredPointerInputModule.MouseState)MPInputModule.m_MouseStateField.GetValue(this);
			base.Awake();
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x000A48F7 File Offset: 0x000A2AF7
		protected override void Start()
		{
			base.Start();
			base.ClearMouseInputSources();
			base.AddMouseInputSource((MPInput)this.m_InputOverride);
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x000A4918 File Offset: 0x000A2B18
		protected void UpdateHover(List<RaycastResult> raycastResults)
		{
			this.isHovering = false;
			if (!this.useCursor)
			{
				return;
			}
			foreach (RaycastResult raycastResult in raycastResults)
			{
				if (raycastResult.gameObject)
				{
					Selectable componentInParent = raycastResult.gameObject.GetComponentInParent<Selectable>();
					if (componentInParent != null && this.<UpdateHover>g__IsHoverable|9_0(componentInParent))
					{
						this.isHovering = true;
						break;
					}
				}
			}
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x000A49A8 File Offset: 0x000A2BA8
		protected override RewiredPointerInputModule.MouseState GetMousePointerEventData(int playerId, int mouseIndex)
		{
			IMouseInputSource mouseInputSource = base.GetMouseInputSource(playerId, mouseIndex);
			if (mouseInputSource == null)
			{
				return null;
			}
			PlayerPointerEventData playerPointerEventData;
			bool pointerData = base.GetPointerData(playerId, mouseIndex, -1, out playerPointerEventData, true, PointerEventType.Mouse);
			playerPointerEventData.Reset();
			if (pointerData)
			{
				playerPointerEventData.position = base.input.mousePosition;
			}
			Vector2 mousePosition = base.input.mousePosition;
			if (mouseInputSource.locked)
			{
				playerPointerEventData.position = new Vector2(-1f, -1f);
				playerPointerEventData.delta = Vector2.zero;
			}
			else
			{
				playerPointerEventData.delta = mousePosition - playerPointerEventData.position;
				playerPointerEventData.position = mousePosition;
			}
			playerPointerEventData.scrollDelta = mouseInputSource.wheelDelta;
			playerPointerEventData.button = PointerEventData.InputButton.Left;
			base.eventSystem.RaycastAll(playerPointerEventData, this.m_RaycastResultCache);
			RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			playerPointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
			this.UpdateHover(this.m_RaycastResultCache);
			this.m_RaycastResultCache.Clear();
			PlayerPointerEventData playerPointerEventData2;
			base.GetPointerData(playerId, mouseIndex, -2, out playerPointerEventData2, true, PointerEventType.Mouse);
			base.CopyFromTo(playerPointerEventData, playerPointerEventData2);
			playerPointerEventData2.button = PointerEventData.InputButton.Right;
			PlayerPointerEventData playerPointerEventData3;
			base.GetPointerData(playerId, mouseIndex, -3, out playerPointerEventData3, true, PointerEventType.Mouse);
			base.CopyFromTo(playerPointerEventData, playerPointerEventData3);
			playerPointerEventData3.button = PointerEventData.InputButton.Middle;
			for (int i = 3; i < mouseInputSource.buttonCount; i++)
			{
				PlayerPointerEventData playerPointerEventData4;
				base.GetPointerData(playerId, mouseIndex, -2147483520 + i, out playerPointerEventData4, true, PointerEventType.Mouse);
				base.CopyFromTo(playerPointerEventData, playerPointerEventData4);
				playerPointerEventData4.button = (PointerEventData.InputButton)(-1);
			}
			this.m_MouseState.SetButtonState(0, base.StateForMouseButton(playerId, mouseIndex, 0), playerPointerEventData);
			this.m_MouseState.SetButtonState(1, base.StateForMouseButton(playerId, mouseIndex, 1), playerPointerEventData2);
			this.m_MouseState.SetButtonState(2, base.StateForMouseButton(playerId, mouseIndex, 2), playerPointerEventData3);
			for (int j = 3; j < mouseInputSource.buttonCount; j++)
			{
				PlayerPointerEventData data;
				base.GetPointerData(playerId, mouseIndex, -2147483520 + j, out data, false, PointerEventType.Mouse);
				this.m_MouseState.SetButtonState(j, base.StateForMouseButton(playerId, mouseIndex, j), data);
			}
			return this.m_MouseState;
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x000A4B90 File Offset: 0x000A2D90
		protected override void ProcessMove(PlayerPointerEventData pointerEvent)
		{
			GameObject newEnterTarget = (!this.useCursor) ? null : pointerEvent.pointerCurrentRaycast.gameObject;
			base.HandlePointerExitAndEnter(pointerEvent, newEnterTarget);
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000A4BC0 File Offset: 0x000A2DC0
		protected override void ProcessDrag(PlayerPointerEventData pointerEvent)
		{
			if (!pointerEvent.IsPointerMoving() || !this.useCursor || pointerEvent.pointerDrag == null)
			{
				return;
			}
			if (!pointerEvent.dragging && MPInputModule.ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float)base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
			{
				ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
				pointerEvent.dragging = true;
			}
			if (pointerEvent.dragging)
			{
				if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
					pointerEvent.eligibleForClick = false;
					pointerEvent.pointerPress = null;
					pointerEvent.rawPointerPress = null;
				}
				ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
			}
		}

		// Token: 0x040025E8 RID: 9704
		private RewiredPointerInputModule.MouseState m_MouseState;

		// Token: 0x040025E9 RID: 9705
		private static readonly FieldInfo m_MouseStateField = typeof(RewiredPointerInputModule).GetField("m_MouseState", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x040025EA RID: 9706
		private static readonly MPInputModule.ShouldStartDragDelegate ShouldStartDrag = (MPInputModule.ShouldStartDragDelegate)typeof(RewiredPointerInputModule).GetMethod("ShouldStartDrag", BindingFlags.Static | BindingFlags.NonPublic).CreateDelegate(typeof(MPInputModule.ShouldStartDragDelegate));

		// Token: 0x040025EB RID: 9707
		public bool isHovering;

		// Token: 0x0200060E RID: 1550
		// (Invoke) Token: 0x060022F1 RID: 8945
		private delegate bool ShouldStartDragDelegate(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold);
	}
}
