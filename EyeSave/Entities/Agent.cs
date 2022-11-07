using System;
using System.Collections.Generic;
using System.Linq;

namespace EyeSave.Entities
{
    public partial class Agent
    {
        public string? _logo;

        public Agent()
        {
            AgentPriorityHistories = new HashSet<AgentPriorityHistory>();
            ProductSales = new HashSet<ProductSale>();
            Shops = new HashSet<Shop>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int AgentTypeId { get; set; }
        public string? Address { get; set; }
        public string Inn { get; set; } = null!;
        public string? Kpp { get; set; }
        public string? DirectorName { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Logo 
        {
            get => (_logo == String.Empty) || (_logo == null)
                ? $"/Resources/picture.png"
                : $"/Resources{_logo}";
            set => _logo = value; 
        }
        public int Priority { get; set; }

        public virtual AgentType AgentType { get; set; } = null!;
        public virtual ICollection<AgentPriorityHistory> AgentPriorityHistories { get; set; }
        public virtual ICollection<ProductSale> ProductSales { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }

        public int SalesPerDecade
        {
            get
            {
                if (ProductSales.Count == 0)
                    return 0;

                else
                {
                    var now = DateTime.Now;
                    var tenYearsAgo = now.AddYears(-10);
                    var sales = 0;

                    foreach (var item in ProductSales.Where(ps => ps.SaleDate > tenYearsAgo))
                    {
                        sales++;
                    }
                    return sales;
                }                
            }
        }

        // [0-10000) -> 0%, [10000-50000) -> 5%, [50000-150000) -> 10%, [150000-500000) -> 20%, [500000-...] -> 25%

        public int Discount
        {
            get
            {
                if (ProductSales.Count == 0)
                    return 0;

                else
                {
                    decimal soldValue = 0;
                    foreach (var item in ProductSales)
                    {
                        soldValue += item.ProductCount * item.Product.MinCostForAgent;
                    }
                    switch (soldValue)
                    {
                        case < 10000: return 0;

                        case < 50000: return 5;

                        case < 150000: return 10;

                        case < 500000: return 20;

                        default: return 25;
                    }
                }
                
            }
        }

        public string TitleEmailPhoneToSearch
        {
            get
            {
                return Title + " " + Email + " " + Phone;
            }
        }
    }
}
