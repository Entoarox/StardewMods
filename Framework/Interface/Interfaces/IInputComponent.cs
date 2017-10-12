namespace Entoarox.Framework.Interface
{
    interface IInputComponent : IDynamicComponent
    {
        /// <summary>
        /// Input components will only receive input events while this value is true
        /// </summary>
        bool Selected { get; set; }
        /// <summary>
        /// Triggers when a new character is input by the user
        /// Conversion of keyboard input to the correct character values is handled by the framework
        /// If <see cref="Selected"/>, <see cref="IDynamicComponent.Enabled"/> or <see cref="IComponent.Visible"/> is false, this event will not trigger
        /// </summary>
        /// <param name="input">The input character</param>
        void ReceiveInput(char input);
    }
}
