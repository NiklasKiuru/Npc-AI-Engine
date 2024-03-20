using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aikom.AIEngine.Editor
{
    public class BranchPopup : EditorWindow
    {
        private static BranchContainer s_container;
        private static Action<string> s_onCreateEntryCb;

        internal static void Open(BranchContainer container, Action<string> onCreateEntrycallBack)
        {
            s_container = container;
            s_onCreateEntryCb = onCreateEntrycallBack;
            var popUp = CreateInstance<BranchPopup>();
            popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            popUp.ShowPopup();
        }

        private void CreateGUI()
        {
            var label = new TextField("Branch name") { value = "New Branch" };
            label.SelectAll();

            var confirmButton = new Button(() =>
            {   
                if(s_container == null)
                {
                    Close();
                    return;
                }

                if (!s_container.Templates.ContainsKey(label.value))
                    s_onCreateEntryCb.Invoke(label.value);
                else
                    Debug.LogWarning("Unable to create template. Branch collection already contains an item named: " + label.value);
                Close();
            });
            confirmButton.text = "Create branch";

            rootVisualElement.Add(label);
            rootVisualElement.Add(confirmButton);
        }
    }

}
