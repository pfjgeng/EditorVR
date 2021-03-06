﻿#if UNITY_EDITOR
using UnityEngine.EventSystems;
using UnityEditor.Experimental.EditorVR.Modules;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;

namespace UnityEditor.Experimental.EditorVR.Handles
{
	sealed class SphereHandle : BaseHandle, IScrollHandler
	{
		private class SphereHandleEventData : HandleEventData
		{
			public float raycastHitDistance;

			public SphereHandleEventData(Transform rayOrigin, bool direct) : base(rayOrigin, direct) {}
		}

		private const float k_InitialScrollRate = 2f;
		private const float k_ScrollAcceleration = 14f;

		readonly static float k_ScaleBump = 1.1f;
		
		private float m_ScrollRate;
		private Vector3 m_LastPosition;
		private float m_CurrentRadius;

		protected override HandleEventData GetHandleEventData(RayEventData eventData)
		{
			return new SphereHandleEventData(eventData.rayOrigin, UIUtils.IsDirectEvent(eventData)) { raycastHitDistance = eventData.pointerCurrentRaycast.distance };
		}

		protected override void OnHandleDragStarted(HandleEventData eventData)
		{
			var sphereEventData = eventData as SphereHandleEventData;

			m_CurrentRadius = sphereEventData.raycastHitDistance;

			m_LastPosition = GetRayPoint(eventData);

			m_ScrollRate = k_InitialScrollRate;

			base.OnHandleDragStarted(eventData);
		}

		protected override void OnHandleDragging(HandleEventData eventData)
		{
			var worldPosition = GetRayPoint(eventData);

			eventData.deltaPosition = worldPosition - m_LastPosition;
			m_LastPosition = worldPosition;

			base.OnHandleDragging(eventData);
		}

		protected override void OnHandleHoverStarted(HandleEventData eventData)
		{
			transform.localScale *= k_ScaleBump;
			base.OnHandleHoverStarted(eventData);
		}

		protected override void OnHandleHoverEnded(HandleEventData eventData)
		{
			transform.localScale /= k_ScaleBump;
			base.OnHandleHoverStarted(eventData);
		}

		public void ChangeRadius(float delta)
		{
			m_CurrentRadius += delta;
			m_CurrentRadius = Mathf.Max(m_CurrentRadius, 0f);
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (m_DragSources.Count == 0)
				return;

			// Scolling changes the radius of the sphere while dragging, and accelerates
			if (Mathf.Abs(eventData.scrollDelta.y) > 0.5f)
				m_ScrollRate += Mathf.Abs(eventData.scrollDelta.y)*k_ScrollAcceleration*Time.unscaledDeltaTime;
			else
				m_ScrollRate = k_InitialScrollRate;

			ChangeRadius(m_ScrollRate*eventData.scrollDelta.y*Time.unscaledDeltaTime);
		}

		private Vector3 GetRayPoint(HandleEventData eventData)
		{
			var rayOrigin = eventData.rayOrigin;
			var ray = new Ray(rayOrigin.position, rayOrigin.forward);
			return ray.GetPoint(m_CurrentRadius);
		}
	}
}
#endif
