using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AugaUnity
{
    public class AugaBindingDisplay : MonoBehaviour
    {
        public string AutomaticKeyName;

        public Text KeybindText;
        public GameObject KeybindBox;
        public Text LongKeybindText;
        public GameObject LongKeybindBox;
        public GameObject Mouse1;
        public GameObject Mouse2;
        public GameObject Mouse3;
        public GameObject MouseX;
        public Text MouseXText;

        public void Update()
        {
            if (!string.IsNullOrEmpty(AutomaticKeyName))
            {
                SetBinding(AutomaticKeyName);
            }
        }

        public void SetBinding(string keyName)
        {
            if (!ZInput.instance.m_buttons.ContainsKey(keyName))
            {
                Debug.LogError($"[AugaBindingDisplay.SetBinding] Couldn't find key: {keyName}");
                return;
            }

            var key = ZInput.instance.m_buttons[keyName].m_key; 
            var localizedKeyString = Localization.instance.GetBoundKeyString(keyName);
            
            var showMouse = -1;
            /* JKA - disabled because keycode vs key is bonkers and not going to use this anyhow.
            switch (key)
            {
                case KeyCode.Mouse0: showMouse = 0; break;
                case KeyCode.Mouse1: showMouse = 1; break;
                case KeyCode.Mouse2: showMouse = 2; break;
                case KeyCode.Mouse3: showMouse = 3; break;
                case KeyCode.Mouse4: showMouse = 4; break;
                case KeyCode.Mouse5: showMouse = 5; break;
                case KeyCode.Mouse6: showMouse = 6; break;
            }
            */
            
            switch (localizedKeyString)
            {
                case "Equals": localizedKeyString = "="; break;
                case "BackQuote": localizedKeyString = "`"; break;
            }

            if (localizedKeyString.StartsWith("Keypad"))
            {
                localizedKeyString = localizedKeyString.Replace("Keypad", "Num");
            }
            else if (localizedKeyString.StartsWith("Alpha"))
            {
                localizedKeyString = localizedKeyString.Replace("Alpha", "");
            }

            switch (key)
            {
                case Key.NumpadDivide: localizedKeyString = localizedKeyString.Replace("Divide", "/"); break;
                case Key.NumpadMinus: localizedKeyString = localizedKeyString.Replace("Minus", "-"); break;
                case Key.NumpadMultiply: localizedKeyString = localizedKeyString.Replace("Multiply", "*"); break;
                case Key.NumpadEquals: localizedKeyString = localizedKeyString.Replace("Equals", "="); break;
                case Key.NumpadPeriod: localizedKeyString = localizedKeyString.Replace("Period", "."); break;
                case Key.NumpadPlus: localizedKeyString = localizedKeyString.Replace("Plus", "+"); break;

                case Key.LeftArrow: localizedKeyString = "←"; break;
                case Key.RightArrow: localizedKeyString = "→"; break;
                case Key.UpArrow: localizedKeyString = "↑"; break;
                case Key.DownArrow: localizedKeyString = "↓"; break;
            }

            if (char.IsPunctuation((char)key))
            {
                localizedKeyString = ((char)key).ToString();
            }

            SetText(localizedKeyString, showMouse);
        }

        public void SetText(string localizedKeyString, int showMouse = -1)
        {
            var isOneCharLong = localizedKeyString.Length == 1;
            (isOneCharLong ? KeybindText : LongKeybindText).text = localizedKeyString;
            KeybindBox.SetActive(showMouse < 0 && isOneCharLong);
            LongKeybindBox.SetActive(showMouse < 0 && !isOneCharLong);

            Mouse1.SetActive(showMouse == 0);
            Mouse2.SetActive(showMouse == 1);
            Mouse3.SetActive(showMouse == 2);
            MouseX.SetActive(showMouse > 2);
            MouseXText.text = (showMouse + 1).ToString();
        }
    }
}
