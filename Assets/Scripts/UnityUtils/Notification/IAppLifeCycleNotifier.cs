using System;

namespace UnityUtils.Notification
{
    public interface IAppLifeCycleNotifier
    {
        // Fires when the application loses or gains focus.
        // [Desktop] Alt-tabbing or Cmd-tabbing can take focus away from the Unity application to another desktop application.
        // This fires event with False. When the user switches back to the Unity application, the event fires with True.
        // [Android] fires False, when the on-screen keyboard is enabled. If "Home" pressed during keyboard is enabled this event is not called
        event AppFocusChangedEventHandler ApplicationFocusChanged;
        
        // Event with True means that the game is not active.
        // [Editor] fire False when the game is running normally in the editor. If an editor window such as the Inspector is chosen the game is paused
        // and event fires True. When the game window is selected and active the event fires False. 
        // [Android] fires True, when "Home" button is pressed at the moment the keyboard is enabled
        event AppPauseStateChangedEventHandler ApplicationPauseStateChanged;
        
        // [Editor] Fires when playmode is stopped.
        // [Mobile] If the user suspends your application, the operating system can quit the application to free up resources.
        // In this case, depending on the operating system, Unity might be unable to fire this event.
        // On mobile platforms, it is best practice to not rely on this method to save the state of your application.
        // Instead, consider every loss of application focus as the exit of the application and use ApplicationFocusChanged event to save any data.
        // [iOS] enable the "Exit on Suspend" property in Player Settings to make the application quit and not suspend, otherwise this event do not fires.
        // If you do not enable the "Exit on Suspend" property then listen to ApplicationPauseStateChanged event.
        event Action ApplicationQuitting;

        bool HasFocus { get; }
        bool IsPaused { get; }
        bool IsQuitting { get; }

        delegate void AppFocusChangedEventHandler(bool hasFocus);
        delegate void AppPauseStateChangedEventHandler(bool isPaused);
    }
}