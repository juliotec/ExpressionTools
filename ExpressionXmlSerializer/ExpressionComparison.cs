using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ExpressionXmlSerializer
{
    public class ExpressionComparison : ExpressionVisitor
    {
        #region Fields
        
        private readonly Queue<Expression>? _candidates;
        private Expression? _candidate;

        #endregion
        #region Constructors

        public ExpressionComparison(Expression? a, Expression? b)
        {
            _candidates = new Queue<Expression>(new ExpressionEnumeration(b));

            Visit(a);

            if (_candidates.Count > 0)
            {
                Stop();
            }
        }

        #endregion
        #region Properties

        public bool AreEqual
        {
            get 
            { 
                return _areEqual; 
            }
        }
        private bool _areEqual = true;

        #endregion
        #region Methods

        private Expression? PeekCandidate()
        {            
            if (_candidates == null || _candidates.Count == 0)
            {
                return default;
            }

            return _candidates.Peek();
        }

        private Expression? PopCandidate()
        {
            if (_candidates == null)
            {
                return default;
            }

            return _candidates.Dequeue();
        }

        private bool CheckAreOfSameType(Expression? candidate, Expression? expression)
        {
            if (candidate == null || expression == null || !CheckEqual(expression.NodeType, candidate.NodeType) || !CheckEqual(expression.Type, candidate.Type))
            {
                return false;
            }

            return true;
        }

        private void Stop()
        {
            _areEqual = false;
        }

        private T? CandidateFor<T>() where T : Expression
        {
            if (_candidate == null)
            {
                return default;
            }

            return (T)_candidate;
        }

        private void CompareList<T>(ReadOnlyCollection<T>? collection, ReadOnlyCollection<T>? candidates)
        {
            CompareList(collection, candidates, (item, candidate) => EqualityComparer<T>.Default.Equals(item, candidate));
        }

        private void CompareList<T>(ReadOnlyCollection<T>? collection, ReadOnlyCollection<T>? candidates, Func<T?, T?, bool>? comparer)
        {
            if (collection == null || candidates == null || comparer == null || !CheckAreOfSameSize(collection, candidates))
            {
                return;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (!comparer(collection[i], candidates[i]))
                {
                    Stop();

                    return;
                }
            }
        }

        private bool CheckAreOfSameSize<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidate)
        {
            return CheckEqual(collection.Count, candidate.Count);
        }

        private bool CheckNotNull<T>(T? t) where T : class
        {
            if (t == null)
            {
                Stop();

                return false;
            }

            return true;
        }

        private bool CheckEqual<T>(T t, T candidate)
        {
            if (!EqualityComparer<T>.Default.Equals(t, candidate))
            {
                Stop();

                return false;
            }

            return true;
        }

        #endregion
        #region ExpressionVisitor

        protected override void Visit(Expression? expression)
        {
            if (expression == null || !AreEqual)
            {
                return;
            }

            _candidate = PeekCandidate();
            
            if (!CheckNotNull(_candidate) || !CheckAreOfSameType(_candidate, expression))
            {
                return;
            }

            PopCandidate();

            base.Visit(expression);
        }

        protected override void VisitConstant(ConstantExpression? constant)
        {
            ConstantExpression? candidate;

            if (constant == null || (candidate = CandidateFor<ConstantExpression>()) == null || !CheckEqual(constant.Value, candidate.Value))
            {
                return;
            }
        }

        protected override void VisitMemberAccess(MemberExpression? member)
        {
            MemberExpression? candidate;

            if (member == null || (candidate = CandidateFor<MemberExpression>()) == null || !CheckEqual(member.Member, candidate.Member))
            {
                return;
            }

            base.VisitMemberAccess(member);
        }

        protected override void VisitMethodCall(MethodCallExpression? methodCall)
        {
            MethodCallExpression? candidate;

            if (methodCall == null || (candidate = CandidateFor<MethodCallExpression>()) == null || !CheckEqual(methodCall.Method, candidate.Method))
            {
                return;
            }

            base.VisitMethodCall(methodCall);
        }

        protected override void VisitParameter(ParameterExpression? parameter)
        {
            ParameterExpression? candidate;

            if (parameter == null || (candidate = CandidateFor<ParameterExpression>()) == null || !CheckEqual(parameter.Name, candidate.Name))
            {
                return;
            }
        }

        protected override void VisitTypeIs(TypeBinaryExpression? type)
        {
            TypeBinaryExpression? candidate;

            if (type == null || (candidate = CandidateFor<TypeBinaryExpression>()) == null || !CheckEqual(type.TypeOperand, candidate.TypeOperand))
            {
                return;
            }

            base.VisitTypeIs(type);
        }

        protected override void VisitBinary(BinaryExpression? binary)
        {
            BinaryExpression? candidate;

            if (binary == null || (candidate = CandidateFor<BinaryExpression>()) == null || !CheckEqual(binary.Method, candidate.Method) || !CheckEqual(binary.IsLifted, candidate.IsLifted) || !CheckEqual(binary.IsLiftedToNull, candidate.IsLiftedToNull))
            {
                return;
            }

            base.VisitBinary(binary);
        }

        protected override void VisitUnary(UnaryExpression? unary)
        {
            UnaryExpression? candidate;

            if (unary == null || (candidate = CandidateFor<UnaryExpression>()) == null || !CheckEqual(unary.Method, candidate.Method) || !CheckEqual(unary.IsLifted, candidate.IsLifted) || !CheckEqual(unary.IsLiftedToNull, candidate.IsLiftedToNull))
            {
                return;
            }

            base.VisitUnary(unary);
        }

        protected override void VisitNew(NewExpression? nex)
        {
            NewExpression? candidate;

            if (nex == null || (candidate = CandidateFor<NewExpression>()) == null || !CheckEqual(nex.Constructor, candidate.Constructor))
            {
                return;
            }

            CompareList(nex.Members, candidate.Members);

            base.VisitNew(nex);
        }

        #endregion
    }
}
