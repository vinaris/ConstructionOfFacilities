using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionOfFacilities.Models
{
    public class BuildingObject
    {
        [Display(Name = "№")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Предмет")]
        public string Name { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Ответственный")]
        public string Responsible { get; set; }
        [Display(Name = "Сумма")]
        public decimal? Amount { get; set; }
        [Display(Name = "Цена контракта")]
        public decimal? PriceOfContract { get; set; }
        [Display(Name = "Исполнитель")]
        public string Executor { get; set; }
        [Display(Name = "Дата размещения")]
        public DateTime? PublishDate { get; set; }
        [Display(Name = "Дата окончания подачи заявок, аукциона")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Дата рассмотрения заявок (кт, кн)")]
        public DateTime? BidsDate { get; set; }
        [Display(Name = "Дата проведения аукциона")]
        public DateTime? AuctionDate { get; set; }
        [Display(Name = "Заявки")]
        public string Bids { get; set; }
        [Display(Name = "Контрагент")]
        public string Partner { get; set; }
        [Display(Name = "Сумма контракта")]
        public decimal? AmountOfContract { get; set; }
        [Display(Name = "Дата контракта")]
        public DateTime? DateOfContract { get; set; }
        [Display(Name = "Этап закупки")]
        public string Stage { get; set; }
        [Display(Name = "Этап исполнения контракта")]
        public string StageOfContract { get; set; }
        [Display(Name = "Источник финансирования")]
        public string SourceOfFinancing { get; set; }
        [Display(Name = "Комментарии")]
        public string Comments { get; set; }
        [Display(Name = "Дата последнего изменения")]
        public DateTime LastUpDateTime { get; set; }
        [Display(Name = "Тип таблицы")]
        public int TypeOfTable { get; set; } //Виды таблиц (1 - Жилые здания; 2 - Коттеджи, дачи; 3 - парки, скверы) 
    }
}
