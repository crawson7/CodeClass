using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Red
{
	public enum EaseType
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic
	}

	public class Easing
	{
		#region Public Members
		public delegate Vector3 ToVector3<T>(T v);
		public delegate float Function(float startValue, float endValue, float value);
		#endregion

		private static IEnumerable<float> NewCounter(int start, int end, int step)
		{
			// Generates sequence of integers from start to end(inclusive) one step at a time.
			for (int i = start; i <= end; i += step)
			{
				yield return i;
			}
		}

		public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration)
		{
			// Returns sequence generator from start to end over duration using the
			// given easing function. The sequence is generated as it is accessed
			// using the Time.deltaTime to calculate the portion of duration that has elapsed
			IEnumerable<float> timer = Easing.NewTimer(duration);
			return NewEase(ease, start, end, duration, timer);
		}

		public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices)
		{
			// Instead of easing based on time, generate n interpolated points(slices)
			// between the start and end positions.
			IEnumerable<float> counter = Easing.NewCounter(0, slices + 1, 1);
			return NewEase(ease, start, end, slices + 1, counter);
		}

		public static Function EaseFunction(EaseType type)
		{
			//Returns the static method that implements the given easing type for scalars.
			//Use this method to easily switch between easing interpolation types.
			//All easing methods clamp elapsedTime so that it is always less than duration.
			Function f = null;
			switch (type)
			{
			case EaseType.Linear: f = Easing.Linear; break;
			case EaseType.EaseInQuad: f = Easing.EaseInQuad; break;
			case EaseType.EaseOutQuad: f = Easing.EaseOutQuad; break;
			case EaseType.EaseInOutQuad: f = Easing.EaseInOutQuad; break;
			case EaseType.EaseInCubic: f = Easing.EaseInCubic; break;
			case EaseType.EaseOutCubic: f = Easing.EaseOutCubic; break;
			case EaseType.EaseInOutCubic: f = Easing.EaseInOutCubic; break;
			case EaseType.EaseInQuart: f = Easing.EaseInQuart; break;
			case EaseType.EaseOutQuart: f = Easing.EaseOutQuart; break;
			case EaseType.EaseInOutQuart: f = Easing.EaseInOutQuart; break;
			case EaseType.EaseInQuint: f = Easing.EaseInQuint; break;
			case EaseType.EaseOutQuint: f = Easing.EaseOutQuint; break;
			case EaseType.EaseInOutQuint: f = Easing.EaseInOutQuint; break;
			case EaseType.EaseInSine: f = Easing.EaseInSine; break;
			case EaseType.EaseOutSine: f = Easing.EaseOutSine; break;
			case EaseType.EaseInOutSine: f = Easing.EaseInOutSine; break;
			case EaseType.EaseInExpo: f = Easing.EaseInExpo; break;
			case EaseType.EaseOutExpo: f = Easing.EaseOutExpo; break;
			case EaseType.EaseInOutExpo: f = Easing.EaseInOutExpo; break;
			case EaseType.EaseInCirc: f = Easing.EaseInCirc; break;
			case EaseType.EaseOutCirc: f = Easing.EaseOutCirc; break;
			case EaseType.EaseInOutCirc: f = Easing.EaseInOutCirc; break;
			case EaseType.EaseInBounce: f = Easing.EaseInBounce; break;
			case EaseType.EaseOutBounce: f = Easing.EaseOutBounce; break;
			case EaseType.EaseInOutBounce: f = Easing.EaseInOutBounce; break;
			case EaseType.EaseInBack: f = Easing.EaseInBack; break;
			case EaseType.EaseOutBack: f = Easing.EaseOutBack; break;
			case EaseType.EaseInOutBack: f = Easing.EaseInOutBack; break;
			case EaseType.EaseInElastic: f = Easing.EaseInElastic; break;
			case EaseType.EaseOutElastic: f = Easing.EaseOutElastic; break;
			case EaseType.EaseInOutElastic: f = Easing.EaseInOutElastic; break;
			}
			return f;
		}

		#region Easing Set Up

		private static Vector3 Identity(Vector3 v)
		{ return v; }

		private static Vector3 TransformDotPosition(Transform t)
		{ return t.position; }


		private static IEnumerable<float> NewTimer(float duration)
		{
			float elapsedTime = 0.0f;
			while (elapsedTime < duration)
			{
				yield return elapsedTime;
				elapsedTime += Time.deltaTime;
				// make sure last value is never skipped
				if (elapsedTime >= duration)
				{
					yield return elapsedTime;
				}
			}
		}

		private static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
		{
			//Generic easing sequence generator used to implement the time and
			//slice variants.Normally you would not use this function directly.
			Vector3 distance = end - start;
			foreach (float i in driver)
			{
				float value = total/i;
				yield return EaseVector(ease, start, distance, value);
			}
		}

		/**
	     * Vector3 interpolation using given easing method. Easing is done independently
	     * on all three vector axis.
	     */
		public static Vector3 EaseVector(Function ease, Vector3 start, Vector3 end, float value)
		{
			start.x = ease(start.x, end.x, value);
			start.y = ease(start.y, end.y, value);
			start.z = ease(start.z, end.z, value);
			return start;
		}

		#endregion

		#region EASE FUNCTIONS
		private static float Linear(float start, float end, float value){
			return Mathf.Lerp(start, end, value);
		}

		private static float Clerp(float start, float end, float value){
			float min = 0.0f;
			float max = 360.0f;
			float half = Mathf.Abs((max - min) * 0.5f);
			float retval = 0.0f;
			float diff = 0.0f;
			if ((end - start) < -half){
				diff = ((max - start) + end) * value;
				retval = start + diff;
			}else if ((end - start) > half){
				diff = -((max - end) + start) * value;
				retval = start + diff;
			}else retval = start + (end - start) * value;
			return retval;
		}

		private static float Spring(float start, float end, float value){
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}
			
		private static float EaseInQuad(float start, float end, float value){
			end -= start;
			return end * value * value + start;
		}

		private static float EaseOutQuad(float start, float end, float value){
			end -= start;
			return -end * value * (value - 2) + start;
		}
		private static float EaseInOutQuad(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end * 0.5f * value * value + start;
			value--;
			return -end * 0.5f * (value * (value - 2) - 1) + start;
		}

		private static float EaseInCubic(float start, float end, float value){
			end -= start;
			return end * value * value * value + start;
		}

		private static float EaseOutCubic(float start, float end, float value){
			value--;
			end -= start;
			return end * (value * value * value + 1) + start;
		}

		private static float EaseInOutCubic(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end * 0.5f * value * value * value + start;
			value -= 2;
			return end * 0.5f * (value * value * value + 2) + start;
		}

		private static float EaseInQuart(float start, float end, float value){
			end -= start;
			return end * value * value * value * value + start;
		}

		private static float EaseOutQuart(float start, float end, float value){
			value--;
			end -= start;
			return -end * (value * value * value * value - 1) + start;
		}

		private static float EaseInOutQuart(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end * 0.5f * value * value * value * value + start;
			value -= 2;
			return -end * 0.5f * (value * value * value * value - 2) + start;
		}

		private static float EaseInQuint(float start, float end, float value){
			end -= start;
			return end * value * value * value * value * value + start;
		}

		private static float EaseOutQuint(float start, float end, float value){
			value--;
			end -= start;
			return end * (value * value * value * value * value + 1) + start;
		}

		private static float EaseInOutQuint(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end * 0.5f * value * value * value * value * value + start;
			value -= 2;
			return end * 0.5f * (value * value * value * value * value + 2) + start;
		}

		private static float EaseInSine(float start, float end, float value){
			end -= start;
			return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
		}

		private static float EaseOutSine(float start, float end, float value){
			end -= start;
			return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
		}

		private static float EaseInOutSine(float start, float end, float value){
			end -= start;
			return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
		}

		private static float EaseInExpo(float start, float end, float value){
			end -= start;
			return end * Mathf.Pow(2, 10 * (value - 1)) + start;
		}

		private static float EaseOutExpo(float start, float end, float value){
			end -= start;
			return end * (-Mathf.Pow(2, -10 * value ) + 1) + start;
		}

		private static float EaseInOutExpo(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
			value--;
			return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
		}

		private static float EaseInCirc(float start, float end, float value){
			end -= start;
			return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
		}

		private static float EaseOutCirc(float start, float end, float value){
			value--;
			end -= start;
			return end * Mathf.Sqrt(1 - value * value) + start;
		}

		private static float EaseInOutCirc(float start, float end, float value){
			value /= .5f;
			end -= start;
			if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
			value -= 2;
			return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
		}

		private static float EaseInBounce(float start, float end, float value){
			end -= start;
			float d = 1f;
			return end - EaseOutBounce(0, end, d-value) + start;
		}

		private static float EaseOutBounce(float start, float end, float value){
			value /= 1f;
			end -= start;
			if (value < (1 / 2.75f)){
				return end * (7.5625f * value * value) + start;
			}else if (value < (2 / 2.75f)){
				value -= (1.5f / 2.75f);
				return end * (7.5625f * (value) * value + .75f) + start;
			}else if (value < (2.5 / 2.75)){
				value -= (2.25f / 2.75f);
				return end * (7.5625f * (value) * value + .9375f) + start;
			}else{
				value -= (2.625f / 2.75f);
				return end * (7.5625f * (value) * value + .984375f) + start;
			}
		}

		private static float EaseInOutBounce(float start, float end, float value){
			end -= start;
			float d = 1f;
			if (value < d* 0.5f) return EaseInBounce(0, end, value*2) * 0.5f + start;
			else return EaseOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
		}

		private static float EaseInBack(float start, float end, float value){
			end -= start;
			value /= 1;
			float s = 1.70158f;
			return end * (value) * value * ((s + 1) * value - s) + start;
		}

		private static float EaseOutBack(float start, float end, float value){
			float s = 1.70158f;
			end -= start;
			value = (value) - 1;
			return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
		}

		private static float EaseInOutBack(float start, float end, float value){
			float s = 1.70158f;
			end -= start;
			value /= .5f;
			if ((value) < 1){
				s *= (1.525f);
				return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
			}
			value -= 2;
			s *= (1.525f);
			return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
		}
			
		private static float EaseInElastic(float start, float end, float value){
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if (value == 0) return start;

			if ((value /= d) == 1) return start + end;

			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p / 4;
			}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		}		

		private static float EaseOutElastic(float start, float end, float value)
		{
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if (value == 0) return start;

			if ((value /= d) == 1) return start + end;

			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p * 0.25f;
			}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
		}		
			
		private static float EaseInOutElastic(float start, float end, float value){
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if (value == 0) return start;

			if ((value /= d*0.5f) == 2) return start + end;

			if (a == 0f || a < Mathf.Abs(end)){
				a = end;
				s = p / 4;
			}else{
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
			return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
		}		
		#endregion
	}

	/**
	 * Sequence generators are used as follows:
	 *
	 * IEnumerable<Vector3> sequence = Easing.NewEase(configuration);
	 * foreach (Vector3 newPoint in sequence) {
	 *   transform.position = newPoint;
	 *   yield return WaitForSeconds(1.0f);
	 * }
	 *
	 * Or:
	 *
	 * IEnumerator<Vector3> sequence = Ease.NewEase(configuration).GetEnumerator();
	 * function Update() {
	 *   if (sequence.MoveNext()) {
	 *     transform.position = sequence.Current;
	 *   }
	 * }
	 *
	 * The low level functions work similarly to Unity's built in Lerp and it is
	 * up to you to track and pass in elapsedTime and duration on every call. 
	 *
	 * float value = ease(start, distance, elapsedTime, duration);
	 */
}
