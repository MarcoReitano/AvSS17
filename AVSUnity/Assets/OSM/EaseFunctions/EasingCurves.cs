using System.Collections.Generic;
using UnityEngine;


public static class EasingCurves
{
    private delegate float EasingFunction(float start, float end, float value);

    /// <summary>
    /// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
    /// </summary>
    public enum EaseType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,

        easeInCubic,
        easeOutCubic,
        easeInOutCubic,

        easeInQuart,
        easeOutQuart,
        easeInOutQuart,

        easeInQuint,
        easeOutQuint,
        easeInOutQuint,

        easeInSine,
        easeOutSine,
        easeInOutSine,

        easeInExpo,
        easeOutExpo,
        easeInOutExpo,

        easeInCirc,
        easeOutCirc,
        easeInOutCirc,

        linear,

        spring,

        /* GFX47 MOD START */
        //bounce,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        /* GFX47 MOD END */

        easeInBack,
        easeOutBack,
        easeInOutBack,

        /* GFX47 MOD START */
        //elastic,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        /* GFX47 MOD END */

        punch,
        none
    }


    //instantiates a cached ease equation refrence:
    private static EasingFunction GetEasingFunction(EaseType easeType)
    {
        switch (easeType)
        {
            case EaseType.easeInQuad:
                return new EasingFunction(easeInQuad);
            case EaseType.easeOutQuad:
                return new EasingFunction(easeOutQuad);
            case EaseType.easeInOutQuad:
                return new EasingFunction(easeInOutQuad);
            case EaseType.easeInCubic:
                return new EasingFunction(easeInCubic);
            case EaseType.easeOutCubic:
                return new EasingFunction(easeOutCubic);
            case EaseType.easeInOutCubic:
                return new EasingFunction(easeInOutCubic);
            case EaseType.easeInQuart:
                return new EasingFunction(easeInQuart);
            case EaseType.easeOutQuart:
                return new EasingFunction(easeOutQuart);
            case EaseType.easeInOutQuart:
                return new EasingFunction(easeInOutQuart);
            case EaseType.easeInQuint:
                return new EasingFunction(easeInQuint);
            case EaseType.easeOutQuint:
                return new EasingFunction(easeOutQuint);
            case EaseType.easeInOutQuint:
                return new EasingFunction(easeInOutQuint);
            case EaseType.easeInSine:
                return new EasingFunction(easeInSine);
            case EaseType.easeOutSine:
                return new EasingFunction(easeOutSine);
            case EaseType.easeInOutSine:
                return new EasingFunction(easeInOutSine);
            case EaseType.easeInExpo:
                return new EasingFunction(easeInExpo);
            case EaseType.easeOutExpo:
                return new EasingFunction(easeOutExpo);
            case EaseType.easeInOutExpo:
                return new EasingFunction(easeInOutExpo);
            case EaseType.easeInCirc:
                return new EasingFunction(easeInCirc);
            case EaseType.easeOutCirc:
                return new EasingFunction(easeOutCirc);
            case EaseType.easeInOutCirc:
                return new EasingFunction(easeInOutCirc);
            case EaseType.linear:
                return new EasingFunction(linear);
            case EaseType.spring:
                return new EasingFunction(spring);
            /* GFX47 MOD START */
            /*case EaseType.bounce:
                return new EasingFunction(bounce);
                break;*/
            case EaseType.easeInBounce:
                return new EasingFunction(easeInBounce);
            case EaseType.easeOutBounce:
                return new EasingFunction(easeOutBounce);
            case EaseType.easeInOutBounce:
                return new EasingFunction(easeInOutBounce);
            /* GFX47 MOD END */
            case EaseType.easeInBack:
                return new EasingFunction(easeInBack);
            case EaseType.easeOutBack:
                return new EasingFunction(easeOutBack);
            case EaseType.easeInOutBack:
                return new EasingFunction(easeInOutBack);
            /* GFX47 MOD START */
            /*case EaseType.elastic:
                return new EasingFunction(elastic);
                break;*/
            case EaseType.easeInElastic:
                return new EasingFunction(easeInElastic);
            case EaseType.easeOutElastic:
                return new EasingFunction(easeOutElastic);
            case EaseType.easeInOutElastic:
                return new EasingFunction(easeInOutElastic);
            /* GFX47 MOD END */
            case EaseType.none:
                return new EasingFunction(none);
            default:
                return null;
        }
    }


    #region Easing Curves
    public static float none(float start, float end, float value)
    {
        return 0f;
    }

    public static float linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    public static float spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float easeInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float easeOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static float easeInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value + start;
        value--;
        return -end / 2 * (value * (value - 2) - 1) + start;
    }

    public static float easeInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float easeOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    public static float easeInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float easeOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    public static float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    public static float easeInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float easeOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    public static float easeInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value * value * value + 2) + start;
    }

    public static float easeInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
    }

    public static float easeOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
    }

    public static float easeInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
    }

    public static float easeInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
    }

    public static float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }

    public static float easeInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public static float easeInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    public static float easeOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    public static float easeInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    /* GFX47 MOD START */
    public static float easeInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - easeOutBounce(0, end, d - value) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float bounce(float start, float end, float value){
    public static float easeOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    public static float easeInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d / 2) return easeInBounce(0, end, value * 2) * 0.5f + start;
        else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }
    /* GFX47 MOD END */

    public static float easeInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    public static float easeOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value / 1) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    public static float easeInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    public static float punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    /* GFX47 MOD START */
    public static float easeInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float elastic(float start, float end, float value){
    public static float easeOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    /* GFX47 MOD START */
    public static float easeInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d / 2) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
    /* GFX47 MOD END */

    #endregion	
	


    public static Rect EaseRect(Rect start, Rect end, EasingCurves.EaseType inType, float t)
    {
        EasingFunction ease = GetEasingFunction(inType);
        return new Rect(
                   ease(start.xMin, end.xMin, t),
                   ease(start.yMin, end.yMin, t),
                   ease(start.width, end.width, t),
                   ease(start.height, end.height, t));
    }



    private class EasingRect
    {
        public Rect current;
        public Rect start;
        public Rect end;
        public EasingCurves.EaseType easeType;
        public float t;
        public bool ease = false;
        public float timeFactor = 1;
    }

    private static Dictionary<string, EasingRect> easingRects = new Dictionary<string, EasingRect>();
        
    public static Rect EaseRect(string id, Rect start, Rect end, EasingCurves.EaseType inType)
    {
        EasingRect rect;
        if (!easingRects.TryGetValue(id, out rect))
        {
            rect = new EasingRect();
            rect.start = start;
            rect.end = end;
            rect.easeType = inType;
            rect.t = 0f;
            rect.ease = true;
            easingRects[id] = rect;
        }

        if (rect.ease)
        {
            if (rect.t < 1)
            {
                //CurrentDragObject.t += GUIDeltaTime.deltaTimeGUI / CurrentDragObject.TimeFactor;
                rect.t += Time.deltaTime / rect.timeFactor;
            }
            //#endif
            rect.t = Mathf.Clamp01(rect.t);

            // calculate the Rectangle for parameter 
            rect.current = EasingCurves.EaseRect(
                rect.start,
                rect.end,
                rect.easeType,
                rect.t);

            // dragObject reached its targetPosition?
            if (rect.t == 1)
            {
                rect.ease = false;
                rect.t = 0;
            }
        }

        easingRects[id] = rect;
        return rect.current;
    }

}

