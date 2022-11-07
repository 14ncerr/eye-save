using EyeSave.Core;
using EyeSave.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSave.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private List<Agent> _displayingAgents;
        private List<string> _filtValues;
        private string _sortValue;
        private string _filtValue;
        private string _searchValue;
     
        #endregion

        #region Properties

        public List<Agent> Agents { get; set; }

        public List<Agent> DisplayingAgents
        {
            get => _displayingAgents;
            set
            {
                Set(ref _displayingAgents, value, nameof(DisplayingAgents));
            }
        }

        public List<string> SortValues => new List<string>()
        {
            "Без сортировки", "Наименование(возрю.)", "Наименование(уб.)", "Размер скидки(возр.)", "Размер скидки(уб.)", "Приоритет(возр.)", "Приоритет(уб.)"
        };
    
        public List<string> FiltValues
        {
            get => _filtValues;
            set => _filtValues = value;
        }

        public string SortValue
        {
            get => _sortValue;
            set
            {
                Set(ref _sortValue, value, nameof(SortValue));
                DisplayAgents();
            }
        }

        public string FiltValue
        {
            get => _filtValue;
            set
            {
                Set(ref _filtValue, value, nameof(FiltValue));
                DisplayAgents();
            }
        }

        public string SearchValue
        {
            get => _searchValue;
            set
            {
                Set(ref _searchValue, value, nameof(SearchValue));
                DisplayAgents();
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            FiltValues = new List<string>();
            FiltValues.Add("Все типы");

            FiltValue = FiltValues[0];
            SortValue = SortValues[0];

            using (AppDbContext context = new())
            {
                Agents = context.Agents
                    .Include(a => a.AgentType)
                    .Include(a => a.ProductSales)
                    .ThenInclude(ps => ps.Product)
                    .AsNoTracking()
                    .ToList();
               
                foreach (var item in context.AgentTypes)
                {
                    FiltValues.Add(item.Title);
                }
            }

            _displayingAgents = Agents;
        }

        private void DisplayAgents()
        {
            DisplayingAgents = Sort(Search(Filt(Agents)));
        }

        #region Sorting searching and filtering methods

        private List<Agent> Sort(List<Agent> incList)
        {
            if (SortValue == SortValues[1])
                return incList.OrderBy(a => a.Title).ToList();
            else if (SortValue == SortValues[2])
                return incList.OrderByDescending(a => a.Title).ToList();
            else if (SortValue == SortValues[3])
                return incList.OrderBy(a => a.Discount).ToList();
            else if (SortValue == SortValues[4])
                return incList.OrderByDescending(a => a.Discount).ToList();
            else if (SortValue == SortValues[5])
                return incList.OrderBy(a => a.Priority).ToList();
            else if (SortValue == SortValues[6])
                return incList.OrderByDescending(a => a.Priority).ToList();
            else
                return incList.OrderBy(a => a.Id).ToList();
        }

        private List<Agent> Search(List<Agent> incList)
        {
            if ((SearchValue == String.Empty) || (SearchValue == null))
                return incList;
            else
                return incList.Where(a => a.TitleEmailPhoneToSearch.ToLower().Contains(SearchValue.ToLower())).ToList();
        }

        private List<Agent> Filt(List<Agent> incList)
        {
            if (FiltValue == FiltValues[0])
                return incList;
            else
                return incList.Where(a => a.AgentType.Title == FiltValue).ToList();          
        }

        #endregion

    }
}
