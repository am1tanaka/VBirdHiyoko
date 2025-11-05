using UnityEngine;
using UnityEngine.InputSystem;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko{
    public class DebugClearData : MonoBehaviour
    {
        DebugControls debugControls;
        bool isRightButton;

        void OnClearDataPerformed(InputAction.CallbackContext context) {
            if (context.ReadValueAsButton())
            {
                // タイトルシーンを起動
                if (RestartTitleSceneStateChanger.Instance.Request(false)) {
                    SEPlayer.Play(SEPlayer.SE.Start);
                }
                else{
                    SEPlayer.Play(SEPlayer.SE.Cancel);
                }
            }
        }

        void OnEnable() {
            debugControls?.Enable();
        }

        void OnDisable() {
            debugControls?.Disable();
        }

        private void OnDestroy()
        {
            debugControls = null;
        }

        void FixedUpdate()
        {
            if (debugControls == null){
                debugControls = new();
                debugControls.GamePad.ClearData.performed += OnClearDataPerformed;

                debugControls?.Enable();
            }            
        }
    }
}