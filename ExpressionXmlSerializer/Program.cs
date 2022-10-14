using System.Linq.Expressions;


namespace ExpressionTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var expressionXmlSerializer = new ExpressionXmlSerializer();


            Expression<Func<Context, int>> expression1 = (c => c.A + c.B);
            var expression1Xml = expressionXmlSerializer.ToXElement(expression1);
            var expression1Result = expressionXmlSerializer.ToExpression(expression1Xml);
            var areEqual = ExpressionComparison.Equal(expression1, expression1Result);






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

        private sealed class Context
        {
            public int A;
            public int B { get; set; }
            public int? C;
            public int[]? Array;
            public int this[string key]
            {
                get
                {
                    switch (key)
                    {
                        case "A": return this.A;
                        case "B": return this.B;
                        case "C": return this.C ?? 0;
                        default: throw new NotImplementedException();
                    }
                }
            }
            public Func<int>? Func;
            public int Method() { return this.A; }
            public int Method(string key) { return this[key]; }
            public object Method3() { return this.A; }
        }
    }
}