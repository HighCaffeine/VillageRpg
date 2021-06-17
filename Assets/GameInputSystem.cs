// GENERATED AUTOMATICALLY FROM 'Assets/GameInputSystem.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInputSystem : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputSystem"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""8e284373-3ec2-4d7a-b54a-c1cd536e3c9e"",
            ""actions"": [
                {
                    ""name"": ""CameraMove"",
                    ""type"": ""Value"",
                    ""id"": ""57a14d24-3576-4896-9be6-cba4ae4c0840"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Touch"",
                    ""type"": ""Button"",
                    ""id"": ""d3321613-ec89-43db-8576-bab3636d80f4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChooseBuilding"",
                    ""type"": ""Value"",
                    ""id"": ""4b07f185-bb12-4143-b1c0-a0647aaa072f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fe1dc8f4-d382-4c5f-be15-e95dce62e03a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a37f96d-1eb7-4a1a-859a-1952351ed389"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""Touch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5cbd32ed-5674-4a75-a8bc-9bd930d88930"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""ChooseBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse"",
            ""bindingGroup"": ""Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_CameraMove = m_Mouse.FindAction("CameraMove", throwIfNotFound: true);
        m_Mouse_Touch = m_Mouse.FindAction("Touch", throwIfNotFound: true);
        m_Mouse_ChooseBuilding = m_Mouse.FindAction("ChooseBuilding", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_CameraMove;
    private readonly InputAction m_Mouse_Touch;
    private readonly InputAction m_Mouse_ChooseBuilding;
    public struct MouseActions
    {
        private @GameInputSystem m_Wrapper;
        public MouseActions(@GameInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraMove => m_Wrapper.m_Mouse_CameraMove;
        public InputAction @Touch => m_Wrapper.m_Mouse_Touch;
        public InputAction @ChooseBuilding => m_Wrapper.m_Mouse_ChooseBuilding;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @CameraMove.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnCameraMove;
                @CameraMove.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnCameraMove;
                @CameraMove.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnCameraMove;
                @Touch.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouch;
                @Touch.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouch;
                @Touch.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouch;
                @ChooseBuilding.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnChooseBuilding;
                @ChooseBuilding.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnChooseBuilding;
                @ChooseBuilding.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnChooseBuilding;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CameraMove.started += instance.OnCameraMove;
                @CameraMove.performed += instance.OnCameraMove;
                @CameraMove.canceled += instance.OnCameraMove;
                @Touch.started += instance.OnTouch;
                @Touch.performed += instance.OnTouch;
                @Touch.canceled += instance.OnTouch;
                @ChooseBuilding.started += instance.OnChooseBuilding;
                @ChooseBuilding.performed += instance.OnChooseBuilding;
                @ChooseBuilding.canceled += instance.OnChooseBuilding;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    private int m_MouseSchemeIndex = -1;
    public InputControlScheme MouseScheme
    {
        get
        {
            if (m_MouseSchemeIndex == -1) m_MouseSchemeIndex = asset.FindControlSchemeIndex("Mouse");
            return asset.controlSchemes[m_MouseSchemeIndex];
        }
    }
    public interface IMouseActions
    {
        void OnCameraMove(InputAction.CallbackContext context);
        void OnTouch(InputAction.CallbackContext context);
        void OnChooseBuilding(InputAction.CallbackContext context);
    }
}
