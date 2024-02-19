using System.Linq.Expressions;

namespace Aukcionas.Utils
{
    public static class Extensions
    {
        public static Expression<Func<T, bool>> CombinePredicatesWithAnd<T>(List<Expression<Func<T, bool>>> predicates)
        {
            if (!predicates.Any())
                return x => true;

            var parameter = Expression.Parameter(typeof(T));
            var body = predicates
                .Select(predicate => (Expression)Expression.Invoke(predicate, parameter))
                .Aggregate(Expression.AndAlso);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
