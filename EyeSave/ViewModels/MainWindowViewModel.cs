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
        private List<string>? _filtValues;
        private string? _sortValue;
        private string? _filtValue;
        private string? _searchValue;

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

            _filtValue = FiltValues[0];
            _sortValue = SortValues[0];

            using (AppDbContext context = new())
            {
                Agents = context.Agents
                    .Include(a => a.AgentType)
                    .Include(a => a.ProductSales)
                    .ThenInclude(ps => ps.Product)
                    .OrderBy(a => a.Id)
                    .ToList();

                FiltValues.AddRange(context.AgentTypes.Select(at => at.Title));
            }

            _pages = GetPages(Agents.Count);
            _selectedPage = _pages[0];
            DisplayAgents();
        }

        private void DisplayAgents()
        {
            var list = Sort(Search(Filt(Agents)));

            Pages = GetPages(list.Count);

            var pageNum = SelectedPage == null
                ? 1
                : SelectedPage.pageNum;

            DisplayingAgents = Paging(list, pageNum, pageSize);

            SelectedPage ??= Pages[0];
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

        #region Paging !!!

        public record Page(int pageNum);
        private const int pageSize = 10;
        private List<Page> _pages;
        private Page _selectedPage;

        public List<Page> Pages
        {
            get => _pages;
            set => Set(ref _pages, value, nameof(Pages));
        }

        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (Set(ref _selectedPage, value, nameof(SelectedPage)))
                    DisplayAgents();
            }
        }

        private List<Page> GetPages(int itemCount)
        {
            double pageCount = Math.Ceiling((double)itemCount / pageSize);
            var list = new List<Page>((int)pageCount);
            list.Add(new Page(1));
            for (int i = 1; i < pageCount; i++)
            {
                list.Add(new Page (i + 1));
            }
            return list;
        }

        private List<Agent> Paging(List<Agent> agents, int pageNum, int pageSize)
        {
            if (pageNum > 0)
                agents = agents.Skip((pageNum - 1) * pageSize).ToList();

            return agents.Take(pageSize).ToList();          
        }

        #endregion

    }
}
