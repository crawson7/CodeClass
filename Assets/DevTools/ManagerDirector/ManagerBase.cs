using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools
{
    // Initialization State of a Manager
    public enum InitState
    {
        Uninitialized, // Has not been initialized
        Initializing,  // Started the initialization process but hasn't finished
        Initialized,    // Finished initializing. NOTE: this is true before Fully Initialized has been called
		SettingUp,
		Complete,
		Failed	// Initialization Attempted but failed.
    };

	public delegate void InitEndAction(bool success);


    /**********************************************************************/
    // ManagerBase: Base class for all managers, managers are generally accessed via the ManagerLocator
    abstract public class ManagerBase : MonoBehaviour
    {
        InitState m_mgrState = InitState.Uninitialized;
                
        // Properties
        public InitState InitState { get { return m_mgrState; } set { m_mgrState = value; } } // Initialization state of the manager

        // Abstract Methods
        // RunInitialization: Started in (quasi) parallel with other manager in the same initialization 
        abstract public IEnumerator RunInitialization();
		abstract public IEnumerator RunSetup();

        // Methods

        // OnFullyInitialized: Call by the ManagerLocator once ALL managers from the ManagerLocator have been initialized
        virtual public void OnFullyInitialized() { }
        virtual public IEnumerator Reset(Action<string> _finished) { yield break; }
        public bool IsInitialized() { return InitState == InitState.Initialized; }

    }

}