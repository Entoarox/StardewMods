namespace Entoarox.Framework.Reflection
{
    public static class ReflectionUtility
    {
        public static T GetField<T>(object target, string field)
        {
            return new ReflectedField<T>(target, field).GetValue();
        }
        public static T GetProperty<T>(object target, string property)
        {
            return new ReflectedProperty<T>(target, property).GetValue();
        }
        public static void InvokeMethod(object target, string method, params object[] arguments)
        {
            new ReflectedMethod(target, method).Invoke(arguments);
        }
        public static T InvokeMethod<T>(object target, string method, params object[] arguments)
        {
            return new ReflectedMethod<T>(target, method).Invoke(arguments);
        }
        public static void SetField<T>(object target, string field, T value)
        {
            new ReflectedField<T>(target, field).SetValue(value);
        }
        public static void SetProperty<T>(object target, string property, T value)
        {
            new ReflectedProperty<T>(target, property).SetValue(value);
        }
    }
}
