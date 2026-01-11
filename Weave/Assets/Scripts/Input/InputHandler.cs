using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

namespace Weave.Controller
{
	public class InputHandler : RetrievableResourceSingleton<InputHandler>
	{
		public PlayerInput PlayerInput
		{
			get
			{
				return this._playerInput;
			}
		}

		public static string GetCurrentControlScheme()
		{
			return RetrievableResourceSingleton<InputHandler>.Instance._playerInput.currentControlScheme;
		}

		public static InputScheme GetCurrentUsedInputScheme()
		{
			return RetrievableResourceSingleton<InputHandler>.Instance._currentControlScheme;
		}

		public static GamepadType GetGamepadType()
		{
			return RetrievableResourceSingleton<InputHandler>.Instance._gamepadType;
		}

		public static bool GamepadUsesHaptics()
		{
			return InputHandler.GetGamepadType() == GamepadType.Dualsense || InputHandler.GetGamepadType() == GamepadType.SteamDeck;
		}

		protected override void OnCreated()
		{
			base.OnCreated();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.Default = InputSystem.actions.FindActionMap("Default", false);
			this._playerInput = base.GetComponent<PlayerInput>();
			Debug.Log("Initialized InputHandler");
		}

		public void Initialize(Func<bool> isRunningOnSteamDeck = null, Func<bool> isGameInputAllowed = null)
		{
			InputHandler._isRunningOnSteamDeck = isRunningOnSteamDeck;
			InputHandler._isGameInputAllowed = isGameInputAllowed;
		}

		private void Update()
		{
			InputScheme inputScheme = InputHandler.ToInputScheme(this._playerInput.currentControlScheme);
			GamepadType gamepadType = InputHandler.FindGamepadType(this._playerInput.devices);
			if (this._currentControlScheme != inputScheme || this._gamepadType != gamepadType)
			{
				this._currentControlScheme = inputScheme;
				this._gamepadType = gamepadType;
				Debug.Log(string.Format("Control scheme changed to {0} {1}", this._currentControlScheme, (this._currentControlScheme == InputScheme.Gamepad) ? string.Format("(gamepad type: {0})", this._gamepadType) : ""));
				Action<InputScheme> inputSchemeChanged = this.InputSchemeChanged;
				if (inputSchemeChanged != null)
				{
					inputSchemeChanged(this._currentControlScheme);
				}
			}
			if (EventSystem.current && EventSystem.current.currentSelectedGameObject && !EventSystem.current.currentSelectedGameObject.activeInHierarchy && !EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			if (this.Default != null)
			{
				if (InputHandler._isGameInputAllowed == null || InputHandler._isGameInputAllowed())
				{
					this.Default.Enable();
					return;
				}
				this.Default.Disable();
			}
		}

		public static bool ShouldSetSelectedGameObject()
		{
			return InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad && EventSystem.current && !EventSystem.current.currentSelectedGameObject && !EventSystem.current.alreadySelecting;
		}

		private static GamepadType FindGamepadType(ReadOnlyArray<InputDevice> devices)
		{
			foreach (InputDevice device in devices)
			{
				GamepadType gamepadType = InputHandler.FindGamepadType(device);
				if (gamepadType != GamepadType.Unkown)
				{
					return gamepadType;
				}
			}
			return GamepadType.Unkown;
		}

		public static GamepadType FindGamepadType(InputDevice device)
		{
			if (InputHandler._isRunningOnSteamDeck != null && InputHandler._isRunningOnSteamDeck())
			{
				return GamepadType.SteamDeck;
			}
			if (device is XInputController)
			{
				return GamepadType.Xbox;
			}
			if (device is DualSenseGamepadHID)
			{
				return GamepadType.Dualsense;
			}
			if (device is DualShockGamepad)
			{
				return GamepadType.Dualshock;
			}
			return GamepadType.Unkown;
		}

		private static InputScheme ToInputScheme(string currentControlScheme)
		{
			InputScheme result;
			if (!(currentControlScheme == "Keyboard&Mouse"))
			{
				if (!(currentControlScheme == "Gamepad"))
				{
					result = InputScheme.Unknown;
				}
				else
				{
					result = InputScheme.Gamepad;
				}
			}
			else
			{
				result = InputScheme.KeyboardMouse;
			}
			return result;
		}

		private PlayerInput _playerInput;

		private static Func<bool> _isRunningOnSteamDeck;

		private static Func<bool> _isGameInputAllowed;

		public Action<InputScheme> InputSchemeChanged;

		[NonSerialized]
		public InputActionMap Default;

		private InputScheme _currentControlScheme;

		private GamepadType _gamepadType;
	}
}
