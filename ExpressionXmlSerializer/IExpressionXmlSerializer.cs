using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionXmlSerializer
{    
    public interface IExpressionXmlSerializer
    {
        #region Methods
        
        XElement? ToXElement(MemberInfo? memberInfo);
        XElement? ToXElement(ElementInit? elementInit);
        XElement? ToXElement(MemberBinding? memberBinding);
        XElement? ToXElement(Expression? expression);
        MemberInfo? ToMemberInfo(XElement? xElement);
        T? ToMemberInfo<T>(XElement? xElement) where T: MemberInfo;
        ElementInit? ToElementInit(XElement? xElement);
        MemberBinding? ToMemberBinding(XElement? xElement);
        T? ToMemberBinding<T>(XElement? xElement) where T : MemberBinding;
        Expression? ToExpression(XElement? xElement);
    	T? ToExpression<T>(XElement? xElement) where T : Expression;
        
        #endregion
    }
}
