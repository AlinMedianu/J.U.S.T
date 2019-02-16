using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoGamepadMenuSelect : MonoBehaviour {

    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;

    private void Update()
    {
        if (!IsInputFieldSelected() && Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
    }

    private void OnDisable()
    {
        buttonSelected = false;
    }

    private bool IsInputFieldSelected() // Without this check, the keys a and d will make the selection move when typing into input fields.
    {
        bool inputFieldSelected = false;
        List<Selectable> inputs = Selectable.allSelectables; // Get a list of all selectable input objects active in the scene.
        foreach ( Selectable s in inputs)
        {
            InputField inputField = s as InputField; // Attempt to cast into inputfield type... produces null if failed.
            if (inputField != null && inputField.isFocused)
            {
                inputFieldSelected = true;
            }
        }
        return inputFieldSelected;
    }

}
