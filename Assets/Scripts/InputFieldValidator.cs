using UnityEngine;
using TMPro;

public class InputFieldValidator : MonoBehaviour
{
    TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.characterLimit = 3; // Limita o número máximo de caracteres
    }
}
