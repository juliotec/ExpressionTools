using System.Reflection;

namespace ExpressionXmlSerializer
{    
    public static class ExtensionesType
    {
        #region Metodos

        private static MethodInfo? GetMethod(string nombre, IEnumerable<MethodInfo> methodInfos, Type[] parametros, params Type[] argumentosGenericos)
        {
            foreach (var methodInfo in methodInfos)
            {
                var methodInfo2 = methodInfo;

                if (methodInfo2.Name != nombre)
                {
                    continue;
                }

                if (methodInfo2.IsGenericMethodDefinition)
                {
                    if (argumentosGenericos == null || argumentosGenericos.Length == 0)
                    {
                        continue;
                    }

                    var argumentosGenericos2 = methodInfo2.GetGenericArguments();

                    if (argumentosGenericos.Length != argumentosGenericos2.Length)
                    {
                        continue;
                    }

                    methodInfo2 = methodInfo2.MakeGenericMethod(argumentosGenericos);
                }
                else if (argumentosGenericos != null && argumentosGenericos.Length > 0)
                {
                    continue;
                }

                var parameterInfos = methodInfo2.GetParameters();

                if (parametros == null || parametros.Length == 0)
                {
                    if (parameterInfos.Length == 0)
                    {
                        return methodInfo2;
                    }

                    continue;
                }
                else if (parametros.Length == parameterInfos.Length)
                {
                    var sonParametrosIguales = true;

                    for (var j = 0; j < parametros.Length; j++)
                    {
                        if (parametros[j] != parameterInfos[j].ParameterType)
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

        public static MethodInfo? GetMethod(this Type type, string nombre, Type[] parametros, params Type[] argumentosGenericos)
        {
            return GetMethod(nombre, type.GetTypeInfo().GetDeclaredMethods(nombre), parametros, argumentosGenericos);
        }

        public static MethodInfo? GetMethod(this Type type, string nombre, BindingFlags bindingFlags, Type[] parametros, params Type[] argumentosGenericos)
        {
            return GetMethod(nombre, type.GetMethods(bindingFlags), parametros, argumentosGenericos);
        }

        #endregion
    }
}
