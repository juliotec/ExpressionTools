using System.Globalization;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionXmlSerializer
{    
    public class ExpressionXmlSerializer : IExpressionXmlSerializer
    {
        #region Campos
                                                            
        private static readonly Type[] _tipos = new Type[] { 
            typeof(Enum), //0
            typeof(string), //1
            typeof(IConvertible) //2
        };
        private static readonly Type[] _tiposVacio = Array.Empty<Type>();
                                                            
        #endregion
        #region Propiedades
                                                            
        private Dictionary<string, ParameterExpression> ParameterExpressions
        {
            get
            {                                           
                return _parametersExpressions;
            }
        }
        private readonly Dictionary<string, ParameterExpression> _parametersExpressions = new();
            
        private Dictionary<int, object> NoConvertibles
        {
            get
            {
                return _noConvertibles;
            }
        }
        private readonly Dictionary<int, object> _noConvertibles = new();

        #endregion
        #region Metodos

        public static XAttribute? CrearXAttribute<TEntrada, TSalida>(string? nombre, TEntrada? tEntrada, Func<TEntrada?, TSalida?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return default;
            }

            TSalida? salida;

            if (tEntrada == null || metodo == null || (salida = metodo(tEntrada)) == null)
            {
                return new XAttribute(nombre, string.Empty);
            }

            return new XAttribute(nombre, salida);
        }
                                                                        
        public static XElement? CrearXElement<TEntrada, TSalida>(string? nombre, TEntrada? tEntrada, Func<TEntrada?, TSalida?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return default;
            }

            TSalida? salida;

            if (tEntrada == null || metodo == null || (salida = metodo(tEntrada)) == null)
            {
                return new XElement(nombre);
            }
                                                                        
            return new XElement(nombre, salida);
        }
                                          
        public static XElement? CrearXElement<TEntrada, TSalida>(string? nombre, IEnumerable<TEntrada>? iEnumerable, Func<TEntrada?, TSalida?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return default;
            }
            else if (iEnumerable == null || !iEnumerable.GetEnumerator().MoveNext() || metodo == null)
            {
                return new XElement(nombre);
            }
                                                                        
            return new XElement(nombre, iEnumerable.Select(metodo));
        }
        
        public static IEnumerable<T?>? ParseElements<T>(XElement? xElement, Func<XElement?, T?>? metodo)
        {
            IEnumerable<XElement> xElements;

            if (xElement == null || xElement.IsEmpty || !(xElements = xElement.Elements()).GetEnumerator().MoveNext() || metodo == null)
            {
                return default;
            }

            return xElements.Select(metodo);
        }
                                                                       
        public static IEnumerable<T?>? ParseElements<T>(string? nombre, XElement? xElement, Func<XElement?, T?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseElements(xElement.Element(nombre), metodo);
        }                                        
                                                                       
        public static T? ParseFirstNode<T>(XElement? xElement, Func<XElement?, T?>? metodo)
        {
            if (xElement == null || xElement.IsEmpty || metodo == null || xElement.FirstNode is not XElement xElement2)
            {
                return default;
            }

            return metodo(xElement2);
        }                                        
                                                                       
        public static T? ParseFirstNode<T>(string? nombre, XElement? xElement, Func<XElement?, T?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseFirstNode(xElement.Element(nombre), metodo);
        }                                        
                                                                       
        public static T? ParseValueXElement<T>(XElement? xElement, Func<string?, T?>? metodo)
        {
            if (xElement == null || xElement.IsEmpty || metodo == null)
            {
                return default;
            }

            return metodo(xElement.Value);            
        }
                                                                        
        public static T? ParseValueXElement<T>(XElement? xElement, Type? type, Func<string?, Type?, T?>? metodo)
        {
            if (xElement == null || xElement.IsEmpty || metodo == null)
            {
                return default;
            }

            return metodo(xElement.Value, type);            
        }
                                                                        
        public static T? ParseValueXElement<T>(string? nombre, XElement? xElement, Func<string?, T?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseValueXElement(xElement.Element(nombre), metodo);
        }
                                                                        
        public static T? ParseValueXElement<T>(string? nombre, XElement? xElement, Type? type, Func<string?, Type?, T?>? metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseValueXElement(xElement.Element(nombre), type, metodo);
        }
                                                                        
        public static T? ParseValueXAttribute<T>(XAttribute? xAttribute, Func<string?, T?>? metodo)
        {
            if (xAttribute == null || metodo == null)
            {
                return default;                
            }

            return metodo(xAttribute.Value);
        }
                                                                                            
        public static T? ParseValueXAttribute<T>(XAttribute? xAttribute, Type? type, Func<string?, Type?, T?>? metodo)
        {
            if (xAttribute == null || metodo == null)
            {
                return default;                
            }
            
            return metodo(xAttribute.Value, type);
        }
                                                                                            
        public static T? ParseValueXAttribute<T>(string? nombre, XElement? xElement, Func<string?, T?> metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null)
            {
                return default;
            }

            return ParseValueXAttribute(xElement.Attribute(nombre), metodo);
        }
        
        public static T? ParseValueXAttribute<T>(string? nombre, XElement? xElement, Type? type, Func<string?, Type?, T?> metodo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || xElement == null)
            {
                return default;
            }

            return ParseValueXAttribute(xElement.Attribute(nombre), type, metodo);
        }
                                                            
        protected T? CrearXElementGenerico<T>(T? valor)
        {
            var t = CrearXElement(valor);

            if (t == null)
            {
                return default;
            }

            return (T)t;
        }
                                                            
        protected T? ParseGenerico<T>(string? valorString)
        {
            var t = Parse(valorString, typeof(T));

            if (t == null)
            {
                return default;
            }

            return (T)t;
        }
                                                            
        protected T? ParseGenerico<T>(XElement? xElement)
        {
            var t = Parse(xElement);

            if (t == null)
            {
                return default;
            }

            return (T)t;
        }
                                                        
        protected virtual object? Parse(string? valorString, Type? type)
        {
            if (string.IsNullOrEmpty(valorString))
            {
                return default;
            }
            else if (_tipos[0].IsAssignableFrom(type))
            {
                return Enum.Parse(type, valorString);
            }
            else if (_tipos[1].IsAssignableFrom(type))
            {
                return valorString;
            }
            else if (_tipos[2].IsAssignableFrom(type))
            {
                return Convert.ChangeType(valorString, type, CultureInfo.CurrentCulture);
            }

            throw new Exception("type desconocido");
        }
                                                       
        protected virtual object? Parse(XElement? xElement)
        {
            if (xElement == null)
            {
                return default;
            }

            return xElement.Name.LocalName switch
            {
                "NoConvertible" => ParseNoConvertible(xElement),
                "Expression" => ParseExpression(xElement),
                "Type" => ParseType(xElement),
                "PropertyInfo" => ParsePropertyInfo(xElement),
                "ConstructorInfo" => ParseConstructorInfo(xElement),
                "MethodInfo" => ParseMethodInfo(xElement),
                "FieldInfo" => ParseFieldInfo(xElement),
                "ElementInit" => ParseElementInit(xElement),
                "MemberAssignment" => ParseMemberAssignment(xElement),
                "MemberListBinding" => ParseMemberListBinding(xElement),
                "MemberMemberBinding" => ParseMemberMemberBinding(xElement),
                _ => throw new Exception("LocalName desconocido")
            };
        }
                                           
        protected virtual object? ParseNoConvertible(XElement? xElement)
        {
            var hashCode = ParseValueXElement("HashCode", xElement, ParseGenerico<int>);

            if (NoConvertibles.TryGetValue(hashCode, out object? referencia))
            {
                return referencia;
            }

            throw new Exception("NoConvertible desconocido");
        }
                                                        
        protected virtual string? ObtenerNombreType(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            var nombre = type.AssemblyQualifiedName;
                                                        
            if (string.IsNullOrEmpty(nombre))
            {
                nombre = type.Name;
            }
                                                        
            return nombre;
        }
                                                           
        #region ToXElement
           
        protected virtual XElement? CrearXElementPropertyInfoAnonymousType(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null)
            {
                return default;
            }

            return new XElement("PropertyInfo",
                CrearXElement("Name", propertyInfo.Name, CrearXElement),
                CrearXElement("PropertyType", propertyInfo.PropertyType, CrearXElementType));
        }
        
        protected virtual XElement? CrearXElementAnonymousType(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            return new XElement("Type",
                CrearXElement("Name", type, ObtenerNombreType),
                CrearXElement("MemberType", type.MemberType, CrearXElementGenerico),
                CrearXElement("Properties", type.GetProperties(), CrearXElementPropertyInfoAnonymousType),
                CrearXElement("ParametersConstructor", type.GetProperties(), CrearXElementPropertyInfoAnonymousType));
        }            
                               
        protected virtual XElement? CrearXElementNoConvertible(object? valor)
        {
            if (valor == null)
            {
                return default;
            }

            var hashCode = valor.GetHashCode();
                                                                        
            if (!NoConvertibles.ContainsKey(hashCode))
            {
                NoConvertibles.Add(hashCode, valor);
            }
                            
            return new XElement(
                "NoConvertible",
                CrearXElement("HashCode", hashCode, CrearXElementGenerico));
        }
                                                        
        protected virtual XElement? CrearXElementType(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            return new XElement(
                "Type",
                CrearXElement("MemberType", type.MemberType, CrearXElementGenerico),
                CrearXElement("Name", type, ObtenerNombreType));
        }
                                                                        
        protected virtual XElement? CrearXElementPropertyInfo(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null)
            {
                return default;
            }

            return new XElement(
                "PropertyInfo",
                CrearXElement("MemberType", propertyInfo.MemberType, CrearXElementGenerico),
                CrearXElement("Name", propertyInfo.Name, CrearXElement),
                CrearXElement("DeclaringType", propertyInfo.DeclaringType, CrearXElementType!),
                CrearXElement("IndexParameters", propertyInfo.GetIndexParameters().Select(p => p.ParameterType), CrearXElementType));
        }
                                                                        
        protected virtual XElement? CrearXElementConstructorInfo(ConstructorInfo? constructorInfo)
        {
            if (constructorInfo == null)
            {
                return default;
            }

            return new XElement(
                "ConstructorInfo",
                CrearXElement("MemberType", constructorInfo.MemberType, CrearXElementGenerico),
                CrearXElement("Name", constructorInfo.Name, CrearXElement),
                CrearXElement("DeclaringType", constructorInfo.DeclaringType, CrearXElementType!),
                CrearXElement("Parameters", constructorInfo.GetParameters().Select(p => p.ParameterType), CrearXElementType));
        }
                                                                        
        protected virtual XElement? CrearXElementMethodInfo(MethodInfo? methodInfo)
        {
            if (methodInfo == null)
            {
                return default;
            }

            return new XElement(
                "MethodInfo",
                CrearXElement("MemberType", methodInfo.MemberType, CrearXElementGenerico),
                CrearXElement("Name", methodInfo.Name, CrearXElement),
                CrearXElement("DeclaringType", methodInfo.DeclaringType, CrearXElementType!),
                CrearXElement("Parameters", methodInfo.GetParameters().Select(p => p.ParameterType), CrearXElementType),
                CrearXElement("GenericArguments", methodInfo.GetGenericArguments(), CrearXElementType));
        }
                                                                        
        protected virtual XElement? CrearXElementFieldInfo(FieldInfo? fieldInfo)
        {
            if (fieldInfo == null)
            {
                return default;
            }

            return new XElement(
                "FieldInfo",
                CrearXElement("MemberType", fieldInfo.MemberType, CrearXElementGenerico),
                CrearXElement("Name", fieldInfo.Name, CrearXElement),
                CrearXElement("DeclaringType", fieldInfo.DeclaringType, CrearXElementType!));
        }
                                                                        
        protected virtual XElement? CrearXElementElementInit(ElementInit? elementInit)
        {
            if (elementInit == null)
            {
                return default;
            }

            return new XElement(
                "ElementInit",
                CrearXElement("AddMethod", elementInit.AddMethod, CrearXElementMethodInfo),
                CrearXElement("Arguments", elementInit.Arguments, CrearXElementExpression));
        }
                                                                        
        protected virtual XElement? CrearXElementMemberAssignment(MemberAssignment? memberAssignment)
        {
            if (memberAssignment == null)
            {
                return default;
            }

            return new XElement(
                "MemberAssignment",
                CrearXElement("BindingType", memberAssignment.BindingType, CrearXElementGenerico),
                CrearXElement("Member", memberAssignment.Member, CrearXElementMemberInfo),
                CrearXElement("Expression", memberAssignment.Expression, CrearXElementExpression));
        }
                                                                        
        protected virtual XElement? CrearXElementMemberListBinding(MemberListBinding? memberListBinding)
        {
            if (memberListBinding == null)
            {
                return default;
            }

            return new XElement(
                "MemberListBinding",
                CrearXElement("BindingType", memberListBinding.BindingType, CrearXElementGenerico),
                CrearXElement("Member", memberListBinding.Member, CrearXElementMemberInfo),
                CrearXElement("Initializers", memberListBinding.Initializers, CrearXElementElementInit));
        }
                                                                        
        protected virtual XElement? CrearXElementMemberMemberBinding(MemberMemberBinding? memberMemberBinding)
        {
            if (memberMemberBinding == null)
            {
                return default;
            }

            return new XElement(
                "MemberMemberBinding",
                CrearXElement("BindingType", memberMemberBinding.BindingType, CrearXElementGenerico),
                CrearXElement("Member", memberMemberBinding.Member, CrearXElementMemberInfo),
                CrearXElement("Bindings", memberMemberBinding.Bindings, CrearXElementMemberBinding));
        }
                                                            
        protected virtual XElement? CrearXElementExpression(Expression? expression)
        {
            if (expression == null)
            {
                return default;
            }

            return new XElement(
                "Expression",
                expression.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => CrearXElement(p.Name, p.GetValue(expression, null), CrearXElement!)));
        }
                                                                        
        protected virtual XElement? CrearXElementMemberInfo(MemberInfo? memberInfo)
        {
            if (memberInfo == null)
            {
                return default;
            }

            return memberInfo.MemberType switch
            {
                MemberTypes.NestedType or MemberTypes.TypeInfo => CrearXElementType((Type)memberInfo),
                MemberTypes.Property => CrearXElementPropertyInfo((PropertyInfo)memberInfo),
                MemberTypes.Method => CrearXElementMethodInfo((MethodInfo)memberInfo),
                MemberTypes.Constructor => CrearXElementConstructorInfo((ConstructorInfo)memberInfo),
                MemberTypes.Field => CrearXElementFieldInfo((FieldInfo)memberInfo),
                _ => throw new Exception("MemberTypes desconocido")
            };
        }
                                                                        
        protected virtual XElement? CrearXElementMemberBinding(MemberBinding? memberBinding)
        {
            if (memberBinding == null)
            {
                return default;
            }

            return memberBinding.BindingType switch
            {
                MemberBindingType.Assignment => CrearXElementMemberAssignment((MemberAssignment)memberBinding),
                MemberBindingType.ListBinding => CrearXElementMemberListBinding((MemberListBinding)memberBinding),
                MemberBindingType.MemberBinding => CrearXElementMemberMemberBinding((MemberMemberBinding)memberBinding),
                _ => throw new Exception("MemberBindingType desconocido")
            };
        }
                                                            
        protected virtual object? CrearXElement(object? valor)
        {
            if (valor == null)
            {
                return default;
            }
            
            if (valor is IConvertible)
            {
                return valor;
            }                            

            if (valor is MemberInfo memberInfo)
            {
                return CrearXElementMemberInfo(memberInfo);
            }

            if (valor is Expression expression)
            {
                return CrearXElementExpression(expression);
            }

            if (valor is IEnumerable<Expression> expressions)
            {
                return expressions.Select(CrearXElementExpression);
            }

            if (valor is IEnumerable<MemberInfo> memberInfos)
            {
                return memberInfos.Select(CrearXElementMemberInfo);
            }

            if (valor is IEnumerable<ElementInit> elementInits)
            {
                return elementInits.Select(CrearXElementElementInit);
            }

            if (valor is IEnumerable<MemberBinding> memberBindings)
            {
                return memberBindings.Select(CrearXElementMemberBinding);
            }

            if (valor is ElementInit elementInit)
            {
                return CrearXElementElementInit(elementInit);
            }

            if (valor is MemberBinding memberBinding)
            {
                return CrearXElementMemberBinding(memberBinding);
            }

            return CrearXElementNoConvertible(valor);
        }
                                                                        
        #endregion
        #region ToMemberInfo
                                                    
        protected virtual Type? ParseType(XElement? xElement)
        {
            return ParseValueXElement("Name", xElement, Type.GetType!)!;
        }
                                                    
        protected virtual FieldInfo? ParseFieldInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name;

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGenerico<string>)) == null)
            {
                return default;
            }

            return declaringType.GetField(
                name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)!;
        }
                                                    
        protected virtual PropertyInfo? ParsePropertyInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name;

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGenerico<string>)) == null)
            {
                return default;
            }

            var indexParameters = ParseElements("IndexParameters", xElement, ParseType);
            var types = _tiposVacio;
            
            if (indexParameters != null)
            {
                types = indexParameters.ToArray()!;
            }

            return declaringType.GetProperty(
                name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                null,
                null,
                types,
                null)!;
        }
                                                    
        protected virtual MethodInfo? ParseMethodInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name; 

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGenerico<string>)) == null)
            {
                return default;
            }

            var parameters = ParseElements("Parameters", xElement, ParseType);
            var types = _tiposVacio;

            if (parameters != null)
            {
                types = parameters.ToArray()!;
            }

            var genericArguments = ParseElements("GenericArguments", xElement, ParseType);
            var types2 = _tiposVacio;

            if (genericArguments != null)
            {
                types2 = genericArguments.ToArray()!;
            }

            return declaringType.GetMethod(
                name, 
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                types,
                types2);
        }
                                                    
        protected virtual ConstructorInfo? ParseConstructorInfo(XElement? xElement)
        {
            Type? declaringType;

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null)
            {
                return default;
            }

            var parameters = ParseElements("Parameters", xElement, ParseType);
            var types = _tiposVacio;

            if (parameters != null)
            {
                types = parameters.ToArray()!;
            }

            return declaringType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                types,
                null)!;
        }
                                                    
        protected virtual MemberInfo? ParseMemberInfo(XElement? xElement)
        {
            return ParseValueXElement("MemberType", xElement, ParseGenerico<MemberTypes>) switch
            {
                MemberTypes.Property => ParsePropertyInfo(xElement),
                MemberTypes.Field => ParseFieldInfo(xElement),
                MemberTypes.NestedType or MemberTypes.TypeInfo => ParseType(xElement),
                MemberTypes.Method => ParseMethodInfo(xElement),
                MemberTypes.Constructor => ParseConstructorInfo(xElement),
                _ => throw new Exception("MemberTypes desconocido")
            };
        }
        
        #endregion
        #region ToElementInit
                                                    
        protected virtual ElementInit? ParseElementInit(XElement? xElement)
        {
            MethodInfo? methodInfo;
            IEnumerable<Expression?>? expressions;

            if ((methodInfo = ParseFirstNode("AddMethod", xElement, ParseMethodInfo)) == null || (expressions = ParseElements("Arguments", xElement, ParseExpression)) == null)
            {
                return default;
            }
            
            return Expression.ElementInit(
                methodInfo,
                expressions!);
        }                                    

        #endregion
        #region ToMemberBinding

        protected virtual MemberAssignment? ParseMemberAssignment(XElement? xElement)
        {
            MemberInfo? memberInfo;
            Expression? expression;

            if ((memberInfo = ParseFirstNode("Member", xElement, ParseMemberInfo)) == null || (expression = ParseFirstNode("Expression", xElement, ParseExpression)) == null)
            {
                return default;
            }

            return Expression.Bind(
                memberInfo,
                expression);
        }
                                
        protected virtual MemberListBinding? ParseMemberListBinding(XElement? xElement)
        {
            MemberInfo? memberInfo;
            IEnumerable<ElementInit?>? elementInits;

            if ((memberInfo = ParseFirstNode("Member", xElement, ParseMemberInfo)) == null || (elementInits = ParseElements("Initializers", xElement, ParseElementInit)) == null)
            {
                return default;
            }

            return Expression.ListBind(
                memberInfo,
                elementInits!);
        }
                                
        protected virtual MemberMemberBinding? ParseMemberMemberBinding(XElement? xElement)
        {
            MemberInfo? memberInfo;
            IEnumerable<MemberBinding?>? memberBindings;

            if ((memberInfo = ParseFirstNode("Member", xElement, ParseMemberInfo)) == null || (memberBindings = ParseElements("Bindings", xElement, ParseMemberBinding)) == null)
            {
                return default;
            }

            return Expression.MemberBind(
                memberInfo,
                memberBindings!);
        }
                                                    
        protected virtual MemberBinding? ParseMemberBinding(XElement? xElement)
        {
            return ParseValueXElement("BindingType", xElement, ParseGenerico<MemberBindingType>) switch
            {
                MemberBindingType.Assignment => ParseMemberAssignment(xElement),
                MemberBindingType.ListBinding => ParseMemberListBinding(xElement),
                MemberBindingType.MemberBinding => ParseMemberMemberBinding(xElement),
                _ => throw new Exception("MemberBindingType desconocido")
            };
        }                                                  
                                                    
        #endregion
        #region ToExpression
                                
        protected virtual InvocationExpression? ParseInvocationExpression(XElement? xElement)
        {
            Expression? expression;
            IEnumerable<Expression?>? expressions;

            if ((expression = ParseFirstNode("Expression", xElement, ParseExpression)) == null || (expressions = ParseElements("Arguments", xElement, ParseExpression)) == null)
            {
                return default;
            }

            return Expression.Invoke(
                expression,
                expressions!);
        }
                                
        protected virtual MemberExpression? ParseMemberExpression(XElement? xElement)
        {
            Expression? expression;
            MemberInfo? memberInfo;

            if ((expression = ParseFirstNode("Expression", xElement, ParseExpression)) == null || (memberInfo = ParseFirstNode("Member", xElement, ParseMemberInfo)) == null)
            {
                return default;
            }
                
            return Expression.MakeMemberAccess(
                expression,
                memberInfo);
        }
                                
        protected virtual MemberInitExpression? ParseMemberInitExpression(XElement? xElement)
        {
            NewExpression? newExpression;
            IEnumerable<MemberBinding?>? memberBindings;

            if ((newExpression = ParseFirstNode("NewExpression", xElement, ParseNewExpression)) == null || (memberBindings = ParseElements("Bindings", xElement, ParseMemberBinding)) == null)
            {
                return default;
            }

            return Expression.MemberInit(
                newExpression,
                memberBindings!);
        }
                                
        protected virtual TypeBinaryExpression? ParseTypeBinaryExpression(XElement? xElement)
        {
            Expression? expression;
            Type? type;

            if ((expression = ParseFirstNode("Expression", xElement, ParseExpression)) == null || (type = ParseFirstNode("TypeOperand", xElement, ParseType)) == null)
            {
                return default;
            }

            return Expression.TypeIs(
                expression,
                type);
        }
                                                    
        protected virtual ConditionalExpression? ParseConditionalExpression(XElement? xElement)
        {
            Expression? testExpression;
            Expression? ifTrueExpression;
            Expression? ifFalseExpression;
            Type? type;

            if ((testExpression = ParseFirstNode("Test", xElement, ParseExpression)) == null || (ifTrueExpression = ParseFirstNode("IfTrue", xElement, ParseExpression)) == null || (ifFalseExpression = ParseFirstNode("IfFalse", xElement, ParseExpression)) == null || (type = ParseFirstNode("Type", xElement, ParseType)) == null)
            {
                return default;
            }

            return Expression.Condition(
                testExpression,
                ifTrueExpression,
                ifFalseExpression, 
                type);
        }
                                    
        protected virtual ListInitExpression ParseListInitExpression(XElement xElement)
        {
            return Expression.ListInit(
                ParseFirstNode("NewExpression", xElement, ParseNewExpression), 
                ParseElements("Initializers", xElement, ParseElementInit));
        }
                                
        protected virtual LambdaExpression ParseLambdaExpression(XElement xElement)
        {
            return Expression.Lambda(
                ParseFirstNode("Type", xElement, ParseType), 
                ParseFirstNode("Body", xElement, ParseExpression), 
                ParseValueXElement("Name", xElement, ParseGenerico<string>), 
                ParseValueXElement("TailCall", xElement, ParseGenerico<bool>), 
                ParseElements("Parameters", xElement, ParseParameterExpression));
        }
                                                    
        protected virtual MethodCallExpression ParseMethodCallExpression(XElement xElement)
        {
            return Expression.Call(
                ParseFirstNode("Object", xElement, ParseExpression), 
                ParseFirstNode("Method", xElement, ParseMethodInfo), 
                ParseElements("Arguments", xElement, ParseExpression));
        }
                            
        protected virtual UnaryExpression ParseUnaryExpression(XElement xElement, ExpressionType expressionType)
        {
            return Expression.MakeUnary(
                expressionType,
                ParseFirstNode("Operand", xElement, ParseExpression),
                ParseFirstNode("Type", xElement, ParseType),
                ParseFirstNode("Method", xElement, ParseMethodInfo));
        }
                            
        protected virtual BinaryExpression ParseBinaryExpression(XElement xElement, ExpressionType expressionType)
        {
            return Expression.MakeBinary(
                expressionType,
                ParseFirstNode("Left", xElement, ParseExpression),
                ParseFirstNode("Right", xElement, ParseExpression),
                ParseValueXElement("IsLiftedToNull", xElement, ParseGenerico<bool>),
                ParseFirstNode("Method", xElement, ParseMethodInfo),
                ParseFirstNode("Conversion", xElement, ParseLambdaExpression));
        }
                                   
        protected virtual ParameterExpression ParseParameterExpression(XElement xElement)
        {
            var type = ParseFirstNode("Type", xElement, ParseType);
            var name = ParseValueXElement("Name", xElement, ParseGenerico<string>);

            if (ParameterExpressions.TryGetValue(name + ObtenerNombreType(type), out ParameterExpression? parameterExpression))
            {
                return parameterExpression;
            }

            parameterExpression = Expression.Parameter(type, name);
            ParameterExpressions.Add(id, parameterExpression);
                                                    
            return parameterExpression;
        }
                                                    
        protected virtual NewExpression ParseNewExpression(XElement xElement)
        {
            var constructor = ParseFirstNode("Constructor", xElement, ParseConstructorInfo);
            var members = ParseElements("Members", xElement, ParseMemberInfo);
            var arguments = ParseElements("Arguments", xElement, ParseExpression);
                            
            if (members == null || !members.GetEnumerator().MoveNext())
            {
                return Expression.New(constructor, arguments);
            }
                                                        
            return Expression.New(constructor, arguments, members);
        }
                            
        protected virtual ConstantExpression ParseConstantExpression(XElement xElement)
        {
            var type = ParseFirstNode("Type", xElement, ParseType);
            var value = ParseFirstNode("Value", xElement, Parse);
                            
            value ??= ParseValueXElement("Value", xElement, type, Parse);
                            
            return Expression.Constant(value, type);
        }
                                     
        protected virtual NewArrayExpression ParseNewArrayExpression(XElement xElement, ExpressionType expressionType)
        {
            var elementType = ParseFirstNode("Type", xElement, ParseType).GetElementType()!;
            var expressions = ParseElements("Expressions", xElement, ParseExpression);

            return expressionType switch
            {
                ExpressionType.NewArrayInit => Expression.NewArrayInit(elementType, expressions),
                ExpressionType.NewArrayBounds => Expression.NewArrayBounds(elementType, expressions),
                _ => throw new Exception("ExpressionType desconocido")
            };
        }
                            
        protected virtual Expression? ParseExpression(XElement? xElement)
        {
            var expressionType = ParseValueXElement("NodeType", xElement, ParseGenerico<ExpressionType>);

            return expressionType switch
            {
                ExpressionType.MemberAccess => ParseMemberExpression(xElement),
                ExpressionType.Parameter => ParseParameterExpression(xElement),
                ExpressionType.Constant => ParseConstantExpression(xElement),
                ExpressionType.Lambda => ParseLambdaExpression(xElement),
                ExpressionType.Call => ParseMethodCallExpression(xElement),
                ExpressionType.New => ParseNewExpression(xElement),
                ExpressionType.ListInit => ParseListInitExpression(xElement),
                ExpressionType.MemberInit => ParseMemberInitExpression(xElement),
                ExpressionType.TypeIs => ParseTypeBinaryExpression(xElement),
                ExpressionType.Invoke => ParseInvocationExpression(xElement),
                ExpressionType.Conditional => ParseConditionalExpression(xElement),
                ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => ParseNewArrayExpression(xElement, expressionType),
                ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.ArrayLength or ExpressionType.Quote or ExpressionType.TypeAs => ParseUnaryExpression(xElement, expressionType),
                ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.Subtract or ExpressionType.SubtractChecked or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.Divide or ExpressionType.Modulo or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.Coalesce or ExpressionType.ArrayIndex or ExpressionType.RightShift or ExpressionType.LeftShift or ExpressionType.ExclusiveOr => ParseBinaryExpression(xElement, expressionType),
                _ => throw new Exception("ExpressionType desconocido")
            };
        }

        #endregion

        #endregion
        #region Miembros IExpressionXmlSerializer

        public XElement? ToXElement(MemberInfo? memberInfo)
        {
            return CrearXElementMemberInfo(memberInfo);
        }

        public XElement? ToXElement(ElementInit? elementInit)
        {
            return CrearXElementElementInit(elementInit);
        }

        public XElement? ToXElement(MemberBinding? memberBinding)
        {
            return CrearXElementMemberBinding(memberBinding);
        }

        public XElement? ToXElement(Expression? expression)
        {
            return CrearXElementExpression(expression);
        }

        public MemberInfo? ToMemberInfo(XElement? xElement)
        {
            return ParseGenerico<MemberInfo>(xElement);
        }

        public T? ToMemberInfo<T>(XElement? xElement) where T : MemberInfo
        {
            return ParseGenerico<T>(xElement);
        }

        public ElementInit? ToElementInit(XElement? xElement)
        {
            return ParseGenerico<ElementInit>(xElement);
        }

        public MemberBinding? ToMemberBinding(XElement? xElement)
        {
            return ParseGenerico<MemberBinding>(xElement);
        }

        public T? ToMemberBinding<T>(XElement? xElement) where T : MemberBinding
        {
            return ParseGenerico<T>(xElement);
        }

        public Expression? ToExpression(XElement? xElement)
        {
            ParameterExpressions.Clear();

            return ParseGenerico<Expression>(xElement);
        }

        public T? ToExpression<T>(XElement? xElement) where T : Expression
        {
            ParameterExpressions.Clear();

            return ParseGenerico<T>(xElement);
        }

        #endregion
    }
}
