using System.IO;
using System.Linq.Expressions;


namespace ExpressionTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var expressionXmlSerializer = new ExpressionXmlSerializer();

            Expression<Func<int, int, int>> expression1 = (c, b) => c + b;
            var expression1Xml = expressionXmlSerializer.ToXElement(expression1);
            var expression1Result = expressionXmlSerializer.ToExpression<Expression<Func<int, int, int>>>(expression1Xml);
            var areEqual1 = ExpressionComparison.Compare(expression1, expression1Result);
            var result1 = expression1Result?.Compile()(1, 2);

            Expression<Func<bool>> expression2 = () => ExpressionComparison.Compare(expression1, expression1Result) == new ExpressionComparison(expression1, expression1Result).AreEqual;
            var expression2String = expressionXmlSerializer.ToXElement(expression2);
            var expression2Result = expressionXmlSerializer.ToExpression<Expression<Func<bool>>>(expression2String);
            var areEqual2 = ExpressionComparison.Compare(expression2, expression2Result);
            var result2 = expression2Result?.Compile()();

            var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expression3 = c =>
                from x in c
                let someConst6547588C372F49698Ec3B242C745Fca0 = 8
                where (x == someConst6547588C372F49698Ec3B242C745Fca0)
                select x;
            var expression3String = expressionXmlSerializer.ToString(expression3);
            var expression3Result = expressionXmlSerializer.ToExpression<Expression<Func<IEnumerable<int>, IEnumerable<int>>>>(expression3String);
            var areEqual3 = new ExpressionComparison(expression3, expression3Result).AreEqual;
            var result3 = expression3Result?.Compile()(list).ToArray();

            Expression<Action> expression4 = () => list.Add(10);
            var expression4String = expressionXmlSerializer.ToString(expression4);
            var expression4Result = expressionXmlSerializer.ToExpression<Expression<Action>>(expression4String);
            var areEqual4 = ExpressionComparison.Compare(expression4, expression4Result);
            expression4Result?.Compile().Invoke();

            Expression<Func<IEnumerable<object>>> expression5 = () => from emp in Employee.GetAllEmployees()
                    join add in Address.GetAddress()
                    on emp.AddressId equals add.ID
                    into EmployeeAddressGroup
                    from address in EmployeeAddressGroup.DefaultIfEmpty()
                    select new { emp, address };
            var expression5String = expressionXmlSerializer.ToString(expression5);
            var expression5Result = expressionXmlSerializer.ToExpression<Expression<Func<IEnumerable<object>>>>(expression5String);
            var areEqual5 = ExpressionComparison.Compare(expression5, expression5Result);
            var result5 = expression5Result?.Compile()().ToArray();


            //Expression<Func<int, int>> expression6 = (x) => x + 1;
            //var expressionString = expressionXmlSerializer.ToString(expression6);
            var expression6Result  = expressionXmlSerializer.ToExpression<Expression<Func<int, int>>>(File.ReadAllText(@"add.txt"));
            var result6 = expression6Result?.Compile()(1);
        }

        public class Employee
        {
            public int ID { get; set; }
            public string? Name { get; set; }
            public int AddressId { get; set; }
            public static List<Employee> GetAllEmployees()
            {
                return new List<Employee>()
                {
                    new Employee { ID = 1, Name = "Preety", AddressId = 1},
                    new Employee { ID = 2, Name = "Priyanka", AddressId =2},
                    new Employee { ID = 3, Name = "Anurag", AddressId = 0},
                    new Employee { ID = 4, Name = "Pranaya", AddressId = 0},
                    new Employee { ID = 5, Name = "Hina", AddressId = 5},
                    new Employee { ID = 6, Name = "Sambit", AddressId = 6}
                };
            }
        }
        public class Address
        {
            public int ID { get; set; }
            public string? AddressLine { get; set; }
            public static List<Address> GetAddress()
            {
                return new List<Address>()
                {
                    new Address { ID = 1, AddressLine = "AddressLine1"},
                    new Address { ID = 2, AddressLine = "AddressLine2"},
                    new Address { ID = 5, AddressLine = "AddressLine5"},
                    new Address { ID = 6, AddressLine = "AddressLine6"},
                };
            }
        }
    }
}