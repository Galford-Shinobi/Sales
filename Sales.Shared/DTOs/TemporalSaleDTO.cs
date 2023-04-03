using System.ComponentModel.DataAnnotations;

namespace Sales.Shared.DTOs
{
    public class TemporalSaleDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public float Quantity { get; set; } = 1;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]

        public string Remarks { get; set; } = string.Empty;
    }
}
