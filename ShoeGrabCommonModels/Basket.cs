using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeGrabCommonModels;
public class Basket
{
    public int Id { get; set; }
    public List<BasketItem> Items { get; set; } = [];
    public int UserId { get; set; }
    public User User { get; set; }
}
