using UnityEngine;

/// <summary>
/// Interfaces for inputs
/// </summary>
namespace InputInterfaces
{
    public interface IDownListener
    {
        void OnDown(Vector3 position);
    }

    public interface IUpListener
    {
        void OnUp(Vector3 position);
    }

    public interface IUpdateListener
    {
        void OnUpdate(Vector3 position);
    }

    public interface ITapListener
    {
        void OnTap(Vector3 position);
    }

    public interface IDoubleTapListener
    {
        void OnDoubleTap(Vector3 position);
    }

    public interface IDragListener
    {
        void OnDragStart(Vector3 position);
        void OnDragUpdate(Vector3 position, Vector3 delta);
        void OnDragEnd(Vector3 position);
    }

    public interface ISwipeListener
    {
        void OnSwipe(Vector3 start, Vector3 end, float time);
    }

    public interface IPinchListener
    {
        void OnPinchStart(Vector3 firstPosition, Vector3 secondPosition);
        void OnPinchUpdate(Vector3 firstPosition, Vector3 secondPosition);
        void OnPinchEnd(Vector3 firstPosition, Vector3 secondPosition);
    }

    public interface IRotateListener
    {
        void OnRotateStart(Vector3 firstPosition, Vector3 secondPosition);
        void OnRotateUpdate(Vector3 firstPosition, Vector3 secondPosition);
        void OnRotateEnd(Vector3 firstPosition, Vector3 secondPosition);
    }

	public interface IKeyListener
	{
		void OnKeyDown(Keystroke key);
	}
}
