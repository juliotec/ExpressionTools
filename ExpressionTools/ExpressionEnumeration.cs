using System.Collections;
using System.Linq.Expressions;

namespace ExpressionTools
{
    public class ExpressionEnumeration : ExpressionVisitor, IEnumerable<Expression>
    {
        #region Fields

        private readonly List<Expression> _expressions = new();

        #endregion
        #region Constructors

        public ExpressionEnumeration(Expression? expression)
        {
            Visit(expression);
        }

        #endregion
        #region ExpressionVisitor

        protected override void Visit(Expression? expression)
        {
            if (expression == null)
            {
                return;
            }

            _expressions.Add(expression);
            base.Visit(expression);
        }

        #endregion
        #region IEnumerable<Expression>

        public IEnumerator<Expression> GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
