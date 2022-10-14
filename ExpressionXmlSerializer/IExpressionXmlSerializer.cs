using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTools
{    
    public interface IExpressionXmlSerializer
    {
        #region Methods
        
        XElement? ToXElement(MemberInfo? memberInfo);
        string? ToString(MemberInfo? memberInfo);
        XElement? ToXElement(ElementInit? elementInit);
        string? ToString(ElementInit? elementInit);
        XElement? ToXElement(MemberBinding? memberBinding);
        string? ToString(MemberBinding? memberBinding);
        XElement? ToXElement(Expression? expression);
        string? ToString(Expression? expression);
        MemberInfo? ToMemberInfo(XElement? xElement);
        MemberInfo? ToMemberInfo(string? val);
        T? ToMemberInfo<T>(XElement? xElement) where T: MemberInfo;
        T? ToMemberInfo<T>(string? val) where T : MemberInfo;
        ElementInit? ToElementInit(XElement? xElement);
        ElementInit? ToElementInit(string? val);
        MemberBinding? ToMemberBinding(XElement? xElement);
        MemberBinding? ToMemberBinding(string? val);
        T? ToMemberBinding<T>(XElement? xElement) where T : MemberBinding;
        T? ToMemberBinding<T>(string? val) where T : MemberBinding;
        Expression? ToExpression(XElement? xElement);
        Expression? ToExpression(string? val);
        T? ToExpression<T>(XElement? xElement) where T : Expression;
        T? ToExpression<T>(string? val) where T : Expression;

        #endregion
    }
}
