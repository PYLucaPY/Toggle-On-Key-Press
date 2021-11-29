using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[System.Serializable]
public class ToggleEvent
{
    public string toggleEventName;
    private static bool ReturnValue;

    [System.Serializable]
    public class ToggleParamCallbackEvent : UnityEvent {}

    [System.Serializable]
    public class ToggleParam
    {
        /// <param name="onToggle">
        /// Unity Event that is used as a parameter for "OnKeyToggle".
        /// </param>
        [Tooltip("Unity Event that is used as a parameter for \"OnKeyToggle\".")]
        public ToggleParamCallbackEvent onToggle;

        /// <param name="usesOptionalToggleParameter">
        /// Decides whether Toggle Param is to be used.
        /// </param>
        [Tooltip("Decides whether Toggle Param is to be used.")]
        public bool usesOptionalToggleParameter;

        /// <param name="setOnFalse">
        /// Will set target gameObjects to unactive if "onToggle" is false.
        /// </param>
        [Tooltip("Will set target gameObjects to unactive if \"onToggle\" is false.")]
        public bool setOnFalse;

        /// <param name="setOnTrue">
        /// Will set target gameObjects to active if "onToggle" is true.
        /// </param>
        [Tooltip("Will set target gameObjects to active if \"onToggle\" is true.")]
        public bool setOnTrue;
    }

    [Header("Functional Params")]
    public ToggleParam toggleParam;

    [Header("Adjustables")]
    public bool isToggled = false;
    public bool isCommand = false;

    public List<KeyCode> commandKeys = new List<KeyCode>() {
        KeyCode.LeftControl
    };

    public KeyCode triggerKey;
    public GameObject[] targetGameObjects;

    public bool GetToggleValue()
    {
        toggleParam.onToggle.Invoke();
        return ReturnValue;
    }

    public static void Return(bool value) {
        ReturnValue = value;
    }
}

public class ToggleOnKeyPress : MonoBehaviour
{
    public ToggleEvent[] toggleEvents;

    void Update()
    {
        UpdateTriggers();
    }

    void UpdateTriggers()
    {
        foreach(ToggleEvent toggleEvent in toggleEvents) {
            bool triggerPressed = Input.GetKeyDown(toggleEvent.triggerKey);

            if(toggleEvent.isCommand) {
                bool commandPressed = true;
                foreach(KeyCode commandKeybind in toggleEvent.commandKeys) {
                    if(!Input.GetKey(commandKeybind)) {
                        commandPressed = false;
                    }
                }

                triggerPressed = commandPressed ? triggerPressed : false;
            }

            if(toggleEvent.toggleParam.usesOptionalToggleParameter) {
                bool paramValue = toggleEvent.GetToggleValue();

                if(!paramValue) {
                    triggerPressed = false;
                }

                if(toggleEvent.toggleParam.setOnTrue && paramValue) {
                    toggleEvent.isToggled = !paramValue;
                    triggerPressed = true;
                }
                if(toggleEvent.toggleParam.setOnFalse && !paramValue) {
                    toggleEvent.isToggled = !paramValue;
                    triggerPressed = true;
                }
            }

            if(triggerPressed) {
                toggleEvent.isToggled = !toggleEvent.isToggled;

                foreach(GameObject targetObject in toggleEvent.targetGameObjects) {
                    targetObject.SetActive(toggleEvent.isToggled);
                }
            }
        }
    }
}
