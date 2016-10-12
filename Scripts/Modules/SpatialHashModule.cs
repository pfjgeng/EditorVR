﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine.VR.Data;

namespace UnityEngine.VR.Modules
{
	public class SpatialHashModule : MonoBehaviour
	{
		public SpatialHash<Renderer> spatialHash { get; private set; }

		void Awake()
		{
			spatialHash = new SpatialHash<Renderer>();
		}

		internal void Setup()
		{
			SetupObjects();
			StartCoroutine(UpdateDynamicObjects());
		}

		void SetupObjects()
		{
			MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
			foreach (var mf in meshFilters)
			{
				if (mf.sharedMesh)
				{
					// Exclude EditorVR objects
					if (mf.GetComponentInParent<EditorVR>())
						continue;

					Renderer renderer = mf.GetComponent<Renderer>();
					if (renderer)
						spatialHash.AddObject(renderer, renderer.bounds);
				}
			}

			SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
			foreach (var smr in skinnedMeshRenderers)
			{
				if (smr.sharedMesh)
				{
					// Exclude EditorVR objects
					if (smr.GetComponentInParent<EditorVR>())
						continue;
					
					spatialHash.AddObject(smr, smr.bounds);
				}
			}
		}

		private IEnumerator UpdateDynamicObjects()
		{
			while (true)
			{
				// TODO AE 9/21/16: Hook updates of new objects that are created
				List<Renderer> allObjects = new List<Renderer>(spatialHash.allObjects);
				foreach (var obj in allObjects)
				{
					if (obj.transform.hasChanged)
					{
						spatialHash.RemoveObject(obj);
						spatialHash.AddObject(obj, obj.bounds);
						obj.transform.hasChanged = false;
					}
				}

				yield return null;
			}
		}
	}
}