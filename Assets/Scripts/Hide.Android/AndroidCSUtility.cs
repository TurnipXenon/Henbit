#if UNITY_ANDROID
using UnityEngine;

namespace Hide.Android
{
    /// <summary>
    /// Utility class for Java-C# interaction
    /// </summary>
    public static class AndroidCSUtility
    {
        private const string JavaArrayType = "java.lang.reflect.Array";
        private const string JavaArrayFuncSet = "set";
        
        private const string JavaStringType = "java.lang.String";
        private const string JavaStringFuncNewInstance = "newInstance";
        
        /// <summary>Converts C# string[] to Java String[]</summary>
        /// <remarks>
        /// Code snippet from: Foggzie. AndroidJavaObject.Call array passing error (Unity for Android).
        /// stackoverflow.com/a/42681889
        /// </remarks>
        /// <param name="values"></param>
        /// <returns></returns>
        public static AndroidJavaObject JavaArrayFromCS(string[] values)
        {
            var arrayClass = new AndroidJavaClass(JavaArrayType);
            var arrayObject = arrayClass.CallStatic<AndroidJavaObject>(
                JavaStringFuncNewInstance,
                new AndroidJavaClass(JavaStringType),
                values.Length);
            for (var i = 0; i < values.Length; ++i)
            {
                arrayClass.CallStatic(JavaArrayFuncSet,
                    arrayObject, i,
                    new AndroidJavaObject(JavaStringType, values[i]));
            }
            return arrayObject;
        }
    }
}
#endif // UNITY_ANDROID