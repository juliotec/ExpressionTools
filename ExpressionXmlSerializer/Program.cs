using System.Linq.Expressions;


namespace ExpressionTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var expressionXmlSerializer = new ExpressionXmlSerializer();


           /* Expression<Func<int, int>> expression1 = c => c + 1;
            var expression1Xml = expressionXmlSerializer.ToXElement(expression1);
            var expression1Result = expressionXmlSerializer.ToExpression<Expression<Func<int, int>>>(expression1Xml);
            var areEqual1 = ExpressionComparison.Equal(expression1, expression1Result);
            var result1 = expression1Result?.Compile()(1);*/


            Expression<Func<bool>> expression2 = () => new object() != null;
            var expression2String = expressionXmlSerializer.ToXElement(expression2);
            var expression2Result = expressionXmlSerializer.ToExpression<Expression<Func<bool>>>(expression2String);
            var areEqual2 = ExpressionComparison.Equal(expression2, expression2Result);
            var result2 = expression2Result?.Compile()();





            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expression23 = c =>
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