using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;

public class Vavi : MonoBehaviour
{

    /* READ ME
        
       AUTHOR: Eric Vander Horst
      
       PURPOSE:
      The purpose of this script is to control a UI image in order to depict a value,
      with enough customizability that it could be used in virtually any project.


       HOW TO USE:
      
      
       ALTERING THE SCRIPT:
      If you're working within earshot of me (Eric) please ask me before you change this script or object,
      as the functionality you want to put in might already be here, or it may mess with other UI elements
      using this script. Thanks!

       TROUBLESHOOTING:
      
      The positive image won't show!
       *Ensure that the positive image appears in front of the negative image. This can be done by putting the positive image object below the negative image object in the inspector.
      
      The image won't update to show the new value when it changes!
       *The image only updates when the Display() function is called. Either call that function yourself when you change the value, or use ValueVisualizer.ValueSet(float f) to change the value.
      
      
     */

    #region Variable Declaration




    public enum DisplayType
    {
        fill,
        tiled,
        size,
        animation,
        //shader,
        simple
    }

    [Header("Display Type")]

    public DisplayType displayType;

    [System.Serializable]
    public class DisplayTypeOptions
    {
        [Header("Tiled")]
        public bool fillCentre;
        public bool vertical;

        [Header("Size")]
        public Vector2 sizeMaxScale;
        public Vector2 sizeMinScale;

        [Header("Fill")]
        public Image.FillMethod fillMethod;
        public int fillOrigin;
        public bool fillClockwise;

        [Header("Animation")]

        public AnimationClip animClip;
        public List<Sprite> animationSprites;

    }

    [SerializeField]
    DisplayTypeOptions displayTypeOptions;






    [Header("Value Handling")]

    public string valueName;
    public string valueUnits;
    public float value;
    public float extraValue;
    float valueFiltered;
    public float valueMax;
    public float valueMin;

    [System.Serializable]
    public class ValueHandlingOptions
    {
        public bool clampMaxAmount;
        public bool clampMinAmount;
        public bool roundFloats;
        public bool warnAtFirstEmpty;
        public float valueAfterWarn;
        [HideInInspector]
        public bool warned = false;
    }

    [SerializeField]
    ValueHandlingOptions valueHandlingOptions;

    [Header("Text")]

    [SerializeField] bool showName;
    [SerializeField] bool showValue;
    [SerializeField] bool showOverMax;
    [SerializeField] bool showPercent;
    [SerializeField] bool showUnits;
    [SerializeField] int valueDecimalPlaces;

    [SerializeField] List<string> atValueOverrideTexts;
    [SerializeField] bool showOverrideText;

    //Color Options
    [Header("Color")]
    [SerializeField] bool useDepletionColors = false;
    [Tooltip("The positive image's color will change depending on value / valueMax")]
    [SerializeField] Gradient depletionColors;

    [Header("Juice")]

    [SerializeField] AnimationCurve animateChange;
    [SerializeField] float animationTime;
    [SerializeField] float animationframePerSecond;
    public float valueChange;
    [SerializeField] UnityEvent onDisplay;
    [SerializeField] UnityEvent onAnimate;






    [Header("Setup")]

    //On-Screen Visuals
    [SerializeField]
    [Tooltip("The part of the image that shows how much you have out of the total")]
    Image positiveImage;

    [SerializeField]
    [Tooltip("The part of the image that shows how much is missing from the total")]
    Image negativeImage;

    [SerializeField]
    [Tooltip("An extra image that can display additional positive information comparatively")]
    Image extraImage;

    [SerializeField]
    [Tooltip("The text object that will display information about the value")]
    Text text;

    //Sprite Sizes
    //(used to regulate onscreen size)
    Vector2 psSize;
    Vector2 exSize;
    Vector2 nsSize;
    public int vaviIndex;
    [Tooltip("Whether the image will update or not outside of play mode")]
    [SerializeField]
    bool testMode;

    #endregion

    void GizmoDisplay()
    {
        GatherSpriteStats();

        SetUpImages();

        Display();
    }

    void Start()
    {

        if (onDisplay == null)
            onDisplay = new UnityEvent();

        if (onAnimate == null)
            onAnimate = new UnityEvent();

        GatherSpriteStats();

        SetUpImages();

        Display();

        //assures that the anchor points of the images are at their left-most side rather than their centres.
        //This way, rescaling the images drags them from left-to-right rather than from centre-outwards.
        negativeImage.rectTransform.pivot = new Vector2(0, 0);
        positiveImage.rectTransform.pivot = new Vector2(0, 0);
        extraImage.rectTransform.pivot = new Vector2(0, 0);

        valueHandlingOptions.warned = false;

    }

    private void Update()
    {
        //if (testMode) Display();
    }

    private void OnDrawGizmos()
    {
        //this updates the image after something was changed in Unity's inspector window.
        if (testMode)
            Invoke("GizmoDisplay", 0.1f);
    }

    void GatherSpriteStats()
    {
        //Gather Sprite Stats
        psSize = positiveImage.sprite.bounds.size;
        if (negativeImage)
        {
            //size is found in units, not pixels
            nsSize = negativeImage.sprite.bounds.size;
        }
        if (extraImage)
        {
            //size is found in units, not pixels
            exSize = extraImage.sprite.bounds.size;
        }
    }

    void SetUpImages()
    {
        switch (displayType)
        {
            case DisplayType.simple:

                positiveImage.type = Image.Type.Simple;
                positiveImage.rectTransform.sizeDelta = new Vector2(100 * nsSize.x, 100 * nsSize.y);
                negativeImage.type = Image.Type.Simple;
                negativeImage.rectTransform.sizeDelta = new Vector2(100 * nsSize.x, 100 * nsSize.y);

                break;

            case DisplayType.tiled:

                positiveImage.type = Image.Type.Tiled;

                break;

            case DisplayType.size:

                positiveImage.type = Image.Type.Simple;

                break;

            case DisplayType.fill:

                positiveImage.rectTransform.sizeDelta = new Vector2(100 * psSize.x, 100 * psSize.y);


                //change image type
                positiveImage.type = Image.Type.Filled;

                //change rect width/height according to values
                positiveImage.fillMethod = displayTypeOptions.fillMethod;
                positiveImage.fillOrigin = displayTypeOptions.fillOrigin;
                positiveImage.fillClockwise = displayTypeOptions.fillClockwise;

                if (negativeImage)
                {
                    negativeImage.rectTransform.sizeDelta = new Vector2(100 * nsSize.x, 100 * nsSize.y);
                    negativeImage.type = Image.Type.Simple;
                }

                if (extraImage)
                {
                    extraImage.rectTransform.sizeDelta = new Vector2(100 * exSize.x, 100 * exSize.y);

                    extraImage.type = Image.Type.Filled;
                    extraImage.fillMethod = displayTypeOptions.fillMethod;
                    extraImage.fillOrigin = displayTypeOptions.fillOrigin;
                    extraImage.fillClockwise = displayTypeOptions.fillClockwise;
                    extraImage.fillAmount = valueFiltered / valueMax;
                }

                break;

            case DisplayType.animation:
                break;

        }
    }



    public void Display()
    {

        onDisplay.Invoke();
        
        #region FilterValue
        //Filter Value

        valueFiltered = value;

        //keep below max
        if (valueHandlingOptions.clampMaxAmount)
        {
            if (valueFiltered > valueMax)
                valueFiltered = valueMax;
        }
        //keep above min
        if (valueHandlingOptions.clampMinAmount)
        {
            if (valueFiltered < valueMin)
                valueFiltered = valueMin;
        }
        //round to nearest int
        if (valueHandlingOptions.roundFloats)
        {
            valueFiltered = Mathf.Round(valueFiltered);
        }
        #endregion

        //Display

        switch (displayType)
        {
            case DisplayType.simple:
                break;
                
            case DisplayType.tiled:

                if (displayTypeOptions.vertical)
                {
                    positiveImage.rectTransform.sizeDelta = new Vector2(100 * psSize.x, valueFiltered * 100 * psSize.y);

                    //positiveImage.fillCenter = displayTypeOptions.fillCentre;

                    negativeImage.type = Image.Type.Tiled;
                    negativeImage.rectTransform.sizeDelta = new Vector2(100 * nsSize.x, valueMax * 100 * nsSize.y);
                }
                else
                {
                    //change rect width/height according to values
                    positiveImage.rectTransform.sizeDelta = new Vector2(valueFiltered * 100 * psSize.x, 100 * psSize.y);
                    //positiveImage.fillCenter = displayTypeOptions.fillCentre;

                    if (negativeImage)
                    {
                        negativeImage.type = Image.Type.Tiled;
                        negativeImage.rectTransform.sizeDelta = new Vector2(valueMax * 100 * nsSize.x, 100 * nsSize.y);
                    }
                }

                break;



            case DisplayType.size:
                
                positiveImage.rectTransform.sizeDelta = Vector2.Lerp(displayTypeOptions.sizeMinScale, displayTypeOptions.sizeMaxScale, valueFiltered / valueMax) * 100;
                negativeImage.rectTransform.sizeDelta = displayTypeOptions.sizeMaxScale * 100f;

                break;



            case DisplayType.fill:

                positiveImage.fillAmount = valueFiltered / valueMax;
                
                if (extraImage)
                {
                    extraImage.fillAmount = valueFiltered / valueMax;
                }

                break;

#if UNITY_EDITOR

            case DisplayType.animation:

                positiveImage.type = Image.Type.Simple;
                positiveImage.rectTransform.sizeDelta = new Vector2(100 * nsSize.x, 100 * nsSize.y);
                negativeImage.type = Image.Type.Simple;
                negativeImage.rectTransform.sizeDelta = new Vector2(0, 0);

                displayTypeOptions.animationSprites = GetSpritesFromClip(displayTypeOptions.animClip);
                positiveImage.sprite = displayTypeOptions.animationSprites[Mathf.Clamp(Mathf.RoundToInt(value / valueMax * displayTypeOptions.animationSprites.Count) - 1, 0, displayTypeOptions.animationSprites.Count - 1)];

                break;
#endif
                //case DisplayType.shader:

        }

        #region Text
        if (text)
        {

            text.text = "";

            if (showName)
            {
                text.text += valueName;

                if (showValue || showOverMax || showPercent || showUnits)
                {
                    text.text += ": ";
                }
            }
            if (showValue)
            {
                text.text += valueFiltered.ToString("F" + valueDecimalPlaces);
            }
            if (showOverMax)
            {
                text.text += " / " + valueMax;
            }
            if (showPercent)
            {
                text.text += ((valueFiltered / valueMax) * 100).ToString("F" + valueDecimalPlaces) + "%";
            }
            if (showUnits)
            {
                if (showValue || showOverMax || showPercent)
                {
                    text.text += " ";
                }
                text.text += valueUnits;
            }
            if (showOverrideText)
            {
                if (atValueOverrideTexts.Count > value)
                    if (atValueOverrideTexts[Mathf.RoundToInt(value)] != null)
                    {
                        text.text += atValueOverrideTexts[Mathf.RoundToInt(value)];
                    }

            }


        }
        #endregion

        #region Colors

        if (useDepletionColors)
        {
            positiveImage.color = depletionColors.Evaluate(valueFiltered / valueMax);
        }

        #endregion

    }



#if UNITY_EDITOR
    //Sourced from https://answers.unity.com/questions/1245599/how-to-get-all-sprites-used-in-a-2d-animator.html
    public static List<Sprite> GetSpritesFromAnimator(Animator anim)
    {
        List<Sprite> _allSprites = new List<Sprite>();
        foreach (AnimationClip ac in anim.runtimeAnimatorController.animationClips)
        {
            _allSprites.AddRange(GetSpritesFromClip(ac));
        }
        return _allSprites;
    }
#endif

#if UNITY_EDITOR
    //Sourced from https://answers.unity.com/questions/1245599/how-to-get-all-sprites-used-in-a-2d-animator.html
    private static List<Sprite> GetSpritesFromClip(AnimationClip clip)
    {
        var _sprites = new List<Sprite>();
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var frame in keyframes)
                {
                    _sprites.Add((Sprite)frame.value);
                }
            }
        }
        return _sprites;
    }
#endif


    public static Vavi GetVavi(int k)
    {
        Vavi[] vavis = FindObjectsOfType<Vavi>();
        Vavi target = null;
        for (int i = 0; i < vavis.Length; i++)
        {
            if (vavis[i].vaviIndex == k)
            {
                target = vavis[i];
                break;
            }
        }
        if (target)
            return target;
        else
        {
            print("Vavi not gotten");
            return null;
        }
    }

    public void ValueSet(float f)
    {
        if (valueHandlingOptions.clampMaxAmount)
        {
            value = Mathf.Clamp(f, valueMin, valueMax);
        }
        else
        {
            value = Mathf.Clamp(f, valueMin, Mathf.Infinity);
        }

        Display();
    }

    public void ExtraValueSet(float f)
    {
        if (valueHandlingOptions.clampMaxAmount)
        {
            extraValue = Mathf.Clamp(f, valueMin, valueMax);
        }
        else
        {
            extraValue = Mathf.Clamp(f, valueMin, Mathf.Infinity);
        }

        Display();
    }

    public void Show(bool show)
    {
        positiveImage.gameObject.SetActive(show);
        negativeImage.gameObject.SetActive(show);
        extraImage.gameObject.SetActive(show);
        text.gameObject.SetActive(show);
    }

    //start animation 
    public void ValueAnimate(float newValue)
    {
        valueChange = newValue - value;
        onAnimate.Invoke();
        StartCoroutine(ValueAnimateTime(value, newValue, Time.time + animationTime, animationTime / animationframePerSecond, animationTime));
    }

    #region Argument Functions

    //start animation, more arguments for customization
    public void ValueAnimate(float newValue, float time, int updatesPerTime)
    {
        valueChange = newValue - value;
        onAnimate.Invoke();
        StartCoroutine(ValueAnimateTime(value, newValue, Time.time + time, time / updatesPerTime, time));
    }

    //start animation, more arguments for customization
    public void ValueAnimate(float newValue, float time)
    {
        valueChange = newValue - value;
        onAnimate.Invoke();
        StartCoroutine(ValueAnimateTime(value, newValue, Time.time + time, time / animationframePerSecond, time));
    }

    //start animation, more arguments for customization
    public void ValueAnimate(float newValue, int updatesPerTime)
    {
        valueChange = newValue - value;
        onAnimate.Invoke();
        StartCoroutine(ValueAnimateTime(value, newValue, Time.time + animationTime, animationTime / updatesPerTime, animationTime));
    }

    #endregion

    //iterative animating
    IEnumerator ValueAnimateTime(float oldValue, float newValue, float killTime, float deltaTime, float time)
    {

        ValueSet(oldValue + (animateChange.Evaluate(time - (killTime - Time.time)) * (newValue - oldValue)));

        yield return new WaitForSeconds(deltaTime);

        if (Time.time < killTime)
        {
            StartCoroutine(ValueAnimateTime(oldValue, newValue, killTime, deltaTime, time));
        }
        else
        {
            ValueSet(newValue);
        }
    }
}