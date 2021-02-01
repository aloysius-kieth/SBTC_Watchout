using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using System.Linq;

public class TrinaxOnScreenKB : MonoBehaviour
{
    const int MAX_MOBILE_NO = 8;
    const int MAX_NAME_CHAR = 12;

    public static TrinaxOnScreenKB Instance;

    public TMP_InputField[] inputFields;
    public Color activeColor;
    public Color inactiveColor;

    TMP_InputField activeField;
    HashSet<int> targetedInputFields = new HashSet<int>();
    int caretPosition;
    int anchorPosition;
    public void OnKeyDown(KeyCode code, string custom = "")
    {
        if (activeField != null)
        {
            string newChar = "";
            string current = activeField.text;

            if (custom.ToUpper() == "CLEAR")
            {
                activeField.text = "";
                return;
            }

            if (code == KeyCode.Backspace)
            {
                if (current.Length == 0)
                {
                    return;
                }
                //activeField.caretPosition;
                //Debug.Log(caretPosition);
                //Debug.Log(current);
                /*
                if(activeField.onFocusSelectAll)
                {
                    Debug.Log("onFocusSelectAll");
                    activeField.text = "";
                    caretPosition = 0;
                    return;
                }
                *
                /*if (activeField.selectionStringFocusPosition)
                {
                    //Debug.Log("selectionStringFocusPosition");
                    activeField.text = "";
                    caretPosition = 0;
                    return;
                }*/
                int startPos = 0;
                int numOfChar = 0;
                if (caretPosition > anchorPosition)
                {
                    startPos = anchorPosition;
                    numOfChar = caretPosition - startPos;
                    activeField.text = current.Remove(startPos, numOfChar);
                    anchorPosition = caretPosition = startPos;
                } else if (anchorPosition > caretPosition)
                {
                    startPos = caretPosition;
                    numOfChar = anchorPosition - startPos;
                    activeField.text = current.Remove(startPos, numOfChar);
                    anchorPosition = caretPosition = startPos;
                }
                else if (caretPosition == anchorPosition && caretPosition - 1 >= 0)
                {
                    Debug.Log(caretPosition);
                    startPos = caretPosition;
                    numOfChar = 1;
                    activeField.text = current.Remove(startPos - 1, numOfChar);
                    --caretPosition;
                    --anchorPosition;
                }

                //Debug.Log(current);
                //current = current.Substring(0, current.Length - 1);
                return;
            }

            if (!string.IsNullOrEmpty(custom))
            {
                newChar = custom;
            }
            else
            {
                switch (code)
                {
                    case KeyCode.Alpha0:
                        newChar = "0";
                        break;
                    case KeyCode.Alpha1:
                        newChar = "1";
                        break;
                    case KeyCode.Alpha2:
                        newChar = "2";
                        break;
                    case KeyCode.Alpha3:
                        newChar = "3";
                        break;
                    case KeyCode.Alpha4:
                        newChar = "4";
                        break;
                    case KeyCode.Alpha5:
                        newChar = "5";
                        break;
                    case KeyCode.Alpha6:
                        newChar = "6";
                        break;
                    case KeyCode.Alpha7:
                        newChar = "7";
                        break;
                    case KeyCode.Alpha8:
                        newChar = "8";
                        break;
                    case KeyCode.Alpha9:
                        newChar = "9";
                        break;
                    case KeyCode.Space:
                        newChar = " ";
                        break;
                    case KeyCode.At:
                        newChar = "@";
                        break;
                    case KeyCode.Period:
                        newChar = ".";
                        break;
                    case KeyCode.Minus:
                        newChar = "-";
                        break;
                    case KeyCode.Underscore:
                        newChar = "_";
                        break;
                    default:
                        newChar = code.ToString();

                        if (string.IsNullOrEmpty(current))
                        {
                            newChar = newChar.ToUpper();
                        }
                        else
                        {
                            char lastChar = current[current.Length - 1];
                            if (char.IsWhiteSpace(lastChar))
                            {
                                newChar = newChar.ToUpper();
                            }
                            else
                            {
                                newChar = newChar.ToLower();
                            }
                        }

                        break;
                }
            }

            if (activeField.CompareTag("Name"))
            {
                if (activeField.text.Length >= MAX_NAME_CHAR)
                {
                    if (Mathf.Abs(caretPosition - anchorPosition) == 0)
                        return;
                }
            }

            if (activeField.CompareTag("ContactNumber"))
            {
                if(activeField.text.Length >= MAX_MOBILE_NO)
                {
                    if(Mathf.Abs(caretPosition - anchorPosition) == 0)
                        return;
                }
                   
                if (newChar.ToLower().Contains('a') ||
                   newChar.ToLower().Contains('b') ||
                   newChar.ToLower().Contains('c') ||
                   newChar.ToLower().Contains('d') ||
                   newChar.ToLower().Contains('e') ||
                   newChar.ToLower().Contains('f') ||
                   newChar.ToLower().Contains('g') ||
                   newChar.ToLower().Contains('h') ||
                   newChar.ToLower().Contains('i') ||
                   newChar.ToLower().Contains('j') ||
                   newChar.ToLower().Contains('k') ||
                   newChar.ToLower().Contains('l') ||
                   newChar.ToLower().Contains('m') ||
                   newChar.ToLower().Contains('n') ||
                   newChar.ToLower().Contains('o') ||
                   newChar.ToLower().Contains('p') ||
                   newChar.ToLower().Contains('q') ||
                   newChar.ToLower().Contains('r') ||
                   newChar.ToLower().Contains('s') ||
                   newChar.ToLower().Contains('t') ||
                   newChar.ToLower().Contains('u') ||
                   newChar.ToLower().Contains('v') ||
                   newChar.ToLower().Contains('w') ||
                   newChar.ToLower().Contains('x') ||
                   newChar.ToLower().Contains('y') ||
                   newChar.ToLower().Contains('z'))
                {
                    return;
                }
            }
            //Debug.Log(caretPosition);
            caretPosition = Mathf.Clamp(caretPosition, 0, activeField.text.Length);
            //Debug.Log(caretPosition);
            //Debug.Log(anchorPosition);
            //Debug.Log(activeField.text.Length);
            if (caretPosition == anchorPosition)
            {
                string substring1 = (caretPosition > 0) ? activeField.text.Substring(0, caretPosition) : "";
                //Debug.Log(substring1);
                string substring2 = (caretPosition < activeField.text.Length) ?
                    activeField.text.Substring(caretPosition, activeField.text.Length - caretPosition) : "";
                //Debug.Log(substring2);
                activeField.text = substring1 + newChar + substring2;
                ++caretPosition;
                ++anchorPosition;
            } else
            {
                int startPos = 0;
                int endPos = 0;
                if (caretPosition > anchorPosition)
                {
                    startPos = anchorPosition;
                    endPos = caretPosition;
                }
                else if (anchorPosition > caretPosition)
                {
                    startPos = caretPosition;
                    endPos = anchorPosition;
                }
                string substring1 = (startPos > 0) ? activeField.text.Substring(0, startPos) : "";
                string substring2 = (endPos < activeField.text.Length) ?
                    activeField.text.Substring(endPos, activeField.text.Length - endPos) : "";

                activeField.text = substring1 + newChar + substring2;
                anchorPosition =  caretPosition = ++startPos;
            }
        }
    }

    public void ActivateFirstInputField()
    {
        if (inputFields != null/* && inputFields.Length > 1*/)
        {
            inputFields[0].ActivateInputField();
            activeField = inputFields[0];
        }

        ColorActiveInputField();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < inputFields.Length; ++i)
        {
            targetedInputFields.Add(inputFields[i].GetHashCode());
        }

        ActivateFirstInputField();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (EventSystem.current.currentSelectedGameObject)
        {
            TMP_InputField input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (input != null)
            {
                if (targetedInputFields.Contains(input.GetHashCode()))
                {
                    activeField = input;

                    //Debug.Log(activeField.isFocused + " " + activeField.caretPosition);
                    if (activeField.isFocused) {
                        //Debug.Log(activeField.selectionFocusPosition);
                        //Debug.Log(activeField.caretWidth);
                        //Debug.Log(activeField.selectionAnchorPosition);
                        caretPosition = activeField.caretPosition;
                        anchorPosition = activeField.selectionAnchorPosition;
                    }
                    //Debug.Log(activeField.caretPosition);
                    ColorActiveInputField();
                }
                else
                {
                    // clicked on not a targeted input field.
                }
            }
            else
            {
                for (int i = 0; i < inputFields.Length; ++i)
                {
                    //inputFields[i].interactable = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        ActivateFirstInputField();
    }

    void ColorActiveInputField()
    {
        for (int i = 0; i < inputFields.Length; ++i)
        {
            inputFields[i].image.color = inactiveColor;
            //inputFields[i].interactable = true;
        }

        if (activeField != null)
        {
            activeField.image.color = activeColor;
            //activeField.interactable = false;
        }

    }
}
