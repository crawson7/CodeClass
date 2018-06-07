using UnityEngine;

namespace InputClasses
{
    public class Pointer
    {
        public Vector3 Position;
        public Vector3 LastPosition;
        public Vector3 StartPosition;
        public Vector3 Delta;
        public float Time;
        public bool Down;
        public bool Drag;

        //---- Ctor
        //---------
        public Pointer()
        {
            Position = LastPosition = Delta = StartPosition = Vector3.zero;
            Time = 0;
            Down = false;
        }

        public Pointer(Vector3 position)
        {
            Position = position;
            LastPosition = position;
            StartPosition = position;
            Delta = Vector3.zero;
            Down = false;
            Time = 0;
        }

        //---- Public
        //-----------
        public void Update(Vector3 position)
        {            
            LastPosition = Position;
            Position = position;
            Delta = Position - LastPosition;
        }
    }
}
