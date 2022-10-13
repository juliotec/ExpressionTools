using System.Globalization;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionXmlSerializer
{    
    public class ExpressionXmlSerializer : IExpressionXmlSerializer
    {
        #region Fields
                                                            
        private static readonly Type[] _types = new Type[] { 
            typeof(Enum), //0
            typeof(string), //1
            typeof(IConvertible) //2
        };
        private static readonly Type[] _typesEmpty = Array.Empty<Type>();
                                                            
        #endregion
        #region Properties
                                                            
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
        #region Methods

        public static XAttribute? CreateXAttribute<TIn, TOut>(string? name, TIn? tIn, Func<TIn?, TOut?>? method)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return default;
            }

            TOut? tOut;

            if (tIn == null || method == null || (tOut = method(tIn)) == null)
            {
                return new XAttribute(name, string.Empty);
            }

            return new XAttribute(name, tOut);
        }
                                                                        
        public static XElement? CreateXElement<TIn, TOut>(string? name, TIn? tIn, Func<TIn?, TOut?>? method)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return default;
            }

            TOut? tOut;

            if (tIn == null || method == null || (tOut = method(tIn)) == null)
            {
                return new XElement(name);
            }
                                                                        
            return new XElement(name, tOut);
        }
                                          
        public static XElement? CreateXElement<TIn, TOut>(string? name, IEnumerable<TIn>? iEnumerable, Func<TIn?, TOut?>? method)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return default;
            }
            else if (iEnumerable == null || !iEnumerable.GetEnumerator().MoveNext() || method == null)
            {
                return new XElement(name);
            }
                                                                        
            return new XElement(name, iEnumerable.Select(method));
        }
        
        public static IEnumerable<T?>? ParseElements<T>(XElement? xElement, Func<XElement?, T?>? method)
        {
            IEnumerable<XElement> xElements;

            if (xElement == null || xElement.IsEmpty || !(xElements = xElement.Elements()).GetEnumerator().MoveNext() || method == null)
            {
                return default;
            }

            return xElements.Select(method);
        }
                                                                       
        public static IEnumerable<T?>? ParseElements<T>(string? name, XElement? xElement, Func<XElement?, T?>? method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseElements(xElement.Element(name), method);
        }                                        
                                                                       
        public static T? ParseFirstNode<T>(XElement? xElement, Func<XElement?, T?>? method)
        {
            if (xElement == null || xElement.IsEmpty || method == null || xElement.FirstNode is not XElement xElement2)
            {
                return default;
            }

            return method(xElement2);
        }                                        
                                                                       
        public static T? ParseFirstNode<T>(string? name, XElement? xElement, Func<XElement?, T?>? method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseFirstNode(xElement.Element(name), method);
        }                                        
                                                                       
        public static T? ParseValueXElement<T>(XElement? xElement, Func<string?, T?>? method)
        {
            if (xElement == null || xElement.IsEmpty || method == null)
            {
                return default;
            }

            return method(xElement.Value);            
        }
                                                                        
        public static T? ParseValueXElement<T>(XElement? xElement, Type? type, Func<string?, Type?, T?>? method)
        {
            if (xElement == null || xElement.IsEmpty || method == null)
            {
                return default;
            }

            return method(xElement.Value, type);            
        }
                                                                        
        public static T? ParseValueXElement<T>(string? name, XElement? xElement, Func<string?, T?>? method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseValueXElement(xElement.Element(name), method);
        }
                                                                        
        public static T? ParseValueXElement<T>(string? name, XElement? xElement, Type? type, Func<string?, Type?, T?>? method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null || xElement.IsEmpty)
            {
                return default;
            }

            return ParseValueXElement(xElement.Element(name), type, method);
        }
                                                                        
        public static T? ParseValueXAttribute<T>(XAttribute? xAttribute, Func<string?, T?>? method)
        {
            if (xAttribute == null || method == null)
            {
                return default;                
            }

            return method(xAttribute.Value);
        }
                                                                                            
        public static T? ParseValueXAttribute<T>(XAttribute? xAttribute, Type? type, Func<string?, Type?, T?>? method)
        {
            if (xAttribute == null || method == null)
            {
                return default;                
            }
            
            return method(xAttribute.Value, type);
        }
                                                                                            
        public static T? ParseValueXAttribute<T>(string? name, XElement? xElement, Func<string?, T?> method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null)
            {
                return default;
            }

            return ParseValueXAttribute(xElement.Attribute(name), method);
        }
        
        public static T? ParseValueXAttribute<T>(string? name, XElement? xElement, Type? type, Func<string?, Type?, T?> method)
        {
            if (string.IsNullOrWhiteSpace(name) || xElement == null)
            {
                return default;
            }

            return ParseValueXAttribute(xElement.Attribute(name), type, method);
        }
                                                            
        protected T? CreateGenericXElement<T>(T? valor)
        {
            var t = CreateXElement(valor);

            if (t == null)
            {
                return default;
            }

            return (T)t;
        }
                                                            
        protected T? ParseGeneric<T>(string? valorString)
        {
            var t = Parse(valorString, typeof(T));

            if (t == null)
            {
                return default;
            }

            return (T)t;
        }
                                                            
        protected T? ParseGeneric<T>(XElement? xElement)
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
            else if (_types[0].IsAssignableFrom(type))
            {
                return Enum.Parse(type, valorString);
            }
            else if (_types[1].IsAssignableFrom(type))
            {
                return valorString;
            }
            else if (_types[2].IsAssignableFrom(type))
            {
                return Convert.ChangeType(valorString, type, CultureInfo.CurrentCulture);
            }

            throw new Exception("type unknown");
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
                _ => throw new Exception("LocalName unknown")
            };
        }
                                           
        protected virtual object? ParseNoConvertible(XElement? xElement)
        {
            var hashCode = ParseValueXElement("HashCode", xElement, ParseGeneric<int>);

            if (NoConvertibles.TryGetValue(hashCode, out object? referencia))
            {
                return referencia;
            }

            throw new Exception("NoConvertible desconocido");
        }
                                                        
        protected virtual string? GetTypeName(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            var name = type.AssemblyQualifiedName;
                                                        
            if (string.IsNullOrEmpty(name))
            {
                name = type.Name;
            }
                                                        
            return name;
        }
        
        #region ToXElement
           
        protected virtual XElement? CreatePropertyInfoAnonymousTypeXElement(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null)
            {
                return default;
            }

            return new XElement("PropertyInfo",
                CreateXElement("Name", propertyInfo.Name, CreateXElement),
                CreateXElement("PropertyType", propertyInfo.PropertyType, CreateTypeXElement));
        }
        
        protected virtual XElement? CreateAnonymousTypeXElement(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            return new XElement("Type",
                CreateXElement("Name", type, GetTypeName),
                CreateXElement("MemberType", type.MemberType, CreateGenericXElement),
                CreateXElement("Properties", type.GetProperties(), CreatePropertyInfoAnonymousTypeXElement),
                CreateXElement("ParametersConstructor", type.GetProperties(), CreatePropertyInfoAnonymousTypeXElement));
        }            
                               
        protected virtual XElement? CreateNoConvertibleXElement(object? valor)
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
                CreateXElement("HashCode", hashCode, CreateGenericXElement));
        }
                                                        
        protected virtual XElement? CreateTypeXElement(Type? type)
        {
            if (type == null)
            {
                return default;
            }

            return new XElement(
                "Type",
                CreateXElement("MemberType", type.MemberType, CreateGenericXElement),
                CreateXElement("Name", type, GetTypeName));
        }
                                                                        
        protected virtual XElement? CreatePropertyInfoXElement(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null)
            {
                return default;
            }

            return new XElement(
                "PropertyInfo",
                CreateXElement("MemberType", propertyInfo.MemberType, CreateGenericXElement),
                CreateXElement("Name", propertyInfo.Name, CreateXElement),
                CreateXElement("DeclaringType", propertyInfo.DeclaringType, CreateTypeXElement),
                CreateXElement("IndexParameters", propertyInfo.GetIndexParameters().Select(p => p.ParameterType), CreateTypeXElement));
        }
                                                                        
        protected virtual XElement? CreateConstructorInfoXElement(ConstructorInfo? constructorInfo)
        {
            if (constructorInfo == null)
            {
                return default;
            }

            return new XElement(
                "ConstructorInfo",
                CreateXElement("MemberType", constructorInfo.MemberType, CreateGenericXElement),
                CreateXElement("Name", constructorInfo.Name, CreateXElement),
                CreateXElement("DeclaringType", constructorInfo.DeclaringType, CreateTypeXElement),
                CreateXElement("Parameters", constructorInfo.GetParameters().Select(p => p.ParameterType), CreateTypeXElement));
        }
                                                                        
        protected virtual XElement? CreateMethodInfoXElement(MethodInfo? methodInfo)
        {
            if (methodInfo == null)
            {
                return default;
            }

            return new XElement(
                "MethodInfo",
                CreateXElement("MemberType", methodInfo.MemberType, CreateGenericXElement),
                CreateXElement("Name", methodInfo.Name, CreateXElement),
                CreateXElement("DeclaringType", methodInfo.DeclaringType, CreateTypeXElement),
                CreateXElement("Parameters", methodInfo.GetParameters().Select(p => p.ParameterType), CreateTypeXElement),
                CreateXElement("GenericArguments", methodInfo.GetGenericArguments(), CreateTypeXElement));
        }
                                                                        
        protected virtual XElement? CreateFieldInfoXElement(FieldInfo? fieldInfo)
        {
            if (fieldInfo == null)
            {
                return default;
            }

            return new XElement(
                "FieldInfo",
                CreateXElement("MemberType", fieldInfo.MemberType, CreateGenericXElement),
                CreateXElement("Name", fieldInfo.Name, CreateXElement),
                CreateXElement("DeclaringType", fieldInfo.DeclaringType, CreateTypeXElement));
        }
                                                                        
        protected virtual XElement? CreateElementInitXElement(ElementInit? elementInit)
        {
            if (elementInit == null)
            {
                return default;
            }

            return new XElement(
                "ElementInit",
                CreateXElement("AddMethod", elementInit.AddMethod, CreateMethodInfoXElement),
                CreateXElement("Arguments", elementInit.Arguments, CreateExpressionXElement));
        }
                                                                        
        protected virtual XElement? CreateMemberAssignmentXElement(MemberAssignment? memberAssignment)
        {
            if (memberAssignment == null)
            {
                return default;
            }

            return new XElement(
                "MemberAssignment",
                CreateXElement("BindingType", memberAssignment.BindingType, CreateGenericXElement),
                CreateXElement("Member", memberAssignment.Member, CreateMemberInfoXElement),
                CreateXElement("Expression", memberAssignment.Expression, CreateExpressionXElement));
        }
                                                                        
        protected virtual XElement? CreateMemberListBindingXElement(MemberListBinding? memberListBinding)
        {
            if (memberListBinding == null)
            {
                return default;
            }

            return new XElement(
                "MemberListBinding",
                CreateXElement("BindingType", memberListBinding.BindingType, CreateGenericXElement),
                CreateXElement("Member", memberListBinding.Member, CreateMemberInfoXElement),
                CreateXElement("Initializers", memberListBinding.Initializers, CreateElementInitXElement));
        }
                                                                        
        protected virtual XElement? CreateMemberMemberBindingXElement(MemberMemberBinding? memberMemberBinding)
        {
            if (memberMemberBinding == null)
            {
                return default;
            }

            return new XElement(
                "MemberMemberBinding",
                CreateXElement("BindingType", memberMemberBinding.BindingType, CreateGenericXElement),
                CreateXElement("Member", memberMemberBinding.Member, CreateMemberInfoXElement),
                CreateXElement("Bindings", memberMemberBinding.Bindings, CreateMemberBindingXElement));
        }
                                                            
        protected virtual XElement? CreateExpressionXElement(Expression? expression)
        {
            if (expression == null)
            {
                return default;
            }

            return new XElement(
                "Expression",
                expression.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => CreateXElement(p.Name, p.GetValue(expression, null), CreateXElement)));
        }
                                                                        
        protected virtual XElement? CreateMemberInfoXElement(MemberInfo? memberInfo)
        {
            if (memberInfo == null)
            {
                return default;
            }

            return memberInfo.MemberType switch
            {
                MemberTypes.NestedType or MemberTypes.TypeInfo => CreateTypeXElement((Type)memberInfo),
                MemberTypes.Property => CreatePropertyInfoXElement((PropertyInfo)memberInfo),
                MemberTypes.Method => CreateMethodInfoXElement((MethodInfo)memberInfo),
                MemberTypes.Constructor => CreateConstructorInfoXElement((ConstructorInfo)memberInfo),
                MemberTypes.Field => CreateFieldInfoXElement((FieldInfo)memberInfo),
                _ => throw new Exception("MemberTypes unknown")
            };
        }
                                                                        
        protected virtual XElement? CreateMemberBindingXElement(MemberBinding? memberBinding)
        {
            if (memberBinding == null)
            {
                return default;
            }

            return memberBinding.BindingType switch
            {
                MemberBindingType.Assignment => CreateMemberAssignmentXElement((MemberAssignment)memberBinding),
                MemberBindingType.ListBinding => CreateMemberListBindingXElement((MemberListBinding)memberBinding),
                MemberBindingType.MemberBinding => CreateMemberMemberBindingXElement((MemberMemberBinding)memberBinding),
                _ => throw new Exception("MemberBindingType unknown")
            };
        }
                                                            
        protected virtual object? CreateXElement(object? val)
        {
            if (val == null)
            {
                return default;
            }
            
            if (val is IConvertible)
            {
                return val;
            }                            

            if (val is MemberInfo memberInfo)
            {
                return CreateMemberInfoXElement(memberInfo);
            }

            if (val is Expression expression)
            {
                return CreateExpressionXElement(expression);
            }

            if (val is IEnumerable<Expression> expressions)
            {
                return expressions.Select(CreateExpressionXElement);
            }

            if (val is IEnumerable<MemberInfo> memberInfos)
            {
                return memberInfos.Select(CreateMemberInfoXElement);
            }

            if (val is IEnumerable<ElementInit> elementInits)
            {
                return elementInits.Select(CreateElementInitXElement);
            }

            if (val is IEnumerable<MemberBinding> memberBindings)
            {
                return memberBindings.Select(CreateMemberBindingXElement);
            }

            if (val is ElementInit elementInit)
            {
                return CreateElementInitXElement(elementInit);
            }

            if (val is MemberBinding memberBinding)
            {
                return CreateMemberBindingXElement(memberBinding);
            }

            return CreateNoConvertibleXElement(val);
        }
                                                                        
        #endregion
        #region ToMemberInfo
                                                    
        protected virtual Type? ParseType(XElement? xElement)
        {
            return ParseValueXElement("Name", xElement, Type.GetType!);
        }
                                                    
        protected virtual FieldInfo? ParseFieldInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name;

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGeneric<string>)) == null)
            {
                return default;
            }

            return declaringType.GetField(
                name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        }
                                                    
        protected virtual PropertyInfo? ParsePropertyInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name;

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGeneric<string>)) == null)
            {
                return default;
            }

            var indexParameters = ParseElements("IndexParameters", xElement, ParseType);
            var types = _typesEmpty;
            
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
                null);
        }
                                                    
        protected virtual MethodInfo? ParseMethodInfo(XElement? xElement)
        {
            Type? declaringType;
            string? name; 

            if ((declaringType = ParseFirstNode("DeclaringType", xElement, ParseType)) == null || (name = ParseValueXElement("Name", xElement, ParseGeneric<string>)) == null)
            {
                return default;
            }

            var parameters = ParseElements("Parameters", xElement, ParseType);
            var types = _typesEmpty;

            if (parameters != null)
            {
                types = parameters.ToArray()!;
            }

            var genericArguments = ParseElements("GenericArguments", xElement, ParseType);
            var types2 = _typesEmpty;

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
            var types = _typesEmpty;

            if (parameters != null)
            {
                types = parameters.ToArray()!;
            }

            return declaringType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                types,
                null);
        }
                                                    
        protected virtual MemberInfo? ParseMemberInfo(XElement? xElement)
        {
            return ParseValueXElement("MemberType", xElement, ParseGeneric<MemberTypes>) switch
            {
                MemberTypes.Property => ParsePropertyInfo(xElement),
                MemberTypes.Field => ParseFieldInfo(xElement),
                MemberTypes.NestedType or MemberTypes.TypeInfo => ParseType(xElement),
                MemberTypes.Method => ParseMethodInfo(xElement),
                MemberTypes.Constructor => ParseConstructorInfo(xElement),
                _ => throw new Exception("MemberTypes unknown")
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
            return ParseValueXElement("BindingType", xElement, ParseGeneric<MemberBindingType>) switch
            {
                MemberBindingType.Assignment => ParseMemberAssignment(xElement),
                MemberBindingType.ListBinding => ParseMemberListBinding(xElement),
                MemberBindingType.MemberBinding => ParseMemberMemberBinding(xElement),
                _ => throw new Exception("MemberBindingType unknown")
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
                                    
        protected virtual ListInitExpression? ParseListInitExpression(XElement? xElement)
        {
            NewExpression? newExpression;
            IEnumerable<ElementInit?>? elementInits;

            if ((newExpression = ParseFirstNode("NewExpression", xElement, ParseNewExpression)) == null || (elementInits = ParseElements("Initializers", xElement, ParseElementInit)) == null)
            {
                return default;
            }

            return Expression.ListInit(
                newExpression,
                elementInits!);
        }
                                
        protected virtual LambdaExpression? ParseLambdaExpression(XElement? xElement)
        {
            Type? type;
            Expression? expression;
            IEnumerable<ParameterExpression?>? parameterExpressions;

            if ((type = ParseFirstNode("Type", xElement, ParseType)) == null || (expression = ParseFirstNode("Body", xElement, ParseExpression)) == null || (parameterExpressions = ParseElements("Parameters", xElement, ParseParameterExpression)) == null)
            {
                return default;
            }

            return Expression.Lambda(
                type,
                expression, 
                ParseValueXElement("Name", xElement, ParseGeneric<string>), 
                ParseValueXElement("TailCall", xElement, ParseGeneric<bool>),
                parameterExpressions!);
        }
                                                    
        protected virtual MethodCallExpression? ParseMethodCallExpression(XElement? xElement)
        {
            MethodInfo? methodInfo;
            IEnumerable<Expression?>? expressions;

            if ((methodInfo = ParseFirstNode("Method", xElement, ParseMethodInfo)) == null || (expressions = ParseElements("Arguments", xElement, ParseExpression)) == null)
            {
                return default;
            }

            return Expression.Call(
                ParseFirstNode("Object", xElement, ParseExpression),
                methodInfo,
                expressions!);
        }
                            
        protected virtual UnaryExpression? ParseUnaryExpression(XElement? xElement, ExpressionType expressionType)
        {
            Expression? expression;
            Type? type;

            if ((expression = ParseFirstNode("Operand", xElement, ParseExpression)) == null || (type = ParseFirstNode("Type", xElement, ParseType)) == null)
            {
                return default;
            }

            return Expression.MakeUnary(
                expressionType,
                expression,
                type,
                ParseFirstNode("Method", xElement, ParseMethodInfo));
        }
                            
        protected virtual BinaryExpression? ParseBinaryExpression(XElement? xElement, ExpressionType expressionType)
        {
            Expression? expression;
            Expression? expression2;

            if ((expression = ParseFirstNode("Left", xElement, ParseExpression)) == null || (expression2 = ParseFirstNode("Right", xElement, ParseExpression)) == null)
            {
                return default;
            }

            return Expression.MakeBinary(
                expressionType,
                expression,
                expression2,
                ParseValueXElement("IsLiftedToNull", xElement, ParseGeneric<bool>),
                ParseFirstNode("Method", xElement, ParseMethodInfo),
                ParseFirstNode("Conversion", xElement, ParseLambdaExpression));
        }
                                   
        protected virtual ParameterExpression? ParseParameterExpression(XElement? xElement)
        {
            var type = ParseFirstNode("Type", xElement, ParseType);

            if (type == null)
            {
                return default;
            }

            var name = ParseValueXElement("Name", xElement, ParseGeneric<string>);
            var id = name + GetTypeName(type);

            if (ParameterExpressions.TryGetValue(id, out ParameterExpression? parameterExpression))
            {
                return parameterExpression;
            }

            parameterExpression = Expression.Parameter(type, name);
            ParameterExpressions.Add(id, parameterExpression);
                                                    
            return parameterExpression;
        }
                                                    
        protected virtual NewExpression? ParseNewExpression(XElement? xElement)
        {
            ConstructorInfo? constructor;
            IEnumerable<Expression?>? arguments;

            if ((constructor = ParseFirstNode("Constructor", xElement, ParseConstructorInfo)) == null || (arguments = ParseElements("Arguments", xElement, ParseExpression)) == null)
            {
                return default;
            }

            var members = ParseElements("Members", xElement, ParseMemberInfo);

            if (members == null || !members.GetEnumerator().MoveNext())
            {
                return Expression.New(constructor, arguments!);
            }
                                                        
            return Expression.New(constructor, arguments!, members!);
        }
                            
        protected virtual ConstantExpression? ParseConstantExpression(XElement? xElement)
        {
            var type = ParseFirstNode("Type", xElement, ParseType);

            if (type == null)
            {
                return default;
            }

            var value = ParseFirstNode("Value", xElement, Parse);
                            
            value ??= ParseValueXElement("Value", xElement, type, Parse);
                            
            return Expression.Constant(value, type);
        }
                                     
        protected virtual NewArrayExpression? ParseNewArrayExpression(XElement? xElement, ExpressionType expressionType)
        {
            var type = ParseFirstNode("Type", xElement, ParseType);
            Type? elementType;
            IEnumerable<Expression?>? expressions;

            if (type == null || (elementType = type.GetElementType()) == null || (expressions = ParseElements("Expressions", xElement, ParseExpression)) == null)
            {
                return default;
            }
            
            return expressionType switch
            {
                ExpressionType.NewArrayInit => Expression.NewArrayInit(elementType, expressions!),
                ExpressionType.NewArrayBounds => Expression.NewArrayBounds(elementType, expressions!),
                _ => throw new Exception("ExpressionType unknown")
            };
        }
                            
        protected virtual Expression? ParseExpression(XElement? xElement)
        {
            var expressionType = ParseValueXElement("NodeType", xElement, ParseGeneric<ExpressionType>);

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
                _ => throw new Exception("ExpressionType unknown")
            };
        }

        #endregion

        #endregion
        #region IExpressionXmlSerializer

        public XElement? ToXElement(MemberInfo? memberInfo)
        {
            return CreateMemberInfoXElement(memberInfo);
        }

        public XElement? ToXElement(ElementInit? elementInit)
        {
            return CreateElementInitXElement(elementInit);
        }

        public XElement? ToXElement(MemberBinding? memberBinding)
        {
            return CreateMemberBindingXElement(memberBinding);
        }

        public XElement? ToXElement(Expression? expression)
        {
            return CreateExpressionXElement(expression);
        }

        public MemberInfo? ToMemberInfo(XElement? xElement)
        {
            return ParseGeneric<MemberInfo>(xElement);
        }

        public T? ToMemberInfo<T>(XElement? xElement) where T : MemberInfo
        {
            return ParseGeneric<T>(xElement);
        }

        public ElementInit? ToElementInit(XElement? xElement)
        {
            return ParseGeneric<ElementInit>(xElement);
        }

        public MemberBinding? ToMemberBinding(XElement? xElement)
        {
            return ParseGeneric<MemberBinding>(xElement);
        }

        public T? ToMemberBinding<T>(XElement? xElement) where T : MemberBinding
        {
            return ParseGeneric<T>(xElement);
        }

        public Expression? ToExpression(XElement? xElement)
        {
            ParameterExpressions.Clear();

            return ParseGeneric<Expression>(xElement);
        }

        public T? ToExpression<T>(XElement? xElement) where T : Expression
        {
            ParameterExpressions.Clear();

            return ParseGeneric<T>(xElement);
        }

        #endregion
    }
}
