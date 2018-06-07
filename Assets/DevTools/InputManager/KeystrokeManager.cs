using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputInterfaces;
using InputClasses;

public enum Keystroke
{
	One,Two,Three,Four,Five,Six,Seven,Eight,Nine,Zero,
	A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,Return, Space, BackSpace
}

public delegate void InputKey(Keystroke key);

namespace GameTools
{
	public class KeystrokeManager : ManagerBase, IKeyListener 
	{
		public bool Active;
		public bool Upper = false;

		private InputKey _keyListeners;

		#region implemented abstract members of ManagerBase
		public override IEnumerator RunInitialization ()
		{
			// run initialization
			Debug.Log("Initializing Keystroke manager");
			InitState = GameTools.InitState.Initialized;
			yield return null;
		}

		public override IEnumerator RunSetup ()
		{
			// run initialization
			Debug.Log("Setting Up Keystroke manager");
			InitState = GameTools.InitState.Complete;
			yield return null;
		}
		#endregion

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (!Active || InitState != GameTools.InitState.Complete)
			{
				return;
			}

			#if UNITY_EDITOR
			CheckKeyboardInputs();
			#endif
		}

		private void CheckKeyboardInputs()
		{
			if(InitState != GameTools.InitState.Complete){return;}

			if(Input.GetKeyDown(KeyCode.LeftShift))
			{Upper = true;}
			if(Input.GetKeyUp(KeyCode.LeftShift))
			{Upper = false;}

			if(Input.GetKeyDown(KeyCode.Return))
			{OnKeyDown(Keystroke.Return);}
			if(Input.GetKeyDown(KeyCode.Space))
			{OnKeyDown(Keystroke.Space);}
			if(Input.GetKeyDown(KeyCode.Backspace))
			{OnKeyDown(Keystroke.BackSpace);}

			#region NUMERICAL INPUTS
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{OnKeyDown(Keystroke.One);}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{OnKeyDown(Keystroke.Two);}
			if(Input.GetKeyDown(KeyCode.Alpha3))
			{OnKeyDown(Keystroke.Three);}
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{OnKeyDown(Keystroke.Four);}
			if(Input.GetKeyDown(KeyCode.Alpha5))
			{OnKeyDown(Keystroke.Five);}
			if(Input.GetKeyDown(KeyCode.Alpha6))
			{OnKeyDown(Keystroke.Six);}
			if(Input.GetKeyDown(KeyCode.Alpha7))
			{OnKeyDown(Keystroke.Seven);}
			if(Input.GetKeyDown(KeyCode.Alpha8))
			{OnKeyDown(Keystroke.Eight);}
			if(Input.GetKeyDown(KeyCode.Alpha9))
			{OnKeyDown(Keystroke.Nine);}
			if(Input.GetKeyDown(KeyCode.Alpha0))
			{OnKeyDown(Keystroke.Zero);}
			#endregion

			#region ALPHABETIC INPUTS
			if(Input.GetKeyDown(KeyCode.A))
			{OnKeyDown(Keystroke.A);}
			if(Input.GetKeyDown(KeyCode.B))
			{OnKeyDown(Keystroke.B);}
			if(Input.GetKeyDown(KeyCode.C))
			{OnKeyDown(Keystroke.C);}
			if(Input.GetKeyDown(KeyCode.D))
			{OnKeyDown(Keystroke.D);}
			if(Input.GetKeyDown(KeyCode.E))
			{OnKeyDown(Keystroke.E);}
			if(Input.GetKeyDown(KeyCode.F))
			{OnKeyDown(Keystroke.F);}
			if(Input.GetKeyDown(KeyCode.G))
			{OnKeyDown(Keystroke.G);}
			if(Input.GetKeyDown(KeyCode.H))
			{OnKeyDown(Keystroke.H);}
			if(Input.GetKeyDown(KeyCode.I))
			{OnKeyDown(Keystroke.I);}
			if(Input.GetKeyDown(KeyCode.J))
			{OnKeyDown(Keystroke.J);}
			if(Input.GetKeyDown(KeyCode.K))
			{OnKeyDown(Keystroke.K);}
			if(Input.GetKeyDown(KeyCode.L))
			{OnKeyDown(Keystroke.L);}
			if(Input.GetKeyDown(KeyCode.M))
			{OnKeyDown(Keystroke.M);}
			if(Input.GetKeyDown(KeyCode.N))
			{OnKeyDown(Keystroke.N);}
			if(Input.GetKeyDown(KeyCode.O))
			{OnKeyDown(Keystroke.O);}
			if(Input.GetKeyDown(KeyCode.P))
			{OnKeyDown(Keystroke.P);}
			if(Input.GetKeyDown(KeyCode.Q))
			{OnKeyDown(Keystroke.Q);}
			if(Input.GetKeyDown(KeyCode.R))
			{OnKeyDown(Keystroke.R);}
			if(Input.GetKeyDown(KeyCode.S))
			{OnKeyDown(Keystroke.S);}
			if(Input.GetKeyDown(KeyCode.T))
			{OnKeyDown(Keystroke.T);}
			if(Input.GetKeyDown(KeyCode.U))
			{OnKeyDown(Keystroke.U);}
			if(Input.GetKeyDown(KeyCode.V))
			{OnKeyDown(Keystroke.V);}
			if(Input.GetKeyDown(KeyCode.W))
			{OnKeyDown(Keystroke.W);}
			if(Input.GetKeyDown(KeyCode.X))
			{OnKeyDown(Keystroke.X);}
			if(Input.GetKeyDown(KeyCode.Y))
			{OnKeyDown(Keystroke.Y);}
			if(Input.GetKeyDown(KeyCode.Z))
			{OnKeyDown(Keystroke.Z);}
			#endregion
		}

		//---- Interfaces
		//---------------
		#region Down
		public void AddKeyListener(InputKey toAdd)
		{
			_keyListeners += toAdd;
		}

		public void RemoveKeyListener(InputKey toRemove)
		{
			if (_keyListeners == null)
			{
				return;
			}
			_keyListeners -= toRemove;
		}

		public void OnKeyDown(Keystroke key)
		{
			if (_keyListeners == null)
			{
				return;
			}
			_keyListeners(key);
		}
		#endregion

	}
}
