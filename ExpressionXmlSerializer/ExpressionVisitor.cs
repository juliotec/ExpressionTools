using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ExpressionXmlSerializer
{
    public abstract class ExpressionVisitor
    {
        #region Methods

        protected virtual void Visit(Expression? expression)
        {
            if (expression == null)
            {
                return;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    VisitUnary((UnaryExpression)expression);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Power:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    VisitBinary((BinaryExpression)expression);
                    break;
                case ExpressionType.TypeIs:
                    VisitTypeIs((TypeBinaryExpression)expression);
                    break;
                case ExpressionType.Conditional:
                    VisitConditional((ConditionalExpression)expression);
                    break;
                case ExpressionType.Constant:
                    VisitConstant((ConstantExpression)expression);
                    break;
                case ExpressionType.Parameter:
                    VisitParameter((ParameterExpression)expression);
                    break;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression)expression);
                    break;
                case ExpressionType.Call:
                    VisitMethodCall((MethodCallExpression)expression);
                    break;
                case ExpressionType.Lambda:
                    VisitLambda((LambdaExpression)expression);
                    break;
                case ExpressionType.New:
                    VisitNew((NewExpression)expression);
                    break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    VisitNewArray((NewArrayExpression)expression);
                    break;
                case ExpressionType.Invoke:
                    VisitInvocation((InvocationExpression)expression);
                    break;
                case ExpressionType.MemberInit:
                    VisitMemberInit((MemberInitExpression)expression);
                    break;
                case ExpressionType.ListInit:
                    VisitListInit((ListInitExpression)expression);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
            }
        }

        protected virtual void VisitBinding(MemberBinding? binding)
        {
            if (binding == null)
            {
                return;
            }

            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    VisitMemberAssignment((MemberAssignment)binding);
                    break;
                case MemberBindingType.MemberBinding:
                    VisitMemberMemberBinding((MemberMemberBinding)binding);
                    break;
                case MemberBindingType.ListBinding:
                    VisitMemberListBinding((MemberListBinding)binding);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual void VisitElementInitializer(ElementInit? initializer)
        {
            if (initializer == null)
            {
                return;
            }

            VisitExpressionList(initializer.Arguments!);
        }

        protected virtual void VisitUnary(UnaryExpression? unary)
        {
            if (unary == null)
            {
                return;
            }

            Visit(unary.Operand);
        }

        protected virtual void VisitBinary(BinaryExpression? binary)
        {
            if (binary == null)
            {
                return;
            }

            Visit(binary.Left);
            Visit(binary.Right);
            Visit(binary.Conversion);
        }

        protected virtual void VisitTypeIs(TypeBinaryExpression? type)
        {
            if (type == null)
            {
                return;
            }

            Visit(type.Expression);
        }

        protected virtual void VisitConstant(ConstantExpression? constant)
        {
        }

        protected virtual void VisitConditional(ConditionalExpression? conditional)
        {
            if (conditional == null)
            {
                return;
            }

            Visit(conditional.Test);
            Visit(conditional.IfTrue);
            Visit(conditional.IfFalse);
        }

        protected virtual void VisitParameter(ParameterExpression? parameter)
        {
        }

        protected virtual void VisitMemberAccess(MemberExpression? member)
        {
            if (member == null)
            {
                return;
            }

            Visit(member.Expression);
        }

        protected virtual void VisitMethodCall(MethodCallExpression? methodCall)
        {
            if (methodCall == null)
            {
                return;
            }

            Visit(methodCall.Object);
            VisitExpressionList(methodCall.Arguments!);
        }

        protected virtual void VisitList<T>(ReadOnlyCollection<T?>? list, Action<T>? visitor)
        {
            if (list == null || visitor == null)
            {
                return;
            }

            for (var i = 0; i < list.Count; i++)
            {
                visitor(list[i]!);
            }
        }

        protected virtual void VisitExpressionList<TExp>(ReadOnlyCollection<TExp?>? list) where TExp : Expression
        {
            VisitList(list, Visit);
        }

        protected virtual void VisitMemberAssignment(MemberAssignment? assignment)
        {
            if (assignment == null)
            {
                return;
            }

            Visit(assignment.Expression);
        }

        protected virtual void VisitMemberMemberBinding(MemberMemberBinding? binding)
        {
            if (binding == null)
            {
                return;
            }

            VisitBindingList(binding.Bindings!);
        }

        protected virtual void VisitMemberListBinding(MemberListBinding? binding)
        {   
            if (binding == null)
            {
                return;
            }

            VisitElementInitializerList(binding.Initializers!);
        }

        protected virtual void VisitBindingList<TBinding>(ReadOnlyCollection<TBinding?>? list) where TBinding : MemberBinding
        {
            if (list == null)
            {
                return;
            }

            VisitList(list, VisitBinding);
        }

        protected virtual void VisitElementInitializerList(ReadOnlyCollection<ElementInit?>? list)
        {
            if (list == null)
            {
                return;
            }

            VisitList(list, VisitElementInitializer);
        }

        protected virtual void VisitLambda(LambdaExpression? lambda)
        {
            if (lambda == null)
            {
                return;
            }

            Visit(lambda.Body);
        }

        protected virtual void VisitNew(NewExpression? nex)
        {
            if (nex == null)
            {
                return;
            }

            VisitExpressionList(nex.Arguments!);
        }

        protected virtual void VisitMemberInit(MemberInitExpression? init)
        {
            if (init == null)
            {
                return;
            }

            VisitNew(init.NewExpression);
            VisitBindingList(init.Bindings!);
        }

        protected virtual void VisitListInit(ListInitExpression? init)
        {
            if (init == null)
            {
                return;
            }

            VisitNew(init.NewExpression);
            VisitElementInitializerList(init.Initializers!);
        }

        protected virtual void VisitNewArray(NewArrayExpression? newArray)
        {
            if (newArray == null)
            {
                return;
            }

            VisitExpressionList(newArray.Expressions!);
        }

        protected virtual void VisitInvocation(InvocationExpression? invocation)
        {
            if (invocation == null)
            {
                return;
            }

            VisitExpressionList(invocation.Arguments!);
            Visit(invocation.Expression);
        }

        #endregion
    }
}
