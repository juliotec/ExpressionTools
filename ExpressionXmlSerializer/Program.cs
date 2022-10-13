using System.Linq.Expressions;


namespace ExpressionXmlSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var expressionXmlSerializer = new ExpressionXmlSerializer();
            

            Expression<Func<int, int>> expression1 = (c => c + 1);
            var expression1Xml = expressionXmlSerializer.ToXElement(expression1);
            var expression1Result = expressionXmlSerializer.ToExpression(expression1Xml);
            var areEqual = new ExpressionComparison(expression1, expression1Result).AreEqual;

            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expression2 = c =>
                from x in c
                let someConst6547588C372F49698Ec3B242C745Fca0 = 8
                where (x == someConst6547588C372F49698Ec3B242C745Fca0)
                select x;


            

            

            //var w =  expressionXmlSerializer.ToExpression(m);

            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expr = c =>
                from x in c
                let someConst6547588C372F49698Ec3B242C745Fca0 = 8
                where (x == someConst6547588C372F49698Ec3B242C745Fca0)
                select x;

            var w = expressionXmlSerializer.ToXElement(expr);
        }
    }
}