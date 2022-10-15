using System.Reflection;

namespace ExpressionTools
{    
    public static class ExtensionsType
    {
        #region Methods

        private static MethodInfo? GetMethod(string name, IEnumerable<MethodInfo> methodInfos, Type[] types, params Type[] genericTypes)
        {
            foreach (var methodInfo in methodInfos)
            {
                var methodInfo2 = methodInfo;

                if (methodInfo2.Name != name)
                {
                    continue;
                }

                if (methodInfo2.IsGenericMethodDefinition)
                {
                    if (genericTypes == null || genericTypes.Length == 0)
                    {
                        continue;
                    }

                    var argumentosGenericos2 = methodInfo2.GetGenericArguments();

                    if (genericTypes.Length != argumentosGenericos2.Length)
                    {
                        continue;
                    }

                    methodInfo2 = methodInfo2.MakeGenericMethod(genericTypes);
                }
                else if (genericTypes != null && genericTypes.Length > 0)
                {
                    continue;
                }

                var parameterInfos = methodInfo2.GetParameters();

                if (types == null || types.Length == 0)
                {
                    if (parameterInfos.Length == 0)
                    {
                        return methodInfo2;
                    }

                    continue;
                }
                else if (types.Length == parameterInfos.Length)
                {
                    var sonParametrosIguales = true;

                    for (var j = 0; j < types.Length; j++)
                    {
                        if (types[j] != parameterInfos[j].ParameterType)
                        {
                            sonParametrosIguales = false;
                            break;
                        }
                    }

                    if (sonParametrosIguales)
                    {
                        return methodInfo2;
                    }
                }
            }

            return null;
        }

        public static MethodInfo? GetMethod(this Type type, string name, Type[] types, params Type[] genericTypes)
        {
            return GetMethod(name, type.GetTypeInfo().GetDeclaredMethods(name), types, genericTypes);
        }

        public static MethodInfo? GetMethod(this Type type, string name, BindingFlags bindingFlags, Type[] types, params Type[] genericTypess)
        {
            return GetMethod(name, type.GetMethods(bindingFlags), types, genericTypess);
        }

        #endregion
    }
}
