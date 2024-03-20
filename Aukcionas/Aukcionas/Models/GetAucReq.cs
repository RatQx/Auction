using Aukcionas.Utils;
using System.Linq.Expressions;

namespace Aukcionas.Models;

public class GetAucReq()
{
    public string? name { get; set; }
    public Expression<Func<Auction, bool>> GetPredicates()
    {
        var predicates = new List<Expression<Func<Auction, bool>>>();

        if (!string.IsNullOrEmpty(name))
        {
            predicates.Add(x => x.name.Contains(name));
        }
        predicates.Add(x => x.auction_end_time.AddHours(1) >= DateTime.UtcNow);

        return Extensions.CombinePredicatesWithAnd(predicates);

    }

}


