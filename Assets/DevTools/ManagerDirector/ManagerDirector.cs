using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools
{

    [System.Serializable]
    /**********************************************************************/
    // ManagerInitLayer: Helper class for specifing Initialization Layers for Managers
    public class ManagerInitLayer
    {
		public string LayerName;
        public List<ManagerBase> m_mgrs;
    }

    /**********************************************************************/
    // ManagerLocator: Handles Ordered/Parallel Initialization of Managers
    public class ManagerDirector : MonoBehaviour
    {
        public static ManagerDirector Inst;

        Dictionary<Type, ManagerBase> m_mgrCache = new Dictionary<Type, ManagerBase>();

   
        public List<ManagerInitLayer> m_managerInitLayers; // Initialization pass to get each manager running.
		public List<ManagerInitLayer> m_managerSetupLayers; // Set Up pass is an optional pass to incorporate downloaded data or account specific info. (Optional)
		private bool _initComplete = false;
		private bool _setupComplete = false;

        // Properties
        public InitState InitState { get; set; }

        /**********************************************************************/
        private void Awake()
        {
            // Force to persist on scene change
            if (Inst == null)
            {
                DontDestroyOnLoad(gameObject);
                Inst = this;


                InitState = InitState.Initializing;

               // StartCoroutine(InitLayers());
				StartCoroutine(FullInit());
            }
            else // Only one Manager locator can be present at a time
            {
                Destroy(gameObject);
            }
        }

		IEnumerator FullInit()
		{
			StartCoroutine(InitLayers());
			while(!_initComplete)
			{
				yield return null;
			}

			StartCoroutine(SetupLayers());
			while(!_setupComplete)
			{
				yield return null;
			}

			InitState = InitState.Initialized;
			OnManagerLocatorInitialized();
		}

        /**********************************************************************/
        // Initialize Managers in order, layer by layer
        IEnumerator InitLayers()
        {
            // Go through the layers
			int layerNum=1;
            foreach (ManagerInitLayer layer in m_managerInitLayers)
            {
				Debug.Log("Initializing Managers Layer " + layerNum + ": " + layer.LayerName);
                List<ManagerBase> workMgr = new List<ManagerBase>(); // List of Managers currently initializing

                // Instantiate GameObjects from Manager prefabs and Initialize
                foreach (ManagerBase mgr in layer.m_mgrs)
                {
					ManagerBase newMgr = mgr.gameObject.GetComponent<ManagerBase>();

                    if (newMgr != null)
                    {
                        workMgr.Add(newMgr);

                        AddManagerToCache(newMgr);
						newMgr.InitState = InitState.Initializing;
                        StartCoroutine(newMgr.RunInitialization());
                    }
                    else
					{
                        Debug.LogError("ManagerLocator.InitLayers: Unable to Init Manager due to missing ManagerBase component - " + mgr.name);
					}
                }

                // Wait for all Managers on this layer to finish
                bool layerFinished = false;
                while(!layerFinished)
                {
                    layerFinished = true;
                    foreach (ManagerBase mgr in workMgr)
                    {
						if(mgr.InitState == InitState.Failed)
						{
							InitState = InitState.Failed;
							break;
						}
                        if(mgr.InitState != InitState.Initialized)
                        {
                            layerFinished = false;
                            break;
                        }
                    }

                    yield return null;
                }

                Debug.Log("ManagerInitLayer: Layer " + layerNum + " Finished");
				if(InitState == InitState.Failed)
				{
					// Initialization has failed.
					RunInitializationFail();
					yield break;
				}
				layerNum ++;
            }
			_initComplete = true;
        }

		IEnumerator SetupLayers()
		{
			// Go through the layers
			int layerNum=1;
			foreach (ManagerInitLayer layer in m_managerSetupLayers)
			{
				Debug.Log("Setup Manager Layer " + layerNum + ": " + layer.LayerName);
				List<ManagerBase> workMgr = new List<ManagerBase>(); // List of Managers currently initializing

				// Instantiate GameObjects from Manager prefabs and Initialize
				foreach (ManagerBase mgr in layer.m_mgrs)
				{
					ManagerBase newMgr = mgr.gameObject.GetComponent<ManagerBase>();

					if (newMgr != null)
					{
						workMgr.Add(newMgr);

						AddManagerToCache(newMgr);
						newMgr.InitState = InitState.SettingUp;
						StartCoroutine(newMgr.RunSetup());
					}
					else
					{
						Debug.LogError("ManagerLocator.SetupLayers: Unable to Init Manager due to missing ManagerBase component - " + mgr.name);
					}
				}

				// Wait for all Managers on this layer to finish
				bool layerFinished = false;
				while(!layerFinished)
				{
					layerFinished = true;
					foreach (ManagerBase mgr in workMgr)
					{
						if(mgr.InitState == InitState.Failed)
						{
							InitState = InitState.Failed;
							break;
						}
						if(mgr.InitState != InitState.Complete)
						{
							layerFinished = false;
							break;
						}
					}

					yield return null;
				}

				Debug.Log("ManagerSetupLayer: Layer " + layerNum + " Finished");
				if(InitState == InitState.Failed)
				{
					// Initialization has failed.
					RunSetupFail();
					yield break;
				}
				layerNum ++;
			}
			_setupComplete = true;
		}

		private void RunInitializationFail()
		{
			Debug.LogError("Initialization Failed");
		}

		private void RunSetupFail()
		{
			Debug.LogError("Manager Setup Failed");
		}

        /**********************************************************************/
        // AddManagerToCache: Adds a manager to the cache
        private void AddManagerToCache(ManagerBase _mgr)
        {
			if(!m_mgrCache.ContainsKey(_mgr.GetType()))
			{
            	m_mgrCache.Add(_mgr.GetType(), _mgr);
			}
        }

        /**********************************************************************/
        // return: Manager of Type T, null if not found
        public T GetManager<T>() where T : ManagerBase
        {
            Type managerType = typeof(T);
            return GetManager(managerType) as T;
        }

        /**********************************************************************/
        // return: Manager of Type managerType, null if not found
        public ManagerBase GetManager(Type managerType)
        {
            ManagerBase mgr = null;
            m_mgrCache.TryGetValue(managerType, out mgr);            
            return mgr;
        }

        /**********************************************************************/
        // OnManagerLocatorInitialized: Calls OnFullyInitialized on all Managers that have been initialized. 
        virtual protected void OnManagerLocatorInitialized()
        {
			Debug.Log("Manager Director Running OnFullyInitialized...");
            foreach(ManagerBase mgr in m_mgrCache.Values)
            {
                mgr.OnFullyInitialized();
            }
			//DemoGame.Inst.ManagersInitializationComplete();
        }
    }
}