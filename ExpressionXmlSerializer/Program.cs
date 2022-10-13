using System.Linq.Expressions;

namespace ExpressionXmlSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var expression = (Expression<Func<int, int>>)(c => c + 1);


            var s = new ExpressionXmlSerializer();

            var m = s.ToXElement(expression);

            var w =  s.ToExpression(m);
        }
    }
}