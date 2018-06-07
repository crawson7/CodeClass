using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBuffer<T>
{
	private int mLength;
	private T[] mBuffer;
	private int mCurrent;
	private int mSize;

	private int Next {get{return (mCurrent<mLength-1)? mCurrent+1 : 0;}}
	private int Prev {get{return (mCurrent>0)? mCurrent-1 : mLength-1;}}
	public int Size {get{return mSize;}}

	public RingBuffer(int size)
	{
		mLength = size;
		mBuffer = new T[size];
		mCurrent = 0;
	}

	public void Add(T val)
	{
		mCurrent = Next;
		mBuffer[mCurrent] = val;
		mSize = (mSize<mLength)? mSize+1 : mLength;
	}

	public T Last()
	{
		return mBuffer[mCurrent];
	}

	public T ValueAt(int i)
	{
		// Index 0 will always return the most recent Value
		// Index Size-1 will always return the oldest Value
		if(i>= mSize || i<0){return default(T);}

		return mBuffer[GetIndex(i)];
	}

	public void Clear()
	{
		mBuffer = new T[mLength];
		mCurrent = 0;
	}

	private int GetIndex(int i)
	{
		if(i>=mSize){return 0;}
		int index = mCurrent - i;
		if(index<0)
		{
			return mLength + index;
		}
		else
		{
			return index;
		}
	}
}
